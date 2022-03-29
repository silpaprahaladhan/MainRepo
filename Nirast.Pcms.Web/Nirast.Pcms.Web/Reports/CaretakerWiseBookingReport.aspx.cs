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
    public partial class CaretakerWiseBookingReport : System.Web.UI.Page
    {
        PCMSLogger Logger = new PCMSLogger();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                try
                {

                    CaretakerBookingsReport.Visible = false;
                    CaretakerBookingsReport.ProcessingMode = ProcessingMode.Local;
                    CaretakerBookingsReport.ShowRefreshButton = false;
                    CaretakerBookingsReport.LocalReport.ReportPath = Server.MapPath("~/Reports/CaretakerBookings.rdlc");
                    CaretakerWiseSearchReport searchInputs = new CaretakerWiseSearchReport();
                    List<PaymentReportDetails> scheduleDetailsList = new List<PaymentReportDetails>();
                    DateTime fromdate = DateTime.MinValue, todate = DateTime.MinValue;
                    int searchDateType = 0, year = 0, month = 0, category = 0, status = 0, serviceid = 0;
                    string caretaker = string.Empty;

                    if (Request.QueryString["searchRange"] != "null" && Request.QueryString["searchRange"] != "")
                    {
                        searchDateType = Convert.ToInt32(Request.QueryString["searchRange"]);
                    }

                    if (Request.QueryString["service"] != "null" && Request.QueryString["service"] != "")
                    {
                        serviceid = Convert.ToInt32(Request.QueryString["service"]);
                    }

                    if (Request.QueryString["caretaker"] != "null" && Request.QueryString["caretaker"] != "")
                    {
                        caretaker = Request.QueryString["caretaker"];
                    }

                    if (Request.QueryString["year"] != "null" && Request.QueryString["year"] != "")
                    {
                        year = Convert.ToInt32(Request.QueryString["year"]);
                    }

                    if (Request.QueryString["month"] != "null" && Request.QueryString["month"] != "")
                    {
                        month = Convert.ToInt32(Request.QueryString["month"]);
                    }

                    if (Request.QueryString["fromdate"] != "")
                    {
                        fromdate = Convert.ToDateTime(Request.QueryString["fromdate"]);
                    }

                    if (Request.QueryString["todate"] != "")
                    {
                        todate = Convert.ToDateTime(Request.QueryString["todate"]);
                    }




                    searchInputs.CareTaker = caretaker;
                    searchInputs.SearchType = searchDateType;
                    searchInputs.FromDate = (fromdate == DateTime.MinValue) ? (DateTime?)null : fromdate;
                    searchInputs.ToDate = (todate == DateTime.MinValue) ? (DateTime?)null : todate;
                    searchInputs.Year = (searchDateType != 3) ? year : 0;
                    searchInputs.Month = month;
                    searchInputs.ServiceId = serviceid;

                    Service service = new Service();
                    string api = "Admin/GetCaretakerBookings";
                    var advancedSearchInputModel = JsonConvert.SerializeObject(searchInputs);
                    var result = service.PostAPIWithData(advancedSearchInputModel, api);
                    scheduleDetailsList = JsonConvert.DeserializeObject<List<PaymentReportDetails>>(result.Result);
                    List<PaymentReportDetails> scheduleDetailsListFilterd = new List<PaymentReportDetails>();
                    scheduleDetailsListFilterd = scheduleDetailsList.ToList();

                    if (scheduleDetailsListFilterd.Count != 0)
                    {
                        CaretakerBookingsReport.Visible = true;
                    }
                    else
                    {
                        lblmessage.Visible = true;

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

                    ReportDataSource datasource = new ReportDataSource("CaretakerBookings", scheduleDetailsListFilterd);
                    CaretakerBookingsReport.LocalReport.DataSources.Clear();
                    CaretakerBookingsReport.LocalReport.DataSources.Add(datasource);

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
                    this.CaretakerBookingsReport.LocalReport.SetParameters(reportParameters);
                    this.CaretakerBookingsReport.LocalReport.Refresh();

                    List<CompanyProfile> companyProfile = new List<CompanyProfile>();
                    string apia = "Admin/GetCompanyProfiles/0";
                    var results = service.GetAPI(apia);
                    CompanyProfile listCompanyProfile = JsonConvert.DeserializeObject<CompanyProfile>(results);
                    companyProfile.Add(listCompanyProfile);
                    ReportDataSource datasourceCompanyProfile = new ReportDataSource("CompanyProfile", companyProfile);
                    CaretakerBookingsReport.LocalReport.DataSources.Add(datasourceCompanyProfile);

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