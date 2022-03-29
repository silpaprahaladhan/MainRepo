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
    public class AdminServices : IAdminServices
    {
        IUnitOfWork _unitOfWork;
        public AdminServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }

        public async Task<UserBookingInvoiceReport> GetBookingHistoryDetail(int BookingId)
        {
            return await _unitOfWork.PatientRepository.GetBookingHistoryDetail(BookingId);
        }
        public async Task<UserBookingInvoiceReport> GetAdminDashboardBookingHistoryDetail(int BookingId)
        {
            return await _unitOfWork.PatientRepository.GetAdminDashboardBookingHistoryDetail(BookingId);
        }
        

        public async Task<IEnumerable<UserBookingInvoiceReport>> GetBookingHistoryList(BookingHistorySearch bookingHistorySearch)
        {
            return await _unitOfWork.PatientRepository.GetBookingHistoryList(bookingHistorySearch);
        }
        public async Task<IEnumerable<UserBookingInvoiceReport>> GetBookingHistoryListById(int publicUserId )
        {
            return await _unitOfWork.PatientRepository.GetBookingHistoryListById(publicUserId);
        }
        public async Task<IEnumerable<UserInvoiceParams>> GetBookingInvoiceList(BookingHistorySearch bookingHistorySearch)
        {
            return await _unitOfWork.PatientRepository.GetBookingInvoiceList(bookingHistorySearch);
        }
        public async Task<IEnumerable<UserInvoiceParams>> GetBookingInvoiceListforUserDashBoard(int publicUserId)
        {
            return await _unitOfWork.PatientRepository.GetBookingInvoiceListforUserDashBoard(publicUserId);
        }
        
        public async Task<IEnumerable<UserBookingInvoiceReport>> GetBookingHistoryListForInvoiceGeneration(BookingHistorySearch bookingHistorySearch)
        {
            return await _unitOfWork.PatientRepository.GetBookingHistoryListForInvoiceGeneration(bookingHistorySearch);
        }

        public string DownloadDbBackup(string path)
        {
            return _unitOfWork.PatientRepository.DownloadDbBackup(path);
        }

        public async Task<IEnumerable<CaretakerBookingReport>> GetBookingHistoryReport(CaretakerBookingReportModel caretakerBookingReport)
        {
            return await _unitOfWork.PatientRepository.GetBookingHistoryReport(caretakerBookingReport);
        }

        public async Task<RoleModulePrivileges> GetRolePrivilege(GetRolePrivilegeModel getRolePrivilege)
        {
            return await _unitOfWork.roleRepository.GetRolePrivilege(getRolePrivilege);
        }

       public async Task<UsersDetails> GetUserDetail(int BookingId)
        {
            return await _unitOfWork.PatientRepository.GetUserDetail(BookingId);
        }

        public async Task<IEnumerable<PaymentReportDetails>> GetCaretakerBookings(CaretakerWiseSearchReport bookingHistorySearch)
        {
            return await _unitOfWork.PatientRepository.GetCaretakerBookings(bookingHistorySearch);
        }
    }
}
