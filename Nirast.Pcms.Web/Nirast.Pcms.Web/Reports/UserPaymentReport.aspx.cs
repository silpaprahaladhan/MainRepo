using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using Nirast.Pcms.Web.Helpers;
using Nirast.Pcms.Web.Logger;
using Nirast.Pcms.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Nirast.Pcms.Web.Reports
{
    public partial class UserPaymentReports : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PCMSLogger Logger = new PCMSLogger();

            if (!IsPostBack)
            {
                try
                {
                    ReportViewer1.Visible = false;
                    ReportViewer1.ShowRefreshButton = false;
                    ReportViewer1.ProcessingMode = ProcessingMode.Local;
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/UserPaymentReport.rdlc");
                    PaymentReport searchInputs = new PaymentReport();
                    List<PaymentReportDetails> scheduleDetailsList = new List<PaymentReportDetails>();
                    DateTime fromdate = DateTime.MinValue, todate = DateTime.MinValue;
                    int searchDateType = 0, year = 0, month = 0, category = 0, status = 0, serviceid = 0;
                    if (Request.QueryString["searchRange"] != "null" && Request.QueryString["searchRange"] != "")
                    {
                        searchDateType = Convert.ToInt32(Request.QueryString["searchRange"]);
                    }
                    if (Request.QueryString["service"] != "null" && Request.QueryString["service"] != "")
                    {
                        serviceid = Convert.ToInt32(Request.QueryString["service"]);
                    }

                    //if (Request.QueryString["searchDateType"] != "null" && Request.QueryString["searchDateType"] != "")
                    //{
                    //    searchDateType = Convert.ToInt32(Request.QueryString["searchDateType"]);
                    //}
                    if (Request.QueryString["year"] != "null" && Request.QueryString["year"] != "")
                    {
                        year = Convert.ToInt32(Request.QueryString["year"]);
                    }
                    if (Request.QueryString["month"] != "null" && Request.QueryString["month"] != "")
                    {
                        month = Convert.ToInt32(Request.QueryString["month"]);
                    }

                    //if (Request.QueryString["category"] != "null" && Request.QueryString["category"] != "")
                    //{
                    //    category = Convert.ToInt32(Request.QueryString["category"]);
                    //}
                    //if (Request.QueryString["status"] != "null" && Request.QueryString["status"] != "")
                    //{
                    //    status = Convert.ToInt32(Request.QueryString["status"]);
                    //}
                    if (Request.QueryString["fromdate"] != "")
                    {
                        fromdate = Convert.ToDateTime(Request.QueryString["fromdate"]);
                    }
                    if (Request.QueryString["todate"] != "")
                    {
                        todate = Convert.ToDateTime(Request.QueryString["todate"]);
                    }




                    searchInputs.TransactionStatus = status;
                    searchInputs.CaretakerType = category;
                    //searchInputs.CareTaker = caretaker;
                    searchInputs.SearchType = searchDateType;
                    searchInputs.FromDate = fromdate;
                    searchInputs.ToDate = todate;
                    searchInputs.Year = (searchDateType != 3) ? year : 0;
                    searchInputs.Month = month;
                    searchInputs.ServiceType = serviceid;

                    if (year != 0 && month != 0)
                    {
                        searchInputs.FromDate = new DateTime(year, month, 1);
                        searchInputs.ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                    }
                    if (year != 0 && month == 0 && fromdate == DateTime.MinValue)
                    {
                        searchInputs.FromDate = new DateTime(year, 1, 1);
                        searchInputs.ToDate = new DateTime(year, 12, 31);
                    }

                    Service service = new Service();
                    string api = "Admin/SearchUserPaymentReport";
                    var advancedSearchInputModel = JsonConvert.SerializeObject(searchInputs);
                    var result = service.PostAPIWithData(advancedSearchInputModel, api);
                    scheduleDetailsList = JsonConvert.DeserializeObject<List<PaymentReportDetails>>(result.Result);
                    List<PaymentReportDetails> scheduleDetailsListFilterd = new List<PaymentReportDetails>();
                    scheduleDetailsListFilterd = scheduleDetailsList.ToList();

                    if (scheduleDetailsListFilterd.Count != 0)
                    {
                        ReportViewer1.Visible = true;
                    }
                    else
                    {
                        lblmessage.Visible = true;

                    }
                    ReportDataSource datasource = new ReportDataSource("UserPayment", scheduleDetailsListFilterd);
                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportViewer1.LocalReport.DataSources.Add(datasource);

                    string ServiceName = "";
                    ServiceName = (Request.QueryString["ServiceName"]).ToString();

                    string monthText = "--Select Month--";
                    ReportParameterCollection reportParameters = new ReportParameterCollection();
                    reportParameters.Add(new ReportParameter("Year", year.ToString()));
                    if (monthText == "--Select Month--" || monthText == null)
                    {
                        reportParameters.Add(new ReportParameter("FromDate", Convert.ToDateTime(searchInputs.FromDate).ToString("dd MMM yyyy")));
                        reportParameters.Add(new ReportParameter("Todate", Convert.ToDateTime(searchInputs.ToDate).ToString("dd MMM yyyy")));
                    }
                    else
                    {
                        reportParameters.Add(new ReportParameter("monthText", monthText.ToString()));
                    }
                    reportParameters.Add(new ReportParameter("Service", ServiceName));
                    this.ReportViewer1.LocalReport.SetParameters(reportParameters);
                    this.ReportViewer1.LocalReport.Refresh();

                    List<CompanyProfile> companyProfile = new List<CompanyProfile>();
                    string apia = "Admin/GetCompanyProfiles/0";
                    var results = service.GetAPI(apia);
                    CompanyProfile listCompanyProfile = JsonConvert.DeserializeObject<CompanyProfile>(results);
                    companyProfile.Add(listCompanyProfile);
                    ReportDataSource datasourceCompanyProfile = new ReportDataSource("CompanyProfile", companyProfile);
                    ReportViewer1.LocalReport.DataSources.Add(datasourceCompanyProfile);

                    //ReportViewer1.SizeToReportContent = true;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error occurred in Admin Controller-Interview");
                }
            }
        }
    }
}