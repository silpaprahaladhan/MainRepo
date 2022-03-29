using Nirast.Pcms.Api.Sdk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Repositories
{
    public interface IQuestionareRepository : IGenericRepository<QuestionareModel>
    {
        Task<int> AddQuestions(QuestionareModel questions);
        Task<IEnumerable<QuestionareModel>> RetrieveQuestions(int id);
        Task<int> DeleteQuestions(int id);
    }
    
}
