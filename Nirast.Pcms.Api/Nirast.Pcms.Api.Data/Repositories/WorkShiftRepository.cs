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
    public class WorkShiftRepository : GenericRepository<WorkShifts>, IWorkShiftRepository
    {
        IConnectionFactory _connectionFactory;
        IPCMSLogger _logger;
        IDbConnection _dbConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkShiftRepository"/> class.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        /// <param name="logger">The logger.</param>
        public WorkShiftRepository(IConnectionFactory connectionFactory, IPCMSLogger logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _dbConnection = connectionFactory.GetConnection();
        }

        /// <summary>
        /// Adds the work shift.
        /// </summary>
        /// <param name="workShift">The work shift.</param>
        /// <returns></returns>
        public async Task<int> AddWorkShift(WorkShifts workShift)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpAddEditWorkShiftDetails";
                var param = new DynamicParameters();
                param.Add("@WorkShiftId", workShift.ShiftId);
                param.Add("@WorkShift", workShift.ShiftName);
                param.Add("@ShowResidentName", workShift.ShowResidentName);
                param.Add("@IsSeparateInvoice", workShift.IsSeparateInvoice);
                var result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to save work shift details");
                if (ex.InnerException.Message.Contains("UNIQUE KEY"))
                    return await Task.FromResult(10001);
                return await Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// Retrieves the work shift.
        /// </summary>
        /// <param name="shiftId">The shift identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<WorkShifts>> RetrieveWorkShift(int shiftId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpSelectWorkShiftDetails";
                var param = new DynamicParameters();
                param.Add("@WorkShiftId", shiftId);

                var list = await SqlMapper.QueryAsync<WorkShifts>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return (list);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve work shift details");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// Deletes the workshift.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<int> DeleteWorkShift(int id)
        {
            try
            {
                _connectionFactory.OpenConnection();
                int result;
                var query = "SpDeleteWorkshift";
                var param = new DynamicParameters();
                param.Add("@ShiftId", id);
                result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete work shift details");
                if (ex.InnerException.Message.Contains("REFERENCE constraint"))
                    return await Task.FromResult(10002);
                return await Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
    }
}
