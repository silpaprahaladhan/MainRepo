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
    public class ServiceRepository : GenericRepository<Services>, IServiceRepository
    {
        IConnectionFactory _connectionFactory;
        IPCMSLogger _logger;
        IDbConnection _dbConnection;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionFactory"></param>
        public ServiceRepository(IConnectionFactory connectionFactory, IPCMSLogger logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _dbConnection = connectionFactory.GetConnection();
        }

        /// <summary>
        /// Function to add Service detail
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public Task<int> AddService(Services service)
        {
            try
            {
                _connectionFactory.OpenConnection();
                int result;
                var query = "SpInsertUpdateServiceDetails";
                var param = new DynamicParameters();
                param.Add("@Id", service.ServiceId);
                param.Add("@Name", service.Name);
                param.Add("@ServiceDescription", service.ServiceDescription);
                param.Add("@ServiceOrder", service.ServiceOrder);
                param.Add("@ServicePic", service.ServicePicture);
                result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while adding services");
                if (ex.InnerException.Message.Contains("UNIQUE KEY"))
                    return Task.FromResult(10001);
                return Task.FromResult(0);

            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public Task<int> DeleteService(int ServiceId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                int result;
                var query = "SpDeleteServiceDetails";
                var param = new DynamicParameters();
                param.Add("@ServiceId", ServiceId);
                result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while deleting services");
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
        /// Retrieves the services.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Services>> RetrieveServices(int serviceId = 0)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpSelectServiceDetails";
                var param = new DynamicParameters();
                param.Add("@ServiceId", serviceId);
                var list = await SqlMapper.QueryAsync<Services>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return (list);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving services");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }


       



    }
}
