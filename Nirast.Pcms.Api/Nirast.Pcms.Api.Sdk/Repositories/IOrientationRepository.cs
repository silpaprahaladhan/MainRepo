using Nirast.Pcms.Api.Sdk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Repositories
{
    public interface IOrientationRepository : IGenericRepository<Orientation>
    {
        Task<int> AddOrientation(Orientation Orientation);

        List<Orientation> RetrieveOrientation(int OrientationId);
        Task<int> DeleteOrientation(int OrientationId);
    }
}
