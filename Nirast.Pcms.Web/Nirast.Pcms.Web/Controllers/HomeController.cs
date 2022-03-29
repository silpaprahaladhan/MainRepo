using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Nirast.Pcms.Web.Helpers;
using Nirast.Pcms.Web.Models;
using System.Configuration;
using Nirast.Pcms.Web.Logger;
using static Nirast.Pcms.Web.Models.Enums;
using System.Net;
using System.Web.UI;
using Novell.Directory.Ldap;


namespace Nirast.Pcms.Web.Controllers
{
    [OutputCache(Duration = 1800, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
    public class HomeController : Controller
    {
        PCMSLogger pCMSLogger = new PCMSLogger();
        Service service = null;

        public HomeController()
        {
            service = new Service(pCMSLogger);
        }

        public ActionResult Footer()
        {
            string api = "Admin/GetCompanyProfiles/0";
            var result = service.GetAPI(api);
            CompanyProfile listCompanyProfile = JsonConvert.DeserializeObject<CompanyProfile>(result);
            return PartialView("_FooterPartial", listCompanyProfile);
        }

        public ActionResult EmailVerificationSuccess()
        {
            if (RouteData.Values["id"] != null)
            {
                string userId = RouteData.Values["id"].ToString();
                int decUserId =  StringCipher.DecodeNumber(userId);
                VerifyUserAccount verifyUser = new VerifyUserAccount
                {
                    UserId = decUserId,
                    Verified = true
                };
                string api = "PublicUser/VerifyUserProfile";
                var serviceContent = JsonConvert.SerializeObject(verifyUser);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.Ambiguous)
                    TempData["UserMessage"] = "Your email has been already verified.";
                else if (result == HttpStatusCode.NotFound)
                    TempData["ErrorMessage"] = "Invalid verification link. Please verify the link sent to your registered mail.";
                else
                    TempData["UserMessage"] = "Your email verification has been completed successfully. <br/>You can now login to the system using your user credentials.";
                if (result != HttpStatusCode.NotFound)
                {
                    api = "PublicUser/GetAllUsersDetails?flag=UserRegnId&value=" + decUserId;
                    var resultItem = service.GetAPI(api);
                    List<PublicUserRegistration> PublicUserList = JsonConvert.DeserializeObject<List<PublicUserRegistration>>(resultItem);
                    TempData["VerifiedUser"] = PublicUserList.FirstOrDefault().FirstName + " " + PublicUserList.FirstOrDefault().LastName;

                }
            }
            return View();
        }



       

        #region Created by Silpa for LDAP Sign in Configuration
        public JsonResult CheckCredential(string loginName, string loginPassword)
        {
            string ldapHost = "localhost";
            int ldapPort = 10389;
            string loginDN = "employeeNumber=1,ou=Aluva,ou=Ernakulam,ou=kerala,ou=India,o=TopCompany";
            string password = "secret";
            string searchBase = "o=TopCompany";
            string searchFilter = "objectClass=inetOrgPerson";
            int cou = 0;
            int checkinvalue = 0;
            int uidvalue = 0;
            string currentdn = string.Empty;
            string userid = string.Empty;
            string EmployeeNo = string.Empty;
            string Employee = string.Empty;
            var dn = string.Empty;

            Session["loginName"] = loginName;
            Session["loginPassword"] = loginPassword;
            try
            {
                Novell.Directory.Ldap.LdapConnection con = new Novell.Directory.Ldap.LdapConnection();
                con.Connect(ldapHost, ldapPort);
                con.Bind(loginDN, password);

                string[] requiredAttributes = { "cn", "sn", "uid" };
                LdapSearchResults lsc = (LdapSearchResults)con.Search(searchBase,
                                               Novell.Directory.Ldap.LdapConnection.ScopeSub,
                                               searchFilter,
                                               requiredAttributes,
                                               false);


                while (lsc.HasMore())
                {
                    LdapEntry nextEntry = null;
                    try
                    {
                        nextEntry = lsc.Next();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        continue;
                    }
                    



                    LdapAttributeSet ldpa = nextEntry.GetAttributeSet();
                    System.Collections.IEnumerator ienum = ldpa.GetEnumerator();


                    while (ienum.MoveNext())
                    {
                        LdapAttribute po = (LdapAttribute)ienum.Current;
                        string attributename = po.Name;
                        string attributevalue = po.StringValue;

                        if (attributename == "cn" && attributevalue == loginName)
                        {
                            cou = cou + 1;
                            uidvalue = uidvalue + 1;
                            dn = nextEntry.Dn;

                        }
                        if (uidvalue == 1)
                        {
                            userid = attributevalue;
                        }
                    }


                   





                }



                if (cou == 1)
                {
                    if (cou != 0 && checkinvalue == 0)
                    {
                        checkinvalue = checkinvalue + 1;
                        con.Bind(dn, loginPassword);
                        currentdn = dn;


                        string[] values = currentdn.Split(',');
                        for (int i = 0; i < values.Length; i++)
                        {
                            Employee = values[0].Trim();
                        }


                        string[] Emp = Employee.Split('=');
                        for (int i = 0; i < Emp.Length; i++)
                        {
                            EmployeeNo = Emp[1].Trim();
                        }

                    }
                }
                else if(cou==0)
                {
                    return Json(string.Empty);
                }
                else
                {
                    
                     return Json("m");
                }


            }
            catch (Exception ex)
            {
                return Json(string.Empty);
            }




            LoginModel loginModel = new LoginModel()
            {
                LoginName = EmployeeNo,

            };
            if (EmployeeNo != "")
            {

                var loginCredential = JsonConvert.SerializeObject(loginModel);
                string loginApi = "Home/CheckLoginCredential";
                var result = service.PostAPIWithData(loginCredential, loginApi);
                var userDetails = JsonConvert.DeserializeObject<LoggedInUser>(result.Result);

                if (userDetails != null)
                {
                    if ((userDetails.UserStatus != 1) || (userDetails.IsVerified == false))
                    {
                        return Json(userDetails);
                    }
                    else
                    {
                        LoggedInUser _LoggedInUser = new LoggedInUser()
                        {
                            UserId = userDetails.UserId,
                            UserType = userDetails.UserType
                        };
                        var usertypedata = JsonConvert.SerializeObject(_LoggedInUser);
                        if (userDetails.UserType == "OFFICE STAFF")
                        {
                            string OfficeType = "Home/CheckAdmin";
                            var resultnew = service.PostAPIWithData(usertypedata, OfficeType);
                            var AdmintestRole = JsonConvert.DeserializeObject<LoggedInUser>(resultnew.Result);
                            Session["CountryId"] = AdmintestRole.CountryId;
                            Session["StateID"] = AdmintestRole.StateId;
                            Session["CityIdk"] = AdmintestRole.CityId;

                            Session["BranchID"] = AdmintestRole.BranchId;


                        }
                        else
                        {
                            Session["CountryId"] = "";
                            Session["StateID"] = "";
                            Session["CityIdk"] = "";
                            Session["BranchID"] = "";
                        }
                        Session["loggedInUserId"] = userDetails.UserId;
                        Session["loggedInUser"] = userDetails.FirstName + " " + userDetails.LastName;
                        if (userDetails.ProfilePicPath != null)
                        {
                            Session["profilePic"] = userDetails.ProfilePicPath;
                        }
                        Session["loginName"] = loginName;
                        // Session["loginPassword"] = StringCipher.Encrypt(loginPassword, ConfigurationManager.AppSettings["EncryptPassword"].ToString());
                        Session["UserType"] = userDetails.UserType;
                        Session["UserRole"] = userDetails.RoleId;
                        Session["UserLocation"] = userDetails.Location;
                        Session["CityId"] = userDetails.CityId;
                        return Json(userDetails);
                    }
                }


                return Json(userDetails);
            }

           else
            {
                return Json(string.Empty);
            }


        }
        #endregion

        public ActionResult LogindoubleEmail(string serviceId)
        {


            LoginModel loginModel = new LoginModel()
            {
                LoginName = serviceId,

            };
                var loginCredential = JsonConvert.SerializeObject(loginModel);
                string loginApi = "Home/CheckLoginCredential";
                var result = service.PostAPIWithData(loginCredential, loginApi);
                var userDetails = JsonConvert.DeserializeObject<LoggedInUser>(result.Result);

                if (userDetails != null)
                {
                    if ((userDetails.UserStatus != 1) || (userDetails.IsVerified == false))
                    {
                        return Json(userDetails);
                    }
                    else
                    {
                        LoggedInUser _LoggedInUser = new LoggedInUser()
                        {
                            UserId = userDetails.UserId,
                            UserType = userDetails.UserType
                        };
                        var usertypedata = JsonConvert.SerializeObject(_LoggedInUser);
                        if (userDetails.UserType == "OFFICE STAFF")
                        {
                            string OfficeType = "Home/CheckAdmin";
                            var resultnew = service.PostAPIWithData(usertypedata, OfficeType);
                            var AdmintestRole = JsonConvert.DeserializeObject<LoggedInUser>(resultnew.Result);
                            Session["CountryId"] = AdmintestRole.CountryId;
                            Session["StateID"] = AdmintestRole.StateId;
                            Session["CityIdk"] = AdmintestRole.CityId;

                            Session["BranchID"] = AdmintestRole.BranchId;


                        }
                        else
                        {
                            Session["CountryId"] = "";
                            Session["StateID"] = "";
                            Session["CityIdk"] = "";
                            Session["BranchID"] = "";
                        }
                        Session["loggedInUserId"] = userDetails.UserId;
                        Session["loggedInUser"] = userDetails.FirstName + " " + userDetails.LastName;
                        if (userDetails.ProfilePicPath != null)
                        {
                            Session["profilePic"] = userDetails.ProfilePicPath;
                        }
                        Session["loginName"] = userDetails.FirstName;
                        // Session["loginPassword"] = StringCipher.Encrypt(loginPassword, ConfigurationManager.AppSettings["EncryptPassword"].ToString());
                        Session["UserType"] = userDetails.UserType;
                        Session["UserRole"] = userDetails.RoleId;
                        Session["UserLocation"] = userDetails.Location;
                        Session["CityId"] = userDetails.CityId;
                        if (userDetails.UserType == "CLIENT")
                        {
                        return RedirectToAction("ClientDashboard", "Client");
                        }
                        else if (userDetails.UserType == "CARETAKER")
                        {
                        return RedirectToAction("CaretakerDashboardView", "Caretaker");
                        }
                        else if (userDetails.UserType == "ADMINISTRATOR" || userDetails.UserType == "OFFICE STAFF")
                        {
                        return RedirectToAction("AdminDashboard", "Admin");
                        }
                        else
                        {
                        return RedirectToAction("Index", "Home");
                        }
                    }
                }


            return RedirectToAction("Index", "Home");


            //return View();
        }

            public ActionResult Dashboard()
        {
            try
            {
                var loginName = Session["loginName"].ToString();
                var loginPassword = Session["loginPassword"].ToString();
                LoginModel loginModel = new LoginModel()
                {
                    LoginName = loginName,
                    Password = loginPassword
                };
                var loginCredential = JsonConvert.SerializeObject(loginModel);
                string loginApi = "Home/CheckLoginCredentialDouble";
                var result = service.PostAPIWithData(loginCredential, loginApi);
                var userDetails = JsonConvert.DeserializeObject<List<LoggedInUser>>(result.Result);
                
                return View(userDetails);
                //var loginame = Session["loginName"].ToString();
                //var loginPassword = Session["loginPassword"].ToString();
                //string api = "Client/GetMappedCaretakerRates/" + loginame + "/" + loginPassword;
                //string api = "Admin/RetrieveServiceDetails/0";
                //var result = service.GetAPI(api);
                //var apiResults = JsonConvert.DeserializeObject<List<ServicesViewModel>>(result);

                //if (apiResults.Count > 0)
                //{
                //    return View(apiResults);
                //}
                //else
                //{
                //    return View();
                //}
            }
            catch (Exception ex)
            {

                return View();
            }
        }



        public JsonResult CheckCredential1(string loginName, string loginPassword)
        {
            try
            {

                

                LoginModel loginModel = new LoginModel()
                {
                    LoginName = loginName,
                    Password = loginPassword
                };
                var loginCredential = JsonConvert.SerializeObject(loginModel);
                string loginApi = "Home/CheckLoginCredential";
                var result = service.PostAPIWithData(loginCredential, loginApi);
                var userDetails = JsonConvert.DeserializeObject<LoggedInUser>(result.Result);
                if (userDetails != null)
                {
                    if ((userDetails.UserStatus != 1 )|| (userDetails.IsVerified == false))
                    {
                        return Json(userDetails);
                    }
                    else
                    {
                        Session["loggedInUserId"] = userDetails.UserId;
                        Session["loggedInUser"] = userDetails.FirstName + " " + userDetails.LastName;
                        if (userDetails.ProfilePicPath != null)
                        {
                            Session["profilePic"] = userDetails.ProfilePicPath;
                        }
                        Session["loginName"] = loginName;
                        Session["loginPassword"] = StringCipher.Encrypt(loginPassword, ConfigurationManager.AppSettings["EncryptPassword"].ToString());
                        Session["UserType"] = userDetails.UserType;
                        Session["UserRole"] = userDetails.RoleId;
                        Session["UserLocation"] = userDetails.Location;
                        Session["CityId"] = userDetails.CityId;
                        return Json(userDetails);
                    }
                    }
                else
                {
                        return Json(string.Empty);
                    }
                }
            
            catch(Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Home Controller-CheckCredential");
                return Json(string.Empty);
            }
        }

        public ActionResult Logout()
        {
            try
            {
                Session["profilePic"] = null;
                Session["loggedInUser"] = null;
                Session.Clear();
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Home Controller-Logout");
                return null;
            }
        }

        public ActionResult Index()        
        
              
        {
            try
            {
                string api = "Admin/RetrieveServiceDetails/0";
                var result = service.GetAPI(api);
                var apiResults = JsonConvert.DeserializeObject<List<ServicesViewModel>>(result);

                if (apiResults.Count > 0)
                {
                    return View(apiResults);
                }
                else
                {
                    return View();
                }
            }
            catch(Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Home Controller-Index");
                return View();
            }
        }

        public ActionResult Careers()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Home Controller-Careers");
                return null;
            }
        }

        public ActionResult About()
        {
            try
            {
                ViewBag.Message = "Your application description page.";
                return View();
            }
            catch(Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Home Controller-About");
                return null;
            }
        }

        public ActionResult Contact()
        {
            try
            {
              
                return View();
            }
            catch(Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Home Controller-Contact");
                return null;
            }
        }
        public ActionResult ContactForm(ContactModel contactModel)
        {
            try
            {
                string api = "Home/SendContactForm";
                contactModel.SiteURL = Request.Url.Scheme + "://" + Request.Url.Authority + "/";
                var serviceContent = JsonConvert.SerializeObject(contactModel);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                if (result == HttpStatusCode.OK)
                {
                    TempData["ContactSuccess"] = "Mail sent Successfully";
                }
                else
                {
                    TempData["ContactFailure"] = "Submission Failed.";
                }
                return RedirectToAction("Contact");
                
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Home Controller-Contact");
                return null;
            }
        }
      
        public ActionResult Services()
        {
            try
            {
                return View();
            }
            catch(Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Home Controller-Services");
                return null;
            }
        }
        public ActionResult ServiceDescription(int serviceId)
        {
            List<ServicesViewModel> listService = new List<ServicesViewModel>();
            try
            {

                string serviceApi = "Admin/RetrieveServiceDetails/"+ serviceId;
                var serviveResult = service.GetAPI(serviceApi);
                listService = JsonConvert.DeserializeObject<List<ServicesViewModel>>(serviveResult);
                string ServiceName = listService[0].Name;
                ViewBag.Service = ServiceName;
                ViewBag.ServiceId = serviceId;
                ViewBag.ServicePicture= listService[0].ServicePicture;
                //   byte ServicePicture= listService[serviceOrder - 1].ServicePicture;
                // ViewBag.ServicePicture = ServicePicture;
            }

            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Home Controller-Search");
            }
            return View(listService);
        }

        public ActionResult TermsAndCondition()
        {
            return View();
        }
        public ActionResult PrivacyPolicy()
        {
            return View();
        }

        public ActionResult Search()
        {
            AdvancedSearchInputModel searchInputs = new AdvancedSearchInputModel();
            UserSessionManager.SetSearchedCareTaker(this, null);
            try
            {
                //if (Session["UserLocation"]!=null && Convert.ToString(Session["UserLocation"]) != string.Empty)
                //{
                //    searchInputs.Location = Session["UserLocation"].ToString();
                //}

                List<CountryViewModel> listCountry = new List<CountryViewModel>();
                List<StateViewModel> listState = new List<StateViewModel>();
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
                List<ServicesViewModel> listService = new List<ServicesViewModel>();
                string serviceApi = "Admin/RetrieveServiceDetails/0";
                var serviveResult = service.GetAPI(serviceApi);
                listService = JsonConvert.DeserializeObject<List<ServicesViewModel>>(serviveResult);
                ViewBag.Service = new SelectList(listService, "ServiceId", "Name", 0);
                List<CategoryViewModel> listCategory = new List<CategoryViewModel>();
                string apicategory = "Admin/GetCategory?flag=*&value=''";
                var categoryResult = service.GetAPI(apicategory);
                listCategory = JsonConvert.DeserializeObject<List<CategoryViewModel>>(categoryResult);
                ViewBag.Category = new SelectList(listCategory, "CategoryId", "Name", 0);
                var experienceList = new SelectList(new[]
                {
                new {ID="4",Name="0-4"},
                new {ID="8",Name="5-8"},
                new {ID="12",Name="9-12"},
                new {ID="100",Name="Above 12"},
            },
                "ID", "Name", 0);
                ViewBag.Experience = experienceList;
                List<CareTakerServices> listRates = new List<CareTakerServices>();
                string apiResult = "Home/GetApprovedRate";
                var resultRates = service.GetAPI(apiResult);
                listRates = JsonConvert.DeserializeObject<List<CareTakerServices>>(resultRates);
                ViewBag.Rates = new SelectList(listRates, "DisplayRate", "DisplayRate", 0);
                var genderList = getGenders();
                ViewBag.Gender = new SelectList(genderList, "GenderId", "Gender", 0);
            }
            catch(Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Home Controller-Search");
            }
            return View(searchInputs); /*_servicesList*/
        }

        /// <summary>
        /// Method to get gender
        /// </summary>
        /// <returns></returns>
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
                pCMSLogger.Error(ex, "Error occurred in Home Controller-getGenders");
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        //[OutputCache(Duration = 1800,VaryByParam = "serviceId")]
        public ActionResult SearchByService(int? serviceId)
        {
            UserSessionManager.SetSearchedCareTaker(this, null);
            AdvancedSearchInputModel searchInputs = new AdvancedSearchInputModel
            {
                Services = serviceId
            };
          
            try
            {
                string api = "Home/SearchCareTaker";
                var advancedSearchInputModel = JsonConvert.SerializeObject(searchInputs);
                var result = service.PostAPIWithData(advancedSearchInputModel, api);
                var apiResults = JsonConvert.DeserializeObject<List<SearchedCareTakers>>(result.Result);
                if (apiResults != null)
                {
                    if (apiResults.Count > 0)
                    {
                        UserSessionManager.SetSearchedCareTaker(this, apiResults);
                        return PartialView(apiResults);
                    }
                }
               
                return PartialView(apiResults);
            }
            catch(Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Home Controller-SearchByService");
                return null;
            }
        }

        [HttpPost]
        public ActionResult SearchCareTaker(AdvancedSearchInputModel searchInputs, string sortField = "Default", int sortBy = 0)
        {
            try
            {
                List<SearchedCareTakers> apiResults = new List<SearchedCareTakers>();
                if (sortField == "Default" && sortBy == 0)
                {
                    if (searchInputs != null)
                    {
                        string api = "Home/SearchCareTaker";
                        var advancedSearchInputModel = JsonConvert.SerializeObject(searchInputs);
                        var result = service.PostAPIWithData(advancedSearchInputModel, api);
                        apiResults = JsonConvert.DeserializeObject<List<SearchedCareTakers>>(result.Result);
                        UserSessionManager.SetSearchedCareTaker(this, apiResults);
                    }
                }
                else
                {
                    apiResults = UserSessionManager.GetSearchedCareTaker(this);
                }
                if (apiResults.Count > 0)
                {
                    if (sortField != "Default" && sortBy != 0)
                    {
                        if (sortBy == 1)
                        {
                            return PartialView(apiResults.OrderByDescending(x => x.TotalExperience));
                        }
                        else if (sortBy == 2)
                        {
                            return PartialView(apiResults.OrderBy(x => x.TotalExperience));
                        }
                        else if (sortBy == 3)
                        {
                            return PartialView(apiResults.OrderByDescending(x => x.DisplayRate));
                        }
                        else 
                        {
                            return PartialView(apiResults.OrderBy(x => x.DisplayRate));
                        }
                    }
                    else
                    {
                        return PartialView(apiResults);
                    }
                }
                else
                {
                    List<CareTakerRegistrationViewModel> caretakerViewModel = new List<CareTakerRegistrationViewModel>();
                    return PartialView(caretakerViewModel);
                }
            }
            catch(Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Home Controller-SearchCareTaker");
                return null;
            }
        }

        public ActionResult KeywordCareTakerSearchDetail(string keyword)
        {
            try
            {
                string api = "Home/KeywordCareTakerSearchDetail";
                var result = service.PostAPIWithData(keyword, api);
                var apiResults = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CareTakerRegistrationViewModel>>(result.Result);

                if (apiResults.Count > 0)
                {
                    return PartialView(apiResults);
                }
                else
                {
                    List<CareTakerRegistrationViewModel> caretakerViewModel = new List<CareTakerRegistrationViewModel>();
                    return PartialView(caretakerViewModel);
                }
            }
            catch(Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Home Controller-KeywordCareTakerSearchDetail");
                return null;
            }
        }

        public ActionResult ServicesMenu()
        {
            
            List<ServicesViewModel> listService = new List<ServicesViewModel>();
            try
            {
                string serviceApi = "Admin/RetrieveServiceDetails/0";
                var serviveResult = service.GetAPI(serviceApi);
                listService = JsonConvert.DeserializeObject<List<ServicesViewModel>>(serviveResult);
            }
            catch(Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Home Controller-ServicesMenu");
            }
            return PartialView(listService);
        }

        public ActionResult TestimonialViewPartial()
        {

            List<Testimonial> listTestimonial = new List<Testimonial>();
            try
            {
                string serviceApi = "Admin/GetTestimonials/0";
                var serviveResult = service.GetAPI(serviceApi);
                listTestimonial = JsonConvert.DeserializeObject<List<Testimonial>>(serviveResult);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Home Controller-TestimonialViewPartial");
            }
            return PartialView(listTestimonial);
        }

        public UserRoleDetailsModel GetUserRoleDetails(int loggedInUserId)
        {
            try
            {
                UserRoleDetailsModel userRoleDetailsModel = new UserRoleDetailsModel();
                string api = "Admin/GetUserRoleDetails/"+ loggedInUserId;
                var result = service.GetAPI(api);
                var UserResult = JsonConvert.DeserializeObject<UserRoleDetailsModel>(result);
                return UserResult;
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Home Controller-GetUserRoleDetails");
                return null;
            }
        }
    }
}