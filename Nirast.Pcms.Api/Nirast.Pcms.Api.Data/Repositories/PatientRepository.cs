using Dapper;
using Nirast.Pcms.Api.Sdk.Entities;
using Nirast.Pcms.Api.Sdk.Infrastructure;
using Nirast.Pcms.Api.Sdk.Logger;
using Nirast.Pcms.Api.Sdk.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Nirast.Pcms.Api.Sdk.Entities.Enums;

namespace Nirast.Pcms.Api.Data.Repositories
{
    public class PatientRepository : GenericRepository<CaretakerBookingModel>, IPatientRepository
    {
        IConnectionFactory _connectionFactory;
        IPCMSLogger _logger;
        IDbConnection _dbConnection;
        INotificationService _notificationService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionFactory"></param>
        public PatientRepository(IConnectionFactory connectionFactory, IPCMSLogger logger, INotificationService notificationService)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _dbConnection = connectionFactory.GetConnection(); ;
            _notificationService = notificationService;
        }

        public async Task<UserBookingInvoiceReport> GetBookingHistoryDetail(int BookingId)
        {
            try
            {
                //BookingHistoryDetail historyDetail = new BookingHistoryDetail();
                var query = "SpSelectBookingHistoryDetail";
                var param = new DynamicParameters();
                param.Add("@BookingId", BookingId);

                var historyDetail = await SqlMapper.QueryFirstOrDefaultAsync<UserBookingInvoiceReport>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return historyDetail;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve Booking History details");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public async Task<UserBookingInvoiceReport> GetAdminDashboardBookingHistoryDetail(int BookingId)
        {
            try
            {
                //BookingHistoryDetail historyDetail = new BookingHistoryDetail();
                var query = "SpSelectBookingHistoryDetailAdminDashboard";
                var param = new DynamicParameters();
                param.Add("@BookingId", BookingId);

                var historyDetail = await SqlMapper.QueryFirstOrDefaultAsync<UserBookingInvoiceReport>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return historyDetail;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve Booking History details");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        
        public async Task<UsersDetails> GetUserDetail(int BookingId)
        {
            try
            {
                //UsersDetails historyDetail = new UsersDetails();
                var query = "SpSelectUserLoginDetails";
                var param = new DynamicParameters();
                param.Add("@userid", BookingId);

                var historyDetail = await SqlMapper.QueryFirstOrDefaultAsync<UsersDetails>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return historyDetail;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve Booking History details");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<IEnumerable<UserBookingInvoiceReport>> GetBookingHistoryList(BookingHistorySearch bookingHistorySearch)
        {
            try
            {
                _connectionFactory.OpenConnection();
                //List<BookingHistory> historyDetail = new List<BookingHistory>();
                var query = "SpSelectPublicUserBookings";
                var param = new DynamicParameters();
                param.Add("@Caretaker", bookingHistorySearch.Caretaker);
                param.Add("@PublicUserId", bookingHistorySearch.PublicUserId);
                param.Add("@ServiceId", bookingHistorySearch.ServiceId);
                param.Add("@SearchDateType", bookingHistorySearch.DateSearchType);
                param.Add("@Year", bookingHistorySearch.Year);
                param.Add("@Month", bookingHistorySearch.Month);
                param.Add("@FromDate", bookingHistorySearch.FromDate);
                param.Add("@ToDate", bookingHistorySearch.ToDate);
                var historyDetail = await SqlMapper.QueryAsync<UserBookingInvoiceReport>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);

                return historyDetail;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve Booking History List details");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public async Task<IEnumerable<UserBookingInvoiceReport>> GetBookingHistoryListById(int publicUserId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                //List<BookingHistory> historyDetail = new List<BookingHistory>();
                var query = "SpSelectPublicUserBookingsById";
                var param = new DynamicParameters();
                param.Add("@PublicUserId", publicUserId);
                var historyDetail = await SqlMapper.QueryAsync<UserBookingInvoiceReport>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);

                return historyDetail;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve Booking History List details");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<IEnumerable<UserInvoiceParams>> GetBookingInvoiceList(BookingHistorySearch bookingHistorySearch)
        {
            try
            {
                _connectionFactory.OpenConnection();
                //List<BookingHistory> historyDetail = new List<BookingHistory>();
                var query = "SpSelectPublicUserInvoiceDetails";
                var param = new DynamicParameters();
                param.Add("@InvoiceSearchInputId", bookingHistorySearch.InvoiceSearchInputId);
                param.Add("@InvoiceNumber", bookingHistorySearch.InvoiceNumber);
                param.Add("@ServiceId", bookingHistorySearch.ServiceId);
                param.Add("@UserId", bookingHistorySearch.PublicUserId);
                param.Add("@FromDate", bookingHistorySearch.FromDate);
                param.Add("@ToDate", bookingHistorySearch.ToDate);
                param.Add("@Year", bookingHistorySearch.Year);
                param.Add("@Month", bookingHistorySearch.Month);
              
              
               
                var historyDetail = await SqlMapper.QueryAsync<UserInvoiceParams>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);

                return historyDetail;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve Booking History List details");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public async Task<IEnumerable<UserInvoiceParams>> GetBookingInvoiceListforUserDashBoard(int publicUserId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                //List<BookingHistory> historyDetail = new List<BookingHistory>();
                var query = "SpSelectPublicUserInvoiceDetailsForUserDashBoard";
                var param = new DynamicParameters();
                param.Add("@UserId", publicUserId);
                var historyDetail = await SqlMapper.QueryAsync<UserInvoiceParams>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);

                return historyDetail;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve Booking History List details");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public async Task<IEnumerable<UserBookingInvoiceReport>> GetBookingHistoryListForInvoiceGeneration(BookingHistorySearch bookingHistorySearch)
        {
            try
            {
                _connectionFactory.OpenConnection();
                //List<BookingHistory> historyDetail = new List<BookingHistory>();
                var query = "SpSelectPublicUserBookingsForInvoiceGeneration";
                var param = new DynamicParameters();
                param.Add("@Caretaker", bookingHistorySearch.Caretaker);
                param.Add("@PublicUserId", bookingHistorySearch.PublicUserId);
                param.Add("@ServiceId", bookingHistorySearch.ServiceId);
                param.Add("@SearchDateType", bookingHistorySearch.DateSearchType);
                param.Add("@Year", bookingHistorySearch.Year);
                param.Add("@Month", bookingHistorySearch.Month);
                param.Add("@FromDate", bookingHistorySearch.FromDate);
                param.Add("@ToDate", bookingHistorySearch.ToDate);
                var historyDetail = await SqlMapper.QueryAsync<UserBookingInvoiceReport>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);

                return historyDetail;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve Booking History List details");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<IEnumerable<PaymentReportDetails>> GetCaretakerBookings(CaretakerWiseSearchReport bookingHistorySearch)
        {
            try
            {
                _connectionFactory.OpenConnection();
                //List<PaymentReportDetails> historyDetail = new List<PaymentReportDetails>();
                var query = "SpSelectCaretakerBookings";
                var param = new DynamicParameters();
                param.Add("@Caretaker", bookingHistorySearch.CareTaker);
                param.Add("@ServiceId", bookingHistorySearch.ServiceId);
                param.Add("@SearchDateType", bookingHistorySearch.SearchType);
                param.Add("@Year", bookingHistorySearch.Year);
                param.Add("@Month", bookingHistorySearch.Month);
                param.Add("@FromDate", bookingHistorySearch.FromDate);
                param.Add("@ToDate", bookingHistorySearch.ToDate);

                var historyDetail = await SqlMapper.QueryAsync<PaymentReportDetails>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return historyDetail;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve Caregiver Booking details");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public string DownloadDbBackup(string path)
        {
            try
            {
                string newpath = System.Web.Hosting.HostingEnvironment.MapPath("~/DbBackup");
                _connectionFactory.OpenConnection();
                string downloadPath = "'" + newpath + "\\" + DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".bak'";
                string downloadPathReturn = newpath + "\\" + DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".bak";
                var query = "backup database " + _dbConnection.Database + " to disk=" + downloadPath;
                int results = SqlMapper.Execute(_dbConnection, query);
                return downloadPathReturn;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve Booking History List details");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<IEnumerable<CaretakerBookingReport>> GetBookingHistoryReport(CaretakerBookingReportModel caretakerBookingReport)
        {
            try
            {
                _connectionFactory.OpenConnection();
                //List<CaretakerBookingReport> historyDetail = new List<CaretakerBookingReport>();
                var query = "SpSearchCaretakerBookingHistory";
                var param = new DynamicParameters();
                param.Add("@ServiceId", caretakerBookingReport.ServiceId);
                param.Add("@Year", caretakerBookingReport.Year);
                param.Add("@Month", caretakerBookingReport.Month);
                param.Add("@FromDate", caretakerBookingReport.FromDate);
                param.Add("@ToDate", caretakerBookingReport.ToDate);

                var historyDetail = await SqlMapper.QueryAsync<CaretakerBookingReport>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return historyDetail;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve Booking History List details");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// Saves the caretaker booking.
        /// </summary>
        /// <param name="booking">The booking.</param>
        /// <returns></returns>
        public Task<int> SaveCaretakerBooking(CaretakerBookingModel booking)
        {
            IDbTransaction transaction = null;
            try
            {
                _connectionFactory.OpenConnection();
                using (transaction = _connectionFactory.BeginTransaction(_dbConnection))
                {
                    int result;
                    var param = new DynamicParameters();
                    var spBooking = "spInsertBookingDetails";
                    param.Add("@CareTakerId", booking.CareTakerId);
                    param.Add("@ServiceRequiredId", booking.ServiceRequiredId);
                    param.Add("@BookedUserId", booking.BookedUserId);
                    param.Add("@BookingDate", booking.BookingDate);
                    param.Add("@BookingStartTime", booking.BookingStartTime);
                    param.Add("@BookingEndTime", booking.BookingEndTime);
                    param.Add("@Status", booking.Status);
                    param.Add("@IsFullDay", booking.IsFullDay);
                    param.Add("@BookingId_OUT", DbType.Int32, direction: ParameterDirection.Output);
                    result = SqlMapper.ExecuteAsync(_dbConnection, spBooking, param, transaction, commandType: CommandType.StoredProcedure).Result;
                    var bookingId = param.Get<int>("@BookingId_OUT");

                    if (bookingId != 999999)
                    {
                        var sp = "spInsertCareReceipentDetails";
                        param = new DynamicParameters();
                        param.Add("@BookingId", bookingId);
                        param.Add("@FirstName", booking.FirstName);
                        param.Add("@LastName", booking.LastName);
                        param.Add("@HouseName", booking.Address);
                        param.Add("@Location", booking.Location);
                        param.Add("@CityId", booking.CityId == 0 ? null : booking.CityId);
                        param.Add("@ZipCode", booking.ZipCode);
                        param.Add("@PrimaryPhoneNo", booking.PrimaryPhoneNo);
                        param.Add("@SecondaryPhoneNo", booking.SecondaryPhoneNo);
                        param.Add("@EmailAddress", booking.EmailAddress);
                        param.Add("@Purpose", booking.Purpose);
                        result = SqlMapper.ExecuteAsync(_dbConnection, sp, param, transaction, commandType: CommandType.StoredProcedure).Result;

                        param = new DynamicParameters();
                        var spQuestionare = "spInsertQuestionnaireDetails";
                        if (booking.Questionaire != null)
                        {
                            param = new DynamicParameters();
                            param.Add("@BookingId", bookingId);
                            param.Add("@Answer1", booking.Questionaire.Answer1);
                            param.Add("@Answer2", booking.Questionaire.Answer2);
                            param.Add("@Answer3", booking.Questionaire.Answer3);
                            param.Add("@Answer4", booking.Questionaire.Answer4);
                            param.Add("@Answer5", booking.Questionaire.Answer5);
                            param.Add("@Answer6", booking.Questionaire.Answer6);
                            int resultQuestion = SqlMapper.QueryAsync<int>(_dbConnection, spQuestionare, param, transaction, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                        }

                        transaction.Commit();
                    }
                    return Task.FromResult(bookingId);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to save booking details");
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                return Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }


    }
}
