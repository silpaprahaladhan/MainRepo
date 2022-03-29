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
    public class CityRepository : GenericRepository<Cities>, ICityRepository
    {
        IConnectionFactory _connectionFactory;
        IDbConnection _dbConnection;
        IPCMSLogger _logger;


        /// <summary>
        /// Initializes a new instance of the <see cref="CityRepository"/> class.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        public CityRepository(IConnectionFactory connectionFactory, IPCMSLogger logger)
        {
            _connectionFactory = connectionFactory;
            _dbConnection = connectionFactory.GetConnection();
            _logger = logger;
        }

        /// <summary>
        /// Adds the city.
        /// </summary>
        /// <param name="city">The city.</param>
        /// <returns></returns>
        public Task<int> AddCity(Cities city)
        {
            IDbTransaction transaction = null;
            try
            {
                _connectionFactory.OpenConnection();
                using (transaction = _connectionFactory.BeginTransaction(_dbConnection))
                {
                    int result;
                    var query = "SpInsertUpdateCity";
                    var param = new DynamicParameters();
                    param.Add("@CityId", city.CityId);
                    param.Add("@StateId", city.StateId);
                    param.Add("@Name", city.CityName);
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

        /// <summary>
        /// Deletes the city.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Task<int> DeleteCity(int id)
        {
            try
            {
                _connectionFactory.OpenConnection();
                int result;
                var query = "SpDeleteCity";
                var param = new DynamicParameters();
                param.Add("@CityId", id);
                result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete city while deleting city from database");
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
        /// Retrieves the cities.
        /// </summary>
        /// <param name="flag">The flag.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Cities>> RetrieveCities(string flag, string value)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpSelectAllCities";
                var param = new DynamicParameters();
                param.Add("@Flag", flag);
                param.Add("@Value", value);
                var result = await SqlMapper.QueryAsync<Cities>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return result;

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve city while retrieving city from database");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        /// <summary>
        /// Retrieves the cities.
        /// </summary>
        /// <param name="flag">The flag.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Cities>> RetrieveBranches(string flag, string value)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpSelectAllBranches";
                var param = new DynamicParameters();
                param.Add("@Flag", flag);
                param.Add("@Value", value);
                var result = await SqlMapper.QueryAsync<Cities>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return result;

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve city while retrieving city from database");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        /// <summary>
        /// Retrieves the branches.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public async Task<Cities> GetBranchByUserId(int Id)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpGetBranchByUserId";
                var param = new DynamicParameters();
                param.Add("@UserId", Id);
                var result = await SqlMapper.QueryFirstOrDefaultAsync<Cities>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return result;

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve city while retrieving city from database");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// Retrieves the cities.
        /// </summary>
        /// <param name="flag">The flag.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Cities>> GetBranchesByLocation(LocationSearchInputs inputs)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpSelectBranchesByLocation";
                var param = new DynamicParameters();
                param.Add("@CountryId", inputs.CountryId);
                param.Add("@StateId", inputs.StateId);
                param.Add("@CityId", inputs.CityId);
                var result = await SqlMapper.QueryAsync<Cities>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return result;

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve city while retrieving city from database");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }


        /// <summary>
        /// Retrieves the Branch by Id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Cities>> RetrieveBranchById(int id)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpGetBranchById";
                var param = new DynamicParameters();
                param.Add("@Id", id);
                var result = await SqlMapper.QueryAsync<Cities>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return result;

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve city while retrieving city from database");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        public async Task<IEnumerable<Cities>> RetrieveBranchesById(string flag, string value)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpSelectBranchId";
                var param = new DynamicParameters();
                param.Add("@Flag", flag);
                param.Add("@Value", value);
                var result = await SqlMapper.QueryAsync<Cities>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return result;

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve city while retrieving city from database");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<IEnumerable<Cities>> RetrieveCountry(string flag, string value)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpSelectCountry";
                var param = new DynamicParameters();
                param.Add("@Flag", flag);
                param.Add("@Value", value);
                var result = await SqlMapper.QueryAsync<Cities>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return result;

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve city while retrieving city from database");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }


        public async Task<IEnumerable<Cities>> Retrievestates(string flag, string value)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpSelectState";
                var param = new DynamicParameters();
                param.Add("@Flag", flag);
                param.Add("@Value", value);
                var result = await SqlMapper.QueryAsync<Cities>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return result;

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve city while retrieving city from database");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<IEnumerable<Cities>> RetrievecityDetails(string flag, string value)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpSelectCity";
                var param = new DynamicParameters();
                param.Add("@Flag", flag);
                param.Add("@Value", value);
                var result = await SqlMapper.QueryAsync<Cities>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return result;

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve city while retrieving city from database");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// Retrieves the cities.
        /// </summary>
        /// <param name="flag">The flag.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Cities>> RetrieveAllBranches()
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpGetSelectAllBranches";
                var param = new DynamicParameters();
                //param.Add("@Flag", flag);
                //param.Add("@Value", value);
                var result = await SqlMapper.QueryAsync<Cities>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return result;

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve city while retrieving city from database");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// To get city by stateid
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Cities>> GetCityByStateId(int stateId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpGetCityByStateId";
                var param = new DynamicParameters();
                param.Add("@StateId", stateId);
                var result = await SqlMapper.QueryAsync<Cities>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving city");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
    }
}
