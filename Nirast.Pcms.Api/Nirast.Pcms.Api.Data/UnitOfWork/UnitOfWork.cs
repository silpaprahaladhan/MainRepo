using Nirast.Pcms.Api.Data.Repositories;
using Nirast.Pcms.Api.Sdk.Repositories;
using Nirast.Pcms.Api.Sdk.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IOfficeStaffRepository _staffRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IStateRepository _stateRepository;
        
        private readonly IUsersDetailsRepository _usersDetailsRepository;
        private readonly ICareTakerRepository _careTakerRepository;
        private readonly ICityRepository _CityRepository;
        private readonly ICategoryRepository _CategoryRepository; 
        private readonly IOrientationRepository _OrientationRepository;
        private readonly IDesignationRepository _DesignationRepository;
        private readonly IQuestionareRepository _QuestionareRepository;
        private readonly IQualificationRepository _QualificationRepository;
        private readonly IPatientRepository _PatientRepository;
        private readonly IHomeRepository _homeRepository;
        private readonly IWorkShiftRepository _workShiftRepository;
        private readonly ITimeShiftRepository _timeShiftRepository;
        private readonly IHolidayRepository _holidayRepository;
        private readonly IClientRepository _clientRepository;
    
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ILoggedInUserRepository _loggedInUserRepository;
        private readonly IRoleRepository _roleRepository;
        public UnitOfWork(ICountryRepository pcmsRepository,IOfficeStaffRepository staffRepository, IUsersDetailsRepository usersDetailsRepository,
            IServiceRepository serviceRepository, ICareTakerRepository careTakerRepository, IStateRepository stateRepository, ICityRepository CityRepository, ICategoryRepository CategoryRepository,ILoggedInUserRepository loggedInUserRepository,
            IOrientationRepository OrientationRepository, IDesignationRepository DesignationRepository, IQualificationRepository QualificationRepository, IPatientRepository PatientRepository,IHomeRepository homeRepository,
            IWorkShiftRepository WorkShiftRepository, ITimeShiftRepository TimeShiftRepository, IHolidayRepository HolidayRepository, IClientRepository clientRepository, IInvoiceRepository invoiceRepository,IQuestionareRepository questionareRepository,IRoleRepository roleRepository)
        {
            _countryRepository = pcmsRepository;
            _staffRepository = staffRepository;
            _serviceRepository = serviceRepository;
            _stateRepository = stateRepository;
            _usersDetailsRepository = usersDetailsRepository;
            _careTakerRepository = careTakerRepository;
            _CityRepository = CityRepository;
            _CategoryRepository = CategoryRepository;
            _OrientationRepository = OrientationRepository;
            _DesignationRepository = DesignationRepository;
            _QualificationRepository = QualificationRepository;
            _PatientRepository = PatientRepository;
            _homeRepository = homeRepository;
            _workShiftRepository = WorkShiftRepository;
            _timeShiftRepository = TimeShiftRepository;
            _holidayRepository = HolidayRepository;
            _clientRepository = clientRepository;
            _invoiceRepository = invoiceRepository;
            _loggedInUserRepository = loggedInUserRepository;
            _QuestionareRepository = questionareRepository;
         
            _roleRepository = roleRepository;
        }

        void IUnitOfWork.Complete()
        {
            throw new NotImplementedException();
        }

        public ICountryRepository CountryRepository
        {
            get
            {
                return _countryRepository;
            }
        }

        public IOfficeStaffRepository OfficeStaffReposoitory
        {
            get
            {
                return _staffRepository;
            }
        }

        public IServiceRepository ServiceReposoitory
        {
            get
            {
                return _serviceRepository;
            }
        }

        public IStateRepository StateRepository
        {
            get
            {
                return _stateRepository;
            }
        }

        public ICareTakerRepository CareTakerRepository
        {
            get
            {
                return _careTakerRepository;
            }
        }

        public IUsersDetailsRepository UsersDetailsRepository
        {
            get
            {
                return _usersDetailsRepository;
            }
        }

        public ICityRepository CityRepository
        {
            get
            {
                return _CityRepository;
            }
        }
        public ICategoryRepository CategoryRepository
        {
            get
            {
                return _CategoryRepository;
            }
        }

        public IOrientationRepository OrientationRepository
        {
            get
            {
                return _OrientationRepository;
            }
        }

        public IDesignationRepository DesignationRepository
        {
            get
            {
                return _DesignationRepository;
            }
        }

        public IQualificationRepository QualificationRepository
        {
            get
            {
                return _QualificationRepository;
            }
        }
        public IPatientRepository PatientRepository
        {
            get
            {
                return _PatientRepository;
            }
        }

        public IHomeRepository homeRepository
        {
            get
            {
                return _homeRepository;
            }
        }

        public IWorkShiftRepository WorkShiftRepository
        {
            get
            {
                return _workShiftRepository;
            }
        }

        public ITimeShiftRepository TimeShiftRepository
        {
            get
            {
                return _timeShiftRepository;
            }
        }

        public IHolidayRepository HolidayRepository
        {
            get
            {
                return _holidayRepository;
            }
        }
        public IClientRepository ClientRepository
        {
            get
            {
                return _clientRepository;
            }
        }
  
        public IClientRepository clientRepository
        {
            get
            {
                return _clientRepository;
            }
        }
        public IInvoiceRepository invoiceRepository
        {
            get
            {
                return _invoiceRepository;
            }
        }

        public ILoggedInUserRepository loggedInUserRepository
        {
            get
            {
                return _loggedInUserRepository;
            }
        }
        public IQuestionareRepository QuestionareRepository
        {
            get
            {
                return _QuestionareRepository;
            }
        }

        public IRoleRepository roleRepository
        {
            get
            {
                return _roleRepository;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls



        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UnitOfWork() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            //Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
