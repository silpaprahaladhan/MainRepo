using Nirast.Pcms.Api.Sdk.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        ICountryRepository CountryRepository { get; }
        IOfficeStaffRepository OfficeStaffReposoitory { get; }
        IServiceRepository ServiceReposoitory { get; }
        IStateRepository StateRepository { get; }
        ICareTakerRepository CareTakerRepository { get; }
        IUsersDetailsRepository UsersDetailsRepository { get; }
        ICityRepository CityRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IOrientationRepository OrientationRepository { get; }
        IDesignationRepository DesignationRepository { get; }
        IQualificationRepository QualificationRepository { get; }
        IPatientRepository PatientRepository { get; }
        IHomeRepository homeRepository { get; }
        IWorkShiftRepository WorkShiftRepository { get; }
        ITimeShiftRepository TimeShiftRepository { get; }
        IHolidayRepository HolidayRepository { get; }
        IClientRepository clientRepository { get; }
        IInvoiceRepository invoiceRepository { get; }
        ILoggedInUserRepository loggedInUserRepository { get; }
        IQuestionareRepository QuestionareRepository { get; }
        IRoleRepository roleRepository { get; }
       
        void Complete();
    }
}
