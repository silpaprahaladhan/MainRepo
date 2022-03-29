using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Nirast.Pcms.Api.Sdk.Entities;
using Nirast.Pcms.Api.Sdk.Infrastructure;
using Nirast.Pcms.Api.Sdk.Logger;
using Nirast.Pcms.Api.Sdk.Repositories;
namespace Nirast.Pcms.Api.Data.Repositories
{
    public class HolidayRepository : GenericRepository<Holidays>, IHolidayRepository
    {
        IConnectionFactory _connectionFactory;
        IPCMSLogger _logger;
        IDbConnection _dbConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="HolidayRepository"/> class.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        /// <param name="logger">The logger.</param>
        public HolidayRepository(IConnectionFactory connectionFactory, IPCMSLogger logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _dbConnection = connectionFactory.GetConnection();
        }

        /// <summary>
        /// Adds the holiday.
        /// </summary>
        /// <param name="holiday">The holiday.</param>
        /// <returns></returns>
        public Task<int> AddHoliday(Holidays holiday)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpAddEditHolidayDetails";
                var param = new DynamicParameters();
                param.Add("@HolidayId", holiday.HolidayId);
                param.Add("@Holiday", holiday.HolidayName);
                param.Add("@HolidayDate", holiday.HolidayDate);
                param.Add("@CountryId", holiday.CountryId);
                param.Add("@StateId", holiday.StateId == 0? null:holiday.StateId);
                var result= SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while adding holidays");
                if (ex.InnerException.Message.Contains("UNIQUE KEY"))
                    return Task.FromResult(10001);
                return Task.FromResult(0); ;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<int> OverrideHoliday()
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpOverrideHoliday";
                
                //var result = SqlMapper.QueryAsync<int>(_dbConnection, query, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return SqlMapper.QueryAsync<int>(_dbConnection, query,  commandType: CommandType.StoredProcedure).Result.First();;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while adding holidays");
                if (ex.InnerException.Message.Contains("UNIQUE KEY"))
                    return Convert.ToInt32("10001");
                return Convert.ToInt32("0"); ;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        /// <summary>
        /// Adds the interval.
        /// </summary>
        /// <param name="">The timeshift.</param>
        /// <returns></returns>
        public  Task<int> AddIntervalHours(ClientTimeShifts shift)
        {
            try
            {
                int result;
                _connectionFactory.OpenConnection();
                var query = "SpUpdateIntervalHours";
                var param = new DynamicParameters();
                param.Add("@IntervalHours", shift.IntervalHours);
                result= SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to save time shift details");
                return Task.FromResult(0); ;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public Task<int> AddHolidayPay(Holidays holiday)
        {
            try
            {
                int result;
                _connectionFactory.OpenConnection();
                var query = "SpUpdateHolidayPay";
                var param = new DynamicParameters();
                param.Add("@HolidayPay", holiday.HolidayPayTimes);
                result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to save time shift details");
                return Task.FromResult(0); ;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// Retrieves the holiday.
        /// </summary>
        /// <param name="holidayId">The holiday identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Holidays>> RetrieveHoliday(Holidays holidaySearchModel)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpSelectHolidayDetails";
                var param = new DynamicParameters();
                param.Add("@HolidayId", holidaySearchModel.HolidayId);
                param.Add("@HolidayYear", holidaySearchModel.HolidayYear);
                param.Add("@CountryId", holidaySearchModel.CountryId);
                param.Add("@StateId", holidaySearchModel.StateId);
                var list = await SqlMapper.QueryAsync<Holidays>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return (list);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve time shift detail");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        /// <summary>
        /// Retrieves the holiday.
        /// </summary>
        /// <param name="holidayId">The holiday identifier.</param>
        /// <returns></returns>
        public async Task<float> RetrieveHolidayPayDetails()
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpSelectHolidayPay";

                return await SqlMapper.QueryFirstOrDefaultAsync<float>(_dbConnection, query,  commandType: CommandType.StoredProcedure);
                 
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve HolidayPaytimes");
                return 0;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public async Task<float> RetrieveGetIntervalHours()
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpSelectIntervalHours";

                return  SqlMapper.QueryAsync<float>(_dbConnection, query,  commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                 
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve HolidayPaytimes");
                return 0;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        /// <summary>
        /// Deletes the designation.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Task<int> DeleteHoliday(int id)
        {
            try
            {
                _connectionFactory.OpenConnection();
                int result;
                var query = "SpDeleteHoliday";
                var param = new DynamicParameters();
                param.Add("@HolidayId", id);
                result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while delete holidays");
                if (ex.InnerException.Message.Contains("REFERENCE constraint"))
                    return Task.FromResult(10002);
                return Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
    }
}
