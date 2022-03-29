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
    public class StateRepository : GenericRepository<States>, IStateRepository
    {
        IConnectionFactory _connectionFactory;
        IPCMSLogger _logger;
        IDbConnection _dbConnection;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionFactory"></param>
        public StateRepository(IConnectionFactory connectionFactory, IPCMSLogger logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _dbConnection = connectionFactory.GetConnection();
        }

        public Task<int> DeleteState(int StateId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                int result;
                var query = "SpDeleteStateDetails";
                var param = new DynamicParameters();
                param.Add("@StateId", StateId);
                result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while deleting states");
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
        /// Function to add Service detail
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public Task<int> AddState(States state)
        {
            try
            {
                _connectionFactory.OpenConnection();

                int result;
                var query = "SpAddEditStateDetails";
                var param = new DynamicParameters();
                param.Add("@StateId", state.StateId);
                param.Add("@CountryId", state.CountryId);
                param.Add("@Code", state.Code);
                param.Add("@Name", state.Name);
                param.Add("@TaxPercent", state.TaxPercent);
                result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while adding states");
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
        /// To get states by country id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IEnumerable<States>> GetStatesById(int countryId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpGetStateByCountryId";
                var param = new DynamicParameters();
                param.Add("@CountryId", countryId);
                var result = await SqlMapper.QueryAsync<States>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving states");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// Retrieves the states.
        /// </summary>
        /// <param name="stateId">The state identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<States>> RetrieveStates(int stateId = 0)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpSelectStateDetails";
                var param = new DynamicParameters();
                param.Add("@StateId", stateId);

                var list = await SqlMapper.QueryAsync<States>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return (list);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving states");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
    }
}
