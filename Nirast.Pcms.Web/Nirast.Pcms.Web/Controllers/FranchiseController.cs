using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using Nirast.Pcms.Web.Helpers;
using Nirast.Pcms.Web.Logger;
using Nirast.Pcms.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using static Nirast.Pcms.Web.Models.Enums;
using System.DirectoryServices.Protocols;
namespace Nirast.Pcms.Web.Controllers
{

    [OutputCache(Duration = 1800, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
    public class FranchiseController : Controller
    {
        PCMSLogger pCMSLogger = new PCMSLogger();
        Service service = null;

        public FranchiseController()
        {
            service = new Service(pCMSLogger);
        }

        #region ClientRegistration
        // GET: Client
        public ActionResult FranchiseRegistration()
        {
            try
            {
                string api = string.Empty;

                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Models.Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 6;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Models.Constants.NoViewPrivilege;
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
                    TempData["Failure"] = Models.Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }

                UserSessionManager.SetCaretakersList(this, null);
                UserSessionManager.SetClientTimeShift(this, null);
                UserSessionManager.SetCategoryRateList(this, null);
                List<CountryViewModel> listCountry = new List<CountryViewModel>();

                string apiCountry = "/Admin/GetCountryDetails/0";
                var countryResult = service.GetAPI(apiCountry);
                listCountry = JsonConvert.DeserializeObject<List<CountryViewModel>>(countryResult);
                int defaultCountry = (listCountry.Where(x => x.Isdefault == true).Count() > 0) ? listCountry.Where(x => x.Isdefault == true).SingleOrDefault().CountryId : listCountry.FirstOrDefault().CountryId;
                var _listCountry = new SelectList(listCountry, "CountryId", "Name", defaultCountry);
                string phoneCode = (listCountry.Where(x => x.Isdefault == true).Count() > 0) ? listCountry.Where(x => x.Isdefault == true).SingleOrDefault().PhoneCode : listCountry.FirstOrDefault().PhoneCode;
                ViewBag.PhoneCode = phoneCode == string.Empty ? "+1" : phoneCode;
                ViewData["CountryList"] = _listCountry;


                List<StateViewModel> stateList = new List<StateViewModel>();
                api = "/Admin/GetStatesByCountryId/" + defaultCountry;
                var stateResult = service.GetAPI(api);
                stateList = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult);

                ViewData["StateList"] = new SelectList(stateList, "StateId", "Name", 0);
                var _stateList = new SelectList(stateList, "StateId", "Name", 0);

                var cityList = new SelectList(new[]
                 {
                new {CityId = "", CityName = "--Select--" },
               },
                 "CityId", "CityName", 0);
                ViewData["ListCity"] = cityList;

                api = "/Admin/GetTimeShiftDetails/0";
                var result = service.GetAPI(api);
                var timeShiftList = JsonConvert.DeserializeObject<List<ClientShiftDetails>>(result);
                ViewData["ListTimeShift"] = new SelectList(timeShiftList, "TimeShiftId", "TimeShiftName", 0);

                api = "/Admin/GetCategory?flag=*&value=''";
                List<CategoryViewModel> categoryList = new List<CategoryViewModel>();
                var resultList = service.GetAPI(api);
                categoryList = JsonConvert.DeserializeObject<List<CategoryViewModel>>(resultList);
                var listCategory = new SelectList(categoryList, "CategoryId", "Name", 0);
                ViewData["ListCategory"] = listCategory;

                api = "Admin/SelectCaretakersByCaretakerStatus/" + (int)CaretakerStatus.Approved;
                List<CareTakerRegistrationViewModel> Caretakers = new List<CareTakerRegistrationViewModel>();
                var caretakerResult = service.GetAPI(api);
                Caretakers = JsonConvert.DeserializeObject<List<CareTakerRegistrationViewModel>>(caretakerResult);
                List<CareTakerRegistrationViewModel> CaretakersToLoad = new List<CareTakerRegistrationViewModel>();
                CaretakersToLoad = Caretakers.Where(a => a.UserStatus == Enums.UserStatus.Active).ToList<CareTakerRegistrationViewModel>();
                ViewData["ListCaretakers"] = CaretakersToLoad;

                api = "/Admin/GetDefaultCountry";
                var resultCurr = service.GetAPI(api);
                CountryViewModel currency = JsonConvert.DeserializeObject<CountryViewModel>(resultCurr);
                if (currency != null)
                {
                    ViewData["CurrencySymbol"] = currency.CurrencySymbol;
                }

            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-ClientRegistration");
                System.IO.File.WriteAllText(Server.MapPath("~/PDFError.txt"), ex.Message);
            }
            return View();
        }

        [HttpPost]
        public ActionResult SaveFranchiseDetails(FranchiseModel objClient)
        {
            try
            {


                if (Request.Form["RegisterClient"] != null)
                {
                    if (Session["UserType"] == null)
                    {
                        TempData["Failure"] = Models.Constants.NotLoggedIn;
                        return RedirectToAction("Login", "Account");
                    }

                    int num = new Random().Next(1000, 9999);
                    string numm = Convert.ToString(num);
                    objClient.EmployeeNumber = numm;
                    string clientna = objClient.ClientName;
                    string userid = "5";
                    string pswd = objClient.Password;
                    if (objClient.ClientId == 0)
                    {
                        string encryptionPassword = ConfigurationManager.AppSettings["EncryptPassword"].ToString();
                        string passwordEpt = StringCipher.Encrypt(objClient.Password, encryptionPassword);
                        objClient.Password = passwordEpt;
                    }
                    objClient.ClientShiftList = UserSessionManager.GetClientTimeShift(this);
                    objClient.ClientCaretakers = UserSessionManager.GetCaretakersList(this);
                    objClient.CategoryRates = UserSessionManager.GetCategoryRateList(this);

                    objClient.SiteURL = Request.Url.Scheme + "://" + Request.Url.Authority + "/";
                    string api = "Client/SaveFranchiseDetails";

                    #region Adding User to LDAP Created by Silpa


                    System.DirectoryServices.Protocols.LdapConnection ldapConnection = new System.DirectoryServices.Protocols.LdapConnection("127.0.0.1:10389");

                    var networkCredential = new NetworkCredential(
                          "employeeNumber=1,ou=Aluva,ou=Ernakulam,ou=kerala,ou=India,o=TopCompany",
                          "secret");

                    ldapConnection.SessionOptions.ProtocolVersion = 3;

                    ldapConnection.AuthType = AuthType.Basic;
                    ldapConnection.Bind(networkCredential);

                    var request = new AddRequest("ou=" + clientna + ",ou=" + objClient.City1 + ",ou=" + objClient.State1 + ",ou=" + objClient.Country1 + ",o=TopCompany", new DirectoryAttribute[] {
                   // var request = new AddRequest("ou=" + clientna + ",o=TopCompany", new DirectoryAttribute[] {
                    new DirectoryAttribute("ou", clientna),

                    new DirectoryAttribute("objectClass", new string[] { "top", "organizationalUnit"})
                });
                    ldapConnection.SendRequest(request);

                    #endregion

                    var ClientDetails = JsonConvert.SerializeObject(objClient);
                    var result = service.PostAPIWithData(ClientDetails, api);
                    ClientModel clientResult = JsonConvert.DeserializeObject<ClientModel>(result.Result);
                    if (objClient.ClientId == 0)
                    {
                        if (clientResult.ClientId != 0)
                        {
                            UserSessionManager.SetCaretakersList(this, null);
                            TempData["Success"] = "Branch details added Successfully.";
                            NotificationHub.Static_Send("notify", "New Client registered", "static");

                            api = "Users/SendVerificationEmail";
                            VerifyEmail verifyEmail = new VerifyEmail
                            {
                                WelcomeMsg = "Welcome to Tranquil care!",
                                FirstName = clientResult.ClientName,
                                MailMsg = "Thank you for registering with us, you have successfully created an account with us.",
                                Mailcontent = string.Format("{0}://{1}/Home/EmailVerificationSuccess/{2}", Request.Url.Scheme, Request.Url.Authority, HttpUtility.UrlEncode(StringCipher.EncodeNumber(Convert.ToInt32(clientResult.UserId)))),
                                ContactNo = "1-800-892-6066",
                                RegardsBy = "Tranquil Care Inc.",
                                siteUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/",
                                CompanyName_TagLine = "Tranquil Care - Delivering Care Excellence.",
                                CompanyName = "Tranquil Care Inc.",
                                Subject = "Email Verification Link",
                                Email = clientResult.EmailId
                            };
                            var serviceContent = JsonConvert.SerializeObject(verifyEmail);
                            var resultEmail = service.PostAPIWithData(serviceContent, api);
                            TempData["Success"] = "Branch details Added Successfully.";

                            return Redirect($"{Url.Action("FranchiseRegistration", "Franchise", new { clientId = clientResult.ClientId })}");
                        }
                        else
                        {
                            TempData["Failure"] = "Failed saving Branch details";
                        }
                    }
                    else
                    {
                        if (clientResult.ClientId != 0)
                        {
                            UserSessionManager.SetCaretakersList(this, null);
                            TempData["Success"] = "Branch details updated Successfully.";
                            return Redirect($"{Url.Action("FranchiseRegistration", "Franchise", new { clientId = clientResult.ClientId })}");
                        }
                        else
                        {
                            TempData["Failure"] = "Failed saving Branch details";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-SaveClientDetails");
                System.IO.File.WriteAllText(Server.MapPath("~/PDFError.txt"), ex.Message);
            }

            return RedirectToAction("FranchiseRegistration");
        }

        [HttpPost]
        public ActionResult UpdateFranchiseDetails(FranchiseModel objClient)
        {
            try
            {


                if (Request.Form["RegisterClient"] != null)
                {
                    if (Session["UserType"] == null)
                    {
                        TempData["Failure"] = Models.Constants.NotLoggedIn;
                        return RedirectToAction("Login", "Account");
                    }

                    int num = new Random().Next(1000, 9999);
                    string numm = Convert.ToString(num);
                    objClient.EmployeeNumber = numm;
                    string clientna = objClient.ClientName;
                    string userid = "5";
                    
                    string pswd = objClient.Password;
                    if (objClient.ClientId == 0)
                    {
                        string encryptionPassword = ConfigurationManager.AppSettings["EncryptPassword"].ToString();
                        string passwordEpt = StringCipher.Encrypt(objClient.Password, encryptionPassword);
                        objClient.Password = passwordEpt;
                    }
                    objClient.ClientShiftList = UserSessionManager.GetClientTimeShift(this);
                    objClient.ClientCaretakers = UserSessionManager.GetCaretakersList(this);
                    objClient.CategoryRates = UserSessionManager.GetCategoryRateList(this);

                    objClient.SiteURL = Request.Url.Scheme + "://" + Request.Url.Authority + "/";
                    string api = "Client/SaveFranchiseDetails";

                    #region Adding User to LDAP Created by Silpa


                    System.DirectoryServices.Protocols.LdapConnection ldapConnection = new System.DirectoryServices.Protocols.LdapConnection("127.0.0.1:10389");

                    var networkCredential = new NetworkCredential(
                          "employeeNumber=1,ou=Aluva,ou=Ernakulam,ou=kerala,ou=India,o=TopCompany",
                          "secret");

                    ldapConnection.SessionOptions.ProtocolVersion = 3;

                    ldapConnection.AuthType = AuthType.Basic;
                    ldapConnection.Bind(networkCredential);
                   // "employeeNumber=" + numm + ",ou=" + objCareTakerViewModel.Branch + ",ou=" + objCareTakerViewModel.City + ",ou=" + objCareTakerViewModel.State + ",ou=" + objCareTakerViewModel.Country + ",o=TopCompany"

                    var request = new AddRequest("ou=" + clientna + ",ou="+objClient.City1+",ou="+objClient.State1+",ou="+objClient.Country1+",o=TopCompany", new DirectoryAttribute[] {
                    new DirectoryAttribute("ou", clientna),

                    new DirectoryAttribute("objectClass", new string[] { "top", "organizationalUnit"})
                });
                    ldapConnection.SendRequest(request);

                    #endregion

                    var ClientDetails = JsonConvert.SerializeObject(objClient);
                    var result = service.PostAPIWithData(ClientDetails, api);
                    ClientModel clientResult = JsonConvert.DeserializeObject<ClientModel>(result.Result);
                    if (objClient.ClientId == 0)
                    {
                        if (clientResult.ClientId != 0)
                        {
                            UserSessionManager.SetCaretakersList(this, null);
                            TempData["Success"] = "Branch details added Successfully.";
                            NotificationHub.Static_Send("notify", "New Client registered", "static");

                            api = "Users/SendVerificationEmail";
                            VerifyEmail verifyEmail = new VerifyEmail
                            {
                                WelcomeMsg = "Welcome to Tranquil care!",
                                FirstName = clientResult.ClientName,
                                MailMsg = "Thank you for registering with us, you have successfully created an account with us.",
                                Mailcontent = string.Format("{0}://{1}/Home/EmailVerificationSuccess/{2}", Request.Url.Scheme, Request.Url.Authority, HttpUtility.UrlEncode(StringCipher.EncodeNumber(Convert.ToInt32(clientResult.UserId)))),
                                ContactNo = "1-800-892-6066",
                                RegardsBy = "Tranquil Care Inc.",
                                siteUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/",
                                CompanyName_TagLine = "Tranquil Care - Delivering Care Excellence.",
                                CompanyName = "Tranquil Care Inc.",
                                Subject = "Email Verification Link",
                                Email = clientResult.EmailId
                            };
                            var serviceContent = JsonConvert.SerializeObject(verifyEmail);
                            var resultEmail = service.PostAPIWithData(serviceContent, api);
                            TempData["Success"] = "Branch details Added Successfully.";

                            return Redirect($"{Url.Action("FranchiseRegistration", "Franchise", new { clientId = clientResult.ClientId })}");
                        }
                        else
                        {
                            TempData["Failure"] = "Failed saving Branch details";
                        }
                    }
                    else
                    {
                        if (clientResult.ClientId != 0)
                        {
                            UserSessionManager.SetCaretakersList(this, null);
                            TempData["Success"] = "Branch details updated Successfully.";
                            return Redirect($"{Url.Action("FranchiseRegistration", "Franchise", new { clientId = clientResult.ClientId })}");
                        }
                        else
                        {
                            TempData["Failure"] = "Failed saving Branch details";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-SaveClientDetails");
                System.IO.File.WriteAllText(Server.MapPath("~/PDFError.txt"), ex.Message);
            }

            return RedirectToAction("FranchiseRegistration");
        }

        /// <summary>
        /// method to get client details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult FranchiseSearch()
        {
            List<ClientModel> clientModel = null;
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
                        ViewBag.Error = Models.Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 8;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = Newtonsoft.Json.JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Models.Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowDelete = apiResults.AllowDelete;
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowView = apiResults.AllowView;
                    }
                    else if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowDelete = true;
                    }
                }
                else
                {
                    TempData["Failure"] = Models.Constants.NotLoggedIn;
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
                api = "Client/GetAllFranchiseDetailsByLocation";
                var clientSearchInputsContent = JsonConvert.SerializeObject(inputs);
                var result = service.PostAPIWithData(clientSearchInputsContent, api);
                clientModel = JsonConvert.DeserializeObject<List<ClientModel>>(result.Result);
                clientModel[0].CountryId1 = inputs.CountryId;
                clientModel[0].StateId1 = inputs.StateId;
                clientModel[0].CityId1 = inputs.CityId;
                ViewData["CurrencySymbol"] = clientModel.FirstOrDefault().CurrencySymbol;
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-ClientSearch");
            }
            return View(clientModel);
        }

        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult FranchiseDetails(int clientId)
        {
            ClientModel clientModelObj = new ClientModel();
            try
            {

                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Models.Constants.NoViewPrivilege;
                        return View();
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 7;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = Newtonsoft.Json.JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Models.Constants.NoViewPrivilege;
                            return View();
                        }

                        ViewBag.AllowDelete = apiResults.AllowDelete;
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
                    TempData["Failure"] = Models.Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                api = "Client/GetAllFranchiseDetailsById?clientId=" + clientId;
                var result = service.GetAPI(api);
                clientModelObj = JsonConvert.DeserializeObject<ClientModel>(result);
                clientModelObj.ClientId = clientId;
                clientModelObj.ClientCaretakerMaps = clientModelObj.ClientCaretakerMaps.FindAll(x => x.EffectiveFrom < DateTime.Now && x.Rate > 0F).ToList();

                ViewData["CurrencySymbol"] = clientModelObj.CurrencySymbol;

                UserSessionManager.SetCaretakersList(this, clientModelObj.ClientCaretakers);
                UserSessionManager.SetClientTimeShift(this, clientModelObj.ClientShiftList);
                UserSessionManager.SetCategoryRateList(this, clientModelObj.CategoryRates);


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


                List<CountryViewModel> countryList = new List<CountryViewModel>();
                api = "Admin/GetCountryDetails/0";
                var countrylist = service.GetAPI(api);
                countryList = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist);
                ViewData["ListCountry"] = new SelectList(countryList, "CountryId", "Name", clientModelObj.CountryId2);

                List<StateViewModel> stateList = new List<StateViewModel>();
                api = "/Admin/GetStatesByCountryId/1";
                var stateResult = service.GetAPI(api);
                stateList = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult);
                ViewData["StateList"] = new SelectList(stateList, "StateId", "Name", clientModelObj.StateId2);

                api = "Admin/GetCity?flag=*&value=''";
                List<Cities> cityList = new List<Cities>();
                var resultCity = service.GetAPI(api);
                cityList = JsonConvert.DeserializeObject<List<Cities>>(resultCity);
                ViewData["ListCity"] = new SelectList(cityList, "CityId", "CityName", clientModelObj.CityId2);

                api = "/Admin/GetTimeShiftDetails/0";
                var timeresult = service.GetAPI(api);
                var timeShiftList = JsonConvert.DeserializeObject<List<ClientShiftDetails>>(timeresult);
                ViewData["ListTimeShift"] = new SelectList(timeShiftList, "TimeShiftId", "TimeShiftName", 0);

                api = "/Admin/GetCategory?flag=*&value=''";
                List<CategoryViewModel> categoryList = new List<CategoryViewModel>();
                var resultList = service.GetAPI(api);
                categoryList = JsonConvert.DeserializeObject<List<CategoryViewModel>>(resultList);
                var listCategory = new SelectList(categoryList, "CategoryId", "Name", 0);
                ViewData["ListCategory"] = listCategory;

                api = "Admin/SelectCaretakersByCaretakerStatus/" + (int)CaretakerStatus.Approved;
                List<CareTakerRegistrationViewModel> Caretakers = new List<CareTakerRegistrationViewModel>();
                var caretakerResult = service.GetAPI(api);
                Caretakers = JsonConvert.DeserializeObject<List<CareTakerRegistrationViewModel>>(caretakerResult);
                List<CareTakerRegistrationViewModel> CaretakersToLoad = new List<CareTakerRegistrationViewModel>();
                CaretakersToLoad = Caretakers.Where(a => a.UserStatus == Enums.UserStatus.Active).ToList<CareTakerRegistrationViewModel>();
                List<CareTakerRegistrationViewModel> clientCaretakers = new List<CareTakerRegistrationViewModel>();
                clientCaretakers = CaretakersToLoad;
                foreach (var care in Caretakers.ToList())
                {
                    foreach (var item in clientModelObj.ClientCaretakers)
                    {
                        if (item.CaretakerId == care.UserId)
                        {
                            clientCaretakers.RemoveAll(x => x.UserId == item.CaretakerId);
                        }
                    }
                }
                ViewData["ListCaretakers"] = clientCaretakers;
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-ClientDetails");
                System.IO.File.WriteAllText(Server.MapPath("~/PDFError.txt"), ex.Message);
            }
            return View(clientModelObj);
        }


        public ActionResult FranchiseCommissionDetails()
       {
            ClientModel clientModelObj = new ClientModel();
            string country = Session["CountryId"].ToString();
            string state = Session["StateID"].ToString();
            string CityIdk = Session["CityIdk"].ToString();
            string api = string.Empty;
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
                    ViewBag.Error = Models.Constants.NoViewPrivilege;
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
                        ViewBag.Error = Models.Constants.NoViewPrivilege;
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
                TempData["Failure"] = Models.Constants.NotLoggedIn;
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

            YearList = new SelectList(years, "Id", "Year", DateTime.Now.Year); ;
            return YearList;
        }


        #endregion



    }


}