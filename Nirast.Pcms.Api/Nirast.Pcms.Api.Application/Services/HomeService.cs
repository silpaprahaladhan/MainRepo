using Nirast.Pcms.Api.Sdk.Entities;
using Nirast.Pcms.Api.Sdk.Services;
using Nirast.Pcms.Api.Sdk.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Application.Services
{
    public class HomeService : IHomeService
    {
        IUnitOfWork _unitOfWork;
        public HomeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #region publlic methods

        /// <summary>
        /// To get caretaker details by advanced search
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SearchedCareTakers>> RetrievecareTakerDetails(AdvancedSearchInputModel inputs)
        {
            return await _unitOfWork.homeRepository.GetcareTakerDetails(inputs);
        }

        public async Task<IEnumerable<CareTakerRegistrationModel>> KeywordCareTakerSearchDetail(string keyword)
        {
            return await _unitOfWork.homeRepository.KeywordCareTakerSearchDetail(keyword);
        }

        /// <summary>
        /// To get approved rate
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<CareTakerServices>> RetrievedApprovedRate()
        {
            return await _unitOfWork.homeRepository.GetApprovedRate();
        }
        #endregion
    }
}
