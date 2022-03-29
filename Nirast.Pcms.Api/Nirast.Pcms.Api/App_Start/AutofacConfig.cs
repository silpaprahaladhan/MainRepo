using Autofac;
using Autofac.Integration.WebApi;
using Nirast.Pcms.Ap.Application.Infrastructure;
using Nirast.Pcms.Api.Application.Services;
using Nirast.Pcms.Api.Data.Infrastructure;
using Nirast.Pcms.Api.Data.Repositories;
using Nirast.Pcms.Api.Data.UnitOfWork;
using Nirast.Pcms.Api.Logger;
using Nirast.Pcms.Api.Sdk.Infrastructure;
using Nirast.Pcms.Api.Sdk.Logger;
using Nirast.Pcms.Api.Sdk.Repositories;
using Nirast.Pcms.Api.Sdk.Services;
using Nirast.Pcms.Api.Sdk.UnitOfWork;
using System.Reflection;
using System.Web.Http;

namespace Nirast.Pcms.Api
{
    public class AutofacConfig
    {
        public static IContainer Container;

        public static void Initialize(HttpConfiguration config)
        {
            config.EnableCors();
            Initialize(config, RegisterServices(new ContainerBuilder()));
        }

        public static void Initialize(HttpConfiguration config, IContainer container)
        {
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        private static IContainer RegisterServices(ContainerBuilder builder)
        {
            //Register your Web API controllers.  
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.RegisterType<PCMSLogger>()
                   .As<IPCMSLogger>()
                   .InstancePerRequest();

            builder.RegisterType<ConnectionFactory>()
                   .As<IConnectionFactory>()
                   .InstancePerRequest();

            builder.RegisterType<PCMSService>()
                   .As<IPCMSService>()
                   .InstancePerRequest();

            builder.RegisterType<AdminServices>()
                   .As<IAdminServices>()
                   .InstancePerRequest();

            builder.RegisterType<OfficeStaffService>()
                  .As<IOfficeStaffService>()
                  .InstancePerRequest();

            builder.RegisterType<ServicesService>()
                 .As<IServicesService>()
                 .InstancePerRequest();

            builder.RegisterType<HomeService>()
                 .As<IHomeService>()
                 .InstancePerRequest();

            builder.RegisterType<ClientService>()
                 .As<IClientService>()
                 .InstancePerRequest();

            builder.RegisterType<InvoiceService>()
                .As<IInvoiceService>()
                .InstancePerRequest();

            builder.RegisterType<NotificationService>()
                .As<INotificationService>()
                .InstancePerRequest();

            builder.RegisterType<CountryRepository>()
                   .As<ICountryRepository>()
                   .InstancePerRequest();

            builder.RegisterType<OfficeStaffRepository>()
                  .As<IOfficeStaffRepository>()
                  .InstancePerRequest();

            builder.RegisterType<ServiceRepository>()
                .As<IServiceRepository>()
                .InstancePerRequest();

            builder.RegisterType<StateRepository>()
                .As<IStateRepository>()
                .InstancePerRequest();

            builder.RegisterGeneric(typeof(GenericRepository<>))
                   .As(typeof(IGenericRepository<>))
                   .InstancePerRequest();

            builder.RegisterType<CareTakerRepository>()
                   .As<ICareTakerRepository>()
                   .InstancePerRequest();

            builder.RegisterType<UsersDetailsRepository>()
                  .As<IUsersDetailsRepository>()
                  .InstancePerRequest();

            builder.RegisterType<CityRepository>()
                  .As<ICityRepository>()
                  .InstancePerRequest();

            builder.RegisterType<CategoryRepository>()
                 .As<ICategoryRepository>()
                 .InstancePerRequest();

            builder.RegisterType<OrientationRepository>()
               .As<IOrientationRepository>()
               .InstancePerRequest();

            builder.RegisterType<DesignationRepository>()
               .As<IDesignationRepository>()
               .InstancePerRequest();

            builder.RegisterType<QualificationRepository>()
               .As<IQualificationRepository>()
               .InstancePerRequest();

            builder.RegisterType<PatientRepository>()
            .As<IPatientRepository>()
            .InstancePerRequest();

            builder.RegisterType<HomeRepository>()
               .As<IHomeRepository>()
               .InstancePerRequest();

            builder.RegisterType<WorkShiftRepository>()
               .As<IWorkShiftRepository>()
               .InstancePerRequest();

            builder.RegisterType<TimeShiftRepository>()
               .As<ITimeShiftRepository>()
               .InstancePerRequest();

            builder.RegisterType<HolidayRepository>()
               .As<IHolidayRepository>()
               .InstancePerRequest();

            builder.RegisterType<ClientRepository>()
               .As<IClientRepository>()
               .InstancePerRequest();

            builder.RegisterType<InvoiceRepository>()
               .As<IInvoiceRepository>()
               .InstancePerRequest();

            builder.RegisterType<LoggedInUserRepository>()
               .As<ILoggedInUserRepository>()
               .InstancePerRequest();

            builder.RegisterType<QuestionareRepository>()
             .As<IQuestionareRepository>()
             .InstancePerRequest();

            builder.RegisterType<RolesRepository>()
            .As<IRoleRepository>()
            .InstancePerRequest();

            builder.RegisterType<UnitOfWork>()
                   .As<IUnitOfWork>()
                   .InstancePerRequest();

            //builder.RegisterFilterProvider();
            //builder.RegisterType<ApiAuthorizeAttribute>().PropertiesAutowired();

            //Set the dependency resolver to be Autofac.  
            Container = builder.Build();

            return Container;
        }
    }
}