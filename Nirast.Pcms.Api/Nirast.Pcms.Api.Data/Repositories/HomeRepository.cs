using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Nirast.Pcms.Api.Sdk.Entities;
using Nirast.Pcms.Api.Sdk.Infrastructure;
using Nirast.Pcms.Api.Sdk.Logger;
using Nirast.Pcms.Api.Sdk.Repositories;

namespace Nirast.Pcms.Api.Data.Repositories
{
    public class HomeRepository : GenericRepository<AdvancedSearchInputModel>, IHomeRepository
    {
        #region public methods
        IConnectionFactory _connectionFactory;
        IPCMSLogger _logger;
        IDbConnection _dbConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeRepository"/> class.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        /// <param name="logger">The logger.</param>
        public HomeRepository(IConnectionFactory connectionFactory, IPCMSLogger logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _dbConnection = connectionFactory.GetConnection();
        }

        /// <summary>
        /// To get approved rate
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<CareTakerServices>> GetApprovedRate()
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "SpGetDisplayRate";
                var result = await SqlMapper.QueryAsync<CareTakerServices>(_dbConnection, query, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving approved rate");
                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// To get caretaker details by advanced searching
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SearchedCareTakers>> GetcareTakerDetails(AdvancedSearchInputModel inputs)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "spSelectCaretakersForSearch";
                var param = new DynamicParameters();
               
                param.Add("@CategoryId", (inputs.Category==0) ? null : inputs.Category);
                param.Add("@ServiceId", (inputs.Services == 0) ? null : inputs.Services);
                param.Add("@ServiceRate", (inputs.Price == 0) ? null : inputs.Price);
                double? minExp = 0;
                double? maxExp = 0;
                if(inputs.Experience==null)
                {
                    maxExp = null;
                    minExp = null;
                }
               
                if (inputs.Experience == 4)
                {
                    minExp = 0;
                    maxExp = 4;
                }
                if (inputs.Experience == 8)
                {
                    minExp = 4.01;
                    maxExp = 8;
                }
                if (inputs.Experience == 12)
                {
                    minExp = 8.01;
                    maxExp = 12;
                }
                if (inputs.Experience > 12)
                {
                    minExp = 12.01;
                    maxExp = 100;
                }
                param.Add("@MinExperience", minExp);
                param.Add("@MaxExperience", maxExp);
                //param.Add("@Experience", (inputs.Experience == 0) ? null : inputs.Experience);
                param.Add("@ProfileId",  inputs.ProfileId);
                param.Add("@GenderId", (inputs.Gender == 0) ? null : inputs.Gender);
                param.Add("@StateId", (inputs.State == 0) ? null : inputs.State);
                param.Add("@CountryId", (inputs.Country == 0) ? null : inputs.Country);
                param.Add("@CityId", (inputs.City == 0) ? null : inputs.City);
                param.Add("@Location", inputs.Location);

                if (inputs.FromDate !=null && inputs.FromTime != null)
                {
                    TimeSpan fromTime = inputs.FromTime.Value.TimeOfDay;
                    inputs.FromDate = inputs.FromDate.Value.Date + fromTime;
                }
                if (inputs.ToDate != null && inputs.ToTime != null)
                {
                    TimeSpan toTime = inputs.ToTime.Value.TimeOfDay;
                    inputs.ToDate = inputs.ToDate.Value.Date + toTime;
                }

                    

                param.Add("@fromdate", inputs.FromDate);
                param.Add("@todate", inputs.ToDate);
                var result = await SqlMapper.QueryAsync<SearchedCareTakers>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving Caregiver details using advanced search");

                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }

        /// <summary>
        /// Keywords the care taker search detail.
        /// </summary>
        /// <param name="keyword">The keyword.</param>
        /// <returns></returns>
        public async Task<IEnumerable<CareTakerRegistrationModel>> KeywordCareTakerSearchDetail(string keyword)
        {
            try
            {
                _connectionFactory.OpenConnection();
                var query = "spSelectCaretakersKeywordSearch";
                var param = new DynamicParameters();
                param.Add("@Keyword", keyword );
                var result = await SqlMapper.QueryAsync<CareTakerRegistrationModel>(_dbConnection, query, param, commandType: CommandType.StoredProcedure);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DB error occured while retrieving Caregiver details using advanced search");

                return null;
            }
            finally
            {
                _connectionFactory.CloseConnection();
            }
        }
        #endregion
    }
}
