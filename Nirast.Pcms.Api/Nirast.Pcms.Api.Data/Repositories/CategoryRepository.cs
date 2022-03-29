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
    public class CategoryRepository : GenericRepository<CategoryModel>, ICategoryRepository
    {
        IConnectionFactory _connectionFactory;
        IDbConnection _dbConnection;
        IPCMSLogger _logger;


        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryRepository"/> class.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        public CategoryRepository(IConnectionFactory connectionFactory, IPCMSLogger logger)
        {
            _connectionFactory = connectionFactory;
            _dbConnection = connectionFactory.GetConnection();
            _logger = logger;
        }

        /// <summary>
        /// Adds the category.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        public Task<int> AddCategory(CategoryModel category)
        {
            IDbTransaction transaction = null;
            try
            {
                _connectionFactory.OpenConnection();
                using (transaction = _connectionFactory.BeginTransaction(_dbConnection))
                {
                    int result;
                    var query = "SpInsertUpdateCategory";
                    var param = new DynamicParameters();
                    param.Add("@CategoryId", category.CategoryId);
					param.Add("@Color", category.Color);
					param.Add("@Name", category.Name);
                    result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, transaction, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();

                    transaction.Commit();
                    return Task.FromResult(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to save category");
                if (transaction != null)
                {
                    transaction.Rollback();
                }
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
        /// Deletes the category.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Task<int> DeleteCategory(int id)
        {
            IDbTransaction transaction = null;
            try
            {
                _connectionFactory.OpenConnection();
                using (transaction = _connectionFactory.BeginTransaction(_dbConnection))
                {
                    int result;
                    var query = "SpDeleteCategory";
                    var param = new DynamicParameters();
                    param.Add("@CategoryId", id);
                    result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, transaction, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                    transaction.Commit();
                    return Task.FromResult(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete category");
                if (transaction != null)
                {
                    transaction.Rollback();
                }
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
        /// Retrieves the category.
        /// </summary>
        /// <param name="flag">The flag.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public async Task<IEnumerable<CategoryModel>> RetrieveCategory(string flag, string value)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpSelectAllCategories";
                var param = new DynamicParameters();
                param.Add("@Flag", flag);
                param.Add("@Value", value);
                var result = await SqlMapper.QueryAsync<CategoryModel>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return result;

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve category");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
    }
}
