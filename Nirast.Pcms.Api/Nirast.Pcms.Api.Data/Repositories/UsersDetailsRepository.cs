using Dapper;
using Nirast.Pcms.Api.Data.Helpers;
using Nirast.Pcms.Api.Sdk.Entities;
using Nirast.Pcms.Api.Sdk.Infrastructure;
using Nirast.Pcms.Api.Sdk.Logger;
using Nirast.Pcms.Api.Sdk.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Nirast.Pcms.Api.Sdk.Entities.Enums;
using static Nirast.Pcms.Api.Sdk.Entities.PublicUserCaretakerBooking;

namespace Nirast.Pcms.Api.Data.Repositories
{
    public class UsersDetailsRepository : GenericRepository<PublicUserRegistration>, IUsersDetailsRepository
    {
        IConnectionFactory _connectionFactory;
        IPCMSLogger _logger;
        IDbConnection _dbConnection;
        INotificationService _notificationService;
        /// <summary>
        /// Initializes a new instance of the <see cref="UsersDetailsRepository"/> class.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        /// <param name="logger">The logger.</param>
        public UsersDetailsRepository(IConnectionFactory connectionFactory, IPCMSLogger logger, INotificationService notificationService)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _dbConnection = connectionFactory.GetConnection();
            _notificationService = notificationService;
        }
      
        public async Task<int> InsertUpdateCompanyDetails(CompanyProfile companyProfile)
        {
            IDbTransaction transaction = null;
            try
            {
                _connectionFactory.OpenConnection();
                using (transaction = _connectionFactory.BeginTransaction(_dbConnection))
                {
                    var sp = "spInsertUpdateCompanySettings";
                    var param = new DynamicParameters();
                    param.Add("@CompanyId", companyProfile.CompanyId);
                    param.Add("@CompanyName", companyProfile.CompanyName);
                    param.Add("@TagLine", companyProfile.TagLine);
                    param.Add("@Logo", companyProfile.Logo);
                    param.Add("@AddressLine", companyProfile.AddressLine);
                    param.Add("@CityId", companyProfile.CityId);
                    param.Add("@ZipCode", companyProfile.ZipCode);
                    param.Add("@PhoneNo1", companyProfile.PhoneNo1);
                    param.Add("@PhoneNo2", companyProfile.PhoneNo2);
                    param.Add("@Fax", companyProfile.Fax);
                    param.Add("@EmailAddress", companyProfile.EmailAddress);
                    param.Add("@Website", companyProfile.Website);
                    param.Add("@Description1", companyProfile.Description1);
                    param.Add("@Description2", companyProfile.Description2);
                    param.Add("@Description3", companyProfile.Description3);
                    param.Add("@EmailAddress", companyProfile.EmailAddress);
                    param.Add("@CompanyID_OUT", DbType.Int32, direction: ParameterDirection.Output);
                    var result = SqlMapper.ExecuteAsync(_dbConnection, sp, param, transaction, commandType: CommandType.StoredProcedure).Result;

                    transaction.Commit();

                    return await Task.FromResult(1);
                }
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                _logger.Error(ex, "Failed to create user");
                return await Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }


        public async Task<CompanyProfile> GetCompanyProfiles(int CompanyId)
        {
            var sp = "spSelectCompanySettings";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@CompanyId", CompanyId);
                var result = await SqlMapper.QueryFirstOrDefaultAsync<CompanyProfile>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get user details.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }

        }
        /// <summary>
        /// Edits the user profile.
        /// </summary>
        /// <param name="usersDetails">The users details.</param>
        /// <returns></returns>
        public async Task<int> EditUserProfile(PublicUserRegistration usersDetails)
        {
            IDbTransaction transaction = null;
            try
            {
                _connectionFactory.OpenConnection();
                using (transaction = _connectionFactory.BeginTransaction(_dbConnection))
                {
                    var sp = "SpInsertUpdateUserDetails";
                    var param = new DynamicParameters();
                    param.Add("@Id", usersDetails.UserRegnId);
                    param.Add("@FirstName", usersDetails.FirstName);
                    param.Add("@LastName", usersDetails.LastName);
                    if (usersDetails.ProfilePicPath != null)
                    {
                        param.Add("@ProfilePicPath", usersDetails.ProfilePicPath);
                    }
                    param.Add("@GenderId", usersDetails.GenderId);
                    param.Add("@Dob", null);
                    param.Add("@Location", usersDetails.Location);
                    param.Add("@CityId", usersDetails.CityId);
                    param.Add("@Housename", usersDetails.HouseName);
                    param.Add("@ZipCode", usersDetails.ZipCode);
                    param.Add("@UserTypeId", usersDetails.UserTypeId);
                    param.Add("@PrimaryPhoneNo", usersDetails.PrimaryPhoneNo);
                    param.Add("@SecondaryPhoneNo", usersDetails.SecondaryPhoneNo);
                    param.Add("@EmailAddress", usersDetails.EmailAddress);
                    param.Add("@Password", usersDetails.Password);
                    param.Add("@UserVerified", true);
                    param.Add("@UserStatus", usersDetails.UserStatus);
                    param.Add("@UserID_OUT", DbType.Int32, direction: ParameterDirection.Output);
                    var result = SqlMapper.ExecuteAsync(_dbConnection, sp, param, transaction, commandType: CommandType.StoredProcedure).Result;
                    if (usersDetails.ProfilePicPath != null)
                    {
                        var spPic = "SpUpdateUserProfilePic";
                        param = new DynamicParameters();
                        param.Add("@Id", usersDetails.UserRegnId);
                        param.Add("@ProfilePicPath", usersDetails.ProfilePicPath);
                        var resultprofileupdate = SqlMapper.QueryAsync<int>(_dbConnection, spPic, param, transaction, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                    }
                    transaction.Commit();


                    return await Task.FromResult(1);
                }
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                _logger.Error(ex, "Failed to create user");
                return await Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// Edits the card details.
        /// </summary>
        /// <param name="usersDetails">The users details.</param>
        /// <returns></returns>
        public async Task<int> EditCardDetails(PublicUserRegistration usersDetails)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var param = new DynamicParameters();
                var saveCardDetails = "SpInsertUpdateUserCardDetails";
                param.Add("@CardId", usersDetails.CardId);
                param.Add("@NameOnCard", usersDetails.NameOnCard);
                param.Add("@UserCard_CardNumber", usersDetails.UserCard_CardNumber);
                param.Add("@ExpiryMonth", usersDetails.ExpiryMonth);
                param.Add("@ExpiryYear", usersDetails.ExpiryYear);
                param.Add("@CardTypeId", usersDetails.CardTypeId);
                param.Add("@IsDefault", usersDetails.IsDefault);
                param.Add("@CreatedUser", usersDetails.UserRegnId);
                param.Add("@LastUpdatedUser", usersDetails.UserRegnId);
                var resultDetails = SqlMapper.QueryAsync<int>(_dbConnection, saveCardDetails, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return resultDetails;
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

        /// <summary>
        /// Adds the public user.
        /// </summary>
        /// <param name="usersDetails">The users details.</param>
        /// <returns></returns>
        public async Task<int> AddPublicUser(PublicUserRegistration usersDetails)
        {
            IDbTransaction transaction = null;
            try
            {
                _connectionFactory.OpenConnection();
                using (transaction = _connectionFactory.BeginTransaction(_dbConnection))
                {
                    var sp = "SpInsertUpdateUserDetails";
                    var param = new DynamicParameters();
                    //var saveCardDetails = "SpInsertUpdateUserCardDetails";
                    param.Add("@Id", usersDetails.UserRegnId);
                    param.Add("@FirstName", usersDetails.FirstName);
                    param.Add("@LastName", usersDetails.LastName);
                    param.Add("@EmployeeNo", usersDetails.EmployeeNumber);
                    param.Add("@BranchId", usersDetails.BranchId1);
                    if (usersDetails.ProfilePicPath != null)
                    {
                        param.Add("@ProfilePicPath", usersDetails.ProfilePicPath);
                    }
                    param.Add("@GenderId", usersDetails.GenderId);
                    param.Add("@Dob", null);
                    param.Add("@Location", usersDetails.Location);
                    param.Add("@Housename", usersDetails.HouseName);
                    param.Add("@CityId", (usersDetails.CityId > 0) ? usersDetails.CityId : (int?)null);
                    param.Add("@ZipCode", usersDetails.ZipCode);
                    param.Add("@UserTypeId", usersDetails.UserTypeId);
                    param.Add("@PrimaryPhoneNo", usersDetails.PrimaryPhoneNo);
                    param.Add("@SecondaryPhoneNo", usersDetails.SecondaryPhoneNo);
                    param.Add("@EmailAddress", usersDetails.EmailAddress);
                    param.Add("@Password", usersDetails.Password);
                    param.Add("@UserVerified", usersDetails.UserVerified);
                    param.Add("@UserStatus", usersDetails.UserStatus);
                    param.Add("@UserID_OUT", DbType.Int32, direction: ParameterDirection.Output);
                    int result = SqlMapper.ExecuteAsync(_dbConnection, sp, param, transaction, commandType: CommandType.StoredProcedure).Result;
                    var userId = param.Get<int>("@UserID_OUT");
                    transaction.Commit();
                    return await Task.FromResult(userId);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed create public user.");
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
        /// Updates the user verification.
        /// </summary>
        /// <param name="verifyUsers">The verify users.</param>
        /// <returns></returns>
        public async Task<int> UpdateUserVerification(VerifyUserAccount verifyUsers)
        {
            var spRecordActive = "spVerifyUserByUserId";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@UserRegistrationId", verifyUsers.UserId);
                param.Add("@UserVerified", verifyUsers.Verified);
                int result = SqlMapper.QueryAsync<int>(_dbConnection, spRecordActive, param, commandType: CommandType.StoredProcedure).Result.FirstOrDefault();
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to verify user.");
                return await Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public Task<int> UpdateUserInvoiceNumber(int userid,int invoiceNumber)
        {

            try
            {
                _connectionFactory.OpenConnection();
                int result;
                var query = "SpUpdateUserInvoiceNumber";
                var param = new DynamicParameters();
               
                param.Add("@InvoiceNumber", invoiceNumber);
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
        public Task<int> AddUserInvoiceDetails(InvoiceSearchInpts invoiceDetails)
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
                    var sp = "SpInsertUpdateUserInvoiceDetails";
                    var param = new DynamicParameters();
                    param.Add("@InvoiceDate", invoiceDetails.InvoiceDate);
                    param.Add("@InvoiceSearchId", invoiceDetails.InvoiceSearchInputId);
                    param.Add("@InvoiceNumber", invoiceDetails.InvoiceNumber);
                    param.Add("@InvoicePrefix", invoiceDetails.InvoicePrefix);
                    param.Add("@PublicUserId", invoiceDetails.PublicUserId);
                    param.Add("@StartDate", startDate);
                    param.Add("@EndDate", endDate);
                    param.Add("@Mode", (invoiceDetails.Mode == 0) ? null : invoiceDetails.Mode);
                    param.Add("@Year", (invoiceDetails.Year == 0) ? null : invoiceDetails.Year);
                    param.Add("@Month", (invoiceDetails.Month == 0) ? null : invoiceDetails.Month);
                    param.Add("@PdfFilePath", invoiceDetails.PdfFilePath);
                    param.Add("@InvoiceSearchId_Out", DbType.Int32, direction: ParameterDirection.Output);
                    result = SqlMapper.ExecuteAsync(_dbConnection, sp, param, transaction, commandType: CommandType.StoredProcedure).Result;
                    var userId = param.Get<int>("@InvoiceSearchId_Out");
                    transaction.Commit();
                    return Task.FromResult(userId);
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
        /// <summary>
        /// Gets the users details by identifier.
        /// </summary>
        /// <param name="flag">The flag.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public async Task<IEnumerable<PublicUserRegistration>> GetUsersDetailsById(string flag, string value)
        {
            var sp = "spSelectAllUserDetails";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@Flag", flag);
                param.Add("@Value", value);
                var result = await SqlMapper.QueryAsync<PublicUserRegistration>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get user details.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }

        }

        /// <summary>
        /// Gets the users details by identifier.
        /// </summary>
        /// <param name="flag">The flag.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public async Task<IEnumerable<PublicUserRegistration>> GetUsersDetailsByLocation(string flag, string value, LocationSearchInputs inputs)
        {
            var sp = "spSelectAllUserDetails";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@Flag", flag);
                param.Add("@Value", value);
                param.Add("@CountryId", inputs.CountryId);
                param.Add("@StateId", inputs.StateId);
                param.Add("@CityId", inputs.CityId);
                var result = await SqlMapper.QueryAsync<PublicUserRegistration>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get user details.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }

        }

        /// <summary>
        /// Deletes the user details.
        /// </summary>
        /// <param name="userStatus">The user status.</param>
        /// <returns></returns>
        public async Task<int> DeleteUserDetails(string userStatus)
        {
            var sp = "spDeleteUserDetails";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@UserStatus", userStatus);
                return SqlMapper.QueryAsync<int>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete user details.");
                return 0;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }

        }

        /// <summary>
        /// Updates the user profile pic.
        /// </summary>
        /// <param name="UsersDetails">The users details.</param>
        /// <returns></returns>
        public async Task<int> UpdateUserProfilePic(PublicUserRegistration UsersDetails)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var sp = "SpUpdateUserProfilePic";
                var param = new DynamicParameters();
                param.Add("@Id", UsersDetails.UserRegnId);
                param.Add("@ProfilePicPath", UsersDetails.ProfilePicPath);
                int result = SqlMapper.QueryAsync<int>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return await Task.FromResult(1);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to update profile pic.");
                return await Task.FromResult(0);
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
        public string CaretakerProfileId()
        {
            try
            {
                _connectionFactory.OpenConnection();
                string ProfileId = _dbConnection.Query<string>("spSelectCareTakerProfileId", null, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return ProfileId;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get caretakerprofile id.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// Checks the login name exist.
        /// </summary>
        /// <param name="LoginName">Name of the login.</param>
        /// <returns></returns>
        public async Task<int> CheckLoginNameExist(string LoginName)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var sp = "SpCheckEmailExists";
                var param = new DynamicParameters();
                param.Add("@EmailId", LoginName);
                int result = _dbConnection.Query<int>(sp, param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to check login Name.");
                return await Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// Edits the card details.
        /// </summary>
        /// <param name="usersDetails">The users details.</param>
        /// <returns></returns>
        public async Task<int> ChangeUserStatus(int userId, int status)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var sp = "SpChangeUserStatus";
                var param = new DynamicParameters();
                param.Add("@UserRegnId", userId);
                param.Add("@Status", status);
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
        public async Task<int> VerifyEmail(int userId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var sp = "SpVerifyEmail";
                var param = new DynamicParameters();
                param.Add("@UserRegnId", userId);
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


        public async Task<IEnumerable<UserBooking>> GetPublicUserBookingDetails(int publicuserId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var param = new DynamicParameters();
                var Details = "SpGetUserBookingDetails";
                param.Add("@PublicUserId", publicuserId);
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
        public async Task<UserBooking> GetPublicUserBookingDetailsById(int bookingId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var param = new DynamicParameters();
                var Details = "SpGetUserBookingDetailsById";
                param.Add("@BookingId", bookingId);
                var result = await SqlMapper.QueryFirstOrDefaultAsync<UserBooking>(_dbConnection, Details, param, commandType: CommandType.StoredProcedure);
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
        
        public async Task<IEnumerable<UserBooking>> GetCaretakerBookingDetails(CalenderEventInput calenderEventInput)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var param = new DynamicParameters();
                var Details = "SpGetCaretakerBookingDetails";
                param.Add("@CaretakerId", calenderEventInput.CaretakerId);
                param.Add("@StartDate", calenderEventInput.StartDate);
                param.Add("@EndDate", calenderEventInput.EndDate);
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
        



        public async Task<IEnumerable<PublicUserPaymentHistory>> GetPublicUserPaymentHistory(int publicuserId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var param = new DynamicParameters();
                var Details = "SpGetPaymentHistory";
                param.Add("@PublicUserId", publicuserId);
                var result = await SqlMapper.QueryAsync<PublicUserPaymentHistory>(_dbConnection, Details, param, commandType: CommandType.StoredProcedure);
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

        public async Task<IEnumerable<AdminBookingNotification>> GetPublicUserNotification(int publicuserId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var param = new DynamicParameters();
                var Details = "SpGetUserNotification";
                param.Add("@PublicUserId", publicuserId);
                var result = await SqlMapper.QueryAsync<AdminBookingNotification>(_dbConnection, Details, param, commandType: CommandType.StoredProcedure);
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

        public async Task<int> ForgotPassword(ForgotPasswordViewModel emailId)
        {
            try
            {
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

        public async Task<int> ChangePassword(ChangePassword changePasswordInputs)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var sp = "SpChangeUserPassword";
                var param = new DynamicParameters();
                param.Add("@EmailId", changePasswordInputs.EmailId);
                param.Add("@Password", changePasswordInputs.NewPassword);
                int result = await SqlMapper.QueryFirstOrDefaultAsync<int>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to update password.");
                return await Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<UserCredential> RetrievePassword(string emailId)
        {
            try
            {
                EmailInput inputs = new EmailInput();
                List<string> ccAddressList = new List<string>();

                //get maild id for admin typeid=4
                var sp = "SpSelectCurrPassword";
                var param = new DynamicParameters();
                param.Add("@EmailId", emailId);
                var user = await SqlMapper.QueryFirstOrDefaultAsync<UserCredential>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);

                return user;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to update card ");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }

        }
        public Task<int> AddInvoiceDetails(PublicUserPaymentInvoiceInfo InvoiceDetails)
        {
            IDbTransaction transaction = null;
            try
            {
                _connectionFactory.OpenConnection();
                using (transaction = _connectionFactory.BeginTransaction(_dbConnection))
                {
                    int result;
                    var query = "SpInsertUpdatePublicUserInvoiceSettings";
                    var param = new DynamicParameters();
                  
                    param.Add("@InvoicePrefix", InvoiceDetails.InvoicePrefix);
                    param.Add("@InvoiceNumber", InvoiceDetails.InvoiceNumber);

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
        public async Task<IEnumerable<UsersDetails>> GetUserInvoiceDetails()
        {
            try
            {
                _connectionFactory.OpenConnection();
                var Details = "SpGetUserInvoiceDetailsSettings";

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

        public async Task<PublicUserNotificationDetails> GetUserNotificationDetailsById(int bookingId)
        {
            try
            {
                var param = new DynamicParameters();
                _connectionFactory.OpenConnection();
                var Details = "SpGetUserNotificationById";
                param.Add("@BookingId", bookingId);
                var result = await SqlMapper.QueryFirstOrDefaultAsync<PublicUserNotificationDetails>(_dbConnection, Details, param, commandType: CommandType.StoredProcedure);
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

        public async Task<int> ChangeBookigStatus(int userId, int status)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var sp = "SpChangeBookigStatus";
                var param = new DynamicParameters();
                param.Add("@UserRegnId", userId);
                param.Add("@Status", status);
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
        public async Task<string> GetAdminEmailId()
        {
            try
            {
                _connectionFactory.OpenConnection();
                var sp = "SpGetEmailIdForAdmin";
                var param = new DynamicParameters();
                param.Add("@UserTypeID", 4);
                var emailId = await SqlMapper.ExecuteScalarAsync<string>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);

                return emailId;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to update card ");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public async Task<UsersDetails> GetAdminProfile(int id)
        {
            UsersDetails adminDetails = new UsersDetails();
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                var staffDetails = "SpGetAdminProfile";
                adminDetails.UserRegnId = id;
                param.Add("@UserRegnId", adminDetails.UserRegnId);
                var result = await SqlMapper.QueryFirstOrDefaultAsync<UsersDetails>(_dbConnection, staffDetails, param, commandType: CommandType.StoredProcedure);
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
        public async Task<int> UpdateAdminProfile(UsersDetails adminDetails)
        {
            IDbTransaction transaction = null;
            try
            {
                _connectionFactory.OpenConnection();
                using (transaction = _connectionFactory.BeginTransaction(_dbConnection))
                {
                    var sp = "SpInsertUpdateAdminDetails";
                    var param = new DynamicParameters();
                    param.Add("@Id", adminDetails.UserRegnId);
                    param.Add("@FirstName", adminDetails.FirstName);
                    param.Add("@LastName", adminDetails.LastName);
                    if (adminDetails.ProfilePicPath != null)
                    {
                        param.Add("@ProfilePicPath", adminDetails.ProfilePicPath);
                    }
                    //param.Add("@GenderId", adminDetails.GenderId);
                    //param.Add("@Dob", null);
                    param.Add("@Location", adminDetails.Location);
                    param.Add("@CityId", adminDetails.CityId);
                    param.Add("@Housename", adminDetails.HouseName);
                    param.Add("@ZipCode", adminDetails.ZipCode);
                    param.Add("@UserTypeId", adminDetails.UserTypeId);
                    param.Add("@PrimaryPhoneNo", adminDetails.PrimaryPhoneNo);
                    param.Add("@SecondaryPhoneNo", adminDetails.SecondaryPhoneNo);
                    param.Add("@EmailAddress", adminDetails.EmailAddress);
                    param.Add("@Password", adminDetails.Password);

                    param.Add("@UserStatus", adminDetails.UserStatus);
                    param.Add("@UserID_OUT", DbType.Int32, direction: ParameterDirection.Output);
                    var result = await SqlMapper.ExecuteAsync(_dbConnection, sp, param, transaction, commandType: CommandType.StoredProcedure);
                    if (adminDetails.ProfilePicPath != null)
                    {
                        var spPic = "SpUpdateUserProfilePic";
                        param = new DynamicParameters();
                        param.Add("@Id", adminDetails.UserRegnId);
                        param.Add("@ProfilePicPath", adminDetails.ProfilePicPath);
                        var resultprofileupdate = await SqlMapper.QueryFirstOrDefaultAsync<int>(_dbConnection, spPic, param, transaction, commandType: CommandType.StoredProcedure);
                    }
                    transaction.Commit();



                    return await Task.FromResult(1);
                }
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                _logger.Error(ex, "Failed to create user");
                return await Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<int> UpdateUserEmail(int id, string emailId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var sp = "SpUpdateUserEmail";
                var param = new DynamicParameters();
                param.Add("@UserRegnId", id);
                param.Add("@EmailId", emailId);
                int result = await SqlMapper.QueryFirstOrDefaultAsync<int>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
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
        public async Task<string> LoadPhoneCodeByCountryId(int countryId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var sp = "SpGetPhoneCodeByCountryId";
                var param = new DynamicParameters();
                param.Add("@CountryId", countryId);

                string result = await SqlMapper.QueryFirstOrDefaultAsync<string>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to update card ");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public Task<int> SaveResidentDetails(Resident residentDetails)
        {
            IDbTransaction transaction = null;
            try
            {
                _connectionFactory.OpenConnection();
                using (transaction = _connectionFactory.BeginTransaction(_dbConnection))
                {
                    var sp = "spInsertUpdateResidentDetails";
                    var param = new DynamicParameters();
                    param.Add("@ResidentId", residentDetails.ResidentId);
                    param.Add("@ClientId", residentDetails.ClientId);
                    param.Add("@ResidentName", residentDetails.ResidentName);
                    param.Add("@OtherInfo", residentDetails.OtherInfo);
                    param.Add("@ResidentID_OUT", DbType.Int32, direction: ParameterDirection.Output);
                    var result = SqlMapper.ExecuteAsync(_dbConnection, sp, param, transaction, commandType: CommandType.StoredProcedure).Result;

                    transaction.Commit();

                    return Task.FromResult(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to save residents");
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                if ((bool)ex.InnerException?.Message.Contains("UNIQUE KEY"))
                    if (ex.InnerException.Message.Contains("UQ__Settings__ResidentName"))
                        return Task.FromResult(10001);
                if (ex.InnerException.Message.Contains("UNIQUE KEY"))
                    return Task.FromResult(10002);

                return Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }

        }


        /// <summary>
        /// method to implement retrieve resident details
        /// </summary>
        /// <param name="residentId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Resident>> RetrieveResidentDetails()
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpGetResidentDetails";
                var result = await SqlMapper.QueryAsync<Resident>(_dbConnection, query, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving residents from datebase");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// method to implement retrieve resident details
        /// </summary>
        /// <param name="residentId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Resident>> RetrieveResidentDetailsById(int clientId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpGetResidentDetailsById";
                var param = new DynamicParameters();
                param.Add("@ClientId", clientId);
                var result = await SqlMapper.QueryAsync<Resident>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving residents from datebase");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        /// <summary>
        /// Deletes the resident details.
        /// </summary>
        /// <param name="residentId">The resident Id.</param>
        /// <returns></returns>
        public Task<int> DeleteResident(int residentId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                int result;
                var query = "SpDeleteResident";
                var param = new DynamicParameters();
                param.Add("@ResidentId", residentId);
                result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while deleting residents");
                if (ex.InnerException.Message.Contains("REFERENCE constraint"))
                    return Task.FromResult(10002);
                return Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payPal"></param>
        /// <returns></returns>
        public async Task<int> InsertUpdatePaypalSettings(PayPalAccount payPal)
        {
            IDbTransaction transaction = null;
            try
            {
                _connectionFactory.OpenConnection();
                using (transaction = _connectionFactory.BeginTransaction(_dbConnection))
                {
                    var sp = "spInsertUpdatePayPalSettings";
                    var param = new DynamicParameters();
                    param.Add("@PaypalId", payPal.PaypalId);
                    param.Add("@ClientId", payPal.ClientId);
                    param.Add("@SecretKey", payPal.SecretKey);
                    var result = SqlMapper.ExecuteAsync(_dbConnection, sp, param, transaction, commandType: CommandType.StoredProcedure).Result;

                    transaction.Commit();

                    return await Task.FromResult(1);
                }
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                _logger.Error(ex, "Failed to create user");
                return await Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<PayPalAccount> GetPayPalAccount(int paypalId)
        {
            var sp = "spSelectPayPalAccount";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@PaypalId", paypalId);
                var result = await SqlMapper.QueryFirstOrDefaultAsync<PayPalAccount>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get PayPal Account details.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }

        }

        public async Task<int> InsertUpdateTestimonials(Testimonial testimonial)
        {
            IDbTransaction transaction = null;
            try
            {
                _connectionFactory.OpenConnection();
                using (transaction = _connectionFactory.BeginTransaction(_dbConnection))
                {
                    var sp = "spInsertUpdateTestimonials";
                    var param = new DynamicParameters();
                    //SqlParameter imageParameter = new SqlParameter("@imgdata", SqlDbType.Image);
                    //imageParameter.Value = DBNull.Value;
                    //cmd.Parameters.Add(imageParameter);
                    param.Add("@TestimonialId", testimonial.TestimonialId);
                    param.Add("@ClientName", testimonial.ClientName);
                    param.Add("@Designation", testimonial.Designation);
                    param.Add("@Description", testimonial.Description);
                    param.Add("@Url", testimonial.URL);
                    param.Add("@Rating", testimonial.Rating);
                    var result = SqlMapper.ExecuteAsync(_dbConnection, sp, param, transaction, commandType: CommandType.StoredProcedure).Result;

                    transaction.Commit();

                    return await Task.FromResult(1);
                }
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                _logger.Error(ex, "Failed to create user");
                return await Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<IEnumerable<Testimonial>> RetrieveTestimonials(int testimonialId)
        {
            var sp = "spSelectTestimonials";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@TestimonialId", testimonialId);
                var result = await SqlMapper.QueryAsync<Testimonial>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get testimonials.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public  Task<int> DeleteTestimonial(int testimonialId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                int result;
                var query = "SpDeleteTestimonial";
                var param = new DynamicParameters();
                param.Add("@TestimonialId", testimonialId);
                result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while deleting questions");
                if (ex.InnerException.Message.Contains("REFERENCE constraint"))
                    return Task.FromResult(10002);
                return Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public Task<int> AddEmailConfiguration(EmailConfiguration emailConfiguration)
        {
            IDbTransaction transaction = null;
            try
            {
                _connectionFactory.OpenConnection();
                using (transaction = _connectionFactory.BeginTransaction(_dbConnection))
                {
                    var sp = "spInsertUpdateConfigDetails";
                    var param = new DynamicParameters();
                    param.Add("@ConfigId", emailConfiguration.ConfigId);
                    param.Add("@ConfigName", emailConfiguration.ConfigName);
                    param.Add("@MailHost", emailConfiguration.MailHost);
                    param.Add("@MailPort", emailConfiguration.MailPort);
                    param.Add("@SSL", emailConfiguration.SSL);
                 
                    param.Add("@ConfigId_OUT", DbType.Int32, direction: ParameterDirection.Output);
                    var result = SqlMapper.ExecuteAsync(_dbConnection, sp, param, transaction, commandType: CommandType.StoredProcedure).Result;

                    transaction.Commit();

                    return Task.FromResult(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to save residents");
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
        public async Task<IEnumerable<EmailConfiguration>> GetConfigList()
        {
            var sp = "spGetConfigDetails";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
               
                var result = await SqlMapper.QueryAsync<EmailConfiguration>(_dbConnection, sp, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get testimonials.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public async Task<int> SetConfig(int configId)
        {
            var sp = "SetDefaultConfiguration";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                param.Add("@ConfigId", configId);
                var result = await SqlMapper.QueryFirstOrDefaultAsync<int>(_dbConnection, sp,param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get testimonials.");
                return 0;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// Deletes the config details.
        /// </summary>
        /// <param name="configId">The config Id.</param>
        /// <returns></returns>
        public Task<int> DeleteConfigDetails(int configId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                int result;
                var query = "SpDeleteConfiguration";
                var param = new DynamicParameters();
                param.Add("@ConfigId", configId);
                result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while deleting config details");
              
                return Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// Get the default configuration
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<EmailConfiguration> GetDefaultConfiguration()
        {
            var sp = "spGetDefaultConfiguration";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();
                var result = await SqlMapper.QueryFirstOrDefaultAsync<EmailConfiguration>(_dbConnection, sp, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get testimonials.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public Task<int> AddEmailTypeConfiguration(EmailTypeConfiguration emailTypeConfiguration)
        {
            IDbTransaction transaction = null;
            try
            {
                _connectionFactory.OpenConnection();
                using (transaction = _connectionFactory.BeginTransaction(_dbConnection))
                {
                    var sp = "spInsertUpdateEmailTypeConfig";
                    var param = new DynamicParameters();
                    param.Add("@ConfigId", emailTypeConfiguration.ConfigId);
                    param.Add("@EmailTypeId", emailTypeConfiguration.EmailtypeId);
                    param.Add("@FromEmail", emailTypeConfiguration.FromEmail);
                    param.Add("@Password", emailTypeConfiguration.Password);
                    param.Add("@BranchId", emailTypeConfiguration.BranchId);
                    param.Add("@ConfigId_OUT", DbType.Int32, direction: ParameterDirection.Output);
                    var result = SqlMapper.ExecuteAsync(_dbConnection, sp, param, transaction, commandType: CommandType.StoredProcedure).Result;

                    transaction.Commit();

                    return Task.FromResult(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to add email id configuration");
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                if (ex.InnerException.Message.Contains("unique index"))
                    return Task.FromResult(10001);

                return Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<IEnumerable<EmailTypeConfiguration>> GetEmailTypeConfig()
        {
            var sp = "spGetEmailTypeConfig";
            var param = new DynamicParameters();
            try
            {
                _connectionFactory.OpenConnection();

                var result = await SqlMapper.QueryAsync<EmailTypeConfiguration>(_dbConnection, sp, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get testimonials.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public Task<int> DeleteEmailTypeConfig(int configId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                int result;
                var query = "SpDeleteEmailTypeConfig";
                var param = new DynamicParameters();
                param.Add("@ConfigId", configId);
                result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while deleting config details");

                return Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<EmailTypeConfiguration> GetEmailIdConfigByType(EmailType emailType, int branchId)
        {
            var sp = "";
           
             sp = "spGetEmailIdConfigByType";
            var param = new DynamicParameters();
            param.Add("@emailType", (int)emailType);
            param.Add("@BranchId", branchId);
            try
            {
                _connectionFactory.OpenConnection();
                var result = await SqlMapper.QueryFirstOrDefaultAsync<EmailTypeConfiguration>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);
               //if(result.FromEmail=="")
               // {
               //      sp = "spGetEmailIdConfigByType";
               //      param = new DynamicParameters();
               //     param.Add("@emailType", (int)emailType);
               //     param.Add("@BranchId", 0);
               //     result = await SqlMapper.QueryFirstOrDefaultAsync<EmailTypeConfiguration>(_dbConnection, sp, param, commandType: CommandType.StoredProcedure);

               // }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get GetEmailIdConfigByType.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public async Task<IEnumerable<BookingPayriseModel>> GetBookingPayriseList(BookingPayriseModel bookingPayriseModel)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var param = new DynamicParameters();
                param.Add("@CaretakerId", bookingPayriseModel.CaretakerName);
                param.Add("@ServiceId", bookingPayriseModel.Service);
                param.Add("@EffectiveFrom", bookingPayriseModel.EffectiveFromDate == DateTime.MinValue ? null : bookingPayriseModel.EffectiveFromDate);


                var query = "SpSearchBookingPayrise";
                var bookingDetail = await SqlMapper.QueryAsync<BookingPayriseModel>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return bookingDetail;
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
        public async Task<IEnumerable<InvoicePayriseModel>> GetInvoicePayriseList(InvoicePayriseModel invoicePayriseModel)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var param = new DynamicParameters();
                param.Add("@ClientId", invoicePayriseModel.ClientName);
                //param.Add("@Category", invoicePayriseModel.CategoryId);
                param.Add("@EffectiveFrom", invoicePayriseModel.EffectiveFromDate == DateTime.MinValue ? null : invoicePayriseModel.EffectiveFromDate);


                var query = "SpSearchInvoicePayrise";
                var invoiceDetail = await SqlMapper.QueryAsync<InvoicePayriseModel>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return invoiceDetail;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve Invoice Payrise List details");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public async Task<IEnumerable<PayrollPayriseModel>> GetPayrollPayriseList(PayrollPayriseModel payrollPayriseModel)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var param = new DynamicParameters();
                param.Add("@ClientId", payrollPayriseModel.ClientName);
                param.Add("@CaretakerId", payrollPayriseModel.CaretakerName);

                param.Add("@EffectiveFrom", payrollPayriseModel.EffectiveFromDate == DateTime.MinValue ? null : payrollPayriseModel.EffectiveFromDate);


                var query = "SpSearchPayrollPayrise";
                var payrollDetail = await SqlMapper.QueryAsync<PayrollPayriseModel>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return payrollDetail;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve Payroll Payrise List details");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public Task<int> DeleteBookingPayrise(int bookingPayriseId)
        {
            IDbTransaction transaction = null;
            try
            {
                _connectionFactory.OpenConnection();
                using (transaction = _connectionFactory.BeginTransaction(_dbConnection))
                {
                    int result;
                    var query = "SpDeleteBookingPayrise";
                    var param = new DynamicParameters();
                    param.Add("@BookingPayriseId", bookingPayriseId);
                    result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, transaction, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                    transaction.Commit();
                    return Task.FromResult(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete booking payrise");
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                if (ex.InnerException.Message.Contains("REFERENCE constraint"))
                    return Task.FromResult(10002);
                return Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public Task<int> DeleteInvoicePayrise(int invoicePayriseId)
        {
            IDbTransaction transaction = null;
            try
            {
                _connectionFactory.OpenConnection();
                using (transaction = _connectionFactory.BeginTransaction(_dbConnection))
                {
                    int result;
                    var query = "SpDeleteInvoicePayrise";
                    var param = new DynamicParameters();
                    param.Add("@InvoicePayriseId", invoicePayriseId);
                    result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, transaction, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                    transaction.Commit();
                    return Task.FromResult(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete invoice payrise");
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                if (ex.InnerException.Message.Contains("REFERENCE constraint"))
                    return Task.FromResult(10002);
                return Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public Task<int> DeletePayrollPayrise(int payrollPayriseId)
        {
            IDbTransaction transaction = null;
            try
            {
                _connectionFactory.OpenConnection();
                using (transaction = _connectionFactory.BeginTransaction(_dbConnection))
                {
                    int result;
                    var query = "SpDeletePayrollPayrise";
                    var param = new DynamicParameters();
                    param.Add("@PayrollPayriseId", payrollPayriseId);
                    result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, transaction, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                    transaction.Commit();
                    return Task.FromResult(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete payroll payrise");
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                if (ex.InnerException.Message.Contains("REFERENCE constraint"))
                    return Task.FromResult(10002);
                return Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public async Task<IEnumerable<BookingPayriseModel>> GetAllBookingPayriseDetails()
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "spGetBookingPayriseDetails";
                var result = await SqlMapper.QueryAsync<BookingPayriseModel>(_dbConnection, query, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving residents from datebase");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public async Task<IEnumerable<InvoicePayriseModel>> GetAllInvoicePayriseDetails()
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "spGetInvoicePayriseDetails";
                var result = await SqlMapper.QueryAsync<InvoicePayriseModel>(_dbConnection, query, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving residents from datebase");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public async Task<IEnumerable<PayrollPayriseModel>> GetAllPayrollPayriseDetails()
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "spGetPayrollPayriseDetails";
                var result = await SqlMapper.QueryAsync<PayrollPayriseModel>(_dbConnection, query, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving residents from datebase");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// DB migration purpose only
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<UsersDetails>> GetAllUserDetails()
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "spGetAllUserDetails";
                var result = await SqlMapper.QueryAsync<UsersDetails>(_dbConnection, query, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving residents from datebase");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public async Task<IEnumerable<UsersDetails>> GetAllUserDetailsByLocation(LocationSearchInputs inputs)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "spGetAllUserDetails";
                var param = new DynamicParameters();
                param.Add("@CountryId", inputs.CountryId);
                param.Add("@StateId", inputs.StateId);
                param.Add("@CityId", inputs.CityId);
                var result = await SqlMapper.QueryAsync<UsersDetails>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving residents from datebase");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public Task<int> DeleteSchedule(ScheduleDeleteData deleteData)
        {
            try
            {
                _connectionFactory.OpenConnection();
                int result;
                var query = "SpDeletePublicUserSchedule";
                var param = new DynamicParameters();
                param.Add("@Id", deleteData.ScheduleId);
                result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();

                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "delete failed");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public Task<int> UpdateUserDetails(UsersDetails usersDetails)
        {
            try
            {
                _connectionFactory.OpenConnection();
                int result;
                var query = "SpUpdateUserDetails";
                var param = new DynamicParameters();
                param.Add("@UserId", usersDetails.UserRegnId);
                param.Add("@UserTypeId", usersDetails.UserTypeId);
                param.Add("@FilePath", usersDetails.ProfilePicPath);
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


        /// <summary>
        /// DB migration purpose only
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DocumentsList>> GetAllCaretakerDocuments()
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "spGetAllCaretakerDocuments";
                var result = await SqlMapper.QueryAsync<DocumentsList>(_dbConnection, query, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving residents from datebase");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// DB migration purpose only
        /// </summary>
        /// <returns></returns>
        public Task<int> UpdateCaretakerDocuments(DocumentsList doc)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpUpdateCaretakerDocuments";
                var param = new DynamicParameters();
                param.Add("@DocId", doc.CaretakerDocumentId);
                param.Add("@DocPath", doc.DocumentPath);
                var result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
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

        public async Task<UserRoleDetailsModel> GetUserRoleDetails(int userId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpGetUserWorkRoles";
                var param = new DynamicParameters();
                param.Add("@UserId",userId);
                var result = SqlMapper.QueryAsync<UserRoleDetailsModel>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "get role failed");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public Task<int> AddBookingDetails(PublicUserCaretakerBooking data, out string message)
        
        {
            IDbTransaction transaction = null;
            message = "Success";
            try
            {
                _connectionFactory.OpenConnection();
                using (transaction = _connectionFactory.BeginTransaction(_dbConnection))
                {
                    int result;
                    var sp = "SpAddEditBookingDetails";
                    var param = new DynamicParameters();
                    param.Add("@BookingDateTime", data.BookingDateTime);
                    param.Add("@BookingId", data.BookingId);
                    param.Add("@Id", data.Id);
                    param.Add("@PublicUserId", data.PublicUserId);
                    param.Add("@WorkShiftId", data.WorkMode);
                    param.Add("@TimeShiftId", data.WorkTime == 0 ? null : data.WorkTime);
                    param.Add("@StartDate", data.Start);
                    param.Add("@EndDate", data.End);
                    param.Add("@ScheduleDescription", data.Description);
                    param.Add("@CareTaker", data.CareTaker);
                    param.Add("@ScheduledID_OUT", DbType.Int32, direction: ParameterDirection.Output);
                    result = SqlMapper.ExecuteAsync(_dbConnection, sp, param, transaction, commandType: CommandType.StoredProcedure).Result;
                    var bookingId = param.Get<int>("@ScheduledID_OUT");

                   

                    if (data.PublicUserSchedulingDate != null)
                    {

                        foreach (var item in data.PublicUserSchedulingDate)
                        {
                            param = new DynamicParameters();
                            var _spSaveClientSchedulingDates = "spSavePublicUserBookingDates";
                            param.Add("@BookingId", bookingId);
                            param.Add("@Date", item.Date);
                            param.Add("@Hours", item.Hours);
                            int resultCareTakerMultiDetails = SqlMapper.QueryAsync<int>(_dbConnection, _spSaveClientSchedulingDates, param, transaction, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();

                        }

                    }
                    transaction.Commit();
                    return Task.FromResult(bookingId);
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
        public async Task<IEnumerable<PublicUserCaretakerBooking>> GetAllBookingdetails(CalenderBookingEventInput calenderEventInput)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var Details = "SpSelectBookingDetails";
                var param = new DynamicParameters();
                param.Add("@BookingId", calenderEventInput.BookingId);
                param.Add("@StartDate", calenderEventInput.StartDate);
                param.Add("@EndDate", calenderEventInput.EndDate);
                var result = await SqlMapper.QueryAsync<PublicUserCaretakerBooking>(_dbConnection, Details, param, commandType: CommandType.StoredProcedure);
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
    }
}
