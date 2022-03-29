using Nirast.Pcms.Api.Sdk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Repositories
{
    public interface IQualificationRepository : IGenericRepository<QualificationDetails> 
    {
        Task<int> AddQualification(QualificationDetails Qualification);

        Task<IEnumerable<QualificationDetails>> RetrieveQualification(int QualificationId);
        Task<int> DeleteQualification (int id);
    }
}
