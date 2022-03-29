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
    public class CountryRepository : GenericRepository<Countries>, ICountryRepository
    {
        IConnectionFactory _connectionFactory;
        IDbConnection _dbConnection;
        IPCMSLogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryRepository"/> class.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        public CountryRepository(IConnectionFactory connectionFactory, IPCMSLogger logger)
        {
            _connectionFactory = connectionFactory;
            _dbConnection = connectionFactory.GetConnection();
            _logger = logger;
        }

        /// <summary>
        /// Adds the country.
        /// </summary>
        /// <param name="country">The country.</param>
        /// <returns></returns>
        public Task<int> AddCountry(Countries country)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpAddEditCountryDetails";
                var param = new DynamicParameters();
                param.Add("@CountryId", country.CountryId);
                param.Add("@Code", country.Code);
                param.Add("@Name", country.Name);
                param.Add("@PhoneCode", country.PhoneCode);
                param.Add("@Currency", country.Currency);
                param.Add("@Symbol", country.CurrencySymbol);
                param.Add("@IsDefault", country.Isdefault);
                var result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while add country");

                if (ex.InnerException.Message.Contains("UNIQUE KEY"))
                    return Task.FromResult(10001);

                return Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public Task<int> DeleteCountry(int CountryId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                int result;
                var query = "SpDeleteCountryDetails";
                var param = new DynamicParameters();
                param.Add("@CountryId", CountryId);
                result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while delete country");
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
        /// Retrieves the country.
        /// </summary>
        /// <param name="countryId">The country identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Countries>> RetrieveCountry(int countryId)
        {
            IDbTransaction transaction = null;
            try
            {
                _connectionFactory.OpenConnection();
                using (transaction = _connectionFactory.BeginTransaction(_dbConnection))
                {
                    var query = "SpSelectCountryDetails";
                    var param = new DynamicParameters();
                    param.Add("@CountryId", countryId);

                    var list = await SqlMapper.QueryAsync<Countries>(_dbConnection, query, param, transaction, commandType: CommandType.StoredProcedure);
                    return (list);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving country details from database");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// Retrieves the country.
        /// </summary>
        /// <param name="countryId">The country identifier.</param>
        /// <returns></returns>
        public async Task<Countries> GetDefaultCountry()
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpSelectDefaultCountry";
                var list = await SqlMapper.QueryAsync<Countries>(_dbConnection, query, commandType: CommandType.StoredProcedure);
                return (list.FirstOrDefault());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving country details from database");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
    }
}
