using Nirast.Pcms.Api.Sdk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Repositories
{
   public interface IHomeRepository:IGenericRepository<AdvancedSearchInputModel>
    {
        #region public methods

        /// <summary>
        /// To get caretaker details by advanced searching
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<SearchedCareTakers>> GetcareTakerDetails(AdvancedSearchInputModel inputs);

        Task<IEnumerable<CareTakerRegistrationModel>> KeywordCareTakerSearchDetail(string inputs);

        /// <summary>
        /// To get approved rate
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<CareTakerServices>> GetApprovedRate();
        #endregion

    }
}
