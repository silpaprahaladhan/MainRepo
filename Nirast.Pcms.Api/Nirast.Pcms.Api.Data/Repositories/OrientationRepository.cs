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
    public class OrientationRepository : GenericRepository<Orientation>, IOrientationRepository
    {
        IConnectionFactory _connectionFactory;
        IPCMSLogger _logger;
        IDbConnection _dbConnection;


        /// <summary>
        /// Initializes a new instance of the <see cref="OrientationRepository"/> class.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        /// <param name="logger">The logger.</param>
        public OrientationRepository(IConnectionFactory connectionFactory, IPCMSLogger logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _dbConnection = connectionFactory.GetConnection();
        }

        /// <summary>
        /// Adds the orientation.
        /// </summary>
        /// <param name="Orientation">The orientation.</param>
        /// <returns></returns>
        public Task<int> AddOrientation(Orientation Orientation)
        {
            IDbTransaction transaction = null;
            try
            {
                _connectionFactory.OpenConnection();
                using (transaction = _connectionFactory.BeginTransaction(_dbConnection))
                {
                    int result;
                    var query = "SpInsertUpdateOrientation";
                    var param = new DynamicParameters();
                    param.Add("@OrientationId", Orientation.OrientationId);
                    param.Add("@Orientation", Orientation.OrientationName);
                    param.Add("@CreatedUser", Orientation.UserId);
                    param.Add("@LastUpdatedUser", Orientation.UserId);
                    result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, transaction, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();

                    transaction.Commit();
                    return Task.FromResult(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while adding orientation");
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
        /// Deletes the orientation.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Task<int> DeleteOrientation(int id)
        {
            try
            {
                _connectionFactory.OpenConnection();
                int result;
                var query = "SpDeleteOrientation";
                var param = new DynamicParameters();
                param.Add("@OrientationId", id);
                result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while deleting orientation");
                return Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// Retrieves the orientation.
        /// </summary>
        /// <param name="OrientationId">The orientation identifier.</param>
        /// <returns></returns>
        public List<Orientation> RetrieveOrientation(int OrientationId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpSelectAllOrientation";
                var param = new DynamicParameters();
                param.Add("@OrientationId", OrientationId);
                var result = SqlMapper.Query<Orientation>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving orientation");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
    }
}
