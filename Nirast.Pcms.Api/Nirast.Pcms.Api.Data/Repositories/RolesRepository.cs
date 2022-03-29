using Dapper;
using Nirast.Pcms.Api.Sdk.Entities;
using Nirast.Pcms.Api.Sdk.Infrastructure;
using Nirast.Pcms.Api.Sdk.Logger;
using Nirast.Pcms.Api.Sdk.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Data.Repositories
{
    public class RolesRepository : GenericRepository<Roles>, IRoleRepository
    {

        IConnectionFactory _connectionFactory;
        IPCMSLogger _logger;
        IDbConnection _dbConnection;

        public RolesRepository(IConnectionFactory connectionFactory, IPCMSLogger logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _dbConnection = connectionFactory.GetConnection();
        }

        private DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        /// <summary>
        /// method to add roles
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        public Task<int> AddRoles(Roles roles)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpInsertUpdateRoles";
                var param = new DynamicParameters();
                param.Add("@RoleId", roles.RoleId);
                param.Add("@RoleName", roles.RoleName);
                int result= SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while adding roles in to database");
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
        /// method to implement delete roles
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<int> DeleteRoles(int id)
        {
            try
            {
                _connectionFactory.OpenConnection();
                int result;
                var query = "SpDeleteRoles";
                var param = new DynamicParameters();
                param.Add("@roleId", id);
                result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while deleting roles from  database");
                if (ex.InnerException.Message.Contains("REFERENCE constraint"))
                    return Task.FromResult(10002);
                return Task.FromResult(0);
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public Task<int> SaveRolePrivileges(SaveRolePrivileges saveRolePrivileges)
        {
            IDbTransaction transaction = null;
            int result;
            try
            {
                _connectionFactory.OpenConnection();
                DataTable dtRolePrivileges = ConvertToDataTable(saveRolePrivileges.Privileges);
                dtRolePrivileges.Columns.Remove("ModuleName");

                var query = "SpSaveRolePrivilege";
                var param = new DynamicParameters();
                param.Add("@RoleId", saveRolePrivileges.RoleId);
                param.Add("@RolePrivileges", dtRolePrivileges, DbType.Object);
                result = SqlMapper.QueryAsync<int>(_dbConnection, query, param, transaction, commandType: CommandType.StoredProcedure).Result.SingleOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<RoleModulePrivileges> GetRolePrivilege(GetRolePrivilegeModel getRolePrivilege)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpGetViewPrivilegeAccess";
                var param = new DynamicParameters();
                param.Add("@RoleId", getRolePrivilege.RoleId);
                param.Add("@ModuleId", getRolePrivilege.ModuleID);
                var result = await SqlMapper.QueryFirstOrDefaultAsync<RoleModulePrivileges>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                if(result == null)
                {
                    result = new RoleModulePrivileges();
                }
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        public async Task<IEnumerable<Privileges>> SelectRolePrivileges(int RoleId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpSelectPrivileges";
                var param = new DynamicParameters();
                param.Add("@RoleId", RoleId);
                var result = await SqlMapper.QueryAsync<Privileges>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving role privileges.");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        /// <summary>
        /// method to implement retrieve roles
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Roles>> RetrieveRoles(int roleId)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpGetRoles";
                var param = new DynamicParameters();
                param.Add("@roleId", roleId);
                var result = await SqlMapper.QueryAsync<Roles>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving roles from datebase");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }


    }
}
