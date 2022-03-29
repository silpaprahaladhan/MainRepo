using Dapper;
using Nirast.Pcms.Api.Data.Helpers;
using Nirast.Pcms.Api.Sdk.Entities;
using Nirast.Pcms.Api.Sdk.Infrastructure;
using Nirast.Pcms.Api.Sdk.Logger;
using Nirast.Pcms.Api.Sdk.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Data.Repositories
{
    public class CareTakerRepository : GenericRepository<CareTakerRegistrationModel>, ICareTakerRepository
    {
        IConnectionFactory _connectionFactory;
        IPCMSLogger _logger;
        IDbConnection _dbConnection;
        INotificationService _notificationService;
        /// <summary>
        /// Initializes a new instance of the <see cref="CareTakerRepository"/> class.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        /// <param name="logger">The logger.</param>
        public CareTakerRepository(IConnectionFactory connectionFactory, IPCMSLogger logger, INotificationService notificationService)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _dbConnection = connectionFactory.GetConnection();
            _notificationService = notificationService;
        }

        /// <summary>
        /// Converts to data table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data.</param>
        /// <returns></returns>
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

        public async Task<int> SaveCareTakerPayRise(List<CareTakerServices> careTaker)
        {
            IDbTransaction transaction = null;
            try
            {
                _connectionFactory.OpenConnection();
                using (transaction = _connectionFactory.BeginTransaction(_dbConnection))
                {
                    var param = new DynamicParameters();
                    var query = "SpInsertUpdateCaretakersPayRise";
                    //DataColumn dtColMapClient = new DataColumn("ClientId", typeof(Int32))
                    //{
                    //    DefaultValue = careTaker.CareTakerId
                    //};
                    DataColumn dtColPayRiseId = new DataColumn("BookingPayRiseId", typeof(Int32))
                    {
                        DefaultValue = 0
                    };
                    DataTable dtMapRates = ConvertToDataTable(careTaker);
                    dtMapRates.Columns.Remove("ServiceName");
                    dtMapRates.Columns.Remove("EffectiveFrom");
                    // dtMapRates.Columns.Add(dtColMapClient);
                    dtMapRates.Columns.Add(dtColPayRiseId);
                    param.Add("@CaretakerId", careTaker[0].CareTakerId);
                    param.Add("@MappedRate", dtMapRates, DbType.Object);
                    param.Add("@Payrisedate", careTaker[0].EffectiveFrom);
                    int resultCareTakerMultiDetails = SqlMapper.QueryAsync<int>(_dbConnection, query, param, transaction, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                    transaction.Commit();
                    return resultCareTakerMultiDetails;
                }


            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to save Caregiver details");
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                return await Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<IEnumerable<CareTakerServices>> GetCaretakerPayRiseRates(int caretakerId)
        {
            var query = "SpGetCaretakerBookingPayRiseRates";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@CaretakerId", caretakerId);

                var result = await SqlMapper.QueryAsync<CareTakerServices>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);

                return result;

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get caretakers service details.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }

        }

        public async Task<IEnumerable<CareTakerServices>> GetCaretakerPayRiseRatesonDateChange(DateTime date, int caretakerId)
        {
            var query = "SpGetCaretakerBookingPayRiseRatesByDate";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@date", date);
                param.Add("@CaretakerId", caretakerId);

                var result = await SqlMapper.QueryAsync<CareTakerServices>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);

                return result;

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get caretakers service details.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// Adds the care taker.
        /// </summary>
        /// <param name="careTaker">The care taker.</param>
        /// <returns></returns>
        public async Task<int> AddCareTaker(CareTakerRegistrationModel careTaker)
        {
            IDbTransaction transaction = null;
            try
            {
                _connectionFactory.OpenConnection();
                using (transaction = _connectionFactory.BeginTransaction(_dbConnection))
                {
                    var saveUsersDetails = "SpInsertUpdateUserDetails";
                    var param = new DynamicParameters();
                    int newUserId = 0;
                    if (careTaker.UserId != 0)
                    {
                        param.Add("@Id", careTaker.UserId);
                        newUserId = careTaker.UserId;
                    }
                    param.Add("@FirstName", careTaker.FirstName);
                    param.Add("@LastName", careTaker.LastName);
                    if (careTaker.ProfilePicPath != null)
                    {
                        param.Add("@ProfilePicPath", careTaker.ProfilePicPath);
                    }
                    param.Add("@GenderId", careTaker.GenderId);
                    param.Add("@Dob", null);
                    param.Add("@Location", careTaker.Location);
                    param.Add("@CityId", careTaker.CityId);
                    param.Add("@Housename", careTaker.HouseName);
                    param.Add("@ZipCode", careTaker.ZipCode);
                    param.Add("@BranchId", careTaker.branchId1);
                    param.Add("@EmployeeNo", careTaker.EmployeeNumber);
                    param.Add("@UserTypeId", careTaker.UserTypeId);
                    param.Add("@PrimaryPhoneNo", careTaker.PrimaryPhoneNo);
                    param.Add("@SecondaryPhoneNo", careTaker.SecondaryPhoneNo);
                    param.Add("@EmailAddress", careTaker.EmailAddress);
                    param.Add("@Password", careTaker.Password);
                    param.Add("@UserVerified", false);
                    param.Add("@UserStatus", careTaker.UserStatus);
                    param.Add("@UserID_OUT", DbType.Int32, direction: ParameterDirection.Output);
                    int result = SqlMapper.ExecuteAsync(_dbConnection, saveUsersDetails, param, transaction, commandType: CommandType.StoredProcedure).Result;
                    var userId = param.Get<int>("@UserID_OUT");

                    careTaker.UserId = userId;
                    param = new DynamicParameters();
                    var saveCareTakerDetails = "SpInsertUpdateCaretakerDetails";
                    if (newUserId != 0)
                        param.Add("@Id", careTaker.UserId);
                    param.Add("@CaretakerUserId", careTaker.UserId);
                    param.Add("@CareTakerTypeId", (careTaker.CategoryId > 0) ? careTaker.CategoryId : (int?)null);
                    param.Add("@ProfileId", careTaker.CaretakerProfileId);
                    param.Add("@SSID", careTaker.SSID);
                    param.Add("@TotalExperience", careTaker.TotalExperience);
                    param.Add("@KeySkills", careTaker.KeySkills);
                    param.Add("@IsPrivate", careTaker.IsPrivate);
                    param.Add("@AboutMe", careTaker.AboutMe);
                    if (careTaker.ConsentDocPath != null)
                    {
                        param.Add("@ConsentDocPath", careTaker.ConsentDocPath);
                    }
                    param.Add("@CaretakerDetailId_OUT", DbType.Int32, direction: ParameterDirection.Output);
                    int resultCareTakerDetails = SqlMapper.QueryAsync<int>(_dbConnection, saveCareTakerDetails, param, transaction, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                    careTaker.CaretakerDetailId = param.Get<int>("@CaretakerDetailId_OUT");

                    if (careTaker.CareTakerExperiences != null)
                    {
                        System.Data.DataColumn dtColCaretakerExperienceUserId = new System.Data.DataColumn("CareTakerUserId", typeof(System.Int32));
                        dtColCaretakerExperienceUserId.DefaultValue = careTaker.UserId;
                        DataTable dtCareTakerExperiences = ConvertToDataTable(careTaker.CareTakerExperiences);
                        dtCareTakerExperiences.Columns.Add(dtColCaretakerExperienceUserId);


                        System.Data.DataColumn dtColCaretakerQualificationUserId = new System.Data.DataColumn("CareTakerUserId", typeof(System.Int32));
                        dtColCaretakerQualificationUserId.DefaultValue = careTaker.UserId;
                        DataTable dtCareTakerQualifications = ConvertToDataTable(careTaker.CareTakerQualifications);
                        dtCareTakerQualifications.Columns.Add(dtColCaretakerQualificationUserId);
                        dtCareTakerQualifications.Columns.Remove("QualificationName");
                        dtCareTakerQualifications.Columns.Remove("CareTakerQualificationId");

                        System.Data.DataColumn dtColCaretakerQualificationCareTakerId = new System.Data.DataColumn("CareTakerUserId", typeof(System.Int32));
                        dtColCaretakerQualificationCareTakerId.DefaultValue = careTaker.UserId;
                        DataTable dtCareTakerQualificationOthers = ConvertToDataTable(careTaker.CareTakerQualifications);
                        dtCareTakerQualificationOthers.Columns.Add(dtColCaretakerQualificationCareTakerId);
                        dtCareTakerQualificationOthers.Columns.Remove("CareTakerQualificationId");



                        DataTable dtCareTakerServices = ConvertToDataTable(careTaker.CareTakerServices);
                        dtCareTakerServices.Columns.Remove("ServiceName");



                        dtCareTakerServices.Columns.Remove("EffectiveFrom");
                        DataColumn dtColPayRiseId = new DataColumn("BookingPayRiseId", typeof(Int32))
                        {
                            DefaultValue = 0
                        };
                        dtCareTakerServices.Columns.Add(dtColPayRiseId);
                        System.Data.DataColumn dtColCaretakerDocumentUserId = new System.Data.DataColumn("CareTakerUserId", typeof(System.Int32));
                        dtColCaretakerDocumentUserId.DefaultValue = careTaker.UserId;
                        DataTable dtCareTakerDocuments = ConvertToDataTable(careTaker.CareTakerDocuments);
                        dtCareTakerDocuments.Columns.Add(dtColCaretakerDocumentUserId);
                        dtCareTakerDocuments.Columns.Remove("DocumentType");
                        dtCareTakerDocuments.Columns.Remove("CaretakerDocumentId");
                        foreach (DataRow item in dtCareTakerDocuments.Rows)
                        {
                            _logger.Info(item[0].ToString() + " -- ");
                            _logger.Info(item[1].ToString());
                        }
                        _logger.Info("Line 272");
                        param = new DynamicParameters();
                        var _spSaveCareTakerMultipleDetails = "spSaveCaretakerMultipleDetails";
                        param.Add("@CaretakerUserId", careTaker.UserId);
                        param.Add("@CaretakerExperience", dtCareTakerExperiences, DbType.Object);
                        param.Add("@CaretakerQualifications", dtCareTakerQualifications, DbType.Object);
                        param.Add("@CaretakerServices", dtCareTakerServices, DbType.Object);
                        param.Add("@CaretakerDocuments", dtCareTakerDocuments, DbType.Object);
                        param.Add("@CaretakerQualificationOthers", dtCareTakerQualificationOthers, DbType.Object);
                        param.Add("@Payrisedate", careTaker.CareTakerServices[0].EffectiveFrom);
                        int resultCareTakerMultiDetails = SqlMapper.QueryAsync<int>(_dbConnection, _spSaveCareTakerMultipleDetails, param, transaction, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                        _logger.Info("Line 283");
                    }
                    transaction.Commit();
                    return userId;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to save Caregiver details");
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                return await Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// Retrieves the caretaker details.
        /// </summary>
        /// <param name="caretakerUserId">The caretaker user identifier.</param>
        /// <returns></returns>
        public async Task<CareTakerRegistrationModel> RetrieveCaretakerDetails(int caretakerUserId)
        {
            IDbTransaction transaction = null;
            try
            {
                _connectionFactory.OpenConnection();
                using (transaction = _connectionFactory.BeginTransaction(_dbConnection))
                {
                    CareTakerRegistrationModel resultModel = new CareTakerRegistrationModel();
                    var query = "SpSelectCaretakerDetails";
                    var param = new DynamicParameters();
                    param.Add("@CaretakerUserId", caretakerUserId);

                    using (var multi = await _dbConnection.QueryMultipleAsync(query, param, transaction, commandType: CommandType.StoredProcedure))
                    {
                        var result = multi.Read<CareTakerRegistrationModel>().FirstOrDefault();
                        var experiences = multi.Read<CareTakerExperiences>().ToList();
                        var qualifications = multi.Read<CareTakerQualifications>().ToList();
                        var services = multi.Read<CareTakerServices>().ToList();
                        var documentList = multi.Read<DocumentsList>().ToList();
                        if (result != null)
                        {
                            result.CareTakerExperiences = experiences;
                            result.CareTakerQualifications = qualifications;
                            result.CareTakerServices = services;
                            result.CareTakerDocuments = documentList;
                        }
                        resultModel = result;
                    }
                    return (resultModel);
                }
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

        public async Task<IEnumerable<CareTakers>> RetrieveCareTakerListForDdl()
        {
            try
            {
                _connectionFactory.OpenConnection();
                var Details = "SpGetCareTakersDdl";
                var result = await SqlMapper.QueryAsync<CareTakers>(_dbConnection, Details, commandType: CommandType.StoredProcedure);
                return result;

                //_connectionFactory.OpenConnection();                
                //var param = new DynamicParameters();
                //var result = _dbConnection.Query<CareTakers>("SpGetCareTakersDdl", null, commandType: CommandType.StoredProcedure).ToList();
                //return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get caretakers list.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }

        }

        public async Task<IEnumerable<CareTakers>> RetrieveCareTakerListForDdlByLocation(LocationSearchInputs inputs)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var Details = "SpGetCareTakersDdlByLocation";
                var param = new DynamicParameters();
                param.Add("@CountryId", inputs.CountryId);
                param.Add("@StateId", inputs.StateId);
                param.Add("@CityId", inputs.CityId);
                var result = await SqlMapper.QueryAsync<CareTakers>(_dbConnection, Details,param, commandType: CommandType.StoredProcedure);
                return result;

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get caretakers list.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }

        }
        public async Task<IEnumerable<ClientScheduling>> GetSchedulingDetails(int caretakerId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var param = new DynamicParameters();
                var Details = "SpGetSchedulingDetails";
                param.Add("@caretakerId", caretakerId);
                var result = await SqlMapper.QueryAsync<ClientScheduling>(_dbConnection, Details, param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get scheduling details.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<IEnumerable<UserBooking>> GetUserBookingDetails(CaretakerScheduleListSearch caretakerBookingListSearch)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var param = new DynamicParameters();
                var Details = "SpGetBookingDetails";
                param.Add("@caretakerId", caretakerBookingListSearch.CaretakerId);
                param.Add("@FromDate", caretakerBookingListSearch.FromDate);
                param.Add("@ToDate", caretakerBookingListSearch.ToDate);
                var result = await SqlMapper.QueryAsync<UserBooking>(_dbConnection, Details, param, commandType: CommandType.StoredProcedure);
                return result;
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

        public async Task<IEnumerable<Notification>> GetNotification(int caretakerId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var param = new DynamicParameters();
                var Details = "SpGetNotification";
                param.Add("@caretakerId", caretakerId);
                var result = await SqlMapper.QueryAsync<Notification>(_dbConnection, Details, param, commandType: CommandType.StoredProcedure);
                return result;
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

        public async Task<NotificationDetails> GetNotificationDetailsById(int bookingId)
        {
            try
            {
                var param = new DynamicParameters();
                _connectionFactory.OpenConnection();
                var Details = "SpGetNotificationById";
                param.Add("@BookingId", bookingId);
                var result = await SqlMapper.QueryFirstOrDefaultAsync<NotificationDetails>(_dbConnection, Details, param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Notification Details.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }


        public async Task<UpcomingAppointment> GetUpcomingNotifications(int caretakerId)
        {
            try
            {
                var param = new DynamicParameters();
                _connectionFactory.OpenConnection();
                var Details = "SpGetUpcomingNotification";
                param.Add("@CaretakerId", caretakerId);
                var result = await SqlMapper.QueryFirstOrDefaultAsync<UpcomingAppointment>(_dbConnection, Details, param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Notification Details.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public async Task<IEnumerable<CareTakers>> RetrieveCareTakerListByCategory(int CategoryId)
        {
            var sp = "SpGetCareTakersByCareTakerType";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@CareTakerTypeId", CategoryId);
                var result = await SqlMapper.QueryAsync<CareTakers>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get caretakers details.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        
        public async Task<IEnumerable<CareTakers>> RetrieveCareTakerListByCategoryAndLocation(int CategoryId, LocationSearchInputs inputs)
        {
            var sp = "SpGetCareTakersByCareTakerTypeAndLocation";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@CareTakerTypeId", CategoryId);
                param.Add("@CountryId", inputs.CountryId);
                param.Add("@StateId", inputs.StateId);
                param.Add("@CityId", inputs.CityId);
                var result = await SqlMapper.QueryAsync<CareTakers>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get caretakers details.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<IEnumerable<CareTakers>> RetrieveCareTakerListByService(int ServiceId)
        {
            var sp = "SpGetCareTakersByServiceType";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@ServiceId", ServiceId);
                var result = await SqlMapper.QueryAsync<CareTakers>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get caretakers details.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<IEnumerable<CareTakers>> RetrieveCareTakerListByCategoryAndClientId(int CategoryId, int ClientId)
        {
            var sp = "SpGetCareTakersByCareTakerTypeAndClientId";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@CareTakerTypeId", CategoryId);
                param.Add("@ClientId", ClientId);
                var result = await SqlMapper.QueryAsync<CareTakers>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get caretakers details.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<IEnumerable<CareTakers>> RetrieveCareTakerListByCategoryAndDate(int categoryId, string startDateTime, int hours, int clientId)
        {
            var sp = "SpGetCareTakersByCareTakerTypeAndDateTime";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@CareTakerTypeId", categoryId);
                param.Add("@start", startDateTime);
                param.Add("@hours", hours);
                param.Add("@clientId", clientId);
                var result = await SqlMapper.QueryAsync<CareTakers>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get caretakers details.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public async Task<IEnumerable<CareTakers>> RetrieveAvailableCareTakerListByCategoryAndDate(int categoryId, string startDateTime, int hours, int clientId, int Workshift)
        {
            var sp = "SpGetAvailableCareTakersByCareTakerTypeAndDate";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@CareTakerTypeId", categoryId);
                param.Add("@start", startDateTime);
                param.Add("@clientId", clientId);
                param.Add("@hours", hours);
                param.Add("@Workshift", Workshift);
                _logger.Error("DB start " + DateTime.Now.ToString() + "SpGetAvailableCareTakersByCareTakerTypeAndDate -" + categoryId + " - " + startDateTime + " - " + hours + " - " + clientId + " - " + Workshift);
                var result = await SqlMapper.QueryAsync<CareTakers>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
                _logger.Error("DB end " + DateTime.Now.ToString() + "SpGetAvailableCareTakersByCareTakerTypeAndDate -" + categoryId + " - " + startDateTime + " - " + hours + " - " + clientId + " - " + Workshift);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get caretakers details.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public async Task<IEnumerable<CareTakers>> RetrieveAvailableCareTakerListForPublicUser(int categoryId, string startDateTime, int hours, int Workshift)
        {
            var sp = "spSelectCaretakersForPublicUserScheduling";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@CareTakerTypeId", categoryId);
                param.Add("@start", startDateTime);
                param.Add("@hours", hours);
                param.Add("@Workshift", Workshift);
                _logger.Error("DB start " + DateTime.Now.ToString() + "SpGetAvailableCareTakersByCareTakerTypeAndDate -" + categoryId + " - " + startDateTime + " - " + hours + " - " + Workshift);
                var result = await SqlMapper.QueryAsync<CareTakers>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
                _logger.Error("DB end " + DateTime.Now.ToString() + "SpGetAvailableCareTakersByCareTakerTypeAndDate -" + categoryId + " - " + startDateTime + " - " + hours + " - " + Workshift);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get caretakers details.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<IEnumerable<CaretakerAvailableReport>> RetrieveAvailableCareTakerListReport(PaymentAdvancedSearch inputs)
        {
            var sp = "SpGetCaretakerAvialablity";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@FromDate", inputs.FromDate);
                param.Add("@ToDate", inputs.ToDate);
                param.Add("@ClientId", inputs.ClientId);
                
                var result = await SqlMapper.QueryAsync<CaretakerAvailableReport>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get caretakers details.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<IEnumerable<CaretakerAvailableReport>> RetrieveCommissionReport(PaymentAdvancedSearch inputs)
        {
            var sp = "SpGetCommission";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@FromDate", inputs.FromDate);
                param.Add("@ToDate", inputs.ToDate);
                param.Add("@ClientId", inputs.ClientId);
                param.Add("@branch", inputs.branch);
                var result = await SqlMapper.QueryAsync<CaretakerAvailableReport>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get caretakers details.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }


        /// <summary>
        /// Caretakers the profile identifier.
        /// </summary>
        /// <returns></returns>
        public async Task<string> CaretakerProfileId()
        {
            try
            {
                _connectionFactory.OpenConnection();
                string ProfileId = await _dbConnection.QueryFirstOrDefaultAsync<string>("spSelectCareTakerProfileId", null, commandType: CommandType.StoredProcedure);
                return ProfileId;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Caregiver profile id");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<IEnumerable<CareTakerRegistrationModel>> SelectRegisteredCaretakers(int status)
        {
            var sp = "SpSelectCaretakersByCaretakerStatus";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@CaretakerStatus", status);

                IEnumerable<CareTakerRegistrationModel> result = await SqlMapper.QueryAsync<CareTakerRegistrationModel>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);

                foreach (CareTakerRegistrationModel caretaker in result)
                {
                    param = new DynamicParameters();
                    sp = "SpSelectCaretakerServices";
                    param.Add("@CaretakerUserId", caretaker.UserId);
                    IEnumerable<CareTakerServices> careTakerServices = await SqlMapper.QueryAsync<CareTakerServices>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
                    caretaker.CareTakerServices = careTakerServices.AsList();
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get caretakers details.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public async Task<IEnumerable<CareTakerRegistrationModel>> SelectRegisteredCaretakersByLocation(int status,LocationSearchInputs inputs)
        {
            var sp = "SpSelectCaretakersByCaretakerStatus";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@CaretakerStatus", status);
                param.Add("@CountryId", inputs.CountryId);
                param.Add("@StateId", inputs.StateId);
                param.Add("@CityId", inputs.CityId);

                IEnumerable<CareTakerRegistrationModel> result = await SqlMapper.QueryAsync<CareTakerRegistrationModel>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);

                foreach (CareTakerRegistrationModel caretaker in result)
                {
                    param = new DynamicParameters();
                    sp = "SpSelectCaretakerServices";
                    param.Add("@CaretakerUserId", caretaker.UserId);
                    IEnumerable<CareTakerServices> careTakerServices = await SqlMapper.QueryAsync<CareTakerServices>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
                    caretaker.CareTakerServices = careTakerServices.AsList();
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get caretakers details.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public async Task<IEnumerable<CareTakerServices>> RetrieveCaregiverServices()
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpRetrieveAllCaregiverServices";
                var param = new DynamicParameters();
              
                var result = await SqlMapper.QueryAsync<CareTakerServices>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return result;

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve category");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<IEnumerable<CareTakerRegistrationModel>> SelectCaretakers(int status)
        {
            var sp = "SpSelectCaretakersByCaretakerStatus";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@CaretakerStatus", status);

                var result = await SqlMapper.QueryAsync<CareTakerRegistrationModel>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get caretakers details.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }


        public async Task<int> RejectCaretakerApplication(RejectCareTaker rejectCareTaker)
        {
            try
            {
                _connectionFactory.OpenConnection();
                int result;
                var query = "SpRejectCaretakerApplication";
                var param = new DynamicParameters();
                param.Add("@UserRegnId", rejectCareTaker.Userid);
                result = await SqlMapper.QueryFirstOrDefaultAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return result;

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to reject Caregiver application.");
                return await Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<int> DeleteCaretaker(int UserRegnId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                int result;
                var query = "SpDeleteCaretaker";
                var param = new DynamicParameters();
                param.Add("@UserRegnId", UserRegnId);
                result = await SqlMapper.QueryFirstOrDefaultAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, "Error while deleting a Caregiver");
                return await Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<int> ApproveCaretaker(ApproveCaretaker approveCaretaker)
        {
            IDbTransaction transaction = null;
            int result;
            try
            {
                _connectionFactory.OpenConnection();
                System.Data.DataColumn dtColCaretakerServiceUserId = new System.Data.DataColumn("CareTakerUserId", typeof(System.Int32));
                dtColCaretakerServiceUserId.DefaultValue = approveCaretaker.CareTakerId;
                DataTable dtCareTakerServices = ConvertToDataTable(approveCaretaker.ApprovedServiceRates);
                dtCareTakerServices.Columns.Add(dtColCaretakerServiceUserId);
                dtCareTakerServices.Columns.Remove("ServiceName");
                dtCareTakerServices.Columns.Remove("EffectiveFrom");

                var query = "SpApproveCaretaker";
                var param = new DynamicParameters();
                param.Add("@UserId", approveCaretaker.CareTakerId);
                param.Add("@IsPrivate", approveCaretaker.IsPrivate);
                param.Add("@CaretakerServices", dtCareTakerServices, DbType.Object);
                param.Add("@Payrisedate", approveCaretaker.ApprovedServiceRates[0].EffectiveFrom);
                result = await SqlMapper.QueryFirstOrDefaultAsync<int>(_dbConnection, query, param, transaction, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<int> ChangeUserStatus(int UserRegnId, int AccountStatus)
        {
            try
            {
                _connectionFactory.OpenConnection();
                int result;
                var query = "SpChangeUserStatus";
                var param = new DynamicParameters();
                param.Add("@Status", AccountStatus);
                param.Add("@UserRegnId", UserRegnId);
                result = await SqlMapper.QueryFirstOrDefaultAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to change user status.");
                return await Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// Update Scheduling Details
        /// </summary>
        /// <param name="usersDetails">Update Scheduling Details.</param>
        /// <returns></returns>
        public async Task<int> ConfirmAppointments(UpcomingAppointment upcomingAppointment)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var sp = "SpConfirmAppointments";
                var param = new DynamicParameters();
                param.Add("@Id", upcomingAppointment.AppointmentId);
                param.Add("@Type", upcomingAppointment.Type);
                int result = SqlMapper.QueryAsync<int>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return await Task.FromResult(1);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to update card ");
                return await Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<UpcomingAppointment> GetAppointmentDetails(UpcomingAppointment upcomingAppointment)
        {

            var sp = "SpGetAppointmentDetails";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@ID", upcomingAppointment.AppointmentId);
                param.Add("@Type", upcomingAppointment.Type);
                var result = await SqlMapper.QueryFirstOrDefaultAsync<UpcomingAppointment>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to GetAppointmentDetails.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// Method to Get EmailId For Admin
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetEmailIdForAdmin()
        {

            var sp = "SpGetEmailIdForAdmin";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@UserTypeID", 4);

                var emaiilId = await SqlMapper.ExecuteScalarAsync(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
                return emaiilId.ToString();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to GetEmailIdForAdmin.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// Method to get Get EmailId For OfficeStaff
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetEmailIdForOfficeStaff()
        {
            List<string> ccAddressList = new List<string>();
            var sp = "SpGetEmailIdForAdmin";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@UserTypeID", 5);

                var userDetails = await SqlMapper.QueryAsync<UsersDetails>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
                if (userDetails.ToList().Count > 0)
                {
                    foreach (var item in userDetails)
                    {
                        ccAddressList.Add(item.EmailAddress);
                    }
                }
                return ccAddressList;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to GetEmailIdForOfficeStaff.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// Method to get Get EmailId For User
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<string> GetEmailIdForUser(int userId)
        {
            List<string> ccAddressList = new List<string>();
            var sp = "SpGetEmailIdForUserId";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@UserID", userId);

                var emailId = await SqlMapper.ExecuteScalarAsync(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
                return emailId.ToString();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to GetEmailIdForOfficeStaff.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<IEnumerable<CaretakerScheduleList>> GetCaretakerScheduleList(CaretakerScheduleListSearch caretakerScheduleListSearch)
        {
            var sp = "spSelectCaretakerScheduleList";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@CaratakerUserId", caretakerScheduleListSearch.CaretakerId);
                param.Add("@ClientId", caretakerScheduleListSearch.ClientId);
                param.Add("@FromDate", caretakerScheduleListSearch.FromDate);
                param.Add("@ToDate", caretakerScheduleListSearch.ToDate);

                var result = await SqlMapper.QueryAsync<CaretakerScheduleList>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get caretakers schedule details.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<IEnumerable<CaretakerType>> GetCaretakerType()
        {
            var sp = "SpSelectCaretakerType";
            try
            {
                _connectionFactory.OpenConnection();
                var result = await SqlMapper.QueryAsync<CaretakerType>(_dbConnection, sp, null, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get caretakers type details.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
    }
}
