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
    public class QualificationRepository : GenericRepository<QualificationDetails>, IQualificationRepository
    {
        IConnectionFactory _connectionFactory;
        IPCMSLogger _logger;
        IDbConnection _dbConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="QualificationRepository"/> class.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        /// <param name="logger">The logger.</param>
        public QualificationRepository(IConnectionFactory connectionFactory, IPCMSLogger logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _dbConnection = connectionFactory.GetConnection();
        }

        /// <summary>
        /// Adds the qualification.
        /// </summary>
        /// <param name="Qualification">The qualification.</param>
        /// <returns></returns>
        public Task<int> AddQualification(QualificationDetails Qualification)
        {
            try
            {
                _connectionFactory.OpenConnection();
                int result;
                var query = "SpInsertUpdateQualification";
                var param = new DynamicParameters();
                param.Add("@QualificationId", Qualification.QualificationId);
                param.Add("@Qualification", Qualification.Qualification);
                result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while adding qualification");
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
        /// Deletes the qualification.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Task<int> DeleteQualification(int id)
        {
            try
            {
                _connectionFactory.OpenConnection();
                int result;
                var query = "SpDeleteQualification";
                var param = new DynamicParameters();
                param.Add("@QualificationId", id);
                result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while deleting qualification");
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
        /// Retrieves the qualification.
        /// </summary>
        /// <param name="QualificationId">The qualification identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<QualificationDetails>> RetrieveQualification(int QualificationId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpSelectQualification";
                var param = new DynamicParameters();
                param.Add("@QualificationId", QualificationId);
                var result = await SqlMapper.QueryAsync<QualificationDetails>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving qualification");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
    }
}
