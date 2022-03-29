using Dapper;
using Nirast.Pcms.Api.Sdk.Entities;
using Nirast.Pcms.Api.Sdk.Infrastructure;
using Nirast.Pcms.Api.Sdk.Logger;
using Nirast.Pcms.Api.Sdk.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Data.Repositories
{
    public class InvoiceRepository : GenericRepository<UserPaymentInvoiceModel>, IInvoiceRepository
    {

        IConnectionFactory _connectionFactory;
        IPCMSLogger _logger;
        IDbConnection _dbConnection;

        public InvoiceRepository(IConnectionFactory connectionFactory, IPCMSLogger logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _dbConnection = connectionFactory.GetConnection();
        }

        /// <summary>
        /// interface method to implement add user payment transaction details
        /// </summary>
        /// <param name="paymentTransactionModel"></param>
        /// <returns></returns>
        public async Task<UserPaymentInvoiceModel> AddPaymentTransactionDetails(UserPaymentTransactionModel paymentTransactionModel)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var param = new DynamicParameters();
                var spTransactionDetails = "SpInsertUserPaymentTransaction";
                param.Add("@InvoiceNumber", paymentTransactionModel.InvoiceNumber);
                param.Add("@Date", paymentTransactionModel.Date);
                param.Add("@Description", paymentTransactionModel.Description);
                param.Add("@Method", paymentTransactionModel.Method);
                param.Add("@Status", paymentTransactionModel.Status);
                param.Add("@Message", paymentTransactionModel.Message);
                param.Add("@TransactionNumber", paymentTransactionModel.TransactionNumber);
                param.Add("@Amount", paymentTransactionModel.Amount);
                param.Add("@TransactionDetails", paymentTransactionModel.TransactionDetails);
                int resultDetails = SqlMapper.ExecuteAsync(_dbConnection, spTransactionDetails, param, commandType: CommandType.StoredProcedure).Result;
                if (resultDetails > 0)
                {
                    var invoiceSp = "SpGetUserInvoiceDetails";
                    var userparam = new DynamicParameters();
                    userparam.Add("@InvoiceNumber", paymentTransactionModel.InvoiceNumber);
                    var result = await SqlMapper.QueryFirstOrDefaultAsync<UserPaymentInvoiceModel>(_dbConnection, invoiceSp, userparam, commandType: CommandType.StoredProcedure);


                    return await Task.FromResult(result);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occure while inserting transaction details  ");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<IEnumerable<AdminBookingNotification>> GetAdminNotificationByLocation(LocationSearchInputs inputs)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CountryId", inputs.CountryId);
                param.Add("@StateId", inputs.StateId);
                param.Add("@CityId", inputs.CityId);
                _connectionFactory.OpenConnection();
                _connectionFactory.OpenConnection();
                var Details = "SpGetAdminNotificationByCaretakerLocation";
                var result = await SqlMapper.QueryAsync<AdminBookingNotification>(_dbConnection, Details, param, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get booking details.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// To get booking listbyLocation
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AdminBookingList>> GetAdminBookingListByLocation(LocationSearchInputs inputs)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CountryId", inputs.CountryId);
                param.Add("@StateId", inputs.StateId);
                param.Add("@CityId", inputs.CityId);
                _connectionFactory.OpenConnection();
                var query = "SpGetAdminBookingListByLocation";
                var result = await SqlMapper.QueryAsync<AdminBookingList>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving payment details using advanced search");

                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// interface method to implement get user payment details
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        public async Task<UserPaymentInvoiceModel> GetUserPaymentInvoiceDetails(int invoiceNumber)
        {
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                List<InvoicePaymentModel> paymentList = new List<InvoicePaymentModel>();
                List<UserPaymentTransactionModel> transactionList = new List<UserPaymentTransactionModel>();

                var invoiceSp = "SpGetUserInvoiceDetails";
                param.Add("@InvoiceNumber", invoiceNumber);
                var result = SqlMapper.Query<UserPaymentInvoiceModel>(_dbConnection, invoiceSp, param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                int userId = result.UserId;


                var paymentSp = "SpGetUserInvoicePayments";
                param = new DynamicParameters();
                param.Add("@UserId", userId);
                var paymentResult = SqlMapper.QueryMultiple(_dbConnection, paymentSp, param, commandType: CommandType.StoredProcedure);
                paymentList = paymentResult.Read<InvoicePaymentModel>().ToList();
                transactionList = paymentResult.Read<UserPaymentTransactionModel>().ToList();

                result.AllPayments = paymentList;
                result.PaymentHistory = transactionList;
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving invoice details");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// method to get payment details
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<PaymentHistory>> GetPaymentDetails()
        {
            try
            {
                _connectionFactory.OpenConnection();
                var Details = "SpGetPaymentDetails";
                var result = SqlMapper.QueryAsync<PaymentHistory>(_dbConnection, Details, commandType: CommandType.StoredProcedure).Result.ToList();
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving office staff details");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// To get payment details by advanced searching
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PaymentHistory>> SearchPaymentDetails(PaymentAdvancedSearch inputs)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpPaymentDetailsSearch";
                var param = new DynamicParameters();
                param.Add("@CareTakerId", (inputs.CareTakerId == 0) ? null : inputs.CareTakerId);
                param.Add("@CategoryId", (inputs.Category == 0) ? null : inputs.Category);
                param.Add("@ServicetypeId", (inputs.Service == 0) ? null : inputs.Service);
                param.Add("@FromDate", inputs.FromDate == DateTime.MinValue ? null : inputs.FromDate);
                param.Add("@ToDate", inputs.ToDate == DateTime.MinValue ? null : inputs.ToDate);
                param.Add("@Year", (inputs.Year == 0) ? null : inputs.Year);
                param.Add("@Month", (inputs.Month == 0) ? null : inputs.Month);

                var result = await SqlMapper.QueryAsync<PaymentHistory>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving payment details using advanced search");

                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<IEnumerable<InvoiceReportData>> SearchClientInvoiceReport(PaymentAdvancedSearch inputs)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "Report_CaregiverPayrollSummary_Group";
                var param = new DynamicParameters();
                param.Add("@ClientId", (inputs.ClientId == 0) ? null : inputs.ClientId);
                param.Add("@CareTakerId", (inputs.CareTakerId == 0) ? null : inputs.CareTakerId);
                param.Add("@CategoryId", (inputs.Category == 0) ? null : inputs.Category);
                param.Add("@FromDate", inputs.FromDate == DateTime.MinValue ? null : inputs.FromDate);
                param.Add("@ToDate", inputs.ToDate == DateTime.MinValue ? null : inputs.ToDate);
                //param.Add("@Year", (inputs.Year == 0) ? null : inputs.Year);
                //param.Add("@Month", (inputs.Month == 0) ? null : inputs.Month);
                param.Add("@IsOrientation", inputs.IsOrientation);

                var result = await SqlMapper.QueryAsync<InvoiceReportData>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving payment details using advanced search");

                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public async Task<IEnumerable<InvoiceReportData>> SearchClientInvoiceReportSummary(PaymentAdvancedSearch inputs)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "Report_CaregiverPayrollSummary_Group";
                var param = new DynamicParameters();
                param.Add("@ClientId", (inputs.ClientId == 0) ? null : inputs.ClientId);
                param.Add("@CareTakerId", (inputs.CareTakerId == 0) ? null : inputs.CareTakerId);
                param.Add("@CategoryId", (inputs.Category == 0) ? null : inputs.Category);
                param.Add("@FromDate", inputs.FromDate == DateTime.MinValue ? null : inputs.FromDate);
                param.Add("@ToDate", inputs.ToDate == DateTime.MinValue ? null : inputs.ToDate);
                //param.Add("@Year", (inputs.Year == 0) ? null : inputs.Year);
                //param.Add("@Month", (inputs.Month == 0) ? null : inputs.Month);
                param.Add("@IsOrientation", inputs.IsOrientation);

                var result = await SqlMapper.QueryAsync<InvoiceReportData>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving payment details using advanced search");

                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<IEnumerable<ScheduledData>> GetClientScheduledDetails(PaymentAdvancedSearch inputs)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpGetInvoiceReport";
                var param = new DynamicParameters();
                param.Add("@ClientId", (inputs.ClientId == 0) ? null : inputs.ClientId);
                param.Add("@CareTakerId", (inputs.CareTakerId == 0) ? null : inputs.CareTakerId);
                param.Add("@CategoryId", (inputs.Category == 0) ? null : inputs.Category);
                param.Add("@FromDate", inputs.FromDate == DateTime.MinValue ? null : inputs.FromDate);
                param.Add("@ToDate", inputs.ToDate == DateTime.MinValue ? null : inputs.ToDate);
                param.Add("@Year", (inputs.Year == 0) ? null : inputs.Year);
                param.Add("@Month", (inputs.Month == 0) ? null : inputs.Month);
                param.Add("@IsOrientation", inputs.IsOrientation);

                var result = await SqlMapper.QueryAsync<ScheduledData>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving payment details using advanced search");

                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }



        public async Task<IEnumerable<ScheduledData>> GetClientScheduledDetailsByBranchWise(PaymentAdvancedSearch inputs)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpGetInvoiceReportByBranchWise";
                var param = new DynamicParameters();
                param.Add("@ClientId", (inputs.ClientId == 0) ? null : inputs.ClientId);
                param.Add("@CareTakerId", (inputs.CareTakerId == 0) ? null : inputs.CareTakerId);
                param.Add("@CategoryId", (inputs.Category == 0) ? null : inputs.Category);
                param.Add("@FromDate", inputs.FromDate == DateTime.MinValue ? null : inputs.FromDate);
                param.Add("@ToDate", inputs.ToDate == DateTime.MinValue ? null : inputs.ToDate);
                param.Add("@Year", (inputs.Year == 0) ? null : inputs.Year);
                param.Add("@Month", (inputs.Month == 0) ? null : inputs.Month);
                param.Add("@IsOrientation", inputs.IsOrientation);
                param.Add("@BranchId", (inputs.BranchId == 0) ? null : inputs.BranchId);

                var result = await SqlMapper.QueryAsync<ScheduledData>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving payment details using advanced search");

                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public async Task<IEnumerable<ScheduledData>> GetInvoiceGenerationDetails(PaymentAdvancedSearch inputs)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpGetInvoiceDetailsGeneration";
                var param = new DynamicParameters();
                param.Add("@ClientId", (inputs.ClientId == 0) ? null : inputs.ClientId);
                param.Add("@CategoryId", inputs.Category);
                param.Add("@FromDate", inputs.FromDate == DateTime.MinValue ? null : inputs.FromDate);
                param.Add("@ToDate", inputs.ToDate == DateTime.MinValue ? null : inputs.ToDate);
                param.Add("@Year", (inputs.Year == 0) ? null : inputs.Year);
                param.Add("@Month", (inputs.Month == 0) ? null : inputs.Month);

                var result = await SqlMapper.QueryAsync<ScheduledData>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving payment details using advanced search");

                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public async Task<IEnumerable<UserBookingInvoiceReport>> GetUserInvoiceGenerationDetails(BookingHistorySearch inputs)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpGetUserInvoiceDetailsGeneration";
                var param = new DynamicParameters();
                param.Add("@PublicUserId", (inputs.PublicUserId == 0) ? null : inputs.PublicUserId);
                param.Add("@ServiceId", inputs.ServiceId);
                param.Add("@FromDate", inputs.FromDate == DateTime.MinValue ? null : inputs.FromDate);
                param.Add("@ToDate", inputs.ToDate == DateTime.MinValue ? null : inputs.ToDate);
                param.Add("@Year", (inputs.Year == 0) ? null : inputs.Year);
                param.Add("@Month", (inputs.Month == 0) ? null : inputs.Month);

                var result = await SqlMapper.QueryAsync<UserBookingInvoiceReport>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving payment details using advanced search");

                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        

        public async Task<IEnumerable<ScheduledData>> SearchClientScheduleReoprt(PaymentAdvancedSearch inputs)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpCaretakerIvoiceSearch";
                var param = new DynamicParameters();
                param.Add("@CareTakerId", (inputs.CareTakerId == 0) ? null : inputs.CareTakerId);
                param.Add("@CategoryId", (inputs.Category == 0) ? null : inputs.Category);
                param.Add("@FromDate", inputs.FromDate == DateTime.MinValue ? null : inputs.FromDate);
                param.Add("@ToDate", inputs.ToDate == DateTime.MinValue ? null : inputs.ToDate);
                param.Add("@Year", (inputs.Year == 0) ? null : inputs.Year);
                param.Add("@Month", (inputs.Month == 0) ? null : inputs.Month);
                param.Add("@Workmode", (inputs.Service == 0) ? null : inputs.Service);
                param.Add("@ClientId", (inputs.ClientId == 0) ? null : inputs.ClientId);

                var result = await SqlMapper.QueryAsync<ScheduledData>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving payment details using advanced search");

                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public async Task<IEnumerable<ScheduledData>> SearchCaretakerPayHoursReoprt(PaymentAdvancedSearch inputs)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpGetPayHoursSummary";
                var param = new DynamicParameters();
                param.Add("@CareTakerId", (inputs.CareTakerId == 0) ? null : inputs.CareTakerId);
                param.Add("@CategoryId", (inputs.Category == 0) ? null : inputs.Category);
                param.Add("@FromDate", inputs.FromDate == DateTime.MinValue ? null : inputs.FromDate);
                param.Add("@ToDate", inputs.ToDate == DateTime.MinValue ? null : inputs.ToDate);
                param.Add("@Year", (inputs.Year == 0) ? null : inputs.Year);
                param.Add("@Month", (inputs.Month == 0) ? null : inputs.Month);
                param.Add("@ClientId", (inputs.ClientId == 0) ? null : inputs.ClientId);

                var result = await SqlMapper.QueryAsync<ScheduledData>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving SpGetPayHoursSummary");

                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        /// <summary>
        /// To get user payment reports by advanced searching
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PaymentReportDetails>> SearchUserPaymentReport(PaymentReport inputs)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpGetPaymentReport";
                var param = new DynamicParameters();
                param.Add("@ServiceId", (inputs.ServiceType == 0) ? null : inputs.ServiceType);
                param.Add("@FromDate", inputs.FromDate == DateTime.MinValue ? null : inputs.FromDate);
                param.Add("@ToDate", inputs.ToDate == DateTime.MinValue ? null : inputs.ToDate);
                param.Add("@Year", (inputs.Year == 0) ? null : inputs.Year);
                param.Add("@Month", (inputs.Month == 0) ? null : inputs.Month);
                param.Add("@SearchDateType", inputs.SearchType);
                var result = await SqlMapper.QueryAsync<PaymentReportDetails>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving payment details using advanced search");

                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public async Task<IEnumerable<AdminBookingNotification>> GetAdminNotification()
        {
            try
            {
                _connectionFactory.OpenConnection();
                var param = new DynamicParameters();
                var Details = "SpGetAdminNotification";
                var result = await SqlMapper.QueryAsync<AdminBookingNotification>(_dbConnection, Details, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get booking details.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// To get booking list
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AdminBookingList>> GetAdminBookingList()
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpGetAdminBookingList";
                var result = await SqlMapper.QueryAsync<AdminBookingList>(_dbConnection, query, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving payment details using advanced search");

                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// To get scheduling list
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AdminSchedulingList>> GetAdminSchedulingList()
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpGetAdminSchedulingList";
                var result = await SqlMapper.QueryAsync<AdminSchedulingList>(_dbConnection, query, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving payment details using advanced search");

                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// To get booking details by id
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public async Task<AdminBookingDetails> GetBookingDetailsById(int bookingId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpGetBookingDetailsById";
                var param = new DynamicParameters();
                param.Add("@BookingId", bookingId);
                var result = await SqlMapper.QueryFirstOrDefaultAsync<AdminBookingDetails>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving payment details using advanced search");

                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// To get payment details by advanced searching
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public async Task<AdminSchedulingDetails> GetSchedulingDetailsById(int schedulingId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpGetSchedulingDetailsById";
                var param = new DynamicParameters();
                param.Add("@SchedulingId", schedulingId);
                var result = await SqlMapper.QueryFirstOrDefaultAsync<AdminSchedulingDetails>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving payment details using advanced search");

                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// To generate invoice
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public async Task<int> GenerateInvoice(InvoiceMail invoiceMail)
        {
            try
            {
                _connectionFactory.OpenConnection();


                BookingHistoryDetail historyDetail = new BookingHistoryDetail();
                var query = "SpSelectBookingHistoryDetailforInvoice";
                var param = new DynamicParameters();
                param.Add("@BookingId", invoiceMail.BookingId);

                historyDetail = await SqlMapper.QueryFirstOrDefaultAsync<BookingHistoryDetail>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);

                if (historyDetail != null)
                {
                    var invquery = "SpGenerateInvoice";
                    param = new DynamicParameters();
                    param.Add("@BookingId", invoiceMail.BookingId);
                    param.Add("@Amount", historyDetail.TotalDisplayAmount);
                    param.Add("@InvoiceNumber", historyDetail.InvoiceNumber);
                    param.Add("@InvoicePrefix", historyDetail.InvoicePrefix);

                    var result = await SqlMapper.QueryFirstOrDefaultAsync<int>(_dbConnection, invquery, param, commandType: CommandType.StoredProcedure);
                    return await Task.FromResult(result);
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving payment details using advanced search");

                return 0;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        /// <summary>
        /// To save payment invoice path
        public async Task<int> AddPaymentInvoiceDetails(InvoiceSearchInpts invoiceMail)
        {
            try
            { 
                _connectionFactory.OpenConnection();
                var invquery = "SpPaymentInvoiceDetails";
                var param = new DynamicParameters();
                param.Add("@InvoiceSearchInputId", invoiceMail.InvoiceSearchInputId);
                param.Add("@BookingId", invoiceMail.BookingId);
                param.Add("@Amount", invoiceMail.TotalPayingAmount);
                param.Add("@InvoiceNumber", invoiceMail.InvoiceNumber);
                param.Add("@InvoicePrefix", invoiceMail.InvoicePrefix);
                param.Add("@InvoiceDate", invoiceMail.InvoiceDate);
                param.Add("@InvoicePath", invoiceMail.PdfFilePath);
                
                param.Add("@StartDate", invoiceMail.StartDate);
                param.Add("@EndDate", invoiceMail.EndDate);
                param.Add("@Mode", invoiceMail.Mode);
                param.Add("@Year", invoiceMail.Year);
                param.Add("@Month", invoiceMail.Month);
                param.Add("@InvoiceSearchInputId_Out", DbType.Int32, direction: ParameterDirection.Output);

                var result = await SqlMapper.QueryFirstOrDefaultAsync<int>(_dbConnection, invquery, param, commandType: CommandType.StoredProcedure);
                var invoiceId = param.Get<int>("@InvoiceSearchInputId_Out");
                return await Task.FromResult(invoiceId);

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving payment details using advanced search");

                return 0;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public async Task<string> SavePublicUserPaymentInvoice(PublicUserPaymentInvoiceInfo invoiceData)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var sp = "SpInsertUpdatePublicUserInvoice";
                var param = new DynamicParameters();
                param.Add("@InvoiceNo", invoiceData.InvoiceNumber);
                param.Add("@InvoicePath", invoiceData.InvoicePath);
                var result = _dbConnection.Query<string>(sp, param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while saving public user invoice info");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
    }
}
