using Nirast.Pcms.Api.Sdk.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Repositories
{
    public interface ILoggedInUserRepository : IGenericRepository<LoggedInUser>
    {
        Task<List<LoggedInUser>> CheckLoginCredentialsCheckLoginCredentials(UserCredential userCredential);
        Task<List<LoggedInUser>> CheckLoginCredentialsCheckLoginCredentialsDouble(UserCredential userCredential);
        Task<List<LoggedInUser>> CheckLoginAdminType(LoggedInUser userCredential);
        Task<LoggedInUser> GetLoggedInUserDetails(int userId);
        Task<bool> AddLoginLog(int userId);
    }
}
