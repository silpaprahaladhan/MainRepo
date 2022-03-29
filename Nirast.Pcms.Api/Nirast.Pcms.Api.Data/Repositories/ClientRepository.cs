using Dapper;
using Newtonsoft.Json;
using Nirast.Pcms.Api.Sdk.Entities;
using Nirast.Pcms.Api.Sdk.Infrastructure;
using Nirast.Pcms.Api.Sdk.Logger;
using Nirast.Pcms.Api.Sdk.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using static Nirast.Pcms.Api.Sdk.Entities.Enums;

namespace Nirast.Pcms.Api.Data.Repositories
{
    public class ClientRepository : GenericRepository<ClientDetails>, IClientRepository
    {
        IConnectionFactory _connectionFactory;
        IPCMSLogger _logger;
        IDbConnection _dbConnection;
        INotificationService _notificationService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionFactory"></param>
        public ClientRepository(IConnectionFactory connectionFactory, IPCMSLogger logger, INotificationService notificationService)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _dbConnection = connectionFactory.GetConnection();
            _notificationService = notificationService;
        }

        public Task<ClientDetails> AddClientDetails(ClientDetails client)
        {
            IDbTransaction transaction = null;
            try
            {
                _connectionFactory.OpenConnection();
                using (transaction = _connectionFactory.BeginTransaction(_dbConnection))
                {
                    int result;
                    int clientId = 0;
                    int userId = 0;
                    var saveUsersDetails = "SpInsertUpdateUserDetails";
                    var param = new DynamicParameters();
                    if (client.ClientId == 0)
                    {
                        int newUserId = 0;
                        if (client.UserId != 0)
                        {
                            param.Add("@Id", client.UserId);
                            newUserId = client.UserId;
                        }
                        param.Add("@FirstName", client.ClientName);
                        param.Add("@LastName", null);
                        param.Add("@EmployeeNo", client.EmployeeNumber);
                        param.Add("@BranchId", client.BranchId1);
                        param.Add("@GenderId", 1);
                        param.Add("@Dob", null);
                        param.Add("@Location", null);
                        param.Add("@CityId", client.CityId1);
                        param.Add("@Housename", client.Address1);
                        param.Add("@ZipCode", null);
                        param.Add("@UserTypeId", 3);
                        param.Add("@PrimaryPhoneNo", null);
                        param.Add("@SecondaryPhoneNo", null);
                        param.Add("@EmailAddress", client.EmailId);
                        param.Add("@Password", client.Password);
                        param.Add("@UserVerified", false);
                        param.Add("@UserStatus", 1);
                        param.Add("@UserID_OUT", DbType.Int32, direction: ParameterDirection.Output);
                        int resultUser = SqlMapper.ExecuteAsync(_dbConnection, saveUsersDetails, param, transaction, commandType: CommandType.StoredProcedure).Result;
                        userId = param.Get<int>("@UserID_OUT");

                        var sp = "SpInsertUpdateClient";
                        param = new DynamicParameters();
                        param.Add("@ClientId", client.ClientId);
                        param.Add("@ClientName", client.ClientName);
                        param.Add("@BuildingName1", client.Address1);
                        param.Add("@CityId1", client.CityId1);
                        param.Add("@BranchId1", client.BranchId1);
                        param.Add("@BranchId2", client.BranchId2);
                        param.Add("@PrimaryPhone1", client.PhoneNo1);
                        param.Add("@BuildingName2", client.Address2);
                        param.Add("@CityId2", client.CityId2 == 0 ? null : client.CityId2);
                        param.Add("@PrimaryPhone2", client.PhoneNo2);
                        param.Add("@EmailAddress", client.EmailId);
                        param.Add("@WebsiteAddress", client.WebsiteAddress);
                        param.Add("@SecondaryPhone1", client.SecondaryPhoneNo1);
                        param.Add("@SecondaryPhone2", client.SecondaryPhoneNo2);
                        param.Add("@InvoiceAddress", client.InvoiceAddress);
                        param.Add("@InvoiceNumber", client.InvoiceNumber);
                        param.Add("@InvoicePrefix", client.InvoicePrefix);
                        param.Add("@HasMidnightCut", client.HasMidnightCut);
                        param.Add("@UserId", userId);
                        param.Add("@ClientId_OUT", DbType.Int32, direction: ParameterDirection.Output);
                        result = SqlMapper.ExecuteAsync(_dbConnection, sp, param, transaction, commandType: CommandType.StoredProcedure).Result;
                        clientId = param.Get<int>("@ClientId_OUT");
                    }
                    else
                    {
                        var sp = "SpInsertUpdateClient";
                        param = new DynamicParameters();
                        param.Add("@ClientId", client.ClientId);
                        param.Add("@ClientName", client.ClientName);
                        param.Add("@BuildingName1", client.Address1);
                        param.Add("@CityId1", client.CityId1);
                        param.Add("@PrimaryPhone1", client.PhoneNo1);
                        param.Add("@BuildingName2", client.Address2);
                        param.Add("@CityId2", client.CityId2 == 0 ? null : client.CityId2);
                        param.Add("@BranchId1", client.BranchId1);
                        param.Add("@BranchId2", client.BranchId2);
                        param.Add("@PrimaryPhone2", client.PhoneNo2);
                        param.Add("@EmailAddress", client.EmailId);
                        param.Add("@WebsiteAddress", client.WebsiteAddress);
                        param.Add("@SecondaryPhone1", client.SecondaryPhoneNo1);
                        param.Add("@SecondaryPhone2", client.SecondaryPhoneNo2);
                        param.Add("@InvoiceAddress", client.InvoiceAddress);
                        param.Add("@InvoiceNumber", client.InvoiceNumber);
                        param.Add("@InvoicePrefix", client.InvoicePrefix);
                        param.Add("@HasMidnightCut", client.HasMidnightCut);
                        param.Add("@UserId", 0);
                        param.Add("@ClientId_OUT", DbType.Int32, direction: ParameterDirection.Output);
                        result = SqlMapper.ExecuteAsync(_dbConnection, sp, param, transaction, commandType: CommandType.StoredProcedure).Result;
                        clientId = param.Get<int>("@ClientId_OUT");
                    }
                    if (clientId > 0)
                    {
                        DataTable dtCategoryRates = ConvertToCareTakerTypeType(client.CategoryRates);

                        //DataTable dtClientCaretakers = ConvertToClientCaretakerType(client.ClientCaretakers);

                        DataTable dtClientShiftList = ConvertToClientShiftIdType(client.ClientShiftList);
                        param = new DynamicParameters();
                        var spSaveClientMultipleDetails = "spSaveClientMultipleDetails";
                        param.Add("@ClientId", clientId);
                        param.Add("@Payrisedate", client.EffectiveFrom);
                        param.Add("@ClientCategoryRates", dtCategoryRates, DbType.Object);
                        //param.Add("@ClientCaretakers", dtClientCaretakers, DbType.Object);
                        param.Add("@ClientShifts", dtClientShiftList, DbType.Object);
                        int resultClientDetails = SqlMapper.QueryAsync<int>(_dbConnection, spSaveClientMultipleDetails, param, transaction, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                    }
                    transaction.Commit();
                    client.ClientId = clientId;
                    client.UserId = userId;
                    return Task.FromResult(client);

                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to save Client details");
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                return Task.FromResult(client);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public Task<ClientDetails> AddFranchiseDetails(ClientDetails client)
        {
            IDbTransaction transaction = null;
            try
            {
                _connectionFactory.OpenConnection();
                using (transaction = _connectionFactory.BeginTransaction(_dbConnection))
                {
                    int result;
                    int clientId = 0;
                    int userId = 0;
                    var saveUsersDetails = "SpInsertUpdateUserDetails";
                    var param = new DynamicParameters();
                    if (client.ClientId == 0)
                    {
                        int newUserId = 0;
                        if (client.UserId != 0)
                        {
                            param.Add("@Id", client.UserId);
                            newUserId = client.UserId;
                        }

                        param.Add("@EmployeeNo", client.EmployeeNumber);
                        param.Add("@FirstName", client.ClientName);
                        param.Add("@LastName", null);
                        param.Add("@BranchId", client.BranchId1);
                        param.Add("@GenderId", 1);
                        param.Add("@Dob", null);
                        param.Add("@Location", null);
                        param.Add("@CityId", client.CityId1);
                        param.Add("@Housename", client.Address1);
                        param.Add("@ZipCode", null);
                        param.Add("@UserTypeId", 3);
                        param.Add("@PrimaryPhoneNo", null);
                        param.Add("@SecondaryPhoneNo", null);
                        param.Add("@EmailAddress", client.EmailId);
                        param.Add("@Password", client.Password);
                        param.Add("@UserVerified", false);
                        param.Add("@UserStatus", 1);
                        param.Add("@UserID_OUT", DbType.Int32, direction: ParameterDirection.Output);
                        int resultUser = SqlMapper.ExecuteAsync(_dbConnection, saveUsersDetails, param, transaction, commandType: CommandType.StoredProcedure).Result;
                        userId = param.Get<int>("@UserID_OUT");

                        var sp = "SpInsertUpdateFranchise";
                        param = new DynamicParameters();
                        param.Add("@ClientId", client.ClientId);
                        param.Add("@ClientName", client.ClientName);
                        param.Add("@BuildingName1", client.Address1);
                        param.Add("@CityId1", client.CityId1);
                        param.Add("@PrimaryPhone1", client.PhoneNo1);
                        param.Add("@BuildingName2", client.Address2);
                        param.Add("@CityId2", client.CityId2 == 0 ? null : client.CityId2);
                        param.Add("@PrimaryPhone2", client.PhoneNo2);
                        param.Add("@EmailAddress", client.EmailId);
                        param.Add("@WebsiteAddress", client.WebsiteAddress);
                        param.Add("@SecondaryPhone1", client.SecondaryPhoneNo1);
                        param.Add("@SecondaryPhone2", client.SecondaryPhoneNo2);
                        param.Add("@InvoiceAddress", client.InvoiceAddress);
                        param.Add("@InvoiceNumber", client.InvoiceNumber);
                        param.Add("@InvoicePrefix", client.InvoicePrefix);
                        param.Add("@franchise", client.franchise);
                        param.Add("@HasMidnightCut", client.HasMidnightCut);
                        param.Add("@UserId", userId);

                        param.Add("@ClientId_OUT", DbType.Int32, direction: ParameterDirection.Output);
                        result = SqlMapper.ExecuteAsync(_dbConnection, sp, param, transaction, commandType: CommandType.StoredProcedure).Result;
                        clientId = param.Get<int>("@ClientId_OUT");
                    }
                    else
                    {
                        var sp = "SpInsertUpdateFranchise";
                        param = new DynamicParameters();
                        param.Add("@ClientId", client.ClientId);
                        param.Add("@ClientName", client.ClientName);
                        param.Add("@BuildingName1", client.Address1);
                        param.Add("@CityId1", client.CityId1);
                        param.Add("@PrimaryPhone1", client.PhoneNo1);
                        param.Add("@BuildingName2", client.Address2);
                        param.Add("@CityId2", client.CityId2 == 0 ? null : client.CityId2);
                        param.Add("@PrimaryPhone2", client.PhoneNo2);
                        param.Add("@EmailAddress", client.EmailId);
                        param.Add("@WebsiteAddress", client.WebsiteAddress);
                        param.Add("@SecondaryPhone1", client.SecondaryPhoneNo1);
                        param.Add("@SecondaryPhone2", client.SecondaryPhoneNo2);
                        param.Add("@InvoiceAddress", client.InvoiceAddress);
                        param.Add("@InvoiceNumber", client.InvoiceNumber);
                        param.Add("@InvoicePrefix", client.InvoicePrefix);
                        param.Add("@franchise", client.franchise);
                        param.Add("@HasMidnightCut", client.HasMidnightCut);

                        param.Add("@UserId", 0);
                        param.Add("@ClientId_OUT", DbType.Int32, direction: ParameterDirection.Output);
                        result = SqlMapper.ExecuteAsync(_dbConnection, sp, param, transaction, commandType: CommandType.StoredProcedure).Result;
                        clientId = param.Get<int>("@ClientId_OUT");
                    }

                    if (clientId == 0)
                    {
                        param = new DynamicParameters();
                        param.Add("@Comission", client.Commission);
                        param.Add("@Rate", client.EffectiveFrom);
                        param.Add("@FranchiseId", clientId);
                        var SpFranchise = "SpFranchise";

                        int resultClientDetails = SqlMapper.ExecuteAsync(_dbConnection, SpFranchise, param, transaction, commandType: CommandType.StoredProcedure).Result;
                    }
                    else
                    {
                        param = new DynamicParameters();
                        param.Add("@Comission", client.Commission);
                        param.Add("@Rate", client.EffectiveFrom);
                        param.Add("@FranchiseId", clientId);
                        var SpFranchise = "UpdateSpFranchise";

                        int resultClientDetails = SqlMapper.ExecuteAsync(_dbConnection, SpFranchise, param, transaction, commandType: CommandType.StoredProcedure).Result;
                    }
                    //if (clientId > 0)
                    //{
                    //    DataTable dtCategoryRates = ConvertToCareTakerTypeType(client.CategoryRates);


                    //    DataTable dtClientShiftList = ConvertToClientShiftIdType(client.ClientShiftList);
                    //    param = new DynamicParameters();
                    //    var spSaveClientMultipleDetails = "spSaveClientMultipleDetails";
                    //    param.Add("@ClientId", clientId);
                    //    param.Add("@Payrisedate", client.EffectiveFrom);
                    //    param.Add("@ClientCategoryRates", dtCategoryRates, DbType.Object);
                    //    //param.Add("@ClientCaretakers", dtClientCaretakers, DbType.Object);
                    //    param.Add("@ClientShifts", dtClientShiftList, DbType.Object);
                    //    int resultClientDetails = SqlMapper.QueryAsync<int>(_dbConnection, spSaveClientMultipleDetails, param, transaction, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                    //}
                    transaction.Commit();
                    client.ClientId = clientId;
                    client.UserId = userId;
                    return Task.FromResult(client);

                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to save Client details");
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                return Task.FromResult(client);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }


        public Task<int> AddClientInvoiceDetails(InvoiceSearchInpts invoiceDetails)
        {
            IDbTransaction transaction = null;
            try
            {
                _connectionFactory.OpenConnection();
                using (transaction = _connectionFactory.BeginTransaction(_dbConnection))
                {
                    DateTime startDate, endDate;

                    if (invoiceDetails.Mode == 1) // Yearly
                    {
                        startDate = new DateTime(Convert.ToInt32(invoiceDetails.Year), 1, 1);
                        endDate = new DateTime(Convert.ToInt32(invoiceDetails.Year), 12, 31);
                    }
                    else if (invoiceDetails.Mode == 2) // Monthly
                    {
                        startDate = new DateTime(Convert.ToInt32(invoiceDetails.Year), Convert.ToInt32(invoiceDetails.Month), 1);
                        endDate = startDate.AddMonths(1).AddDays(-1);
                    }
                    else
                    {
                        startDate = Convert.ToDateTime(invoiceDetails.StartDate);
                        endDate = Convert.ToDateTime(invoiceDetails.EndDate);
                    }

                    int result;
                    var sp = "SpInsertUpdateClientInvoiceDetails";
                    var param = new DynamicParameters();
                    param.Add("@InvoiceDate", invoiceDetails.InvoiceDate);
                    param.Add("@InvoiceSearchInputId", invoiceDetails.InvoiceSearchInputId);
                    param.Add("@InvoiceNumber", invoiceDetails.InvoiceNumber);
                    param.Add("@InvoicePrefix", invoiceDetails.InvoicePrefix);
                    param.Add("@ClientId", invoiceDetails.ClientId);
                    param.Add("@StartDate", startDate);
                    param.Add("@EndDate", endDate);
                    param.Add("@Mode", (invoiceDetails.Mode == 0) ? null : invoiceDetails.Mode);
                    param.Add("@Year", (invoiceDetails.Year == 0) ? null : invoiceDetails.Year);
                    param.Add("@Month", (invoiceDetails.Month == 0) ? null : invoiceDetails.Month);
                    param.Add("@Seperateinvoice", invoiceDetails.Seperateinvoice);
                    param.Add("@Description", invoiceDetails.Description);
                    param.Add("@Category", invoiceDetails.Category);
                    param.Add("@PdfFilePath", invoiceDetails.PdfFilePath);
                    param.Add("@InvoiceSearchInputId_Out", DbType.Int32, direction: ParameterDirection.Output);
                    result = SqlMapper.ExecuteAsync(_dbConnection, sp, param, transaction, commandType: CommandType.StoredProcedure).Result;
                    var clientId = param.Get<int>("@InvoiceSearchInputId_Out");
                    transaction.Commit();
                    return Task.FromResult(clientId);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to save Client Invoice details");
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

        public Task<int> AddScheduledDetails(ScheduledData data, out string message)
        {
            IDbTransaction transaction = null;
            message = "Success";
            try
            {
                _connectionFactory.OpenConnection();
                using (transaction = _connectionFactory.BeginTransaction(_dbConnection))
                {
                    int result;
                    var sp = "SpAddEditScheduleDetails";
                    var param = new DynamicParameters();
                    param.Add("@Id", data.Id);
                    param.Add("@ClientId", data.ClientId);
                    param.Add("@WorkShiftId", data.WorkMode);
                    param.Add("@TimeShiftId", data.WorkTime == 0 ? null : data.WorkTime);
                    param.Add("@StartDate", data.Start);
                    param.Add("@EndDate", data.End);
                    param.Add("@ScheduleDescription", data.Description);
                    param.Add("@CareTaker", data.CareTaker);
                    param.Add("@ScheduledID_OUT", DbType.Int32, direction: ParameterDirection.Output);
                    result = SqlMapper.ExecuteAsync(_dbConnection, sp, param, transaction, commandType: CommandType.StoredProcedure).Result;
                    var scheduledId = param.Get<int>("@ScheduledID_OUT");

                    var spDeleteScheduleDetails = "SpDeleteClientScheduleOneToOneTable";
                    param = new DynamicParameters();
                    param.Add("@ScheduleId", scheduledId);
                    int resultOnetoOne = SqlMapper.QueryAsync<int>(_dbConnection, spDeleteScheduleDetails, param, transaction, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();

                    if (!string.IsNullOrEmpty(data.ContactPerson))
                    {
                        var spAddSubScheduleDetails = "SpAddEditClientScheduleOneToOneTable";
                        param = new DynamicParameters();
                        param.Add("@ScheduleId", scheduledId);
                        param.Add("@SubDescription", data.ContactPerson);
                        param.Add("@ClientId", data.ClientId);
                        int resultShift = SqlMapper.QueryAsync<int>(_dbConnection, spAddSubScheduleDetails, param, transaction, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                    }


                    if (data.ClientSchedulingDate != null)
                    {
                        var spspSaveClientSchedulingDates = "SpDeleteClientSchedulingDates";
                        param = new DynamicParameters();
                        param.Add("@ScheduleId", scheduledId);
                        int resultdelete = SqlMapper.QueryAsync<int>(_dbConnection, spspSaveClientSchedulingDates, param, transaction, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();

                        foreach (var item in data.ClientSchedulingDate)
                        {
                            param = new DynamicParameters();
                            var _spSaveClientSchedulingDates = "spSaveClientSchedulingDates";
                            param.Add("@SchedulingId", scheduledId);
                            param.Add("@Date", item.Date);
                            param.Add("@Hours", item.Hours);
                            int resultCareTakerMultiDetails = SqlMapper.QueryAsync<int>(_dbConnection, _spSaveClientSchedulingDates, param, transaction, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();

                        }

                    }
                    transaction.Commit();
                    return Task.FromResult(scheduledId);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to Scheduling Details");
                message = "failed:" + ex.ToString();
                transaction.Rollback();
                return Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }

        }


        /// <summary>
        /// method to search client
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ClientDetails>> ClientSearch(ClientSearchInputs inputs)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpClientSearch";
                var param = new DynamicParameters();
                param.Add("ClientName", inputs.ClientName);
                param.Add("@Location", inputs.Location);
                var result = await SqlMapper.QueryAsync<ClientDetails>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while searching client details");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// To get client details
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ClientDetails>> GetClientDetails()
        {
            try
            {
                _connectionFactory.OpenConnection();
                var Details = "SpGetClientDetails";
                var param = new DynamicParameters();
                var result = await SqlMapper.QueryAsync<ClientDetails>(_dbConnection, Details, param, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving client details");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<IEnumerable<ClientDetails>> GetClientDetailsByLocation(LocationSearchInputs inputs)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var Details = "SpGetClientDetails";
                var param = new DynamicParameters();
                param.Add("@CountryId", inputs.CountryId);
                param.Add("@StateId", inputs.StateId);
                param.Add("@CityId", inputs.CityId); ;
                var result = await SqlMapper.QueryAsync<ClientDetails>(_dbConnection, Details, param, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving client details");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public async Task<IEnumerable<ClientDetails>> GetFranchiseDetailsByLocation(LocationSearchInputs inputs)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var Details = "SpGeFranchiseDetails";
                var param = new DynamicParameters();
                param.Add("@CountryId", inputs.CountryId);
                param.Add("@StateId", inputs.StateId);
                param.Add("@CityId", inputs.CityId); ;
                var result = await SqlMapper.QueryAsync<ClientDetails>(_dbConnection, Details, param, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving client details");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public async Task<IEnumerable<UsersDetails>> GetUserTypeId()
        {
            try
            {
                _connectionFactory.OpenConnection();
                var Details = "SpGetUserType";
                var result = await SqlMapper.QueryAsync<UsersDetails>(_dbConnection, Details, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving client details");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        /// <summary>
        /// To get client details
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ScheduledData>> GetAllScheduleLogDetails()
        {
            try
            {
                _connectionFactory.OpenConnection();
                var Details = "SpGetScheduleLogDetails";
                var result = await SqlMapper.QueryAsync<ScheduledData>(_dbConnection, Details, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving client details");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public async Task<IEnumerable<LoginLog>> GetLoginLogDetailsByTypeId(int typeId)
        {
            var Details = "SpGetLoginLogDetailsByUserType";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@UserTypeId", typeId);
                var result = await SqlMapper.QueryAsync<LoginLog>(_dbConnection, Details, param, commandType: CommandType.StoredProcedure);
                //var result = await SqlMapper.QueryFirstOrDefaultAsync<List<LoginLog>>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to SpGetLoginLogDetailsByUserType.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<IEnumerable<RejectedCaretaker>> GetAllScheduleRejectedCaretaker(BookingHistorySearch bookingHistorySearch)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var Details = "SpGetAllScheduleRejectedCaretake";
                var param = new DynamicParameters();
                param.Add("@Caretaker", bookingHistorySearch.Caretaker);
                param.Add("@DateFrom", bookingHistorySearch.FromDate);
                param.Add("@DateTo", bookingHistorySearch.ToDate);
                var result = await SqlMapper.QueryAsync<RejectedCaretaker>(_dbConnection, Details, param, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving client details");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public Task<ClientDetails> GetClientDetailsByID(int clientId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                ClientDetails result = new ClientDetails();
                var query = "SpGetClientDetailsById";
                var param = new DynamicParameters();
                param.Add("@clientID", clientId);

                using (var multi = _dbConnection.QueryMultiple(query, param, commandType: CommandType.StoredProcedure))
                {
                    result = multi.Read<ClientDetails>().FirstOrDefault();
                    var addressModel = multi.Read<ClientAddressModel>().ToList();

                    result.Address1 = addressModel[0].BuildingName;
                    result.PhoneNo1 = addressModel[0].Phone1;
                    result.PhoneNo2 = addressModel[0].Phone2;
                    result.CityId1 = addressModel[0].CityId;
                    result.StateId1 = addressModel[0].StateId;
                    result.CountryId1 = addressModel[0].CountryId;
                    result.City1 = addressModel[0].CityName;
                    result.State1 = addressModel[0].StateName;
                    result.Country1 = addressModel[0].CountryName;
                    if (addressModel.Count > 1)
                    {
                        result.Address2 = addressModel[1].BuildingName;

                        result.CityId2 = addressModel[1].CityId;
                        result.StateId2 = addressModel[1].StateId;
                        result.CountryId2 = addressModel[1].CountryId;
                        result.City2 = addressModel[1].CityName;
                        result.State2 = addressModel[1].StateName;
                        result.Country2 = addressModel[1].CountryName;
                        result.SecondaryPhoneNo1 = addressModel[1].Phone1;
                        result.SecondaryPhoneNo2 = addressModel[1].Phone2;
                    }

                    //var caretakerdetails = multi.Read<CareTakerRegistrationModel>().ToList();
                    var categoryrate = multi.Read<ClientCategoryRate>().ToList();
                    var clientcaretaker = multi.Read<ClientCaretakers>().ToList();
                    var clientCaretakerMaps = multi.Read<ClientCaretakerMap>().ToList();
                    var clientTimeShift = multi.Read<ClientShiftDetails>().ToList();
                    //var registerdCaretakers = multi.Read<CareTakerRegistrationModel>().ToList();

                    result.ClientCaretakerMaps = clientCaretakerMaps;
                    result.ClientCaretakers = clientcaretaker;
                    result.CategoryRates = categoryrate;
                    result.ClientCaretakers = clientcaretaker;
                    result.ClientShiftList = clientTimeShift;
                    //result.RegistredCaretakers = registerdCaretakers;

                }


                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve Caregiver details");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }


        public Task<ClientDetails> GetFranchiseDetailsByID(int clientId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                ClientDetails result = new ClientDetails();
                var query = "SpGetFranchiseDetailsById";
                var param = new DynamicParameters();
                param.Add("@clientID", clientId);

                using (var multi = _dbConnection.QueryMultiple(query, param, commandType: CommandType.StoredProcedure))
                {
                    result = multi.Read<ClientDetails>().FirstOrDefault();
                    var addressModel = multi.Read<ClientAddressModel>().ToList();

                    result.Address1 = addressModel[0].BuildingName;
                    result.PhoneNo1 = addressModel[0].Phone1;
                    result.PhoneNo2 = addressModel[0].Phone2;
                    result.CityId1 = addressModel[0].CityId;
                    result.StateId1 = addressModel[0].StateId;
                    result.CountryId1 = addressModel[0].CountryId;
                    result.City1 = addressModel[0].CityName;
                    result.State1 = addressModel[0].StateName;
                    result.Country1 = addressModel[0].CountryName;
                    if (addressModel.Count > 1)
                    {
                        result.Address2 = addressModel[1].BuildingName;

                        result.CityId2 = addressModel[1].CityId;
                        result.StateId2 = addressModel[1].StateId;
                        result.CountryId2 = addressModel[1].CountryId;
                        result.City2 = addressModel[1].CityName;
                        result.State2 = addressModel[1].StateName;
                        result.Country2 = addressModel[1].CountryName;
                        result.SecondaryPhoneNo1 = addressModel[1].Phone1;
                        result.SecondaryPhoneNo2 = addressModel[1].Phone2;
                    }

                    //var caretakerdetails = multi.Read<CareTakerRegistrationModel>().ToList();
                    var categoryrate = multi.Read<ClientCategoryRate>().ToList();
                    var clientcaretaker = multi.Read<ClientCaretakers>().ToList();
                    var clientCaretakerMaps = multi.Read<ClientCaretakerMap>().ToList();
                    var clientTimeShift = multi.Read<ClientShiftDetails>().ToList();
                    //var registerdCaretakers = multi.Read<CareTakerRegistrationModel>().ToList();

                    result.ClientCaretakerMaps = clientCaretakerMaps;
                    result.ClientCaretakers = clientcaretaker;
                    result.CategoryRates = categoryrate;
                    result.ClientCaretakers = clientcaretaker;
                    result.ClientShiftList = clientTimeShift;
                    //result.RegistredCaretakers = registerdCaretakers;

                }


                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve Caregiver details");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<IEnumerable<ScheduledData>> GetAllScheduledetails(CalenderEventInput calenderEventInput)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var Details = "SpSelectScheduleDetails";
                var param = new DynamicParameters();
                param.Add("@ScheduleId", calenderEventInput.ScheduleId);
                param.Add("@StartDate", calenderEventInput.StartDate);
                param.Add("@EndDate", calenderEventInput.EndDate);
                var result = await SqlMapper.QueryAsync<ScheduledData>(_dbConnection, Details, param, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving client details");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public Task<int> DeleteSchedule(ScheduleDeleteData deleteData)
        {
            string message = "Success";
            IDbTransaction transaction = null;
            try
            {
                _connectionFactory.OpenConnection();
                int result;
                var query = "SpDeleteSchedule";
                var param = new DynamicParameters();
                param.Add("@Id", deleteData.ScheduleId);
                result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();

                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "delete failed");
                message = "Failed";
                return null;
            }
            finally
            {
                var param = new DynamicParameters();
                var auditSp = "SpAddScheduleLogDetails";
                param = new DynamicParameters();
                param.Add("@FeatureId", deleteData.AuditLogType);
                param.Add("@UserId", deleteData.UserId);
                param.Add("@Action", deleteData.AuditLogActionType);
                param.Add("@Message", message);
                param.Add("@OldData", deleteData.OldData);
                param.Add("@NewData", null);
                param.Add("@UpdatedDate", DateTime.Now);
                param.Add("@CareTakerName", deleteData.CareTakerName);
                param.Add("@ClientId", deleteData.ClientId);
                param.Add("@LogId_OUT", DbType.Int32, direction: ParameterDirection.Output);
                int resultLog = SqlMapper.ExecuteAsync(_dbConnection, auditSp, param, transaction, commandType: CommandType.StoredProcedure).Result;
                _connectionFactory.CloseConnection();
            }
        }


        public Task<int> ModifyClientStatusById(int id, int status)
        {

            try
            {
                _connectionFactory.OpenConnection();
                int result;
                var query = "SpModifyClientStatusById";
                var param = new DynamicParameters();
                param.Add("@ClientId", id);
                param.Add("@status", status);
                result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "update failed");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }



        public Task<int> ModifyFranchiseStatusById(int id, int status)
        {

            try
            {
                _connectionFactory.OpenConnection();
                int result;
                var query = "SpModifyFranchiseStatusById";
                var param = new DynamicParameters();
                param.Add("@ClientId", id);
                param.Add("@status", status);
                result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "update failed");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }



        public Task<int> ChangeClientEmailStatus(int id, int emailstatus)
        {

            try
            {
                _connectionFactory.OpenConnection();
                int result;
                var query = "SpModifyClientEmailStatusById";
                var param = new DynamicParameters();
                param.Add("@ClientId", id);
                param.Add("@status", emailstatus);
                result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "update failed");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }


        public Task<int> UpdateClientInvoiceNumber(int clientId, int InvoiceNumber)
        {

            try
            {
                _connectionFactory.OpenConnection();
                int result;
                var query = "SpUpdateClientInvoiceNumber";
                var param = new DynamicParameters();
                param.Add("@ClientId", clientId);
                param.Add("@InvoiceNumber", InvoiceNumber);
                result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "update failed");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<int> SaveClientCareTakerMapping(WorkShiftPayRates shiftPayRates)
        {
            try
            {
                _connectionFactory.OpenConnection();
                //int result = 0;
                //var query = "SpInsertUpdateClientCaretakers";
                //var param = new DynamicParameters();

                //DataColumn dtColMapClient = new DataColumn("ClientId", typeof(Int32))
                //{
                //    DefaultValue = clientcaretaker.ClientId
                //};
                //DataColumn dtColMapCaretaker = new DataColumn("CaretakerId", typeof(Int32))
                //{
                //    DefaultValue = clientcaretaker.CaretakerId
                //};
                //DataTable dtMapRates = ConvertToDataTable(clientcaretaker.MapRates);
                //dtMapRates.Columns.Remove("WorkShiftName");
                //dtMapRates.Columns.Add(dtColMapClient);
                //dtMapRates.Columns.Add(dtColMapCaretaker);
                //param.Add("@ClientId", clientcaretaker.ClientId);
                //param.Add("@CaretakerId", clientcaretaker.CaretakerId);
                //param.Add("@MappedRate", dtMapRates, DbType.Object);

                //result = await SqlMapper.QueryFirstOrDefaultAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return 0;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "insert failed");
                return 0;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public async Task<int> SaveClientCareTakerPayRise(List<WorkShiftRates> workShiftRates)
        {
            try
            {
                _connectionFactory.OpenConnection();
                int result = 0;
                var query = "SpInsertUpdateClientCaretakersPayRise";
                var param = new DynamicParameters();

                DataColumn dtColPayriseDate = new DataColumn("PayrollPayriseId", typeof(Int32))
                {
                    DefaultValue = 0
                };
                DataTable dtMapRates = ConvertToDataTable(workShiftRates);
                dtMapRates.Columns.Add(dtColPayriseDate);
                dtMapRates.Columns.Remove("WorkShiftName");
                dtMapRates.Columns.Remove("EffectiveFrom");
                param.Add("@ClientId", workShiftRates[0].ClientId);
                param.Add("@Payrisedate", workShiftRates[0].EffectiveFrom);
                param.Add("@CaretakerId", workShiftRates[0].CaretakerId);
                param.Add("@MappedRate", dtMapRates, DbType.Object);

                result = await SqlMapper.QueryFirstOrDefaultAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "insert failed");
                return 0;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<int> SaveClientcategoryCareTakerPayRise(List<ClientCategoryRate> categoryRates)
        {
            try
            {
                _connectionFactory.OpenConnection();
                int result = 0;
                var param = new DynamicParameters();
                var sp = "SpGetLastClientInvoiceRates";
                param.Add("@ClientId", categoryRates.FirstOrDefault().ClientId);
                param.Add("@Payrisedate", categoryRates.FirstOrDefault().EffectiveFrom);
                var rates = await SqlMapper.QueryAsync<ClientCategoryRate>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
                if (rates.Count() > 0)
                {
                    foreach (var s in rates)
                    {
                        var sa = categoryRates.FirstOrDefault(x => x.CategoryId == s.CategoryId);
                        sa.IsTaxApplicable = s.IsTaxApplicable;
                        categoryRates.RemoveAt(categoryRates.FindIndex(x => x.CategoryId == s.CategoryId));
                        categoryRates.Add(sa);
                    }
                }


                var query = "SpInsertUpdateClientInvoicePayRise";

                DataColumn dtColMapClient = new DataColumn("ClientId", typeof(Int32))
                {
                    DefaultValue = categoryRates.FirstOrDefault().ClientId
                };
                DataColumn dtColPayriseId = new DataColumn("InvoicePayriseId", typeof(Int32))
                {
                    DefaultValue = 0
                };
                DataTable dtMapRates = ConvertToDataTable(categoryRates);
                dtMapRates.Columns.Remove("CategoryName");
                dtMapRates.Columns.Remove("ClientId");
                dtMapRates.Columns.Remove("EffectiveFrom");
                dtMapRates.Columns.Add(dtColMapClient);
                dtMapRates.Columns.Add(dtColPayriseId);
                param.Add("@ClientId", categoryRates.FirstOrDefault().ClientId);
                param.Add("@ClientInvoicePayRiseRates", dtMapRates, DbType.Object);
                param.Add("@Payrisedate", categoryRates.FirstOrDefault().EffectiveFrom);
                result = await SqlMapper.QueryFirstOrDefaultAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "insert failed");
                return 0;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public Task<int> DeleteClientCareTakerMapping(ClientCaretakers clientcaretaker)
        {
            try
            {
                _connectionFactory.OpenConnection();
                int result;
                var query = "SpDeleteClientMappedCaretakers";
                var param = new DynamicParameters();
                param.Add("@ClientId", clientcaretaker.ClientId);
                param.Add("@CareTakerId", clientcaretaker.CaretakerId);
                result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "insert failed");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public Task<int> SaveScheduleRejectedCareTaker(RejectedCaretaker careTaker)
        {
            try
            {
                _connectionFactory.OpenConnection();
                int result;
                var query = "SpInsertScheduleRejectedCaretakers";
                var param = new DynamicParameters();
                param.Add("@CareTakerId", careTaker.CareTakerId);
                //param.Add("@Datetime", careTaker.DateTime);
                param.Add("@Description", careTaker.Description);
                param.Add("@ClientId", careTaker.ClientId);
                param.Add("@Workshift", careTaker.Workshift);
                param.Add("@ScheduleDate", careTaker.ScheduleDate);
                result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "insert failed");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        private DataTable ConvertToCareTakerTypeType(List<ClientCategoryRate> list)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("CareTakerTypeId");
            dt.Columns.Add("Rate");
            dt.Columns.Add("IsTaxApplicable");
            foreach (var item in list)
            {
                DataRow dr = dt.NewRow();
                dr["CareTakerTypeId"] = item.CategoryId;
                dr["Rate"] = item.Rate;
                dr["IsTaxApplicable"] = item.IsTaxApplicable;
                dt.Rows.Add(dr);
            }
            return dt;
        }

        private DataTable ConvertToClientCaretakerType(List<ClientCaretakers> list)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("CaretakerId");
            dt.Columns.Add("Rate");
            foreach (var item in list)
            {
                DataRow dr = dt.NewRow();
                dr["CaretakerId"] = item.CaretakerId;
                dr["Rate"] = item.Rate;
                dt.Rows.Add(dr);
            }
            return dt;
        }

        private DataTable ConvertToClientShiftIdType(List<ClientShiftDetails> list)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ShiftId");
            foreach (var item in list)
            {
                DataRow dr = dt.NewRow();
                dr["ShiftId"] = item.TimeShiftId;
                dt.Rows.Add(dr);
            }
            return dt;
        }


        private DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        /// <summary>
        ///  Method to Get EmailId For Client
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<string> GetEmailIdForClient(int userId)
        {
            List<string> ccAddressList = new List<string>();
            var sp = "SpGetEmailIdOfClientByClientId";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@ClientId", userId);

                var emailId = await SqlMapper.ExecuteScalarAsync(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
                return emailId.ToString();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to GetEmailIdOfClientByClientId.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// To get client details
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ClientDetails>> GetClientInvoiceDetails()
        {
            try
            {
                _connectionFactory.OpenConnection();
                var Details = "SpGetClientInvoiceDetails";

                var result = await SqlMapper.QueryAsync<ClientDetails>(_dbConnection, Details, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving client details");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// Adds the city.
        /// </summary>
        /// <param name="city">The city.</param>
        /// <returns></returns>
        public Task<int> AddInvoiceDetails(ClientDetails clientInvoiceDetails)
        {
            IDbTransaction transaction = null;
            try
            {
                _connectionFactory.OpenConnection();
                using (transaction = _connectionFactory.BeginTransaction(_dbConnection))
                {
                    int result;
                    var query = "SpInsertUpdateClientInvoice";
                    var param = new DynamicParameters();
                    param.Add("@ClientId", clientInvoiceDetails.ClientId);
                    param.Add("@ClientName", clientInvoiceDetails.ClientName);
                    param.Add("@InvoicePrefix", clientInvoiceDetails.InvoicePrefix);
                    param.Add("@InvoiceNumber", clientInvoiceDetails.InvoiceNumber);

                    result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, transaction, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();

                    transaction.Commit();
                    return Task.FromResult(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to add city while save city to DB");
                transaction.Rollback();
                if (ex.InnerException.Message.Contains("UNIQUE KEY"))
                    return Task.FromResult(10001);
                return Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<IEnumerable<InvoiceSearchInpts>> GetInvoiceHistoryList(InvoiceHistory invoiceHistory)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var param = new DynamicParameters();
                param.Add("@InvoiceNumber", invoiceHistory.InvoiceNumber);
                param.Add("@Clientid", invoiceHistory.ClientId);
                param.Add("@FromDate", invoiceHistory.FromDate);
                param.Add("@ToDate", invoiceHistory.ToDate);
                param.Add("@Year", invoiceHistory.Year);
                param.Add("@Month", invoiceHistory.Month);
                param.Add("@InvoiceSearchInputId", invoiceHistory.InvoiceSearchInputId);

                var query = "SpSelectClientInvoiceDetails";
                var invoiceDetail = await SqlMapper.QueryAsync<InvoiceSearchInpts>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return invoiceDetail;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve Invoice History List details");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<IEnumerable<InvoiceSearchInpts>> GetInvoiceHistoryById(int id)
        {
            try
            {
                _connectionFactory.OpenConnection();
                ClientDetails result = new ClientDetails();
                var query = "SpGetInvoiceHistoryById";
                var param = new DynamicParameters();
                param.Add("@InvId", id);

                var list = await SqlMapper.QueryAsync<InvoiceSearchInpts>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return (list);

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve Caregiver details");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public Task<int> GetClientFromUserId(int id)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "GetClientFromClientId";
                var param = new DynamicParameters();
                param.Add("@Id", id);
                var result = SqlMapper.QueryAsync<ClientDetails>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return Task.FromResult(result.Result.FirstOrDefault().ClientId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "insert failed");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public async Task<IEnumerable<WorkShiftRates>> GetMappedCaretakerRates(int clientId, int caretakerId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var param = new DynamicParameters();
                param.Add("@ClientId", clientId);
                param.Add("@CaretakerId", caretakerId);

                var query = "SpGetMappedCaretakerRates";
                var workShiftRates = await SqlMapper.QueryAsync<WorkShiftRates>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return workShiftRates;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve Invoice History List details");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<IEnumerable<WorkShiftRates>> GetMappedCaretakersLatestPayRiseRates(int clientId, int caretakerId)
        {

            try
            {
                _connectionFactory.OpenConnection();
                var param = new DynamicParameters();
                param.Add("@ClientId", clientId);
                param.Add("@CaretakerId", caretakerId);

                var query = "SpGetMappedCaretakersLatestPayRiseRates";
                var workShiftRates = await SqlMapper.QueryAsync<WorkShiftRates>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return workShiftRates;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve MappedCaretakersLatestPayRates");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<IEnumerable<WorkShiftRates>> GetMappedCaretakersPayRiseRatesByDate(PayriseData payriseData)
        {

            try
            {
                _connectionFactory.OpenConnection();
                var param = new DynamicParameters();
                param.Add("@ClientId", payriseData.ClientId);
                param.Add("@CaretakerId", payriseData.CaretakerId);
                param.Add("@Date", payriseData.Date);

                var query = "SpGetMappedCaretakersPayRiseRatesByDate";
                var workShiftRates = await SqlMapper.QueryAsync<WorkShiftRates>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return workShiftRates;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve MappedCaretakersPayRiseRatesByDate");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<IEnumerable<ClientCategoryRate>> GetCategoryClientPayRiseRates(int clientId)
        {
            var sp = "SpGetClientInvoicePayRiseRates";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@ClientId", clientId);
                var result = await SqlMapper.QueryAsync<ClientCategoryRate>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to GetCategoryClientPayRiseRates.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<IEnumerable<ClientCategoryRate>> GetClientInvoicePayRiseRatesonDateChange(int clientId, DateTime date)
        {
            var sp = "SpGetClientInvoicePayRiseRatesonDateChange";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@ClientId", clientId);
                param.Add("@date", date);
                var result = await SqlMapper.QueryAsync<ClientCategoryRate>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to GetCategoryClientPayRiseRates.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        //public async Task<IEnumerable<ClientCategoryPayRiseRate>> GetCategoryClientPayRiseRates(int clientId)
        //{
        //    try
        //    {
        //        _connectionFactory.OpenConnection();
        //        var param = new DynamicParameters();
        //        param.Add("@ClientId", clientId);

        //        var query = "SpGetMappedClientCaretakerPayRiseRates";
        //        var workShiftRates = await SqlMapper.QueryAsync<ClientCategoryPayRiseRate>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
        //        return workShiftRates;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error(ex, "Failed to retrieve Invoice History List details");
        //        return null;
        //    }
        //    finally
        //    {
        //        _connectionFactory.CloseConnection();
        //    }
        //}
        public async Task<ScheduledData> GetSchdeuleDetaildById(int scheduleId)
        {

            var sp = "SpGetSchdeuleDetaildById";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@SchedulingId", scheduleId);
                ScheduledData result = await SqlMapper.QueryFirstOrDefaultAsync<ScheduledData>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to GetSchdeuleDetaildById.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<ScheduledData> GetSchedulingLogDetailsById(int logId)
        {
            var sp = "SpGetSchedulingLogDetailsById";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@logId", logId);
                ScheduledData result = await SqlMapper.QueryFirstOrDefaultAsync<ScheduledData>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to GetSchdeulingLogDetaildById.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }


        public async Task<string> AddScheduledDetailsAuditLog(ScheduledData data, string message)
        {
            IDbTransaction transaction = null;
            try
            {
                _connectionFactory.OpenConnection();
                using (transaction = _connectionFactory.BeginTransaction(_dbConnection))
                {
                    var param = new DynamicParameters();
                    var auditSp = "SpAddScheduleLogDetails";
                    param = new DynamicParameters();
                    param.Add("@FeatureId", data.AuditLogType);
                    param.Add("@UserId", data.UserId);
                    param.Add("@Action", data.AuditLogActionType);
                    param.Add("@Message", message);

                    param.Add("@OldData", data.Id != 0 ? data.OldData : null);
                    string newData = data.NewData;
                    param.Add("@NewData", newData);
                    param.Add("@UpdatedDate", DateTime.Now);
                    param.Add("@CareTakerName", data.CareTakerName);
                    param.Add("@LogId_OUT", DbType.Int32, direction: ParameterDirection.Output);
                    param.Add("@ClientId", data.ClientId);
                    int resultLog = SqlMapper.ExecuteAsync(_dbConnection, auditSp, param, transaction, commandType: CommandType.StoredProcedure).Result;
                    transaction.Commit();
                    return message;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to GetSchdeulingLogDetaildById.");
                throw ex;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public Task<int> UpdateClientInvoice(InvoiceSearchInpts searchInpts)
        {

            try
            {
                _connectionFactory.OpenConnection();
                int result;
                var query = "SpUpdateClientInvoice";
                var param = new DynamicParameters();
                param.Add("@ClientId", searchInpts.ClientId);
                param.Add("@InvoiceId", searchInpts.InvoiceSearchInputId);
                param.Add("@PdfFilePath", searchInpts.PdfFilePath);
                result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "update failed");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public Task<int> GetClientEmailStatus(int clientId)
        {

            try
            {
                _connectionFactory.OpenConnection();
                int result;
                var query = "SpGetClientEmailStatus";
                var param = new DynamicParameters();
                param.Add("@ClientId", clientId);
                result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Getting Emailstatus failed");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
    }
}
