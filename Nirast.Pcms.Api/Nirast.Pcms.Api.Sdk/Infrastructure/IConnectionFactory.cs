using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Infrastructure
{
    public interface IConnectionFactory : IDisposable
    {
        IDbTransaction BeginTransaction(IDbConnection dbConnection);
        void Commit(IDbTransaction dbTransaction);
        void Rollback(IDbTransaction dbTransaction);
        void OpenConnection();
        void CloseConnection();
        IDbConnection GetConnection();
    }
}
