using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Nirast.Pcms.Api.Sdk.Entities;
using Nirast.Pcms.Api.Sdk.Infrastructure;
using Nirast.Pcms.Api.Sdk.Logger;
using Nirast.Pcms.Api.Sdk.Repositories;
using static Nirast.Pcms.Api.Sdk.Entities.Enums;

namespace Nirast.Pcms.Api.Data.Repositories
{
    public class OfficeStaffRepository : GenericRepository<OfficeStaffRegistration>, IOfficeStaffRepository
    {
        IConnectionFactory _connectionFactory;
        IPCMSLogger _logger;
        IDbConnection _dbConnection;
        INotificationService _notificationService;
        /// <summary>
        /// Initializes a new instance of the <see cref="OfficeStaffRepository"/> class.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        /// <param name="logger">The logger.</param>
        public OfficeStaffRepository(IConnectionFactory connectionFactory, IPCMSLogger logger, INotificationService notificationService)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _dbConnection = connectionFactory.GetConnection();
            _notificationService = notificationService;
        }
        #region public methods

        /// <summary>
        /// method to add office staff
        /// </summary>
        /// <param name="officeStaff"></param>
        /// <returns></returns>
        public async Task<int> AddOfficeStaff(OfficeStaffRegistration officeStaff)
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
                    if (officeStaff.UserId != 0)
                    {
                        param.Add("@Id", officeStaff.UserId);
                        newUserId = officeStaff.UserId;
                    }
                    param.Add("@FirstName", officeStaff.FirstName);
                    param.Add("@LastName", officeStaff.LastName);
                    if (officeStaff.ProfilePicPath != null)
                    {
                        param.Add("@ProfilePic", officeStaff.ProfilePicPath);
                    }
                    param.Add("@EmployeeNo", officeStaff.EmployeeNo);
                    param.Add("@BranchId", officeStaff.BranchId1);
                    param.Add("@GenderId", officeStaff.GenderId);
                    param.Add("@Dob", null);
                    param.Add("@Location", officeStaff.Location);
                    param.Add("@CityId", officeStaff.CityId);
                    param.Add("@Housename", officeStaff.HouseName);
                    param.Add("@ZipCode", officeStaff.ZipCode);
                    param.Add("@UserTypeId", officeStaff.UserTypeId);
                    param.Add("@PrimaryPhoneNo", officeStaff.PrimaryPhoneNo);
                    param.Add("@SecondaryPhoneNo", officeStaff.SecondaryPhoneNo);
                    param.Add("@EmailAddress", officeStaff.EmailAddress);
                    param.Add("@Password", officeStaff.Password);
                    param.Add("@UserVerified", false);
                    param.Add("@UserStatus", officeStaff.UserStatus);
                    param.Add("@UserID_OUT", DbType.Int32, direction: ParameterDirection.Output);
                    int result = SqlMapper.ExecuteAsync(_dbConnection, saveUsersDetails, param, transaction, commandType: CommandType.StoredProcedure).Result;
                    var userId = param.Get<int>("@UserID_OUT");

                    officeStaff.UserId = userId;
                    var param1 = new DynamicParameters();
                    var saveOfficeStaffDetails = "SpInsertOfficeStaffRegistration";
                    if (newUserId != 0)
                        param1.Add("@Id", userId);
                    param1.Add("@DesignationId", officeStaff.DesignationId);
                    param1.Add("@RoleId", officeStaff.RoleId);
                    param1.Add("@UserId", officeStaff.UserId);
                    param1.Add("@QualificationId", officeStaff.QualificationId);
                    int registrationResult = await SqlMapper.ExecuteScalarAsync<int>(_dbConnection, saveOfficeStaffDetails, param1, transaction, commandType: CommandType.StoredProcedure);

                    //save officeStaff WorkRole
                    var saveOfficeStaffRole = "SpInsertUpdateOfficeStaff_TestRole";
                    var param2 = new DynamicParameters();
                    param2.Add("@WorkRoleTableId", officeStaff.WorkRoleTableId);
                    param2.Add("@UserId", officeStaff.UserId);
                    param2.Add("@WorkRoleId", officeStaff.Stafftype);
                    param2.Add("@CountryId", officeStaff.CountryId1);
                    param2.Add("@StateId", officeStaff.StateId1);
                    param2.Add("@CityId", officeStaff.CityId1);
                    param2.Add("@BranchId", officeStaff.BranchId1);
                    int officeResult = await SqlMapper.ExecuteScalarAsync<int>(_dbConnection, saveOfficeStaffRole, param2, transaction, commandType: CommandType.StoredProcedure);

                    transaction.Commit();
                    return await Task.FromResult(userId);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while save office staff details details");
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
        /// method to get office staff profile
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<OfficeStaffRegistration> GetOfficeStaffDetails(int id)
        {
            OfficeStaffRegistration officeRegistration = new OfficeStaffRegistration();
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                var staffDetails = "SpGetOfficeStaffDetails";
                officeRegistration.UserRegnId = id;
                param.Add("@UserRegnId", officeRegistration.UserRegnId);
                var result = await SqlMapper.QueryFirstOrDefaultAsync<OfficeStaffRegistration>(_dbConnection, staffDetails, param, commandType: CommandType.StoredProcedure);
                return result;
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
        /// method to delete office staff details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<int> DeleteOfficeStaffDetails(int id)
        {
            IDbTransaction transaction = null;
            try
            {
                _connectionFactory.OpenConnection();
                using (transaction = _connectionFactory.BeginTransaction(_dbConnection))
                {
                    var staffParam = new DynamicParameters();
                    staffParam.Add("@UserId", id);
                    int staffResult = SqlMapper.ExecuteAsync(_dbConnection, "SpDeleteOfficeStaffDetails", staffParam, transaction, commandType: CommandType.StoredProcedure).Result;

                    var userParam = new DynamicParameters();
                    userParam.Add("@UserRegnId", id);
                    int UserResult = SqlMapper.ExecuteAsync(_dbConnection, "SpDeleteUserDetails", userParam, transaction, commandType: CommandType.StoredProcedure).Result;

                    transaction.Commit();
                    return Task.FromResult(1);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while deleting office staff details");
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

        /// <summary>
        /// method to get all staff details
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<OfficeStaffRegistration>> GetOfficeStaffDetails()
        {
            try
            {
                _connectionFactory.OpenConnection();
                var Details = "SpGetAllOfficeStaffDetails";
                var result = await SqlMapper.QueryAsync<OfficeStaffRegistration>(_dbConnection, Details, commandType: CommandType.StoredProcedure);
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
        /// method to get all staff details
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<OfficeStaffRegistration>> GetOfficeStaffDetailsByLocation(LocationSearchInputs inputs)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var Details = "SpGetAllOfficeStaffDetailsByLocation";
                var param = new DynamicParameters();
                param.Add("@CountryId", inputs.CountryId);
                param.Add("@StateId", inputs.StateId);
                param.Add("@CityId", inputs.CityId);
                var result = await SqlMapper.QueryAsync<OfficeStaffRegistration>(_dbConnection, Details, param, commandType: CommandType.StoredProcedure);
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
        #endregion
    }
}
