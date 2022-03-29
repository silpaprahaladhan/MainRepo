using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nirast.Pcms.Web.Models;
using Nirast.Pcms.Web.Helpers;
using Newtonsoft.Json;
using System.IO;
using System.Configuration;
using PayPal.Api;
using Nirast.Pcms.Web.Configuration;
using System.Net;
using PayPal;
using Nirast.Pcms.Web.Logger;
using static Nirast.Pcms.Web.Models.Enums;
using Newtonsoft.Json.Linq;
using System.Web.UI;
using static Nirast.Pcms.Web.Models.PublicUserCaretakerBooking;
using System.Reflection;
using System.DirectoryServices.Protocols;
using System.ComponentModel;

namespace Nirast.Pcms.Web.Controllers
{
    [OutputCache(Duration = 1800, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
    public class PublicUserController : Controller
    {
        private PayPal.Api.Payment payment;
        PCMSLogger pCMSLogger = new PCMSLogger();
        Service service = null;
        DateTimeHelper dateTimeHelper = new DateTimeHelper();

        public PublicUserController()
        {
            service = new Service(pCMSLogger);
        }
        public ActionResult DeleteEvent(ScheduleDeleteData data)
        {
            try
            {
                if (Session["UserType"] == null)
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        TempData["Failure"] = Constants.NoViewPrivilege;
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
                            TempData["Failure"] = Constants.NoViewPrivilege;
                            return new JsonResult { Data = new { status = false } };
                        }

                        ViewBag.AllowDelete = apiResults.AllowDelete;
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                    }
                    if (Convert.ToString(Session["UserType"]) == "ADMINISTRATOR") ViewBag.AllowEdit = true;
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                    //return new JsonResult { Data = new { status = false } };
                }
                string siteUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/";
                data.SiteURL = siteUrl;
                data.UserId = (int)Session["loggedInUserId"];
                data.AuditLogType = AuditLogType.Scheduling;
                data.AuditLogActionType = AuditLogActionType.Delete;
                api = "User/DeleteSchedule";
                var deleteData = JsonConvert.SerializeObject(data);
                var result = service.PostAPIWithData(deleteData, api);
                if (result.Result == "success")
                {
                    TempData["EventSuccess"] = "Event deleted Successfully";
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


        // GET: PublicUser
        public ActionResult UserRegistration()
        {
            try
            {
                ViewData["Month"] = dateTimeHelper.GetMonthList();

                ViewData["Year"] = dateTimeHelper.GetYearList();

                ViewData["Year"] = GetYears(); ;

                List<CountryViewModel> listCountry = new List<CountryViewModel>();
                string apiCountry = "Admin/GetCountryDetails/0";
                var countryResult = service.GetAPI(apiCountry);
                listCountry = JsonConvert.DeserializeObject<List<CountryViewModel>>(countryResult);
                int defaultCountry = (listCountry.Where(x => x.Isdefault == true).Count() > 0) ? listCountry.Where(x => x.Isdefault == true).SingleOrDefault().CountryId : listCountry.FirstOrDefault().CountryId;
                var _listCountry = new SelectList(listCountry, "CountryId", "Name", defaultCountry);
                string phoneCode = (listCountry.Where(x => x.Isdefault == true).Count() > 0) ? listCountry.Where(x => x.Isdefault == true).SingleOrDefault().PhoneCode : listCountry.FirstOrDefault().PhoneCode;
                ViewBag.PhoneCode = phoneCode == string.Empty ? "+1" : phoneCode;
                ViewData["CountryList"] = _listCountry;
                ViewBag.Country = _listCountry;

                //List<StateViewModel> stateList = new List<StateViewModel>();
                //var api = "/Admin/GetStatesByCountryId/" + defaultCountry;
                //var stateResult = service.GetAPI(api);
                //stateList = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult);
                //stateList.Insert(0, new StateViewModel { StateId = 0, Name = "--Select--" });

                ////var _stateList = new SelectList(stateList, "StateId", "Name", 0);
                //ViewBag.States = new SelectList(stateList, "StateId", "Name", 0);

                //List<Cities> cityList = new List<Cities>();
                //cityList.Add(new Cities { CityId = 0, CityName = "--Select--" });
                //ViewBag.Cities = new SelectList(cityList, "CityId", "CityName", 0);

                var listCardType = getCardType();
                var newListcardType = new SelectList(listCardType, "CardTypeId", "CardType");

                ViewData["CardTypeList"] = newListcardType;
                // ViewData["ListCardType"] = cardType;

                // ViewBag.CardType = cardType;
                //ViewBag.Month = month;
                ViewBag.Year = GetYears();

            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in PublicUser Controller-UserRegistration");

            }
            return View();
        }
        public JsonResult LoadAvailableCaretakers(int categoryId, string startDatetime, int Workshift, int totalhours = 0)
        {
            try
            {

                string api = "CareTaker/GetAvailableCareTakerListforPublicUser/" + categoryId + "?startDatetime=" + startDatetime + "&hours=" + totalhours  + "&Workshift=" + Workshift;
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
        public ActionResult SaveCareRecipientDetails(CaretakerBooking caretakerBooking)
       {
            int caregiverId = Convert.ToInt32(ConfigurationManager.AppSettings["CaregiverId"]);
            caretakerBooking.CareTakerId = caregiverId;
            try
            {
                DateTime reportStart = DateTime.Now;
                DateTime reportend = DateTime.Now;

                if (Session["UserType"] == null)
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }

                if (Request.Form["SubmitBooking"] != null)
                {
                    caretakerBooking.BookingDate = DateTime.UtcNow;
                    caretakerBooking.CareTakerId = caretakerBooking.CareTakerId;
                    caretakerBooking.BookedUserId = Convert.ToInt32(Session["loggedInUserId"]);
                    caretakerBooking.ServiceRequiredId = caretakerBooking.ServiceRequiredId; //Service Id
                    caretakerBooking.Status = (int)BookingStatus.PendingBookingAcceptance;

                    reportStart = caretakerBooking.BookingStartTime;
                    reportend = caretakerBooking.BookingEndTime;
                    List<BookigDate> listBookingDates = new List<BookigDate>();
                    if (reportend.Date > reportStart.Date)
                    {
                        int reportDateDiff = reportend.Date.Subtract(reportStart.Date).Days;
                        BookigDate test = new BookigDate();
                        List<DateTime> scheduledDate = Enumerable.Range(0, 1 + reportend.Date.Subtract(reportStart.Date).Days).Select(offset => reportStart.Date.AddDays(offset)).ToList();
                        foreach (DateTime item in scheduledDate)
                        {
                            BookigDate objschedulingDate = new BookigDate();
                            objschedulingDate.Date = item;

                            var hours = reportend.Subtract(reportStart);
                            var startDayhours = TimeSpan.FromHours(24) - reportStart.TimeOfDay;
                            var endDayhours = hours - startDayhours;

                            if (item.Date == reportStart.Date)
                            {
                                objschedulingDate.Hours = startDayhours.TotalHours;
                            }
                            else if (item.Date == reportend.Date)
                            {
                                objschedulingDate.Hours = endDayhours.TotalHours;
                            }
                            else
                            {
                                objschedulingDate.Hours = 24;
                            }

                            listBookingDates.Add(objschedulingDate);
                        }
                    }
                    else
                    {
                        int reportDateDiff = reportend.Date.Subtract(reportStart.Date).Days;
                        SchedulingDate test = new SchedulingDate();
                        List<DateTime> scheduledDate = Enumerable.Range(0, 1 + reportend.Date.Subtract(reportStart.Date).Days).Select(offset => reportStart.Date.AddDays(offset)).ToList();
                        foreach (DateTime item in scheduledDate)
                        {
                            BookigDate objschedulingDate = new BookigDate();
                            objschedulingDate.Date = item;
                            var hours = reportend.Subtract(reportStart);
                            objschedulingDate.Hours = hours.TotalHours;
                            listBookingDates.Add(objschedulingDate);
                        }
                    }

                    caretakerBooking.PublicUserBookigDates = listBookingDates;

                    CareRecipientQuestionare questionare = new CareRecipientQuestionare();
                    if (!string.IsNullOrEmpty(Request.Form["txtServicesRequired"].ToString()))
                    {
                        questionare.Answer1 = Request.Form["txtServicesRequired"].ToString();
                    }

                    if (!string.IsNullOrEmpty(Request.Form["txtDiseases"].ToString()))
                    {
                        questionare.Answer2 = Request.Form["txtDiseases"].ToString();
                    }

                    if (!string.IsNullOrEmpty(Request.Form["txtMedicationHistory"].ToString()))
                    {
                        questionare.Answer3 = Request.Form["txtMedicationHistory"].ToString();
                    }

                    if (!string.IsNullOrEmpty(Request.Form["txtAllergy"].ToString()))
                    {
                        questionare.Answer4 = Request.Form["txtAllergy"].ToString();
                    }

                    if (!string.IsNullOrEmpty(Request.Form["txtExtraService"].ToString()))
                    {
                        questionare.Answer5 = Request.Form["txtExtraService"].ToString();
                    }

                    if (!string.IsNullOrEmpty(Request.Form["txtRemarks"].ToString()))
                    {
                        questionare.Answer6 = Request.Form["txtRemarks"].ToString();
                    }

                    caretakerBooking.Questionaire = questionare;
                    caretakerBooking.SiteURL = Request.Url.Scheme + "://" + Request.Url.Authority + "/";
                    string api = "PublicUser/SaveCaretakerBooking";
                    var serviceContent = JsonConvert.SerializeObject(caretakerBooking);
                    HttpStatusCode result = service.PostAPI(serviceContent, api);
                    if (result == HttpStatusCode.OK)
                    {
                        TempData["SaveBooking"] = "Booking Details Recorded Successfully";
                        NotificationHub.Static_Send("notify", "New Booking for a Caregiver", "static");
                    }
                    else if (result == HttpStatusCode.Unauthorized)
                    {
                        TempData["SaveBooking"] = "You are not authorized to perform this action";
                    }
                    else if(result == HttpStatusCode.Found)
                    {
                        TempData["SaveBooking"] = "Booking already exist for this Caregiver!";
                    }
                    else
                    {
                        TempData["SaveBooking"] = "Booking failed";
                    }

                    //EmailInput emailInput = new EmailInput();
                    //emailInput.Body = "Hi " + caretakerBooking.FirstName + " " + caretakerBooking.LastName + ", <BR/><BR/>" +
                    //    "This mail is to inform you that we have recieved your booking request for <b>" + caretakerBooking.CareTakerId;

                }
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in PublicUser Controller-SaveCareRecipientDetails");

            }
            return RedirectToAction("PublicUserBooking", new { serviceId = caretakerBooking.ServiceRequiredId });
        }

        public ActionResult CreatePublicUser(PublicUserRegistration publicUserDetails)
        {
            try
            {
                int num = new Random().Next(1000, 9999);
                string numm = Convert.ToString(num);
                publicUserDetails.EmployeeNumber = numm;
                
                var response = Request["g-recaptcha-response"];
                string secretKey = "6LcGiGMUAAAAALSs8J4ZJNqIVM2ej-5dvbf_wP5q";
                var client = new WebClient();
                var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));
                var obj = JObject.Parse(result);
                var sts = (bool)obj.SelectToken("success");
                string encryptionPassword = ConfigurationManager.AppSettings["EncryptPassword"].ToString();
                string passwordEpt = StringCipher.Encrypt(publicUserDetails.Password, encryptionPassword);
                publicUserDetails.Password = passwordEpt;
                publicUserDetails.UserTypeId = 2;
                publicUserDetails.UserStatus = UserStatus.Active;
                publicUserDetails.GenderId = Convert.ToInt32(Gender.Male);
                publicUserDetails.SiteURL = Request.Url.Scheme + "://" + Request.Url.Authority + "/";

                // write profile pic to a folder and save its path in db.
                Guid fileName = Guid.NewGuid();
                if (publicUserDetails.ProfilePicByte != null)
                {
                    string FilePath = Server.MapPath("~/PCMS/ProfilePics/PublicUser/")  + fileName.ToString() + ".jpg";
                    if (!Directory.Exists(Server.MapPath("~/PCMS/ProfilePics/PublicUser/")))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/PCMS/ProfilePics/PublicUser/"));
                    }
                    System.IO.File.WriteAllBytes(FilePath, publicUserDetails.ProfilePicByte);
                    publicUserDetails.ProfilePicByte = null;
                    FilePath = publicUserDetails.SiteURL + "PCMS/ProfilePics/PublicUser/" + fileName.ToString() + ".jpg";
                    publicUserDetails.ProfilePicPath = FilePath;
                }

                string api = "PublicUser/AddPublicUser";
                var serviceContent = JsonConvert.SerializeObject(publicUserDetails);
                //bool result = service.PostAPI(serviceContent, api);
                var data = service.PostAPIWithData(serviceContent, api);
                NotificationHub.Static_Send("notify", "New Public User Registered", "static");
                TempData["Success"] = "User registration completed successfully. Please verify your email using the verification link sent to your registered email: " + publicUserDetails.EmailAddress;
                //TempData["Success"] = "Registered Successfully, Email verification link has been sent to your registered email: " + publicUserDetails.EmailAddress;
                api = "Users/SendVerificationEmail";//+ publicUserDetails.EmailAddress
                string url = string.Format("{0}://{1}/Home/EmailVerificationSuccess/{2}", Request.Url.Scheme, Request.Url.Authority, HttpUtility.UrlEncode(StringCipher.EncodeNumber(Convert.ToInt32(data.Result))));
                #region created by silpa for ldap sign in

                System.DirectoryServices.Protocols.LdapConnection ldapConnection = new System.DirectoryServices.Protocols.LdapConnection("127.0.0.1:10389");

                var networkCredential = new NetworkCredential(
                      "employeeNumber=1,ou=Aluva,ou=Ernakulam,ou=kerala,ou=India,o=TopCompany",
                      "secret");

                ldapConnection.SessionOptions.ProtocolVersion = 3;

                ldapConnection.AuthType = AuthType.Basic;
                ldapConnection.Bind(networkCredential);


                var request = new AddRequest("employeeNumber=" + numm + ",ou="+ publicUserDetails.BranchId1Name+ ",ou="+publicUserDetails.CityId1Name+",ou="+publicUserDetails.StateId1Name+",ou="+publicUserDetails.CountryId1Name+",o=TopCompany", new DirectoryAttribute[] {
                    new DirectoryAttribute("employeeNumber", numm),
                    new DirectoryAttribute("cn", publicUserDetails.FirstName),
                      new DirectoryAttribute("uid", publicUserDetails.EmployeeNumber),
                     new DirectoryAttribute("sn", publicUserDetails.LastName),
                    new DirectoryAttribute("userPassword", publicUserDetails.Password),
                    new DirectoryAttribute("objectClass", new string[] { "top", "person", "organizationalPerson","inetOrgPerson" })
                });
                ldapConnection.SendRequest(request);




                #endregion
                VerifyEmail verifyEmail = new VerifyEmail
                {
                    WelcomeMsg = "Welcome to Tranquil Care!",
                    FirstName = publicUserDetails.FirstName,
                    MailMsg = "Thank you for registering with us, you have successfully created an account with us.",
                    Mailcontent = url,
                    ContactNo = "1-800-892-6066",
                    RegardsBy = "Tranquil Care Inc.",
                    siteUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/",
                    CompanyName_TagLine = "Tranquil Care - Delivering Care Excellence.",
                    CompanyName = "Tranquil Care Inc.",
                    Subject = "Email Verification Link",
                    Email = publicUserDetails.EmailAddress,
                    UserId = Convert.ToInt32(data.Result)
                };
                serviceContent = JsonConvert.SerializeObject(verifyEmail);
                var otpResult = service.PostAPIWithData(serviceContent, api);
                return RedirectToAction("UserRegistration");
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in PublicUser Controller-CreatePublicUser");
                return RedirectToAction("UserRegistration");
            }
        }

        [HttpPost]
        public JsonResult SendUserInvoiceEmail(string username, string email, int fileId)
        {
            try
            {
                InvoiceHistory searchInputs = new InvoiceHistory();
                List<UserInvoiceParams> bookingDetailsList = new List<UserInvoiceParams>();
                List<UserInvoiceParams> bookingDetailsListfiltered = new List<UserInvoiceParams>();
                string api = "Admin/GetBookingInvoiceList";
                searchInputs.InvoiceSearchInputId = fileId;
                var advancedSearchInputModel = JsonConvert.SerializeObject(searchInputs);
                var result = service.PostAPIWithData(advancedSearchInputModel, api);
                bookingDetailsList = JsonConvert.DeserializeObject<List<UserInvoiceParams>>(result.Result);
             

                if (bookingDetailsList != null)
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
                    string filenname = bookingDetailsList[0].InvoicePath;

                    EmailInput emailinputs = new EmailInput()
                    {
                        EmailType = EmailType.Invoice,
                        Attachments = filenname,
                        Body = GetEmailBody(username),
                        Subject = "Invoice ",
                        EmailId = email,
                        UserId = 1//need default or corresponding userId
                    };
                    api = "Admin/SendEmailNotification";
                    var serviceContent = JsonConvert.SerializeObject(emailinputs);
                    var results = service.PostAPIWithData(serviceContent, api);
                }
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in PublicUser Controller-SendUserEmail");
            }
            return Json(string.Empty);
        }
        private string GetEmailBody(string userName)
        {
            try
            {
                string url = "https://tranquilcare.ca/Account/Login";
                string WelcomeMsg = "PublicUser Invoice.";
                string MailMsg = "Invoice Details.<br/>";

                string Mailcontent = @" <center>Thank you for choosing to work with us.</center><br/><center> We are attaching the copy of the invoice.</center><br/>
                                                    <div></div>
                                                 <div style = 'font-family:Roboto,Tahoma,Arial,Helvetica,sans-serif; text-align: center;'>
                                                 <a href = '" + url + "' style = 'display:inline-block;background-color:#37abc8;color:#ffffff;font-size:1.2em;font-weight:300;text-decoration:none;padding:13px 25px 13px 25px;border-radius:10px' target = '_blank'> Click Here to login to your TranquilCare account to proceed payment<a></div>";

                string ContactNo = "1-800-892-6066";
                string RegardsBy = "Tranquil Care Inc.";
                string siteUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/";
                string CompanyName_TagLine = "Tranquil Care - Delivering Care Excellence.";
                string CompanyName = "Tranquil Care Inc.";
                string body = "";
                string sd = AppDomain.CurrentDomain.BaseDirectory + "EmailTemplates/EmailCommon.html";
                var sr = new StreamReader(sd);
                body = sr.ReadToEnd();
                body = string.Format(body, WelcomeMsg, userName, MailMsg, Mailcontent, ContactNo, RegardsBy, siteUrl, CompanyName_TagLine, CompanyName);
                return body;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public string CheckLoginNameExist(string loginName)
        {
            try
            {
                var api = "User/GetCheckLoginNameAlreadyExist/" + loginName + "/";
                var result = service.GetAPI(api);
                if (Convert.ToInt32(result) > 0)
                {
                    return "N";
                }
                else
                {
                    return "Y";
                }

            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in PublicUser Controller-CheckLoginNameExist");
            }
            return "";
        }
        public ActionResult AddInvoiceDetails(PublicUserPaymentInvoiceInfo clientModel)
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
                        TempData["Failure"] = Constants.NoViewPrivilege;
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
                api = "User/AddInvoiceDetails";
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
            return RedirectToAction("PublicUserInvoiceSettings");
        }

        public ActionResult UpdateUserDetails(PublicUserRegistration publicUserDetails)
        {
            try
            {
                if (Request.Form["update"] != null)
                {
                    publicUserDetails.GenderId = Convert.ToInt32(Gender.Male);
                    //string strCountryValue = Request.Form["ddlCountry"].ToString();
                    //string strStatesValue = Request.Form["ddlStates"].ToString();
                    //string strCityValue = Request.Form["ddlCity"].ToString();
                    //publicUserDetails.CountryId = Convert.ToInt32(strCountryValue);
                    //publicUserDetails.StateId = Convert.ToInt32(strStatesValue);
                    //publicUserDetails.CityId = Convert.ToInt32(strCityValue);
                    publicUserDetails.SiteURL = Request.Url.Scheme + "://" + Request.Url.Authority + "/";

                    // write profile pic to a folder and save its path in db.
                    Guid fileName = Guid.NewGuid();
                    if (publicUserDetails.ProfilePicByte != null)
                    {
                        string FilePath = Server.MapPath("~/PCMS/ProfilePics/PublicUser/") + fileName.ToString() + ".jpg";
                        if (!Directory.Exists(Server.MapPath("~/PCMS/ProfilePics/PublicUser/")))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/PCMS/ProfilePics/PublicUser/"));
                        }
                        System.IO.File.WriteAllBytes(FilePath, publicUserDetails.ProfilePicByte);
                        publicUserDetails.ProfilePicByte = null;
                        FilePath = publicUserDetails.SiteURL + "PCMS/ProfilePics/PublicUser/" + fileName.ToString() + ".jpg";
                        publicUserDetails.ProfilePicPath = FilePath;
                    }
                    string api = "PublicUser/UpdateUserDetails";
                    var serviceContent = JsonConvert.SerializeObject(publicUserDetails);
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
                    if (Convert.ToInt32(Session["loggedInUserId"].ToString()) == publicUserDetails.UserRegnId)
                        Session["profilePic"] = publicUserDetails.ProfilePicPath;
                }
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in PublicUser Controller-UpdateUserDetails");
            }
            return RedirectToAction("UserProfile", new { id = publicUserDetails.UserRegnId });
        }
        public ActionResult PublicUserBooking(int serviceId)
        {
            try
            {
                if (Session["UserType"] != null)
                {
                    CountryViewModel countryViewModel = new CountryViewModel();
                    string apiCountry = "Admin/GetCountryDetails/0";
                    var countrylist = service.GetAPI(apiCountry);
                    var newcountryList = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist);
                    int defaultCountry = (newcountryList.Where(x => x.Isdefault == true).Count() > 0) ? newcountryList.Where(x => x.Isdefault == true).SingleOrDefault().CountryId : newcountryList.FirstOrDefault().CountryId;
                    var _listCountry = new SelectList(newcountryList, "CountryId", "Name", defaultCountry);
                    //var _countryList = new SelectList(newcountryList, "CountryId", "Name", 1);
                    ViewBag.Country = _listCountry;

                    List<StateViewModel> stateList = new List<StateViewModel>();
                    var api = "/Admin/GetStatesByCountryId/" + defaultCountry;
                    var stateResult = service.GetAPI(api);
                    stateList = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult);
                    var _stateList = new SelectList(stateList, "StateId", "Name", 0);
                    ViewBag.States = _stateList;

                    List<Questionare> questions = new List<Questionare>();
                    api = "Admin/GetQuestions/0";
                    var result = service.GetAPI(api);
                    questions = JsonConvert.DeserializeObject<List<Questionare>>(result);
                    ViewBag.Questions = questions;

                    List<ServicesViewModel> listService = new List<ServicesViewModel>();
                    string serviceApi = "Admin/RetrieveServiceDetails/0";
                    var serviveResult = service.GetAPI(serviceApi);
                    listService = JsonConvert.DeserializeObject<List<ServicesViewModel>>(serviveResult);
                    ViewBag.Service = new SelectList(listService, "ServiceId", "Name", 0);
                    ViewBag.ServiceId = serviceId;

                    string apicategory = "Admin/GetCategory?flag=*&value=''";
                    var categoryResult = service.GetAPI(apicategory);
                    List<CategoryViewModel> listCategory = new List<CategoryViewModel>();
                    listCategory = JsonConvert.DeserializeObject<List<CategoryViewModel>>(categoryResult);
                    ViewBag.Category = new SelectList(listCategory, "CategoryId", "Name", 0);
                }
                else
                {
                    TempData["Failure"] = Constants.PublicUserNotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Failed to process Public User Booking page");
            }
            return View();

        }
        public ActionResult UpdateCardDetails(PublicUserRegistration publicUserDetails)
        {
            try
            {
                if (Request.Form["UpdateCard"] != null)
                {
                    string strCardId = Request.Form["ddlCardType"].ToString();
                    //publicUserDetails.CardTypeId = Convert.ToInt32(strCardId);
                    string api = "PublicUser/UpdateCardDetails";
                    var serviceContent = JsonConvert.SerializeObject(publicUserDetails);
                    HttpStatusCode result = service.PostAPI(serviceContent, api);
                    TempData["Userupdate"] = "Card details updated Successfully";
                }
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in PublicUser Controller-UpdateCardDetails");
            }
            return RedirectToAction("UserProfile", new { id = publicUserDetails.UserRegnId });
        }
        public ActionResult FileUpload(HttpPostedFileBase file, PublicUserRegistration publicUserDetails)
        {
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string extension = Path.GetExtension(file.FileName);
                    string imageName = "UserPic1" + extension;
                    string path = System.IO.Path.Combine(
                                           Server.MapPath("~/images/"), imageName);
                    // file is uploaded
                    file.SaveAs(path);

                    // save the image path path to the database or you can send image 
                    // directly to database
                    // in-case if you want to store byte[] ie. for DB
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                        string api = "PublicUser/UploadUserProfilePic";
                        publicUserDetails.ProfilePicByte = array;
                        var serviceContent = JsonConvert.SerializeObject(publicUserDetails);
                        HttpStatusCode result = service.PostAPI(serviceContent, api);
                    }

                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in PublicUser Controller-FileUpload");
            }
            // after successfully uploading redirect the user
            return RedirectToAction("UserProfile", new { id = publicUserDetails.UserRegnId });
        }

        private List<CardTypeViewModel> getCardType()
        {
            try
            {
                var listCardType = Enum.GetValues(typeof(UserCardType))
                   .Cast<UserCardType>()
                   .Select(t => new CardTypeViewModel
                   {
                       CardTypeId = ((int)t),
                       CardType = t.ToString()
                   }).ToList();
                return listCardType;
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in PublicUser Controller-getCardType");
                return null;
            }
        }

        public ActionResult UserProfile(int id)
        {
            List<PublicUserRegistration> PublicUserList = new List<PublicUserRegistration>();
            PublicUserRegistration userRegistration = new PublicUserRegistration();
            try
            {
                string api = string.Empty;
                if (Session["UserType"] == null)
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR" && Convert.ToInt32(Session["loggedInUserId"].ToString()) != id)
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
                        getRolePrivilege.ModuleID = 17;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = Newtonsoft.Json.JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
                            return View();
                        }
                        ViewBag.AllowEdit = apiResults.AllowEdit;
                        ViewBag.AllowView = apiResults.AllowView;

                    }
                    else if (Convert.ToString(Session["UserType"]) == "PUBLIC USER" || Convert.ToString(Session["UserType"]) == "ADMINISTRATOR")
                    {
                        ViewBag.AllowView = true;
                        ViewBag.AllowEdit = true;
                    }
                }
                api = "PublicUser/GetAllUsersDetails?flag=UserRegnId&value=" + id;
                var result = service.GetAPI(api);
                PublicUserList = JsonConvert.DeserializeObject<List<PublicUserRegistration>>(result);

                List<CountryViewModel> countryList = new List<CountryViewModel>();
                api = "Admin/GetCountryDetails/0";
                var countrylist = service.GetAPI(api);
                countryList = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist);
                var _countryList = new SelectList(countryList, "CountryId", "Name", PublicUserList.FirstOrDefault().CountryId);
                ViewData["ListCountry"] = _countryList;

                List<StateViewModel> stateList = new List<StateViewModel>();
                api = "Admin/GetStatesByCountryId/" + PublicUserList.FirstOrDefault().CountryId;
                var stateResult = service.GetAPI(api);
                stateList = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult);
                var listState = new SelectList(stateList, "StateId", "Name", PublicUserList.FirstOrDefault().StateId);
                ViewData["ListState"] = listState;

                api = "Admin/GetCityByStateId/" + PublicUserList.FirstOrDefault().StateId;
                List<Cities> cityList = new List<Cities>();
                Cities cityModel = new Cities();
                var resultCity = service.GetAPI(api);
                cityList = JsonConvert.DeserializeObject<List<Cities>>(resultCity);
                var listCity = new SelectList(cityList, "CityId", "CityName", PublicUserList.FirstOrDefault().CityId);
                ViewData["ListCity"] = listCity;

                ViewBag.Country = _countryList;
                ViewBag.State = listState;
                ViewBag.City = listCity;

                userRegistration.FirstName = PublicUserList.FirstOrDefault().FirstName;
                userRegistration.LastName = PublicUserList.FirstOrDefault().LastName;
                userRegistration.EmailAddress = PublicUserList.FirstOrDefault().EmailAddress;
                userRegistration.HouseName = PublicUserList.FirstOrDefault().HouseName;
                userRegistration.PrimaryPhoneNo = PublicUserList.FirstOrDefault().PrimaryPhoneNo;
                userRegistration.SecondaryPhoneNo = PublicUserList.FirstOrDefault().SecondaryPhoneNo;
                userRegistration.ZipCode = PublicUserList.FirstOrDefault().ZipCode;

                userRegistration.Country = PublicUserList.FirstOrDefault().Country;
                userRegistration.State = PublicUserList.FirstOrDefault().State;
                userRegistration.City = PublicUserList.FirstOrDefault().City;

                userRegistration.CountryId = PublicUserList.FirstOrDefault().CountryId;
                userRegistration.StateId = PublicUserList.FirstOrDefault().StateId;
                userRegistration.CityId = PublicUserList.FirstOrDefault().CityId;

                userRegistration.UserRegnId = PublicUserList.FirstOrDefault().UserRegnId;
                userRegistration.ProfilePicPath = PublicUserList.FirstOrDefault().ProfilePicPath;
                userRegistration.Location = PublicUserList.FirstOrDefault().Location;
                userRegistration.HouseName = PublicUserList.FirstOrDefault().HouseName;
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in PublicUser Controller-UserProfile");

            }
            return View(userRegistration);
        }

        public ActionResult CaretakerBooking(int ServiceId, int CaretakerId, string CaretakerName)
        {
            if (Session["UserType"] == null)
            {
                TempData["Failure"] = Constants.NotLoggedIn;
                return RedirectToAction("Login", "Account");
            }

            CountryViewModel countryViewModel = new CountryViewModel();
            string apiCountry = "Admin/GetCountryDetails/0";
            var countrylist = service.GetAPI(apiCountry);
            var newcountryList = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist);
            int defaultCountry = (newcountryList.Where(x => x.Isdefault == true).Count() > 0) ? newcountryList.Where(x => x.Isdefault == true).SingleOrDefault().CountryId : newcountryList.FirstOrDefault().CountryId;
            var _listCountry = new SelectList(newcountryList, "CountryId", "Name", defaultCountry);
            //var _countryList = new SelectList(newcountryList, "CountryId", "Name", 1);
            ViewBag.Country = _listCountry;

            List<StateViewModel> stateList = new List<StateViewModel>();
            var api = "/Admin/GetStatesByCountryId/" + defaultCountry;
            var stateResult = service.GetAPI(api);
            stateList = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult);
            var _stateList = new SelectList(stateList, "StateId", "Name", 0);
            ViewBag.States = _stateList;

            List<Questionare> questions = new List<Questionare>();
            api = "Admin/GetQuestions/0";
            var result = service.GetAPI(api);
            questions = JsonConvert.DeserializeObject<List<Questionare>>(result);
            ViewBag.Questions = questions;

            CaretakerBooking CaretakerBooking = new CaretakerBooking();
            CaretakerBooking.ServiceRequiredId = ServiceId;
            CaretakerBooking.CareTakerId = CaretakerId;
            CaretakerBooking.CareTakerName = CaretakerName;
            CaretakerBooking.CountryId = defaultCountry;
            CaretakerBooking.BookingDate.ToString("MM/dd/yyyy");

            return View(CaretakerBooking);
        }
        public ActionResult PublicUserCaretakerBooking()
        {
            if (Session["UserType"] == null)
            {
                TempData["Failure"] = Constants.NotLoggedIn;
                return RedirectToAction("Login", "Account");
            }

            CountryViewModel countryViewModel = new CountryViewModel();
            string apiCountry = "Admin/GetCountryDetails/0";
            var countrylist = service.GetAPI(apiCountry);
            var newcountryList = JsonConvert.DeserializeObject<List<CountryViewModel>>(countrylist);
            int defaultCountry = (newcountryList.Where(x => x.Isdefault == true).Count() > 0) ? newcountryList.Where(x => x.Isdefault == true).SingleOrDefault().CountryId : newcountryList.FirstOrDefault().CountryId;
            var _listCountry = new SelectList(newcountryList, "CountryId", "Name", defaultCountry);
            //var _countryList = new SelectList(newcountryList, "CountryId", "Name", 1);
            ViewBag.Country = _listCountry;

            List<StateViewModel> stateList = new List<StateViewModel>();
            var api = "/Admin/GetStatesByCountryId/" + defaultCountry;
            var stateResult = service.GetAPI(api);
            stateList = JsonConvert.DeserializeObject<List<StateViewModel>>(stateResult);
            var _stateList = new SelectList(stateList, "StateId", "Name", 0);
            ViewBag.States = _stateList;

            List<Questionare> questions = new List<Questionare>();
            api = "Admin/GetQuestions/0";
            var result = service.GetAPI(api);
            questions = JsonConvert.DeserializeObject<List<Questionare>>(result);
            ViewBag.Questions = questions;

          
            return PartialView("_PublicUserCaretakerBooking");
        }


        /// <summary>
        /// Invoices the payments.
        /// </summary>
        /// <param name="invoice">The invoice.</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult InvoicePayments(string invoice, bool isSearch = false, UserPaymentInvoiceModel invModel = null)
        {
            try
            {
                if (!isSearch)
                {
                    string decInvoice = invoice;
                    UserPaymentInvoiceModel userPaymentInvoice = new UserPaymentInvoiceModel();
                    string api = "Admin/GetUserPaymentInvoiceDetails/" + decInvoice;
                    var result = service.GetAPI(api);
                    userPaymentInvoice = JsonConvert.DeserializeObject<UserPaymentInvoiceModel>(result);
                    ViewData["CurrencySymbol"] = userPaymentInvoice.CurrencySymbol;
                    ViewData["Currency"] = userPaymentInvoice.Currency;
                    return View(userPaymentInvoice);
                }
                else
                {
                    return View(invModel);
                }
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in PublicUser Controller-InvoicePayments");
                return View(invModel);

            }
        }

        [HttpPost]
        public JsonResult SearchInvoice(UserPaymentInvoiceModel invModel, string invNo, string dateFrom, string dateTo, string item, string amtFrom, string amtTo, string desc, string status)
        {
            try
            {
                if (invNo != string.Empty)
                {
                    invModel.AllPayments = invModel.AllPayments.Where(x => x.InvoiceNumber.ToString() == invNo).ToList();
                }

                DateTime fromDate;
                if (DateTime.TryParse(dateFrom, out fromDate))
                {
                    invModel.AllPayments = invModel.AllPayments.Where(x => x.Date >= fromDate).ToList();
                }

                DateTime toDate;
                if (DateTime.TryParse(dateTo, out toDate))
                {
                    invModel.AllPayments = invModel.AllPayments.Where(x => x.Date <= toDate).ToList();
                }

                float fromAmt;
                if (float.TryParse(amtFrom, out fromAmt))
                {
                    invModel.AllPayments = invModel.AllPayments.Where(x => x.Amount >= fromAmt).ToList();
                }

                float toAmt;
                if (float.TryParse(amtTo, out toAmt))
                {
                    invModel.AllPayments = invModel.AllPayments.Where(x => x.Amount <= toAmt).ToList();
                }

                if (desc != string.Empty)
                {
                    invModel.AllPayments = invModel.AllPayments.Where(x => x.Description == desc).ToList();
                }


                int paidStatus;
                if (int.TryParse(status, out paidStatus))
                {
                    invModel.AllPayments = invModel.AllPayments.Where(x => x.PaidStatus == Convert.ToBoolean(paidStatus)).ToList();
                }
                return Json(invModel);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in PublicUser Controller-SearchInvoice");
                return Json(null);
            }
        }

        public ActionResult PaymentWithPaypal(string invNumber, float amount, float tax, float total, string curr)
        {
            //getting the apiContext  

            UserPaymentTransactionModel txnModel = new UserPaymentTransactionModel();
            try
            {
                string api = "Admin/GetPayPalAccount/1";
                var result = service.GetAPI(api);
                PayPalAccount payPal = JsonConvert.DeserializeObject<PayPalAccount>(result);
                PaypalConfiguration config = new PaypalConfiguration();
                APIContext apiContext = config.GetAPIContext(payPal.ClientId, payPal.SecretKey);
                TempData["TxnStaus"] = null;
                TempData["Message"] = null;
                //A resource representing a Payer that funds a payment Payment Method as paypal  
                //Payer Id will be returned when payment proceeds or click to pay  
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {

                    //here we are generating guid for storing the paymentID received in session  
                    //which will be used in the payment execution  
                    var guid = Convert.ToString((new Random()).Next(100000));

                    //this section will be executed first because PayerID doesn't exist  
                    //it is returned by the create function call of the payment class  
                    // Creating a payment  
                    // baseURL is the url on which paypal sendsback the data.  

                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/PublicUser/DoPaypalTransaction?guid=" + guid + "&invNo=" + invNumber;

                    //CreatePayment function gives us the payment approval url  
                    //on which payer is redirected for paypal account payment  

                    var createdPayment = this.CreatePayment(apiContext, baseURI, invNumber, amount, tax, total, curr);
                    //get links returned from paypal in response to Create function call  
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment  
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    // saving the paymentID in the key guid  
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
            }
            catch (IdentityException ex)
            {
                pCMSLogger.Error(ex, "Error occurred in PublicUser Controller-PaymentWithPaypal");
                TempData["TxnStaus"] = "false";
                TempData["Message"] = "Transaction failed - " + ex.Details.error_description;
                return RedirectToAction("InvoicePayments", "PublicUser", new { invoice = invNumber });
            }
            catch (PaymentsException ex)
            {
                pCMSLogger.Error(ex, "Error occurred in PublicUser Controller-PaymentWithPaypal");
                TempData["TxnStaus"] = "false";
                TempData["Message"] = "Transaction failed - " + ex.Details.message;
                return RedirectToAction("InvoicePayments", "PublicUser", new { invoice = invNumber });
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in PublicUser Controller-PaymentWithPaypal");
                TempData["TxnStaus"] = "false";
                TempData["Message"] = "Transaction failed";
                return RedirectToAction("InvoicePayments", "PublicUser", new { invoice = invNumber });
            }
            //on successful payment, show success page to user with excutedPayment details.  
            return RedirectToAction("InvoicePayments", "PublicUser", new { invoice = invNumber });
        }

        public ActionResult DoPaypalTransaction()
        {
            //getting the apiContext  
            string apiPay = "Admin/GetPayPalAccount/1";
            var results = service.GetAPI(apiPay);
            PayPalAccount payPal = JsonConvert.DeserializeObject<PayPalAccount>(results);
            PaypalConfiguration config = new PaypalConfiguration();
            APIContext apiContext = config.GetAPIContext(payPal.ClientId, payPal.SecretKey);
            UserPaymentTransactionModel txnModel = new UserPaymentTransactionModel();
            string invNo = string.Empty;
            try
            {
                invNo = Request.Params["invNo"];
                //A resource representing a Payer that funds a payment Payment Method as paypal  
                //Payer Id will be returned when payment proceeds or click to pay  
                bool isCancelled = Request.Params["Cancel"] != null ? Convert.ToBoolean(Request.Params["Cancel"]) : false;
                if (!isCancelled)
                {

                    string payerId = Request.Params["PayerID"];
                    if (!string.IsNullOrEmpty(payerId))
                    {
                        // This function exectues after receving all parameters for the payment  
                        var guid = Request.Params["guid"];
                        var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);

                        txnModel.Amount = float.Parse(executedPayment.transactions[0].amount.total);
                        txnModel.Currency = executedPayment.transactions[0].amount.currency;
                        txnModel.InvoiceNumber = Convert.ToInt32(executedPayment.transactions[0].invoice_number);
                        txnModel.Description = executedPayment.transactions[0].description;
                        txnModel.TransactionNumber = executedPayment.cart;
                        txnModel.Message = executedPayment.failure_reason;
                        txnModel.Method = executedPayment.payer.payment_method;
                        txnModel.Date = Convert.ToDateTime(executedPayment.create_time);
                        txnModel.Status = executedPayment.state.ToLower() != "approved" ? false : true;
                        txnModel.SiteURL = Request.Url.Scheme + "://" + Request.Url.Authority + "/";
                        txnModel.TransactionDetails = JsonConvert.SerializeObject(executedPayment);
                        string api = "Admin/SaveUserPaymentTransactions";
                        var serviceContent = JsonConvert.SerializeObject(txnModel);
                        HttpStatusCode result = service.PostAPI(serviceContent, api);
                        //If executed payment failed then we will show payment failure message to user  
                        if (executedPayment.state.ToLower() != "approved")
                        {
                            TempData["TxnStaus"] = "true";
                            TempData["Message"] = "Your transaction id is " + executedPayment.cart;
                            return RedirectToAction("InvoicePayments", "PublicUser", new { invoice = invNo });
                        }
                    }
                }
                else
                {
                    TempData["TxnStaus"] = "false";
                    TempData["Message"] = "Transaction is cancelled by the user.";
                    return RedirectToAction("InvoicePayments", "PublicUser", new { invoice = invNo });
                }
            }
            catch (PaymentsException ex)
            {
                pCMSLogger.Error(ex, "Error occurred in PublicUser Controller-DoPaypalTransaction");
                txnModel.Status = false;
                txnModel.Message = ex.Details.message;
                txnModel.Date = DateTime.UtcNow;
                txnModel.InvoiceNumber = Convert.ToInt32(invNo);
                txnModel.TransactionDetails = ex.Response;
                string api = "Admin/SaveUserPaymentTransactions";
                var serviceContent = JsonConvert.SerializeObject(txnModel);
                HttpStatusCode result = service.PostAPI(serviceContent, api);
                TempData["TxnStaus"] = "false";
                TempData["Message"] = "Transaction failed. " + ex.Details.message;
                return RedirectToAction("InvoicePayments", "PublicUser", new { invoice = invNo });
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in PublicUser Controller-DoPaypalTransaction");
                TempData["TxnStaus"] = "false";
                TempData["Message"] = "Transaction failed";
                return RedirectToAction("InvoicePayments", "PublicUser", new { invoice = invNo });
            }
            //on successful payment, show success page to user with excutedPayment details.  
            TempData["TxnStaus"] = "true";
            TempData["Message"] = "Your transaction id is " + txnModel.TransactionNumber;
            return RedirectToAction("InvoicePayments", "PublicUser", new { invoice = invNo });
        }

        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }

        private Payment CreatePayment(APIContext apiContext, string redirectUrl, string invNo, float amt, float tax, float total, string curr)
        {
            //create itemlist and add item objects to it
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            //Adding Item Details like name, currency, price etc  
            itemList.items.Add(new Item()
            {
                name = "Invoice Payment",
                currency = curr,
                price = (amt).ToString("#.##"),
                quantity = "1"
            });
            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details  
            var details = new Details()
            {
                tax = tax.ToString(),
                shipping = "0",
                subtotal = amt.ToString()
            };
            //Final amount with details  
            var amount = new Amount()
            {
                currency = curr,
                total = total.ToString("#.##"), // Total must be equal to sum of tax, shipping and subtotal.  
                details = details
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            transactionList.Add(new Transaction()
            {
                //description = "Transaction description",
                invoice_number = invNo, //Generate an Invoice No  
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "order",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return this.payment.Create(apiContext);
        }

        public ActionResult PublicUserDashboard()
        {
            try
            {
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "PUBLIC USER")
                    {
                        TempData["Error"] = Constants.NoViewPrivilege;
                        return RedirectToAction("Error", "Admin");
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
                pCMSLogger.Error(ex, "Error occurred in PublicUser Controller-PublicUserDashboard");
                return null;
            }
        }


        public ActionResult PublicUserCalendarView()
        {
            try
            {
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "PUBLIC USER")
                    {
                        TempData["Error"] = Constants.NoViewPrivilege;
                        return RedirectToAction("Error", "Admin");
                    }
                }
                else
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
            }

            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in  Public User Calendar View");

            }
            return PartialView("_PublicUserCalendarViewPartial");
        }

        public ActionResult PublicUserNotification()
        {
            List<AdminBookingNotification> notificationDetails = null;
            int publicuserid = 0;
            try
            {
                if (Session["loggedInUserId"] != null)
                {
                    publicuserid = (int)Session["loggedInUserId"];
                }
                string api = "User/GetPublicUserNotification/" + publicuserid;
                var result = service.GetAPI(api);
                notificationDetails = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AdminBookingNotification>>(result);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in  PublicUser Controller-Notification");
            }
            return PartialView("_PublicUserNotification", notificationDetails);
        }

        public ActionResult PublicUserBookingHistory()
        {
            List<UserBooking> userbookingDetails = null;
            int publicuserid = 0;
            try
            {
                if (Session["loggedInUserId"] != null)
                {
                    publicuserid = (int)Session["loggedInUserId"];
                }
                string api = "User/GetPublicUserBookingDetails/" + publicuserid;
                var result = service.GetAPI(api);
                userbookingDetails = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserBooking>>(result);
                userbookingDetails = userbookingDetails.OrderByDescending(x => x.BookingDate).ToList();
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in  PublicUser Controller-UserBooking");

            }

            return PartialView("_PublicUserBookingHistory", userbookingDetails);

        }
        public ActionResult PublicUserBookingHistoryinPublicUserDashboard()
        {
            List<UserBookingInvoiceReport> bookingHistoryList = null;
            int publicuserid = 0;
            try
            {
                if (Session["loggedInUserId"] != null)
                {
                    publicuserid = (int)Session["loggedInUserId"];
                }

                
                string api = "Admin/GetBookingHistoryListById/" + publicuserid;
                var result = service.GetAPI(api);
                bookingHistoryList = JsonConvert.DeserializeObject<List<UserBookingInvoiceReport>>(result);
              
            }

            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in  PublicUser Controller-UserBooking");
            }
            return PartialView("_searchPublicUserBookingHistory", bookingHistoryList);
        }
        public ActionResult PublicUserInvoiceHistoryinPublicUserDashboard()
        {
            List<UserInvoiceParams> InvoiceHistoryList = null;
            try
            {
                int publicuserid = 0;
                if (Session["loggedInUserId"] != null)
                {
                    publicuserid = (int)Session["loggedInUserId"];
                }
               
                string api = "Admin/GetBookingInvoiceListforUserDashBoard/" + publicuserid;
                var result = service.GetAPI(api);
                InvoiceHistoryList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserInvoiceParams>>(result);
                InvoiceHistoryList = InvoiceHistoryList.Where(x => x.InvoiceNumber != 0).ToList();
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in  PublicUser Controller-UserBooking");

            }

            return PartialView("_SearchPublicUserInvoiceHistory", InvoiceHistoryList);



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


        [HttpPost]
        public JsonResult GetBookingForPublicUser(int publicuserid)
        {

            List<UserBooking> userbookingDetails = null;
           
            try
            {
                if (Session["loggedInUserId"] != null)
                {
                    publicuserid = (int)Session["loggedInUserId"];
                }
                string api = "User/GetPublicUserBookingDetails/" + publicuserid;
                var result = service.GetAPI(api);
                userbookingDetails = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserBooking>>(result);
                userbookingDetails.ForEach(x =>
                {
                    x.Start = (DateTime.SpecifyKind(x.Start, DateTimeKind.Utc));
                    x.EndDate = (DateTime.SpecifyKind(x.EndDate, DateTimeKind.Utc));
                    x.BookingDate = (DateTime.SpecifyKind(x.BookingDate, DateTimeKind.Utc));
                });

                return new JsonResult { Data = userbookingDetails, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Publice Controller-GetBookingForPublicUser");
                return null;
            }

        }

        public ActionResult PublicUserPaymentHistory()
        {
            List<PublicUserPaymentHistory> paymentHistory = null;
            int publicuserid = 0;
            try
            {
                if (Session["loggedinuserid"] != null)
                {
                    publicuserid = (int)Session["loggedinuserid"];
                }
                string api = "User/GetPublicUserPaymentHistory/" + publicuserid;
                var result = service.GetAPI(api);
                paymentHistory = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PublicUserPaymentHistory>>(result);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in  PublicUser Controller-PaymentHistory");

            }

            return PartialView("_PublicUserPaymentHistory", paymentHistory);
        }

        public ActionResult ViewNotificationDetails(int bookingId)
        {
            PublicUserNotificationDetails viewNotificationDetails = null;
            try
            {
                string api = "User/GetUserNotificationDetailsById/" + bookingId;
                var result = service.GetAPI(api);
                viewNotificationDetails = Newtonsoft.Json.JsonConvert.DeserializeObject<PublicUserNotificationDetails>(result);
                string statusDescription = EnumHelper.GetDescription(viewNotificationDetails.Status);
                viewNotificationDetails.StatusDescription = statusDescription;
                List<Questionare> questions = new List<Questionare>();
                api = "Admin/GetQuestions/0";
                result = service.GetAPI(api);
                questions = JsonConvert.DeserializeObject<List<Questionare>>(result);
                ViewBag.Questions = questions;
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in  CareTaker Controller-UserBooking");

            }
            return PartialView("_UserNotificationDetailsById", viewNotificationDetails);
        }
        public class EnumHelper
        {
            public static string GetDescription(Enum @enum)
            {
                if (@enum == null)
                    return null;

                string description = @enum.ToString();

                try
                {
                    FieldInfo fi = @enum.GetType().GetField(@enum.ToString());

                    DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                    if (attributes.Length > 0)
                        description = attributes[0].Description;
                }
                catch
                {
                }

                return description;
            }
        }
        public ActionResult PaymentReport()
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
                    api = "/Admin/GetStatesByCountryId/1";
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
            api = "Admin/RetrieveServiceDetails/0";
            var serviveResult = service.GetAPI(api);
            List<ServicesViewModel> listService = new List<ServicesViewModel>();
            listService = JsonConvert.DeserializeObject<List<ServicesViewModel>>(serviveResult);
            ViewBag.Service = new SelectList(listService, "ServiceId", "Name", 0);

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

            var transactionStatus = new SelectList(new[]
          {
                new { ID = "2", Name = "--Select Status--" },
                new { ID = "0", Name = "Failed" },
                new { ID = "1", Name = "Success" },
          },
          "ID", "Name", 2);

            ViewData["TransactionStatus"] = transactionStatus;
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

            YearList = new SelectList(years, "Id", "Year", DateTime.Now.Year);
            return YearList;
        }

        public ActionResult UpdateBooking(int ServiceId, int caretakerId, string fromDate, string toDate, string toTime, string fromTime,int bookingId)
        {
            DateTime reportStart = DateTime.Now;
            DateTime reportend = DateTime.Now;
            CaretakerBooking caretakerBooking = new CaretakerBooking();
            caretakerBooking.ServiceRequiredId = ServiceId;
            caretakerBooking.CareTakerId = caretakerId;
            reportStart = caretakerBooking.BookingStartTime;
            reportend = caretakerBooking.BookingEndTime;
            List<BookigDate> listBookingDates = new List<BookigDate>();
            if (reportend.Date > reportStart.Date)
            {
                int reportDateDiff = reportend.Date.Subtract(reportStart.Date).Days;
                BookigDate test = new BookigDate();
                List<DateTime> scheduledDate = Enumerable.Range(0, 1 + reportend.Date.Subtract(reportStart.Date).Days).Select(offset => reportStart.Date.AddDays(offset)).ToList();
                foreach (DateTime item in scheduledDate)
                {
                    BookigDate objschedulingDate = new BookigDate();
                    objschedulingDate.Date = item;

                    var hours = reportend.Subtract(reportStart);
                    var startDayhours = TimeSpan.FromHours(24) - reportStart.TimeOfDay;
                    var endDayhours = hours - startDayhours;

                    if (item.Date == reportStart.Date)
                    {
                        objschedulingDate.Hours = startDayhours.TotalHours;
                    }
                    else if (item.Date == reportend.Date)
                    {
                        objschedulingDate.Hours = endDayhours.TotalHours;
                    }
                    else
                    {
                        objschedulingDate.Hours = 24;
                    }

                    listBookingDates.Add(objschedulingDate);
                }
            }
            else
            {
                int reportDateDiff = reportend.Date.Subtract(reportStart.Date).Days;
                SchedulingDate test = new SchedulingDate();
                List<DateTime> scheduledDate = Enumerable.Range(0, 1 + reportend.Date.Subtract(reportStart.Date).Days).Select(offset => reportStart.Date.AddDays(offset)).ToList();
                foreach (DateTime item in scheduledDate)
                {
                    BookigDate objschedulingDate = new BookigDate();
                    objschedulingDate.Date = item;
                    var hours = reportend.Subtract(reportStart);
                    objschedulingDate.Hours = hours.TotalHours;
                    listBookingDates.Add(objschedulingDate);
                }
            }

            caretakerBooking.PublicUserBookigDates = listBookingDates;
            caretakerBooking.BookingId = bookingId;

            string api = "PublicUser/SaveCaretakerBooking";
            var serviceContent = JsonConvert.SerializeObject(caretakerBooking);
            HttpStatusCode result = service.PostAPI(serviceContent, api);
            return View();
        }
        public JsonResult VerifyEmail(int userId)
        {
            Service service = new Service();
            string api = "Admin/VerifyEmail/" + userId;
            HttpStatusCode result = service.PostAPI(null, api);
            if (result == HttpStatusCode.OK)
            {
                TempData["Success"] = "Public User Email Verified Successfully";
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
        [HttpGet]
        public ActionResult PublicUserInvoiceSettings()
        {
            UsersDetails userModel = new UsersDetails();
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
                        getRolePrivilege.ModuleID = 25;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = Newtonsoft.Json.JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
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
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }

                //api = "Client/GetAllClientDetails";
                //var result = service.GetAPI(api);
                //clientModel = JsonConvert.DeserializeObject<List<ClientModel>>(result);
                //ViewData["CurrencySymbol"] = clientModel.FirstOrDefault().CurrencySymbol;
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-ClientSearch");
            }
            return View(userModel);
        }


        public ActionResult PublicInvoiceSettingsList()
        {
            List<UsersDetails> clientModel = new List<UsersDetails>();
            try
            {
                string api = string.Empty;
                if (Session["UserType"] != null)
                {
                    if (Convert.ToString(Session["UserType"]) != "OFFICE STAFF" && Convert.ToString(Session["UserType"]) != "ADMINISTRATOR")
                    {
                        ViewBag.Error = Constants.NoViewPrivilege;
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
                            ViewBag.Error = Constants.NoViewPrivilege;
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
                api = "User/GetPublicInvoiceDetails";
                var result = service.GetAPI(api);
                clientModel = JsonConvert.DeserializeObject<List<UsersDetails>>(result);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Admin Controller-CityList");
            }
            return PartialView("_PublicUserInvoiceSettingsPartial", clientModel);
        }

        public ActionResult LoadUser()
        {
            try
            {
                string api = "PublicUser/GetAllUsersDetails?flag=UserType&value=2";
                var Clients = service.GetAPI(api);
                List<UsersDetails> userDetailsList = new List<UsersDetails>();
                userDetailsList = JsonConvert.DeserializeObject<List<UsersDetails>>(Clients);
               
                var userDetails = JsonConvert.SerializeObject(userDetailsList);
                return Json(userDetails, JsonRequestBehavior.AllowGet);



               
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-LoadClient");
                return null;
            }
        }
        public ActionResult SaveBookingDetails(PublicUserCaretakerBooking data)
        {
            try
            {
                if (Session["UserType"] == null)
                {
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }
                string startTime;
                string endTime;
                DateTime actualStart = DateTime.Now;
                DateTime actualEnd = DateTime.Now;
                DateTime reportStart = DateTime.Now;
                DateTime reportend = DateTime.Now;
                string[] workTimeValues = null;
               
                if (data.WorkTimeDetails != null)//work time values from drop down. in wrok time dropdown 
                {
                    workTimeValues = data.WorkTimeDetails.Split('|');
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
                List<BookingDate> listschedulingDate = new List<BookingDate>();
                if (reportend.Date > reportStart.Date)
                {
                    int reportDateDiff = reportend.Date.Subtract(reportStart.Date).Days;
                    PublicUserCaretakerBooking test = new PublicUserCaretakerBooking();

                    List<DateTime> scheduledDate = Enumerable.Range(0, 1 + reportend.Date.Subtract(reportStart.Date).Days).Select(offset => reportStart.Date.AddDays(offset)).ToList();
                    foreach (DateTime item in scheduledDate)
                    {
                        BookingDate objschedulingDate = new BookingDate();
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
                    PublicUserCaretakerBooking test = new PublicUserCaretakerBooking();
                    List<DateTime> scheduledDate = Enumerable.Range(0, 1 + reportend.Date.Subtract(reportStart.Date).Days).Select(offset => reportStart.Date.AddDays(offset)).ToList();
                    foreach (DateTime item in scheduledDate)
                    {
                        BookingDate objschedulingDate = new BookingDate();
                        objschedulingDate.Date = item;

                        var hours = reportend.Subtract(reportStart);
                        objschedulingDate.Hours = hours.TotalHours;
                        objschedulingDate.Date = objschedulingDate.Date + reportStart.TimeOfDay;
                        listschedulingDate.Add(objschedulingDate);
                    }
                }


                PublicUserCaretakerBooking saveData = new PublicUserCaretakerBooking();
               
                saveData.Id = data.Id;
                saveData.BookingId = data.BookingId;
               
                saveData.CareTaker = data.CareTaker;
                saveData.ClientName = data.ClientName;
                saveData.CareTakerName = data.CareTakerName;
                saveData.WorkModeName = data.WorkModeName;
                saveData.ServiceTypeName = data.ServiceTypeName;
                saveData.BookingDateTime = DateTime.UtcNow;
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

                
                saveData.WorkMode = data.WorkMode;
                saveData.CareTakerType = data.CareTakerType;
                saveData.PublicUserId = data.PublicUserId;
                saveData.Start = actualStart;
                saveData.End = actualEnd;
                saveData.Description = data.Description;
                saveData.ContactPerson = data.ContactPerson;
                saveData.PublicUserSchedulingDate = listschedulingDate;
                saveData.UserId = (int)Session["loggedInUserId"];
                saveData.SiteUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/";


                Service service = new Service();
                string api = "User/SaveBookingDetails";
               
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
        public JsonResult GetEventsByPublicUserId(int bookingId, int month, int year)
        {

            List<PublicUserCaretakerBooking> scheduleDetailsList = new List<PublicUserCaretakerBooking>();
            try
            {
                CalenderEventInput calenderEventInput = new CalenderEventInput
                {
                    BookingId = bookingId
                };
                if (year != 0 && month != 0)
                {
                    calenderEventInput.StartDate = new DateTime(year, month, 1);
                    calenderEventInput.EndDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                    calenderEventInput.EndDate = calenderEventInput.StartDate.AddMonths(1).AddSeconds(-1);
                }
                string api = "/User/GetAllBookingdetails";
                var data = JsonConvert.SerializeObject(calenderEventInput);
                var result = service.PostAPIWithData(data, api);
                scheduleDetailsList = JsonConvert.DeserializeObject<List<PublicUserCaretakerBooking>>(result.Result);
                if (scheduleDetailsList!=null)
                {
                    scheduleDetailsList.ForEach(x =>
                    {
                        x.Start = (DateTime.SpecifyKind(x.Start, DateTimeKind.Utc));
                    });
                }
                
               
                    return new JsonResult { Data = scheduleDetailsList, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
          

            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-GetEventsByClientId");
                return null;
            }

        }
        public ActionResult SavePublicUserInvoiceReport()
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
        public ActionResult LoadPublicUser()
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
                var clientSearchInputsContent = JsonConvert.SerializeObject(inputs);
                string api = "Admin/GetAllUserDetailsByLocation/";
                var Clients = service.PostAPIWithData(clientSearchInputsContent, api);
                List<UsersDetails> scheduleDetailsList = new List<UsersDetails>();
                scheduleDetailsList = JsonConvert.DeserializeObject<List<UsersDetails>>(Clients.Result);
                var events = scheduleDetailsList.Where(a => a.UserTypeId == 2 && a.UserVerified==true).ToList();
                var json = JsonConvert.SerializeObject(events);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-LoadClient");
                return null;
            }
        }

        public ActionResult LoadPublicUserByLocation(LocationSearchInputs inputs)
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
                string api = "Admin/GetAllUserDetailsByLocation/";
                var Clients = service.PostAPIWithData(clientSearchInputsContent, api);
                List<UsersDetails> scheduleDetailsList = new List<UsersDetails>();
                scheduleDetailsList = JsonConvert.DeserializeObject<List<UsersDetails>>(Clients.Result);
                var events = scheduleDetailsList.Where(a => a.UserTypeId == 2 && a.UserVerified == true).ToList();
                var json = JsonConvert.SerializeObject(events);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-LoadClient");
                return null;
            }
        }

        public ActionResult LoadCaregiverServices()
        {
            try
            {
                string api = "Admin/GetCaregiverServices";
                var Category = service.GetAPI(api);
                return Json(Category, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in Client Controller-LoadCategory");
                return null;
            }
        }
        public ActionResult RegenerateUserInvoiceDetails(int InvoiceSearchInputId)
        {
            List<UserInvoiceParams> bookingDetailsList = new List<UserInvoiceParams>();
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
                        getRolePrivilege.ModuleID = 7;
                        var getPrivilege = JsonConvert.SerializeObject(getRolePrivilege);
                        var roleModulePrivileges = service.PostAPIWithData(getPrivilege, api);
                        RoleModulePrivileges apiResults = Newtonsoft.Json.JsonConvert.DeserializeObject<RoleModulePrivileges>(roleModulePrivileges.Result);
                        if (!apiResults.AllowView)
                        {
                            ViewBag.Error = Constants.NoViewPrivilege;
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
                    TempData["Failure"] = Constants.NotLoggedIn;
                    return RedirectToAction("Login", "Account");
                }

                List<ClientModel> scheduleDetailsList = new List<ClientModel>();
                List<ClientModel> activeClient = new List<ClientModel>();
                List<InvoiceSearchInpts> apiResult = new List<InvoiceSearchInpts>();
                InvoiceHistory searchInputs = new InvoiceHistory();
                List<UsersDetails> users = new List<UsersDetails>();

                api = "Admin/GetBookingInvoiceList";
                searchInputs.InvoiceSearchInputId = InvoiceSearchInputId;
                var advancedSearchInputModel = JsonConvert.SerializeObject(searchInputs);
                var result = service.PostAPIWithData(advancedSearchInputModel, api);
                bookingDetailsList = JsonConvert.DeserializeObject<List<UserInvoiceParams>>(result.Result);


                api = "PublicUser/GetAllUsersDetails?flag=UserType&value=2";
                var result1 = service.GetAPI(api);
                users = JsonConvert.DeserializeObject<List<UsersDetails>>(result1);


              

                
                ViewData["User"] = new SelectList(users, "UserRegnId", "FullName", bookingDetailsList.FirstOrDefault().UserId);

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
                pCMSLogger.Error(ex, "Error occurred in PublicUser Controller-RegenerateUserInvoiceDetails");
                System.IO.File.WriteAllText(Server.MapPath("~/PDFError.txt"), ex.Message);
            }
            return View(bookingDetailsList.FirstOrDefault());
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
    }
}