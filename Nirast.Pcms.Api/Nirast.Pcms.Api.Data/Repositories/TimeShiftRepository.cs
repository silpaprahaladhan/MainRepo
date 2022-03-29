using Dapper;
using Nirast.Pcms.Api.Sdk.Entities;
using Nirast.Pcms.Api.Sdk.Infrastructure;
using Nirast.Pcms.Api.Sdk.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Nirast.Pcms.Api.Sdk.Logger;

namespace Nirast.Pcms.Api.Data.Repositories
{
    public class TimeShiftRepository : GenericRepository<ClientTimeShifts>, ITimeShiftRepository
    {
        IConnectionFactory _connectionFactory;
        IPCMSLogger _logger;
        IDbConnection _dbConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeShiftRepository"/> class.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        /// <param name="logger">The logger.</param>
        public TimeShiftRepository(IConnectionFactory connectionFactory, IPCMSLogger logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _dbConnection = connectionFactory.GetConnection();
        }

        /// <summary>
        /// Adds the time shift.
        /// </summary>
        /// <param name="timeShift">The time shift.</param>
        /// <returns></returns>
        public  Task<int> AddTimeShift(ClientTimeShifts timeShift)
        {
            try
            {
                var query = "SpAddEditTimeShiftDetails";
                var param = new DynamicParameters();
                param.Add("@TimeShiftId", timeShift.TimeShiftId);
                param.Add("@TimeShift", timeShift.TimeShiftName);
                param.Add("@WorkingHours", timeShift.WorkingHours);
                param.Add("@PayingHours", timeShift.PayingHours);
                param.Add("@StartTime", timeShift.StartTime);
                var result= SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to save time shift details");
                if (ex.InnerException.Message.Contains("UNIQUE KEY"))
                    return Task.FromResult(10001);
                return Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }


        /// <summary>
        /// Retrieves the time shift.
        /// </summary>
        /// <param name="timeShiftId">The time shift identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<ClientTimeShifts>> RetrieveTimeShift(int timeShiftId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpSelectTimeShiftDetails";
                var param = new DynamicParameters();
                param.Add("@TimeShiftId", timeShiftId);
                var list = await SqlMapper.QueryAsync<ClientTimeShifts>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return (list);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve time shift details");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        /// <summary>
        /// Retrieves the time shift by client id.
        /// </summary>
        /// <param name="clinet id">The time shift identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<ClientTimeShifts>> RetrieveTimeShiftByClientId(int clientId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpSelectTimeShiftDetailsByClientid";
                var param = new DynamicParameters();
                param.Add("@clientId", clientId);
                var list = await SqlMapper.QueryAsync<ClientTimeShifts>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return (list);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve time shift details");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// Deletes the client shift detail.
        /// </summary>
        /// <param name="TimeShiftId">The time shift identifier.</param>
        /// <returns></returns>
        public Task<int> DeleteClientShiftDetail(int TimeShiftId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                int result;
                var query = "spDeleteClientTimeShiftDetails";
                var param = new DynamicParameters();
                param.Add("@TimeShiftId", TimeShiftId);
                result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete time shift details");
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
