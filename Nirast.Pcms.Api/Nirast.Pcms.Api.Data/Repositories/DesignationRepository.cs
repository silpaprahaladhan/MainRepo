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
    public class DesignationRepository : GenericRepository<DesignationDetails>, IDesignationRepository
    {
        IConnectionFactory _connectionFactory;
        IPCMSLogger _logger;
        IDbConnection _dbConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="DesignationRepository"/> class.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        /// <param name="logger">The logger.</param>
        public DesignationRepository(IConnectionFactory connectionFactory, IPCMSLogger logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _dbConnection = connectionFactory.GetConnection();
        }

        /// <summary>
        /// Adds the designation.
        /// </summary>
        /// <param name="Designation">The designation.</param>
        /// <returns></returns>
        public Task<int> AddDesignation(DesignationDetails Designation)
        {
            IDbTransaction transaction = null;
            try
            {
                _connectionFactory.OpenConnection();
                using (transaction = _connectionFactory.BeginTransaction(_dbConnection))
                {
                    int result;
                    var query = "SpInsertUpdateDesignation";
                    var param = new DynamicParameters();
                    param.Add("@DesignationId", Designation.DesignationId);
                    param.Add("@Designation", Designation.Designation);
                    result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, transaction, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();

                    transaction.Commit();
                    return Task.FromResult(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while add designation");
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

        /// <summary>
        /// Deletes the designation.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Task<int> DeleteDesignation(int id)
        {
            try
            {
                _connectionFactory.OpenConnection();
                int result;
                var query = "SpDeleteDesignation";
                var param = new DynamicParameters();
                param.Add("@DesignationId", id);
                result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while delete designation");
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
        /// Retrieves the designation.
        /// </summary>
        /// <param name="DesignationId">The designation identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<DesignationDetails>> RetrieveDesignation(int DesignationId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpSelectDesignation";
                var param = new DynamicParameters();
                param.Add("@DesignationId", DesignationId);
                var result = await SqlMapper.QueryAsync<DesignationDetails>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving designation ");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<DesignationDetails> RetrieveDesignationById(int DesignationId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpSelectDesignation";
                var param = new DynamicParameters();
                param.Add("@DesignationId", DesignationId);
                var result = await SqlMapper.QueryFirstOrDefaultAsync<DesignationDetails>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving designation ");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
    }
}
