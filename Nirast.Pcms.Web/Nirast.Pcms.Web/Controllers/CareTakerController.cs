using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nirast.Pcms.Web.Helpers;
using Nirast.Pcms.Web.Logger;
using Nirast.Pcms.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Nirast.Pcms.Web.Models.Enums;
using System.DirectoryServices.Protocols;

namespace Nirast.Pcms.Web.Controllers
{
    [OutputCache(Duration = 1800, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
    public class CareTakerController : Controller
    {
        PCMSLogger Logger = new PCMSLogger();
        Service service = null;

        public CareTakerController()
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

            YearList = new SelectList(years, "Id", "Year", DateTime.Now.Year); ;
            return YearList;
        }

        public ActionResult CaretakerScheduleHistory()
        {
            string api = string.Empty;
            if (Session["UserType"] != null)
            {
                if (Convert.ToString(Session["UserType"]).ToLower() != "caretaker")
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
                TempData["Failure"] = Constants.NotLoggedIn;
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        // GET: CareTaker
        public ActionResult CaretakerRegistration()
        {
            try
            {
                var listGender = getGenders();
                ViewData["GenderList"] = new SelectList(listGender, "GenderId", "Gender", 0);

                UserSessionManager.SetCareTakerExperience(this, null);
                UserSessionManager.SetCareTakerQualification(this, null);
                UserSessionManager.SetCareTakerService(this, null);
                UserSessionManager.SetCareTakerClient(this, null);
                UserSessionManager.SetCareTakerDocuments(this, null, "CaretakerCertificates");
                UserSessionManager.SetCareTakerDocuments(this, null, "CaretakerSins");
                UserSessionManager.SetCareTakerDocuments(this, null, "CaretakerOthers");

                List<ServicesViewModel> serviceList = new List<ServicesViewModel>();
                string api = "Admin/RetrieveServiceDetails/0";
                var resultService = service.GetAPI(api);
                serviceList = JsonConvert.DeserializeObject<List<ServicesViewModel>>(resultService);

                var _servicesList = new SelectList(serviceList, "ServiceId", "Name", 0);
                ViewData["ServiceList"] = _servicesList;

                List<CountryViewModel> listCountry = new List<CountryViewModel>();
                api = "Admin/GetCountryDetails/0";
                var countryResult = service.GetAPI(api);
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


                var cityList = new SelectList(new[]
                {
                new {CityId = "0", CityName = "--Select--" },
               },
                "CityId", "CityName", 0);
                ViewData["ListCity"] = cityList;

                string profileId = "Caretaker/GetCaretakerProfileId";
                profileId = service.GetAPI(profileId);
                ViewBag.ProfileId = "CT" + Convert.ToInt32(profileId).ToString("0000");

                api = "Admin/GetCategory?flag=*&value=''";
                List<CategoryViewModel> categoryList = new List<CategoryViewModel>();
                var result = service.GetAPI(api);
                categoryList = JsonConvert.DeserializeObject<List<CategoryViewModel>>(result);
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
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Caretaker Controller-Caregiver Registration");
            }
            return View();

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
                Logger.Error(ex, "Error occurred in Caretaker Controller-getGenders");
                return null;
            }
        }

        //[AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveCaretaker(CareTakerRegistrationViewModel objCareTakerViewModel)
        {
            try
            {
                var response = Request["g-recaptcha-response"];
                string secretKey = "6LcGiGMUAAAAALSs8J4ZJNqIVM2ej-5dvbf_wP5q";
                var client = new WebClient();
                var results = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));
                var obj = JObject.Parse(results);
                var sts = (bool)obj.SelectToken("success");
                string api = "";
                string firstname = objCareTakerViewModel.FirstName;
                string lastname = objCareTakerViewModel.LastName;
                var pswd = objCareTakerViewModel.Password;
                string uid = "1";
                Session["userId"] = 0;

                if (objCareTakerViewModel.FirstName != null && objCareTakerViewModel.LastName != null)
                {
                    if (UserSessionManager.GetCareTakerService(this).Count > 0)
                    {

                        string encryptionPassword = ConfigurationManager.AppSettings["EncryptPassword"].ToString();
                        string passwordEpt = StringCipher.Encrypt(objCareTakerViewModel.Password, encryptionPassword);
                        objCareTakerViewModel.Password = passwordEpt;
                        objCareTakerViewModel.UserVerified = false;

                        string profileId = "Caretaker/GetCaretakerProfileId";
                        profileId = service.GetAPI(profileId);
                        objCareTakerViewModel.CaretakerProfileId = "CT" + Convert.ToInt32(profileId).ToString("0000");

                        int num = new Random().Next(1000, 9999);
                        string numm = Convert.ToString(num);
                        objCareTakerViewModel.EmployeeNumber = numm;

                        api = "CareTaker/SaveCareTaker";
                        objCareTakerViewModel.UserStatus = UserStatus.Active;
                        objCareTakerViewModel.UserTypeId = 1;
                        objCareTakerViewModel.CareTakerExperiences = UserSessionManager.GetCareTakerExperience(this);
                        objCareTakerViewModel.CareTakerQualifications = UserSessionManager.GetCareTakerQualification(this);
                        objCareTakerViewModel.CareTakerServices = UserSessionManager.GetCareTakerService(this);
                        objCareTakerViewModel.CareTakerClients = UserSessionManager.GetCareTakerClient(this);

                      


                        if (Session["userId"] != null)
                            objCareTakerViewModel.UserId = (int)Session["userId"];

                        List<DocumentsList> lstDocumentDetails = new List<DocumentsList>();
                        lstDocumentDetails.AddRange(UserSessionManager.GetCareTakerDocuments(this, "CaretakerCertificates"));
                        lstDocumentDetails.AddRange(UserSessionManager.GetCareTakerDocuments(this, "CaretakerSins"));
                        lstDocumentDetails.AddRange(UserSessionManager.GetCareTakerDocuments(this, "CaretakerOthers"));
                        objCareTakerViewModel.CareTakerDocuments = lstDocumentDetails;

                        objCareTakerViewModel.SiteURL = Request.Url.Scheme + "://" + Request.Url.Authority + "/";
                        if (!Directory.Exists(Server.MapPath("~/PCMS/Documents/CareGiver/")))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/PCMS/Documents/CareGiver/"));
                        }
                        
                        foreach ( var item in lstDocumentDetails)
                        {
                            item.DocumentPath = Server.MapPath("~/PCMS/Documents/CareGiver/") + objCareTakerViewModel.CaretakerProfileId + "_" + item.DocumentType + "_" + item.DocumentName;
                            System.IO.File.WriteAllBytes(item.DocumentPath, item.DocumentContent);
                            string docPath = objCareTakerViewModel.SiteURL + "PCMS/Documents/CareGiver/" + objCareTakerViewModel.CaretakerProfileId + "_" + item.DocumentType + "_" + item.DocumentName;
                            item.DocumentPath = docPath;
                            item.DocumentContent = null;
                        }

                        //objCareTakerViewModel.CareTakerDocuments = lstDocumentDetails;
                        objCareTakerViewModel.CertificateFiles = null;
                        objCareTakerViewModel.SINFile = null;
                        objCareTakerViewModel.OtherDocuments = null;
                        

                        // write profile pic to a folder and save its path in db.
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

                        var careTakerViewModel = JsonConvert.SerializeObject(objCareTakerViewModel);
                        var resultPost = service.PostAPIWithData(careTakerViewModel, api);
                        #region Created by Silpa on Caretaker registration
                        System.DirectoryServices.Protocols.LdapConnection ldapConnection = new System.DirectoryServices.Protocols.LdapConnection("127.0.0.1:10389");

                        var networkCredential = new NetworkCredential(
                              "employeeNumber=1,ou=Aluva,ou=Ernakulam,ou=kerala,ou=India,o=TopCompany",
                              "secret");

                        ldapConnection.SessionOptions.ProtocolVersion = 3;

                        ldapConnection.AuthType = AuthType.Basic;
                        ldapConnection.Bind(networkCredential);


                        var request = new AddRequest("employeeNumber=" + numm + ",ou="+ objCareTakerViewModel.Branch+ ",ou="+objCareTakerViewModel.City+ ",ou="+objCareTakerViewModel.State+ ",ou="+objCareTakerViewModel.Country+",o=TopCompany", new DirectoryAttribute[] {
                    new DirectoryAttribute("employeeNumber", numm),
                    new DirectoryAttribute("cn", firstname),
                      new DirectoryAttribute("uid", uid),
                     new DirectoryAttribute("sn", lastname),
                    new DirectoryAttribute("userPassword",pswd),
                    new DirectoryAttribute("objectClass", new string[] { "top", "person", "organizationalPerson","inetOrgPerson" })
                });
                        ldapConnection.SendRequest(request);
                        #endregion

                        Session["userId"] = null;
                        if (resultPost.Result != "0")
                        {
                            NotificationHub.Static_Send("notify", "New Caregiver registration completed successfully!", "static");// to send notification
                            TempData["CareTakerMsg"] = "Thank you for registering with us, Please activate your account by clicking on the email verification link sent to your registered email: " + objCareTakerViewModel.EmailAddress;
                            TempData["CareTakerContact"] = "We will contact you after the verification process, which usually takes upto 72 hours.";
                            api = "Users/SendVerificationEmail";//+ publicUserDetails.EmailAddress
                            VerifyEmail verifyEmail = new VerifyEmail
                            {
                                WelcomeMsg = "Welcome to Tranquil Care!",
                                FirstName = objCareTakerViewModel.FirstName,
                                MailMsg = "Thank you for registering with us, you have successfully created an account with us.",
                                Mailcontent = string.Format("{0}://{1}/Home/EmailVerificationSuccess/{2}", Request.Url.Scheme, Request.Url.Authority, HttpUtility.UrlEncode(StringCipher.EncodeNumber(Convert.ToInt32(resultPost.Result)))),
                                ContactNo = "1-800-892-6066",
                                RegardsBy = "Tranquil Care Inc.",
                                siteUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/",
                                CompanyName_TagLine = "Tranquil Care - Delivering Care Excellence.",
                                CompanyName = "Tranquil Care Inc.",
                                Subject = "Email Verification Link",
                                Email = objCareTakerViewModel.EmailAddress
                            };
                            var serviceContent = JsonConvert.SerializeObject(verifyEmail);
                            var result = service.PostAPIWithData(serviceContent, api);
                        }
                        else
                        {
                            TempData["CareTakerMsg"] = "Caregiver registration failed";
                        }
                    }
                    else
                    {
                        TempData["CareTakerMsg"] = "Session Expired! Please try again.";
                    }
                }
                UserSessionManager.SetCareTakerExperience(this, null);
                UserSessionManager.SetCareTakerQualification(this, null);
                UserSessionManager.SetCareTakerService(this, null);
                UserSessionManager.SetCareTakerClient(this, null);
                UserSessionManager.SetCareTakerDocuments(this, null, "CaretakerCertificates");
                UserSessionManager.SetCareTakerDocuments(this, null, "CaretakerSins");
                UserSessionManager.SetCareTakerDocuments(this, null, "CaretakerOthers");

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Caretaker Controller-Save Caregiver");
            }
            return RedirectToAction("CareTakerRegistration");
        }

        [HttpPost]
        public JsonResult UploadCertificateFiles()
        {
            try
            {
                List<DocumentsList> objCertificateList = UserSessionManager.GetCareTakerDocuments(this, "CaretakerCertificates");
                try
                {
                    //long size = objCertificateList.Sum(x => x.DocumentContent.Length);
                    //long newsize = 0;
                    foreach (string file in Request.Files)
                    {
                        var fileContent = Request.Files[file];
                        if (fileContent != null && fileContent.ContentLength > 0)
                        {
                            // get a stream
                            var stream = fileContent.InputStream;
                            // and optionally write the file to disk
                            var fileName = Path.GetFileName(file);
                            int fileId = Convert.ToInt32(fileName.Split('_')[1]);

                            DocumentsList objDocumentDetails = new DocumentsList();
                            objDocumentDetails.CaretakerDocumentId = fileId;
                            objDocumentDetails.DocumentName = fileContent.FileName;
                            objDocumentDetails.ContentType = fileContent.ContentType;
                            using (var binaryReader = new BinaryReader(fileContent.InputStream))
                            {
                                objDocumentDetails.DocumentContent = binaryReader.ReadBytes(fileContent.ContentLength);
                            }
                            objDocumentDetails.DocumentTypeId = 1;
                            objDocumentDetails.DocumentType = Enum.GetName(typeof(DocumentType), 1);

                            string docPath = Server.MapPath("~/PCMS/Documents/CareGiver/") + objDocumentDetails.DocumentName;
                            
                            objDocumentDetails.DocumentPath = docPath;
                            objCertificateList.Add(objDocumentDetails);
                            //newsize = objDocumentDetails.DocumentContent.Length + newsize;
                        }
                    }
                    //long mBsize = ((size + newsize) / 1024) / 1024;
                    //if (mBsize > 10)
                    //{
                    //    return Json("Upload file size should not exceed 10Mb.");
                    //}


                    UserSessionManager.AddCareTakerDocuments(this, objCertificateList, "CaretakerCertificates");
                    objCertificateList = UserSessionManager.GetCareTakerDocuments(this, "CaretakerCertificates");
                    string json = JsonConvert.SerializeObject(objCertificateList);

                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error occurred in Caretaker Controller-UploadCertificateFiles");
                }
                return Json(string.Empty);

            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }
        }

        [HttpPost]
        public JsonResult RemoveCertificateFiles(int id)
        {
            try
            {
                List<DocumentsList> objCertificateList = UserSessionManager.GetCareTakerDocuments(this, "CaretakerCertificates");
                objCertificateList.RemoveAll(x => x.CaretakerDocumentId == id);
                return Json(string.Empty);

            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }
        }
        [HttpPost]
        public JsonResult UploadSinFiles()
        {
            try
            {
                List<DocumentsList> objSinList = UserSessionManager.GetCareTakerDocuments(this, "CaretakerSins");
                try
                {
                    foreach (string file in Request.Files)
                    {
                        var fileContent = Request.Files[file];
                        if (fileContent != null && fileContent.ContentLength > 0)
                        {
                            // get a stream
                            var stream = fileContent.InputStream;
                            // and optionally write the file to disk
                            var fileName = Path.GetFileName(file);
                            int fileId = Convert.ToInt32(fileName.Split('_')[1]);

                            DocumentsList objDocumentDetails = new DocumentsList();
                            objDocumentDetails.CaretakerDocumentId = fileId;
                            objDocumentDetails.DocumentName = fileContent.FileName;
                            objDocumentDetails.ContentType = fileContent.ContentType;
                            using (var binaryReader = new BinaryReader(fileContent.InputStream))
                            {
                                objDocumentDetails.DocumentContent = binaryReader.ReadBytes(fileContent.ContentLength);
                            }
                            objDocumentDetails.DocumentTypeId = 2;
                            objDocumentDetails.DocumentType = Enum.GetName(typeof(DocumentType), 2);
                            string docPath = Server.MapPath("~/PCMS/Documents/CareGiver/") +objDocumentDetails.DocumentName;
                            objDocumentDetails.DocumentPath = docPath;
                            objSinList.Add(objDocumentDetails);
                        }
                    }

                    UserSessionManager.AddCareTakerDocuments(this, objSinList, "CaretakerSins");
                    objSinList = UserSessionManager.GetCareTakerDocuments(this, "CaretakerSins");
                    string json = JsonConvert.SerializeObject(objSinList);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error occurred in Caretaker Controller-UploadSinFiles");
                }
                return Json(string.Empty);

            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }
        }

        [HttpPost]
        public JsonResult RemoveSinFiles(int id)
        {
            try
            {
                List<DocumentsList> objSinList = UserSessionManager.GetCareTakerDocuments(this, "CaretakerSins");
                objSinList.RemoveAll(x => x.CaretakerDocumentId == id);
                return Json(string.Empty);

            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }
        }

        [HttpPost]
        public JsonResult UploadOtherFiles()
        {
            try
            {
                List<DocumentsList> objOtherList = UserSessionManager.GetCareTakerDocuments(this, "CaretakerOthers");
                try
                {
                    foreach (string file in Request.Files)
                    {
                        var fileContent = Request.Files[file];
                        if (fileContent != null && fileContent.ContentLength > 0)
                        {
                            // get a stream
                            var stream = fileContent.InputStream;
                            // and optionally write the file to disk
                            var fileName = Path.GetFileName(file);
                            int fileId = Convert.ToInt32(fileName.Split('_')[1]);

                            DocumentsList objDocumentDetails = new DocumentsList();
                            objDocumentDetails.CaretakerDocumentId = fileId;
                            objDocumentDetails.DocumentName = fileContent.FileName;
                            objDocumentDetails.ContentType = fileContent.ContentType;
                            using (var binaryReader = new BinaryReader(fileContent.InputStream))
                            {
                                objDocumentDetails.DocumentContent = binaryReader.ReadBytes(fileContent.ContentLength);
                            }
                            objDocumentDetails.DocumentTypeId = 3;
                            objDocumentDetails.DocumentType = Enum.GetName(typeof(DocumentType), 3);
                            string docPath = Server.MapPath("~/PCMS/Documents/CareGiver/") + objDocumentDetails.DocumentName;
                            objDocumentDetails.DocumentPath = docPath;
                            objOtherList.Add(objDocumentDetails);
                        }
                    }

                    UserSessionManager.AddCareTakerDocuments(this, objOtherList, "CaretakerOthers");
                    objOtherList = UserSessionManager.GetCareTakerDocuments(this, "CaretakerOthers");
                    string json = JsonConvert.SerializeObject(objOtherList);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error occurred in Caretaker Controller-UploadOtherFiles");
                }
                return Json(string.Empty);

            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }
        }

        [HttpPost]
        public JsonResult RemoveOtherFiles(int id)
        {
            try
            {
                List<DocumentsList> objOtherList = UserSessionManager.GetCareTakerDocuments(this, "CaretakerOthers");
                objOtherList.RemoveAll(x => x.CaretakerDocumentId == id);
                return Json(string.Empty);

            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }
        }



        [HttpPost]
        public JsonResult AddExperienceDetails(string dateFrom, string dateTo, string company)
        {
            List<CaretakerExperiences> objCareTakerExperienceList = UserSessionManager.GetCareTakerExperience(this);
            CaretakerExperiences objCareTakerExperience = new CaretakerExperiences();
            try
            {
                objCareTakerExperience.DateFrom = DateTime.ParseExact(dateFrom, "MM/dd/yyyy", new CultureInfo("en-US"), DateTimeStyles.None);
                objCareTakerExperience.DateTo = DateTime.ParseExact(dateTo, "MM/dd/yyyy", new CultureInfo("en-US"), DateTimeStyles.None);
                objCareTakerExperience.Company = company;
                UserSessionManager.AddCareTakerExperience(this, objCareTakerExperience);
                objCareTakerExperienceList = UserSessionManager.GetCareTakerExperience(this);
                string json = JsonConvert.SerializeObject(objCareTakerExperienceList);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Caretaker Controller-AddExperienceDetails");
            }
            return Json(objCareTakerExperienceList);
        }

        [HttpPost]
        public JsonResult AddQualificationDetails(string qualificationId, string qualification)//, string qualificationDate, string university
        {
            List<CareTakerQualifications> objCareTakerQualificationList = UserSessionManager.GetCareTakerQualification(this);
            CareTakerQualifications objCareTakerQualification = new CareTakerQualifications();
            try
            {
                Random generator = new Random();
                int randomId = generator.Next(0, 9999);
                int count = 0;
                if (qualificationId != "999") count = objCareTakerQualificationList.Count(x => x.QualificationId == Convert.ToInt32(qualificationId));
                var qualCount = objCareTakerQualificationList.Count(x => x.QualificationName.ToLower() == qualification.ToLower());
                if (count == 0 && qualCount == 0)
                {
                    objCareTakerQualification.QualificationId = Convert.ToInt32(qualificationId);
                    objCareTakerQualification.QualificationName = qualification;
                    objCareTakerQualification.CareTakerQualificationId = randomId;
                    UserSessionManager.AddCareTakerQualification(this, objCareTakerQualification);
                }
                objCareTakerQualificationList = UserSessionManager.GetCareTakerQualification(this);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Caretaker Controller-AddQualificationDetails");
            }
            return Json(objCareTakerQualificationList);
        }

        [HttpPost]
        public JsonResult AddServiceDetails(string serviceId, string serviceName, string expectedRate, string approvedRate, DateTime? effdate, int caretakerId)
        {
            List<CareTakerServices> objCareTakerServiceList = UserSessionManager.GetCareTakerService(this);
            CareTakerServices objCareTakerService = new CareTakerServices();
            try
            {
                var count = objCareTakerServiceList.Count(x => x.ServiceId == Convert.ToInt32(serviceId));
                if (count > 0)
                {
                    objCareTakerServiceList.RemoveAll(x => x.ServiceId == Convert.ToInt32(serviceId));
                }
                objCareTakerService.ServiceId = Convert.ToInt32(serviceId);
                objCareTakerService.ServiceName = serviceName;
                objCareTakerService.DisplayRate = expectedRate == null ? 0 : float.Parse(expectedRate);
                objCareTakerService.PayingRate = approvedRate == null ? 0 : float.Parse(approvedRate);
                objCareTakerService.EffectiveFrom = effdate;
                objCareTakerService.CareTakerId = caretakerId;
                UserSessionManager.AddCareTakerService(this, objCareTakerService);

                objCareTakerServiceList = UserSessionManager.GetCareTakerService(this);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Caretaker Controller-AddServiceDetails");
            }
            return Json(objCareTakerServiceList);
        }
        [HttpPost]
        public ActionResult SavePayRise(List<CareTakerServices> data)
        {
            try
            {
                HttpStatusCode result = HttpStatusCode.OK;

                Service service = new Service();
                string api = "Caretaker/SaveCareTakerPayRise";
                var serviceContent = JsonConvert.SerializeObject(data);
                result = service.PostAPI(serviceContent, api);
                return new JsonResult { Data = new { status = (result == HttpStatusCode.OK) } };
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Client Controller-MapCaretakers");
                return new JsonResult { Data = new { status = false } };
            }
        }

        //[HttpPost]
        //public void AddApprovedServiceRates(string serviceId, string expectedRate, string approvedRate)
        //{
        //    try
        //    {
        //        List<CareTakerServices> objCareTakerServiceList = UserSessionManager.GetCareTakerService(this);
        //        CareTakerServices objCareTakerService = new CareTakerServices();
        //        objCareTakerService.ServiceId = Convert.ToInt32(serviceId);
        //        objCareTakerService.DisplayRate = float.Parse(expectedRate);
        //        objCareTakerService.PayingRate = float.Parse(approvedRate);
        //        UserSessionManager.AddApprovedRates(this, objCareTakerService);
        //    }
        //    catch(Exception ex)
        //    {
        //        Logger.Error(ex, "Error occurred in Caretaker Controller-AddApprovedServiceRates");

        //    }
        //}

        [HttpPost]
        public JsonResult AddClientDetails(string clientId, string clientName)
        {
            List<CareTakerClients> objCareTakerClientList = UserSessionManager.GetCareTakerClient(this);
            CareTakerClients objCareTakerClient = new CareTakerClients();
            try
            {
                objCareTakerClient.ClientId = Convert.ToInt32(clientId); ;
                objCareTakerClient.ClientFirstName = clientName;
                UserSessionManager.AddCareTakerClient(this, objCareTakerClient);
                objCareTakerClientList = UserSessionManager.GetCareTakerClient(this);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Caretaker Controller-AddClientDetails");
            }
            return Json(objCareTakerClientList);
        }

        //[HttpPost]
        //public JsonResult AddOrientationDetails(string orientationId, string orientation)
        //{
        //    List<CareTakerOrientations> objOrientationList = UserSessionManager.GetCareTakerOrientation(this);
        //    CareTakerOrientations objCareTakerOrientation = new CareTakerOrientations();
        //    try
        //    {
        //        objCareTakerOrientation.OrientationId = Convert.ToInt32(orientationId);
        //        objCareTakerOrientation.OrientationName = orientation;
        //        UserSessionManager.AddCareTakerOrientation(this, objCareTakerOrientation);
        //        objOrientationList = UserSessionManager.GetCareTakerOrientation(this);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error(ex, "Error occurred in Caretaker Controller-AddOrientationDetails");
        //    }
        //    return Json(objOrientationList);
        //}

        [HttpPost]
        public JsonResult RemoveExperienceDetails(string company)
        {
            List<CaretakerExperiences> objExperienceList = UserSessionManager.GetCareTakerExperience(this);
            try
            {
                objExperienceList = objExperienceList.Where(x => x.Company != company).ToList();
                UserSessionManager.SetCareTakerExperience(this, objExperienceList);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Caretaker Controller-RemoveExperienceDetails");
            }
            return Json(objExperienceList);
        }

        [HttpPost]
        public JsonResult RemoveQualificationDetails(string qualificationId, int caretakerQualificationId)
        {
            List<CareTakerQualifications> objQualificationList = UserSessionManager.GetCareTakerQualification(this);
            try
            {
                int qualiId = Convert.ToInt32(qualificationId);
                objQualificationList.RemoveAll(x => x.QualificationId == qualiId && x.CareTakerQualificationId == caretakerQualificationId);
                UserSessionManager.SetCareTakerQualification(this, objQualificationList);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Caretaker Controller-RemoveQualificationDetails");
            }
            return Json(objQualificationList);
        }

        [HttpPost]
        public JsonResult RemoveAllSessions()
        {
            try
            {
                UserSessionManager.SetCareTakerExperience(this, null);
                UserSessionManager.SetCareTakerQualification(this, null);
                UserSessionManager.SetCareTakerService(this, null);
                UserSessionManager.SetCareTakerClient(this, null);
                UserSessionManager.SetCareTakerDocuments(this, null, "CaretakerCertificates");
                UserSessionManager.SetCareTakerDocuments(this, null, "CaretakerSins");
                UserSessionManager.SetCareTakerDocuments(this, null, "CaretakerOthers");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Caretaker Controller-RemoveAllSessions");
            }
            return Json(string.Empty);
        }

        [HttpPost]
        public JsonResult RemoveServiceDetails(string serviceId)
        {
            List<CareTakerServices> objServiceList = UserSessionManager.GetCareTakerService(this);
            try
            {
                int _serviceId = Convert.ToInt32(serviceId);
                objServiceList = objServiceList.Where(x => x.ServiceId != _serviceId).ToList();
                UserSessionManager.SetCareTakerService(this, objServiceList);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Caretaker Controller-RemoveServiceDetails");
            }
            return Json(objServiceList);
        }

        [HttpPost]
        public JsonResult RemoveClientDetails(string clientId)
        {
            List<CareTakerClients> objClientList = UserSessionManager.GetCareTakerClient(this);
            try
            {
                int _clientId = Convert.ToInt32(clientId);
                objClientList = objClientList.Where(x => x.ClientId != _clientId).ToList();
                UserSessionManager.SetCareTakerClient(this, objClientList);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Caretaker Controller-RemoveClientDetails");
            }
            return Json(objClientList);
        }

        //[HttpPost]
        //public JsonResult RemoveOrientationDetails(string orientationId)
        //{
        //    List<CareTakerOrientations> objOrientationList = UserSessionManager.GetCareTakerOrientation(this);
        //    try
        //    {
        //        int _orientationId = Convert.ToInt32(orientationId);
        //        objOrientationList = objOrientationList.Where(x => x.OrientationId != _orientationId).ToList();
        //        UserSessionManager.SetCareTakerOrientation(this, objOrientationList);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error(ex, "Error occurred in Caretaker Controller-RemoveOrientationDetails");
        //    }
        //    return Json(objOrientationList);
        //}

        public ActionResult CareTakerDetailPage(int id)
        {
            try
            {
                var listGender = getGenders();
                ViewData["GenderList"] = new SelectList(listGender, "GenderId", "Gender", 0);

                List<ServicesViewModel> serviceList = new List<ServicesViewModel>();
                string api = "Admin/RetrieveServiceDetails/0";
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

                //api = "Admin/GetQualification?qualificationId=0";
                //List<QualificationViewModel> qualificationList = new List<QualificationViewModel>();
                //var resultQual = service.GetAPI(api);
                //qualificationList = JsonConvert.DeserializeObject<List<QualificationViewModel>>(resultQual);
                //var listQualification = new SelectList(qualificationList, "QualificationId", "Qualification", 0);
                //ViewData["ListQualification"] = listQualification;


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

                api = "CareTaker/GetCareTakerDetails/" + id;
                var result = service.GetAPI(api);
                caretakerProfile = Newtonsoft.Json.JsonConvert.DeserializeObject<CareTakerRegistrationViewModel>(result);
                if (caretakerProfile != null)
                {
                    UserSessionManager.SetCareTakerExperience(this, caretakerProfile.CareTakerExperiences);
                    UserSessionManager.SetCareTakerQualification(this, caretakerProfile.CareTakerQualifications);
                    UserSessionManager.SetCareTakerService(this, caretakerProfile.CareTakerServices);
                    UserSessionManager.SetCareTakerClient(this, caretakerProfile.CareTakerClients);
                    UserSessionManager.SetCareTakerDocuments(this, null, "CaretakerCertificates");
                    UserSessionManager.SetCareTakerDocuments(this, null, "CaretakerSins");
                    UserSessionManager.SetCareTakerDocuments(this, null, "CaretakerOthers");

                    List<StateViewModel> stateList = new List<StateViewModel>();
                    api = "Admin/GetStatesByCountryId/" + caretakerProfile.CountryId;
                    var resultState = service.GetAPI(api);
                    stateList = JsonConvert.DeserializeObject<List<StateViewModel>>(resultState);
                    var _stateList = new SelectList(stateList, "StateId", "Name", 0);
                    ViewData["StateList"] = _stateList;

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
                ViewData["dateFrom"] = "";
                ViewData["dateTo"] = "";
                ViewData["timeFrom"] = "";
                ViewData["timeTo"] = "";
                ViewData["totalhours"] = "";
                ViewData["totalholidayhours"] = "";
                ViewData["holidayamount"] = "";
                ViewData["rate"] = "";
                ViewData["ServiceRate"] = "";
                string datefrom = "";
                string dateto = "";
                string timefrom = "";
                string timeto = "";
                double totalHolidayHours = 0;
                double hoildayAmout = 0;
                double serviceRate = 0;
                double rate = 0;
                // checks if query string has date values
                if (Request.QueryString["dateFrom"] != null)
                {
                    datefrom = (Request.QueryString["dateFrom"]).ToString();
                    dateto = (Request.QueryString["dateTo"]).ToString();
                    timefrom = (Request.QueryString["timeFrom"]).ToString();
                    timeto = (Request.QueryString["timeTo"]).ToString();
                    int serviceid = Convert.ToInt32(Request.QueryString["serviceId"]);
                    caretakerProfile.ServiceId = serviceid;
                    DateTime reportStart = DateTime.Now;
                    DateTime reportend = DateTime.Now;
                    DateTime actualStart = DateTime.Now;
                    DateTime actualEnd = DateTime.Now;
                    actualStart = Convert.ToDateTime(datefrom + " " + timefrom);
                    actualEnd = Convert.ToDateTime(dateto + " " + timeto);
                    reportStart = Convert.ToDateTime(actualStart);
                    reportend = Convert.ToDateTime(actualEnd);
                    var hours = reportend.Subtract(reportStart);
                    var startDayhours = TimeSpan.FromHours(24) - reportStart.TimeOfDay;
                    var endDayhours = hours - startDayhours;

                    List<BookigDate> listBookingDates = new List<BookigDate>();
                    //below code gets the list of dates beteween two bookig dates
                    List<DateTime> scheduledDate = Enumerable.Range(0, 1 + reportend.Date.Subtract(reportStart.Date).Days).Select(offset => reportStart.Date.AddDays(offset)).ToList();
                    // below fore each loop checks the date set hrs like date is start then startdayhours, day is end then edn day hours,else 24hrs
                    foreach (DateTime item in scheduledDate)
                    {
                        BookigDate objschedulingDate = new BookigDate();
                        objschedulingDate.Date = item;
                        //if (item.Date == reportStart.Date)
                        //{
                        //    objschedulingDate.Hours = startDayhours.Hours;
                        //}
                        //else if (item.Date == reportend.Date)
                        //{
                        //    objschedulingDate.Hours = endDayhours.Hours;
                        //}
                        //else
                        //{
                        //    objschedulingDate.Hours = 24;
                        //}



                        if (reportend.Date > reportStart.Date)
                        {
                            if (item.Date == reportStart.Date)
                            {
                                objschedulingDate.Date = item;
                                objschedulingDate.Hours = startDayhours.Hours;
                            }
                            else if (item.Date == reportend.Date)
                            {
                                objschedulingDate.Date = item;
                                objschedulingDate.Hours = endDayhours.Hours;
                            }
                            else
                            {
                                objschedulingDate.Date = item;
                                objschedulingDate.Hours = 24;
                            }
                        }
                        else
                        {
                            objschedulingDate.Date = item;
                            objschedulingDate.Hours = hours.TotalHours;
                        }

                        listBookingDates.Add(objschedulingDate);
                    }
                    //after above code we will get all booked date with hours
                    //below code collecting holiday list from db
                    List<HolidayViewModel> holidayList = new List<HolidayViewModel>();
                    HolidayViewModel holidaySearchModel = new HolidayViewModel()
                    {
                        HolidayId = 0
                    };
                    string apis = "Admin/GetHolidayDetails";
                    var holidaySearch = JsonConvert.SerializeObject(holidaySearchModel);
                    var results = service.PostAPIWithData(holidaySearch, apis);
                    //var results = service.GetAPI(apis);
                    holidayList = JsonConvert.DeserializeObject<List<HolidayViewModel>>(results.Result);
                    //below code adding holidate date to seprate list
                    List<DateTime> holidayDates = new List<DateTime>();
                    foreach (var item in holidayList)
                    {
                        if (item.HolidayDate != null)
                        {
                            holidayDates.Add(item.HolidayDate ?? DateTime.MinValue);
                        }
                    }
                    //below code will compare booked dates and holiday dates and returns holiday dates in bookedHolidays
                    var bookedHolidays = scheduledDate.Intersect(holidayDates).ToList();
                    List<BookigDate> listHolidayDates = new List<BookigDate>();
                    // in the below code same logic as above here we will get a list of holiday days and hours
                    foreach (DateTime item in bookedHolidays)
                    {
                        BookigDate objschedulingDate = new BookigDate();
                        objschedulingDate.Date = item;
                        if (reportend.Date > reportStart.Date)
                        {
                            if (item.Date == reportStart.Date)
                            {
                                objschedulingDate.Date = item;
                                objschedulingDate.Hours = startDayhours.Hours;
                            }
                            else if (item.Date == reportend.Date)
                            {
                                objschedulingDate.Date = item;
                                objschedulingDate.Hours = endDayhours.Hours;
                            }
                            else
                            {
                                objschedulingDate.Date = item;
                                objschedulingDate.Hours = 24;
                            }
                        }
                        else
                        {

                            objschedulingDate.Date = item;
                            objschedulingDate.Hours = hours.TotalHours;

                        }

                        listHolidayDates.Add(objschedulingDate);
                    }
                    //below code gets sum of hours of listHolidayDates as totalHolidayHours
                    totalHolidayHours = listHolidayDates.Sum(item => item.Hours);
                    //below code get holiday pay value
                    HolidayViewModel holidayDetails = new HolidayViewModel();
                    string holidayPayApi = "Admin/GetHolidayPayDetails";
                    var holidayPayResult = service.GetAPI(holidayPayApi);


                    //get display rate from caretaker details by using query string value serviceid
                    serviceRate = Convert.ToDouble(caretakerProfile.CareTakerServices.Where(x => x.ServiceId == serviceid).SingleOrDefault().DisplayRate);

                    hoildayAmout = totalHolidayHours * serviceRate * Convert.ToDouble(holidayPayResult);
                    // rate multpling display rate and hours
                    rate = (serviceRate * Convert.ToDouble(hours.TotalHours - totalHolidayHours)) + hoildayAmout;


                    ViewData["dateFrom"] = datefrom;
                    ViewData["dateTo"] = dateto;
                    ViewData["timeFrom"] = timefrom;
                    ViewData["timeTo"] = timeto;
                    ViewData["totalhours"] = hours.TotalHours;
                    ViewData["totalholidayhours"] = totalHolidayHours;
                    ViewData["holidayamount"] = hoildayAmout;
                    ViewData["rate"] = rate.ToString("0.00");
                    ViewData["ServiceRate"] = serviceRate.ToString("0.00");

                }
                return View(caretakerProfile);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Caretaker Controller-CareTakerDetailPage");
                return View(string.Empty);
            }
        }

        //[HttpPost]
        //public ActionResult CareTakerDetailPageWithRate(int caretakerId, DateTime dateFrom, DateTime dateTo, DateTime timeFrom, DateTime timeTo)
        //{
        //    try
        //    {
        //        var listGender = getGenders();
        //        ViewData["GenderList"] = new SelectList(listGender, "GenderId", "Gender", 0);

        //        List<ServicesViewModel> serviceList = new List<ServicesViewModel>();
        //        string api = "Admin/RetrieveServiceDetails/0";
        //        var resultService = service.GetAPI(api);
        //        serviceList = JsonConvert.DeserializeObject<List<ServicesViewModel>>(resultService);
        //        var _servicesList = new SelectList(serviceList, "ServiceId", "Name", 0);
        //        ViewData["ServiceList"] = _servicesList;

        //        List<CountryViewModel> countryList = new List<CountryViewModel>();
        //        api = "Admin/GetCountryDetails/0";
        //        var resultCountry = service.GetAPI(api);
        //        countryList = JsonConvert.DeserializeObject<List<CountryViewModel>>(resultCountry);
        //        var _countryList = new SelectList(countryList, "CountryId", "Name", 0);
        //        ViewData["CountryList"] = _countryList;

        //        List<CategoryViewModel> lstCategory = new List<CategoryViewModel>();
        //        api = "Admin/GetCategory?flag=*&value=''";

        //        List<CategoryViewModel> categoryList = new List<CategoryViewModel>();
        //        CategoryViewModel categoryModel = new CategoryViewModel();
        //        var resultCategory = service.GetAPI(api);
        //        categoryList = JsonConvert.DeserializeObject<List<CategoryViewModel>>(resultCategory);


        //        var listCategory = new SelectList(categoryList, "CategoryId", "Name", 0);
        //        ViewData["ListCategory"] = listCategory;

        //        //api = "Admin/GetQualification?qualificationId=0";
        //        //List<QualificationViewModel> qualificationList = new List<QualificationViewModel>();
        //        //var resultQual = service.GetAPI(api);
        //        //qualificationList = JsonConvert.DeserializeObject<List<QualificationViewModel>>(resultQual);
        //        //var listQualification = new SelectList(qualificationList, "QualificationId", "Qualification", 0);
        //        //ViewData["ListQualification"] = listQualification;


        //        api = "Admin/GetQualification/0";
        //        List<QualificationViewModel> qualificationList = new List<QualificationViewModel>();
        //        QualificationViewModel qlfyVM = new QualificationViewModel();
        //        qlfyVM.QualificationId = 999;
        //        qlfyVM.Qualification = "Others";
        //        var resultQual = service.GetAPI(api);
        //        qualificationList = JsonConvert.DeserializeObject<List<QualificationViewModel>>(resultQual);
        //        qualificationList.Add(qlfyVM);
        //        var listQualification = new SelectList(qualificationList, "QualificationId", "Qualification", 0);
        //        ViewData["ListQualification"] = listQualification;

        //        CareTakerRegistrationViewModel caretakerProfile = null;

        //        api = "CareTaker/GetCareTakerDetails/" + caretakerId;
        //        var result = service.GetAPI(api);
        //        caretakerProfile = Newtonsoft.Json.JsonConvert.DeserializeObject<CareTakerRegistrationViewModel>(result);
        //        if (caretakerProfile != null)
        //        {
        //            UserSessionManager.SetCareTakerExperience(this, caretakerProfile.CareTakerExperiences);
        //            UserSessionManager.SetCareTakerQualification(this, caretakerProfile.CareTakerQualifications);
        //            UserSessionManager.SetCareTakerService(this, caretakerProfile.CareTakerServices);
        //            UserSessionManager.SetCareTakerClient(this, caretakerProfile.CareTakerClients);

        //            List<StateViewModel> stateList = new List<StateViewModel>();
        //            api = "Admin/GetStatesByCountryId/" + caretakerProfile.CountryId;
        //            var resultState = service.GetAPI(api);
        //            stateList = JsonConvert.DeserializeObject<List<StateViewModel>>(resultState);
        //            var _stateList = new SelectList(stateList, "StateId", "Name", 0);
        //            ViewData["StateList"] = _stateList;

        //            //List<Cities> cityList = new List<Cities>();
        //            string apiCity = "City/GetCityByStateId/" + caretakerProfile.StateId;
        //            List<Cities> cityList = new List<Cities>();
        //            Cities cityModel = new Cities();
        //            result = service.GetAPI(apiCity);
        //            cityList = JsonConvert.DeserializeObject<List<Cities>>(result);
        //            var listCity = new SelectList(cityList, "CityId", "CityName", 0);
        //            ViewData["ListCity"] = listCity;
        //            caretakerProfile.Gender = Enum.GetName(typeof(Gender), caretakerProfile.GenderId);
        //        }
        //        ViewData["dateFrom"]  = dateFrom;
        //        ViewData["dateTo"]    = dateTo;
        //        ViewData["timeFrom"]  = timeFrom;
        //        ViewData["timeTo"]    = timeTo;

        //        return View("CareTakerDetailPage","CareTaker", caretakerProfile);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error(ex, "Error occurred in CareTaker Controller-CareTakerDetailPage");
        //        return View(string.Empty);
        //    }
        //}
        public ActionResult CareTakerDashboard()
        {
            try
            {
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "CARETAKER")
                    {
                        TempData["Error"] = Constants.NoViewPrivilege;
                        return RedirectToAction("Error");
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
                Logger.Error(ex, "Error occurred in Caretaker Controller-CareTakerDashboard");
                return null;
            }
        }

        /// <summary>
        /// To view office staff profile
        /// </summary>
        /// <returns></returns>
        public ActionResult CareTakerProfile(int id)
        {
            try
            {

                ViewBag.Message = "Caregiver profile page.";
                //Session["userId"] = id;
                string api = string.Empty;
                if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR" && Convert.ToInt32(Session["loggedInUserId"]) != id)
                {
                    ViewBag.Error = Constants.NoViewPrivilege;
                    return View();
                }
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) == "OFFICE STAFF")
                    {
                        api = "Admin/GetRolePrivilege";
                        GetRolePrivilegeModel getRolePrivilege = new GetRolePrivilegeModel();
                        getRolePrivilege.RoleId = Convert.ToInt32(Session["UserRole"].ToString());
                        getRolePrivilege.ModuleID = 12;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = Newtonsoft.Json.JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;

                    }
                    else if (Convert.ToString(Session["UserType"]) != "ADMINISTRATOR" || (int)Session["loggedInUserId"] == id)
                    {
                        ViewBag.AllowEdit = true;
                        ViewBag.AllowView = true;
                        ViewBag.AllowDelete = true;
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

                api = "CareTaker/GetCareTakerProfile/" + id;
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
                    UserSessionManager.SetCareTakerDocuments(this, null, "CaretakerCertificates");
                    UserSessionManager.SetCareTakerDocuments(this, null, "CaretakerSins");
                    UserSessionManager.SetCareTakerDocuments(this, null, "CaretakerOthers");

                    List<StateViewModel> stateList = new List<StateViewModel>();
                    api = "Admin/GetStatesByCountryId/" + caretakerProfile.CountryId;
                    var resultState = service.GetAPI(api);
                    stateList = JsonConvert.DeserializeObject<List<StateViewModel>>(resultState);
                    var _stateList = new SelectList(stateList, "StateId", "Name", 0);
                    ViewData["StateList"] = _stateList;

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
                Logger.Error(ex, "Error occurred in Caretaker Controller-CareTakerProfile");
                return null;
            }
        }

        public ActionResult CalendarView()
        {
                    
            int caretakerId = 0;
            try
            {
                if (Session["UserType"] == null)
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                if (Session["loggedInUserId"] != null)
                {
                    caretakerId = (int)Session["loggedInUserId"];
                }             
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Caretaker Controller-Notification");

            }

            return PartialView("_PublicUserCalendarViewPartial");
        }


        [HttpPost]
        public JsonResult GetBookingForCaretaker(int clientId)
        {

            List<UserBooking> userbookingDetails = null;
            int publicuserid = 0;
            try
            {

                if (Session["loggedInUserId"] != null)
                {
                    publicuserid = (int)Session["loggedInUserId"];
                }
                string api = "User/GetCaretakerBookingDetails/" + publicuserid;
                var result = service.GetAPI(api);
                userbookingDetails = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserBooking>>(result);
                userbookingDetails.ForEach(x =>
                {
                    x.Start = (DateTime.SpecifyKind(x.Start, DateTimeKind.Utc));
                    x.EndDate = (DateTime.SpecifyKind(x.EndDate, DateTimeKind.Utc));
                });

                return new JsonResult { Data = userbookingDetails, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Caretaker Controller-GetBookingForCaretaker");
                return null;
            }

        }

        [HttpPost]
        public JsonResult GetEventsByCareTaker(int caretakerId, int month, int year)
        {
            List<BookingSchedulingData> scheduleDetailsList = new List<BookingSchedulingData>();
            try
            {
                CalenderEventInput calenderEventInput = new CalenderEventInput
                {
                    ScheduleId = 0,
                    CaretakerId=caretakerId
                };
                if (year != 0 && month != 0)
                {
                    calenderEventInput.StartDate = new DateTime(year, month, 1);
                    calenderEventInput.EndDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                    calenderEventInput.EndDate = calenderEventInput.StartDate.AddMonths(1).AddSeconds(-1);
                }
                string api = "CareTaker/GetAllBookingSchedulingData";
                var data = JsonConvert.SerializeObject(calenderEventInput);
                var result = service.PostAPIWithData(data, api);
                scheduleDetailsList = JsonConvert.DeserializeObject<List<BookingSchedulingData>>(result.Result);
                scheduleDetailsList.ForEach(x =>
                {
                    x.Start = (DateTime.SpecifyKind(x.Start, DateTimeKind.Utc));
                });
                if (caretakerId != 0)
                {
                    var events = scheduleDetailsList.Where(a => a.CaretakerId == caretakerId);
                    return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    return new JsonResult { Data = scheduleDetailsList, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Caretaker Controller-GetEventsByCareTaker");
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

                    List<DocumentsList> lstDocumentDetails = new List<DocumentsList>();
                    lstDocumentDetails.AddRange(UserSessionManager.GetCareTakerDocuments(this, "CaretakerCertificates"));
                    lstDocumentDetails.AddRange(UserSessionManager.GetCareTakerDocuments(this, "CaretakerSins"));
                    lstDocumentDetails.AddRange(UserSessionManager.GetCareTakerDocuments(this, "CaretakerOthers"));
                    objCareTakerViewModel.CareTakerDocuments = lstDocumentDetails;

                    objCareTakerViewModel.SiteURL = Request.Url.Scheme + "://" + Request.Url.Authority + "/";
                    if (!Directory.Exists(Server.MapPath("~/PCMS/Documents/CareGiver/")))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/PCMS/Documents/CareGiver/"));
                    }

                    foreach (var item in lstDocumentDetails)
                    {
                        item.DocumentPath = Server.MapPath("~/PCMS/Documents/CareGiver/") + objCareTakerViewModel.CaretakerProfileId + "_" + item.DocumentType + "_" + item.DocumentName;
                        System.IO.File.WriteAllBytes(item.DocumentPath, item.DocumentContent);
                        string docPath = objCareTakerViewModel.SiteURL + "PCMS/Documents/CareGiver/" + objCareTakerViewModel.CaretakerProfileId + "_" + item.DocumentType + "_" + item.DocumentName;
                        item.DocumentPath = docPath;
                        item.DocumentContent = null;
                    }

                    //objCareTakerViewModel.CareTakerDocuments = lstDocumentDetails;
                    objCareTakerViewModel.CertificateFiles = null;
                    objCareTakerViewModel.SINFile = null;
                    objCareTakerViewModel.OtherDocuments = null;

                    // write profile pic to a folder and save its path in db.
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

                    var careTakerViewModel = JsonConvert.SerializeObject(objCareTakerViewModel);
                    HttpStatusCode resultPost = service.PostAPI(careTakerViewModel, api);
                    if (resultPost == HttpStatusCode.OK)
                    {
                        TempData["Success"] = "Caregiver Profile updated successfully.";
                    }
                    else if (resultPost == HttpStatusCode.Unauthorized)
                    {
                        TempData["Failure"] = "You are not authorized to perform this action.";
                    }
                    else if (resultPost == HttpStatusCode.Conflict)
                    {
                        TempData["Failure"] = "Data already exist. Please enter different data.";
                    }
                    else
                    {
                        TempData["Failure"] = "Updation Failed. Please try again later.";
                    }
                }
                if (Convert.ToInt32(Session["loggedInUserId"].ToString()) == objCareTakerViewModel.UserId)
                    Session["profilePic"] = objCareTakerViewModel.ProfilePicPath;
                UserSessionManager.SetCareTakerExperience(this, null);
                UserSessionManager.SetCareTakerQualification(this, null);
                UserSessionManager.SetCareTakerService(this, null);
                UserSessionManager.SetCareTakerClient(this, null);
                UserSessionManager.SetCareTakerDocuments(this, null, "CaretakerCertificates");
                UserSessionManager.SetCareTakerDocuments(this, null, "CaretakerSins");
                UserSessionManager.SetCareTakerDocuments(this, null, "CaretakerOthers");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Caretaker Controller-UpdateCaretaker");

            }
            return RedirectToAction("CaretakerProfile", new { id = objCareTakerViewModel.UserId });
        }

        [HttpPost]
        public JsonResult Download_Document_Click(int caretakerDocumentId, byte[] documentContent)
        {
            try
            {
                string sFilePath;
                sFilePath = System.IO.Path.GetTempFileName();
                System.IO.File.Move(sFilePath, System.IO.Path.ChangeExtension(sFilePath, ".pdf"));
                sFilePath = System.IO.Path.ChangeExtension(sFilePath, ".pdf");
                System.IO.File.WriteAllBytes(sFilePath, documentContent);//buffer
                //Action<string> act = new Action<string>(OpenPDFFile);
                //act.BeginInvoke(sFilePath, null, null);

                using (System.Diagnostics.Process p = new System.Diagnostics.Process())
                {
                    try
                    {
                        p.StartInfo = new System.Diagnostics.ProcessStartInfo(sFilePath);
                        p.Start();
                        p.WaitForExit();

                        System.IO.File.Delete(sFilePath);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "Error occurred in Caretaker Controller-Download_Document_Click");

                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Caretaker Controller-Download_Document_Click");
            }
            return Json(null);
        }

        public ActionResult CaretakerBookingHistory()
        {
            string api = string.Empty;
            if (Session["UserType"] != null)
            {
                if (Convert.ToString(Session["UserType"]).ToLower() != "caretaker")
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
                TempData["Failure"] = Constants.NotLoggedIn;
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        public ActionResult CaretakerRejectedHistory()
        {
            string api = string.Empty;
            if (Session["UserType"] != null)
            {
                if (Convert.ToString(Session["UserType"]).ToLower() != "caretaker")
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
                TempData["Failure"] = Constants.NotLoggedIn;
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
       
        public ActionResult ServiceRequest()
        {
            List<BookingHistory> bookingList = new List<BookingHistory>();
            return PartialView("_UserserviceRequestPartial", bookingList);
        }
        public ActionResult CaretakerPieChart()
        {
            try
            {
                return PartialView("_CaretakerPieChartPartial");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Caretaker Controller-CaretakerPieChart");
                return null;
            }
        }
        public ActionResult CaretakerBarChart()
        {
            try
            {
                return PartialView("_CaretakerbarChartPartial");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Caretaker Controller-CaretakerBarChart");
                return null;
            }
        }
        public ActionResult CaretakerCalendar()
        {
            try
            {
                return PartialView("_CaretakerCalendarPartial");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Caretaker Controller-CaretakerCalendar");
                return null;
            }
        }
        public ActionResult CaretakerNotification()
        {
            try
            {
                return PartialView("_CaretakerNotificationPartial");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Caretaker Controller-CaretakerNotification");
                return null;
            }
        }
        public ActionResult CaretakerDashboardView()
        {
            if (Session["UserType"] != null)
            {
                if (Convert.ToString(Session["UserType"]) != "CARETAKER")
                {
                    TempData["Error"] = Constants.NoViewPrivilege;
                    return RedirectToAction("Error", "Admin");
                }
                UpcomingAppointment upcomingNotifications = null;
                int caretakerId = 0;
                try
                {
                    if (Session["loggedInUserId"] != null)
                    {
                        caretakerId = (int)Session["loggedInUserId"];
                    }

                    string api = "CareTaker/GetUpcomingNotifications/" + caretakerId;
                    var result = service.GetAPI(api);
                    upcomingNotifications = Newtonsoft.Json.JsonConvert.DeserializeObject<UpcomingAppointment>(result);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error occurred in Caretaker Controller-Upcoming Notification");

                }
                return View(upcomingNotifications);
            }
            else
            {
                TempData["Failure"] = Constants.NotLoggedIn;
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult ConfirmAppointments(UpcomingAppointment upcomingAppointment)
        {
            try
            {
                if (Session["UserType"] == null)
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                upcomingAppointment.SiteURL = string.Format("{0}://{1}/", Request.Url.Scheme, Request.Url.Authority);
                var options = JsonConvert.SerializeObject(upcomingAppointment);
                string api = "CareTaker/ConfirmAppointments";
                HttpStatusCode schedulingresult = service.PostAPI(options, api);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Caretaker Controller-Upcoming Notification");

            }
            return RedirectToAction("CaretakerDashboardView", "CareTaker");
        }


        public ActionResult Notification()
        {
            List<Notification> notifications = null;
            int caretakerId = 0;
            try
            {
                if (Session["UserType"] == null)
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                if (Session["loggedInUserId"] != null)
                {
                    caretakerId = (int)Session["loggedInUserId"];
                }
                string api = "CareTaker/GetNotification/" + caretakerId;
                var result = service.GetAPI(api);
                notifications = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Notification>>(result);

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Caretaker Controller-Notification");

            }

            return PartialView("_CaretakerDashboardNotificationPartial", notifications);
        }
        public ActionResult ClientScheduling(int? clientId, string fromDate, string toDate)
        {
            string api = string.Empty;
            List<CaretakerScheduleList> schedulingDetails = null;
            try
            {
                int caretakerId = 0;
                if (Session["UserType"] == null)
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                if (Session["loggedInUserId"] != null)
                {
                    caretakerId = (int)Session["loggedInUserId"];
                }
                CaretakerScheduleListSearch searchOptions = new CaretakerScheduleListSearch()
                {
                    CaretakerId = caretakerId,
                    ClientId = clientId,
                    FromDate = (fromDate != "") ? Convert.ToDateTime(fromDate) : (DateTime?)null,
                    ToDate = (toDate != "") ? Convert.ToDateTime(toDate) : (DateTime?)null
                };


                api = "CareTaker/GetCaretakerScheduleList/";
                var careTakerSearch = JsonConvert.SerializeObject(searchOptions);
                var result = service.PostAPIWithData(careTakerSearch, api);
                schedulingDetails = JsonConvert.DeserializeObject<List<CaretakerScheduleList>>(result.Result);

                for (int i = 0; i < schedulingDetails.Count; i++)
                {
                    string s = schedulingDetails[i].StartDateTime.ToString("dd/MM/yyyy h:mm tt");
                    System.Globalization.CultureInfo customCulture = new System.Globalization.CultureInfo("en-US", true);

                    customCulture.DateTimeFormat.ShortDatePattern = "MM/dd/yyyy";

                    System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
                    System.Threading.Thread.CurrentThread.CurrentUICulture = customCulture;

                    DateTime dt = DateTime.ParseExact(s, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture);
                    DateTime newDate = System.Convert.ToDateTime(dt.ToString("yyyy-MM-dd h:mm tt"));

                    schedulingDetails[i].StartDateTime = dt;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Caretaker Controller-ClientScheduling");

            }

            return PartialView("_CaretakerClientSchedulingPartial", schedulingDetails);
        }
        public ActionResult UserBooking(string fromDate, string toDate)
        {
            List<UserBooking> bookingDetails = null;
            int caretakerId = 0;
            try
            {
                if (Session["UserType"] == null)
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                if (Session["loggedInUserId"] != null)
                {
                    caretakerId = (int)Session["loggedInUserId"];
                }
                CaretakerScheduleListSearch searchOptions = new CaretakerScheduleListSearch()
                {
                    CaretakerId = caretakerId,
                    ClientId = null,
                    FromDate = (fromDate != "") ? Convert.ToDateTime(fromDate) : (DateTime?)null,
                    ToDate = (toDate != "") ? Convert.ToDateTime(toDate) : (DateTime?)null
                };

                string api = "CareTaker/GetUserBookingDetails/" + caretakerId;
                var careTakerSearch = JsonConvert.SerializeObject(searchOptions);
                var result = service.PostAPIWithData(careTakerSearch, api);
                bookingDetails = JsonConvert.DeserializeObject<List<UserBooking>>(result.Result);

                for (int i = 0; i < bookingDetails.Count; i++)
                {
                    string s = bookingDetails[i].BookingDate.ToString("dd/MM/yyyy h:mm tt");
                    System.Globalization.CultureInfo customCulture = new System.Globalization.CultureInfo("en-US", true);

                    customCulture.DateTimeFormat.ShortDatePattern = "MM/dd/yyyy";

                    System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
                    System.Threading.Thread.CurrentThread.CurrentUICulture = customCulture;

                    DateTime dt = DateTime.ParseExact(s, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture);
                    bookingDetails[i].BookingDate = dt;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Caretaker Controller-UserBooking");

            }

            return PartialView("_CaretakerDashboardUserBookingsPartial", bookingDetails);
        }
        public ActionResult ScheduleRejected(string fromDate, string toDate)
        {
            List<RejectedCaretaker> bookingDetails = null;
            List<RejectedCaretaker> bookingDetailsfiltered = null;

            int caretakerId = 0;
            try
            {
                if (Session["UserType"] == null)
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                if (Session["loggedInUserId"] != null)
                {
                    caretakerId = (int)Session["loggedInUserId"];
                }
                CaretakerScheduleListSearch searchOptions = new CaretakerScheduleListSearch()
                {
                    CaretakerId = caretakerId,
                    ClientId = null,
                    FromDate = (fromDate != "") ? Convert.ToDateTime(fromDate) : (DateTime?)null,
                    ToDate = (toDate != "") ? Convert.ToDateTime(toDate) : (DateTime?)null
                };

                string api = "Client/GetAllScheduleRejectedCaretaker";
                var advancedSearchInputModel = JsonConvert.SerializeObject(searchOptions);
                var result = service.PostAPIWithData(advancedSearchInputModel, api);
                bookingDetails = JsonConvert.DeserializeObject<List<RejectedCaretaker>>(result.Result);
                bookingDetailsfiltered = bookingDetails.FindAll(x => x.CareTakerId == caretakerId);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Caretaker Controller-UserBooking");

            }
            return PartialView("_ScheduleRejectedCaretakerPartial", bookingDetailsfiltered);
        }
        
        public ActionResult SchedulingCalendar()
        {
            return PartialView("_CaretakerCalendarViewPartial");
        }

        public ActionResult ViewNotificationDetails(int bookingId)
        {
            NotificationDetails viewNotificationDetails = null;
            try
            {
                string api = "CareTaker/GetNotificationDetailsById/" + bookingId;
                var result = service.GetAPI(api);
                viewNotificationDetails = Newtonsoft.Json.JsonConvert.DeserializeObject<NotificationDetails>(result);

                List<Questionare> questions = new List<Questionare>();
                api = "Admin/GetQuestions/0";
                result = service.GetAPI(api);
                questions = JsonConvert.DeserializeObject<List<Questionare>>(result);
                ViewBag.Questions = questions;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in Caretaker Controller-UserBooking");

            }
            return PartialView("_NotificationDetailsPartial", viewNotificationDetails);
        }

        public ActionResult AvailableCaretakerReport()
        {
            ClientModel clientModelObj = new ClientModel();
            string country = Session["CountryId"].ToString();
            string state = Session["StateID"].ToString();
            string CityIdk = Session["CityIdk"].ToString();
            string api = string.Empty;
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

        public ActionResult CaretakerPayRollDetails()
        {
            ClientModel clientModelObj = new ClientModel();
            string country = Session["CountryId"].ToString();
            string state = Session["StateID"].ToString();
            string CityIdk = Session["CityIdk"].ToString();
            string api = string.Empty;
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
        public ActionResult CaretakerPayRollSummary()
        {
            ClientModel clientModelObj = new ClientModel();
            string country = Session["CountryId"].ToString();
            string state = Session["StateID"].ToString();
            string CityIdk = Session["CityIdk"].ToString();
            string api = string.Empty;
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

        public ActionResult CaretakerWiseScheduleReport()
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

        [HttpPost]
        public JsonResult GetCaretakerPayRiseRates(int CareTakerId)
        {
            string api = "Caretaker/GetCaretakerPayRiseRates/" +CareTakerId;
            var result = service.GetAPI(api);
            return Json(result);
        }
        [HttpPost]
        public JsonResult GetCaretakerPayRiseRatesonDateChange(DateTime Date,int CaretakerId)
        {

            PayriseData bookingPayriseData = new PayriseData()
            {
               CaretakerId = CaretakerId,
               Date= Date
            };
            string api = "Caretaker/GetCaretakerPayRiseRatesonDateChange";
            var getcareTakerPayRise = JsonConvert.SerializeObject(bookingPayriseData);
            var result = service.PostAPIWithData(getcareTakerPayRise, api);
            return Json(result);
        }
    }
}
