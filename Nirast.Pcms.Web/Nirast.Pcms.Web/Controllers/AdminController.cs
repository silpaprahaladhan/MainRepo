using iTextSharp.text.pdf;
using Newtonsoft.Json;
using Nirast.Pcms.Web.Helpers;
using Nirast.Pcms.Web.Logger;
using Nirast.Pcms.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.Mvc;
using System.Linq;
using static Nirast.Pcms.Web.Models.Enums;
using System.Web.Script.Serialization;
using System.Net.Mail;
using System.Globalization;
using System.DirectoryServices.Protocols;

namespace Nirast.Pcms.Web.Controllers
{
    [OutputCache(Duration = 1800, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
    public class AdminController : Controller
    {
        PCMSLogger Logger = new PCMSLogger();
        Service service = null;

        public AdminController()
        {
            service = new Service(Logger);
        }

        public SelectList GetYears()
        {
            List<Years> years = new List<Years>();
            SelectList YearList = null;
            Years year = new Years();
            year.Id = 0;
            year.Year = "--Year--";
            for (int i = 2017; i <= DateTime.Now.Year + 5; i++)
            {
                year = new Years();
                year.Id = i;
                year.Year = i.ToString();
                years.Add(year);
            }

            YearList = new SelectList(years, "Id", "Year", DateTime.Now.Year);
            return YearList;
        }

        public ActionResult InvoiceHistory()
        {
            ClientModel clientModelObj = new ClientModel();
            string api = string.Empty;


            string country = Session["CountryId"].ToString();
            string state = Session["StateID"].ToString();
            string CityIdk = Session["CityIdk"].ToString();
            if (Session["UserType"] != null)
            {
                if(country!=""&&state!=""&&CityIdk!="")
                {
                    List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                    api = "Admin/GetCountryDetails/0";
                    var countrylist1 = service.GetAPI(api);
                    countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                    ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                    List<StateViewModel> stateList1 = new List<StateViewModel>();
                    api = "/Admin/GetStatesByCountryId/"+country;
                    var stateResult1 = service.GetAPI(api);
                    stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                    ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", state);

                    api = "Admin/GetCity?flag=StateId&value=" + state;
                    List<Cities> cityList1 = new List<Cities>();
                    var resultCity1 = service.GetAPI(api);
                    cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                    ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", CityIdk);

                    //api = "Admin/GetAllBranch/";
                    api = "Admin/GetBranch?flag=StateId&value=" + CityIdk;
                    List<Cities> branchList1 = new List<Cities>();
                    var resultBranch1 = service.GetAPI(api);
                    cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultBranch1);
                    ViewData["ListBranch"] = new SelectList(cityList1, "branchId", "branchName", clientModelObj.BranchId2);
                }
                else
                {
                    List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                    api = "Admin/GetCountryDetails/0";
                    var countrylist1 = service.GetAPI(api);
                    countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                    ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                    List<StateViewModel> stateList1 = new List<StateViewModel>();
                    api = "/Admin/GetStatesByCountryId/1";
                    var stateResult1 = service.GetAPI(api);
                    stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                    ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", clientModelObj.StateId1);

                    api = "Admin/GetCity?flag=StateId&value=" + clientModelObj.StateId1;
                    List<Cities> cityList1 = new List<Cities>();
                    var resultCity1 = service.GetAPI(api);
                    cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                    ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", clientModelObj.CityId1);

                    api = "Admin/GetAllBranch/";
                    List<Cities> branchList1 = new List<Cities>();
                    var resultBranch1 = service.GetAPI(api);
                    cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultBranch1);
                    ViewData["ListBranch"] = new SelectList(cityList1, "branchId", "branchName", clientModelObj.BranchId2);
                }

                if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                {
                    ViewBag.Error = Constants.NoViewPrivilege;
                    return View();
                }
                if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                {
                    api = "Admin/GetRolePrivilege";
                    GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                    getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                    getRolePrivilege.ModuleID = 24;
                    var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                    var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                    RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                    if (!apiResults.AllowView)
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    ViewBag.AllowEdit = apiResults.AllowEdit;
                }
                if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                {
                    ViewBag.AllowEdit = true;
                    ViewBag.AllowView = true;
                }
            }
            else
            {
                Logger.Error("Redirect to Login from InvoiceHistory-Admin Controller");
                TempData["Failure"] = Constants.NotLoggedIn;
                return RedirectToAction("Login", "Account");
            }
            var listPaySearch = new SelectList(new[]
            {
                new { ID = "0", Name = "--Select--" },
                new { ID = "1", Name = "Yearly" },
                new { ID = "2", Name = "Monthly" },
                new { ID = "3", Name = "Date Range" },
            },
            "ID", "Name", 0);

            ViewData["listPaySearch"] = listPaySearch;
            ViewData["listYears"] = GetYears();
            Months months = new Months();
            ViewData["listMonths"] = months.GetMonths();
            #region created by silpa
            var inputs = new LocationSearchInputs();
            HomeController home = new HomeController();
            var loggedInUserId = (int)Session["loggedInUserId"];
            //get Work Role
            var userRoleDetails = home.GetUserRoleDetails(loggedInUserId);
            if (userRoleDetails != null)
            {

                if (userRoleDetails.WorkRoleId == 1)
                {
                    inputs.CountryId = userRoleDetails.CountryId;
                    inputs.StateId = null;
                    inputs.CityId = null;
                    ViewData["UserWorkRole"] = 1;
                }
                if (userRoleDetails.WorkRoleId == 2)
                {
                    inputs.CountryId = userRoleDetails.CountryId;
                    inputs.StateId = userRoleDetails.StateId;
                    inputs.CityId = null;
                    ViewData["UserWorkRole"] = 2;
                }
                if (userRoleDetails.WorkRoleId == 3)
                {
                    inputs.CountryId = userRoleDetails.CountryId;
                    inputs.StateId = userRoleDetails.StateId;
                    inputs.CityId = userRoleDetails.CityId;
                    ViewData["UserWorkRole"] = 3;
                }
                if (userRoleDetails.WorkRoleId == 4)
                {
                    inputs.CountryId = userRoleDetails.CountryId;
                    inputs.StateId = userRoleDetails.StateId;
                    inputs.CityId = userRoleDetails.CityId;
                    ViewData["UserWorkRole"] = 4;
                }
            }
            else
            {
                inputs.CountryId = null;
                inputs.StateId = null;
                inputs.CityId = null;
                ViewData["UserWorkRole"] = null;
            }
            #endregion

            return View();
        }
        public ActionResult ManageInvoice()
        {
            ClientModel clientModelObj = new ClientModel();
            string api = string.Empty;
            string country = Session["CountryId"].ToString();
            string state = Session["StateID"].ToString();
            string CityIdk = Session["CityIdk"].ToString();
            if (Session["UserType"] != null)
            {
                if(country!=""&&state!=""&&CityIdk!="")
                {
                    List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                    api = "Admin/GetCountryDetails/0";
                    var countrylist1 = service.GetAPI(api);
                    countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                    ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                    List<StateViewModel> stateList1 = new List<StateViewModel>();
                    api = "/Admin/GetStatesByCountryId/" + country;
                    var stateResult1 = service.GetAPI(api);
                    stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                    ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", clientModelObj.StateId1);

                    api = "Admin/GetCity?flag=StateId&value=" + state;
                    List<Cities> cityList1 = new List<Cities>();
                    var resultCity1 = service.GetAPI(api);
                    cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                    ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", clientModelObj.CityId1);

                    api = "Admin/GetBranch?flag=StateId&value=" + CityIdk;
                   // api = "Admin/GetAllBranch/";
                    List<Cities> branchList1 = new List<Cities>();
                    var resultBranch1 = service.GetAPI(api);
                    cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultBranch1);
                    ViewData["ListBranch"] = new SelectList(cityList1, "branchId", "branchName", clientModelObj.BranchId2);

                }
                else
                {
                    List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                    api = "Admin/GetCountryDetails/0";
                    var countrylist1 = service.GetAPI(api);
                    countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                    ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                    List<StateViewModel> stateList1 = new List<StateViewModel>();
                    api = "/Admin/GetStatesByCountryId/1" ;
                    var stateResult1 = service.GetAPI(api);
                    stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                    ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", clientModelObj.StateId1);

                    api = "Admin/GetCity?flag=StateId&value=" + clientModelObj.StateId1;
                    List<Cities> cityList1 = new List<Cities>();
                    var resultCity1 = service.GetAPI(api);
                    cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                    ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", clientModelObj.CityId1);

                    api = "Admin/GetAllBranch/";
                    List<Cities> branchList1 = new List<Cities>();
                    var resultBranch1 = service.GetAPI(api);
                    cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultBranch1);
                    ViewData["ListBranch"] = new SelectList(cityList1, "branchId", "branchName", clientModelObj.BranchId2);

                }

                if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                {
                    ViewBag.Error = Constants.NoViewPrivilege;
                    return View();
                }
                if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                {
                    api = "Admin/GetRolePrivilege";
                    GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                    getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                    getRolePrivilege.ModuleID = 24;
                    var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                    var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                    RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                    if (!apiResults.AllowView)
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    ViewBag.AllowEdit = apiResults.AllowEdit;
                }
                if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                {
                    ViewBag.AllowEdit = true;
                    ViewBag.AllowView = true;
                }
            }
            else
            {
                Logger.Error("Redirect to Login from InvoiceHistory-Admin Controller");
                TempData["Failure"] = Constants.NotLoggedIn;
                return RedirectToAction("Login", "Account");
            }
            var listPaySearch = new SelectList(new[]
            {
                new { ID = "0", Name = "--Select--" },
                new { ID = "1", Name = "Yearly" },
                new { ID = "2", Name = "Monthly" },
                new { ID = "3", Name = "Date Range" },
            },
            "ID", "Name", 0);

            ViewData["listPaySearch"] = listPaySearch;
            ViewData["listYears"] = GetYears();
            Months months = new Months();
            ViewData["listMonths"] = months.GetMonths();
            var inputs = new LocationSearchInputs();
            HomeController home = new HomeController();
            var loggedInUserId = (int)Session["loggedInUserId"];
            //get Work Role
            var userRoleDetails = home.GetUserRoleDetails(loggedInUserId);
            if (userRoleDetails != null)
            {

                if (userRoleDetails.WorkRoleId == 1)
                {
                    inputs.CountryId = userRoleDetails.CountryId;
                    inputs.StateId = null;
                    inputs.CityId = null;
                    ViewData["UserWorkRole"] = 1;
                }
                if (userRoleDetails.WorkRoleId == 2)
                {
                    inputs.CountryId = userRoleDetails.CountryId;
                    inputs.StateId = userRoleDetails.StateId;
                    inputs.CityId = null;
                    ViewData["UserWorkRole"] = 2;
                }
                if (userRoleDetails.WorkRoleId == 3)
                {
                    inputs.CountryId = userRoleDetails.CountryId;
                    inputs.StateId = userRoleDetails.StateId;
                    inputs.CityId = userRoleDetails.CityId;
                    ViewData["UserWorkRole"] = 3;
                }
                if (userRoleDetails.WorkRoleId == 4)
                {
                    inputs.CountryId = userRoleDetails.CountryId;
                    inputs.StateId = userRoleDetails.StateId;
                    inputs.CityId = userRoleDetails.CityId;
                    ViewData["UserWorkRole"] = 4;
                }

            }
            else
            {
                inputs.CountryId = null;
                inputs.StateId = null;
                inputs.CityId = null;
                ViewData["UserWorkRole"] = null;
            }

            return View();
        }
        public ActionResult ScheduleCaregiver(int bookingId)
        {
            UserBooking userbookingDetails = null;
            string api = string.Empty;
            if (Session["UserType"] != null)
            {

                if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF" || Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                {
                    api = "User/GetPublicUserBookingDetailsById/" + bookingId;
                    var result = service.GetAPI(api);
                    userbookingDetails = Newtonsoft.Json.JsonConvert.DeserializeObject<UserBooking>(result);
                    ViewBag.PublicUserId = userbookingDetails.PublicUserId;
                    ViewBag.BookingId = bookingId;
                    ViewBag.ServiceID = userbookingDetails.ServiceId;
                }
                if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF" || Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                {
                    ViewBag.AllowEdit = true;
                    ViewBag.AllowView = true;
                    ViewBag.AllowDelete = true;
                }
                else
                {
                    Logger.Error("Redirect to Login from InvoiceHistory-Admin Controller");
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }

            }
            return View(userbookingDetails);
        }



        public ActionResult CompanySettings()
        {
            List<CountryViewModel> listCountry = new List<CountryViewModel>();
            List<StateViewModel> listState = new List<StateViewModel>();
            CompanyProfile companyProfile = new CompanyProfile();
            try
            {
                TempData["Userupdate"] = null;
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 25;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    Logger.Error("Redirect to Login from CompanySettings-Admin Controller");
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                api = "Admin/GetCompanyProfiles/0";
                var result = service.GetAPI(api);
                CompanyProfile listCompanyProfile = JsonConvert.DeserializeObject<CompanyProfile>(result);
                companyProfile = listCompanyProfile;
                int defaultCountry = 0;
                int defaultState = 0;
                string countryApi = "Admin/GetCountryDetails/0";
                var countryResult = service.GetAPI(countryApi);
                listCountry = JsonConvert.DeserializeObject<List<CountryViewModel>>(countryResult);
                List<Cities> cityList = new List<Cities>();
                if (companyProfile == null)
                {
                    defaultCountry = (listCountry.Where(x => x.Isdefault == true).Count() > 0) ? listCountry.Where(x => x.Isdefault == true).SingleOrDefault().CountryId : listCountry.FirstOrDefault().CountryId;
                    ViewBag.City = new SelectList(cityList);
                }
                else
                {
                    defaultCountry = companyProfile.CountryId;
                    defaultState = companyProfile.StateId;
                    string stateApi = "Admin/GetStatesByCountryId/" + defaultCountry;

                    var stateResult = service.GetAPI(stateApi);
                    listState = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult);
                var _listState= new SelectList(listState, "StateId", "Name", defaultState);
                ViewData["States"] = _listState;
                    string apiCity = "Admin/GetCity?flag=StateId&value=" + companyProfile.StateId;
                    var resultCity = service.GetAPI(apiCity);
                    cityList = JsonConvert.DeserializeObject<List<Cities>>(resultCity);
                    ViewBag.City = new SelectList(cityList, "CityId", "CityName", companyProfile.CityId);
                }
                var _listCountry = new SelectList(listCountry, "CountryId", "Name", defaultCountry);
                ViewData["CountryList"] = _listCountry;
              
                
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-ViewOfficeStaffProfile");

            }
            return View(companyProfile);
        }

        [HttpPost]
        public ActionResult CompanySettings(CompanyProfile companyProfile)
        {
            List<CountryViewModel> listCountry = new List<CountryViewModel>();
            List<StateViewModel> listState = new List<StateViewModel>();
            //CompanyProfile companyProfile = new CompanyProfile();
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 25;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    Logger.Error("Redirect to Login from CompanySettings-Admin Controller");
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                string countryApi = "Admin/GetCountryDetails/0";
                var countryResult = service.GetAPI(countryApi);
                listCountry = JsonConvert.DeserializeObject<List<CountryViewModel>>(countryResult);
                ViewBag.Country = new SelectList(listCountry, "CountryId", "Name", 0);
                string stateApi = "Admin/GetStateDetails/0";
                var stateResult = service.GetAPI(stateApi);
                listState = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult);
                ViewBag.States = new SelectList(listState, "StateId", "Name", 0);
                List<Cities> cityList = new List<Cities>();
                string apiCity = "Admin/GetCity?flag=*&value=''";
                var resultCity = service.GetAPI(apiCity);
                cityList = JsonConvert.DeserializeObject<List<Cities>>(resultCity);
                ViewBag.City = new SelectList(cityList, "CityId", "CityName", 0);

                if (companyProfile.LogoImage == null && companyProfile.Logo == null)
                {
                    companyProfile.Logo = System.IO.File.ReadAllBytes(Server.MapPath(@"~/images/logo_sample.png"));
                }
                else if (companyProfile.LogoImage != null)
                {
                    using (var binaryReader = new BinaryReader(companyProfile.LogoImage.InputStream))
                    {
                        companyProfile.Logo = binaryReader.ReadBytes(companyProfile.LogoImage.ContentLength);
                    }
                }
                api = "Admin/InsertUpdateCompany";
                var companyContent = JsonConvert.SerializeObject(companyProfile);
                HttpStatusCode result = service.PostAPI(companyContent, api);
                TempData["Userupdate"] = "Company details saved successfully.";
                //api = "Admin/GetCompanyProfiles/0";
                //var companyResult = service.GetAPI(api);
                //List<CompanyProfile> listCompanyProfile = JsonConvert.DeserializeObject<List<CompanyProfile>>(companyResult);
                //companyProfile = listCompanyProfile.First();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-ViewOfficeStaffProfile");

            }
            return RedirectToAction("CompanySettings");
        }
        public ActionResult CaretakerBookingReport()
        {
            ClientModel clientModelObj = new ClientModel();
            string api = string.Empty;


            string country = Session["CountryId"].ToString();
            string state = Session["StateID"].ToString();
            string CityIdk = Session["CityIdk"].ToString();
            if (Session["UserType"] != null)
            {

                if(country!=""&&state!=""&&CityIdk!="")
                {
                    List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                    api = "Admin/GetCountryDetails/0";
                    var countrylist1 = service.GetAPI(api);
                    countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                    ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                    List<StateViewModel> stateList1 = new List<StateViewModel>();
                    api = "/Admin/GetStatesByCountryId/"+country;
                    var stateResult1 = service.GetAPI(api);
                    stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                    ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", state);

                    api = "Admin/GetCity?flag=StateId&value=" + state;
                    List<Cities> cityList1 = new List<Cities>();
                    var resultCity1 = service.GetAPI(api);
                    cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                    ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", CityIdk);

                    api = "Admin/GetBranch?flag=StateId&value=" + CityIdk;
                    //api = "Admin/GetAllBranch/";
                    List<Cities> branchList1 = new List<Cities>();
                    var resultBranch1 = service.GetAPI(api);
                    cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultBranch1);
                    ViewData["ListBranch"] = new SelectList(cityList1, "branchId", "branchName", clientModelObj.BranchId2);
                }
                else
                {
                    List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                    api = "Admin/GetCountryDetails/0";
                    var countrylist1 = service.GetAPI(api);
                    countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                    ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                    List<StateViewModel> stateList1 = new List<StateViewModel>();
                    api = "/Admin/GetStatesByCountryId/1";
                    var stateResult1 = service.GetAPI(api);
                    stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                    ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", clientModelObj.StateId1);

                    api = "Admin/GetCity?flag=StateId&value=" + clientModelObj.StateId1;
                    List<Cities> cityList1 = new List<Cities>();
                    var resultCity1 = service.GetAPI(api);
                    cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                    ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", clientModelObj.CityId1);

                    api = "Admin/GetAllBranch/";
                    List<Cities> branchList1 = new List<Cities>();
                    var resultBranch1 = service.GetAPI(api);
                    cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultBranch1);
                    ViewData["ListBranch"] = new SelectList(cityList1, "branchId", "branchName", clientModelObj.BranchId2);
                }
            

                if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                {
                    ViewBag.Error = Constants.NoViewPrivilege;
                    return View();
                }
                if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                {
                    api = "Admin/GetRolePrivilege";
                    GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                    getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                    getRolePrivilege.ModuleID = 24;
                    var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                    var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                    RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                    if (!apiResults.AllowView)
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    ViewBag.AllowEdit = apiResults.AllowEdit;
                }
                if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                {
                    ViewBag.AllowEdit = true;
                    ViewBag.AllowView = true;
                }
            }
            else
            {
                Logger.Error("Redirect to Login from CaretakerBookingReport-Admin Controller");
                TempData["Failure"] = Constants.NotLoggedIn;
                return RedirectToAction("Login", "Account");
            }
            api = "Admin/RetrieveServiceDetails/0";
            var serviveResult = service.GetAPI(api);
            List<ServicesViewModel> listService = new List<ServicesViewModel>();
            listService = JsonConvert.DeserializeObject<List<ServicesViewModel>>(serviveResult);
            ViewBag.Service = new SelectList(listService, "ServiceId", "Name", 0);
            List<CategoryViewModel> listCategory = new List<CategoryViewModel>();
            string apicategory = "Admin/GetCategory?flag=*&value=''";
            var categoryResult = service.GetAPI(apicategory);
            listCategory = JsonConvert.DeserializeObject<List<CategoryViewModel>>(categoryResult);
            ViewBag.Category = new SelectList(listCategory, "CategoryId", "Name", 0);

            var listPaySearch = new SelectList(new[]
            {
                new { ID = "0", Name = "--Select--" },
                new { ID = "1", Name = "Yearly" },
                new { ID = "2", Name = "Monthly" },
                new { ID = "3", Name = "Date Range" },
            },
            "ID", "Name", 0);

            ViewData["listPaySearch"] = listPaySearch;
            ViewData["listYears"] = GetYears();
            Months months = new Months();
            ViewData["listMonths"] = months.GetMonths();
            #region created by silpa
            var inputs = new LocationSearchInputs();
            HomeController home = new HomeController();
            var loggedInUserId = (int)Session["loggedInUserId"];
            //get Work Role
            var userRoleDetails = home.GetUserRoleDetails(loggedInUserId);
            if (userRoleDetails != null)
            {

                if (userRoleDetails.WorkRoleId == 1)
                {
                    inputs.CountryId = userRoleDetails.CountryId;
                    inputs.StateId = null;
                    inputs.CityId = null;
                    ViewData["UserWorkRole"] = 1;
                }
                if (userRoleDetails.WorkRoleId == 2)
                {
                    inputs.CountryId = userRoleDetails.CountryId;
                    inputs.StateId = userRoleDetails.StateId;
                    inputs.CityId = null;
                    ViewData["UserWorkRole"] = 2;
                }
                if (userRoleDetails.WorkRoleId == 3)
                {
                    inputs.CountryId = userRoleDetails.CountryId;
                    inputs.StateId = userRoleDetails.StateId;
                    inputs.CityId = userRoleDetails.CityId;
                    ViewData["UserWorkRole"] = 3;
                }

                if (userRoleDetails.WorkRoleId == 4)
                {
                    inputs.CountryId = userRoleDetails.CountryId;
                    inputs.StateId = userRoleDetails.StateId;
                    inputs.CityId = userRoleDetails.CityId;
                    ViewData["UserWorkRole"] = 4;
                }
            }
            else
            {
                inputs.CountryId = null;
                inputs.StateId = null;
                inputs.CityId = null;
                ViewData["UserWorkRole"] = null;
            }
            #endregion

            return View();
        }
        public ActionResult Interview()
        {
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 23;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowView = apiResults.AllowView;
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowView = true;
                    }
                }
                else
                {
                    Logger.Error("Redirect to Login from Interview-Admin Controller");
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                SignatureModel signatureModel = new SignatureModel();
                //signatureModel.policyContent = System.IO.File.ReadAllText(Server.MapPath("~/Settings/CompanyPolicy.txt"));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-Interview");
            }
            return View();
        }
        public ActionResult PaymentHistory()
        {
            try
            {
                ClientModel clientModelObj = new ClientModel();
                string api = string.Empty;
                string country = Session["CountryId"].ToString();
                string state = Session["StateID"].ToString();
                string CityIdk = Session["CityIdk"].ToString();
                if (Session["UserType"] != null)
                {
                    if(country!=""&&state!=""&&CityIdk!="")
                    {
                        List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                        api = "Admin/GetCountryDetails/0";
                        var countrylist1 = service.GetAPI(api);
                        countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                        ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                        List<StateViewModel> stateList1 = new List<StateViewModel>();
                        api = "/Admin/GetStatesByCountryId/" + country;
                        var stateResult1 = service.GetAPI(api);
                        stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                        ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", state);

                        api = "Admin/GetCity?flag=StateId&value=" + state;
                        List<Cities> cityList1 = new List<Cities>();
                        var resultCity1 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                        ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", CityIdk);

                        api = "Admin/GetBranch?flag=StateId&value=" + CityIdk;
                        //api = "Admin/GetAllBranch/";
                        List<Cities> branchList1 = new List<Cities>();
                        var resultBranch1 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultBranch1);
                        ViewData["ListBranch"] = new SelectList(cityList1, "branchId", "branchName", clientModelObj.BranchId2);
                    }
                    else
                    {
                        List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                        api = "Admin/GetCountryDetails/0";
                        var countrylist1 = service.GetAPI(api);
                        countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                        ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                        List<StateViewModel> stateList1 = new List<StateViewModel>();
                        api = "/Admin/GetStatesByCountryId/1";
                        var stateResult1 = service.GetAPI(api);
                        stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                        ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", clientModelObj.StateId1);

                        api = "Admin/GetCity?flag=StateId&value=" + clientModelObj.StateId1;
                        List<Cities> cityList1 = new List<Cities>();
                        var resultCity1 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                        ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", clientModelObj.CityId1);


                        api = "Admin/GetAllBranch/";
                        List<Cities> branchList1 = new List<Cities>();
                        var resultBranch1 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultBranch1);
                        ViewData["ListBranch"] = new SelectList(cityList1, "branchId", "branchName", clientModelObj.BranchId2);

                    }


                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 21;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                    }
                }
                else
                {
                    Logger.Error("Redirect to Login from PaymentHistory-Admin Controller");
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }


                var inputs = new LocationSearchInputs();
                HomeController home = new HomeController();
                var loggedInUserId = (int)Session["loggedInUserId"];
                //get Work Role
                var userRoleDetails = home.GetUserRoleDetails(loggedInUserId);
                if (userRoleDetails != null)
                {

                    if (userRoleDetails.WorkRoleId == 1)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = null;
                        inputs.CityId = null;
                        ViewData["UserWorkRole"] = 1;
                    }
                    if (userRoleDetails.WorkRoleId == 2)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = userRoleDetails.StateId;
                        inputs.CityId = null;
                        ViewData["UserWorkRole"] = 2;
                    }
                    if (userRoleDetails.WorkRoleId == 3)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = userRoleDetails.StateId;
                        inputs.CityId = userRoleDetails.CityId;
                        ViewData["UserWorkRole"] = 3;
                    }
                    if (userRoleDetails.WorkRoleId == 4)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = userRoleDetails.StateId;
                        inputs.CityId = userRoleDetails.CityId;
                        ViewData["UserWorkRole"] = 4;
                    }
                }
                else
                {
                    inputs.CountryId = null;
                    inputs.StateId = null;
                    inputs.CityId = null;
                    ViewData["UserWorkRole"] = null;
                }

                return View();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-PaymentHistory");
                return null;
            }
        }
        public ActionResult PaymentAdvancedSearch()
        {
            try
            {
                var listPaySearch = new SelectList(new[]
            {
                new { ID = "0", Name = "--Select--" },
                new { ID = "1", Name = "Yearly" },
                new { ID = "2", Name = "Monthly" },
                new { ID = "3", Name = "Date Range" },

            },
            "ID", "Name", 0);

                ViewData["listPaySearch"] = listPaySearch;
                ViewData["listYears"] = GetYears();
                Months months = new Months();
                ViewData["listMonths"] = months.GetMonths();

                string serviceApi = "Admin/RetrieveServiceDetails/0";
                var serviveResult = service.GetAPI(serviceApi);
                List<ServicesViewModel> listService = new List<ServicesViewModel>();
                listService = JsonConvert.DeserializeObject<List<ServicesViewModel>>(serviveResult);
                ViewBag.Service = new SelectList(listService, "ServiceId", "Name", 0);
                List<CategoryViewModel> listCategory = new List<CategoryViewModel>();
                string apicategory = "Admin/GetCategory?flag=*&value=''";
                var categoryResult = service.GetAPI(apicategory);
                listCategory = JsonConvert.DeserializeObject<List<CategoryViewModel>>(categoryResult);
                ViewBag.Category = new SelectList(listCategory, "CategoryId", "Name", 0);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-PaymentAdvancedSearch");
            }
            return PartialView("_PaymentAdvancedSearchPartial");
        }
        public ActionResult PaymentList()
        {
            List<Models.PaymentHistory> paymentDetails = null;
            try
            {
                string api = "Admin/GetPaymentDetails";
                var result = service.GetAPI(api);
                paymentDetails = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Models.PaymentHistory>>(result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-PaymentList");

            }
            return PartialView("_PaymentListPartial", paymentDetails);
        }

        public ActionResult SearchPaymentHistory(PaymentAdvancedSearch searchInputs)
        {
            List<Models.PaymentHistory> apiResults = new List<Models.PaymentHistory>();

            if (searchInputs != null)
            {
                if (searchInputs.FromDate != null && searchInputs.ToDate != null)
                {
                    searchInputs.Year = null;
                    searchInputs.Month = null;
                }
                string api = "Home/SearchPaymentDetails";
                var advancedSearchInputModel = JsonConvert.SerializeObject(searchInputs);
                var result = service.PostAPIWithData(advancedSearchInputModel, api);
                apiResults = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Models.PaymentHistory>>(result.Result);


                //UserSessionManager.SetSearchedCareTaker(this, apiResults);
            }
            return PartialView("_PaymentListPartial", apiResults);
            //  return View(apiResults);


        }
        public ActionResult Designation()
        {
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    Logger.Error("Redirect to Login from Designation-Admin Controller");
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                return View();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-Designation");
                return null;
            }
        }
        public ActionResult DesignationList()
        {
            List<DesignationViewModel> designationList = new List<DesignationViewModel>();
            DesignationViewModel serviceModel = new DesignationViewModel();
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                api = "Admin/GetDesignation/0";
                var result = service.GetAPI(api);
                designationList = JsonConvert.DeserializeObject<List<DesignationViewModel>>(result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-DesignationList");
            }
            return PartialView("_DesignationPartial", designationList);
        }

        public ActionResult AddDesignation(DesignationViewModel designationModel)
        {
            try
            {
                TempData["Success"] = "";
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        TempData["Failure"] = Constants.NoViewPrivilege;
                        return RedirectToAction("Designation");
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowEdit)
                        {
                            TempData["Failure"] = Constants.NoActionPrivilege;
                            return RedirectToAction("Designation");
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    Logger.Error("Redirect to Login from AddDesignation-Admin Controller");
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                designationModel.Designation = designationModel.Designation.Trim();
                api = "Admin/SaveDesignation";
                var serviceContent = JsonConvert.SerializeObject(designationModel);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    TempData["Success"] = "Designation settings added successfully";
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    TempData["Failure"] = "You are not authorized to perform this action.";
                }
                else if (result == HttpStatusCode.Conflict)
                {
                    TempData["Failure"] = "Data already exist. Please enter different data.";
                }
                else
                {
                    TempData["Failure"] = "Update Failed. Please try again later.";
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-AddDesignation");
            }
            return RedirectToAction("Designation");
        }

        public JsonResult DeleteDesignation(int designationId)
        {
            try
            {
                string api = string.Empty;

                api = "Admin/DeleteDesignation/" + designationId;
                var serviceContent = JsonConvert.SerializeObject(designationId);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    return Json("Designation settings deleted successfully");
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    return Json("You are not authorized to perform this action.");
                }
                else if (result == HttpStatusCode.Ambiguous)
                {
                    return Json("Data cannot be deleted due to reference.");
                }
                else
                {
                    return Json("Updation Failed. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-DeleteDesignation");

            }
            return Json(string.Empty);
        }

        public ActionResult Qualification()
        {
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    Logger.Error("Redirect to Login from Qualification-Admin Controller");
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                QualificationViewModel qualificationModel = new QualificationViewModel();
                return View(qualificationModel);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-Qualification");
                return null;
            }
        }

        public ActionResult QualificationList()
        {
            List<QualificationViewModel> qualificationList = new List<QualificationViewModel>();
            QualificationViewModel serviceModel = new QualificationViewModel();
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                api = "Admin/GetQualification/0";
                var result = service.GetAPI(api);
                qualificationList = JsonConvert.DeserializeObject<List<QualificationViewModel>>(result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-QualificationList");

            }
            return PartialView("_QualificationPartial", qualificationList);
        }

        public ActionResult AddQualification(QualificationViewModel qualificationViewModel)
        {
            try
            {
                TempData["Success"] = "";
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        TempData["Failure"] = Constants.NoViewPrivilege;
                        return RedirectToAction("Qualification");
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowEdit)
                        {
                            TempData["Failure"] = Constants.NoActionPrivilege;
                            return RedirectToAction("Qualification");
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    Logger.Error("Redirect to Login from AddQualification-Admin Controller");
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                qualificationViewModel.Qualification = qualificationViewModel.Qualification.Trim();
                api = "Admin/SaveQualification";
                var serviceContent = JsonConvert.SerializeObject(qualificationViewModel);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    TempData["Success"] = "Qualification settings added successfully";
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    TempData["Failure"] = "You are not authorized to perform this action.";
                }
                else if (result == HttpStatusCode.Conflict)
                {
                    TempData["Failure"] = "Data already exist. Please enter different data.";
                }
                else
                {
                    TempData["Failure"] = "Updation Failed. Please try again later.";
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-AddQualification");
            }
            return RedirectToAction("Qualification");
        }

        public JsonResult DeleteQualification(int qualificationId)
        {
            try
            {
                string api = string.Empty;

                api = "Admin/DeleteQualification/" + qualificationId;
                var serviceContent = JsonConvert.SerializeObject(qualificationId);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    return Json("Qualification settings deleted successfully");
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    return Json("You are not authorized to perform this action.");
                }
                else if (result == HttpStatusCode.Ambiguous)
                {
                    return Json("Data cannot be deleted due to reference.");
                }
                else
                {
                    return Json("Updation Failed. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-DeleteQualification");
            }
            return Json("Qualification");
        }

        public ActionResult Questionare()
        {
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    Logger.Error("Redirect to Login from Questionare-Admin Controller");
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                return View();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-Questionare");
                return null;
            }
        }

        public ActionResult Testimonials()
        {
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                return View();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-Questionare");
                return null;
            }
        }


        public ActionResult PayPal()
        {
            PayPalAccount payPalAccount = new PayPalAccount();
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                api = "Admin/GetPayPalAccount/1";
                var result = service.GetAPI(api);
                PayPalAccount payPal = JsonConvert.DeserializeObject<PayPalAccount>(result);
                payPalAccount = payPal;
                return View(payPal);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-Questionare");
                return null;
            }
        }

        public ActionResult TestimonialsList()
        {
            List<Testimonial> testimonials = new List<Testimonial>();
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                api = "Admin/GetTestimonials/0";
                var result = service.GetAPI(api);
                testimonials = JsonConvert.DeserializeObject<List<Testimonial>>(result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-QuestionareList");
            }
            return PartialView("_TestimonialsPartial", testimonials);
        }

        public ActionResult QuestionareList()
        {
            List<Questionare> questions = new List<Questionare>();
            Questionare serviceModel = new Questionare();
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                api = "Admin/GetQuestions/0";
                var result = service.GetAPI(api);
                questions = JsonConvert.DeserializeObject<List<Questionare>>(result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-QuestionareList");
            }
            return PartialView("_QuestionarePartial", questions);
        }
        public ActionResult AddQuestions(Questionare questionareViewModel)
        {
            try
            {
                TempData["Success"] = "";
                questionareViewModel.Questions = questionareViewModel.Questions.Trim();
                string api = "Admin/AddQuestions";
                var serviceContent = JsonConvert.SerializeObject(questionareViewModel);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    TempData["Success"] = "Questionnaire settings added successfully";
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    TempData["Failure"] = "You are not authorized to perform this action.";
                }
                else if (result == HttpStatusCode.Conflict)
                {
                    TempData["Failure"] = "Data already exist. Please enter different data.";
                }
                else
                {
                    TempData["Failure"] = "Updation Failed. Please try again later.";
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-AddQuestions");

            }
            return RedirectToAction("Questionare");
        }

        public JsonResult DeleteQuestions(int questionId)
        {
            try
            {
                string api = string.Empty;

                api = "Admin/DeleteQuestions/" + questionId;
                var serviceContent = JsonConvert.SerializeObject(questionId);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    return Json("Questions deleted successfully");
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    return Json("You are not authorized to perform this action.");
                }
                else if (result == HttpStatusCode.Ambiguous)
                {
                    return Json("Data cannot be deleted due to reference.");
                }
                else
                {
                    return Json("Updation Failed. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-DeleteQuestions");
            }
            return Json(string.Empty);
        }

        public JsonResult DeleteTestimonial(int testimonialId)
        {
            try
            {
                string api = string.Empty;

                api = "Admin/DeleteTestimonial/" + testimonialId;
                var serviceContent = JsonConvert.SerializeObject(testimonialId);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    return Json("Testimonial deleted successfully");
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    return Json("You are not authorized to perform this action.");
                }
                else if (result == HttpStatusCode.Ambiguous)
                {
                    return Json("Data cannot be deleted due to reference.");
                }
                else
                {
                    return Json("Updation Failed. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-DeleteTestimonial");
            }
            return Json(string.Empty);
        }

        public ActionResult AddTestimonials(Testimonial testimonial)
        {
            try
            {
                TempData["Success"] = "";
                string api = "Admin/InsertUpdateTestimonials";
                var serviceContent = JsonConvert.SerializeObject(testimonial);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    TempData["Success"] = "Testimonial settings added successfully";
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    TempData["Failure"] = "You are not authorized to perform this action.";
                }
                else if (result == HttpStatusCode.Conflict)
                {
                    TempData["Failure"] = "Data already exist. Please enter different data.";
                }
                else
                {
                    TempData["Failure"] = "Updation Failed. Please try again later.";
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-AddTestimonials");

            }
            return RedirectToAction("Testimonials");
        }

        [HttpPost]
        public ActionResult AddPayPalAccount(PayPalAccount payPal)
        {
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 25;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                api = "Admin/InsertUpdatePaypalSettings";
                var paypalContent = JsonConvert.SerializeObject(payPal);
                HttpStatusCode result = service.PostAPI(paypalContent, api);
                TempData["Userupdate"] = "PayPal Account details saved successfully.";
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-AddPayPalAccount");

            }
            return RedirectToAction("PayPal");
        }
        //post method when file selected in fileuploder
        [HttpPost]
        public ActionResult Show(HttpPostedFileBase postedFiles)
        {
            InterviewViewModel intModel = new InterviewViewModel();
            try
            {
                TempData["doc"] = "";
                string fname;
                string DocNames = "";
                string pdfDocName = "";
                string spireDocName = string.Empty;
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {

                    HttpPostedFileBase file = files[i];
                    if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                    {
                        string[] testfiles = file.FileName.Split(new char[] { '\\' });
                        fname = testfiles[testfiles.Length - 1];
                    }
                    else
                    {
                        fname = file.FileName;
                    }
                    fname = Path.Combine(Server.MapPath("~/Temp/"), fname);

                    object filename = fname;
                    spireDocName = fname;
                    if (!Directory.Exists(Server.MapPath("~/Temp/")))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Temp/"));
                    }
                    file.SaveAs(fname);
                    DocNames = AddPageNumber(spireDocName);
                }

                intModel.FileName = DocNames;
                // TempData["doc"] = pdfDocName;
                TempData["doc"] = spireDocName;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-Show");
            }
            return Json(intModel);
        }


        //this method fires when iframe is loaded this is invoked in ajax
        public FileResult ShowDocument(string FilePath)
        {
            try
            {
                return File(Server.MapPath("~/Temp/") + FilePath, GetMimeType(FilePath));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-ShowDocument");
                return null;
            }
        }

        public FileResult ShowFileDocument(string FilePath)
        {
            try
            {
                return File(Server.MapPath("~/PCMS/Settings/Company/ConsentForm/") + FilePath, GetMimeType(FilePath));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-ShowDocument");
                return null;
            }
        }
        private string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            try
            {
                string ext = System.IO.Path.GetExtension(fileName).ToLower();
                Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
                if (regKey != null && regKey.GetValue("Content Type") != null)
                    mimeType = regKey.GetValue("Content Type").ToString();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-GetMimeType");
            }
            return mimeType;
        }

        protected string AddPageNumber(string pdfDocName)
        {
            try
            {
                byte[] bytes = System.IO.File.ReadAllBytes(pdfDocName);
                iTextSharp.text.Font blackFont = iTextSharp.text.FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
                using (MemoryStream stream = new MemoryStream())
                {
                    PdfReader reader = new PdfReader(bytes);
                    PdfStamper stamper = new PdfStamper(reader, stream);

                    int pages = reader.NumberOfPages;
                    for (int i = 1; i <= pages; i++)
                    {
                        ColumnText.ShowTextAligned(stamper.GetUnderContent(i), iTextSharp.text.Element.ALIGN_RIGHT, new iTextSharp.text.Phrase(i.ToString(), blackFont), 568f, 15f, 0);
                        PdfContentByte pbunder = stamper.GetUnderContent(i);
                    }
                    stamper.Close();
                    bytes = stream.ToArray();
                }

                string randomName = DateTime.Now.Ticks.ToString();
                string filenname = Server.MapPath("~/Temp/") + randomName + ".pdf";
                string returnFileName = randomName + ".pdf";
                System.IO.File.WriteAllBytes(filenname, bytes);
                return returnFileName;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-AddPageNumber");
            }
            return string.Empty;
        }

        public ActionResult UploadPolicy()
        {
            try
            {
                ViewBag.Message = "Upload Policy Document";
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-UploadPolicy");
            }
            return View();

        }

        public ActionResult TermsAndPolicy()
        {
            string api = string.Empty;
            if (Session["UserType"] != null)
            {
                if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                {
                    TempData["Failure"] = Constants.NoViewPrivilege;
                    return View();
                }
                if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                {
                    api = "Admin/GetRolePrivilege";
                    GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                    getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                    getRolePrivilege.ModuleID = 1;
                    var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                    var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                    RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                    if (!apiResults.AllowDelete)
                    {
                        TempData["Failure"] = Constants.NoActionPrivilege;
                        return View();
                    }
                    ViewBag.AllowEdit = apiResults.AllowEdit;
                }
                if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                {
                    ViewBag.AllowView = true;
                    ViewBag.AllowEdit = true;
                    ViewBag.AllowDelete = true;
                }
            }
            else
            {
                Logger.Error("Redirect to Login from TermsAndPolicy-Admin Controller");
                TempData["Failure"] = Constants.NotLoggedIn;
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        public ActionResult SaveTermsAndPolicy(HttpPostedFileBase termsnCondition, HttpPostedFileBase privacyPolicy, HttpPostedFileBase consentForm)
        {
            string api = string.Empty;
            if (Session["UserType"] != null)
            {
                if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                {
                    TempData["Failure"] = Constants.NoViewPrivilege;
                    return RedirectToAction("TermsAndPolicy");
                }
                if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                {
                    api = "Admin/GetRolePrivilege";
                    GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                    getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                    getRolePrivilege.ModuleID = 1;
                    var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                    var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                    RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                    if (!apiResults.AllowDelete)
                    {
                        TempData["Failure"] = Constants.NoActionPrivilege;
                        return RedirectToAction("TermsAndPolicy");
                    }
                    ViewBag.AllowEdit = apiResults.AllowEdit;
                    ViewBag.AllowDelete = apiResults.AllowDelete;
                }
                if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                {
                    ViewBag.AllowView = true;
                    ViewBag.AllowEdit = true;
                    ViewBag.AllowDelete = true;
                }
            }
            else
            {
                Logger.Error("Redirect to Login from SaveTermsAndPolicy-Admin Controller");
                TempData["Failure"] = Constants.NotLoggedIn;
                return RedirectToAction("Login", "Account");
            }

            bool hasFile = false;
            string path = Path.Combine(Server.MapPath("~/PCMS/Settings/Company/TermsAndPrivacy/"));
            string consentform = Path.Combine(Server.MapPath("~/PCMS/Settings/Company/ConsentForm/"));
            if (termsnCondition != null)
            {
                string fname;
                string DocNames = "";
                string spireDocName = string.Empty;
                HttpPostedFileBase file = termsnCondition;
                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                {
                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                    fname = "TermsAndConditions.pdf";
                }
                else
                {
                    fname = "TermsAndConditions.pdf";
                }
                fname = Path.Combine(Server.MapPath("~/PCMS/Settings/Company/TermsAndPrivacy/"), fname);
                object filename = fname;
                spireDocName = fname;
                if (!Directory.Exists(Server.MapPath("~/PCMS/Settings/Company/TermsAndPrivacy/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/PCMS/Settings/Company/TermsAndPrivacy/"));
                }
                file.SaveAs(fname);
                DocNames = AddPageNumber(spireDocName);
                TempData["doc"] = spireDocName;
                hasFile = true;



                //BinaryReader b = new BinaryReader(termsnCondition.InputStream);
                //byte[] binData = b.ReadBytes(termsnCondition.ContentLength);

                //string result = System.Text.Encoding.UTF8.GetString(binData);
                //if (!Directory.Exists(path))
                //{
                //    DirectoryInfo di = Directory.CreateDirectory(path);
                //}
                //System.IO.File.WriteAllText(path + "TermsAndConditions.txt", result);
                //hasFile = true;
            }
            if (privacyPolicy != null)
            {
                string fname;
                string DocNames = "";
                string spireDocName = string.Empty;
                HttpPostedFileBase file = privacyPolicy;
                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                {
                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                    fname = "PrivacyPolicy.pdf";
                }
                else
                {
                    fname = "PrivacyPolicy.pdf";
                }
                fname = Path.Combine(Server.MapPath("/PCMS/Settings/Company/TermsAndPrivacy/"), fname);
                object filename = fname;
                spireDocName = fname;

                file.SaveAs(fname);
                DocNames = AddPageNumber(spireDocName);
                TempData["doc"] = spireDocName;
                hasFile = true;







                //BinaryReader b = new BinaryReader(privacyPolicy.InputStream);
                //byte[] binData = b.ReadBytes(privacyPolicy.ContentLength);

                //string result = System.Text.Encoding.UTF8.GetString(binData);
                //System.IO.File.WriteAllText(path + "PrivacyAndPolicy.txt", result);
                //hasFile = true;
            }
            if (consentForm != null)
            {
                string fname;
                string DocNames = "";
                string spireDocName = string.Empty;
                HttpPostedFileBase file = consentForm;
                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                {
                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                    fname = "ConsentForm.pdf";
                }
                else
                {
                    fname = "ConsentForm.pdf";
                }
                fname = Path.Combine(Server.MapPath("~/PCMS/Settings/Company/ConsentForm/"), fname);
                object filename = fname;
                spireDocName = fname;
                if (!Directory.Exists(Server.MapPath("~/PCMS/Settings/Company/ConsentForm/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/PCMS/Settings/Company/ConsentForm/"));
                }
                file.SaveAs(fname);
                DocNames = AddPageNumber(spireDocName);
                TempData["doc"] = spireDocName;
                hasFile = true;
            }

            if (hasFile)
            {
                TempData["Success"] = "Files uploaded successfully.";
            }
            else
            {
                TempData["Failure"] = "No files to upload.";
            }
            string sess = Session["UserType"].ToString();
            return RedirectToAction("TermsAndPolicy");
        }
        [HttpPost]
        public ActionResult Upload(string fileContent)
        {
            try
            {
                var path = Path.Combine(Server.MapPath("~/PCMS/Settings/Company/CompanyPolicy.txt"));
                TextWriter tw = new StreamWriter(path);
                tw.WriteLine(fileContent);
                tw.Close();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-Upload");

            }
            return RedirectToAction("UploadPolicy");
        }

        //here signature image,date and caretakername is added to pdf before sending mail
        protected string AddPageNumberAndImage(string pdfDocName, string signature, string careTakername, string initial)
        {
            try
            {
                iTextSharp.text.Image image;
                byte[] bytes = System.IO.File.ReadAllBytes(pdfDocName);
                iTextSharp.text.Font blackFont = iTextSharp.text.FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
                using (MemoryStream stream = new MemoryStream())
                {
                    PdfReader reader = new PdfReader(bytes);
                    PdfStamper stamper = new PdfStamper(reader, stream);
                    int pages = reader.NumberOfPages;
                    for (int i = 1; i <= pages; i++)
                    {
                        DateTime dateTime = DateTime.UtcNow.Date;
                        string currentDate = dateTime.ToString("dd/MM/yyyy");
                        if (i <= pages - 1)
                        {
                            ColumnText.ShowTextAligned(stamper.GetUnderContent(i), iTextSharp.text.Element.ALIGN_RIGHT, new iTextSharp.text.Phrase(initial, blackFont), 568f, 30f, 0);
                        }
                        PdfContentByte pbunder = stamper.GetUnderContent(i);
                        string base64Image = Regex.Replace(signature, "^data:image\\//[a-zA-Z]+;base64,", string.Empty);
                        var dataBytes = Convert.FromBase64String(signature);
                        image = iTextSharp.text.Image.GetInstance(dataBytes);
                        image.ScaleToFit(100, 100);
                        image.SetAbsolutePosition(2, 2);
                        if (i == pages)
                        {
                            pbunder.AddImage(image);
                            ColumnText.ShowTextAligned(stamper.GetUnderContent(i), iTextSharp.text.Element.ALIGN_RIGHT, new iTextSharp.text.Phrase(careTakername, blackFont), 568f, 30f, 0);
                            ColumnText.ShowTextAligned(stamper.GetUnderContent(i), iTextSharp.text.Element.ALIGN_RIGHT, new iTextSharp.text.Phrase(currentDate, blackFont), 568f, 15f, 0);
                        }

                    }
                    stamper.Close();
                    bytes = stream.ToArray();
                }
                string randomName = DateTime.Now.Ticks.ToString();
                string filenname = Server.MapPath("~/Temp/") + careTakername + randomName + ".pdf";
                System.IO.File.WriteAllBytes(filenname, bytes);
                return filenname;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-AddPageNumberAndImage");
            }
            return string.Empty;

        }


        public ActionResult CreatePDF(string policyContent, string signatureString, string toMail, string initial, string caretakerName = "Policy")
        {


            string pdfToAddImage = Convert.ToString(TempData["doc"]);
            string pdfToSend = AddPageNumberAndImage(pdfToAddImage, signatureString, caretakerName, initial);

            string pdfpath = Server.MapPath("~/PCMS/Settings/Interview/PDFs");
            if (!Directory.Exists(Server.MapPath("~PCMS/Settings/Interview/PDFs")))
            {
                Directory.CreateDirectory(Server.MapPath("~PCMS/Settings/Interview/PDFs/"));
            }
            string imagepath = Server.MapPath("~PCMS/Settings/Interview/Images");
            if (!Directory.Exists(Server.MapPath("~PCMS/Settings/Interview/PDFs")))
            {
                Directory.CreateDirectory(Server.MapPath("~PCMS/Settings/Interview/Images"));
            }
            iTextSharp.text.Document doc = new iTextSharp.text.Document();
            MemoryStream memoryStream = new MemoryStream();
            //Log.Logger = new LoggerConfiguration()
            //.WriteTo.File("log.txt")
            //.CreateLogger();
            try
            {
                //string fromEmail = ConfigurationManager.AppSettings["FromEmail"].ToString();
                //string fromEmailPassword = ConfigurationManager.AppSettings["FromEmailPassword"].ToString();
                //string mailHost = ConfigurationManager.AppSettings["MailHost"].ToString();
                //string mailPort = ConfigurationManager.AppSettings["MailPort"].ToString();
                //System.IO.File.WriteAllText(Server.MapPath("~/WriteLines.txt"), "First");
                //using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage(fromEmail, toMail))
                //{
                //    PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);
                //    //PdfWriter.GetInstance(doc, new FileStream(Server.MapPath("~/" + caretakerName + ".pdf"), FileMode.OpenOrCreate));
                //    try
                //    {
                //        //PdfWriter.GetInstance(doc, new FileStream("D:/" + caretakerName + ".pdf", FileMode.OpenOrCreate));
                //        doc.Open();
                //        iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(Server.MapPath("~/images/emailLogo.jpg"));
                //        doc.Add(gif);
                //        iTextSharp.text.Paragraph para = new iTextSharp.text.Paragraph(policyContent);
                //        para.Alignment = Element.ALIGN_JUSTIFIED;
                //        doc.Add(para);
                //        //var arr = Convert.ToByte(sModel);
                //        string base64Image = Regex.Replace(signatureString, "^data:image\\//[a-zA-Z]+;base64,", string.Empty);
                //        var dataBytes = Convert.FromBase64String(signatureString);
                //        doc.Add(new iTextSharp.text.Paragraph(""));
                //        doc.Add(new iTextSharp.text.Paragraph(caretakerName));
                //        gif = iTextSharp.text.Image.GetInstance(dataBytes);
                //        gif.ScaleAbsoluteWidth(300);
                //        gif.ScaleAbsoluteHeight(75);
                //        doc.Add(gif);
                //        doc.Add(new iTextSharp.text.Paragraph("Date: " + DateTime.Now.ToShortDateString()));
                //        writer.CloseStream = false;
                //        doc.Close();
                //        memoryStream.Position = 0;
                //    }
                //    catch (Exception ex)
                //    {
                //        Logger.Error(ex, "Error occurred in Admin Controller-CreatePDF");
                //        System.IO.File.WriteAllText(Server.MapPath("~/PDFError.txt"), ex.Message);
                //        //Log.Error(ex.InnerException.ToString()); ;
                //    }
                //    mail.Subject = "Agreed Policy Statement";
                //    mail.Body = "Hi " + caretakerName + ",<BR />" +
                //        "Thank you for choosing to work with.<BR />We are attaching the copy of the company policy that you have signed with us for your Reference." +
                //        "<BR /><BR /> We will shortly contact you with further details.<BR />" +
                //        "<BR /> Thanks &amp; Regards,<BR />" +
                //        "<BR />Team Tranquilcare";
                //    mail.IsBodyHtml = true;
                //    //mail.Attachments.Add(new Attachment(memoryStream, "CompanyPolicy.pdf"));
                //    Attachment data = new Attachment(pdfToSend, MediaTypeNames.Application.Octet);
                //    mail.Attachments.Add(data);
                //    SmtpClient smtp = new SmtpClient()
                //    {
                //        Host = "smtp.gmail.com",
                //        EnableSsl = true,
                //        Port = 587,
                //        UseDefaultCredentials = true,
                //        Credentials = new NetworkCredential(fromEmail, fromEmailPassword)
                //    };

                //    smtp.Port = 587;
                //    //memoryStream.Close();
                //    smtp.Send(mail);
                //    ViewBag.Message = "Sent";
                //}

                EmailInput emailinputs = new EmailInput
                {
                    Body = "Hi " + caretakerName + ",<BR />" +
                            "Thank you for choosing to work with Us.<BR />We are attaching the copy of the company policy that you have signed with us for your Reference." +
                            "<BR /><BR /> We will contact you shortly with further details.<BR />" +
                            "<BR /> Thanks &amp; Regards,<BR />" +
                            "<BR />Team Tranquil Care",
                    Subject = "Agreed Policy Statement",
                    EmailId = toMail,
                    Attachments = pdfToSend,
                    EmailType = EmailType.Registration,
                    UserId = 1//need default or corresponding userId
                };
                HttpStatusCode result = HttpStatusCode.OK;
                string api = "Admin/SendEmailNotification";
                var serviceContent = JsonConvert.SerializeObject(emailinputs);
                result = service.PostAPI(serviceContent, api);
            }

            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-CreatePDF");
                System.IO.File.WriteAllText(Server.MapPath("~/PCMS/Settings/Interview/EmailError.txt"), ex.Message);
            }
            finally
            {
                memoryStream.Close();
            }

            TempData["Doc"] = "";
            return RedirectToAction("Interview");
        }

        // GET: Admin
        public ActionResult CreateCategory(CategoryViewModel Category)
        {
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        TempData["Failure"] = Constants.NoViewPrivilege;
                        return RedirectToAction("Category");
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowEdit)
                        {
                            TempData["Failure"] = Constants.NoActionPrivilege;
                            return RedirectToAction("Category");
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    Logger.Error("Redirect to Login from CreateCategory-Admin Controller");
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                Category.Name = Category.Name.Trim();
                api = "Admin/SaveCategory";
                var categoryContent = JsonConvert.SerializeObject(Category);
                HttpStatusCode result = service.PostAPI(categoryContent, api);
                if (result == HttpStatusCode.OK)
                {
                    TempData["Success"] = "Category details updated Successfully.";
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    TempData["Failure"] = "You are not authorized to perform this action.";
                }
                else if (result == HttpStatusCode.Conflict)
                {
                    TempData["Failure"] = "Data already exist. Please enter different data.";
                }
                else
                {
                    TempData["Failure"] = "Updation Failed. Please try again later.";
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-CreateCategory");
            }
            return RedirectToAction("Category");
        }

        public ActionResult CategoryList()
        {
            List<CategoryViewModel> categoryList = new List<CategoryViewModel>();
            CategoryViewModel categoryModel = new CategoryViewModel();
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                api = "Admin/GetCategory?flag=*&value=''";
                var result = service.GetAPI(api);
                categoryList = JsonConvert.DeserializeObject<List<CategoryViewModel>>(result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-CategoryList");
            }
            return PartialView("_CategoryPartialView", categoryList);
        }

        public ActionResult Category()
        {
            Dictionary<string, string> colors = new Dictionary<string, string>();

            try
            {
                CategoryViewModel categoryModel = new CategoryViewModel();
                System.Drawing.Color MyColor;

                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    Logger.Error("Redirect to Login from Category-Admin Controller");
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                foreach (string ColorName in Enum.GetNames(typeof(System.Drawing.KnownColor)))
                {
                    MyColor = System.Drawing.Color.FromName(ColorName);
                    if (MyColor.IsSystemColor == true)
                        continue;
                    colors.Add(ColorName, ColorName);
                }
                categoryModel.ColorList = colors;
                return View(categoryModel);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-Category");
                return null;
            }
        }

        public JsonResult GetColors()
        {
            List<string> colorList = new List<string>();
            try
            {
                System.Drawing.Color MyColor;
                foreach (string ColorName in Enum.GetNames(typeof(System.Drawing.KnownColor)))
                {
                    MyColor = System.Drawing.Color.FromName(ColorName);
                    if (MyColor.IsSystemColor == true)
                        continue;
                    colorList.Add(ColorName);
                }
                return Json(colorList);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-SubCategory");
                return null;
            }
        }
        
        public ActionResult ManageCaretaker()
        {
            List<CareTakerRegistrationViewModel> caretakers = new List<CareTakerRegistrationViewModel>();
            ClientModel clientModelObj = new ClientModel();
            try
            {
                string api = string.Empty;
                string country = Session["CountryId"].ToString();
                string state = Session["StateID"].ToString();
                string CityIdk = Session["CityIdk"].ToString();
                if (Session["UserType"] != null)
                {
                    if(country!=""&&state!=""&&CityIdk!="")
                    {
                        List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                        api = "Admin/GetCountryDetails/0";
                        var countrylist1 = service.GetAPI(api);
                        countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                        ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                        List<StateViewModel> stateList1 = new List<StateViewModel>();
                        api = "/Admin/GetStatesByCountryId/" + country;
                        var stateResult1 = service.GetAPI(api);
                        stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                        ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", clientModelObj.StateId1);

                        api = "Admin/GetCity?flag=StateId&value=" + state;
                        List<Cities> cityList1 = new List<Cities>();
                        var resultCity1 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                        ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", clientModelObj.CityId1);

                        api = "Admin/GetBranch?flag=StateId&value=" + CityIdk;
                        //api = "Admin/GetAllBranch/";
                        List<Cities> branchList1 = new List<Cities>();
                        var resultBranch1 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultBranch1);
                        ViewData["ListBranch"] = new SelectList(cityList1, "branchId", "branchName", clientModelObj.BranchId2);
                    }
                    else
                    {
                        List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                        api = "Admin/GetCountryDetails/0";
                        var countrylist1 = service.GetAPI(api);
                        countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                        ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                        List<StateViewModel> stateList1 = new List<StateViewModel>();
                        api = "/Admin/GetStatesByCountryId/1";
                        var stateResult1 = service.GetAPI(api);
                        stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                        ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", clientModelObj.StateId1);

                        api = "Admin/GetCity?flag=StateId&value=" + clientModelObj.StateId1;
                        List<Cities> cityList1 = new List<Cities>();
                        var resultCity1 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                        ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", clientModelObj.CityId1);

                        api = "Admin/GetAllBranch/";
                        List<Cities> branchList1 = new List<Cities>();
                        var resultBranch1 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultBranch1);
                        ViewData["ListBranch"] = new SelectList(cityList1, "branchId", "branchName", clientModelObj.BranchId2);
                    }


                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 13;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    Logger.Error("Redirect to Login from ManageCaretaker-Admin Controller");
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                var inputs = new LocationSearchInputs();
                HomeController home = new HomeController();
                var loggedInUserId = (int)Session["loggedInUserId"];
                //get Work Role
                var userRoleDetails = home.GetUserRoleDetails(loggedInUserId);
                if (userRoleDetails != null)
                {

                    if (userRoleDetails.WorkRoleId == 1)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = null;
                        inputs.CityId = null;
                        ViewData["UserWorkRole"] = 1;
                    }
                    if (userRoleDetails.WorkRoleId == 2)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = userRoleDetails.StateId;
                        inputs.CityId = null;
                        ViewData["UserWorkRole"] = 2;
                    }
                    if (userRoleDetails.WorkRoleId == 3)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = userRoleDetails.StateId;
                        inputs.CityId = userRoleDetails.CityId;
                        ViewData["UserWorkRole"] = 3;
                    }
                    if (userRoleDetails.WorkRoleId == 4)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = userRoleDetails.StateId;
                        inputs.CityId = userRoleDetails.CityId;
                        ViewData["UserWorkRole"] = 4;
                    }
                }
                else
                {
                    inputs.CountryId = null;
                    inputs.StateId = null;
                    inputs.CityId = null;
                    ViewData["UserWorkRole"] = null;
                }
                var clientSearchInputsContent = JsonConvert.SerializeObject(inputs);
                string manageCaretaker = "Admin/SelectCaretakersByLocation/" + (int)CaretakerStatus.Approved;
                var countryResult = service.PostAPIWithData(clientSearchInputsContent,manageCaretaker);
                caretakers = JsonConvert.DeserializeObject<List<CareTakerRegistrationViewModel>>(countryResult.Result);

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-ManageCaretaker");
            }
            return View(caretakers);//CaretakerList
        }

        public ActionResult ManageCaretakerByLocation(LocationSearchInputs inputs)
        {
            List<CareTakerRegistrationViewModel> caretakers = new List<CareTakerRegistrationViewModel>();
            ClientModel clientModelObj = new ClientModel();
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                    api = "Admin/GetCountryDetails/0";
                    var countrylist1 = service.GetAPI(api);
                    countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                    ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                    List<StateViewModel> stateList1 = new List<StateViewModel>();
                    api = "/Admin/GetStatesByCountryId/1";
                    var stateResult1 = service.GetAPI(api);
                    stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                    ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", clientModelObj.StateId1);

                    api = "Admin/GetCity?flag=StateId&value=" + clientModelObj.StateId1;
                    List<Cities> cityList1 = new List<Cities>();
                    var resultCity1 = service.GetAPI(api);
                    cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                    ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", clientModelObj.CityId1);

                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 13;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    Logger.Error("Redirect to Login from ManageCaretaker-Admin Controller");
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                var SearchInputsContent = JsonConvert.SerializeObject(inputs);
                string manageCaretaker = "Admin/SelectCaretakersByLocation/" + (int)CaretakerStatus.Approved;
                var countryResult = service.PostAPIWithData(SearchInputsContent,manageCaretaker);
                caretakers = JsonConvert.DeserializeObject<List<CareTakerRegistrationViewModel>>(countryResult.Result);

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-ManageCaretaker");
            }
            return PartialView("_ManageCaretakerPartial", caretakers);//CaretakerList
        }
        public ActionResult AppliedCaretakers()
        {
            List<CareTakerRegistrationViewModel> Caretakers = new List<CareTakerRegistrationViewModel>();
            ClientModel clientModelObj = new ClientModel();
            try
            {
                string api = string.Empty;
                string country = Session["CountryId"].ToString();
                string state = Session["StateID"].ToString();
                string CityIdk = Session["CityIdk"].ToString();
                if (Session["UserType"] != null)
                {
                    if(country!=""&&state!=""&&CityIdk!="")
                    {
                        List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                        api = "Admin/GetCountryDetails/0";
                        var countrylist1 = service.GetAPI(api);
                        countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                        ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                        List<StateViewModel> stateList1 = new List<StateViewModel>();
                        api = "/Admin/GetStatesByCountryId/" + country;
                        var stateResult1 = service.GetAPI(api);
                        stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                        ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", state);

                        api = "Admin/GetCity?flag=StateId&value=" + state;
                        List<Cities> cityList1 = new List<Cities>();
                        var resultCity1 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                        ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", CityIdk);
                        
                        api = "Admin/GetBranch?flag=StateId&value=" + CityIdk;
                        //api = "Admin/GetAllBranch/";
                        List<Cities> branchList1 = new List<Cities>();
                        var resultBranch1 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultBranch1);
                        ViewData["ListBranch"] = new SelectList(cityList1, "branchId", "branchName", clientModelObj.BranchId2);
                    }
                    else
                    {
                        List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                        api = "Admin/GetCountryDetails/0";
                        var countrylist1 = service.GetAPI(api);
                        countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                        ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                        List<StateViewModel> stateList1 = new List<StateViewModel>();
                        api = "/Admin/GetStatesByCountryId/1";
                        var stateResult1 = service.GetAPI(api);
                        stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                        ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", clientModelObj.StateId1);

                        api = "Admin/GetCity?flag=StateId&value=" + clientModelObj.StateId1;
                        List<Cities> cityList1 = new List<Cities>();
                        var resultCity1 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                        ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", clientModelObj.CityId1);

                        api = "Admin/GetAllBranch/";
                        List<Cities> branchList1 = new List<Cities>();
                        var resultBranch1 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultBranch1);
                        ViewData["ListBranch"] = new SelectList(cityList1, "branchId", "branchName", clientModelObj.BranchId2);
                    }
                

                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 15;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    Logger.Error("Redirect to Login from AppliedCaretakers-Admin Controller");
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                var inputs = new LocationSearchInputs();
                HomeController home = new HomeController();
                var loggedInUserId = (int)Session["loggedInUserId"];
                //get Work Role
                var userRoleDetails = home.GetUserRoleDetails(loggedInUserId);
                if (userRoleDetails != null)
                {

                    if (userRoleDetails.WorkRoleId == 1)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = null;
                        inputs.CityId = null;
                        ViewData["UserWorkRole"] = 1;
                    }
                    if (userRoleDetails.WorkRoleId == 2)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = userRoleDetails.StateId;
                        inputs.CityId = null;
                        ViewData["UserWorkRole"] = 2;
                    }
                    if (userRoleDetails.WorkRoleId == 3)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = userRoleDetails.StateId;
                        inputs.CityId = userRoleDetails.CityId;
                        ViewData["UserWorkRole"] = 3;
                    }
                    if (userRoleDetails.WorkRoleId == 4)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = userRoleDetails.StateId;
                        inputs.CityId = userRoleDetails.CityId;
                        ViewData["UserWorkRole"] = 4;
                    }
                }
                else
                {
                    inputs.CountryId = null;
                    inputs.StateId = null;
                    inputs.CityId = null;
                    ViewData["UserWorkRole"] = null;
                }
                api = "Admin/SelectCaretakersByLocation/" + (int)CaretakerStatus.Applied;
                var clientSearchInputsContent = JsonConvert.SerializeObject(inputs);
                var countryResult = service.PostAPIWithData(clientSearchInputsContent, api);
                Caretakers = JsonConvert.DeserializeObject<List<CareTakerRegistrationViewModel>>(countryResult.Result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-AppliedCaretakers");
            }
            return View(Caretakers);//CaretakerList
        }

        [HttpPost]
        public ActionResult AppliedCaretakersByLocation(LocationSearchInputs inputs)
        {
            if (inputs == null)
            {
                inputs = new LocationSearchInputs();
                inputs.CountryId = null;
                inputs.StateId = null;
                inputs.CityId = null;
            }
            if (inputs.CountryId == 0)
            {
                inputs.CountryId = null;
            }
            if (inputs.StateId == 0)
            {
                inputs.StateId = null;
            }
            if (inputs.CityId == 0)
            {
                inputs.CityId = null;
            }
            List<CareTakerRegistrationViewModel> Caretakers = new List<CareTakerRegistrationViewModel>();
            ClientModel clientModelObj = new ClientModel();
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                    api = "Admin/GetCountryDetails/0";
                    var countrylist1 = service.GetAPI(api);
                    countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                    ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                    List<StateViewModel> stateList1 = new List<StateViewModel>();
                    api = "/Admin/GetStatesByCountryId/1";
                    var stateResult1 = service.GetAPI(api);
                    stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                    ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", clientModelObj.StateId1);

                    api = "Admin/GetCity?flag=StateId&value=" + clientModelObj.StateId1;
                    List<Cities> cityList1 = new List<Cities>();
                    var resultCity1 = service.GetAPI(api);
                    cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                    ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", clientModelObj.CityId1);

                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 15;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    Logger.Error("Redirect to Login from AppliedCaretakers-Admin Controller");
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                
                api = "Admin/SelectCaretakersByLocation/" + (int)CaretakerStatus.Applied;
                var clientSearchInputsContent = JsonConvert.SerializeObject(inputs);
                var countryResult = service.PostAPIWithData(clientSearchInputsContent, api);
                Caretakers = JsonConvert.DeserializeObject<List<CareTakerRegistrationViewModel>>(countryResult.Result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-AppliedCaretakers");
            }
            return PartialView("_AppliedCaretakerPartial",Caretakers);//CaretakerList
        }

        public ActionResult OfficeStaffRegistration()
        {
            OfficeStaffRegistration officeStaffRegistration = new Models.OfficeStaffRegistration();

            try
            {
                string api = string.Empty;

                if(Session["CountryId"].ToString()!="" && Session["StateID"].ToString()!=""&& Session["BranchID"].ToString()!="")
                {
                    ViewData["cou"] = Session["CountryId"].ToString();
                    ViewData["stat"] = Session["StateID"].ToString();
                    ViewData["city"] = Session["CityIdk"].ToString();
                    ViewData["BranchID"] = Session["BranchID"].ToString();

                }
                else
                {
                    ViewData["cou"] = "";
                    ViewData["stat"] = "";
                    ViewData["city"] = "";
                    ViewData["BranchID"] ="";

                }
              
               

                ClientModel clientModelObj = new ClientModel();
                if (Session["UserType"] != null)
                {
                    List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                    api = "Admin/GetCountryDetails/0";
                    var countrylist1 = service.GetAPI(api);
                    countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                    ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                    List<StateViewModel> stateList1 = new List<StateViewModel>();
                    api = "/Admin/GetStatesByCountryId/1";
                    var stateResult1 = service.GetAPI(api);
                    stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                    ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", clientModelObj.StateId1);

                    api = "Admin/GetCity?flag=StateId&value=" + clientModelObj.StateId1;
                    List<Cities> cityList1 = new List<Cities>();
                    var resultCity1 = service.GetAPI(api);
                    cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                    ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", clientModelObj.CityId1);

                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 3;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);



                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    Logger.Error("Redirect to Login from OfficeStaffRegistration-Admin Controller");
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                List<CountryViewModel> listCountry = new List<CountryViewModel>();
                string apiCountry = "Admin/GetCountryDetails/0";
                var countryResult = service.GetAPI(apiCountry);
                listCountry = JsonConvert.DeserializeObject<List<CountryViewModel>>(countryResult);
                int defaultCountry = (listCountry.Where(x => x.Isdefault == true).Count() > 0) ? listCountry.Where(x => x.Isdefault == true).SingleOrDefault().CountryId : listCountry.FirstOrDefault().CountryId;
                var _listCountry = new SelectList(listCountry, "CountryId", "Name", defaultCountry);
                ViewData["CountryList"] = _listCountry;

                //List<StateViewModel> stateList = new List<StateViewModel>();
                //var stateapi = "/Admin/GetStatesByCountryId/" + defaultCountry;
                //var stateResult = service.GetAPI(stateapi);
                //stateList = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult);
                //stateList.Insert(0, new StateViewModel { StateId = 0, Name = "--Select--" });

                //var _stateList = new SelectList(stateList, "StateId", "Name", 0);
                //ViewData["StateList"] = new SelectList(stateList, "StateId", "Name", 0);
                //ViewBag.State = new SelectList(stateList, "StateId", "Name", 1);

                //string apiCity = "Admin/GetCity?flag=*&value=''";
                //var resultCity = service.GetAPI(apiCity);
                //List<Cities> cityList = JsonConvert.DeserializeObject<List<Cities>>(resultCity);
                //cityList.Add(new Cities { CityId = 0, CityName = "--Select--" });
                //ViewBag.City = new SelectList(cityList, "CityId", "CityName", 0);
                //ViewData["CityList"] = new SelectList(cityList, "CityId", "CityName", 0);


                api = "Admin/GetDesignation/0";
                List<DesignationViewModel> designationList = new List<DesignationViewModel>();
                var result = service.GetAPI(api);
                designationList = JsonConvert.DeserializeObject<List<DesignationViewModel>>(result);
                //designationList.Insert(0, new DesignationViewModel { DesignationId = 0, Designation = "--Select--" });
                var listDesignation = new SelectList(designationList, "DesignationId", "Designation", 0);
                ViewData["listDesignation"] = listDesignation;


                api = "Admin/GetQualification/0";
                List<QualificationViewModel> qualificationList = new List<QualificationViewModel>();
                var resultQual = service.GetAPI(api);
                qualificationList = JsonConvert.DeserializeObject<List<QualificationViewModel>>(resultQual);
                qualificationList.Insert(0, new QualificationViewModel { QualificationId = 0, Qualification = "--Select--" });
                var listQualification = new SelectList(qualificationList, "QualificationId", "Qualification", 0);
                ViewData["ListQualification"] = listQualification;

                string roleApi = "Admin/GetRoles/0";
                List<Roles> roles = new List<Roles>();
                var roleResult = service.GetAPI(roleApi);
                roles = JsonConvert.DeserializeObject<List<Roles>>(roleResult);
                var rolesList = new SelectList(roles, "RoleId", "RoleName", 0);
                ViewData["RolesList"] = rolesList;

                //officeStaffRegistration.CountryId = defaultCountry;
                //ViewData["CountryID"] = defaultCountry;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-OfficeStaffRegistration");
            }
            return View(officeStaffRegistration);
        }

        public ActionResult CreateServices(ServicesViewModel serviceDetails)
        {
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return RedirectToAction("Services");
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 3;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowEdit)
                        {
                            ViewBag.Error = Constants.NoActionPrivilege;
                            return RedirectToAction("Services");
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }

                }
                else
                {
                    Logger.Error("Redirect to Login from CreateServices-Admin Controller");
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                api = "Admin/SaveServiceDetails";
                if (string.IsNullOrEmpty(serviceDetails.Name))
                {
                    ModelState.AddModelError("Name", "Service Name Required");
                }

                if (ModelState.IsValid)
                {
                    serviceDetails.Name = serviceDetails.Name.Trim();
                    if (serviceDetails.ServicePicImage == null && serviceDetails.ServicePicture == null)
                    {
                        serviceDetails.ServicePicture = System.IO.File.ReadAllBytes(Server.MapPath(@"~/images/NOImage.jpg"));
                    }
                    else if (serviceDetails.ServicePicImage != null)
                    {
                        using (var binaryReader = new BinaryReader(serviceDetails.ServicePicImage.InputStream))
                        {
                            serviceDetails.ServicePicture = binaryReader.ReadBytes(serviceDetails.ServicePicImage.ContentLength);
                        }
                    }
                    serviceDetails.ServicePicImage = null;
                    var serviceContent = JsonConvert.SerializeObject(serviceDetails);
                    HttpStatusCode result = service.PostAPI(serviceContent, api);
                    if (result == HttpStatusCode.OK)
                    {
                        TempData["Success"] = " Service details updated Successfully.";
                    }
                    else if (result == HttpStatusCode.Unauthorized)
                    {
                        TempData["Failure"] = "You are not authorized to perform this action.";
                    }
                    else if (result == HttpStatusCode.Conflict)
                    {
                        TempData["Failure"] = "Data already exist. Please enter different data.";
                    }
                    else
                    {
                        TempData["Failure"] = "Updation Failed. Please try again later.";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-CreateServices");

            }

            return RedirectToAction("Services");
        }

        /// <summary>
        /// To post office staff details
        /// </summary>
        /// <param name="objOfficeStaffRegistration"></param>
        /// <returns></returns>
        public ActionResult CreateOfficeStaffRegistration(OfficeStaffRegistration objOfficeStaffRegistration)
        {
            try
            {
                string stafftyp = objOfficeStaffRegistration.Stafftype.ToString();
                string api = string.Empty;
                var fname = objOfficeStaffRegistration.FirstName;
                var lname = objOfficeStaffRegistration.LastName;
                var userid = "5";
                int num = new Random().Next(1000, 9999);
                string numm = Convert.ToString(num);
                var paswd = objOfficeStaffRegistration.Password;
                objOfficeStaffRegistration.EmployeeNo = numm;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 3;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return RedirectToAction("OfficeStaffRegistration");
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    Logger.Error("Redirect to Login from CreateOfficeStaffRegistration-Admin Controller");
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                objOfficeStaffRegistration.DesignationId = objOfficeStaffRegistration.DesignationId == 0 ? null : objOfficeStaffRegistration.DesignationId;
                objOfficeStaffRegistration.QualificationId = objOfficeStaffRegistration.QualificationId == 0 ? null : objOfficeStaffRegistration.QualificationId;
                api = "Admin/SaveOfficeStaffDetails";
                objOfficeStaffRegistration.UserTypeId = 5;
                #region Created by silpa on 03-01-2022 On LDAP

                if (stafftyp == "CountryStaff")
                {
                    System.DirectoryServices.Protocols.LdapConnection ldapConnectionc = new System.DirectoryServices.Protocols.LdapConnection("127.0.0.1:10389");

                    var networkCredentialc = new NetworkCredential(
                          "employeeNumber=1,ou=Aluva,ou=Ernakulam,ou=kerala,ou=India,o=TopCompany",
                          "secret");

                    ldapConnectionc.SessionOptions.ProtocolVersion = 3;

                    ldapConnectionc.AuthType = AuthType.Basic;
                    ldapConnectionc.Bind(networkCredentialc);


                    var requestc = new AddRequest("employeeNumber=" + numm + ",ou=" + objOfficeStaffRegistration.CountryId1Name + ",o=TopCompany", new DirectoryAttribute[] {
                    new DirectoryAttribute("employeeNumber", numm),
                    new DirectoryAttribute("cn", objOfficeStaffRegistration.EmailAddress),
                    new DirectoryAttribute("uid", userid),
                    new DirectoryAttribute("sn",lname),
                    new DirectoryAttribute("userPassword", paswd),
                    new DirectoryAttribute("objectClass", new string[] { "top", "person", "organizationalPerson","inetOrgPerson" })
                   });
                    ldapConnectionc.SendRequest(requestc);

                }
                else if (stafftyp == "StateStaff")
                {
                    System.DirectoryServices.Protocols.LdapConnection ldapConnectioncs = new System.DirectoryServices.Protocols.LdapConnection("127.0.0.1:10389");

                    var networkCredentialcs = new NetworkCredential(
                          "employeeNumber=1,ou=Aluva,ou=Ernakulam,ou=kerala,ou=India,o=TopCompany",
                          "secret");

                    ldapConnectioncs.SessionOptions.ProtocolVersion = 3;

                    ldapConnectioncs.AuthType = AuthType.Basic;
                    ldapConnectioncs.Bind(networkCredentialcs);

                    var requestcs = new AddRequest("employeeNumber=" + numm + ",ou=" + objOfficeStaffRegistration.StateId1Name + ",ou=" + objOfficeStaffRegistration.CountryId1Name + ",o=TopCompany", new DirectoryAttribute[] {
                    new DirectoryAttribute("employeeNumber", numm),
                    new DirectoryAttribute("cn", objOfficeStaffRegistration.EmailAddress),
                    new DirectoryAttribute("uid", userid),
                    new DirectoryAttribute("sn",lname),
                    new DirectoryAttribute("userPassword", paswd),
                    new DirectoryAttribute("objectClass", new string[] { "top", "person", "organizationalPerson","inetOrgPerson" })
                   });
                    ldapConnectioncs.SendRequest(requestcs);
                }
                else if (stafftyp == "CityStaff")
                {
                    System.DirectoryServices.Protocols.LdapConnection ldapConnectioncsc = new System.DirectoryServices.Protocols.LdapConnection("127.0.0.1:10389");

                    var networkCredentialcsc = new NetworkCredential(
                          "employeeNumber=1,ou=Aluva,ou=Ernakulam,ou=kerala,ou=India,o=TopCompany",
                          "secret");

                    ldapConnectioncsc.SessionOptions.ProtocolVersion = 3;

                    ldapConnectioncsc.AuthType = AuthType.Basic;
                    ldapConnectioncsc.Bind(networkCredentialcsc);
                    var requestcsc = new AddRequest("employeeNumber=" + numm + ",ou=" + objOfficeStaffRegistration.CityId1Name + ",ou=" + objOfficeStaffRegistration.StateId1Name + ",ou=" + objOfficeStaffRegistration.CountryId1Name + ",o=TopCompany", new DirectoryAttribute[] {
                    //var requestcsc = new AddRequest("employeeNumber=" + numm + ",ou=" + objOfficeStaffRegistration.StateId1Name + ",ou=" + objOfficeStaffRegistration.CountryId1Name + ",o=TopCompany", new DirectoryAttribute[] {
                    new DirectoryAttribute("employeeNumber", numm),
                    new DirectoryAttribute("cn", objOfficeStaffRegistration.EmailAddress),
                    new DirectoryAttribute("uid", userid),
                    new DirectoryAttribute("sn",lname),
                    new DirectoryAttribute("userPassword", paswd),
                    new DirectoryAttribute("objectClass", new string[] { "top", "person", "organizationalPerson","inetOrgPerson" })
                   });
                    ldapConnectioncsc.SendRequest(requestcsc);
                }
                else
                {
                    System.DirectoryServices.Protocols.LdapConnection ldapConnection = new System.DirectoryServices.Protocols.LdapConnection("127.0.0.1:10389");

                    var networkCredential = new NetworkCredential(
                          "employeeNumber=1,ou=Aluva,ou=Ernakulam,ou=kerala,ou=India,o=TopCompany",
                          "secret");

                    ldapConnection.SessionOptions.ProtocolVersion = 3;

                    ldapConnection.AuthType = AuthType.Basic;
                    ldapConnection.Bind(networkCredential);


                    var request = new AddRequest("employeeNumber=" + numm + ",ou="+objOfficeStaffRegistration.BranchId1Name+",ou=" + objOfficeStaffRegistration.CityId1Name + ",ou=" + objOfficeStaffRegistration.StateId1Name + ",ou=" + objOfficeStaffRegistration.CountryId1Name + ",o=TopCompany", new DirectoryAttribute[] {
                    new DirectoryAttribute("employeeNumber", numm),
                    new DirectoryAttribute("cn", objOfficeStaffRegistration.EmailAddress),
                      new DirectoryAttribute("uid", userid),
                     new DirectoryAttribute("sn",lname),
                    new DirectoryAttribute("userPassword", paswd),
                    new DirectoryAttribute("objectClass", new string[] { "top", "person", "organizationalPerson","inetOrgPerson" })
                });
                    ldapConnection.SendRequest(request);
                }

                #endregion


                #region Created by silpa on 03-01-2022 On group


                int nums = new Random().Next(1000, 9999);
                string numms = Convert.ToString(nums);
                var roleID = objOfficeStaffRegistration.RoleName;
               
                    System.DirectoryServices.Protocols.LdapConnection ldapConnection1 = new System.DirectoryServices.Protocols.LdapConnection("127.0.0.1:10389");

                    var networkCredential1 = new NetworkCredential(
                          "employeeNumber=1,ou=Aluva,ou=Ernakulam,ou=kerala,ou=India,o=TopCompany",
                          "secret");
                    var unique = "";
                    if (stafftyp == "CountryStaff")
                    {
                        unique = "employeeNumber=" + numm + ",ou=" + objOfficeStaffRegistration.CountryId1Name + ",o=TopCompany";
                    }
                    else if (stafftyp == "StateStaff")
                    {
                        unique = "employeeNumber=" + numm + ",ou=" + objOfficeStaffRegistration.StateId1Name + ",ou=" + objOfficeStaffRegistration.CountryId1Name + ",o=TopCompany";
                    }
                    else if (stafftyp == "CityStaff")
                    {
                        unique = "employeeNumber=" + numm + ",ou=" + objOfficeStaffRegistration.CityId1Name + ",ou=" + objOfficeStaffRegistration.StateId1Name + ",ou=" + objOfficeStaffRegistration.CountryId1Name + ",o=TopCompany";
                    }
                    else
                    {
                        unique = "employeeNumber=" + numm + ",ou=" + objOfficeStaffRegistration.BranchId1Name + ",ou=" + objOfficeStaffRegistration.CityId1Name + ",ou=" + objOfficeStaffRegistration.StateId1Name + ",ou=" + objOfficeStaffRegistration.CountryId1Name + ",o=TopCompany";
                    }

                    ldapConnection1.SessionOptions.ProtocolVersion = 3;

                    ldapConnection1.AuthType = AuthType.Basic;
                    ldapConnection1.Bind(networkCredential1);

                    var request1 = new AddRequest("cn=" + fname + ",ou="+roleID+",o=TopCompany", new DirectoryAttribute[] {

                    new DirectoryAttribute("cn", fname),


                     new DirectoryAttribute("uniqueMember", unique),
                    new DirectoryAttribute("objectClass", new string[] { "top", "groupOfUniqueNames",})
                });
                    ldapConnection1.SendRequest(request1);
               


                #endregion


                objOfficeStaffRegistration.UserStatus = UserStatus.Active;
                string encryptionPassword = ConfigurationManager.AppSettings["EncryptPassword"].ToString();
                string passwordEpt = StringCipher.Encrypt(objOfficeStaffRegistration.Password, encryptionPassword);
                objOfficeStaffRegistration.Password = passwordEpt;
                objOfficeStaffRegistration.UserVerified = true;
                objOfficeStaffRegistration.SiteURL = Request.Url.Scheme + "://" + Request.Url.Authority + "/";
                var officeStaffRegistrationContent = JsonConvert.SerializeObject(objOfficeStaffRegistration);
                var result = service.PostAPIWithData(officeStaffRegistrationContent, api);
                if (result.Result != "0")
                {
                    TempData["OfficeStaff"] = "Registered Successfully, Email verification link has been sent to your registered email: " + objOfficeStaffRegistration.EmailAddress;
                    api = "Users/SendVerificationEmail";//+ publicUserDetails.EmailAddress
                    VerifyEmail verifyEmail = new VerifyEmail
                    {
                        WelcomeMsg = "Welcome to Tranquil Care!",
                        FirstName = objOfficeStaffRegistration.FirstName,
                        MailMsg = "Thank you for registering with us, you have successfully created an account with us.",
                        Mailcontent = string.Format("{0}://{1}/Home/EmailVerificationSuccess/{2}", Request.Url.Scheme, Request.Url.Authority, HttpUtility.UrlEncode(StringCipher.EncodeNumber(Convert.ToInt32(result.Result)))),
                        ContactNo = "1-800-892-6066",
                        RegardsBy = "Tranquil Care Inc.",
                        siteUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/",
                        CompanyName_TagLine = "Tranquil Care - Delivering Care Excellence.",
                        CompanyName = "Tranquil Care Inc.",
                        Subject = "Email Verification Link",
                        Email = objOfficeStaffRegistration.EmailAddress
                    };
                    var serviceContent = JsonConvert.SerializeObject(verifyEmail);
                    var otpResult = service.PostAPIWithData(serviceContent, api);
                    NotificationHub.Static_Send("notify", "New Office staff registered", "static");
                }
                else
                {
                    TempData["OfficeStaffFailure"] = "Failed saving officestaff details!";
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-CreateOfficeStaffRegistration");

            }
            return RedirectToAction("OfficeStaffRegistration");

        }

        /// <summary>
        /// To view office staff profile
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewOfficeStaffProfile(int id)
        {
            List<CountryViewModel> listCountry = new List<CountryViewModel>();
            List<StateViewModel> listState = new List<StateViewModel>();
            OfficeStaffRegistration staffRegistration = null;
            try
            {
                string api = string.Empty;
                ClientModel clientModelObj = new ClientModel();
                if (Session["UserType"] != null)
                {
                    List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                    api = "Admin/GetCountryDetails/0";
                    var countrylist1 = service.GetAPI(api);
                    countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                    ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                    List<StateViewModel> stateList1 = new List<StateViewModel>();
                    api = "/Admin/GetStatesByCountryId/1";
                    var stateResult1 = service.GetAPI(api);
                    stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                    ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", clientModelObj.StateId1);

                    api = "Admin/GetCity?flag=StateId&value=" + clientModelObj.StateId1;
                    List<Cities> cityList1 = new List<Cities>();
                    var resultCity1 = service.GetAPI(api);
                    cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                    ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", clientModelObj.CityId1);

                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF" && Convert.ToInt32(Session["loggedInUserId"]) != id)
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 4;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR" || Convert.ToInt32(Session["loggedInUserId"]) == id)
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    Logger.Error("Redirect to Login from ViewOfficeStaffProfile-Admin Controller");
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }

                string countryApi = "Admin/GetCountryDetails/0";
                var countryResult = service.GetAPI(countryApi);
                listCountry = JsonConvert.DeserializeObject<List<CountryViewModel>>(countryResult);
                ViewBag.Country = new SelectList(listCountry, "CountryId", "Name", 0);
                string stateApi = "Admin/GetStateDetails/0";
                var stateResult = service.GetAPI(stateApi);
                listState = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult);
                ViewBag.StateName = new SelectList(listState, "StateId", "Name", 0);
                List<Cities> cityList = new List<Cities>();
                string apiCity = "Admin/GetCity?flag=*&value=''";
                var resultCity = service.GetAPI(apiCity);
                cityList = JsonConvert.DeserializeObject<List<Cities>>(resultCity);
                ViewBag.City = new SelectList(cityList, "CityId", "CityName", 0);
                string roleApi = "Admin/GetRoles/0";
                List<Roles> roles = new List<Roles>();
                var roleResult = service.GetAPI(roleApi);
                roles = JsonConvert.DeserializeObject<List<Roles>>(roleResult);
                ViewBag.Role = new SelectList(roles, "RoleId", "RoleName", 0);
                api = "Admin/GetDesignation/0";
                List<DesignationViewModel> designationList = new List<DesignationViewModel>();
                var result = service.GetAPI(api);
                designationList = JsonConvert.DeserializeObject<List<DesignationViewModel>>(result);
                designationList.Insert(0, new DesignationViewModel { DesignationId = 0, Designation = "--Select--" });
                var listDesignation = new SelectList(designationList, "DesignationId", "Designation", 0);
                ViewData["listDesignation"] = listDesignation;


                api = "Admin/GetQualification/0";
                List<QualificationViewModel> qualificationList = new List<QualificationViewModel>();
                var resultQual = service.GetAPI(api);
                qualificationList = JsonConvert.DeserializeObject<List<QualificationViewModel>>(resultQual);
                qualificationList.Insert(0, new QualificationViewModel { QualificationId = 0, Qualification = "--Select--" });
                var listQualification = new SelectList(qualificationList, "QualificationId", "Qualification", 0);
                ViewData["ListQualification"] = listQualification;

                ViewBag.Message = "Admin";
                api = "Admin/GetOfficeStaffProfile/" + id;
                result = service.GetAPI(api);
                staffRegistration = Newtonsoft.Json.JsonConvert.DeserializeObject<OfficeStaffRegistration>(result);
                staffRegistration.UserId = id;

                //Get country1,state1,city1 name
                List<CountryViewModel> listCountry2 = new List<CountryViewModel>();
                List<StateViewModel> listState2 = new List<StateViewModel>();
                List<CityViewModel> cityList2 = new List<CityViewModel>();
                List<Cities> BranchList2 = new List<Cities>();
                string countryApi2 = "Admin/GetCountryDetails/" + staffRegistration.CountryId1;
                var countryResult2 = service.GetAPI(countryApi2);
                listCountry2 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countryResult2);
                staffRegistration.CountryId1Name = listCountry2[0].Name;
                string stateApi2 = "Admin/GetStateDetails/" + staffRegistration.StateId1;
                var stateResult2 = service.GetAPI(stateApi2);
                listState2 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult2);
                staffRegistration.StateId1Name = listState2[0].Name;
                string cityApi2 = "Admin/GetCity?flag=CityId&value=" + staffRegistration.CityId1;
                var cityResult2 = service.GetAPI(cityApi2);
                cityList2 = JsonConvert.DeserializeObject<List<CityViewModel>>(cityResult2);
                staffRegistration.CityId1Name = cityList2[0].CityName;
                string BranchApi2 = "Admin/GetBranchByIds?BranchId=" + staffRegistration.BranchId1;
                var BranchResult2 = service.GetAPI(BranchApi2);
                BranchList2 = JsonConvert.DeserializeObject<List<Cities>>(BranchResult2);
                staffRegistration.BranchId1Name = BranchList2[0].BranchName;

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-ViewOfficeStaffProfile");

            }
            return View(staffRegistration);
        }

        /// <summary>
        /// To edit office staff profile
        /// </summary>
        /// <param name="objOfficeStaffRegistration"></param>
        /// <returns></returns>
        public ActionResult EditOfficeStaffProfile(OfficeStaffRegistration objOfficeStaffRegistration)
        {
            try
            {

                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        TempData["Error"] = Constants.NoViewPrivilege;
                        return RedirectToAction("Error");
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 4;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            TempData["Error"] = Constants.NoViewPrivilege;
                            return RedirectToAction("Error");
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                api = "Admin/SaveOfficeStaffDetails";
                var officeStaffRegistrationContent = JsonConvert.SerializeObject(objOfficeStaffRegistration);
                var result = service.PostAPIWithData(officeStaffRegistrationContent, api);

                if (result.Result != "0")
                {
                    TempData["Userupdate"] = "Office Staff details updated Successfully";
                }
                //else if (result == HttpStatusCode.Unauthorized)
                //{
                //    TempData["Userupdate"] = "You are not authorized to perform this action.";
                //}
                else
                {
                    TempData["OfficeStaffFailure"] = "Updation Failed";
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-EditOfficeStaffProfile");

            }
            return RedirectToAction("ViewOfficeStaffProfile", new
            {
                id = objOfficeStaffRegistration.UserRegnId
            });

        }

        /// <summary>
        /// To manage office staff
        /// </summary>
        /// <returns></returns>
        //public ActionResult ManageOfficeStaff()
        //{
        //    List<OfficeStaffRegistration> officeStaffRegistration = null;
        //    try
        //    {
        //        string api = string.Empty;
        //        ClientModel clientModelObj = new ClientModel();
        //        if (Session["UserType"] != null)
        //        {
        //            List<CountryViewModel> countryList1 = new List<CountryViewModel>();
        //            api = "Admin/GetCountryDetails/0";
        //            var countrylist1 = service.GetAPI(api);
        //            countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
        //            ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

        //            List<StateViewModel> stateList1 = new List<StateViewModel>();
        //            api = "/Admin/GetStatesByCountryId/1";
        //            var stateResult1 = service.GetAPI(api);
        //            stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
        //            ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", clientModelObj.StateId1);

        //            api = "Admin/GetCity?flag=StateId&value=" + clientModelObj.StateId1;
        //            List<Cities> cityList1 = new List<Cities>();
        //            var resultCity1 = service.GetAPI(api);
        //            cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
        //            ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", clientModelObj.CityId1);

        //            if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
        //            {
        //                ViewBag.Error = Constants.NoViewPrivilege;
        //                return View();
        //            }
        //            if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
        //            {
        //                api = "Admin/GetRolePrivilege";
        //                GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
        //                getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
        //                getRolePrivilege.ModuleID = 4;
        //                var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
        //                var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
        //                RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
        //                if (!apiResults.AllowView)
        //                {
        //                    ViewBag.Error = Constants.NoViewPrivilege;
        //                    return View();
        //                }
        //                ViewBag.AllowEdit = apiResults.AllowEdit;
        //                ViewBag.AllowDelete = apiResults.AllowDelete;
        //            }
        //            if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
        //            {
        //                ViewBag.AllowView = true;
        //                ViewBag.AllowEdit = true;
        //                ViewBag.AllowDelete = true;
        //            }
        //        }
        //        else
        //        {
        //            TempData["Failure"] = Constants.NotLoggedIn;
        //            return RedirectToAction("Login", "Account");
        //        }
        //        var inputs = new LocationSearchInputs();
        //        HomeController home = new HomeController();
        //        var loggedInUserId = (int)Session["loggedInUserId"];
        //        //get Work Role
        //        var userRoleDetails = home.GetUserRoleDetails(loggedInUserId);
        //        if (userRoleDetails != null)
        //        {

        //            if (userRoleDetails.WorkRoleId == 1)
        //            {
        //                inputs.CountryId = userRoleDetails.CountryId;
        //                inputs.StateId = null;
        //                inputs.CityId = null;
        //                ViewData["UserWorkRole"] = 1;
        //            }
        //            if (userRoleDetails.WorkRoleId == 2)
        //            {
        //                inputs.CountryId = userRoleDetails.CountryId;
        //                inputs.StateId = userRoleDetails.StateId;
        //                inputs.CityId = null;
        //                ViewData["UserWorkRole"] = 2;
        //            }
        //            if (userRoleDetails.WorkRoleId == 3)
        //            {
        //                inputs.CountryId = userRoleDetails.CountryId;
        //                inputs.StateId = userRoleDetails.StateId;
        //                inputs.CityId = userRoleDetails.CityId;
        //                ViewData["UserWorkRole"] = 3;
        //            }
        //            if (userRoleDetails.WorkRoleId == 4)
        //            {
        //                inputs.CountryId = userRoleDetails.CountryId;
        //                inputs.StateId = userRoleDetails.StateId;
        //                inputs.CityId = userRoleDetails.CityId;
        //                ViewData["UserWorkRole"] = 4;
        //            }
        //        }
        //        else
        //        {
        //            inputs.CountryId = null;
        //            inputs.StateId = null;
        //            inputs.CityId = null;
        //            ViewData["UserWorkRole"] = null;
        //        }
        //        var clientSearchInputsContent = JsonConvert.SerializeObject(inputs);
        //        api = "Admin/GetAllOfficeStaffDetailsByLocation";
        //        var result = service.PostAPIWithData(clientSearchInputsContent,api);
        //        officeStaffRegistration = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OfficeStaffRegistration>>(result.Result);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error(ex, "Error occurred in Admin Controller-ManageOfficeStaff");

        //    }
        //    return View(officeStaffRegistration);
        //}

        public ActionResult ManageOfficeStaff()
        {
            List<OfficeStaffRegistration> officeStaffRegistration = null;
            try
            {

                string country = Session["CountryId"].ToString();
                string state = Session["StateID"].ToString();
                string CityIdk = Session["CityIdk"].ToString();
                string api = string.Empty;
                ClientModel clientModelObj = new ClientModel();
                if (Session["UserType"] != null)
                {
                    if (country != "" && state != "" && CityIdk != "")
                    {
                        List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                        api = "Admin/GetCountryDetails/0";
                        var countrylist1 = service.GetAPI(api);
                        countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                        ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                        List<StateViewModel> stateList1 = new List<StateViewModel>();
                        api = "/Admin/GetStatesByCountryId/" + country;
                        var stateResult1 = service.GetAPI(api);
                        stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                        ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", state);

                        api = "Admin/GetCity?flag=StateId&value=" + state;
                        List<Cities> cityList1 = new List<Cities>();
                        var resultCity1 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                        ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", CityIdk);

                        api = "Admin/GetAllBranch/";
                        List<Cities> branchList1 = new List<Cities>();
                        var resultBranch1 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultBranch1);
                        ViewData["ListBranch"] = new SelectList(cityList1, "branchId", "branchName", clientModelObj.BranchId2);
                    }
                    else
                    {
                        List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                        api = "Admin/GetCountryDetails/0";
                        var countrylist1 = service.GetAPI(api);
                        countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                        ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                        List<StateViewModel> stateList1 = new List<StateViewModel>();
                        api = "/Admin/GetStatesByCountryId/1";
                        var stateResult1 = service.GetAPI(api);
                        stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                        ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", clientModelObj.StateId1);

                        api = "Admin/GetCity?flag=StateId&value=" + clientModelObj.StateId1;
                        List<Cities> cityList1 = new List<Cities>();
                        var resultCity1 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                        ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", clientModelObj.CityId1);

                        api = "Admin/GetAllBranch/";
                        List<Cities> branchList1 = new List<Cities>();
                        var resultBranch1 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultBranch1);
                        ViewData["ListBranch"] = new SelectList(cityList1, "branchId", "branchName", clientModelObj.BranchId2);
                    }


                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 24;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowView = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }

                #region created by silpa
                var inputs = new LocationSearchInputs();
                HomeController home = new HomeController();
                var loggedInUserId = (int)Session["loggedInUserId"];
                //get Work Role
                var userRoleDetails = home.GetUserRoleDetails(loggedInUserId);
                if (userRoleDetails != null)
                {

                    if (userRoleDetails.WorkRoleId == 1)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = null;
                        inputs.CityId = null;
                        ViewData["UserWorkRole"] = 1;
                    }
                    if (userRoleDetails.WorkRoleId == 2)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = userRoleDetails.StateId;
                        inputs.CityId = null;
                        ViewData["UserWorkRole"] = 2;
                    }
                    if (userRoleDetails.WorkRoleId == 3)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = userRoleDetails.StateId;
                        inputs.CityId = userRoleDetails.CityId;
                        ViewData["UserWorkRole"] = 3;
                    }



                    if (userRoleDetails.WorkRoleId == 4)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = userRoleDetails.StateId;
                        inputs.CityId = userRoleDetails.CityId;
                        ViewData["UserWorkRole"] = 4;
                    }
                }
                else
                {
                    inputs.CountryId = null;
                    inputs.StateId = null;
                    inputs.CityId = null;
                    ViewData["UserWorkRole"] = null;
                }
                #endregion

                var clientSearchInputsContent = JsonConvert.SerializeObject(inputs);
                api = "Admin/GetAllOfficeStaffDetailsByLocation";
                var result = service.PostAPIWithData(clientSearchInputsContent, api);
                officeStaffRegistration = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OfficeStaffRegistration>>(result.Result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-ManageOfficeStaff");

            }
            return View(officeStaffRegistration);
        }


        /// <summary>
        /// To manage office staff
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageOfficeStaffByLocation(LocationSearchInputs inputs)
        {
            List<OfficeStaffRegistration> officeStaffRegistration = null;
            try
            {
                if (inputs == null)
                {
                    inputs = new LocationSearchInputs();
                    inputs.CountryId = null;
                    inputs.StateId = null;
                    inputs.CityId = (int)Session["CityId"];
                }
                if (inputs.CountryId == 0 && inputs.StateId == 0 && inputs.CityId == 0 || inputs.CountryId == null && inputs.StateId == null && inputs.CityId == null)
                {
                    inputs.CountryId = null;
                    inputs.StateId = null;
                    inputs.CityId = (int)Session["CityId"];
                }
                string api = string.Empty;
                ClientModel clientModelObj = new ClientModel();
                if (Session["UserType"] != null)
                {
                    List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                    api = "Admin/GetCountryDetails/0";
                    var countrylist1 = service.GetAPI(api);
                    countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                    ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                    List<StateViewModel> stateList1 = new List<StateViewModel>();
                    api = "/Admin/GetStatesByCountryId/1";
                    var stateResult1 = service.GetAPI(api);
                    stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                    ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", clientModelObj.StateId1);

                    api = "Admin/GetCity?flag=StateId&value=" + clientModelObj.StateId1;
                    List<Cities> cityList1 = new List<Cities>();
                    var resultCity1 = service.GetAPI(api);
                    cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                    ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", clientModelObj.CityId1);

                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 4;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                var clientSearchInputsContent = JsonConvert.SerializeObject(inputs);
                api = "Admin/GetAllOfficeStaffDetailsByLocation";
                var result = service.PostAPIWithData(clientSearchInputsContent, api);
                officeStaffRegistration = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OfficeStaffRegistration>>(result.Result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-ManageOfficeStaff");

            }
            return PartialView("_ManageOfficeStaffPartial",officeStaffRegistration);
        }

        /// <summary>
        /// To manage office staff
        /// </summary>
        /// <returns></returns>
        public ActionResult SchedulingLogDetails()
        {
            List<ScheduledData> scheduleData = null;
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 4;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                api = "Client/GetAllScheduleLogDetails";
                var result = service.GetAPI(api);
                scheduleData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ScheduledData>>(result);
                for (int i = 0; i < scheduleData.Count; i++)
                {
                    string s = scheduleData[i].UpdatedDate.ToString("dd/MM/yyyy h:mm tt");
                    System.Globalization.CultureInfo customCulture = new System.Globalization.CultureInfo("en-US", true);

                    customCulture.DateTimeFormat.ShortDatePattern = "MM/dd/yyyy";

                    System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
                    System.Threading.Thread.CurrentThread.CurrentUICulture = customCulture;

                    DateTime dt = DateTime.ParseExact(s, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture);
                    DateTime newDate = System.Convert.ToDateTime(dt.ToString("yyyy-MM-dd h:mm tt"));

                    scheduleData[i].UpdatedDate = dt;
                }



                //Enums.AuditLogActionType ;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-ManageOfficeStaff");

            }
            return View(scheduleData);
        }

        public ActionResult LoginLogDetails(int id)
        {
            List<LoginLogs> loginData = null;
            List<UsersDetails> userTypes = null;
            //LoginLogs loginData = null;
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 4;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                api = "Client/GetLoginLogDetailsByTypeId/" + id;
                var result = service.GetAPI(api);
                //JsonConvert.DeserializeObject<LoginLogs>(result);
                loginData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LoginLogs>>(result);

                for (int i = 0; i < loginData.Count; i++)
                {
                    string s = loginData[i].LoginDate.ToString("dd/MM/yyyy h:mm tt");
                    System.Globalization.CultureInfo customCulture = new System.Globalization.CultureInfo("en-US", true);

                    customCulture.DateTimeFormat.ShortDatePattern = "MM/dd/yyyy";

                    System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
                    System.Threading.Thread.CurrentThread.CurrentUICulture = customCulture;

                    DateTime dt = DateTime.ParseExact(s, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture);
                    DateTime newDate = System.Convert.ToDateTime(dt.ToString("yyyy-MM-dd h:mm tt"));

                    loginData[i].LoginDate = dt;
                }


                api = "Client/GetUserTypeId";
                var result1 = service.GetAPI(api);
                //JsonConvert.DeserializeObject<LoginLogs>(result);
                userTypes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UsersDetails>>(result1);
                userTypes.Insert(0, new UsersDetails { UserTypeId = 0, UserType = "--Select--" });
                //var _userTypes = new SelectList(userTypes, "UserType", "UserTypeName");
                //var listCountry = _userTypes;
                //ViewBag["listUserType"] = listCountry;
                ViewBag.listUserType = new SelectList(userTypes, "UserTypeId", "UserType");
                ViewBag.selectedUserType = id;

                //Enums.AuditLogActionType ;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Client Controller-LoginLogDetails");

            }
            return View(loginData);
        }

        public ActionResult LoadScheduleLogById()
        {
            try
            {
                string api = "Admin/RetrieveSchedulelogById/0";
                var result = service.GetAPI(api);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Client Controller-LoadWorkShift");
                return null;
            }
        }
        public ActionResult GetLoginLogbyId(int UserTypeId)
        {
            try
            {
                string api = "Client/GetLoginLogDetailsByTypeId/" + UserTypeId;
                var result = service.GetAPI(api);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Client Controller-LoadWorkShift");
                return null;
            }
        }

        public ActionResult DeleteOfficeStaff(int id)
        {
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        TempData["Error"] = Constants.NoActionPrivilege;
                        return RedirectToAction("Error");
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 4;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            TempData["Error"] = Constants.NoViewPrivilege;
                            return RedirectToAction("Error");
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                api = "Admin/DeleteOfficeStaffDetails/" + id;
                HttpStatusCode result = service.PostAPI(null, api);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-DeleteOfficeStaff");

            }
            return RedirectToAction("ManageOfficeStaff");
        }

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult RolePrivileges(RolePrivileges rolePrivileges = null)
        {
            string api = string.Empty;
            if (Session["UserType"] != null)
            {
                if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                {
                    TempData["Error"] = Constants.NoActionPrivilege;
                    return RedirectToAction("Error");
                }
                if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                {
                    api = "Admin/GetRolePrivilege";
                    GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                    getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                    getRolePrivilege.ModuleID = 4;
                    var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                    var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                    RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                    if (!apiResults.AllowView)
                    {
                        TempData["Error"] = Constants.NoViewPrivilege;
                        return RedirectToAction("Error");
                    }
                    ViewBag.AllowEdit = apiResults.AllowEdit;
                    ViewBag.AllowDelete = apiResults.AllowDelete;
                }
                if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                {
                    ViewBag.AllowView = true;
                    ViewBag.AllowEdit = true;
                    ViewBag.AllowDelete = true;
                }
            }
            else
            {
                TempData["Failure"] = Constants.NotLoggedIn;
                return RedirectToAction("Login", "Account");
            }
            api = "Admin/GetRoles/0";
            var roleList = JsonConvert.DeserializeObject<List<Roles>>(service.GetAPI(api));
            //listCountry = JsonConvert.DeserializeObject<List<CountryViewModel>>(roleList);
            ViewBag.RoleList = new SelectList(roleList, "RoleId", "RoleName", 0);

            if (rolePrivileges.Privileges != null)
            {
                api = "Admin/SaveRolePrivileges";
                foreach (var item in rolePrivileges.Privileges)
                {
                    item.RoleId = rolePrivileges.RoleId;
                    item.PrivilegeId = (item.PrivilegeId == 0) ? item.RoleId : item.PrivilegeId;
                }
                var serviceContent = JsonConvert.SerializeObject(rolePrivileges);

                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    TempData["Success"] = "Role details updated Successfully.";
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    TempData["Failure"] = "You are not authorized to perform this action.";
                }
                else if (result == HttpStatusCode.Conflict)
                {
                    TempData["Failure"] = "Data already exist. Please enter different data.";
                }
                else
                {
                    TempData["Failure"] = "Updation Failed. Please try again later.";
                }
            }

            return View(rolePrivileges);
        }

        public ActionResult LoadSelectedPrivileges(int RoleId)
        {

            string api = string.Empty;
            if (Session["UserType"] != null)
            {
                if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                {
                    TempData["Error"] = Constants.NoActionPrivilege;
                    return RedirectToAction("Error");
                }
                if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                {
                    api = "Admin/GetRolePrivilege";
                    GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                    getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                    getRolePrivilege.ModuleID = 4;
                    var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                    var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                    RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                    if (!apiResults.AllowView)
                    {
                        TempData["Error"] = Constants.NoViewPrivilege;
                        return RedirectToAction("Error");
                    }
                    ViewBag.AllowEdit = apiResults.AllowEdit;
                    ViewBag.AllowDelete = apiResults.AllowDelete;
                }
                if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                {
                    ViewBag.AllowView = true;
                    ViewBag.AllowEdit = true;
                    ViewBag.AllowDelete = true;
                }
            }
            api = "Admin/GetRolePrivileges/" + RoleId;
            var result = service.GetAPI(api);
            RolePrivileges rolePrivileges = new RolePrivileges();
            if (RoleId > 0)
                rolePrivileges.Privileges = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Privileges>>(result);
            rolePrivileges.RoleId = RoleId;
            return PartialView("_PrivilegesList", rolePrivileges);
        }

        public ActionResult AdminDashboard()
        {
            ClientModel clientModelObj = new ClientModel();

            try
            {
                string api = string.Empty;
                string country = Session["CountryId"].ToString();
                string state = Session["StateID"].ToString();
                string CityIdk = Session["CityIdk"].ToString();
                string branch = Session["BranchID"].ToString();
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }

                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                        api = "Admin/GetCountryDetails/0";
                        var countrylist1 = service.GetAPI(api);
                        countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                        ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                        List<StateViewModel> stateList1 = new List<StateViewModel>();
                        api = "/Admin/GetStatesByCountryId/"+country;
                        var stateResult1 = service.GetAPI(api);
                        stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                        ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", clientModelObj.StateId1);

                        api = "Admin/GetCity?flag=StateId&value=" + state;
                        List<Cities> cityList1 = new List<Cities>();
                        var resultCity1 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                        ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", clientModelObj.CityId1);


                        api = "Admin/GetAllBranch/";
                        List<Cities> branchList1 = new List<Cities>();
                        var resultBranch1 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultBranch1);
                        ViewData["ListBranch"] = new SelectList(cityList1, "branchId", "branchName", clientModelObj.BranchId2);

                        api = "Admin/GetBranch?flag=StateId&value=" + CityIdk;
                        List<Cities> cityList11 = new List<Cities>();
                        var resultCity11 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity11);
                        ViewData["ListBranchdemo"] = new SelectList(cityList1, "branchId", "branchName", clientModelObj.BranchId2);


                        //api = "Admin/GetCity?flag=StateId&value=" + "";
                        //List<Cities> cityList11 = new List<Cities>();
                        //var resultCity11 = service.GetAPI(api);
                        //cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity11);
                        //ViewData["ListBranchdemo"] = new SelectList(cityList1, "CityId", "CityName", clientModelObj.CityId1);

                    }
                    else
                    {
                        List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                        api = "Admin/GetCountryDetails/0";
                        var countrylist1 = service.GetAPI(api);
                        countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                        ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                        List<StateViewModel> stateList1 = new List<StateViewModel>();
                        api = "/Admin/GetStatesByCountryId/1";
                        var stateResult1 = service.GetAPI(api);
                        stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                        ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", clientModelObj.StateId1);

                        api = "Admin/GetCity?flag=StateId&value=" + clientModelObj.StateId1;
                        List<Cities> cityList1 = new List<Cities>();
                        var resultCity1 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                        ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", clientModelObj.CityId1);



                        api = "Admin/GetAllBranch/";
                        List<Cities> branchList1 = new List<Cities>();
                        var resultBranch1 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultBranch1);
                        ViewData["ListBranch"] = new SelectList(cityList1, "branchId", "branchName", clientModelObj.BranchId2);


                        api = "Admin/GetCity?flag=StateId&value=" + "";
                        List<Cities> cityList11 = new List<Cities>();
                        var resultCity11 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity11);
                        ViewData["ListBranchdemo"] = new SelectList(cityList1, "CityId", "CityName", clientModelObj.CityId1);
                        
                    }

                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                       
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 2;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            Session["Adminchecking"] = 1;
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }

               

                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                #region created by silpa
                var inputs = new LocationSearchInputs();
                HomeController home = new HomeController();
                var loggedInUserId = (int)Session["loggedInUserId"];
                //get Work Role
                var userRoleDetails = home.GetUserRoleDetails(loggedInUserId);
                if (userRoleDetails != null)
                {

                    if (userRoleDetails.WorkRoleId == 1)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = null;
                        inputs.CityId = null;
                        ViewData["UserWorkRole"] = 1;
                    }
                    if (userRoleDetails.WorkRoleId == 2)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = userRoleDetails.StateId;
                        inputs.CityId = null;
                        ViewData["UserWorkRole"] = 2;
                    }
                    if (userRoleDetails.WorkRoleId == 3)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = userRoleDetails.StateId;
                        inputs.CityId = userRoleDetails.CityId;
                        ViewData["UserWorkRole"] = 3;
                    }
                    if (userRoleDetails.WorkRoleId == 4)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = userRoleDetails.StateId;
                        inputs.CityId = userRoleDetails.CityId;
                        ViewData["UserWorkRole"] = 4;


                    }
                    if (branch != "0")
                    {

                        ViewData["Admin"] = 0;
                    }
                    else
                    {

                        if (country != "0" && state != "0" && CityIdk != "0")
                        {
                            ViewData["Admin"] = 0;
                        }
                        else
                        {
                            ViewData["Admin"] = 1;
                        }
                        
                    }

                }
                else
                {
                    inputs.CountryId = null;
                    inputs.StateId = null;
                    inputs.CityId = null;
                    ViewData["UserWorkRole"] = null;
                    ViewData["Admin"] = 1;
                }

                if(inputs.CountryId != null||inputs.StateId!=null||inputs.CityId!=null)
                {
                    Session["Adminchecking"] = 1;

                }
                #endregion
                return View();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-AdminDashboard");
                return null;
            }
        }

       


        public ActionResult GetCity(int state)
        {
            string api = string.Empty;
          
            api = "Admin/GetCity?flag=StateId&value=" + state;
            List<Cities> cityList1 = new List<Cities>();
            var resultCity1 = service.GetAPI(api);
            cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
            //ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", "");
            
           // temp.Add


            return Json(cityList1);


        }


        #region created by silpa for branch or franchise
        public ActionResult GetCountry(int city)
        {
            string api = string.Empty;

            api = "Admin/GetCountry?flag=StateId&value=" + city;
            List<Cities> cityList1 = new List<Cities>();
            var resultCity1 = service.GetAPI(api);
            cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
           

            return Json(cityList1);


        }
        #endregion


        #region created by silpa for branch or franchise
        public ActionResult Getstate(int city)
        {
            string api = string.Empty;

            api = "Admin/GetstateId?flag=StateId&value=" + city;
            List<Cities> cityList1 = new List<Cities>();
            var resultCity1 = service.GetAPI(api);
            cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
            //ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", "");

            // temp.Add


            return Json(cityList1);


        }
        #endregion


        #region created by silpa 
        public ActionResult GetCityDetails(int city)
        {
            string api = string.Empty;

            api = "Admin/GetCityDetails?flag=StateId&value=" + city;
            List<Cities> cityList1 = new List<Cities>();
            var resultCity1 = service.GetAPI(api);
            cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
           
            return Json(cityList1);


        }
        #endregion

        #region created by silpa for branch or franchise
        public ActionResult GetBranch(int city)
        {
            string api = string.Empty;

            api = "Admin/GetBranch?flag=StateId&value=" + city;
            List<Cities> cityList1 = new List<Cities>();
            var resultCity1 = service.GetAPI(api);
            cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
            //ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", "");

            // temp.Add


            return Json(cityList1);


        }
        #endregion
        public ActionResult GetBranchById(int id)
        {
            string api = string.Empty;

            api = "Admin/GetBranchByIds?BranchId=" + id;
            List<Cities> cityList1 = new List<Cities>();
            var resultCity1 = service.GetAPI(api);
            cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
            //ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", "");

            // temp.Add


            return Json(cityList1);


        }

        public ActionResult GetBranchbyId(int city)
        {
            string api = string.Empty;

            api = "Admin/GetBranchById?flag=StateId&value=" + city;
            List<Cities> cityList1 = new List<Cities>();
            var resultCity1 = service.GetAPI(api);
            cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
            //ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", "");

            // temp.Add


            return Json(cityList1);


        }
        public ActionResult AdminBookingHistory()
        {
            ClientModel clientModelObj = new ClientModel();
            string api = string.Empty;
            string country = Session["CountryId"].ToString();
            string state = Session["StateID"].ToString();
            string CityIdk = Session["CityIdk"].ToString();
            if (Session["UserType"] != null)
            {
                if(country!=""&&state!=""&&CityIdk!="")
                {
                    List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                    api = "Admin/GetCountryDetails/0";
                    var countrylist1 = service.GetAPI(api);
                    countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                    ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                    List<StateViewModel> stateList1 = new List<StateViewModel>();
                    api = "/Admin/GetStatesByCountryId/" + country;
                    var stateResult1 = service.GetAPI(api);
                    stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                    ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", clientModelObj.StateId1);

                    api = "Admin/GetCity?flag=StateId&value=" + state;
                    List<Cities> cityList1 = new List<Cities>();
                    var resultCity1 = service.GetAPI(api);
                    cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                    ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", clientModelObj.CityId1);

                    api = "Admin/GetBranch?flag=StateId&value=" + CityIdk;
                    //api = "Admin/GetAllBranch/";
                    List<Cities> branchList1 = new List<Cities>();
                    var resultBranch1 = service.GetAPI(api);
                    cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultBranch1);
                    ViewData["ListBranch"] = new SelectList(cityList1, "branchId", "branchName", clientModelObj.BranchId2);
                }
                else
                {
                    List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                    api = "Admin/GetCountryDetails/0";
                    var countrylist1 = service.GetAPI(api);
                    countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                    ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                    List<StateViewModel> stateList1 = new List<StateViewModel>();
                    api = "/Admin/GetStatesByCountryId/1";
                    var stateResult1 = service.GetAPI(api);
                    stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                    ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", state);

                    api = "Admin/GetCity?flag=StateId&value=" + clientModelObj.StateId1;
                    List<Cities> cityList1 = new List<Cities>();
                    var resultCity1 = service.GetAPI(api);
                    cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                    ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", clientModelObj.CityId1);

                    api = "Admin/GetAllBranch/";
                    List<Cities> branchList1 = new List<Cities>();
                    var resultBranch1 = service.GetAPI(api);
                    cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultBranch1);
                    ViewData["ListBranch"] = new SelectList(cityList1, "branchId", "branchName", clientModelObj.BranchId2);
                }
                

                if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                {
                    ViewBag.Error = Constants.NoViewPrivilege;
                    return View();
                }
                if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                {
                    api = "Admin/GetRolePrivilege";
                    GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                    getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                    getRolePrivilege.ModuleID = 22;
                    var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                    var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                    RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                    if (!apiResults.AllowView)
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    ViewBag.AllowEdit = apiResults.AllowEdit;
                    ViewBag.AllowDelete = apiResults.AllowDelete;
                }
                if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                {
                    ViewBag.AllowView = true;
                    ViewBag.AllowEdit = true;
                    ViewBag.AllowDelete = true;
                }
            }
            else
            {
                TempData["Failure"] = Constants.NotLoggedIn;
                return RedirectToAction("Login", "Account");
            }
            var listPaySearch = new SelectList(new[]
            {
                new { ID = "0", Name = "--Select--" },
                new { ID = "1", Name = "Yearly" },
                new { ID = "2", Name = "Monthly" },
                new { ID = "3", Name = "Date Range" },
            },
            "ID", "Name", 0);

            ViewData["listPaySearch"] = listPaySearch;
            ViewData["listYears"] = GetYears();
            Months months = new Months();
            ViewData["listMonths"] = months.GetMonths();


            #region created by silpa
            var inputs = new LocationSearchInputs();
            HomeController home = new HomeController();
            var loggedInUserId = (int)Session["loggedInUserId"];
            //get Work Role
            var userRoleDetails = home.GetUserRoleDetails(loggedInUserId);
            if (userRoleDetails != null)
            {

                if (userRoleDetails.WorkRoleId == 1)
                {
                    inputs.CountryId = userRoleDetails.CountryId;
                    inputs.StateId = null;
                    inputs.CityId = null;
                    ViewData["UserWorkRole"] = 1;
                }
                if (userRoleDetails.WorkRoleId == 2)
                {
                    inputs.CountryId = userRoleDetails.CountryId;
                    inputs.StateId = userRoleDetails.StateId;
                    inputs.CityId = null;
                    ViewData["UserWorkRole"] = 2;
                }
                if (userRoleDetails.WorkRoleId == 3)
                {
                    inputs.CountryId = userRoleDetails.CountryId;
                    inputs.StateId = userRoleDetails.StateId;
                    inputs.CityId = userRoleDetails.CityId;
                    ViewData["UserWorkRole"] = 3;
                }
                if (userRoleDetails.WorkRoleId == 4)
                {
                    inputs.CountryId = userRoleDetails.CountryId;
                    inputs.StateId = userRoleDetails.StateId;
                    inputs.CityId = userRoleDetails.CityId;
                    ViewData["UserWorkRole"] = 4;
                }
            }
            else
            {
                inputs.CountryId = null;
                inputs.StateId = null;
                inputs.CityId = null;
                ViewData["UserWorkRole"] = null;
            }
            #endregion


            List<ServicesViewModel> listService = new List<ServicesViewModel>();
            api = "Admin/RetrieveServiceDetails/0";
            var serviveResult = service.GetAPI(api);
            listService = JsonConvert.DeserializeObject<List<ServicesViewModel>>(serviveResult);
            ViewBag.Service = new SelectList(listService, "ServiceId", "Name", 0);
            return View();
        }

        public JsonResult GetServiceById(int serviceId)
        {
            try
            {
                string api = "Admin/RetrieveServiceDetails/" + serviceId;
                var services = service.GetAPI(api);
                return Json(services, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-GetServiceById");
                return null;
            }
        }

        public ActionResult PublicUserInvoiceHistory()
        {
            ClientModel clientModelObj = new ClientModel();
            string api = string.Empty;
            string country = Session["CountryId"].ToString();
            string state = Session["StateID"].ToString();
            string CityIdk = Session["CityIdk"].ToString();
            if (Session["UserType"] != null)
            {
                if(country!=""&&state!=""&&CityIdk!="")
                {
                    List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                    api = "Admin/GetCountryDetails/0";
                    var countrylist1 = service.GetAPI(api);
                    countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                    ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                    List<StateViewModel> stateList1 = new List<StateViewModel>();
                    api = "/Admin/GetStatesByCountryId/" + country;
                    var stateResult1 = service.GetAPI(api);
                    stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                    ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", state);

                    api = "Admin/GetCity?flag=StateId&value=" + state;
                    List<Cities> cityList1 = new List<Cities>();
                    var resultCity1 = service.GetAPI(api);
                    cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                    ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", CityIdk);

                    //api = "Admin/GetAllBranch/";
                    api = "Admin/GetBranch?flag=StateId&value=" + CityIdk;
                    List<Cities> branchList1 = new List<Cities>();
                    var resultBranch1 = service.GetAPI(api);
                    cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultBranch1);
                    ViewData["ListBranch"] = new SelectList(cityList1, "branchId", "branchName", clientModelObj.BranchId2);
                }
                else
                {
                    List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                    api = "Admin/GetCountryDetails/0";
                    var countrylist1 = service.GetAPI(api);
                    countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                    ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                    List<StateViewModel> stateList1 = new List<StateViewModel>();
                    api = "/Admin/GetStatesByCountryId/1";
                    var stateResult1 = service.GetAPI(api);
                    stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                    ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", clientModelObj.StateId1);

                    api = "Admin/GetCity?flag=StateId&value=" + clientModelObj.StateId1;
                    List<Cities> cityList1 = new List<Cities>();
                    var resultCity1 = service.GetAPI(api);
                    cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                    ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", clientModelObj.CityId1);

                    api = "Admin/GetAllBranch/";
                    List<Cities> branchList1 = new List<Cities>();
                    var resultBranch1 = service.GetAPI(api);
                    cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultBranch1);
                    ViewData["ListBranch"] = new SelectList(cityList1, "branchId", "branchName", clientModelObj.BranchId2);
                }


                if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                {
                    ViewBag.Error = Constants.NoViewPrivilege;
                    return View();
                }
                if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                {
                    api = "Admin/GetRolePrivilege";
                    GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                    getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                    getRolePrivilege.ModuleID = 22;
                    var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                    var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                    RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                    if (!apiResults.AllowView)
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    ViewBag.AllowEdit = apiResults.AllowEdit;
                    ViewBag.AllowDelete = apiResults.AllowDelete;
                }
                if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                {
                    ViewBag.AllowView = true;
                    ViewBag.AllowEdit = true;
                    ViewBag.AllowDelete = true;
                }
            }
            else
            {
                TempData["Failure"] = Constants.NotLoggedIn;
                return RedirectToAction("Login", "Account");
            }
            var listPaySearch = new SelectList(new[]
            {
                new { ID = "0", Name = "--Select--" },
                new { ID = "1", Name = "Yearly" },
                new { ID = "2", Name = "Monthly" },
                new { ID = "3", Name = "Date Range" },
            },
            "ID", "Name", 0);

            ViewData["listPaySearch"] = listPaySearch;
            ViewData["listYears"] = GetYears();
            Months months = new Months();
            ViewData["listMonths"] = months.GetMonths();

            List<ServicesViewModel> listService = new List<ServicesViewModel>();
            api = "Admin/RetrieveServiceDetails/0";
            var serviveResult = service.GetAPI(api);
            listService = JsonConvert.DeserializeObject<List<ServicesViewModel>>(serviveResult);
            ViewBag.Service = new SelectList(listService, "ServiceId", "Name", 0);

            var CityId = (int)Session["CityId"];
            var inputs = new LocationSearchInputs { CityId = CityId };
            var clientSearchInputsContent = JsonConvert.SerializeObject(inputs);
            string UserApi = "Admin/GetAllUserDetailsByLocation";
            var Clients = service.PostAPIWithData(clientSearchInputsContent, UserApi);
            List<UsersDetails> scheduleDetailsList = new List<UsersDetails>();
            List<UsersDetails> fiteredscheduleDetailsList = new List<UsersDetails>();
            scheduleDetailsList = JsonConvert.DeserializeObject<List<UsersDetails>>(Clients.Result);
            fiteredscheduleDetailsList = scheduleDetailsList.Where(a => a.UserTypeId == 2 && a.UserVerified == true).ToList();
            ViewBag.User = new SelectList(fiteredscheduleDetailsList, "UserRegnId", "FullName", 0);

           // var inputs = new LocationSearchInputs();
            HomeController home = new HomeController();
            var loggedInUserId = (int)Session["loggedInUserId"];
            //get Work Role
            var userRoleDetails = home.GetUserRoleDetails(loggedInUserId);
            if (userRoleDetails != null)
            {

                if (userRoleDetails.WorkRoleId == 1)
                {
                    inputs.CountryId = userRoleDetails.CountryId;
                    inputs.StateId = null;
                    inputs.CityId = null;
                    ViewData["UserWorkRole"] = 1;
                }
                if (userRoleDetails.WorkRoleId == 2)
                {
                    inputs.CountryId = userRoleDetails.CountryId;
                    inputs.StateId = userRoleDetails.StateId;
                    inputs.CityId = null;
                    ViewData["UserWorkRole"] = 2;
                }
                if (userRoleDetails.WorkRoleId == 3)
                {
                    inputs.CountryId = userRoleDetails.CountryId;
                    inputs.StateId = userRoleDetails.StateId;
                    inputs.CityId = userRoleDetails.CityId;
                    ViewData["UserWorkRole"] = 3;
                }
                if (userRoleDetails.WorkRoleId == 4)
                {
                    inputs.CountryId = userRoleDetails.CountryId;
                    inputs.StateId = userRoleDetails.StateId;
                    inputs.CityId = userRoleDetails.CityId;
                    ViewData["UserWorkRole"] = 4;
                }
            }
            else
            {
                inputs.CountryId = null;
                inputs.StateId = null;
                inputs.CityId = null;
                ViewData["UserWorkRole"] = null;
            }



            return View();
        }

        public ActionResult CaretakerWiseBooking()
        {
            ClientModel clientModelObj = new ClientModel();
            string api = string.Empty;
            string country = Session["CountryId"].ToString();
            string state = Session["StateID"].ToString();
            string CityIdk = Session["CityIdk"].ToString();
            if (Session["UserType"] != null)
            {
                if(country!=""&&state!=""&&CityIdk!="")
                {
                    List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                    api = "Admin/GetCountryDetails/0";
                    var countrylist1 = service.GetAPI(api);
                    countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                    ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                    List<StateViewModel> stateList1 = new List<StateViewModel>();
                    api = "/Admin/GetStatesByCountryId/"+country;
                    var stateResult1 = service.GetAPI(api);
                    stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                    ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", state);

                    api = "Admin/GetCity?flag=StateId&value=" + state;
                    List<Cities> cityList1 = new List<Cities>();
                    var resultCity1 = service.GetAPI(api);
                    cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                    ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", CityIdk);

                    //api = "Admin/GetAllBranch/";
                    api = "Admin/GetBranch?flag=StateId&value=" + CityIdk;
                    List<Cities> branchList1 = new List<Cities>();
                    var resultBranch1 = service.GetAPI(api);
                    cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultBranch1);
                    ViewData["ListBranch"] = new SelectList(cityList1, "branchId", "branchName", clientModelObj.BranchId2);
                }
                else
                {
                    List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                    api = "Admin/GetCountryDetails/0";
                    var countrylist1 = service.GetAPI(api);
                    countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                    ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                    List<StateViewModel> stateList1 = new List<StateViewModel>();
                    api = "/Admin/GetStatesByCountryId/1";
                    var stateResult1 = service.GetAPI(api);
                    stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                    ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", clientModelObj.StateId1);

                    api = "Admin/GetCity?flag=StateId&value=" + clientModelObj.StateId1;
                    List<Cities> cityList1 = new List<Cities>();
                    var resultCity1 = service.GetAPI(api);
                    cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                    ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", clientModelObj.CityId1);

                    api = "Admin/GetAllBranch/";
                    List<Cities> branchList1 = new List<Cities>();
                    var resultBranch1 = service.GetAPI(api);
                    cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultBranch1);
                    ViewData["ListBranch"] = new SelectList(cityList1, "branchId", "branchName", clientModelObj.BranchId2);
                }
            

                if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                {
                    ViewBag.Error = Constants.NoViewPrivilege;
                    return View();
                }
                if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                {
                    api = "Admin/GetRolePrivilege";
                    GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                    getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                    getRolePrivilege.ModuleID = 22;
                    var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                    var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                    RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                    if (!apiResults.AllowView)
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    ViewBag.AllowEdit = apiResults.AllowEdit;
                    ViewBag.AllowDelete = apiResults.AllowDelete;
                }
                if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                {
                    ViewBag.AllowView = true;
                    ViewBag.AllowEdit = true;
                    ViewBag.AllowDelete = true;
                }
            }
            else
            {
                TempData["Failure"] = Constants.NotLoggedIn;
                return RedirectToAction("Login", "Account");
            }
            var listPaySearch = new SelectList(new[]
            {
                new { ID = "0", Name = "--Select--" },
                new { ID = "1", Name = "Yearly" },
                new { ID = "2", Name = "Monthly" },
                new { ID = "3", Name = "Date Range" },
            },
            "ID", "Name", 0);

            ViewData["listPaySearch"] = listPaySearch;
            ViewData["listYears"] = GetYears();
            Months months = new Months();
            ViewData["listMonths"] = months.GetMonths();

            List<ServicesViewModel> listService = new List<ServicesViewModel>();
            api = "Admin/RetrieveServiceDetails/0";
            var serviveResult = service.GetAPI(api);
            listService = JsonConvert.DeserializeObject<List<ServicesViewModel>>(serviveResult);
            ViewBag.Service = new SelectList(listService, "ServiceId", "Name", 0);
            string caretakerapi = "CareTaker/RetrieveCareTakerListForDdlByLocation/";
            var inputs = new LocationSearchInputs();
            HomeController home = new HomeController();
            var loggedInUserId = (int)Session["loggedInUserId"];
            //get Work Role
            var userRoleDetails = home.GetUserRoleDetails(loggedInUserId);
            if (userRoleDetails != null)
            {

                if (userRoleDetails.WorkRoleId == 1)
                {
                    inputs.CountryId = userRoleDetails.CountryId;
                    inputs.StateId = null;
                    inputs.CityId = null;
                    ViewData["UserWorkRole"] = 1;
                }
                if (userRoleDetails.WorkRoleId == 2)
                {
                    inputs.CountryId = userRoleDetails.CountryId;
                    inputs.StateId = userRoleDetails.StateId;
                    inputs.CityId = null;
                    ViewData["UserWorkRole"] = 2;
                }
                if (userRoleDetails.WorkRoleId == 3)
                {
                    inputs.CountryId = userRoleDetails.CountryId;
                    inputs.StateId = userRoleDetails.StateId;
                    inputs.CityId = userRoleDetails.CityId;
                    ViewData["UserWorkRole"] = 3;
                }
                if (userRoleDetails.WorkRoleId == 4)
                {
                    inputs.CountryId = userRoleDetails.CountryId;
                    inputs.StateId = userRoleDetails.StateId;
                    inputs.CityId = userRoleDetails.CityId;
                    ViewData["UserWorkRole"] = 4;
                }
            }
            else
            {
                inputs.CountryId = null;
                inputs.StateId = null;
                inputs.CityId = null;
                ViewData["UserWorkRole"] = null;
            }
            var clientSearchInputsContent = JsonConvert.SerializeObject(inputs);
            var CareTakerResult = service.PostAPIWithData(clientSearchInputsContent, caretakerapi);
            List<Caretakers> listCaretakers = new List<Caretakers>();
            listCaretakers = JsonConvert.DeserializeObject<List<Caretakers>>(CareTakerResult.Result);
            ViewBag.Caretaker = new SelectList(listCaretakers, "CaretakerId", "CareTakerName", 0);
            return View();
        }

        public ActionResult BookingHistoryList()
        {
            List<BookingHistory> bookingHistoryList = new List<BookingHistory>();
            try
            {
                BookingHistorySearch bookingHistorySearch = new BookingHistorySearch()
                {
                    StatusId = 0
                };
                string api = "Admin/GetBookingHistoryList";
                var searchInputModel = JsonConvert.SerializeObject(bookingHistorySearch);
                var result = service.PostAPIWithData(searchInputModel, api);
                bookingHistoryList = JsonConvert.DeserializeObject<List<BookingHistory>>(result.Result);

                return PartialView("_BookingHistoryPartial", bookingHistoryList);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public ActionResult SearchClentInvoiceHistory(InvoiceHistory searchInputs)
        {
            string siteUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/";
            List<InvoiceSearchInpts> apiResults = new List<InvoiceSearchInpts>();
            try
            {
                if (searchInputs != null)
                {
                    if (searchInputs.FromDate != null && searchInputs.ToDate != null)
                    {
                        searchInputs.Year = null;
                        searchInputs.Month = null;
                    }
                    string api = "Client/GetInvoiceHistoryList";
                    var advancedSearchInputModel = JsonConvert.SerializeObject(searchInputs);
                    var result = service.PostAPIWithData(advancedSearchInputModel, api);
                    apiResults = JsonConvert.DeserializeObject<List<InvoiceSearchInpts>>(result.Result);
                }
                foreach (var res in apiResults)
                {
                    if (res.PdfFile != null)
                    {
                        var file = Server.MapPath("~/PCMS/Invoice/Client/") + res.ClientName + '_' + res.InvoicePrefix + '_' + res.InvoiceNumber + ".pdf";
                        System.IO.File.WriteAllBytes(file, res.PdfFile);

                        res.PdfFilePath = siteUrl + "PCMS/Invoice/Client/" + res.ClientName + '_' + res.InvoicePrefix + '_' + res.InvoiceNumber + ".pdf";
                        string api = "Client/UpdateClientInvoice";
                        var residentContent = JsonConvert.SerializeObject(res);
                        var result = service.PostAPIWithData(residentContent, api);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-SearchClentInvoiceHistory");
                return null;
            }
            return PartialView("_InvoiceHistoryPartial", apiResults);
        }

        public ActionResult SearchBookingHistory(BookingHistorySearch bookingHistorySearch)
        {
            try
            {
                string api = string.Empty;

                List<UserBookingInvoiceReport> bookingHistoryList = new List<UserBookingInvoiceReport>();
                string bookingHistoryapi = "Admin/GetBookingHistoryList";

                if (bookingHistorySearch == null)
                {
                    bookingHistorySearch = new BookingHistorySearch();
                    bookingHistorySearch.StatusId = 0;
                }

                bookingHistorySearch.StatusId = 0;
                var searchInputModel = JsonConvert.SerializeObject(bookingHistorySearch);
                var result = service.PostAPIWithData(searchInputModel, bookingHistoryapi);
                bookingHistoryList = JsonConvert.DeserializeObject<List<UserBookingInvoiceReport>>(result.Result);
                return PartialView("_SearchBookingHistory", bookingHistoryList);
            }

            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-Country");
                return null;
            }
        }

        public ActionResult SearchPublicUserInvoiceHistory(BookingHistorySearch invoiceHistorySearch)
        {
            try
            {
                string api = string.Empty;

                List<UserInvoiceParams> InvoiceHistoryList = new List<UserInvoiceParams>();
                string bookingHistoryapi = "Admin/GetBookingInvoiceList";

                if (invoiceHistorySearch == null)
                {
                    invoiceHistorySearch = new BookingHistorySearch();
                    invoiceHistorySearch.StatusId = 0;
                }

                invoiceHistorySearch.StatusId = 0;
                var searchInputModel = JsonConvert.SerializeObject(invoiceHistorySearch);
                var result = service.PostAPIWithData(searchInputModel, bookingHistoryapi);
                InvoiceHistoryList = JsonConvert.DeserializeObject<List<UserInvoiceParams>>(result.Result);
              
                return PartialView("_SearchPublicUserInvoiceHistory", InvoiceHistoryList);
            }

            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in searching invoice history");
                return null;
            }
        }


        public ActionResult UserBookingDetails(int? bookingId)
        {

            
            UserBookingInvoiceReport bookingDetail = new UserBookingInvoiceReport();
            string api = "Admin/GetBookingHistoryDetail/" + bookingId;
            var serviceResult = service.GetAPI(api);
            bookingDetail = JsonConvert.DeserializeObject<UserBookingInvoiceReport>(serviceResult);

            //api = "Admin/RetrieveServiceDetails/0";
            //var serviveResult = service.GetAPI(api);
            //List<ServicesViewModel> listService = new List<ServicesViewModel>();
            //listService = JsonConvert.DeserializeObject<List<ServicesViewModel>>(serviveResult);
            //ViewBag.Service = new SelectList(listService, "ServiceId", "Name", bookingDetail.ServiceName);

            List<ServicesViewModel> listService = new List<ServicesViewModel>();
            api = "Admin/RetrieveServiceDetails/0";
            var countrylist = service.GetAPI(api);
            listService = JsonConvert.DeserializeObject<List<ServicesViewModel>>(countrylist);
            var _countryList = new SelectList(listService, "ServiceId", "Name", bookingDetail.ServiceId);
            ViewData["Service"] = _countryList;

            AdvancedSearchInputModel searchInputs = new AdvancedSearchInputModel
            {
                Services = bookingDetail.ServiceId
            };

            List<SearchedCareTakers> listCaretaker = new List<SearchedCareTakers>();
            api = "Home/SearchCareTaker";
            var advancedSearchInputModel = JsonConvert.SerializeObject(searchInputs);
            var result = service.PostAPIWithData(advancedSearchInputModel, api);
            listCaretaker = JsonConvert.DeserializeObject<List<SearchedCareTakers>>(result.Result);
            var _caretakerList = new SelectList(listCaretaker, "CaretakerId", "Fullname", bookingDetail.CaretakerId);
            ViewData["Caretaker"] = _caretakerList;

            return PartialView(bookingDetail);
        }
        public ActionResult AdminDashboardUserBookingDetails(int? bookingId)
        {

            UserBookingInvoiceReport bookingDetail = new UserBookingInvoiceReport();
            string api = "Admin/GetAdminDashboardBookingHistoryDetail/" + bookingId;
            var serviceResult = service.GetAPI(api);
            bookingDetail = JsonConvert.DeserializeObject<UserBookingInvoiceReport>(serviceResult);

            //api = "Admin/RetrieveServiceDetails/0";
            //var serviveResult = service.GetAPI(api);
            //List<ServicesViewModel> listService = new List<ServicesViewModel>();
            //listService = JsonConvert.DeserializeObject<List<ServicesViewModel>>(serviveResult);
            //ViewBag.Service = new SelectList(listService, "ServiceId", "Name", bookingDetail.ServiceName);

            List<ServicesViewModel> listService = new List<ServicesViewModel>();
            api = "Admin/RetrieveServiceDetails/0";
            var countrylist = service.GetAPI(api);
            listService = JsonConvert.DeserializeObject<List<ServicesViewModel>>(countrylist);
            var _countryList = new SelectList(listService, "ServiceId", "Name", bookingDetail.ServiceId);
            ViewData["Service"] = _countryList;

            AdvancedSearchInputModel searchInputs = new AdvancedSearchInputModel
            {
                Services = bookingDetail.ServiceId
            };

            List<SearchedCareTakers> listCaretaker = new List<SearchedCareTakers>();
            api = "Home/SearchCareTaker";
            var advancedSearchInputModel = JsonConvert.SerializeObject(searchInputs);
            var result = service.PostAPIWithData(advancedSearchInputModel, api);
            listCaretaker = JsonConvert.DeserializeObject<List<SearchedCareTakers>>(result.Result);
            var _caretakerList = new SelectList(listCaretaker, "CaretakerId", "Fullname", bookingDetail.CaretakerId);
            ViewData["Caretaker"] = _caretakerList;

            return PartialView(bookingDetail);
        }

        /// <summary>
        /// Countries this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Country()
        {
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                CountryViewModel countryModel = new CountryViewModel();
                return View(countryModel);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-Country");
                return null;

            }
        }

        /// <summary>
        /// Creates the country.
        /// </summary>
        /// <param name="countryDetails">The country details.</param>
        /// <returns></returns>
        public ActionResult CreateCountry(CountryViewModel countryDetails)
        {
            try
            {

                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoActionPrivilege;
                        return RedirectToAction("Country");
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowEdit)
                        {
                            ViewBag.Error = Constants.NoActionPrivilege;
                            return RedirectToAction("Country");
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                countryDetails.Name = countryDetails.Name.Trim();
                countryDetails.Currency = (countryDetails.Currency != null) ? countryDetails.Currency.Trim() : null;
                countryDetails.CurrencySymbol = (countryDetails.CurrencySymbol != null) ? countryDetails.CurrencySymbol.Trim() : null;

                api = "Admin/AddCountry";
                var serviceContent = JsonConvert.SerializeObject(countryDetails);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    TempData["Success"] = "Country settings added successfully.";
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    TempData["Failure"] = "You are not authorized to perform this action.";
                }
                else if (result == HttpStatusCode.Conflict)
                {
                    TempData["Failure"] = "Data already exist. Please enter different data.";
                }
                else
                {
                    TempData["Failure"] = "Updation Failed. Please try again later.";
                }

                #region created by silpa for ldap sign in

                System.DirectoryServices.Protocols.LdapConnection ldapConnection = new System.DirectoryServices.Protocols.LdapConnection("127.0.0.1:10389");

                var networkCredential = new NetworkCredential(
                      "employeeNumber=1,ou=Aluva,ou=Ernakulam,ou=kerala,ou=India,o=TopCompany",
                      "secret");

                ldapConnection.SessionOptions.ProtocolVersion = 3;

                ldapConnection.AuthType = AuthType.Basic;
                ldapConnection.Bind(networkCredential);


                var request = new AddRequest("ou=" + countryDetails.Name + ",o=TopCompany", new DirectoryAttribute[] {
                    new DirectoryAttribute("ou", countryDetails.Name),

                    new DirectoryAttribute("objectClass", new string[] { "top", "organizationalUnit"})
                });
                ldapConnection.SendRequest(request);




                #endregion

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-CreateCountry");
            }
            return RedirectToAction("Country");
        }

        /// <summary>
        /// Countries the list.
        /// </summary>
        /// <returns></returns>
        public ActionResult CountryList()
        {
            List<CountryViewModel> countryList = new List<CountryViewModel>();
            ServicesViewModel serviceModel = new ServicesViewModel();
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                api = "Admin/GetCountryDetails/0";
                var result = service.GetAPI(api);
                countryList = JsonConvert.DeserializeObject<List<CountryViewModel>>(result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-CountryList");
            }
            return PartialView("_CountryListPartial", countryList);
        }

        public ActionResult CreateWorkShift(WorkShiftViewModel workShiftDetails)
        {
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowEdit)
                        {
                            TempData["Error"] = Constants.NoActionPrivilege;
                            return RedirectToAction("Error");
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                workShiftDetails.ShiftName = workShiftDetails.ShiftName.Trim();
                api = "Admin/AddWorkShift";
                var serviceContent = JsonConvert.SerializeObject(workShiftDetails);
                HttpStatusCode result = service.PostAPI(serviceContent, api);

                if (result == HttpStatusCode.Conflict)
                {
                    TempData["Failure"] = "Data already exist. Please enter different data.";
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-CreateWorkShift");
            }
            return RedirectToAction("WorkShiftSettings");
        }

        public ActionResult WorkShiftSettings()
        {
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                WorkShiftViewModel workShiftDetails = new WorkShiftViewModel();
                return View(workShiftDetails);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-CreateWorkShift");
                return null;
            }
        }

        public ActionResult WorkShiftList()
        {
            List<WorkShiftViewModel> workShiftList = new List<WorkShiftViewModel>();
            ServicesViewModel serviceModel = new ServicesViewModel();
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                api = "Admin/GetWorkShiftDetails/0";
                var result = service.GetAPI(api);
                workShiftList = JsonConvert.DeserializeObject<List<WorkShiftViewModel>>(result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-WorkShiftList");
            }
            return PartialView("_WorkShiftPartial", workShiftList);
        }

        public JsonResult DeleteWorkShift(int shiftID)
        {
            try
            {
                string api = string.Empty;
                api = "Admin/DeleteWorkShift/" + shiftID;
                var serviceContent = JsonConvert.SerializeObject(shiftID);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    return Json("Workshift settings deleted successfully");
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    return Json("You are not authorized to perform this action.");
                }
                else if (result == HttpStatusCode.Ambiguous)
                {
                    return Json("Data cannot be deleted due to reference.");
                }
                else
                {
                    return Json("Updation Failed. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-DeleteWorkShift");
            }
            return Json(string.Empty);
        }
        [HttpPost]
        public ActionResult CreateTimeShift(TimeShiftViewModel timeShiftDetails)
        {
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        TempData["Error"] = Constants.NoViewPrivilege;
                        return RedirectToAction("Error");
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowEdit)
                        {
                            TempData["Error"] = Constants.NoActionPrivilege;
                            return RedirectToAction("Error");
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                timeShiftDetails.TimeShiftName = timeShiftDetails.TimeShiftName.Trim();
                api = "Admin/AddTimeShift";
                var serviceContent = JsonConvert.SerializeObject(timeShiftDetails);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    TempData["Success"] = "Timeshifts created Successfully";
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    TempData["Failure"] = "You are not authorized to perform this action.";
                }
                else if (result == HttpStatusCode.Conflict)
                {
                    TempData["Failure"] = "Data already exist. Please enter different data.";
                }
                else
                {
                    TempData["Failure"] = "Updation Failed. Please try again later.";
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-CreateTimeShift");

            }
            return RedirectToAction("TimeShiftSettings");
        }

        [HttpPost]
        public ActionResult Draft(TimeShiftViewModel objDraft)
        {
            try
            {
                ViewBag.Msg = "Details Saved as draft.";
                return View();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-Draft");
                return null;
            }
        }

        public ActionResult TimeShiftSettings()
        {
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                TimeShiftViewModel timeShiftDetails = new TimeShiftViewModel();

                string GetIntervalHours = "Admin/GetIntervalHours";
                var IntervalHours = service.GetAPI(GetIntervalHours);
                ViewBag.IntervalHours = IntervalHours.ToString();

                return View(timeShiftDetails);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-TimeShiftSettings");
                return null;
            }
        }

        public ActionResult TimeShiftList()
        {
            List<TimeShiftViewModel> timeShiftList = new List<TimeShiftViewModel>();
            ServicesViewModel serviceModel = new ServicesViewModel();
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                api = "Admin/GetTimeShiftDetails/0";
                var result = service.GetAPI(api);
                timeShiftList = JsonConvert.DeserializeObject<List<TimeShiftViewModel>>(result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-TimeShiftList");
            }
            return PartialView("_TimeShiftPartial", timeShiftList);
        }

        public JsonResult DeleteClientTimeShiftDetail(int TimeShiftId)
        {
            try
            {

                string api = string.Empty;

                api = "Admin/DeleteClientTimeShiftDetail/" + TimeShiftId;
                HttpStatusCode result = service.PostAPI(null, api);
                if (result == HttpStatusCode.OK)
                {
                    return Json("Timeshift settings deleted successfully");
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    return Json("You are not authorized to perform this action.");
                }
                else if (result == HttpStatusCode.Ambiguous)
                {
                    return Json("Data cannot be deleted due to reference.");
                }
                else
                {
                    return Json("Updation Failed. Please try again later.");
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-DeleteClientTimeShiftDetail");
                System.IO.File.WriteAllText(Server.MapPath("~/PDFError.txt"), ex.Message); throw;
            }

        }
        public ActionResult CreateHoliday(HolidayViewModel holidayDetails)
        {
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        TempData["Error"] = Constants.NoViewPrivilege;
                        return RedirectToAction("Error");
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowEdit)
                        {
                            TempData["Error"] = Constants.NoActionPrivilege;
                            return RedirectToAction("Error");
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                holidayDetails.HolidayName = holidayDetails.HolidayName.Trim();

                api = "Admin/AddHoliday";
                var serviceContent = JsonConvert.SerializeObject(holidayDetails);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    TempData["Success"] = "Holiday settings added successfully";
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    TempData["Failure"] = "You are not authorized to perform this action.";
                }
                else if (result == HttpStatusCode.Conflict)
                {
                    TempData["Failure"] = "Data already exist. Please enter different data.";
                }
                else
                {
                    TempData["Failure"] = "Updation Failed. Please try again later.";
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-CreateHoliday");
            }
            return RedirectToAction("HolidaySettings");
        }

        public ActionResult OverrideHoliday()
        {
            try
            {
                string api = string.Empty;
                api = "Admin/OverrideHoliday";
                //var serviceContent = JsonConvert.SerializeObject(holidayDetails);
                HttpStatusCode result = service.PostAPI("", api);
                if (result == HttpStatusCode.OK)
                {
                    TempData["Success"] = "Holiday settings added successfully";
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    TempData["Failure"] = "You are not authorized to perform this action.";
                }
                else if (result == HttpStatusCode.Conflict)
                {
                    TempData["Failure"] = "Data already exist. Please enter different data.";
                }
                else
                {
                    TempData["Failure"] = "Updation Failed. Please try again later.";
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-CreateHoliday");
            }
            return RedirectToAction("HolidaySettings");
        }

        public ActionResult HolidayPayList()
        {
            List<HolidayViewModel> holidayList = new List<HolidayViewModel>();
            ServicesViewModel serviceModel = new ServicesViewModel();
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                api = "Admin/GetHolidayPayDetails/0";
                var result = service.GetAPI(api);
                holidayList = JsonConvert.DeserializeObject<List<HolidayViewModel>>(result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-HolidayPayList");
            }
            return RedirectToAction("HolidaySettings");
        }

        public ActionResult CreateHolidayPay(HolidayViewModel holidayDetails)
        {
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        TempData["Error"] = Constants.NoViewPrivilege;
                        return RedirectToAction("Error");
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowEdit)
                        {
                            TempData["Error"] = Constants.NoActionPrivilege;
                            return RedirectToAction("Error");
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                api = "Admin/CreateHolidayPay";
                var serviceContent = JsonConvert.SerializeObject(holidayDetails);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    TempData["PaySuccess"] = "Holiday Pay settings added successfully";
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    TempData["PayFailure"] = "You are not authorized to perform this action.";
                }
                else
                {
                    TempData["PayFailure"] = "Updation Failed. Please try again later.";
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-CreateHolidayPay");
            }
            return RedirectToAction("HolidaySettings");
        }

        public ActionResult CreateIntervalHours(TimeShiftViewModel timeshiftmodel)
        {
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        TempData["Error"] = Constants.NoViewPrivilege;
                        return RedirectToAction("Error");
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowEdit)
                        {
                            TempData["Error"] = Constants.NoActionPrivilege;
                            return RedirectToAction("Error");
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                api = "Admin/CreateIntervalHours";
                var serviceContent = JsonConvert.SerializeObject(timeshiftmodel);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    TempData["Success"] = "Interval Hours settings added successfully";
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    TempData["Failure"] = "You are not authorized to perform this action.";
                }
                else
                {
                    TempData["Failure"] = "Updation Failed. Please try again later.";
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-CreateIntervalHours");
            }
            return RedirectToAction("TimeShiftSettings");
        }

        public ActionResult HolidaySettings()
        {
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }

                List<CountryViewModel> listCountry = new List<CountryViewModel>();
                api = "Admin/GetCountryDetails/0";
                var countryResult = service.GetAPI(api);
                listCountry = JsonConvert.DeserializeObject<List<CountryViewModel>>(countryResult);
                ViewData["CountryList"] = new SelectList(listCountry, "CountryId", "Name", 1);

                List<StateViewModel> stateList = new List<StateViewModel>();
                api = "/Admin/GetStatesByCountryId/1";
                var stateResult = service.GetAPI(api);
                stateList = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult);
                ViewData["StateList"] = new SelectList(stateList, "StateId", "Name", 0);


                HolidayViewModel holidayDetails = new HolidayViewModel();
                string holidayPayApi = "Admin/GetHolidayPayDetails";
                var holidayPayResult = service.GetAPI(holidayPayApi);
                ViewBag.PayTimes = holidayPayResult.ToString();

                holidayDetails.CountryId = 1;
                ViewData["listYears"] = GetYears();
                return View(holidayDetails);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-HolidaySettings");
                return null;
            }
        }
        [HttpPost]
        public ActionResult HolidayList(HolidayViewModel holidaySearchModel)
        {
            List<HolidayViewModel> holidayList = new List<HolidayViewModel>();
            try
            {
                ServicesViewModel serviceModel = new ServicesViewModel();
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                api = "Admin/GetHolidayDetails";
                var holidaySearch = JsonConvert.SerializeObject(holidaySearchModel);
                var result = service.PostAPIWithData(holidaySearch, api);
                holidayList = JsonConvert.DeserializeObject<List<HolidayViewModel>>(result.Result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-HolidayList");
            }
            return PartialView("_HolidayPartial", holidayList);
        }

        public JsonResult DeleteHoliday(int holidayID)
        {
            try
            {
                string api = string.Empty;
                api = "Admin/DeleteHoliday/" + holidayID;
                var serviceContent = JsonConvert.SerializeObject(holidayID);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    return Json("Holiday settings deleted successfully");
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    return Json("You are not authorized to perform this action.");
                }
                else if (result == HttpStatusCode.Ambiguous)
                {
                    return Json("Data cannot be deleted due to reference.");
                }
                else
                {
                    return Json("Updation Failed. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-DeleteHoliday");
            }
            return Json(string.Empty);
        }

        public ActionResult Services()
        {
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                ServicesViewModel serviceModel = new ServicesViewModel();
                return View(serviceModel);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-Services");
                return null;
            }
        }

        public ActionResult ServiceList()
        {
            List<ServicesViewModel> serviceList = new List<ServicesViewModel>();
            ServicesViewModel serviceModel = new ServicesViewModel();
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                api = "Admin/RetrieveServiceDetails/0";
                var result = service.GetAPI(api);
                serviceList = JsonConvert.DeserializeObject<List<ServicesViewModel>>(result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-ServiceList");
            }
            return PartialView("_ServiceListPartial", serviceList);
        }

        public ActionResult LoadServiceList()
        {
            try
            {
                string api = "Admin/RetrieveServiceDetails/0";
                var result = service.GetAPI(api);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Client Controller-LoadWorkShift");
                return null;
            }
        }
        public JsonResult GetTestimonialsById(int testimonialId)
        {
            try
            {
                string api = "Admin/GetTestimonials/" + testimonialId;
                var TestimonialResult = service.GetAPI(api);
                return Json(TestimonialResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-RetrieveServiceDetailsById");
                return null;
            }
        }

        public ActionResult States()
        {
            List<CountryViewModel> countryList = new List<CountryViewModel>();

            ServicesViewModel serviceModel = new ServicesViewModel();
            StateViewModel stateModel = new StateViewModel();
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                api = "Admin/GetCountryDetails/0";
                var result = service.GetAPI(api);
                countryList = JsonConvert.DeserializeObject<List<CountryViewModel>>(result);
                int defaultCountry = (countryList.Where(x => x.Isdefault == true).Count() > 0) ? countryList.Where(x => x.Isdefault == true).SingleOrDefault().CountryId : countryList.FirstOrDefault().CountryId;
                var _listCountry = new SelectList(countryList, "CountryId", "Name", defaultCountry);
                var listCountry = _listCountry;
                ViewData["listCountry"] = listCountry;
                stateModel.CountryId = defaultCountry;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-States");
            }
            return View(stateModel);
        }

        /// <summary>
        /// States the list.
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPost]
        public ActionResult StateList(int countryId)
        {
            List<StateViewModel> stateList = new List<StateViewModel>();
            ServicesViewModel serviceModel = new ServicesViewModel();
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                api = "Admin/GetStatesByCountryId/" + countryId;
                var result = service.GetAPI(api);
                stateList = JsonConvert.DeserializeObject<List<StateViewModel>>(result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-StateList");
            }
            return PartialView("_StatesListPartial", stateList);
        }

        public ActionResult CreateState(StateViewModel stateDetails)
        {
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return RedirectToAction("States");
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowEdit)
                        {
                            ViewBag.Error = Constants.NoActionPrivilege;
                            return RedirectToAction("States");
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                stateDetails.Name = stateDetails.Name.Trim();
                api = "Admin/AddState";
                var serviceContent = JsonConvert.SerializeObject(stateDetails);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    TempData["Success"] = "State settings added successfully.";
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    TempData["Failure"] = "You are not authorized to perform this action.";
                }
                else if (result == HttpStatusCode.Conflict)
                {
                    TempData["Failure"] = "Data already exist. Please enter different data.";
                }
                else
                {
                    TempData["Failure"] = "Updation Failed. Please try again later.";
                }


                #region created by silpa for ldap sign in

                System.DirectoryServices.Protocols.LdapConnection ldapConnection = new System.DirectoryServices.Protocols.LdapConnection("127.0.0.1:10389");

                var networkCredential = new NetworkCredential(
                      "employeeNumber=1,ou=Aluva,ou=Ernakulam,ou=kerala,ou=India,o=TopCompany",
                      "secret");

                ldapConnection.SessionOptions.ProtocolVersion = 3;

                ldapConnection.AuthType = AuthType.Basic;
                ldapConnection.Bind(networkCredential);


                var request = new AddRequest("ou=" + stateDetails.Name + ",ou="+stateDetails.CountryName+",o=TopCompany", new DirectoryAttribute[] {
                   
                    new DirectoryAttribute("ou", stateDetails.Name),
                    new DirectoryAttribute("objectClass", new string[] { "top", "organizationalUnit"})
                });
                ldapConnection.SendRequest(request);




                #endregion

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-CreateState");
            }
            return RedirectToAction("States");
        }

        public ActionResult City()
        {
            Cities cityModel = new Cities();
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                List<CountryViewModel> listCountry = new List<CountryViewModel>();
                string apiCountry = "Admin/GetCountryDetails/0";
                var countryResult = service.GetAPI(apiCountry);
                listCountry = JsonConvert.DeserializeObject<List<CountryViewModel>>(countryResult);
                int defaultCountry = (listCountry.Where(x => x.Isdefault == true).Count() > 0) ? listCountry.Where(x => x.Isdefault == true).SingleOrDefault().CountryId : listCountry.FirstOrDefault().CountryId;
                var _listCountry = new SelectList(listCountry, "CountryId", "Name", defaultCountry);
                ViewData["listCountry"] = _listCountry;

                List<StateViewModel> stateList = new List<StateViewModel>();
                api = "Admin/GetStatesByCountryId/" + defaultCountry;
                var stateResult = service.GetAPI(api);
                stateList = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult);
                int stateId = stateList.FirstOrDefault().StateId;
                var listState = new SelectList(stateList, "StateId", "Name", stateId);
                ViewData["listState"] = listState;
                cityModel.CountryId = defaultCountry;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-City");
            }
            return View(cityModel);
        }
        public ActionResult CreateCity(Cities City)
        {
            try
            {
                TempData["Success"] = "";
                City.UserId = Convert.ToInt32(Session["loggedInUserId"].ToString());
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        TempData["Failure"] = Constants.NoViewPrivilege;
                        return RedirectToAction("City");
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowEdit)
                        {
                            TempData["Failure"] = Constants.NoActionPrivilege;
                            return RedirectToAction("City");
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                api = "Admin/SaveCity";
                City.CityName = City.CityName.Trim();
                var serviceContent = JsonConvert.SerializeObject(City);
                HttpStatusCode result = service.PostAPI(serviceContent, api);

                if (result == HttpStatusCode.OK)
                {
                    TempData["Success"] = "City settings added successfully.";
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    TempData["Failure"] = "You are not authorized to perform this action.";
                }
                else if (result == HttpStatusCode.Conflict)
                {
                    TempData["Failure"] = "Data already exist. Please enter different data.";
                }
                else
                {
                    TempData["Failure"] = "Updation Failed. Please try again later.";
                }


                #region created by silpa for ldap sign in

                System.DirectoryServices.Protocols.LdapConnection ldapConnection = new System.DirectoryServices.Protocols.LdapConnection("127.0.0.1:10389");

                var networkCredential = new NetworkCredential(
                      "employeeNumber=1,ou=Aluva,ou=Ernakulam,ou=kerala,ou=India,o=TopCompany",
                      "secret");

                ldapConnection.SessionOptions.ProtocolVersion = 3;

                ldapConnection.AuthType = AuthType.Basic;
                ldapConnection.Bind(networkCredential);


                var request = new AddRequest("ou=" + City.CityName + ",ou=" + City.StateName + ",ou=" + City.CountryName + ",o=TopCompany", new DirectoryAttribute[] {

                    new DirectoryAttribute("ou", City.CityName),
                    new DirectoryAttribute("objectClass", new string[] { "top", "organizationalUnit"})
                });
                ldapConnection.SendRequest(request);




                #endregion


            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-CreateCity");
            }
            return RedirectToAction("City");
        }
        public ActionResult CityList(string flag = "*", string value = "")
        {
            List<Cities> cityList = new List<Cities>();
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return RedirectToAction("City");
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return RedirectToAction("City");
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                api = "Admin/GetCity?flag=" + flag + "&value=" + value;
                Cities cityModel = new Cities();
                var result = service.GetAPI(api);
                cityList = JsonConvert.DeserializeObject<List<Cities>>(result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-CityList");
            }
            return PartialView("_CityListPartial", cityList);
        }

        public JsonResult DeleteCity(int cityId)
        {
            try
            {
                string api = string.Empty;

                api = "Admin/DeleteCity/" + cityId;
                var serviceContent = JsonConvert.SerializeObject(cityId);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    return Json("City settings deleted successfully");
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    return Json("You are not authorized to perform this action.");
                }
                else if (result == HttpStatusCode.Ambiguous)
                {
                    return Json("Data cannot be deleted due to reference.");
                }
                else
                {
                    return Json("Updation Failed. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-DeleteCity");
            }
            return Json(string.Empty);
        }

        public JsonResult DeleteCountry(int CountryId)
        {
            try
            {
                string api = string.Empty;

                api = "Admin/DeleteCountry/" + CountryId;
                var serviceContent = JsonConvert.SerializeObject(CountryId);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    return Json("Country settings deleted successfully");

                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    return Json("You are not authorized to perform this action.");

                }
                else if (result == HttpStatusCode.Ambiguous)
                {
                    return Json("Data cannot be deleted due to reference.");

                }
                else
                {
                    return Json("Updation Failed. Please try again later.");

                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-DeleteCountry");
            }
            return Json(string.Empty);
        }

        public JsonResult DeleteService(int ServiceId)
        {
            try
            {
                string api = string.Empty;

                api = "Admin/DeleteService/" + ServiceId;
                var serviceContent = JsonConvert.SerializeObject(ServiceId);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    return Json("Service settings deleted successfully");
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    return Json("You are not authorized to perform this action.");
                }
                else if (result == HttpStatusCode.Ambiguous)
                {
                    return Json("Data cannot be deleted due to reference.");
                }
                else
                {
                    return Json("Updation Failed. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-DeleteService");
            }
            return Json(string.Empty);
        }

        public JsonResult DeleteState(int StateId)
        {
            try
            {
                string api = string.Empty;

                api = "Admin/DeleteState/" + StateId;
                var serviceContent = JsonConvert.SerializeObject(StateId);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    return Json("State settings deleted successfully");
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    return Json("You are not authorized to perform this action.");
                }
                else if (result == HttpStatusCode.Ambiguous)
                {
                    return Json("Data cannot be deleted due to reference.");
                }
                else
                {
                    return Json("Updation Failed. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-DeleteState");
            }
            return Json(string.Empty);
        }

        public JsonResult DeleteCategory(int CategoryId)
        {
            try
            {
                string api = string.Empty;

                api = "Admin/DeleteCategory/" + CategoryId;
                var serviceContent = JsonConvert.SerializeObject(CategoryId);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    return Json("Category settings deleted successfully");
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    return Json("You are not authorized to perform this action.");
                }
                else if (result == HttpStatusCode.Ambiguous)
                {
                    return Json("Data cannot be deleted due to reference.");
                }
                else
                {
                    return Json("Updation Failed. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-DeleteCategory");
            }
            return Json(string.Empty);
        }

        public ActionResult RejectedBookingDetails()
        {
            try
            {
                ClientModel clientModelObj = new ClientModel();
                string api = string.Empty;
                string country = Session["CountryId"].ToString();
                string state = Session["StateID"].ToString();
                string CityIdk = Session["CityIdk"].ToString();
                if (Session["UserType"] != null)
                {
                    if(country!=""&&state!=""&& CityIdk != "")
                    {
                        List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                        api = "Admin/GetCountryDetails/0";
                        var countrylist1 = service.GetAPI(api);
                        countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                        ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                        List<StateViewModel> stateList1 = new List<StateViewModel>();
                        api = "/Admin/GetStatesByCountryId/" + country;
                        var stateResult1 = service.GetAPI(api);
                        stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                        ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name",state);

                        api = "Admin/GetCity?flag=StateId&value=" + state;
                        List<Cities> cityList1 = new List<Cities>();
                        var resultCity1 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                        ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", CityIdk);


                        //api = "Admin/GetAllBranch/";
                        api = "Admin/GetBranch?flag=StateId&value=" + CityIdk;
                        List<Cities> branchList1 = new List<Cities>();
                        var resultBranch1 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultBranch1);
                        ViewData["ListBranch"] = new SelectList(cityList1, "branchId", "branchName", clientModelObj.BranchId2);
                    }
                    else
                    {
                        List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                        api = "Admin/GetCountryDetails/0";
                        var countrylist1 = service.GetAPI(api);
                        countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                        ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                        List<StateViewModel> stateList1 = new List<StateViewModel>();
                        api = "/Admin/GetStatesByCountryId/1";
                        var stateResult1 = service.GetAPI(api);
                        stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                        ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", clientModelObj.StateId1);

                        api = "Admin/GetCity?flag=StateId&value=" + clientModelObj.StateId1;
                        List<Cities> cityList1 = new List<Cities>();
                        var resultCity1 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                        ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", clientModelObj.CityId1);

                        api = "Admin/GetAllBranch/";
                        List<Cities> branchList1 = new List<Cities>();
                        var resultBranch1 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultBranch1);
                        ViewData["ListBranch"] = new SelectList(cityList1, "branchId", "branchName", clientModelObj.BranchId2);
                    }


                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 22;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                var inputs = new LocationSearchInputs();
                HomeController home = new HomeController();
                var loggedInUserId = (int)Session["loggedInUserId"];
                //get Work Role
                var userRoleDetails = home.GetUserRoleDetails(loggedInUserId);
                if (userRoleDetails != null)
                {

                    if (userRoleDetails.WorkRoleId == 1)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = null;
                        inputs.CityId = null;
                        ViewData["UserWorkRole"] = 1;
                    }
                    if (userRoleDetails.WorkRoleId == 2)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = userRoleDetails.StateId;
                        inputs.CityId = null;
                        ViewData["UserWorkRole"] = 2;
                    }
                    if (userRoleDetails.WorkRoleId == 3)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = userRoleDetails.StateId;
                        inputs.CityId = userRoleDetails.CityId;
                        ViewData["UserWorkRole"] = 3;
                    }

                    if (userRoleDetails.WorkRoleId == 4)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = userRoleDetails.StateId;
                        inputs.CityId = userRoleDetails.CityId;
                        ViewData["UserWorkRole"] = 4;
                    }
                }
                else
                {
                    inputs.CountryId = null;
                    inputs.StateId = null;
                    inputs.CityId = null;
                    ViewData["UserWorkRole"] = null;
                }
                string caretakerapi = "CareTaker/RetrieveCareTakerListForDdlByLocation/";
                var clientSearchInputsContent = JsonConvert.SerializeObject(inputs);
                var CareTakerResult = service.PostAPIWithData(clientSearchInputsContent,caretakerapi);
                List<Caretakers> listCaretakers = new List<Caretakers>();
                listCaretakers = JsonConvert.DeserializeObject<List<Caretakers>>(CareTakerResult.Result);
                ViewBag.Caretaker = new SelectList(listCaretakers, "CaretakerId", "CareTakerName", 0);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-RejectedBookingDetails");

            }
            return View();
        }
        [HttpPost]
        public ActionResult RejectedBookingDetails(BookingHistorySearch bookingHistorySearch)
        {
            List<BookingHistory> bookingHistoryList = new List<BookingHistory>();
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 22;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                api = "Admin/GetBookingHistoryList";

                var searchInputModel = JsonConvert.SerializeObject(bookingHistorySearch);
                var result = service.PostAPIWithData(searchInputModel, api);
                bookingHistoryList = JsonConvert.DeserializeObject<List<BookingHistory>>(result.Result);
                ViewBag.RejectedList = bookingHistoryList;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-RejectedBookingDetails");

            }
            return PartialView("_BookingRejectedCaretakerPartial", bookingHistoryList);
        }
        public ActionResult CareTakerList()
        {
            List<CareTakerRegistrationViewModel> caretakerList = new List<CareTakerRegistrationViewModel>();
            try
            {
                var inputs = new LocationSearchInputs();
                HomeController home = new HomeController();
                var loggedInUserId = (int)Session["loggedInUserId"];
                //get Work Role
                var userRoleDetails = home.GetUserRoleDetails(loggedInUserId);
                if (userRoleDetails != null)
                {

                    if (userRoleDetails.WorkRoleId == 1)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = null;
                        inputs.CityId = null;
                        ViewData["UserWorkRole"] = 1;
                    }
                    if (userRoleDetails.WorkRoleId == 2)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = userRoleDetails.StateId;
                        inputs.CityId = null;
                        ViewData["UserWorkRole"] = 2;
                    }
                    if (userRoleDetails.WorkRoleId == 3)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = userRoleDetails.StateId;
                        inputs.CityId = userRoleDetails.CityId;
                        ViewData["UserWorkRole"] = 3;
                    }
                }
                else
                {
                    inputs.CountryId = null;
                    inputs.StateId = null;
                    inputs.CityId = null;
                    ViewData["UserWorkRole"] = null;
                }
                var clientSearchInputsContent = JsonConvert.SerializeObject(inputs);
                string manageCaretaker = "Admin/SelectCaretakersByLocation/" + (int)CaretakerStatus.Applied;
                var result = service.PostAPIWithData(clientSearchInputsContent, manageCaretaker);
                caretakerList = JsonConvert.DeserializeObject<List<CareTakerRegistrationViewModel>>(result.Result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-ManagePublicUser");
            }
            return PartialView("_CaretakerListPartial", caretakerList);
        }

        [HttpPost]
        public ActionResult CareTakerListByLocation(LocationSearchInputs inputs)
        {
            List<CareTakerRegistrationViewModel> caretakerList = new List<CareTakerRegistrationViewModel>();
            try
            {
                if (inputs == null)
                {
                    inputs = new LocationSearchInputs();
                    inputs.CountryId = null;
                    inputs.StateId = null;
                    inputs.CityId = null;
                }
                if (inputs.CountryId == 0)
                {
                    inputs.CountryId = null;
                }
                if (inputs.StateId == 0)
                {
                    inputs.StateId = null;
                }
                if (inputs.CityId == 0)
                {
                    inputs.CityId = null;
                }
                var clientSearchInputsContent = JsonConvert.SerializeObject(inputs);
                string manageCaretaker = "Admin/SelectCaretakersByLocation/" + (int)CaretakerStatus.Applied;
                var result = service.PostAPIWithData(clientSearchInputsContent, manageCaretaker);
                caretakerList = JsonConvert.DeserializeObject<List<CareTakerRegistrationViewModel>>(result.Result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-ManagePublicUser");
            }
            return PartialView("_CaretakerListPartial", caretakerList);
        }

        public ActionResult UserList()
        {
            List<UsersDetails> userList = new List<UsersDetails>();
            try
            {
                string api = "PublicUser/GetAllUsersDetails?flag=UserType&value=2";
                var result = service.GetAPI(api);
                userList = JsonConvert.DeserializeObject<List<UsersDetails>>(result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-ManagePublicUser");
            }
            return PartialView("_UserListPartial", userList);
        }

        public ActionResult ClientList()
        {
            List<ClientModel> clients = new List<ClientModel>();
            try
            {
                string api = "Client/GetAllClientDetails";
                var result = service.GetAPI(api);
                clients = JsonConvert.DeserializeObject<List<ClientModel>>(result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-ManagePublicUser");
            }
            return PartialView("_ClientListPartial", clients);
        }

        public ActionResult AdminPieChart()
        {
            try
            {
                return PartialView("_AdminPieChartPartial");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-AdminPieChart");
                return null;
            }
        }
        public ActionResult AdminCalendar()
        {
            try
            {
                return PartialView("_AdminCalendarPartial");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-AdminCalendar");
                return null;
            }
        }
        public ActionResult AdminBarChart()
        {
            try
            {
                return PartialView("_AdminBarChartPartial");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-AdminBarChart");
                return null;
            }
        }

        /// <summary>
        /// Admins the notification.
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminNotification()
        {
            try
            {
                return PartialView("_AdminNotificationPartial");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-AdminNotification");
                return null;
            }
        }

        /// <summary>
        /// To List invoice monthly/Yearly
        /// </summary>
        /// <returns></returns>
        public ActionResult Invoice()
        {
            var listPaySearch = new SelectList(new[]
             {
                new { ID = "0", Name = "--Select--" },
                new { ID = "1", Name = "Yearly" },
                new { ID = "2", Name = "Monthly" },
                new { ID = "3", Name = "Date Range" },
            },
             "ID", "Name", 0);

            ViewData["listPaySearch"] = listPaySearch;
            ViewData["listYears"] = GetYears();
            Months months = new Months();
            ViewData["listMonths"] = months.GetMonths();

            List<InvoiceViewModel> _InvoiceList = new List<InvoiceViewModel>();
            InvoiceViewModel _InvoiceModel = new InvoiceViewModel();
            _InvoiceModel.CaretakerName = "David Brown";
            _InvoiceModel.InvoiceDateString = "06-15-2018";
            _InvoiceModel.CaretakerCategory = "Home nurse";
            _InvoiceModel.Shift = "Floor Shift";
            _InvoiceModel.TimeIn = "7:00 AM";
            _InvoiceModel.Timeout = "8:00 PM";
            _InvoiceModel.Hours = 12.5F;
            _InvoiceModel.Amount = 100;
            _InvoiceModel.Rate = 8;
            _InvoiceList.Add(_InvoiceModel);

            _InvoiceModel = new InvoiceViewModel();
            _InvoiceModel.CaretakerName = "Jessica clark";
            _InvoiceModel.InvoiceDateString = "05-19-2018";
            _InvoiceModel.CaretakerCategory = "Physiotherapy";
            _InvoiceModel.Shift = "Individual Shift";
            _InvoiceModel.TimeIn = "11:00 AM";
            _InvoiceModel.Timeout = "8:00 PM";
            _InvoiceModel.Hours = 8.5F;
            _InvoiceModel.Amount = 51;
            _InvoiceModel.Rate = 6;
            _InvoiceList.Add(_InvoiceModel);

            _InvoiceModel = new InvoiceViewModel();
            _InvoiceModel.CaretakerName = "Pepitha John";
            _InvoiceModel.InvoiceDateString = "05-05-2018";
            _InvoiceModel.CaretakerCategory = "Child Care";
            _InvoiceModel.Shift = "individual Shift";
            _InvoiceModel.TimeIn = "9:00 AM";
            _InvoiceModel.Timeout = "6:00 PM";
            _InvoiceModel.Hours = 8.5F;
            _InvoiceModel.Amount = 85;
            _InvoiceModel.Rate = 10;
            _InvoiceList.Add(_InvoiceModel);

            _InvoiceModel = new InvoiceViewModel();
            _InvoiceModel.CaretakerName = "Lara Smith";
            _InvoiceModel.InvoiceDateString = "06-20-2018";
            _InvoiceModel.CaretakerCategory = "Pediatric";
            _InvoiceModel.Shift = "Floor Shift";
            _InvoiceModel.TimeIn = "7:00 AM";
            _InvoiceModel.Timeout = "5:00 PM";
            _InvoiceModel.Hours = 9.5F;
            _InvoiceModel.Amount = 85.5F;
            _InvoiceModel.Rate = 9;
            _InvoiceList.Add(_InvoiceModel);

            _InvoiceModel = new InvoiceViewModel();
            _InvoiceModel.CaretakerName = "Hussain Bolt";
            _InvoiceModel.InvoiceDateString = "04-15-2018";
            _InvoiceModel.CaretakerCategory = "Home nurse";
            _InvoiceModel.Shift = "Work Shift";
            _InvoiceModel.TimeIn = "7:00 AM";
            _InvoiceModel.Timeout = "8:00 PM";
            _InvoiceModel.Hours = 12.5F;
            _InvoiceModel.Amount = 87.5F;
            _InvoiceModel.Rate = 7;
            _InvoiceList.Add(_InvoiceModel);

            _InvoiceModel = new InvoiceViewModel();
            _InvoiceModel.CaretakerName = "Dany Brown";
            _InvoiceModel.InvoiceDateString = "06-15-2018";
            _InvoiceModel.CaretakerCategory = "Home nurse";
            _InvoiceModel.Shift = "Floor Shift";
            _InvoiceModel.TimeIn = "10:00 AM";
            _InvoiceModel.Timeout = "5:00 PM";
            _InvoiceModel.Hours = 6.5F;
            _InvoiceModel.Amount = 52F;
            _InvoiceModel.Rate = 8;
            _InvoiceList.Add(_InvoiceModel);


            _InvoiceModel = new InvoiceViewModel();
            _InvoiceModel.CaretakerName = "Jennifer Wagon";
            _InvoiceModel.InvoiceDateString = "06-15-2018";
            _InvoiceModel.CaretakerCategory = "Home nurse";
            _InvoiceModel.Shift = "Floor Shift";
            _InvoiceModel.TimeIn = "7:00 AM";
            _InvoiceModel.Timeout = "11:00 AM";
            _InvoiceModel.Hours = 4;
            _InvoiceModel.Amount = 32;
            _InvoiceModel.Rate = 8;
            _InvoiceList.Add(_InvoiceModel);


            return View(_InvoiceList);
        }



        public ActionResult ClientSettings()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-ClientSettings");
                return null;
            }
        }
        public ActionResult LoadStatesbyId(int StateId)
        {
            List<StateViewModel> stateList = new List<StateViewModel>();
            try
            {
                string api = "Admin/GetStateDetails/" + StateId;
                var result = service.GetAPI(api);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-LoadStatesbyId");
                return null;
            }
        }


        public ActionResult LoadStatesByCountry(int CountryId)
        {
            string api = string.Empty;

            api = "Admin/GetStatesByCountryId/" + CountryId;
            List<StateViewModel> cityList1 = new List<StateViewModel>();
            var resultCity1 = service.GetAPI(api);
            cityList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(resultCity1);
            
            return Json(cityList1);


        }


        public ActionResult LoadStatesByCountryId(int CountryId)
        {
            List<StateViewModel> stateList = new List<StateViewModel>();
            try
            {
                string api = string.Empty;
                api = "Admin/GetStatesByCountryId/" + CountryId;
                var resultState = service.GetAPI(api);
                return Json(resultState, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-LoadStatesByCountryId");
                return null;
            }
        }
        public ActionResult GetCaretakerType()
        {
            List<CaretakerType> ctType = new List<CaretakerType>();
            try
            {
                string api = string.Empty;
                api = "Admin/GetCaretakerType";
                var result = service.GetAPI(api);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-GetCaretakerType");
                return null;
            }
        }
        public ActionResult LoadPhoneCodeByCountryId(int CountryId)
        {
            try
            {
                string api = string.Empty;
                api = "Admin/LoadPhoneCodeByCountryId/" + CountryId;
                if (CountryId > 0)
                {
                    var resultState = service.GetAPI(api);
                    return Json(resultState, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-LoadStatesByCountryId");
                return null;
            }
        }
        public ActionResult LoadCitiesbyId(int StateId)
        {
            try
            {
                string api = "Admin/GetCity?flag=StateId&value=" + StateId;
                List<Cities> cityList = new List<Cities>();
                Cities cityModel = new Cities();
                var result = service.GetAPI(api);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-LoadCitiesbyId");
                return null;
            }
        }

        public ActionResult LoadCitiesbyStateId(int StateId)
        {
            try
            {
                string api = "Admin/GetCityByStateId/" + StateId;
                List<Cities> cityList = new List<Cities>();
                Cities cityModel = new Cities();
                var result = service.GetAPI(api);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-LoadCitiesbyStateId");
                return null;
            }
        }

        public ActionResult ManagePublicUser()
        {
            List<UsersDetails> users = new List<UsersDetails>();
            ClientModel clientModelObj = new ClientModel();
            try
            {
                string api = string.Empty;
                string country = Session["CountryId"].ToString();
                string state = Session["StateID"].ToString();
                string CityIdk = Session["CityIdk"].ToString();
                if (Session["UserType"] != null)
                {
                    if(country!=""&&state!=""&&CityIdk!="")
                    {
                        List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                        api = "Admin/GetCountryDetails/0";
                        var countrylist1 = service.GetAPI(api);
                        countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                        ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                        List<StateViewModel> stateList1 = new List<StateViewModel>();
                        api = "/Admin/GetStatesByCountryId/" + country;
                        var stateResult1 = service.GetAPI(api);
                        stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                        ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", state);

                        api = "Admin/GetCity?flag=StateId&value=" + state;
                        List<Cities> cityList1 = new List<Cities>();
                        var resultCity1 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                        ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", CityIdk);

                        // api = "Admin/GetAllBranch/";
                        api = "Admin/GetBranch?flag=StateId&value=" + CityIdk;
                        List<Cities> branchList1 = new List<Cities>();
                        var resultBranch1 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultBranch1);
                        ViewData["ListBranch"] = new SelectList(cityList1, "branchId", "branchName", clientModelObj.BranchId2);

                    }
                    else
                    {
                        List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                        api = "Admin/GetCountryDetails/0";
                        var countrylist1 = service.GetAPI(api);
                        countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                        ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                        List<StateViewModel> stateList1 = new List<StateViewModel>();
                        api = "/Admin/GetStatesByCountryId/1";
                        var stateResult1 = service.GetAPI(api);
                        stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                        ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", clientModelObj.StateId1);

                        api = "Admin/GetCity?flag=StateId&value=" + clientModelObj.StateId1;
                        List<Cities> cityList1 = new List<Cities>();
                        var resultCity1 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                        ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", clientModelObj.CityId1);

                        api = "Admin/GetAllBranch/";
                        List<Cities> branchList1 = new List<Cities>();
                        var resultBranch1 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultBranch1);
                        ViewData["ListBranch"] = new SelectList(cityList1, "branchId", "branchName", clientModelObj.BranchId2);

                    }

                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 18;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                LocationSearchInputs inputs = new LocationSearchInputs();
                HomeController home = new HomeController();
                var loggedInUserId = (int)Session["loggedInUserId"];
                //get Work Role
                var userRoleDetails = home.GetUserRoleDetails(loggedInUserId);
                if (userRoleDetails != null)
                {

                    if (userRoleDetails.WorkRoleId == 1)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = null;
                        inputs.CityId = null;
                        ViewData["UserWorkRole"] = 1;
                    }
                    if (userRoleDetails.WorkRoleId == 2)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = userRoleDetails.StateId;
                        inputs.CityId = null;
                        ViewData["UserWorkRole"] = 2;
                    }
                    if (userRoleDetails.WorkRoleId == 3)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = userRoleDetails.StateId;
                        inputs.CityId = userRoleDetails.CityId;
                        ViewData["UserWorkRole"] = 3;
                    }

                    if (userRoleDetails.WorkRoleId == 4)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = userRoleDetails.StateId;
                        inputs.CityId = userRoleDetails.CityId;
                        ViewData["UserWorkRole"] = 4;
                    }
                }
                else
                {
                    inputs.CountryId = null;
                    inputs.StateId = null;
                    inputs.CityId = null;
                    ViewData["UserWorkRole"] = null;
                }
                api = "PublicUser/GetAllUsersDetailsByLocation?flag=UserType&value=2";
                var clientSearchInputsContent = JsonConvert.SerializeObject(inputs);
                var result = service.PostAPIWithData(clientSearchInputsContent, api);
                users = JsonConvert.DeserializeObject<List<UsersDetails>>(result.Result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-ManagePublicUser");
            }
            return View(users);
        }

        public ActionResult ManagePublicUserByLocation(LocationSearchInputs inputs)
        {
            List<UsersDetails> users = new List<UsersDetails>();
            ClientModel clientModelObj = new ClientModel();
            try
            {
                if (inputs == null)
                {
                    inputs = new LocationSearchInputs();
                    inputs.CountryId = null;
                    inputs.StateId = null;
                    inputs.CityId = null;
                }
                if (inputs.CountryId == 0)
                {
                    inputs.CountryId = null;
                }
                if (inputs.StateId == 0)
                {
                    inputs.StateId = null;
                }
                if (inputs.CityId == 0)
                {
                    inputs.CityId = null;
                }
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                    api = "Admin/GetCountryDetails/0";
                    var countrylist1 = service.GetAPI(api);
                    countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                    ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                    List<StateViewModel> stateList1 = new List<StateViewModel>();
                    api = "/Admin/GetStatesByCountryId/1";
                    var stateResult1 = service.GetAPI(api);
                    stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                    ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", clientModelObj.StateId1);

                    api = "Admin/GetCity?flag=StateId&value=" + clientModelObj.StateId1;
                    List<Cities> cityList1 = new List<Cities>();
                    var resultCity1 = service.GetAPI(api);
                    cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                    ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", clientModelObj.CityId1);

                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 18;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                api = "PublicUser/GetAllUsersDetailsByLocation?flag=UserType&value=2";
                var clientSearchInputsContent = JsonConvert.SerializeObject(inputs);
                var result = service.PostAPIWithData(clientSearchInputsContent, api);
                users = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UsersDetails>>(result.Result);

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-ManagePublicUser");
            }
            return PartialView("_PublicUserManagePartial", users);
        }

        //[HttpPost]
        //public JsonResult FilterPublicUser(string filterValue)
        //{
        //    List<UsersDetails> users = new List<UsersDetails>();
        //    try
        //    {
        //        string api = string.Empty;

        //        api = "PublicUser/GetAllUsersDetails?flag=UserType&value=2";

        //        var result = service.GetAPI(api);
        //        users = JsonConvert.DeserializeObject<List<UsersDetails>>(result);
        //        if (filterValue == "active")
        //        {

        //            users = users.Where(x => x.UserStatus == UserStatus.Active).ToList();
        //        }
        //        else if (filterValue == "inactive")
        //        {

        //            users = users.Where(x => x.UserStatus == UserStatus.InActive).ToList();
        //        }
        //        else if (filterValue == "unverified")
        //        {

        //            users = users.Where(x => x.UserVerified == false).ToList();
        //        }
        //        else
        //        {
        //            users = JsonConvert.DeserializeObject<List<UsersDetails>>(result);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error(ex, "Error occurred in Admin Controller-ManagePublicUser");
        //    }
        //    return Json(users);
        //}
        public JsonResult ChangeStatus(int userId, int status)
        {
            Service service = new Service();
            string api = "Admin/ChangeUserStatus/" + userId + "/" + status;
            HttpStatusCode result = service.PostAPI(null, api);
            if (result == HttpStatusCode.OK && status == 3)
            {
                TempData["Success"] = "Caregiver Details Deleted Successfully";
            }
            else if (result == HttpStatusCode.OK && status != 3)
            {
                TempData["Success"] = "Status Changed Successfully";
            }
            else if (result == HttpStatusCode.Unauthorized)
            {
                TempData["Failure"] = "You are not authorized to perform this action.";
            }
            else
            {
                TempData["Failure"] = "Updation Failed. Please try again later.";
            }
            return Json(string.Empty);
        }

        public JsonResult VerifyEmail(int userId)
        {
            Service service = new Service();
            string api = "Admin/VerifyEmail/" + userId;
            HttpStatusCode result = service.PostAPI(null, api);
            if (result == HttpStatusCode.OK)
            {
                TempData["Success"] = "Caregiver Email Verified Successfully";
            }
            else if (result == HttpStatusCode.OK)
            {
                TempData["Success"] = "Status Changed Successfully";
            }
            else if (result == HttpStatusCode.Unauthorized)
            {
                TempData["Failure"] = "You are not authorized to perform this action.";
            }
            else
            {
                TempData["Failure"] = "Updation Failed. Please try again later.";
            }
            return Json(string.Empty);
        }

        [HttpPost]
        public JsonResult SendClentEmail(string clientname, string email, int fileId, int clientid)
        {
            try
            {
                Service service = new Service();
                string api = "Client/GetInvoiceHistoryById/" + fileId;
                var result = service.GetAPI(api);
                InvoiceSearchInpts scheduleDetailsListFilterd = new InvoiceSearchInpts();
                scheduleDetailsListFilterd = JsonConvert.DeserializeObject<List<InvoiceSearchInpts>>(result).ToList().FirstOrDefault();
                if (scheduleDetailsListFilterd != null)
                {
                    //byte[] bytes = scheduleDetailsListFilterd.PdfFile;
                    ////string filenname = Server.MapPath("~/Temp/") + "tdt.pdf";
                    //string filenname = Server.MapPath("~PCMS/Invoice/Client") + "Invoice For " + scheduleDetailsListFilterd.ClientName + "_" + scheduleDetailsListFilterd.InvoicePrefix + ".pdf";
                    //if (!Directory.Exists(Server.MapPath("~/PCMS/Invoice/Client/")))
                    //{
                    //    Directory.CreateDirectory(Server.MapPath("~/PCMS/Invoice/Client/"));
                    //}
                    //System.IO.File.WriteAllBytes(filenname, bytes);
                    ////string siteUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/";
                    //filenname = siteUrl + "/PCMS/Invoice/Client/" + "Invoice For " + scheduleDetailsListFilterd.ClientName + "_" + scheduleDetailsListFilterd.InvoicePrefix + ".pdf";
                    string filenname = scheduleDetailsListFilterd.PdfFilePath;

                    EmailInput emailinputs = new EmailInput()
                    {
                        EmailType = EmailType.Invoice,
                        Attachments = filenname,
                        Body = GetEmailBody(clientname),
                        Subject = "Invoice ",
                        EmailId = email,
                        UserId = clientid
                    };
                    api = "Admin/SendEmailNotification";
                    var serviceContent = JsonConvert.SerializeObject(emailinputs);
                    var results = service.PostAPIWithData(serviceContent, api);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-SendClientEmail");
            }
            return Json(string.Empty);
        }

        private string GetEmailBody(string clientname)
        {
            try
            {
                string WelcomeMsg = "Client Invoice.";
                string MailMsg = "Invoice Details.</p><p> Thank you for choosing to work with us.</p><p>We are attaching the copy of the invoice.</p> ";
                string Mailcontent = "";
                string ContactNo = "1-800-892-6066";
                string RegardsBy = "Tranquil Care Inc.";
                string siteUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/";
                string CompanyName_TagLine = "Tranquil Care - Delivering Care Excellence.";
                string CompanyName = "Tranquil Care Inc.";
                string body = "";
                string sd = AppDomain.CurrentDomain.BaseDirectory + "EmailTemplates/EmailCommon.html";
                var sr = new StreamReader(sd);
                body = sr.ReadToEnd();
                body = string.Format(body, WelcomeMsg, clientname, MailMsg, Mailcontent, ContactNo, RegardsBy, siteUrl, CompanyName_TagLine, CompanyName);
                return body;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public JsonResult RejectCaretakerApplication(RejectCareTaker rejectCareTaker)
        {
            try
            {
                List<UsersDetails> users = new List<UsersDetails>();
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return Json(string.Empty);
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 13;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return Json(string.Empty);
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return Json("LoginFailed");
                    //return RedirectToAction("Login", "Account");
                }
                rejectCareTaker.SiteURL = Request.Url.Scheme + "://" + Request.Url.Authority + "/";
                string siteUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/";
                api = "Admin/RejectCaretakerApplication";
                var rejectCT = JsonConvert.SerializeObject(rejectCareTaker);
                HttpStatusCode result = service.PostAPI(rejectCT, api);
                if (result == HttpStatusCode.OK)
                {
                    TempData["ApproveMessage"] = "Caregiver rejected Successfully";
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    TempData["ApproveMessage"] = "You are not authorized to perform this action.";
                }
                else
                {
                    TempData["ApproveMessage"] = "Caregiver updation Failed.";
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-RejectCaretakerApplication");
            }
            return Json(string.Empty);
        }

        public JsonResult DeleteCaretaker(int userId)
        {
            List<UsersDetails> users = new List<UsersDetails>();
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return Json(string.Empty);
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 13;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowDelete)
                        {
                            ViewBag.Error = Constants.NoActionPrivilege;
                            return Json(string.Empty);
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return Json("LoginFailed");
                    //return RedirectToAction("Login", "Account");
                }
                api = "Admin/DeleteCaretaker/" + userId;
                HttpStatusCode result = service.PostAPI(null, api);
                if (result == HttpStatusCode.OK)
                {
                    TempData["Success"] = "User deleted Successfully";
                }
                else
                {
                    TempData["Failure"] = "Deletion Failed. Please try again later.";
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-DeleteCaretaker");
            }
            return Json(string.Empty);
        }

        public ActionResult Roles()
        {
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                Roles roles = new Roles();
                return View(roles);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-Roles");
                return null;
            }
        }

        public ActionResult AddRoles(Roles roles)
        {
            try
            {
                TempData["Success"] = "";
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        TempData["Failure"] = Constants.NoViewPrivilege;
                        return RedirectToAction("Roles");
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            TempData["Failure"] = Constants.NoViewPrivilege;
                            return RedirectToAction("Roles");
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                roles.RoleName = roles.RoleName.Trim();
                api = "Admin/SaveRoles";
                var serviceContent = JsonConvert.SerializeObject(roles);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {

                    #region created by silpa for ldap sign in

                    System.DirectoryServices.Protocols.LdapConnection ldapConnection = new System.DirectoryServices.Protocols.LdapConnection("127.0.0.1:10389");

                    var networkCredential = new NetworkCredential(
                          "employeeNumber=1,ou=Aluva,ou=Ernakulam,ou=kerala,ou=India,o=TopCompany",
                          "secret");

                    ldapConnection.SessionOptions.ProtocolVersion = 3;

                    ldapConnection.AuthType = AuthType.Basic;
                    ldapConnection.Bind(networkCredential);


                    var request = new AddRequest("ou=" + roles.RoleName + ",o=TopCompany", new DirectoryAttribute[] {
                    new DirectoryAttribute("cn", roles.RoleName),
                    new DirectoryAttribute("ou", roles.RoleName),

                    new DirectoryAttribute("objectClass", new string[] { "top", "organizationalRole"})
                });
                    ldapConnection.SendRequest(request);




                    #endregion


                    TempData["Success"] = "Roles Saved Successfully";
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    TempData["Failure"] = "You are not authorized to perform this action.";
                }
                else if (result == HttpStatusCode.Conflict)
                {
                    TempData["Failure"] = "Data already exist. Please enter different data.";
                }
                else
                {
                    TempData["Failure"] = "Updation failed. Please try again later.";
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-Roles");
            }
            return RedirectToAction("Roles");
        }

        public ActionResult RoleList()
        {
            List<Roles> roleList = new List<Roles>();
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        TempData["Failure"] = Constants.NoViewPrivilege;
                        return RedirectToAction("Roles");
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            TempData["Failure"] = Constants.NoViewPrivilege;
                            return RedirectToAction("Roles");
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                api = "Admin/GetRoles/0";
                var result = service.GetAPI(api);
                roleList = JsonConvert.DeserializeObject<List<Roles>>(result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-RoleList");
            }
            return PartialView("_RoleListPartial", roleList);
        }
        public JsonResult DeleteRoles(int roleId)
        {
            try
            {
                string api = string.Empty;

                api = "Admin/DeleteRoles/" + roleId;
                var serviceContent = JsonConvert.SerializeObject(roleId);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    return Json("Role settings deleted successfully");
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    return Json("You are not authorized to perform this action.");
                }
                else if (result == HttpStatusCode.Ambiguous)
                {
                    return Json("Data cannot be deleted due to reference.");
                }
                else
                {
                    return Json("Updation Failed. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-DeleteRoles");
            }
            return Json(string.Empty);
        }

        public ActionResult Notification()
        {
            NotificationHub.Static_Send("notify", "controller", "static");
            //var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            //hubContext.Clients.All.displayNotification("notify", "controller", "static");
            return View();
        }

        public ActionResult ApproveNewCaretaker(int caretakerId)
        {
            try
            {
                ViewBag.Message = "Caregiver profile page.";

                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        TempData["Failure"] = Constants.NoViewPrivilege;
                        return RedirectToAction("Roles");
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 15;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            TempData["Failure"] = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                var listGender = getGenders();
                ViewData["GenderList"] = new SelectList(listGender, "GenderId", "Gender", 0);

                List<ServicesViewModel> serviceList = new List<ServicesViewModel>();
                api = "Admin/RetrieveServiceDetails/0";
                var resultService = service.GetAPI(api);
                serviceList = JsonConvert.DeserializeObject<List<ServicesViewModel>>(resultService);
                var _servicesList = new SelectList(serviceList, "ServiceId", "Name", 0);
                ViewData["ServiceList"] = _servicesList;

                List<CountryViewModel> countryList = new List<CountryViewModel>();
                api = "Admin/GetCountryDetails/0";
                var resultCountry = service.GetAPI(api);
                countryList = JsonConvert.DeserializeObject<List<CountryViewModel>>(resultCountry);
                var _countryList = new SelectList(countryList, "CountryId", "Name", 0);
                ViewData["CountryList"] = _countryList;

                List<CategoryViewModel> lstCategory = new List<CategoryViewModel>();
                api = "Admin/GetCategory?flag=*&value=''";

                List<CategoryViewModel> categoryList = new List<CategoryViewModel>();
                CategoryViewModel categoryModel = new CategoryViewModel();
                var resultCategory = service.GetAPI(api);
                categoryList = JsonConvert.DeserializeObject<List<CategoryViewModel>>(resultCategory);


                var listCategory = new SelectList(categoryList, "CategoryId", "Name", 0);
                ViewData["ListCategory"] = listCategory;
                api = "Admin/GetQualification/0";
                List<QualificationViewModel> qualificationList = new List<QualificationViewModel>();
                QualificationViewModel qlfyVM = new QualificationViewModel();
                qlfyVM.QualificationId = 999;
                qlfyVM.Qualification = "Others";
                var resultQual = service.GetAPI(api);
                qualificationList = JsonConvert.DeserializeObject<List<QualificationViewModel>>(resultQual);
                qualificationList.Add(qlfyVM);
                var listQualification = new SelectList(qualificationList, "QualificationId", "Qualification", 0);

                ViewData["ListQualification"] = listQualification;


                CareTakerRegistrationViewModel caretakerProfile = null;

                api = "CareTaker/GetCareTakerProfile/" + caretakerId;
                var result = service.GetAPI(api);
                if (result == "Unauthorized")
                {
                    ViewBag.Unauthorized = "You are not authorized to view this page.";
                    return View();
                }
                caretakerProfile = Newtonsoft.Json.JsonConvert.DeserializeObject<CareTakerRegistrationViewModel>(result);

                if (caretakerProfile != null)
                {
                    UserSessionManager.SetCareTakerExperience(this, caretakerProfile.CareTakerExperiences);
                    UserSessionManager.SetCareTakerQualification(this, caretakerProfile.CareTakerQualifications);
                    UserSessionManager.SetCareTakerService(this, caretakerProfile.CareTakerServices);
                    UserSessionManager.SetCareTakerClient(this, caretakerProfile.CareTakerClients);

                    List<StateViewModel> stateList = new List<StateViewModel>();
                    api = "Admin/GetStatesByCountryId/" + caretakerProfile.CountryId;
                    var resultState = service.GetAPI(api);
                    stateList = JsonConvert.DeserializeObject<List<StateViewModel>>(resultState);
                    var _stateList = new SelectList(stateList, "StateId", "Name", 0);
                    ViewData["StateList"] = _stateList;
                    caretakerProfile.SiteURL = Request.Url.Scheme + "://" + Request.Url.Authority + "/";
                    if (caretakerProfile.ConsentForm != null)
                    {
                        //var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                        //serializer.MaxJsonLength = Int32.MaxValue;
                        //var jsonModel = serializer.Serialize(Convert.ToBase64String(caretakerProfile.ConsentForm));
                        //caretakerProfile.ConcentFormData = jsonModel;
                    }
                    else
                    {
                        caretakerProfile.ConcentFormData = String.Empty;
                    }
                    //List<Cities> cityList = new List<Cities>();
                    string apiCity = "City/GetCityByStateId/" + caretakerProfile.StateId;
                    List<Cities> cityList = new List<Cities>();
                    Cities cityModel = new Cities();
                    result = service.GetAPI(apiCity);
                    cityList = JsonConvert.DeserializeObject<List<Cities>>(result);
                    var listCity = new SelectList(cityList, "CityId", "CityName", 0);
                    ViewData["ListCity"] = listCity;
                    caretakerProfile.Gender = Enum.GetName(typeof(Gender), caretakerProfile.GenderId);
                }
                return View(caretakerProfile);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-ApproveNewCaretaker");
                return null;
            }
        }
        private IEnumerable<GenderViewModel> getGenders()
        {
            try
            {
                var listGender = Enum.GetValues(typeof(Gender))
                   .Cast<Gender>()
                   .Select(t => new GenderViewModel
                   {
                       GenderId = ((int)t),
                       Gender = t.ToString()
                   });
                return listGender;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-getGenders");
                return null;
            }
        }

        public JsonResult NewCaretakerApprove(int careTakerId, bool isPrivate, List<CareTakerServices> services)
        {
            try
            {

                ApproveCaretaker careTakerModel = new ApproveCaretaker();
                careTakerModel.CareTakerId = careTakerId;
                careTakerModel.IsPrivate = isPrivate;
                careTakerModel.ApprovedServiceRates = services;
                careTakerModel.SiteURL = Request.Url.Scheme + "://" + Request.Url.Authority + "/";
                string api = "Admin/ApproveCaretaker/";
                var serviceContent = JsonConvert.SerializeObject(careTakerModel);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    TempData["ApproveMessage"] = "Caregiver Approved.";
                    NotificationHub.Static_Send("notify", "Caregiver Approved", "static");
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    TempData["ApproveMessage"] = "You are not authorized to perform this action.";
                }
                else
                {
                    TempData["ApproveMessage"] = "Caregiver Approve failed.";
                }
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-NewCaretakerApprove");
                return null;
            }
        }

        public ActionResult UpdateCaretaker(CareTakerRegistrationViewModel objCareTakerViewModel)

        {
            try
            {
                if (Session["UserType"] == null)
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                if (objCareTakerViewModel.FirstName != null && objCareTakerViewModel.LastName != null)
                {
                    string api = "CareTaker/SaveCareTaker";
                    objCareTakerViewModel.UserTypeId = 1;
                    objCareTakerViewModel.CareTakerExperiences = UserSessionManager.GetCareTakerExperience(this);
                    objCareTakerViewModel.CareTakerQualifications = UserSessionManager.GetCareTakerQualification(this);
                    objCareTakerViewModel.CareTakerServices = UserSessionManager.GetCareTakerService(this);
                    objCareTakerViewModel.CareTakerClients = UserSessionManager.GetCareTakerClient(this);

                    objCareTakerViewModel.SiteURL = Request.Url.Scheme + "://" + Request.Url.Authority + "/";
                    Guid fileName = Guid.NewGuid();
                    if (objCareTakerViewModel.ProfilePicByte != null)
                    {
                        string filePath = Server.MapPath("~/PCMS/ProfilePics/CareGiver/") + fileName.ToString() + ".jpg";
                        if (!Directory.Exists(Server.MapPath("~/PCMS/ProfilePics/CareGiver/")))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/PCMS/ProfilePics/CareGiver/"));
                        }
                        System.IO.File.WriteAllBytes(filePath, objCareTakerViewModel.ProfilePicByte);
                        objCareTakerViewModel.ProfilePicByte = null;
                        filePath = objCareTakerViewModel.SiteURL + "PCMS/ProfilePics/CareGiver/" + fileName.ToString() + ".jpg";
                        objCareTakerViewModel.ProfilePicPath = filePath;
                    }

                    if (objCareTakerViewModel.ConsentDocument != null)
                    {
                        using (var binaryReader = new BinaryReader(objCareTakerViewModel.ConsentDocument.InputStream))
                        {
                            objCareTakerViewModel.ConsentForm = binaryReader.ReadBytes(objCareTakerViewModel.ConsentDocument.ContentLength);
                            string ConsentDocPath = Server.MapPath("~/PCMS/Documents/CareGiver/") + objCareTakerViewModel.CaretakerProfileId + "_ConsentForm" + ".pdf";
                            if (!Directory.Exists(Server.MapPath("~/PCMS/Documents/CareGiver/")))
                            {
                                Directory.CreateDirectory(Server.MapPath("~/PCMS/Documents/CareGiver/"));
                            }
                            System.IO.File.WriteAllBytes(ConsentDocPath, objCareTakerViewModel.ConsentForm);
                            objCareTakerViewModel.ConsentForm = null;
                            ConsentDocPath = objCareTakerViewModel.SiteURL + "PCMS/Documents/CareGiver/" + objCareTakerViewModel.CaretakerProfileId + "_ConsentForm" + ".pdf";
                            objCareTakerViewModel.ConsentDocPath = ConsentDocPath;
                        }
                    }
                    objCareTakerViewModel.ConsentDocument = null;

                    var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    serializer.MaxJsonLength = Int32.MaxValue;
                    var jsonModel = serializer.Serialize(objCareTakerViewModel.ConsentForm);


                    var careTakerViewModel = JsonConvert.SerializeObject(objCareTakerViewModel);
                    HttpStatusCode resultPost = service.PostAPI(careTakerViewModel, api);
                }
                UserSessionManager.SetCareTakerExperience(this, null);
                UserSessionManager.SetCareTakerQualification(this, null);
                UserSessionManager.SetCareTakerService(this, null);
                UserSessionManager.SetCareTakerClient(this, null);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-UpdateCaretaker");

            }
            return RedirectToAction("ApproveNewCaretaker", new { caretakerId = objCareTakerViewModel.UserId });
        }
        public ActionResult BookingNotification()
        {
            List<AdminBookingNotification> notifications = null;
            try
            {
                LocationSearchInputs inputs = new LocationSearchInputs();
                HomeController home = new HomeController();
                var loggedInUserId = (int)Session["loggedInUserId"];
                //get Work Role
                var userRoleDetails = home.GetUserRoleDetails(loggedInUserId);
                if (userRoleDetails != null)
                {

                    if (userRoleDetails.WorkRoleId == 1)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = null;
                        inputs.CityId = null;
                        ViewData["UserWorkRole"] = 1;
                    }
                    if (userRoleDetails.WorkRoleId == 2)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = userRoleDetails.StateId;
                        inputs.CityId = null;
                        ViewData["UserWorkRole"] = 2;
                    }
                    if (userRoleDetails.WorkRoleId == 3)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = userRoleDetails.StateId;
                        inputs.CityId = userRoleDetails.CityId;
                        ViewData["UserWorkRole"] = 3;
                    }
                    if (userRoleDetails.WorkRoleId == 4)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = userRoleDetails.StateId;
                        inputs.CityId = userRoleDetails.CityId;
                        ViewData["UserWorkRole"] = 4;
                    }
                }
                else
                {
                    inputs.CountryId = null;
                    inputs.StateId = null;
                    inputs.CityId = null;
                    ViewData["UserWorkRole"] = null;
                }

                var clientSearchInputsContent = JsonConvert.SerializeObject(inputs);
                string api = "Admin/GetAdminNotificationByLocation";
                var result = service.PostAPIWithData(clientSearchInputsContent, api);
                notifications = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AdminBookingNotification>>(result.Result);

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in CareTaker Controller-Notification");

            }

            return PartialView("_AdminBookingNotificationPartial", notifications);
        }



        private IEnumerable<Cities> GetBranchByLocation()
        {
            string api = string.Empty;
            List<Cities> BranchList = new List<Cities>();
            var inputs = new LocationSearchInputs();
            HomeController home = new HomeController();
            var loggedInUserId = (int)Session["loggedInUserId"];
            //get Work Role
            var userRoleDetails = home.GetUserRoleDetails(loggedInUserId);
            if (userRoleDetails != null)
            {

                if (userRoleDetails.WorkRoleId == 1)
                {
                    inputs.CountryId = userRoleDetails.CountryId;
                    inputs.StateId = null;
                    inputs.CityId = null;
                    ViewData["UserWorkRole"] = 1;
                }
                if (userRoleDetails.WorkRoleId == 2)
                {
                    inputs.CountryId = userRoleDetails.CountryId;
                    inputs.StateId = userRoleDetails.StateId;
                    inputs.CityId = null;
                    ViewData["UserWorkRole"] = 2;
                }
                if (userRoleDetails.WorkRoleId == 3)
                {
                    inputs.CountryId = userRoleDetails.CountryId;
                    inputs.StateId = userRoleDetails.StateId;
                    inputs.CityId = userRoleDetails.CityId;
                    ViewData["UserWorkRole"] = 3;
                }
                if (userRoleDetails.WorkRoleId == 4)
                {
                    inputs.CountryId = userRoleDetails.CountryId;
                    inputs.StateId = userRoleDetails.StateId;
                    inputs.CityId = userRoleDetails.CityId;
                    ViewData["UserWorkRole"] = 4;
                }
            }
            else
            {
                inputs.CountryId = null;
                inputs.StateId = null;
                inputs.CityId = null;
                ViewData["UserWorkRole"] = null;
            }
            try
            {
                api = "Admin/GetBranchByLocation/";
                var clientSearchInputsContent = JsonConvert.SerializeObject(inputs);
                var Result = service.PostAPIWithData(clientSearchInputsContent, api);
                BranchList = JsonConvert.DeserializeObject<List<Cities>>(Result.Result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin getBranch");
            }

            return BranchList;
        }

        [HttpPost]
        public JsonResult GetBranchByLocation(LocationSearchInputs inputs)
        {
            List<Cities> BranchList = new List<Cities>();
            string api = string.Empty;
            if (inputs == null)
            {
                inputs = new LocationSearchInputs();
                inputs.CountryId = null;
                inputs.StateId = null;
                inputs.CityId = null;
            }
            if (inputs.CountryId == 0)
            {
                inputs.CountryId = null;
            }
            if (inputs.StateId == 0)
            {
                inputs.StateId = null;
            }
            if (inputs.CityId == 0)
            {
                inputs.CityId = null;
            }
            try
            {
                api = "Admin/GetBranchByLocation/";
                var clientSearchInputsContent = JsonConvert.SerializeObject(inputs);
                var Result = service.PostAPIWithData(clientSearchInputsContent, api);
                BranchList = JsonConvert.DeserializeObject<List<Cities>>(Result.Result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin getBranch");
            }
            ViewData["BranchList"] = new SelectList(BranchList, "BranchId", "BranchName", "");
            return Json(BranchList);
        }

        public ActionResult BookingNotificationByCaretakerLocation(LocationSearchInputs inputs)
        {
            List<AdminBookingNotification> notifications = null;
            try
            {
                if (inputs == null)
                {
                    inputs = new LocationSearchInputs();
                    inputs.CountryId = null;
                    inputs.StateId = null;
                    inputs.CityId = null;
                }
                if (inputs.CountryId == 0)
                {
                    inputs.CountryId = null;
                }
                if (inputs.StateId == 0)
                {
                    inputs.StateId = null;
                }
                if (inputs.CityId == 0)
                {
                    inputs.CityId = null;
                }
                var clientSearchInputsContent = JsonConvert.SerializeObject(inputs);
                string api = "Admin/GetAdminNotificationByLocation";
                var result = service.PostAPIWithData(clientSearchInputsContent, api);
                notifications = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AdminBookingNotification>>(result.Result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in CareTaker Controller-Notification");

            }

            return PartialView("_AdminBookingNotificationPartial", notifications);
        }
        public ActionResult SendInvoiceToUser(string invoiceNo, string emailId)
        {
            InvoiceMail invoiceMail = new InvoiceMail();
            string paymentLink = "<br /><a href = '" + string.Format("{0}://{1}/PublicUser/InvoicePayments/?invoice={2}", Request.Url.Scheme, Request.Url.Authority, invoiceNo) + "'>Click here for payment.</a>";
            try
            {
                invoiceMail.EmailId = emailId;
                invoiceMail.paymentLink = paymentLink;
                string invoiceapi = "Admin/SendInvoice";
                var serviceContent = JsonConvert.SerializeObject(invoiceMail);
                var invoiceresult = service.PostAPIWithData(serviceContent, invoiceapi);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in CareTaker Controller-Notification");
            }
            return View();
        }

        public ActionResult GenerateInvoice(int bookingId, string emailId)
        {
            InvoiceMail invoiceMail = new InvoiceMail();
            invoiceMail.BookingId = bookingId;
            invoiceMail.SiteUrl = string.Format("{0}://{1}/", Request.Url.Scheme, Request.Url.Authority);
            invoiceMail.EmailId = emailId;
            string Invoiceapi = "Admin/GenerateInvoice";
            var serviceContent = JsonConvert.SerializeObject(invoiceMail);
            var invoiceresult = service.PostAPIWithData(serviceContent, Invoiceapi);
            string invoiceNo = invoiceresult.Result;
            //string encryptionPassword = ConfigurationManager.AppSettings["EncryptPassword"].ToString();
            //string passwordEpt = StringCipher.Encrypt(bookingId.ToString(), encryptionPassword);
            //SendInvoiceToUser(invoiceNo, emailId);
            return RedirectToAction("AdminDashboard");
        }
        public ActionResult NewBooking()
        {
            List<AdminBookingList> bookingList = null;
            try
            {
                LocationSearchInputs inputs = new LocationSearchInputs();
                HomeController home = new HomeController();
                var loggedInUserId = (int)Session["loggedInUserId"];
                //get Work Role
                var userRoleDetails = home.GetUserRoleDetails(loggedInUserId);
                if (userRoleDetails != null)
                {

                    if (userRoleDetails.WorkRoleId == 1)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = null;
                        inputs.CityId = null;
                        ViewData["UserWorkRole"] = 1;
                    }
                    if (userRoleDetails.WorkRoleId == 2)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = userRoleDetails.StateId;
                        inputs.CityId = null;
                        ViewData["UserWorkRole"] = 2;
                    }
                    if (userRoleDetails.WorkRoleId == 3)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = userRoleDetails.StateId;
                        inputs.CityId = userRoleDetails.CityId;
                        ViewData["UserWorkRole"] = 3;
                    }
                    if (userRoleDetails.WorkRoleId == 4)
                    {
                        inputs.CountryId = userRoleDetails.CountryId;
                        inputs.StateId = userRoleDetails.StateId;
                        inputs.CityId = userRoleDetails.CityId;
                        ViewData["UserWorkRole"] = 4;
                    }
                }
                else
                {
                    inputs.CountryId = null;
                    inputs.StateId = null;
                    inputs.CityId = null;
                    ViewData["UserWorkRole"] = null;
                }
                var clientSearchInputsContent = JsonConvert.SerializeObject(inputs);
                string api = "Admin/GetAdminBookingListByLocation";
                var result = service.PostAPIWithData(clientSearchInputsContent, api);
                bookingList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AdminBookingList>>(result.Result);

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in CareTaker Controller-Notification");

            }
            return PartialView("_AdminBookingListPartial", bookingList);
        }

        [HttpPost]
        public ActionResult NewBookingByLocation(LocationSearchInputs inputs)
        {
            List<AdminBookingList> bookingList = null;
            try
            {
                if (inputs == null)
                {
                    inputs = new LocationSearchInputs();
                    inputs.CountryId = null;
                    inputs.StateId = null;
                    inputs.CityId = null;
                }
                if (inputs.CountryId == 0)
                {
                    inputs.CountryId = null;
                }
                if (inputs.StateId == 0)
                {
                    inputs.StateId = null;
                }
                if (inputs.CityId == 0)
                {
                    inputs.CityId = null;
                }
                var clientSearchInputsContent = JsonConvert.SerializeObject(inputs);
                string api = "Admin/GetAdminBookingListByLocation";
                var result = service.PostAPIWithData(clientSearchInputsContent, api);
                bookingList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AdminBookingList>>(result.Result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-ManagePublicUser");
            }
            return PartialView("_AdminBookingListPartial", bookingList);
        }
        public ActionResult NewScheduling()
        {
            List<AdminSchedulingList> schedulingList = null;
            try
            {
                string api = "Admin/GetAdminSchedulingList";
                var result = service.GetAPI(api);
                schedulingList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AdminSchedulingList>>(result);

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in CareTaker Controller-Notification");

            }
            return PartialView("_AdminSchedulingListPartial", schedulingList);
        }
        public ActionResult ViewBookingDetails(int bookingId)
        {
            AdminBookingDetails viewBookingDetails = null;
            try
            {
                string api = "Admin/GetBookingDetailsById/" + bookingId;
                var result = service.GetAPI(api);
                viewBookingDetails = Newtonsoft.Json.JsonConvert.DeserializeObject<AdminBookingDetails>(result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in  CareTaker Controller-UserBooking");

            }
            return PartialView("_AdminBookingDetailsPartial", viewBookingDetails);
        }

        public ActionResult ViewSchedulingDetails(int schedulingId)
        {
            AdminSchedulingDetails viewSchedulingDetails = null;
            try
            {
                string api = "Admin/GetSchedulingDetailsById/" + schedulingId;
                var result = service.GetAPI(api);
                viewSchedulingDetails = Newtonsoft.Json.JsonConvert.DeserializeObject<AdminSchedulingDetails>(result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in  CareTaker Controller-UserBooking");

            }
            return PartialView("_AdminSchedulingDetailsPartial", viewSchedulingDetails);
        }
        public ActionResult AdminCalendarView()
        {
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 9;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = Newtonsoft.Json.JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in  CareTaker Controller-UserBooking");

            }
            return PartialView("_AdminCalendarViewPartial");
        }

        public ActionResult UserLoginDetails(int? bookingId)
        {
            UsersDetails userDetail = new UsersDetails();
            string api = "Admin/GetUserDetail/" + bookingId;
            var serviceResult = service.GetAPI(api);
            userDetail = JsonConvert.DeserializeObject<UsersDetails>(serviceResult);
            string encryptionPassword = ConfigurationManager.AppSettings["EncryptPassword"].ToString();
            string passwordEpt = StringCipher.Decrypt(userDetail.Password, encryptionPassword);
            userDetail.Password = passwordEpt;
            return PartialView(userDetail);
        }

        public ActionResult ChangePswd(string newPswd, string emailID)
        {
            string encryptionPassword = ConfigurationManager.AppSettings["EncryptPassword"].ToString();
            ChangePassword changePwd = new ChangePassword();
            changePwd.EmailId = emailID;
            changePwd.NewPassword = StringCipher.Encrypt(newPswd, encryptionPassword);
            string api = "Home/ChangePassword";
            var serviceContent = JsonConvert.SerializeObject(changePwd);
            HttpStatusCode result = service.PostAPI(serviceContent, api);
            TempData["Success"] = "Password reset successful";
            return View("ManageOfficeStaff");

        }

        public JsonResult CancelBookingStatus(int userId, int status,string reason)
        {
            string SiteURL =( Request.Url.Scheme + "://" + Request.Url.Authority + "/");

            BookingStatusUpdate bookingStatus = new BookingStatusUpdate();
            bookingStatus.userId = userId;
            bookingStatus.status = status;
            bookingStatus.SiteUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/";
            bookingStatus.Reason = reason;
            var serviceContent = JsonConvert.SerializeObject(bookingStatus);
            string api = "Admin/ChangeBookingStatus";
            HttpStatusCode result = service.PostAPI(serviceContent, api);
            if (result == HttpStatusCode.OK)
            {
                TempData["Success"] = "Booking cancellation completed successfully";
            }
            else if (result == HttpStatusCode.Unauthorized)
            {
                TempData["Failure"] = "You are not authorized to perform this action.";
            }
            else
            {
                TempData["Failure"] = "Updation failed. Please try again later.";
            }
            return Json(string.Empty);
        }

        public ActionResult ShowMap()
        {
            return View();
        }

        public ActionResult BackupDatabase()
        {
            try
            {
                TempData["Failure"] = null;
                //ViewBag.Message = "Care taker profile page.";
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        TempData["Failure"] = Constants.NoViewPrivilege;
                        return RedirectToAction("Roles");
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 15;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            TempData["Failure"] = Constants.NoViewPrivilege;
                            return RedirectToAction("Roles");
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                    return View();
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in  CareTaker Controller-UserBooking");

            }
            return RedirectToAction("", "");
        }

        public JsonResult LogJqueryError(string err, string actionName)
        {
            Logger.Error(err, "Error occurred in " + actionName);

            return Json(string.Empty);
        }
        public ActionResult DownloadDbBackup()
        {

            string path = "tesd";
            Service service = new Service();
            string api = "Admin/DownloadDbBackup/" + path;
            var result = service.PostAPIWithData(null, api);
            if (result.Result != "Unable to download")
            {

                byte[] fileBytes = System.IO.File.ReadAllBytes(result.Result);
                string fileName = result.Result.Split('\\').Last();
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            else if (result.Result == "")
            {

            }
            else
            {
                TempData["Failure"] = "DataBase backup failed!";
                return RedirectToAction("BackupDatabase");
            }
            return RedirectToAction("BackupDatabase");
        }
        public ActionResult AdminProfile(int id)
        {
            List<CountryViewModel> listCountry = new List<CountryViewModel>();
            List<StateViewModel> listState = new List<StateViewModel>();
            UsersDetails adminProfile = null;
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF" && Convert.ToInt32(Session["loggedInUserId"]) != id)
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 4;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR" || Convert.ToInt32(Session["loggedInUserId"]) == id)
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                string countryApi = "Admin/GetCountryDetails/0";
                var countryResult = service.GetAPI(countryApi);
                listCountry = JsonConvert.DeserializeObject<List<CountryViewModel>>(countryResult);
                ViewBag.Country = new SelectList(listCountry, "CountryId", "Name", 0);
                string stateApi = "Admin/GetStateDetails/0";
                var stateResult = service.GetAPI(stateApi);
                listState = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult);
                ViewBag.Name = new SelectList(listState, "StateId", "Name", 0);
                List<Cities> cityList = new List<Cities>();
                string apiCity = "Admin/GetCity?flag=*&value=''";
                var resultCity = service.GetAPI(apiCity);
                cityList = JsonConvert.DeserializeObject<List<Cities>>(resultCity);
                ViewBag.City = new SelectList(cityList, "CityId", "CityName", 0);

                ViewBag.Message = "Admin";
                api = "Admin/GetAdminProfile/" + id;
                var result = service.GetAPI(api);
                adminProfile = Newtonsoft.Json.JsonConvert.DeserializeObject<UsersDetails>(result);
                adminProfile.UserRegnId = id;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-ViewOfficeStaffProfile");

            }
            return View(adminProfile);
        }
        public ActionResult UpdateAdminProfile(UsersDetails adminDetails)
        {
            try
            {
                if (Request.Form["update"] != null)
                {
                    string strCountryValue = Request.Form["CountryId"].ToString();
                    string strStatesValue = Request.Form["StateId"].ToString();
                    string strCityValue = Request.Form["CityId"].ToString();
                    adminDetails.CountryId = Convert.ToInt32(strCountryValue);
                    adminDetails.StateId = Convert.ToInt32(strStatesValue);
                    adminDetails.CityId = Convert.ToInt32(strCityValue);

                    adminDetails.SiteURL = Request.Url.Scheme + "://" + Request.Url.Authority + "/";
                    Guid fileName = Guid.NewGuid();
                    if (adminDetails.ProfilePicByte != null)
                    {
                        string FilePath = Server.MapPath("~/PCMS/ProfilePics/Admin/") + fileName.ToString() + ".jpg";
                        if (!Directory.Exists(Server.MapPath("~/PCMS/ProfilePics/Admin/")))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/PCMS/ProfilePics/Admin/"));
                        }
                        System.IO.File.WriteAllBytes(FilePath, adminDetails.ProfilePicByte);
                        adminDetails.ProfilePicByte = null;
                        FilePath = adminDetails.SiteURL + "PCMS/ProfilePics/Admin/" + fileName.ToString() + ".jpg";
                        adminDetails.ProfilePicPath = FilePath;
                    }

                    string api = "Admin/UpdateAdminProfile";
                    var serviceContent = JsonConvert.SerializeObject(adminDetails);
                    HttpStatusCode result = service.PostAPI(serviceContent, api);
                    if (result == HttpStatusCode.OK)
                    {
                        TempData["Userupdate"] = "User details updated Successfully";
                    }
                    else if (result == HttpStatusCode.Unauthorized)
                    {
                        TempData["Userupdate"] = "You are not authorized to perform this action.";
                    }
                    else
                    {
                        TempData["Userupdate"] = "Update failed";
                    }
                    if (Convert.ToInt32(Session["loggedInUserId"].ToString()) == adminDetails.UserRegnId)
                        Session["profilePic"] = adminDetails.ProfilePicPath;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in PublicUser Controller-UpdateUserDetails");
            }
            return RedirectToAction("AdminProfile", new { id = adminDetails.UserRegnId });
        }
        [HttpPost]
        public ActionResult UpdateUserEmail(int id, string emailId, string name, int usertype)
        {
            try
            {
                var api = "Users/SendVerificationEmail";//+ publicUserDetails.EmailAddress
                VerifyEmail verifyEmail = new VerifyEmail
                {
                    WelcomeMsg = "Welcome to Tranquil Care!",
                    FirstName = name,
                    MailMsg = "Thank you for registering with us, you have successfully created an account with us.",
                    Mailcontent = string.Format("{0}://{1}/Home/EmailVerificationSuccess/{2}", Request.Url.Scheme, Request.Url.Authority, HttpUtility.UrlEncode(StringCipher.EncodeNumber(id))),
                    ContactNo = "1-800-892-6066",
                    RegardsBy = "Tranquil Care Inc.",
                    siteUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/",
                    CompanyName_TagLine = "Tranquil Care - Delivering Care Excellence.",
                    CompanyName = "Tranquil Care Inc.",
                    Subject = "Email Verification Link",
                    Email = emailId
                };
                var serviceContent = JsonConvert.SerializeObject(verifyEmail);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                NotificationHub.Static_Send("notify", "New Office staff registered", "static");
                string updateEmailapi = string.Empty;
                UsersDetails userDetails = new UsersDetails();
                userDetails.UserRegnId = id;
                userDetails.EmailAddress = emailId;
                userDetails.FirstName = name;
                updateEmailapi = "Admin/UpdateUserEmail";
                var emailserviceContent = JsonConvert.SerializeObject(userDetails);
                var emailResult = service.PostAPI(emailserviceContent, updateEmailapi);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in PublicUser Controller-UpdateUserDetails");
            }
            if (usertype == 1)
            {
                return RedirectToAction("ApproveNewCaretaker", "Admin", new { caretakerId = id });
            }
            else if (usertype == 5)
            {
                return RedirectToAction("ViewOfficeStaffProfile", "Admin", new { id });
            }
            else
            {
                return RedirectToAction("UserProfile", "PublicUser", new { id });
            }
        }

        [HttpPost]
        public ActionResult UpdateClientEmail(int id, string emailId, string name)
        {
            try
            {
                var api = "Users/SendVerificationEmail";//+ publicUserDetails.EmailAddress
                VerifyEmail verifyEmail = new VerifyEmail
                {
                    WelcomeMsg = "Welcome to Tranquil care!",
                    FirstName = name,
                    MailMsg = "Thank you for registering with us, you have successfully created an account with us.",
                    Mailcontent = string.Format("{0}://{1}/Home/EmailVerificationSuccess/{2}", Request.Url.Scheme, Request.Url.Authority, HttpUtility.UrlEncode(StringCipher.EncodeNumber(id))),
                    ContactNo = "1-800-892-6066",
                    RegardsBy = "Tranquil Care Inc.",
                    siteUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/",
                    CompanyName_TagLine = "Tranquil Care - Delivering Care Excellence.",
                    CompanyName = "Tranquil Care Inc.",
                    Subject = "Email Verification Link",
                    Email = emailId
                };
                var serviceContent = JsonConvert.SerializeObject(verifyEmail);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                string updateEmailapi = string.Empty;
                UsersDetails userDetails = new UsersDetails();
                userDetails.UserRegnId = id;
                userDetails.EmailAddress = emailId;
                userDetails.FirstName = name;
                updateEmailapi = "Admin/UpdateUserEmail";
                var emailserviceContent = JsonConvert.SerializeObject(userDetails);
                var emailResult = service.PostAPI(emailserviceContent, updateEmailapi);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in PublicUser Controller-UpdateClientEmail");
            }

            return null;

        }

        public ActionResult Residents()
        {
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }

                List<ClientModel> clientList = new List<ClientModel>();
                string clientApi = "Client/GetAllClientDetails";
                var Clients = service.GetAPI(clientApi);
                clientList = JsonConvert.DeserializeObject<List<ClientModel>>(Clients);
                ViewBag.clientList = new SelectList(clientList, "ClientId", "ClientName", 0);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-ManageOfficeStaff");

            }
            return View();

        }

        public ActionResult SaveResidentDetails(Resident residentDetails)
        {
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        TempData["Failure"] = Constants.NoViewPrivilege;
                        return RedirectToAction("Category");
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowEdit)
                        {
                            TempData["Failure"] = Constants.NoActionPrivilege;
                            return RedirectToAction("Category");
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }

                api = "Admin/SaveResidentDetails";
                var residentContent = JsonConvert.SerializeObject(residentDetails);
                var result = service.PostAPIWithData(residentContent, api);

                if (result.Result == "OK")
                {
                    TempData["Success"] = "Resident details saved successfully.";
                }
                else if (result.Result == "Not Found")
                {
                    TempData["Failure"] = "Updation Failed. Please try again later.";
                }
                else if (result.Result == "Name")
                {
                    TempData["Failure"] = "Name already exists. Please enter different data.";
                }
                else if (result.Result == "Duplicate")
                {
                    TempData["Failure"] = "The Resident already exists for the client. Please enter different data.";
                }
                else
                {
                    TempData["Failure"] = " You are not authorized to perform this action.";
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-SaveResidentDetails");
            }
            return RedirectToAction("Residents");

        }
        public ActionResult ResidentList()
        {
            List<Resident> residentList = new List<Resident>();
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        TempData["Failure"] = Constants.NoViewPrivilege;
                        return RedirectToAction("Roles");
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            TempData["Failure"] = Constants.NoViewPrivilege;
                            return RedirectToAction("Roles");
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                api = "Admin/GetResidentDetails";
                var result = service.GetAPI(api);
                residentList = JsonConvert.DeserializeObject<List<Resident>>(result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-ResidentList");
            }
            return PartialView("_ResidentListPartial", residentList);
        }
        public JsonResult DeleteResident(int residentID)
        {
            try
            {
                string api = string.Empty;
                api = "Admin/DeleteResident/" + residentID;
                var serviceContent = JsonConvert.SerializeObject(residentID);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    return Json("Resident settings deleted successfully");
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    return Json("You are not authorized to perform this action.");
                }
                else if (result == HttpStatusCode.Ambiguous)
                {
                    return Json("Data cannot be deleted due to reference.");
                }
                else
                {
                    return Json("Updation Failed. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-DeleteResident");
            }
            return Json(string.Empty);
        }

        public ActionResult LoadResidentList(int clientId)
        {
            try
            {
                string api = "Admin/GetResidentDetailsById/" + clientId;
                var residentResult = service.GetAPI(api);
                return Json(residentResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-LoadResidentList");
                return null;
            }
        }
        public ActionResult EmailIDConfiguration()
        {
            ClientModel clientModelObj = new ClientModel();
            try
            {
                string api = string.Empty;
                string country = Session["CountryId"].ToString();
                string state = Session["StateID"].ToString();
                string CityIdk = Session["CityIdk"].ToString();
                if (Session["UserType"] != null)
                {
                    if (country != "" && state != "" && CityIdk != "")
                    {
                        List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                        api = "Admin/GetCountryDetails/0";
                        var countrylist1 = service.GetAPI(api);
                        countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                        ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                        List<StateViewModel> stateList1 = new List<StateViewModel>();
                        api = "/Admin/GetStatesByCountryId/" + country;
                        var stateResult1 = service.GetAPI(api);
                        stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                        ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", clientModelObj.StateId1);

                        api = "Admin/GetCity?flag=StateId&value=" + state;
                        List<Cities> cityList1 = new List<Cities>();
                        var resultCity1 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                        ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", clientModelObj.CityId1);

                        api = "Admin/GetAllBranch/";
                        List<Cities> branchList1 = new List<Cities>();
                        var resultBranch1 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultBranch1);
                        ViewData["ListBranch"] = new SelectList(cityList1, "branchId", "branchName", clientModelObj.BranchId2);
                    }
                    else
                    {
                        List<CountryViewModel> countryList1 = new List<CountryViewModel>();
                        api = "Admin/GetCountryDetails/0";
                        var countrylist1 = service.GetAPI(api);
                        countryList1 = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist1);
                        ViewData["ListCountry1"] = new SelectList(countryList1, "CountryId", "Name", clientModelObj.CountryId1);

                        List<StateViewModel> stateList1 = new List<StateViewModel>();
                        api = "/Admin/GetStatesByCountryId/1";
                        var stateResult1 = service.GetAPI(api);
                        stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                        ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", clientModelObj.StateId1);

                        api = "Admin/GetCity?flag=StateId&value=" + clientModelObj.StateId1;
                        List<Cities> cityList1 = new List<Cities>();
                        var resultCity1 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultCity1);
                        ViewData["ListCity1"] = new SelectList(cityList1, "CityId", "CityName", clientModelObj.CityId1);

                        api = "Admin/GetAllBranch/";
                        List<Cities> branchList1 = new List<Cities>();
                        var resultBranch1 = service.GetAPI(api);
                        cityList1 = JsonConvert.DeserializeObject<List<Cities>>(resultBranch1);
                        ViewData["ListBranch"] = new SelectList(cityList1, "branchId", "branchName", clientModelObj.BranchId2);
                    }
                }
                else
                {
                    Logger.Error("Redirect to Login ");
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }

                var listEmailType = GetEmailType();
                ViewData["GenderList"] = new SelectList(listEmailType, "EmailTypeId", "EmailType", 0);
                var BranchList = GetBranchByLocation();
                ViewData["BranchList"] = new SelectList(BranchList, "BranchId", "BranchName", "");
                return View();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-LoadResidentList");
                return null;
            }
        }
        public ActionResult SchedulingLog()
        {
            List<OfficeStaffRegistration> officeStaffRegistration = null;
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 4;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                api = "Admin/GetAllSchedulingLog";
                var result = service.GetAPI(api);
                officeStaffRegistration = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OfficeStaffRegistration>>(result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-ManageOfficeStaff");

            }
            return View(officeStaffRegistration);
        }
        private IEnumerable<EmailTypeViewModel> GetEmailType()
        {
            try
            {
                var listGender = Enum.GetValues(typeof(EmailType))
                   .Cast<EmailType>()
                   .Select(t => new EmailTypeViewModel
                   {
                       EmailtypeId = ((int)t),
                       Emailtype = t.ToString()
                   });
                return listGender;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Caretaker Controller-getGenders");
                return null;
            }
        }

        public ActionResult AddEmailTypeConfiguration(EmailTypeConfiguration emailTypeConfiguration)
        {
            try
            {
                TempData["Success"] = "";
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        TempData["Failure"] = Constants.NoViewPrivilege;
                        return RedirectToAction("Roles");
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            TempData["Failure"] = Constants.NoViewPrivilege;
                            return RedirectToAction("Roles");
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                string encryptionPassword = ConfigurationManager.AppSettings["EncryptPassword"].ToString();
                string passwordEpt = StringCipher.Encrypt(emailTypeConfiguration.Password, encryptionPassword);
                emailTypeConfiguration.Password = passwordEpt;
                api = "Admin/AddEmailTypeConfiguration";
                var serviceContent = JsonConvert.SerializeObject(emailTypeConfiguration);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    TempData["Success"] = "Configurations saved successfully";
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    TempData["Failure"] = "You are not authorized to perform this action.";
                }
                else if (result == HttpStatusCode.Conflict)
                {
                    TempData["Failure"] = "Data already exist. Please enter different data.";
                }
                else
                {
                    TempData["Failure"] = "Updation failed. Please try again later.";
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-Email configuration");
            }
            return RedirectToAction("EmailIDConfiguration");
        }

        public ActionResult GetEmailTypeConfigList()
        {
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        TempData["Failure"] = Constants.NoViewPrivilege;
                        return RedirectToAction("Roles");
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            TempData["Failure"] = Constants.NoViewPrivilege;
                            return RedirectToAction("Roles");
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                api = "Admin/GetEmailTypeConfig";
                var result = service.GetAPI(api);
                var configurationList = JsonConvert.DeserializeObject<List<EmailTypeConfiguration>>(result);
                foreach (var item in configurationList)
                {
                    string decryptionPassword = ConfigurationManager.AppSettings["EncryptPassword"].ToString();
                    string passwordDecrpt = StringCipher.Decrypt(item.Password, decryptionPassword);
                    item.Password = passwordDecrpt;
                }
                return PartialView("_EmailTypeConfigurationPartial", configurationList);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-GetConfigurationlist");
                return null;
            }
        }
        public JsonResult DeleteEmailTypeConfig(int configId)
        {
            try
            {
                string api = string.Empty;

                api = "Admin/DeleteEmailTypeConfig/" + configId;
                var serviceContent = JsonConvert.SerializeObject(configId);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    return Json("Config settings deleted successfully");
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    return Json("You are not authorized to perform this action.");
                }
                else if (result == HttpStatusCode.Ambiguous)
                {
                    return Json("Data cannot be deleted due to reference.");
                }
                else
                {
                    return Json("Updation Failed. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-DeleteCity");
            }
            return Json(string.Empty);
        }

        public ActionResult SMTPConfiguration()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-SMTP configuration");
                return null;
            }
        }

        public ActionResult AddEmailConfiguration(EmailConfiguration emailConfiguration)
        {
            try
            {
                TempData["Success"] = "";
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        TempData["Failure"] = Constants.NoViewPrivilege;
                        return RedirectToAction("Roles");
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            TempData["Failure"] = Constants.NoViewPrivilege;
                            return RedirectToAction("Roles");
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }

                api = "Admin/AddEmailConfiguration";
                var serviceContent = JsonConvert.SerializeObject(emailConfiguration);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    TempData["Success"] = "Configurations saved successfully";
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    TempData["Failure"] = "You are not authorized to perform this action.";
                }
                else if (result == HttpStatusCode.Conflict)
                {
                    TempData["Failure"] = "Data already exist. Please enter different data.";
                }
                else
                {
                    TempData["Failure"] = "Updation failed. Please try again later.";
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-Email configuration");
            }
            return RedirectToAction("SMTPConfiguration");
        }

        public ActionResult GetConfigurationList()
        {
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        TempData["Failure"] = Constants.NoViewPrivilege;
                        return RedirectToAction("Roles");
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            TempData["Failure"] = Constants.NoViewPrivilege;
                            return RedirectToAction("Roles");
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                api = "Admin/GetConfigList";
                var result = service.GetAPI(api);
                var configurationList = JsonConvert.DeserializeObject<List<EmailConfiguration>>(result);
                return PartialView("_EmailConfigListPartial", configurationList);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-GetConfigurationlist");
                return null;
            }
        }
        public ActionResult SetConfiguration(int configId)
        {
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        TempData["Failure"] = Constants.NoViewPrivilege;
                        return RedirectToAction("Roles");
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            TempData["Failure"] = Constants.NoViewPrivilege;
                            return RedirectToAction("Roles");
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                api = "Admin/SetConfig/" + configId;
                var serviceContent = JsonConvert.SerializeObject(configId);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    TempData["Success"] = "Default Status Changed Successfully";
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    TempData["Failure"] = "You are not authorized to perform this action.";
                }
                else if (result == HttpStatusCode.Conflict)
                {
                    TempData["Failure"] = "Data already exist. Please enter different data.";
                }
                else
                {
                    TempData["Failure"] = "Updation failed. Please try again later.";
                }
                return View("SMTPConfiguration");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-GetConfigurationlist");
                return null;
            }
        }
        public JsonResult DeleteConfigDetails(int configId)
        {
            try
            {
                string api = string.Empty;

                api = "Admin/DeleteConfigDetails/" + configId;
                var serviceContent = JsonConvert.SerializeObject(configId);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    return Json("Config settings deleted successfully");
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    return Json("You are not authorized to perform this action.");
                }
                else if (result == HttpStatusCode.Ambiguous)
                {
                    return Json("Data cannot be deleted due to reference.");
                }
                else
                {
                    return Json("Updation Failed. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-DeleteCity");
            }
            return Json(string.Empty);
        }
        public JsonResult SchedulingLogDetailsById(int? logId)
        {
            ScheduledData scheduledData = new ScheduledData();
            string api = "Client/GetSchedulingLogDetailsById/" + logId;
            var serviceResult = service.GetAPI(api);
            scheduledData = JsonConvert.DeserializeObject<ScheduledData>(serviceResult);
            return Json(scheduledData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ManageBookingPayrise()
        {

            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        TempData["Failure"] = Constants.NoViewPrivilege;
                        return RedirectToAction("Category");
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowEdit)
                        {
                            TempData["Failure"] = Constants.NoActionPrivilege;
                            return RedirectToAction("Category");
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    Logger.Error("Redirect to Login from CreateCategory-Admin Controller");
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                api = "Admin/RetrieveServiceDetails/0";
                var serviveResult = service.GetAPI(api);
                List<ServicesViewModel> listService = new List<ServicesViewModel>();
                listService = JsonConvert.DeserializeObject<List<ServicesViewModel>>(serviveResult);
                ViewBag.Service = new SelectList(listService, "ServiceId", "Name", 0);

                string caretakerapi = "CareTaker/RetrieveCareTakerListForDdl/";
                var CareTakerResult = service.GetAPI(caretakerapi);
                List<Caretakers> listCaretakers = new List<Caretakers>();
                listCaretakers = JsonConvert.DeserializeObject<List<Caretakers>>(CareTakerResult);
                ViewBag.Caretaker = new SelectList(listCaretakers, "CaretakerId", "CareTakerName", 0);

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-CreateCategory");
            }
            return View();

        }
        public ActionResult ManageInvoicePayrise()
        {
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        TempData["Failure"] = Constants.NoViewPrivilege;
                        return RedirectToAction("Category");
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowEdit)
                        {
                            TempData["Failure"] = Constants.NoActionPrivilege;
                            return RedirectToAction("Category");
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    Logger.Error("Redirect to Login from CreateCategory-Admin Controller");
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }

                string clientApi = "Client/GetAllClientDetails";
                var Clients = service.GetAPI(clientApi);
                List<ClientModel> scheduleDetailsList = new List<ClientModel>();
                scheduleDetailsList = JsonConvert.DeserializeObject<List<ClientModel>>(Clients);
                ViewBag.ClientDetails = new SelectList(scheduleDetailsList, "ClientId", "ClientName", 0);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-CreateCategory");
            }
            return View();
        }
        public ActionResult ManagePayrollPayrise()
        {
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        TempData["Failure"] = Constants.NoViewPrivilege;
                        return RedirectToAction("Category");
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 1;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowEdit)
                        {
                            TempData["Failure"] = Constants.NoActionPrivilege;
                            return RedirectToAction("Category");
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    Logger.Error("Redirect to Login from CreateCategory-Admin Controller");
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }

                string clientApi = "Client/GetAllClientDetails";
                var Clients = service.GetAPI(clientApi);
                List<ClientModel> scheduleDetailsList = new List<ClientModel>();
                scheduleDetailsList = JsonConvert.DeserializeObject<List<ClientModel>>(Clients);
                ViewBag.ClientDetails = new SelectList(scheduleDetailsList, "ClientId", "ClientName", 0);

                string caretakerapi = "CareTaker/RetrieveCareTakerListForDdl/";
                var CareTakerResult = service.GetAPI(caretakerapi);
                List<Caretakers> listCaretakers = new List<Caretakers>();
                listCaretakers = JsonConvert.DeserializeObject<List<Caretakers>>(CareTakerResult);
                ViewBag.Caretaker = new SelectList(listCaretakers, "CaretakerId", "CareTakerName", 0);

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-CreateCategory");
            }
            return View();
        }
        public ActionResult BookingPayrisePartial()
        {
            List<BookingPayriseModel> bookingPayrise = new List<BookingPayriseModel>();
            var api = "Admin/GetAllBookingPayriseDetails";
            var result = service.GetAPI(api);
            bookingPayrise = JsonConvert.DeserializeObject<List<BookingPayriseModel>>(result);

            return PartialView("_ManageBookingPayrisePartial", bookingPayrise);
        }
        public ActionResult InvoicePayrisePartial()
        {

            List<InvoicePayriseModel> invoicePayrise = new List<InvoicePayriseModel>();
            var api = "Admin/GetAllInvoicePayriseDetails";
            var result = service.GetAPI(api);
            invoicePayrise = JsonConvert.DeserializeObject<List<InvoicePayriseModel>>(result);
            return PartialView("_ManageInvoicePayrisePartial", invoicePayrise);

        }
        public ActionResult PayrollPayrisePartial()
        {
            List<PayrollPayriseModel> payrollPayrise = new List<PayrollPayriseModel>();
            var api = "Admin/GetAllPayrollPayriseDetails";
            var result = service.GetAPI(api);
            payrollPayrise = JsonConvert.DeserializeObject<List<PayrollPayriseModel>>(result);
            return PartialView("_ManagePayrollPayrisePartial", payrollPayrise);

        }
        public ActionResult SearchBookingPayriseList(BookingPayriseModel bookingPayriseModel)
        {
            List<BookingPayriseModel> bookingapiResults = new List<BookingPayriseModel>();
            try
            {
                if (bookingPayriseModel != null)
                {

                    string api = "Admin/GetBookingPayriseList";
                    var searchInputModel = JsonConvert.SerializeObject(bookingPayriseModel);
                    var result = service.PostAPIWithData(searchInputModel, api);
                    bookingapiResults = JsonConvert.DeserializeObject<List<BookingPayriseModel>>(result.Result);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-SearchBookingPayriseList");
                return null;
            }
            return PartialView("_ManageBookingPayrisePartial", bookingapiResults);
        }
        public ActionResult SearchInvoicePayriseList(InvoicePayriseModel invoicePayriseModel)
        {
            List<InvoicePayriseModel> invoiceapiResults = new List<InvoicePayriseModel>();
            try
            {
                if (invoicePayriseModel != null)
                {
                    string api = "Admin/GetInvoicePayriseList";
                    var searchInputModel = JsonConvert.SerializeObject(invoicePayriseModel);
                    var result = service.PostAPIWithData(searchInputModel, api);
                    invoiceapiResults = JsonConvert.DeserializeObject<List<InvoicePayriseModel>>(result.Result);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-SearchClentInvoiceHistory");
                return null;
            }
            return PartialView("_ManageInvoicePayrisePartial", invoiceapiResults);
        }
        public ActionResult SearchPayrollPayriseList(PayrollPayriseModel payrollPayriseModel)
        {
            List<PayrollPayriseModel> payrollapiResults = new List<PayrollPayriseModel>();
            try
            {
                if (payrollPayriseModel != null)
                {

                    string api = "Admin/GetPayrollPayriseList";
                    var searchInputModel = JsonConvert.SerializeObject(payrollPayriseModel);
                    var result = service.PostAPIWithData(searchInputModel, api);
                    payrollapiResults = JsonConvert.DeserializeObject<List<PayrollPayriseModel>>(result.Result);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-SearchClentInvoiceHistory");
                return null;
            }
            return PartialView("_ManagePayrollPayrisePartial", payrollapiResults);
        }
        [HttpPost]
        public JsonResult DeleteBookingPayrise(int bookingPayriseId)
        {
            try
            {
                string api = string.Empty;

                api = "Admin/DeleteBookingPayrise/" + bookingPayriseId;
                var serviceContent = JsonConvert.SerializeObject(bookingPayriseId);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    return Json("Payrise Details deleted successfully");
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    return Json("You are not authorized to perform this action.");
                }
                else if (result == HttpStatusCode.Ambiguous)
                {
                    return Json("Data cannot be deleted due to reference.");
                }
                else
                {
                    return Json("Updation Failed. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-DeleteCategory");
            }
            return Json(string.Empty);
        }
        public JsonResult DeleteInvoicePayrise(int invoicePayriseId)
        {
            try
            {
                string api = string.Empty;

                api = "Admin/DeleteInvoicePayrise/" + invoicePayriseId;
                var serviceContent = JsonConvert.SerializeObject(invoicePayriseId);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    return Json("Payrise Details deleted successfully");
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    return Json("You are not authorized to perform this action.");
                }
                else if (result == HttpStatusCode.Ambiguous)
                {
                    return Json("Data cannot be deleted due to reference.");
                }
                else
                {
                    return Json("Updation Failed. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-DeleteCategory");
            }
            return Json(string.Empty);
        }
        public JsonResult DeletePayrollPayrise(int payrollPayriseId)
        {
            try
            {
                string api = string.Empty;

                api = "Admin/DeletePayrollPayrise/" + payrollPayriseId;
                var serviceContent = JsonConvert.SerializeObject(payrollPayriseId);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    return Json("Payrise Details deleted successfully");
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    return Json("You are not authorized to perform this action.");
                }
                else if (result == HttpStatusCode.Ambiguous)
                {
                    return Json("Data cannot be deleted due to reference.");
                }
                else
                {
                    return Json("Updation Failed. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-DeleteCategory");
            }
            return Json(string.Empty);
        }

        public ActionResult DBMerge()
        {
            return View();
        }


        /// <summary>
        /// DB migration purpose only
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateUserProfilePic()
        {
            List<UsersDetails> usersDetails = null;
            try
            {
                string api = "Admin/GetAllUserDetails";
                string file = string.Empty;
                string filepath = string.Empty;
                string siteUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/";
                var result = service.GetAPI(api);
                usersDetails = JsonConvert.DeserializeObject<List<UsersDetails>>(result);

                foreach (var user in usersDetails)
                {
                    if (user.ProfilePicByte != null)
                    {
                        switch (user.UserType)
                        {
                            case "CARETAKER":
                                {
                                    file = Server.MapPath("~/PCMS/ProfilePics/CareGiver/") + user.UserType + "_" + user.UserRegnId + ".jpg";
                                    filepath = siteUrl + "PCMS/ProfilePics/CareGiver/" + user.UserType + "_" + user.UserRegnId + ".jpg";
                                    break;
                                }
                            case "PUBLIC USER":
                                {
                                    file = Server.MapPath("~/PCMS/ProfilePics/PublicUser/") + user.UserType + "_" + user.UserRegnId + ".jpg";
                                    filepath = siteUrl + "PCMS/ProfilePics/PublicUser/" + user.UserType + "_" + user.UserRegnId + ".jpg";
                                    break;
                                }
                            case "ADMINISTRATOR ":
                                {
                                    file = Server.MapPath("~/PCMS/ProfilePics/Admin/") + user.UserType + "_" + user.UserRegnId + ".jpg";
                                    filepath = siteUrl + "PCMS/ProfilePics/Admin/" + user.UserType + "_" + user.UserRegnId + ".jpg";
                                    break;
                                }

                        }
                        System.IO.File.WriteAllBytes(file, user.ProfilePicByte);

                        user.ProfilePicPath = filepath;
                        api = "Admin/UpdateUserDetails";
                        var residentContent = JsonConvert.SerializeObject(user);
                        var results = service.PostAPIWithData(residentContent, api);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-UpdateUserProfilePic");
                return null;
            }
        }


        /// <summary>
        /// DB migration purpose only
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateCaretakerDoc()
        {
            List<DocumentsList> ctDocs = null;
            try
            {
                string api = "Admin/GetAllCaretakerDocuments";
                string file = string.Empty;
                string filepath = string.Empty;
                string siteUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/";
                var result = service.GetAPI(api);
                ctDocs = JsonConvert.DeserializeObject<List<DocumentsList>>(result);

                foreach (var doc in ctDocs)
                {
                    if (doc.DocumentContent != null)
                    {

                        file = Server.MapPath("~/PCMS/Documents/CareGiver/") + doc.DocumentName;
                        filepath = siteUrl + "PCMS/Documents/CareGiver/" + doc.DocumentName;


                        System.IO.File.WriteAllBytes(file, doc.DocumentContent);

                        doc.DocumentPath = filepath;
                        api = "Admin/UpdateCaretakerDocuments";
                        var residentContent = JsonConvert.SerializeObject(doc);
                        var results = service.PostAPIWithData(residentContent, api);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Admin Controller-UpdateCaretakerDoc");
                return null;
            }
        }
    }

}

