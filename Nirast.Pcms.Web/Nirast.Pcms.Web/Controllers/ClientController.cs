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
    public class ClientController : Controller
    {
        PCMSLogger pCMSLogger = new PCMSLogger();
        Service service = null;

        public ClientController()
        {
            service = new Service(pCMSLogger);
        }

        #region ClientRegistration
        // GET: Client
        public ActionResult ClientRegistration()
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

        public JsonResult LoadCaretakersByCategory(int categoryId)
        {
            try
            {
                HomeController home = new HomeController();
                var inputs = new LocationSearchInputs();
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
                string api = "CareTaker/GetCareTakerListByCategoryAndLocation/" + categoryId;
                var CareTakerResult = service.PostAPIWithData(clientSearchInputsContent, api);
                return Json(CareTakerResult.Result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-LoadCaretakersByCategory");
                return null;
            }
        }

        public JsonResult LoadCaretakersByCategoryAndLocation(int categoryId, LocationSearchInputs inputs)
        {
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
                string api = "CareTaker/GetCareTakerListByCategoryAndLocation/" + categoryId;
                var CareTakerResult = service.PostAPIWithData(clientSearchInputsContent, api);
                return Json(CareTakerResult.Result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-LoadCaretakersByCategory");
                return null;
            }
        }

        public JsonResult LoadCaretakersByService(int serviceid)
        {
            try
            {
                string api = "CareTaker/GetCaretakersByService/" + serviceid;
                var CareTakerResult = service.GetAPI(api);
                return Json(CareTakerResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-LoadCaretakersByCategory");
                return null;
            }
        }

        public JsonResult LoadCaretakersByCategoryAndClientId(int categoryId, int clientId)
        {
            try
            {
                string api = "CareTaker/GetCareTakerListByCategoryAndClientId/" + categoryId + "?clientId=" + clientId;
                var CareTakerResult = service.GetAPI(api);
                return Json(CareTakerResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-LoadCaretakersByCategory");
                return null;
            }
        }

        public JsonResult LoadCaretakersByCategoryAndDate(int categoryId, string startDatetime, int clientId, int totalhours = 0)
        {
            try
            {
                string api = "CareTaker/GetCareTakerListByCategoryAndDateTime/" + categoryId + "?startDatetime=" + startDatetime + "&hours=" + totalhours + "&clientId=" + clientId;
                var CareTakerResult = service.GetAPI(api);
                return Json(CareTakerResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-LoadCaretakersByCategory");
                return null;
            }
        }

        public JsonResult LoadAvailableCaretakersByCategoryAndDate(int categoryId, string startDatetime, int clientId, int Workshift, int totalhours = 0)
        {
            try
            {

                string api = "CareTaker/GetAvailableCareTakerListByCategoryAndDateTime/" + categoryId + "?startDatetime=" + startDatetime + "&hours=" + totalhours + "&clientId=" + clientId + "&Workshift=" + Workshift;
                pCMSLogger.Error("API Call start " + DateTime.Now.ToString() + " - " + api);
                var CareTakerResult = service.GetAPI(api);
                pCMSLogger.Error("API Call end " + DateTime.Now.ToString() + " - " + api);
                return Json(CareTakerResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-LoadCaretakersByCategory");
                return null;
            }
        }

        public JsonResult AvailableCaretakersReport(int categoryId, string FromTime, string Totime)
        {
            try
            {
                CaretakerAvailableReport lstCaretakers = new CaretakerAvailableReport();
                string api = "CareTaker/GetAvailableCareTakerListReport/" + categoryId + "?FromTime=" + FromTime + "&Totime=" + Totime;
                var CareTakerResult = service.GetAPI(api);
                lstCaretakers = JsonConvert.DeserializeObject<CaretakerAvailableReport>(CareTakerResult);
                return Json(CareTakerResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-LoadCaretakersByCategory");
                return null;
            }
        }

        public JsonResult LoadCaretakersByClientID(int categoryId)
        {
            try
            {
                ClientModel clientModelObj = new ClientModel();
                string api = "Client/GetAllClientDetailsById?clientId=" + categoryId;
                var CareTakerResult = service.GetAPI(api);
                clientModelObj = JsonConvert.DeserializeObject<ClientModel>(CareTakerResult);
                CareTakerResult = clientModelObj.ClientCaretakers.ToString();
                ViewData["CurrencySymbol"] = clientModelObj.CurrencySymbol;
                var json = JsonConvert.SerializeObject(clientModelObj.ClientCaretakers);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-LoadCaretakersByClientID");
                return null;
            }

        }
        public JsonResult LoadInvoiceByClientID(int clientId)
        {
            try
            {
                ClientModel clientModelObj = new ClientModel();
                string api = "Client/GetAllClientDetailsById?clientId=" + clientId;
                var CareTakerResult = service.GetAPI(api);
                clientModelObj = JsonConvert.DeserializeObject<ClientModel>(CareTakerResult);


                return Json(clientModelObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-LoadCaretakersByClientID");
                return null;
            }

        }

        [HttpPost]
        public JsonResult AddClientTimeShift(string timeShiftId, string timeShiftName)
        {
            List<ClientShiftDetails> objClientTimeShiftList = UserSessionManager.GetClientTimeShift(this);
            try
            {
                List<ClientShiftDetails> objClientTimeShiftListSaearch = objClientTimeShiftList.Where(x => x.TimeShiftId == Convert.ToInt32(timeShiftId)).ToList();
                if (objClientTimeShiftListSaearch.Count == 0)
                {
                    ClientShiftDetails objClientTimeShift = new ClientShiftDetails();
                    objClientTimeShift.TimeShiftId = Convert.ToInt32(timeShiftId);
                    objClientTimeShift.TimeShiftName = timeShiftName;
                    UserSessionManager.AddClientTimeShift(this, objClientTimeShift);
                }
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-AddClientTimeShift");
                System.IO.File.WriteAllText(Server.MapPath("~/PDFError.txt"), ex.Message);
            }
            objClientTimeShiftList = UserSessionManager.GetClientTimeShift(this);
            return Json(objClientTimeShiftList);
        }

        [HttpPost]
        public JsonResult GetMappedCaretakerRates(ClientCaretakers clientCaretakers)
        {
            string api = "Client/GetMappedCaretakerRates/" + clientCaretakers.ClientId + "/" + clientCaretakers.CaretakerId;
            var result = service.GetAPI(api);
            return Json(result);
        }
        [HttpPost]
        public JsonResult GetMappedCaretakersLatestPayRiseRates(int clientId, int caretakerId)
        {
            string api = "Client/GetMappedCaretakersLatestPayRiseRates/" + clientId + "/" + caretakerId;
            var result = service.GetAPI(api);
            return Json(result);
        }
        [HttpPost]
        public JsonResult GetMappedCaretakersPayRiseRatesByDate(PayriseData payriseData)
        {
            string api = "Client/GetMappedCaretakersPayRiseRatesByDate";
            var deleteData = JsonConvert.SerializeObject(payriseData);
            var result = service.PostAPIWithData(deleteData, api);
            return Json(result.Result);
        }
        [HttpPost]
        public JsonResult GetCategoryClientPayRiseRates(ClientInvoicePayriseData clientPayRise)
        {
            string api = "Client/GetCategoryClientPayRiseRates/" + clientPayRise.ClientId;
            var result = service.GetAPI(api);
            return Json(result);
        }
        public ActionResult DeleteClientTimeShiftDetail(int TimeShiftId)
        {
            try
            {

                string api = "Admin/DeleteClientTimeShiftDetail/" + TimeShiftId;
                HttpStatusCode result = service.PostAPI(null, api);
                if (result == HttpStatusCode.OK)
                {
                    TempData["Success"] = "Time shift deleted successfully";
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    TempData["Failure"] = "You are not authorized to perform this action.";
                }
                else
                {
                    TempData["Failure"] = "Deletion Failed.";
                }
                return RedirectToAction("TimeShiftSettings", "Admin");
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-DeleteClientTimeShiftDetail");
                System.IO.File.WriteAllText(Server.MapPath("~/PDFError.txt"), ex.Message); throw;
            }
        }
        [HttpPost]
        public JsonResult RemoveClientTimeShift(string timeShiftId)
        {
            try
            {
                List<ClientShiftDetails> objClientTimeShiftList = UserSessionManager.GetClientTimeShift(this);
                int shiftId = Convert.ToInt32(timeShiftId);
                objClientTimeShiftList = objClientTimeShiftList.Where(x => x.TimeShiftId != shiftId).ToList();
                UserSessionManager.SetClientTimeShift(this, objClientTimeShiftList);
                return Json(objClientTimeShiftList);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-RemoveClientTimeShift");
                System.IO.File.WriteAllText(Server.MapPath("~/PDFError.txt"), ex.Message); throw;
            }
        }

        [HttpPost]
        public JsonResult AddCareTakerList(string caretakerId, string careTakerName, string categoryTypeId, string serviceRate, string categoryName)
        {
            List<ClientCaretakers> objClientCaretaker = UserSessionManager.GetCaretakersList(this);
            try
            {
                List<ClientCaretakers> objClientCaretakerListSaearch = objClientCaretaker.Where(x => x.CaretakerId == Convert.ToInt32(caretakerId)).ToList();
                if (objClientCaretakerListSaearch.Count == 0)
                {
                    ClientCaretakers objCareTaker = new ClientCaretakers();
                    objCareTaker.CaretakerId = Convert.ToInt32(caretakerId);
                    objCareTaker.CareTakerName = careTakerName;
                    objCareTaker.CategoryName = categoryName;
                    objCareTaker.CategoryTypeId = Convert.ToInt32(categoryTypeId);
                    objCareTaker.Rate = float.Parse(serviceRate);
                    UserSessionManager.AddCaretakersList(this, objCareTaker);
                }
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-AddCareTakerList");
                System.IO.File.WriteAllText(Server.MapPath("~/PDFError.txt"), ex.Message); throw;
            }

            List<ClientCaretakers> objCareTakerList = UserSessionManager.GetCaretakersList(this);
            objCareTakerList = UserSessionManager.GetCaretakersList(this);
            return Json(objCareTakerList);
        }

        [HttpPost]
        public JsonResult AddCategoryandRateList(string categoryTypeId, string serviceRate, string taxApplicable, string categoryName, DateTime? EffectiveFrom, int ClientId)
        {
            List<ClientCategoryRate> objCareTakerList = UserSessionManager.GetCategoryRateList(this);
            ClientCategoryRate objCareTaker = new ClientCategoryRate();
            try
            {
                var count = objCareTakerList.Count(x => x.CategoryId == Convert.ToInt32(categoryTypeId));
                if (count > 0)
                {
                    objCareTakerList.RemoveAll(x => x.CategoryId == Convert.ToInt32(categoryTypeId));
                }
                objCareTaker.CategoryName = categoryName;
                objCareTaker.CategoryId = Convert.ToInt32(categoryTypeId);
                objCareTaker.Rate = float.Parse(serviceRate);
                objCareTaker.IsTaxApplicable = taxApplicable == "Y" ? true : false;
                objCareTaker.EffectiveFrom = EffectiveFrom;
                objCareTaker.ClientId = ClientId;
                UserSessionManager.AddCategoryRateList(this, objCareTaker);

                objCareTakerList = UserSessionManager.GetCategoryRateList(this);

            }

            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-AddCareTakerList");
                System.IO.File.WriteAllText(Server.MapPath("~/PDFError.txt"), ex.Message); throw ex;
            }
            return Json(objCareTakerList);

            //ClientCategoryRate objCareTakerListSaearch = objCareTakerList.Find(x => x.CategoryId == Convert.ToInt32(categoryTypeId));
            //if (objCareTakerListSaearch == null)
            //{
            //    ClientCategoryRate objCareTaker = new ClientCategoryRate();
            //    objCareTaker.CategoryName = categoryName;
            //    objCareTaker.CategoryId = Convert.ToInt32(categoryTypeId);
            //    objCareTaker.Rate = float.Parse(serviceRate);
            //    objCareTaker.IsTaxApplicable = taxApplicable == "Y" ? true : false;
            //    UserSessionManager.AddCategoryRateList(this, objCareTaker);
            //    objCareTakerList = UserSessionManager.GetCategoryRateList(this);
            //}
            //else
            //{
            //    objCareTakerListSaearch.CategoryName = categoryName;
            //    objCareTakerListSaearch.CategoryId = Convert.ToInt32(categoryTypeId);
            //    objCareTakerListSaearch.Rate = float.Parse(serviceRate);
            //    objCareTakerListSaearch.IsTaxApplicable = taxApplicable == "Y" ? true : false;
            //    UserSessionManager.AddCategoryRateList(this, objCareTakerListSaearch);
            //    objCareTakerList = UserSessionManager.GetCategoryRateList(this);
            //}
            //    return Json(objCareTakerList);
            //}
            //catch (Exception ex)
            //{
            //    pCMSLogger.Error(ex, "Error occurred in Client Controller-AddCareTakerList");
            //    System.IO.File.WriteAllText(Server.MapPath("~/PDFError.txt"), ex.Message); throw ex;
            //}
        }

        [HttpPost]
        public JsonResult RemoveCategoryandRateList(string categoryTypeId)
        {
            try
            {

                List<ClientCategoryRate> objCareTakersList = UserSessionManager.GetCategoryRateList(this);
                Session["Date"] = objCareTakersList[0].EffectiveFrom;
                int CategoryId = Convert.ToInt32(categoryTypeId);
                objCareTakersList = objCareTakersList.Where(x => x.CategoryId != CategoryId).ToList();
                UserSessionManager.SetCategoryRateList(this, objCareTakersList);
                return Json(objCareTakersList);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-RemoveCareTakerList");
                System.IO.File.WriteAllText(Server.MapPath("~/PDFError.txt"), ex.Message); throw ex;
            }
        }


        [HttpPost]
        public JsonResult RemoveCareTakerList(string caretakerId)
        {
            try
            {

                List<ClientCaretakers> objCareTakersList = UserSessionManager.GetCaretakersList(this);
                int CaretakerId = Convert.ToInt32(caretakerId);
                objCareTakersList = objCareTakersList.Where(x => x.CaretakerId != CaretakerId).ToList();
                UserSessionManager.SetCaretakersList(this, objCareTakersList);
                return Json(objCareTakersList);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-RemoveCareTakerList");
                System.IO.File.WriteAllText(Server.MapPath("~/PDFError.txt"), ex.Message); throw ex;
            }
        }

        [HttpPost]
        public ActionResult SaveClientDetails(ClientModel objClient)
        {
            try
            {


                if (Request.Form["RegisterClient"] != null)
                {
                    string pswd = objClient.Password;
                    if (Session["UserType"] == null)
                    {
                        TempData["Failure"] = Models.Constants.NotLoggedIn;
                        return RedirectToAction("Login", "Account");
                    }
                    if (objClient.ClientId == 0)
                    {
                        string encryptionPassword = ConfigurationManager.AppSettings["EncryptPassword"].ToString();
                        string passwordEpt = StringCipher.Encrypt(objClient.Password, encryptionPassword);
                        objClient.Password = passwordEpt;
                    }
                    objClient.ClientShiftList = UserSessionManager.GetClientTimeShift(this);
                    objClient.ClientCaretakers = UserSessionManager.GetCaretakersList(this);
                    objClient.CategoryRates = UserSessionManager.GetCategoryRateList(this);
                    int num = new Random().Next(1000, 9999);
                    string numm = Convert.ToString(num);
                    objClient.EmployeeNumber = numm;
                    string clientna = objClient.ClientName;
                    string userid = "3";
                   
                    objClient.SiteURL = Request.Url.Scheme + "://" + Request.Url.Authority + "/";
                    string api = "Client/SaveClientDetails";
               
                    var ClientDetails = JsonConvert.SerializeObject(objClient);
                    var result = service.PostAPIWithData(ClientDetails, api);
                    ClientModel clientResult = JsonConvert.DeserializeObject<ClientModel>(result.Result);

                    #region Adding User to LDAP Created by Silpa
                    
                    System.DirectoryServices.Protocols.LdapConnection ldapConnection = new System.DirectoryServices.Protocols.LdapConnection("127.0.0.1:10389");

                    var networkCredential = new NetworkCredential(
                          "employeeNumber=1,ou=Aluva,ou=Ernakulam,ou=kerala,ou=India,o=TopCompany",
                          "secret");

                    ldapConnection.SessionOptions.ProtocolVersion = 3;

                    ldapConnection.AuthType = AuthType.Basic;
                    ldapConnection.Bind(networkCredential);


                    var request = new AddRequest("employeeNumber=" + numm + ",ou="+objClient.Branch1+",ou="+objClient.City1+",ou="+objClient.State1+",ou="+objClient.Country1+",o=TopCompany", new DirectoryAttribute[] {
                    new DirectoryAttribute("employeeNumber", numm),
                    new DirectoryAttribute("cn",objClient.EmailId),
                      new DirectoryAttribute("uid", userid),
                     new DirectoryAttribute("sn", clientna),
                    new DirectoryAttribute("userPassword", pswd),
                    new DirectoryAttribute("objectClass", new string[] { "top", "person", "organizationalPerson","inetOrgPerson" })
                });
                    ldapConnection.SendRequest(request);

                    #endregion




                //    #region Created by silpa on 03-01-2022 On group



                //    System.DirectoryServices.Protocols.LdapConnection ldapConnection1 = new System.DirectoryServices.Protocols.LdapConnection("127.0.0.1:10389");

                //    var networkCredential1 = new NetworkCredential(
                //          "employeeNumber=1,ou=Aluva,ou=Ernakulam,ou=kerala,ou=India,o=TopCompany",
                //          "secret");
                //    var unique = "";
                  
                //        unique = "employeeNumber=" + numm + ",ou=" + objClient.Branch1 + ",ou=" + objClient.City1 + ",ou=" + objClient.State1 + ",ou=" + objClient.Country1 + ",o=TopCompany";
                  

                //    ldapConnection1.SessionOptions.ProtocolVersion = 3;

                //    ldapConnection1.AuthType = AuthType.Basic;
                //    ldapConnection1.Bind(networkCredential1);

                
                //    var request1 = new AddRequest("cn=" + clientna + ",ou=Client,o=TopCompany", new DirectoryAttribute[] {

                //    new DirectoryAttribute("cn", clientna),


                //     new DirectoryAttribute("uniqueMember", unique),
                //    new DirectoryAttribute("objectClass", new string[] { "top", "groupOfUniqueNames",})
                //});
                //    ldapConnection1.SendRequest(request1);



                //    #endregion



                    if (objClient.ClientId == 0)
                    {
                        if (clientResult.ClientId != 0)
                        {
                            UserSessionManager.SetCaretakersList(this, null);
                            TempData["Success"] = "Client details added Successfully.";
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
                            return Redirect($"{Url.Action("ClientDetails", "Client", new { clientId = clientResult.ClientId })}");
                        }
                        else
                        {
                            TempData["Failure"] = "Failed saving client details";
                        }
                    }
                    else
                    {
                        if (clientResult.ClientId != 0)
                        {
                            UserSessionManager.SetCaretakersList(this, null);
                            TempData["Success"] = "Client details updated Successfully.";
                            return Redirect($"{Url.Action("ClientDetails", "Client", new { clientId = clientResult.ClientId })}");
                        }
                        else
                        {
                            TempData["Failure"] = "Failed saving client details";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-SaveClientDetails");
                System.IO.File.WriteAllText(Server.MapPath("~/PDFError.txt"), ex.Message);
            }

            return RedirectToAction("ClientSearch");
        }


        #endregion

        public ActionResult ModifyClientStatus(int clientId, int status)
        {
            ClientModel clientModelObj = new ClientModel();
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        TempData["Failure"] = Models.Constants.NoViewPrivilege;
                        return RedirectToAction("ClientSearch");
                    }
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 8;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowDelete)
                        {
                            TempData["Failure"] = Models.Constants.NoActionPrivilege;
                            return RedirectToAction("ClientSearch");
                        }

                    }
                }
                else
                {
                    TempData["Failure"] = Models.Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                api = "Client/ModifyClientStatusById/" + clientId + "/" + status;
                var serviceContent = JsonConvert.SerializeObject(clientId);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    TempData["Success"] = "Client status updated Successfully.";
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    TempData["Failure"] = "You are not authorized to perform this action.";
                }
                else
                {
                    TempData["Failure"] = "Client status updated Failed.";
                }
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-DeleteClient");
                System.IO.File.WriteAllText(Server.MapPath("~/PDFError.txt"), ex.Message);
            }
            return RedirectToAction("ClientSearch");
        }


        public ActionResult ChangeEmailStatus(int clientId, int emailstatus)
        {
            try
            {
                string api = string.Empty;
             
                api = "Client/ChangeClientEmailStatus/" + clientId + "/" + emailstatus;
                var serviceContent = JsonConvert.SerializeObject(clientId);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    TempData["Success"] = "Email status updated Successfully.";
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    TempData["Failure"] = "You are not authorized to perform this action.";
                }
                else
                {
                    TempData["Failure"] = "Client Emailstatus update Failed.";
                }
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-DeleteClient");
                System.IO.File.WriteAllText(Server.MapPath("~/PDFError.txt"), ex.Message);
            }
            return RedirectToAction("ClientSearch");
        }


        public ActionResult RegenerateInvoiceDetails(int InvoiceSearchInputId)
        {
            List<InvoiceSearchInpts> scheduleDetailsListFilterd = new List<InvoiceSearchInpts>();
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

                List<ClientModel> scheduleDetailsList = new List<ClientModel>();
                List<ClientModel> activeClient = new List<ClientModel>();
                List<InvoiceSearchInpts> apiResult = new List<InvoiceSearchInpts>();
                InvoiceHistory searchInputs = new InvoiceHistory();
                api = "Client/GetInvoiceHistoryList";
                searchInputs.InvoiceSearchInputId = InvoiceSearchInputId;
                var advancedSearchInputModel = JsonConvert.SerializeObject(searchInputs);
                var result = service.PostAPIWithData(advancedSearchInputModel, api);
                apiResult = JsonConvert.DeserializeObject<List<InvoiceSearchInpts>>(result.Result);

                //scheduleDetailsListFilterd = apiResult.Where(a => a.InvoiceSearchInputId == InvoiceSearchInputId).ToList();
                scheduleDetailsListFilterd = apiResult;
                


                api = "Client/GetAllClientDetails";
                var Clients = service.GetAPI(api);
                scheduleDetailsList = JsonConvert.DeserializeObject<List<ClientModel>>(Clients);
               
                activeClient = scheduleDetailsList.Where(a => a.ClientStatus == ClientStatus.Active).ToList();
                ViewData["Client"] = new SelectList(activeClient, "ClientId", "ClientName", scheduleDetailsListFilterd.FirstOrDefault().ClientId);
               
                var listPaySearch = new SelectList(new[]
                {
                new { ID = "0", Name = "--Select--" },
                new { ID = "1", Name = "Yearly" },
                new { ID = "2", Name = "Monthly" },
                new { ID = "3", Name = "Date Range" },
            },
                "ID", "Name", 0);
                ViewData["listPaySearch"] = listPaySearch;
                ViewData["listYears"] = GetYearsCustom();
                Months months = new Months();
                ViewData["listMonths"] = months.GetMonths();
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-RegenerateInvoiceDetails");
                System.IO.File.WriteAllText(Server.MapPath("~/PDFError.txt"), ex.Message);
            }
            return View(scheduleDetailsListFilterd.FirstOrDefault());
        }
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult ClientDetails(int clientId)
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
                api = "Client/GetAllClientDetailsById?clientId=" + clientId;
                var result = service.GetAPI(api);
                clientModelObj = JsonConvert.DeserializeObject<ClientModel>(result);

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

        /// <summary>
        /// method to search client details
        /// </summary>
        /// <param name="clientName"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Client/ClientSearch")]
        public ActionResult ClientSearch(string clientName, string location)
        {
            ClientSearchInputs clientSearchInputs = new ClientSearchInputs();
            List<ClientModel> clientModel = null;
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
                api = "Client/SearchClient";
                clientSearchInputs.ClientName = clientName;
                clientSearchInputs.Location = location;
                var clientSearchInputsContent = JsonConvert.SerializeObject(clientSearchInputs);
                var result = service.PostAPIWithData(clientSearchInputsContent, api);
                clientModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ClientModel>>(result.Result);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-ClientSearch");
            }
            return View(clientModel);
        }

       
        /// <summary>
        /// method to get client details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ClientSearch()
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
                    if(country!="" &&state!=""&&CityIdk!="")
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
                    else {
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
                    if(userRoleDetails.WorkRoleId==4)
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
                api = "Client/GetAllClientDetailsByLocation";
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

        [HttpPost]
        [Route("Client/ClientSearchByLocation")]
        public ActionResult ClientSearchByLocation(LocationSearchInputs inputs)
        {
            List<ClientModel> clientModel = null;
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
                api = "Client/GetAllClientDetailsByLocation";
                var clientSearchInputsContent = JsonConvert.SerializeObject(inputs);
                var result = service.PostAPIWithData(clientSearchInputsContent, api);
                clientModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ClientModel>>(result.Result);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-ClientSearch");
            }
            return PartialView("_ClientSearchPartial", clientModel);
        }

        public JsonResult DeleteClient(int clientId, int status)
        {
            Service service = new Service();
            string api = "Client/ModifyClientStatusById/" + clientId + "/" + status;
            HttpStatusCode result = service.PostAPI(null, api);
            if (result == HttpStatusCode.OK && status == 3)
            {
                TempData["Success"] = "Client Details Deleted Successfully";
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


        public JsonResult DeleteFranchise(int clientId, int status)
        {
            Service service = new Service();
            string api = "Client/ModifyFranchiseStatusById/" + clientId + "/" + status;
            HttpStatusCode result = service.PostAPI(null, api);
            if (result == HttpStatusCode.OK && status == 3)
            {
                TempData["Success"] = "Franchise Details Deleted Successfully";
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


        [HttpGet]
        public ActionResult ScheduleRejectedCaretaker()
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
                        api = "/Admin/GetStatesByCountryId/"+country;
                        var stateResult1 = service.GetAPI(api);
                        stateList1 = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult1);
                        ViewData["StateList1"] = new SelectList(stateList1, "StateId", "Name", country);

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
                var clientSearchInputsContent = JsonConvert.SerializeObject(inputs);
                string caretakerapi = "CareTaker/RetrieveCareTakerListForDdlByLocation/";
                var CareTakerResult = service.PostAPIWithData(clientSearchInputsContent, caretakerapi);
                List<Caretakers> listCaretakers = new List<Caretakers>();
                listCaretakers = JsonConvert.DeserializeObject<List<Caretakers>>(CareTakerResult.Result);
                ViewBag.Caretaker = new SelectList(listCaretakers, "CaretakerId", "CareTakerName", 0);
                //ViewData["CurrencySymbol"] = clientModel.FirstOrDefault().CurrencySymbol;

            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-ClientSearch");
            }
            return View();
        }
        [HttpPost]
        public ActionResult ScheduleRejectedCaretaker(BookingHistorySearch bookingHistorySearch)
        {
           
            List<RejectedCaretaker> rejectedCaretaker = null;
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
                api = "Client/GetAllScheduleRejectedCaretaker";
                var searchInputModel = JsonConvert.SerializeObject(bookingHistorySearch);
                var result = service.PostAPIWithData(searchInputModel, api);
                //var result = service.GetAPI(api);
                rejectedCaretaker = JsonConvert.DeserializeObject<List<RejectedCaretaker>>(result.Result);
                //ViewData["CurrencySymbol"] = clientModel.FirstOrDefault().CurrencySymbol;
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-ClientSearch");
            }
            return PartialView("_ScheduleRejectedCaretakerPartial", rejectedCaretaker);
        }
        

        //Added by Jyothi -25-03-2022
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

        public ActionResult ScheduledCalenderView()
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


                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
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
                        ViewBag.Error = Models.Constants.NoViewPrivilege;
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
                            ViewBag.Error = Models.Constants.NoViewPrivilege;
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
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-ScheduledCalenderView");
                return null;
            }
        }

        public ActionResult ClientScheduleCalender()
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
                        RoleModulePrivileges apiResults = Newtonsoft.Json.JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Models.Constants.NoViewPrivilege;
                            return View();
                        }
                    }
                }
                return PartialView("_AdminCalendarPartial");
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-ClientScheduleCalender");
                return null;
            }
        }

        [HttpPost]
        public ActionResult MapCaretakers(List<WorkShiftRates> workshiftRates)
        {
            try
            {
                HttpStatusCode result = HttpStatusCode.OK;

                Service service = new Service();
                string api = "Client/SaveClientCareTakerPayRise";
                workshiftRates.ForEach(x => x.MapStatus = (int)MapStatus.Map);
                var serviceContent = JsonConvert.SerializeObject(workshiftRates);
                result = service.PostAPI(serviceContent, api);
                return new JsonResult { Data = new { status = (result == HttpStatusCode.OK) } };
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-MapCaretakers");
                return new JsonResult { Data = new { status = false } };
            }
        }
        [HttpPost]
        public ActionResult SavePayRisePayrol(List<WorkShiftRates> workShiftRates)
        {
            try
            {
                HttpStatusCode result = HttpStatusCode.OK;

                Service service = new Service();
                string api = "Client/SaveClientCareTakerPayRise";
                workShiftRates.ForEach(x => x.MapStatus = (int)MapStatus.Map);
                var serviceContent = JsonConvert.SerializeObject(workShiftRates);
                result = service.PostAPI(serviceContent, api);
                return new JsonResult { Data = new { status = (result == HttpStatusCode.OK) } };
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-MapCaretakers");
                return new JsonResult { Data = new { status = false } };
            }
        }

        [HttpPost]
        public ActionResult SavePayRiseInvoice(List<ClientCategoryRate> categoryRate)
        {
            try
            {
                HttpStatusCode result = HttpStatusCode.OK;

                Service service = new Service();
                string api = "Client/SaveClientcategoryCareTakerPayRise";
                var serviceContent = JsonConvert.SerializeObject(categoryRate);
                result = service.PostAPI(serviceContent, api);
                return new JsonResult { Data = new { status = (result == HttpStatusCode.OK) } };
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-MapCaretakers");
                return new JsonResult { Data = new { status = false } };
            }
        }
        public ActionResult UnMapCaretakers(int ClientId, int CaretakerId)
        {
            try
            {
                HttpStatusCode result = HttpStatusCode.OK;
                ClientCaretakers cc = new ClientCaretakers
                {
                    CaretakerId = CaretakerId,
                    ClientId = ClientId
                };
                Service service = new Service();
                string api = "Client/DeleteClientCareTakerMapping";
                var serviceContent = JsonConvert.SerializeObject(cc);
                result = service.PostAPI(serviceContent, api);
                return new JsonResult { Data = new { status = (result == HttpStatusCode.OK) } };
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-UnMapCaretakers");
                return new JsonResult { Data = new { status = false } };
            }
        }

        public ActionResult RejectCaretaker(RejectedCaretaker[] data)
        {
            try
            {
                HttpStatusCode result = HttpStatusCode.OK;
                foreach (var item in data)
                {
                    RejectedCaretaker cc = new RejectedCaretaker();
                    cc.CareTakerId = item.CareTakerId;
                    //cc.DateTime = DateTime.Now;
                    cc.Description = item.Description;
                    cc.ClientId = item.ClientId;
                    cc.ScheduleDate = item.ScheduleDate;
                    cc.Workshift = item.Workshift;
                    Service service = new Service();
                    string api = "Client/SaveScheduleRejectedCareTaker";
                    var serviceContent = JsonConvert.SerializeObject(cc);
                    result = service.PostAPI(serviceContent, api);
                }
                return new JsonResult { Data = new { status = (result == HttpStatusCode.OK) } };
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-UnMapCaretakers");
                return new JsonResult { Data = new { status = false } };
            }
        }

        public ActionResult SaveEvent(ScheduledData data)
        {
            try
            {
                if (Session["UserType"] == null)
                {
                    TempData["Failure"] = Models.Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                string startTime;
                string endTime;
                DateTime actualStart = DateTime.Now;
                DateTime actualEnd = DateTime.Now;
                DateTime reportStart = DateTime.Now;
                DateTime reportend = DateTime.Now;
                string[] workTimeValues = null;
                string[] workShiftVlaues = null;
                if (data.WorkTimeDetails != null)//work time values from drop down. in wrok time dropdown 
                {
                    workTimeValues = data.WorkTimeDetails.Split('|');
                }
                if (data.WorkShifDetails != null)//work time values from drop down. in wrok time dropdown 
                {
                    workShiftVlaues = data.WorkShifDetails.Split('|');
                }
                //Client_Scheduling start date end date settings starts here
                if (data.CustomTiming == true)
                {
                    startTime = data.FromTime;
                    endTime = data.EndTime;

                    string[] starttimeShift = startTime.Split(' ').ToArray();
                    string[] endtimeShift = endTime.Split(' ').ToArray();

                    string startshift = starttimeShift[1];
                    string endshift = endtimeShift[1];


                    if (startshift == "PM" && endshift == "PM")
                    {
                        actualStart = Convert.ToDateTime(data.Start.ToString("yyyy/MM/dd") + " " + data.FromTime);
                        actualEnd = Convert.ToDateTime(data.Start.ToString("yyyy/MM/dd") + " " + data.EndTime);

                        reportStart = actualStart;
                        reportend = actualEnd;

                    }
                    else if (startshift == "PM" && endshift == "AM")
                    {
                        string[] addDays = endtimeShift[0].Split(':');
                        int sum = Convert.ToInt32(addDays[0]);
                        if (sum < 9)
                        {
                            actualStart = Convert.ToDateTime(data.Start.ToString("yyyy/MM/dd") + " " + data.FromTime);
                            actualEnd = Convert.ToDateTime(data.Start.AddDays(1).ToString("yyyy/MM/dd") + " " + data.EndTime);
                        }
                        else
                        {
                            actualStart = Convert.ToDateTime(data.Start.ToString("yyyy/MM/dd") + " " + data.FromTime);
                            actualEnd = Convert.ToDateTime(data.Start.AddDays(1).ToString("yyyy/MM/dd") + " " + data.EndTime);
                        }

                        reportStart = actualStart;
                        reportend = Convert.ToDateTime(data.Start.AddDays(1).ToString("yyyy/MM/dd") + " " + data.EndTime);
                    }
                    else
                    {
                        TimeSpan duration = DateTime.Parse(endTime).Subtract(DateTime.Parse(startTime));
                        actualStart = Convert.ToDateTime(data.Start.ToString("yyyy/MM/dd") + " " + data.FromTime);
                        actualEnd = actualStart.AddHours(Math.Abs(duration.TotalHours));

                        reportStart = actualStart;
                        reportend = actualEnd;

                    }

                    //TimeSpan duration = DateTime.Parse(endTime).Subtract(DateTime.Parse(startTime));
                    //actualStart = Convert.ToDateTime(data.Start.ToString("yyyy/MM/dd") + " " + data.FromTime);
                    //actualEnd = actualStart.AddHours(Math.Abs(duration.TotalHours));
                }
                else
                {
                    // work time dropdown selected value splitted                 
                    double hours = Convert.ToDouble(workTimeValues[1]);


                    reportStart = Convert.ToDateTime(data.Start.ToString("yyyy/MM/dd") + " " + workTimeValues[2]);
                    reportend = reportStart.AddHours(hours);

                    //actualStart = Convert.ToDateTime(data.Start.ToString("yyyy/MM/dd") + " " + workTimeValues[2]);                    
                    ////actualEnd = Convert.ToDateTime(data.Start.AddHours(hours).AddDays(1).ToString("yyyy/MM/dd"));
                    //actualEnd = actualStart.AddHours(hours);

                    startTime = workTimeValues[2];
                    endTime = Convert.ToDateTime(data.Start.ToString("yyyy/MM/dd") + " " + workTimeValues[2]).AddHours(hours).ToShortTimeString();

                    string[] starttimeShift = startTime.Split(' ').ToArray();
                    string[] endtimeShift = endTime.Split(' ').ToArray();
                    string startshift = starttimeShift[1];
                    string endshift = endtimeShift[1];

                    if (startshift == "PM" && endshift == "AM")
                    {
                        string[] addDays = endtimeShift[0].Split(':');
                        int sum = Convert.ToInt32(addDays[0]);
                        if (sum < 9)
                        {
                            actualStart = Convert.ToDateTime(data.Start.ToString("yyyy/MM/dd") + " " + workTimeValues[2]);
                            //actualEnd = actualStart.AddHours(hours).AddDays(1);
                            actualEnd = actualStart.AddHours(hours);
                        }
                        else
                        {
                            actualStart = Convert.ToDateTime(data.Start.ToString("yyyy/MM/dd") + " " + workTimeValues[2]);
                            actualEnd = actualStart.AddHours(hours);
                        }
                    }
                    else
                    {
                        actualStart = Convert.ToDateTime(data.Start.ToString("yyyy/MM/dd") + " " + workTimeValues[2]);
                        //actualEnd = Convert.ToDateTime(data.Start.AddHours(hours).AddDays(1).ToString("yyyy/MM/dd"));
                        actualEnd = actualStart.AddHours(hours);
                    }

                }
                //Client_Scheduling start date end date settings ends here

                //Client_Scheduling_Dates dates adn hour seperate starts
                //if statement checks if there is mulitple date , else part will trigger when there is only one date
                List<SchedulingDate> listschedulingDate = new List<SchedulingDate>();
                if (reportend.Date > reportStart.Date)
                {
                    int reportDateDiff = reportend.Date.Subtract(reportStart.Date).Days;
                    SchedulingDate test = new SchedulingDate();

                    List<DateTime> scheduledDate = Enumerable.Range(0, 1 + reportend.Date.Subtract(reportStart.Date).Days).Select(offset => reportStart.Date.AddDays(offset)).ToList();
                    foreach (DateTime item in scheduledDate)
                    {
                        SchedulingDate objschedulingDate = new SchedulingDate();
                        objschedulingDate.Date = item;

                        var hours = reportend.Subtract(reportStart);
                        var startDayhours = TimeSpan.FromHours(24) - reportStart.TimeOfDay;
                        var endDayhours = hours - startDayhours;

                        if (item.Date == reportStart.Date)
                        {
                            objschedulingDate.Hours = startDayhours.TotalHours;
                            objschedulingDate.Date = objschedulingDate.Date + reportStart.TimeOfDay;
                        }
                        else
                        {
                            objschedulingDate.Hours = endDayhours.TotalHours;
                            objschedulingDate.Date = objschedulingDate.Date + reportend.TimeOfDay;
                        }

                        listschedulingDate.Add(objschedulingDate);
                    }
                }
                else
                {
                    int reportDateDiff = reportend.Date.Subtract(reportStart.Date).Days;
                    SchedulingDate test = new SchedulingDate();
                    List<DateTime> scheduledDate = Enumerable.Range(0, 1 + reportend.Date.Subtract(reportStart.Date).Days).Select(offset => reportStart.Date.AddDays(offset)).ToList();
                    foreach (DateTime item in scheduledDate)
                    {
                        SchedulingDate objschedulingDate = new SchedulingDate();
                        objschedulingDate.Date = item;

                        var hours = reportend.Subtract(reportStart);
                        objschedulingDate.Hours = hours.TotalHours;
                        objschedulingDate.Date = objschedulingDate.Date + reportStart.TimeOfDay;
                        listschedulingDate.Add(objschedulingDate);
                    }
                }


                ScheduledData saveData = new ScheduledData();
                saveData.Id = data.Id;
                saveData.ClientId = data.ClientId;
                saveData.CareTaker = data.CareTaker;
                saveData.ClientName = data.ClientName;
                saveData.CareTakerName = data.CareTakerName;
                saveData.WorkModeName = data.WorkModeName;
                saveData.ServiceTypeName = data.ServiceTypeName;
                //saveData.SendMailCaregiver = data.SendMailCaregiver;
                //saveData.SendMailClient = data.SendMailClient;
                if (workTimeValues != null)
                {
                    saveData.WorkTime = Convert.ToInt32(workTimeValues[0]);
                }
                else
                {
                    saveData.WorkTime = data.WorkTime;
                }

                if (workShiftVlaues != null)
                {
                    saveData.WorkMode = Convert.ToInt32(workShiftVlaues[0]);
                }
                else
                {
                    saveData.WorkMode = data.WorkMode;
                }
                //saveData.WorkMode = data.WorkMode;
                saveData.CareTakerType = data.CareTakerType;
                saveData.Start = actualStart;
                saveData.End = actualEnd;
                saveData.Description = data.Description;
                saveData.ContactPerson = data.ContactPerson;
                saveData.ClientSchedulingDate = listschedulingDate;
                saveData.UserId = (int)Session["loggedInUserId"];
                saveData.AuditLogType = AuditLogType.Scheduling;
                if (saveData.Id != 0)
                {
                    saveData.AuditLogActionType = AuditLogActionType.Edit;
                }
                else
                {
                    saveData.AuditLogActionType = AuditLogActionType.Add;
                }
                saveData.SiteURL = Request.Url.Scheme + "://" + Request.Url.Authority + "/";
                Service service = new Service();
                string api = "Client/SaveScheduleDetails";
                var serviceContent = JsonConvert.SerializeObject(saveData);

                HttpStatusCode result = service.PostAPI(serviceContent, api);

                NotificationHub.Static_Send("notify", "New Schedule created", "static");

                return new JsonResult { Data = new { status = (result == HttpStatusCode.OK) } };
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-SaveEvent");
                return new JsonResult { Data = new { status = false } };
            }

        }




        [HttpPost]
        public JsonResult GetEventsBySearchParams(EventSearchParams searchParams)
        {

            List<ScheduledData> scheduleDetailsList = new List<ScheduledData>();
            try
            {
                CalenderEventInput calenderEventInput = new CalenderEventInput
                {
                    ScheduleId = 0
                };
                if (searchParams.year != 0 && searchParams.month != 0)
                {
                    calenderEventInput.StartDate = new DateTime(searchParams.year, searchParams.month, 1);
                    calenderEventInput.EndDate = new DateTime(searchParams.year, searchParams.month, DateTime.DaysInMonth(searchParams.year, searchParams.month));
                    calenderEventInput.EndDate = calenderEventInput.StartDate.AddMonths(1).AddSeconds(-1);
                }
                string api = "Client/GetAllScheduledetails";
                var data = JsonConvert.SerializeObject(calenderEventInput);
                var result = service.PostAPIWithData(data, api);
                scheduleDetailsList = JsonConvert.DeserializeObject<List<ScheduledData>>(result.Result);
                scheduleDetailsList.ForEach(x =>
                {
                    x.Start = (DateTime.SpecifyKind(x.Start, DateTimeKind.Utc));
                });
                if (searchParams.clientId != 0)
                {
                    scheduleDetailsList.OrderByDescending(s => s.FromTime);
                    var events = scheduleDetailsList.Where(a => a.ClientId == searchParams.clientId);

                    return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    return new JsonResult { Data = scheduleDetailsList, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }

            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-GetEventsByClientId");
                return null;
            }

        }

        [HttpPost]
        public JsonResult GetEventsByClientId(int clientId, int month, int year)
        {

            List<ScheduledData> scheduleDetailsList = new List<ScheduledData>();
            try
            {
                CalenderEventInput calenderEventInput = new CalenderEventInput
                {
                    ScheduleId = 0
                };
                if (year != 0 && month != 0)
                {
                    calenderEventInput.StartDate = new DateTime(year, month, 1);
                    calenderEventInput.EndDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                    calenderEventInput.EndDate = calenderEventInput.StartDate.AddMonths(1).AddSeconds(-1);
                }
                string api = "Client/GetAllScheduledetails";
                var data = JsonConvert.SerializeObject(calenderEventInput);
                var result = service.PostAPIWithData(data, api);
                scheduleDetailsList = JsonConvert.DeserializeObject<List<ScheduledData>>(result.Result);
                scheduleDetailsList.ForEach(x =>
                {
                    x.Start = (DateTime.SpecifyKind(x.Start, DateTimeKind.Utc));
                });
                if (clientId != 0)
                {
                    scheduleDetailsList.OrderByDescending(s => s.FromTime);
                    var events = scheduleDetailsList.Where(a => a.ClientId == clientId);

                    return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    return new JsonResult { Data = scheduleDetailsList, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }

            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-GetEventsByClientId");
                return null;
            }

        }
        public JsonResult GetEvents(int month, int year)
        {
            List<ScheduledData> scheduleDetailsList = new List<ScheduledData>();
            try
            {
                CalenderEventInput calenderEventInput = new CalenderEventInput
                {
                    ScheduleId = 0
                };
                if (year != 0 && month != 0)
                {
                    calenderEventInput.StartDate = new DateTime(year, month, 1);
                    calenderEventInput.EndDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                }
                string api = "Client/GetAllScheduledetails";
                var data = JsonConvert.SerializeObject(calenderEventInput);
                var result = service.PostAPIWithData(data, api);
                scheduleDetailsList = JsonConvert.DeserializeObject<List<ScheduledData>>(result.Result);
                scheduleDetailsList.ForEach(x =>
                {
                    x.Start = (DateTime.SpecifyKind(x.Start, DateTimeKind.Utc));
                });
                var events = scheduleDetailsList;
                return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-GetEvents");
                return null;
            }
        }

        public JsonResult GetHolidays()
        {
            try
            {
                HolidayViewModel holidaySearchModel = new HolidayViewModel();
                string api = "Admin/GetHolidayDetails";
               
                var holidaySearch = JsonConvert.SerializeObject(holidaySearchModel);
                var result = service.PostAPIWithData(holidaySearch, api);
                var Clients = JsonConvert.DeserializeObject<List<HolidayViewModel>>(result.Result);

                Clients.ForEach(x =>
                {
                    x.HolidayDate = (DateTime.SpecifyKind(x.HolidayDate ?? DateTime.MinValue, DateTimeKind.Utc));
                });

                // string api = "Admin/GetHolidayDetails/0";
                //var Clients = service.GetAPI(api);
                return Json(JsonConvert.SerializeObject(Clients), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-GetHolidays");
                return null;
            }
        }

        public ActionResult DeleteEvent(ScheduleDeleteData data)
        {
            try
            {
                if (Session["UserType"] == null)
                {
                    TempData["Failure"] = Models.Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        TempData["Failure"] = Models.Constants.NoViewPrivilege;
                        return new JsonResult { Data = new { status = false } };
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
                            TempData["Failure"] = Models.Constants.NoViewPrivilege;
                            return new JsonResult { Data = new { status = false } };
                        }

                        ViewBag.AllowDelete = apiResults.AllowDelete;
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR") ViewBag.AllowEdit = true;
                }
                else
                {
                    TempData["Failure"] = Models.Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                    //return new JsonResult { Data = new { status = false } };
                }
                string siteUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/";
                data.SiteURL = siteUrl;
                data.UserId = (int)Session["loggedInUserId"];
                data.AuditLogType = AuditLogType.Scheduling;
                data.AuditLogActionType = AuditLogActionType.Delete;
                api = "Client/DeleteSchedule";
                var deleteData = JsonConvert.SerializeObject(data);
                var result = service.PostAPIWithData(deleteData, api);
                if (result.Result == "success")
                {
                    TempData["EventSuccess"] = "Events deleted Successfully";
                }
                else
                {
                    TempData["EventFailure"] = "Event deletion Failed.";
                }
                return new JsonResult { Data = new { status = (result.Result) } };
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-DeleteEvent");
                return new JsonResult { Data = new { status = false } };
            }
        }

        public ActionResult LoadCareTaker()
        {
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
                string api = "CareTaker/RetrieveCareTakerListForDdlByLocation/";
                var clientSearchInputsContent = JsonConvert.SerializeObject(inputs);
                var CareTakerResult = service.PostAPIWithData(clientSearchInputsContent, api);
                return Json(CareTakerResult.Result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-LoadCareTaker");
                return null;
            }
        }

        [HttpPost]
        [Route("Client/LoadCareTakerByLocation")]
        public ActionResult LoadCareTakerByLocation(LocationSearchInputs inputs)
        {
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

                string api = "CareTaker/RetrieveCareTakerListForDdlByLocation/";
                var clientSearchInputsContent = JsonConvert.SerializeObject(inputs);
                var CareTakerResult = service.PostAPIWithData(clientSearchInputsContent, api);
                return Json(CareTakerResult.Result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-LoadCareTaker");
                return null;
            }
        }


        public ActionResult LoadClient()
        {
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
                string api = "Client/GetAllClientDetailsByLocation/";
                var Clients = service.PostAPIWithData(clientSearchInputsContent, api);
                List<ClientModel> scheduleDetailsList = new List<ClientModel>();
                scheduleDetailsList = JsonConvert.DeserializeObject<List<ClientModel>>(Clients.Result);
                var events = scheduleDetailsList.Where(a => a.ClientStatus == ClientStatus.Active).ToList();
                var json = JsonConvert.SerializeObject(events);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-LoadClient");
                return null;
            }
        }
        public ActionResult LoadClientByLocationInvoice(LocationSearchInputs inputs)
        {
            List<ClientModel> clientModel = null;
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
                api = "Client/GetAllClientDetailsByLocation";
                var clientSearchInputsContent = JsonConvert.SerializeObject(inputs);
                var result = service.PostAPIWithData(clientSearchInputsContent, api);
                clientModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ClientModel>>(result.Result);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-ClientSearch");
            }
            return PartialView("_ClientInvoiceSettingsListPartial", clientModel);
        }
        [HttpPost]
        [Route("Client/LoadClient")]
      
        public ActionResult LoadClient(LocationSearchInputs inputs)
        {
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
                string api = "Client/GetAllClientDetailsByLocation/";
                var Clients = service.PostAPIWithData(clientSearchInputsContent, api);
                List<ClientModel> scheduleDetailsList = new List<ClientModel>();
                scheduleDetailsList = JsonConvert.DeserializeObject<List<ClientModel>>(Clients.Result);
                var events = scheduleDetailsList.Where(a => a.ClientStatus == ClientStatus.Active).ToList();
                var json = JsonConvert.SerializeObject(events);



                //return PartialView("_ClientInvoiceSettingsListPartial", events);

                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-LoadClient");
                return null;
            }
        }





        [HttpPost]
        [Route("Client/LoadClientByLocationInvoiceReport")]
        public ActionResult LoadClientByLocationInvoiceReport(LocationSearchInputs inputs)
        {
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
                string api = "Client/GetAllClientDetailsByLocation/";
                var Clients = service.PostAPIWithData(clientSearchInputsContent, api);
                List<ClientModel> scheduleDetailsList = new List<ClientModel>();
                scheduleDetailsList = JsonConvert.DeserializeObject<List<ClientModel>>(Clients.Result);
                var events = scheduleDetailsList.Where(a => a.ClientStatus == ClientStatus.Active).ToList();
                var json = JsonConvert.SerializeObject(events);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-LoadClient");
                return null;
            }
        }



        [HttpPost]
        [Route("Client/LoadClientByLocation")]
        public ActionResult LoadClientByLocation(EventSearchParams searchParams)
        {

           
            List<ScheduledData> scheduleDetailsList = new List<ScheduledData>();
            try
            {
                CalenderEventInput calenderEventInput = new CalenderEventInput
                {
                    ScheduleId = 0
                };
                if (searchParams.year != 0 && searchParams.month != 0)
                {
                    calenderEventInput.StartDate = new DateTime(searchParams.year, searchParams.month, 1);
                    calenderEventInput.EndDate = new DateTime(searchParams.year, searchParams.month, DateTime.DaysInMonth(searchParams.year, searchParams.month));
                    calenderEventInput.EndDate = calenderEventInput.StartDate.AddMonths(1).AddSeconds(-1);
                }
                string api = "Client/GetAllScheduledetails";
                var data = JsonConvert.SerializeObject(calenderEventInput);
                var result = service.PostAPIWithData(data, api);
                scheduleDetailsList = JsonConvert.DeserializeObject<List<ScheduledData>>(result.Result);
                scheduleDetailsList.ForEach(x =>
                {
                    x.Start = (DateTime.SpecifyKind(x.Start, DateTimeKind.Utc));
                });


                if(searchParams.CountryId!=0 && searchParams.StateId==0&&searchParams.CityId==0 &&searchParams.BranchId==0)
                {
                    scheduleDetailsList.OrderByDescending(s => s.FromTime);
                    var events = scheduleDetailsList.Where(a => a.CountryId == searchParams.CountryId);

                    return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }

                else if (searchParams.CountryId != 0 && searchParams.StateId != 0 && searchParams.CityId == 0 &&searchParams.BranchId==0)
                {
                    scheduleDetailsList.OrderByDescending(s => s.FromTime);
                    var events = scheduleDetailsList.Where(a => a.CountryId == searchParams.CountryId && a.StateId==searchParams.StateId);

                    return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else if (searchParams.CountryId != 0 && searchParams.StateId != 0 && searchParams.CityId != 0 && searchParams.BranchId!=0)
                {
                    scheduleDetailsList.OrderByDescending(s => s.FromTime);
                    var events = scheduleDetailsList.Where(a => a.CityId == searchParams.CityId && a.CountryId == searchParams.CountryId && a.StateId==searchParams.StateId);

                    return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else if(searchParams.CountryId==0&&searchParams.CityId!=0&&searchParams.StateId!=0&&searchParams.BranchId!=0)
                {
                    scheduleDetailsList.OrderByDescending(s => s.FromTime);
                    var events = scheduleDetailsList.Where(a => a.CityId == searchParams.CityId && a.StateId == searchParams.StateId);

                    return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else if(searchParams.CountryId == 0 && searchParams.CityId != 0 && searchParams.StateId == 0&&searchParams.BranchId!=0)
                {
                    scheduleDetailsList.OrderByDescending(s => s.FromTime);
                    var events = scheduleDetailsList.Where(a => a.CityId == searchParams.CityId);

                    return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }

                else if (searchParams.CountryId == 0 && searchParams.CityId == 0 && searchParams.StateId == 0 && searchParams.BranchId != 0)
                {
                    scheduleDetailsList.OrderByDescending(s => s.FromTime);
                    var events = scheduleDetailsList.Where(a => a.BranchID == searchParams.BranchId);

                    return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                //if (searchParams.clientId != 0)
                //{
                //    scheduleDetailsList.OrderByDescending(s => s.FromTime);
                //    var events = scheduleDetailsList.Where(a => a.ClientId == searchParams.clientId);

                //    return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                //}
                else
                {
                    return new JsonResult { Data = scheduleDetailsList, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }

            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-GetEventsByClientId");
                return null;
            }


        }

        public ActionResult LoadCategory()
        {
            try
            {
                string api = "Admin/GetCategory?flag=*&value";
                var Category = service.GetAPI(api);
                return Json(Category, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-LoadCategory");
                return null;
            }
        }

        public ActionResult LoadTimeShift()
        {
            try
            {
                string api = "Admin/GetTimeShiftDetails/0";
                var Timeshift = service.GetAPI(api);
                return Json(Timeshift, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-LoadTimeShift");
                return null;
            }
        }

        public ActionResult LoadTimeShiftByClientId(int clientId)
        {
            try
            {
                string api = "Admin/GetTimeShiftDetailsByClientId/" + clientId;
                var Timeshift = service.GetAPI(api);
                return Json(Timeshift, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-LoadTimeShift");
                return null;
            }
        }

        public ActionResult LoadWorkShift()
        {
            try
            {
                string api = "Admin/GetWorkShiftDetails/0";
                var Timeshift = service.GetAPI(api);
                return Json(Timeshift, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-LoadWorkShift");
                return null;
            }
        }

        public ActionResult ClientInvoiceReport()
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


        public ActionResult SaveClientInvoiceReport()
        {
            ClientModel clientModelObj = new ClientModel();
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

        public ActionResult ClientSchedulingReport()
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

            return View();
        }

        public ActionResult PayrollReport()
        {
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

            YearList = new SelectList(years, "Id", "Year", DateTime.Now.Year);
            return YearList;
        }
        public SelectList GetYearsCustom()
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
            YearList = new SelectList(years, "Id", "Year", 0);

            return YearList;
        }

        /// <summary>
        /// method to get client details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ClientInvoiceSettings()
        {
            ClientModel clientModel = new ClientModel();
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
                        ViewBag.Error = Models.Constants.NoViewPrivilege;
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
                var clientSearchInputsContent = JsonConvert.SerializeObject(inputs);
                string resultApi = "Client/GetAllClientDetailsByLocation/";
                var Clients = service.PostAPIWithData(clientSearchInputsContent, resultApi);
                List < ClientModel >clientdetailslist = JsonConvert.DeserializeObject<List<ClientModel>>(Clients.Result);
                var _listClients = new SelectList(clientdetailslist, "ClientId", "ClientName", 0);
               
                ViewData["listClients"] = _listClients;
               
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-ClientSearch");
            }
            return View(clientModel);
        }
        public ActionResult ClientInvoiceSettingsList()
        {
            List<ClientModel> clientModel = new List<ClientModel>();
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Models.Constants.NoViewPrivilege;
                        return RedirectToAction("ClientInvoiceSettings");
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
                            ViewBag.Error = Models.Constants.NoViewPrivilege;
                            return RedirectToAction("ClientInvoiceSettings");
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
                api = "Client/GetClientInvoiceDetails";
                var result = service.GetAPI(api);
                clientModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ClientModel>>(result);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Admin Controller-CityList");
            }
            return PartialView("_ClientInvoiceSettingsListPartial", clientModel);
        }
        public ActionResult AddInvoiceDetails(ClientModel clientModel)
        {
            try
            {
                TempData["Success"] = "";
                //clientModel.ClientId = Convert.ToInt32(Session["loggedInUserId"].ToString());
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        TempData["Failure"] = Models.Constants.NoViewPrivilege;
                        return RedirectToAction("ClientInvoiceSettings");
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
                            TempData["Failure"] = Models.Constants.NoActionPrivilege;
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
                    TempData["Failure"] = Models.Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                api = "Client/AddInvoiceDetails";
                //clientModel.ClientName = clientModel.ClientName.Trim();
                var serviceContent = JsonConvert.SerializeObject(clientModel);
                HttpStatusCode result = service.PostAPI(serviceContent, api);

                if (result == HttpStatusCode.OK)
                {
                    TempData["Success"] = "Invoice details added successfully.";
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
                pCMSLogger.Error(ex, "Error occurred in Admin Controller-CreateCity");
            }
            return RedirectToAction("ClientInvoiceSettings");
        }


        public ActionResult ClientCalendarView()
        {
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "CLIENT")
                    {
                        ViewBag.Error = Models.Constants.NoViewPrivilege;
                        return View();
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
                pCMSLogger.Error(ex, "Error occurred in  Client Calendar View");

            }
            return PartialView("_ClientCalendarViewPartial");
        }

        public ActionResult ClientDashboard()
        {

            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "CLIENT")
                    {
                        ViewBag.Error = Models.Constants.NoViewPrivilege;
                        return View();
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
                api = "Client/GetClientFromUserId/" + Session["loggedInUserId"];
                var result = service.GetAPI(api);
                ViewData["ClientId"] = result;
                return View();
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Admin Controller-AdminDashboard");
                return null;
            }
        }

        [HttpPost]
        public JsonResult GetClientInvoicePayRiseRatesonDateChange(DateTime Date, int ClientId)
        {

            PayriseData bookingPayriseData = new PayriseData()
            {
                ClientId = ClientId,
                Date = Date
            };
            string api = "Client/GetClientInvoicePayRiseRatesonDateChange";
            var getcareTakerPayRise = JsonConvert.SerializeObject(bookingPayriseData);
            var result = service.PostAPIWithData(getcareTakerPayRise, api);
            return Json(result);
        }


    }


}