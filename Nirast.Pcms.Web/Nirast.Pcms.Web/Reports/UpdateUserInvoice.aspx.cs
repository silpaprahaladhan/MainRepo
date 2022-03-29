using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using Nirast.Pcms.Web.Helpers;
using Nirast.Pcms.Web.Logger;
using Nirast.Pcms.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Nirast.Pcms.Web.Reports
{
    public partial class UpdateUserInvoice : System.Web.UI.Page
    {
        PCMSLogger Logger = new PCMSLogger();
        List<UserBookingInvoiceReport> bookingDetailsList = new List<UserBookingInvoiceReport>();
        CaretakerWiseSearchReport searchInputs = new CaretakerWiseSearchReport();
        string InvoiceNumber = "";
        protected void Page_Load(object sender, EventArgs e)
        {

            ReportViewer1.Visible = false;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.ShowRefreshButton = false;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/PublicUserInvoice.rdlc");
           
            List<UserBookingInvoiceReport> bookingDetailsfilteredList = new List<UserBookingInvoiceReport>();
            List<UserBookingInvoiceReport> invoiceDetailsList = new List<UserBookingInvoiceReport>();

            DateTime fromdate = DateTime.MinValue, todate = DateTime.MinValue;
            int searchDateType = 0, year = 0, month = 0, status = 0, serviceid = 0;
            string monthText = "";
            string InvoicePrefix = "";
            int publicuserId = 0;
            

            try
            {
                DateTime invoiceDate = Convert.ToDateTime(Request.QueryString["invoiceDate"]);
                publicuserId = Convert.ToInt32(Request.QueryString["userId"]);
                year = Convert.ToInt32(Request.QueryString["year"]);
                month = Convert.ToInt32(Request.QueryString["month"]);
                int mode = Convert.ToInt32(Request.QueryString["Mode"]);
                monthText = (Request.QueryString["monthText"]);
                serviceid = Convert.ToInt32(Request.QueryString["service"]);
                int InvoiceSearchInputId = Convert.ToInt32(Request.QueryString["InvoiceSearchInputId"]);

                InvoiceNumber = (Request.QueryString["invoiceNumber"]);

                InvoicePrefix = (Request.QueryString["invoicePrefix"]);

                if (Request.QueryString["fromdate"] != "")
                {
                    fromdate = Convert.ToDateTime(Request.QueryString["fromdate"]);
                }
                if (Request.QueryString["todate"] != "")
                {
                    todate = Convert.ToDateTime(Request.QueryString["todate"]);
                }
                searchInputs.PublicUserId = publicuserId;
                searchInputs.FromDate = fromdate;
                searchInputs.ToDate = todate;
                searchInputs.InvoiceDate = invoiceDate;

                if (month == 0)
                {
                    searchInputs.Year = 0;
                }
                else
                {
                    searchInputs.Year = year;
                }
                searchInputs.Month = 0;
                if (year != 0 && month != 0)
                {
                    searchInputs.FromDate = new DateTime(year, month, 1);   //new DateTime(year, month, 1);
                    searchInputs.ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

                }
                if (year != 0 && month == 0 && fromdate == DateTime.MinValue)
                {
                    searchInputs.FromDate = new DateTime(year, 1, 1);
                    searchInputs.ToDate = new DateTime(year, 12, 31);
                }


                if (!IsPostBack)
                {

                    Service service = new Service();
                    string api = "Admin/GetBookingHistoryListForInvoiceGeneration";
                    var advancedSearchInputModel = JsonConvert.SerializeObject(searchInputs);
                    var result = service.PostAPIWithData(advancedSearchInputModel, api);
                    bookingDetailsList = JsonConvert.DeserializeObject<List<UserBookingInvoiceReport>>(result.Result);

                    bookingDetailsList.ForEach(x =>
                    {
                        x.InvoiceNumber = Convert.ToInt32(InvoiceNumber);
                        x.InvoicePrefix = InvoicePrefix;
                    });

                    api = "Admin/GetCaregiverServices";
                    List<CareTakerServices> servicesList = new List<CareTakerServices>();
                    var resultList = service.GetAPI(api);
                    servicesList = JsonConvert.DeserializeObject<List<CareTakerServices>>(resultList);



                    bookingDetailsfilteredList = bookingDetailsList.Where(x => x.BookingId != 0).GroupBy(l => l.BookingId)
                                                                .Select(cl => new UserBookingInvoiceReport
                                                                {
                                                                    BookingId = cl.First().BookingId,

                                                                    InvoiceNumber = cl.First().InvoiceNumber,
                                                                    InvoicePrefix = cl.First().InvoicePrefix,
                                                                    EffAmount = cl.Sum(k => k.TotalPayingAmount),
                                                                    EffTaxAmount = cl.Sum(k => k.TaxAmount),

                                                                }).ToList();


                    if (bookingDetailsList.Count != 0)
                    {
                        emailTxt.Text = bookingDetailsList[0].UserEmail;
                        ReportViewer1.Visible = true;

                    }
                    else
                    {
                        lblmessage.Visible = true;
                        divShowpdf.Visible = false;
                        div1.Visible = false;

                    }

                    if (year != 0 && month != 0)
                    {
                        searchInputs.FromDate = new DateTime(year, month, 1);   //new DateTime(year, month, 1);
                        searchInputs.ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

                    }
                    if (year != 0 && month == 0 && fromdate == DateTime.MinValue)
                    {
                        searchInputs.FromDate = new DateTime(year, 1, 1);
                        searchInputs.ToDate = new DateTime(year, 12, 31);
                    }

                    ReportDataSource datasource = new ReportDataSource("PublicUserInvoice", bookingDetailsList);
                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportViewer1.LocalReport.DataSources.Add(datasource);

                    ReportParameterCollection reportParameters = new ReportParameterCollection();
                    reportParameters.Add(new ReportParameter("Year", year.ToString()));
                    if (monthText == "--Select Month--")
                    {
                        reportParameters.Add(new ReportParameter("FromDate", Convert.ToDateTime(searchInputs.FromDate).ToString("dd MMM yyyy")));
                        reportParameters.Add(new ReportParameter("ToDate", Convert.ToDateTime(searchInputs.ToDate).ToString("dd MMM yyyy")));
                        reportParameters.Add(new ReportParameter("InvoiceDate", Convert.ToDateTime(searchInputs.InvoiceDate).ToString("dd MMM yyyy")));
                        ReportViewer1.LocalReport.DisplayName = "PublicUserInvoice" + "  " + fromdate.ToString("dd MMM yyyy") + " to " + todate.ToString("dd MMM yyyy");
                    }
                    else
                    {
                        reportParameters.Add(new ReportParameter("monthText", monthText.ToString()));
                        reportParameters.Add(new ReportParameter("InvoiceDate", Convert.ToDateTime(searchInputs.InvoiceDate).ToString("dd MMM yyyy")));
                        ReportViewer1.LocalReport.DisplayName = "PublicUserInvoice" + " " + monthText.ToString() + " " + year.ToString();

                    }

                    this.ReportViewer1.LocalReport.SetParameters(reportParameters);
                    this.ReportViewer1.LocalReport.Refresh();


                    List<CompanyProfile> companyProfile = new List<CompanyProfile>();
                    string apia = "Admin/GetCompanyProfiles/0";
                    var results = service.GetAPI(apia);
                    CompanyProfile listCompanyProfile = JsonConvert.DeserializeObject<CompanyProfile>(results);
                    companyProfile.Add(listCompanyProfile);
                    ReportDataSource datasourceCompanyProfile = new ReportDataSource("CompanyProfile", companyProfile);
                    ReportViewer1.LocalReport.DataSources.Add(datasourceCompanyProfile);
                    Warning[] warnings;
                    string[] streamIds;
                    string mimeType = string.Empty;
                    string encoding = string.Empty;
                    string extension = string.Empty;

                    if (bookingDetailsList != null && bookingDetailsList.Count > 0)
                    {

                        byte[] bytes = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                        string siteUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/";
                        string filePath = Server.MapPath("~/PCMS/Invoice/PublicUser/") + "Invoice_" + bookingDetailsList[0].UserName + "_" + bookingDetailsList[0].InvoiceNumber + ".pdf";
                        if (!Directory.Exists(Server.MapPath("~/PCMS/Invoice/PublicUser/")))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/PCMS/Invoice/PublicUser/"));
                        }
                        Logger.Info("filePath: " + filePath.ToString());
                        System.IO.File.WriteAllBytes(filePath, bytes);
                        filePath = siteUrl + ("PCMS/Invoice/PublicUser/") + "Invoice_" + bookingDetailsList[0].UserName + "_" + bookingDetailsList[0].InvoiceNumber + ".pdf";

                        ifrmpdfshow.Src = "~/PCMS/Invoice/PublicUser/" + "Invoice_" + bookingDetailsList[0].UserName + "_" + bookingDetailsList[0].InvoiceNumber + ".pdf";
                        divNotSeperate.Visible = false;
                        divShowpdf.Visible = true;
                        float amountPayTotal = 0;
                        foreach (var item in bookingDetailsfilteredList)
                        {
                            item.TotalPayingAmount = item.EffAmount + item.EffTaxAmount;
                            amountPayTotal = amountPayTotal + item.TotalPayingAmount;
                        }
                        InvoiceSearchInpts invoicesearchinputs = new InvoiceSearchInpts()
                        {
                            InvoiceSearchInputId = InvoiceSearchInputId,
                            InvoiceNumber = bookingDetailsList[0].InvoiceNumber,
                            InvoicePrefix = bookingDetailsList[0].InvoicePrefix,
                            PublicUserId = publicuserId,
                            StartDate = searchInputs.FromDate,
                            EndDate = searchInputs.ToDate,
                            Mode = mode,
                            Year = year,
                            Month = month,
                            PdfFilePath = filePath,
                            InvoiceDate = invoiceDate,
                            BookingId = bookingDetailsfilteredList[0].BookingId,
                            TotalPayingAmount = amountPayTotal
                        };

                        string Invoiceapi = "Admin/AddPaymentInvoiceDetails";
                        var serviceContent1 = JsonConvert.SerializeObject(invoicesearchinputs);
                        var invoiceresult = service.PostAPIWithData(serviceContent1, Invoiceapi);
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred in ShowPublicUserInvoiceReport-Page_Load");
            }
        }


        protected void Btnsend_Click(object sender, EventArgs e)
        {
            Service service = new Service();
            string api = "Admin/GetBookingHistoryListForInvoiceGeneration";
            var advancedSearchInputModel = JsonConvert.SerializeObject(searchInputs);
            var result = service.PostAPIWithData(advancedSearchInputModel, api);
            bookingDetailsList = JsonConvert.DeserializeObject<List<UserBookingInvoiceReport>>(result.Result);
            EmailInput emailinputs = new EmailInput
            {
                EmailType = Enums.EmailType.Invoice,
                Body = GetEmailBody(bookingDetailsList[0].UserName),
                Subject = "Invoice ",
                EmailId = emailTxt.Text,
              
                Attachments = Server.MapPath("~/PCMS/Invoice/PublicUser/") + "Invoice_" + bookingDetailsList[0].UserName + "_" + InvoiceNumber + ".pdf",
                UserId = bookingDetailsList[0].UserId
            };
            api = "Admin/SendEmailNotification";
            var serviceContent = JsonConvert.SerializeObject(emailinputs);
            var results = service.PostAPIWithData(serviceContent, api);
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "script", "<script type='text/javascript'>hideProgress();</script>", false);
            ScriptManager.RegisterClientScriptBlock(this, GetType(), "none", "<script>hideProgress();</script>", false);
            if (results.Result != "0" && results.Result != "")
            {
                MsgBox("Invoice has been sent to: " + emailinputs.EmailId, this.Page, this);
            }
            else
            {
                MsgBox("Failed send invoice to: " + emailinputs.EmailId, this.Page, this);
            }
        }
        public void MsgBox(String ex, Page pg, Object obj)
        {
            string s = "<SCRIPT language='javascript'>alert('" + ex.Replace("\r\n", "\\n").Replace("'", "") + "'); </SCRIPT>";
            Type cstype = obj.GetType();
            ClientScriptManager cs = pg.ClientScript;
            cs.RegisterClientScriptBlock(cstype, s, s.ToString());
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
                int i = 0;
                return string.Empty;
            }
        }
    }
}