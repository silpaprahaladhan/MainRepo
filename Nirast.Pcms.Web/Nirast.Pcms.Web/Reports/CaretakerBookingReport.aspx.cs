using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using Nirast.Pcms.Web.Helpers;
using Nirast.Pcms.Web.Logger;
using Nirast.Pcms.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nirast.Pcms.Web.Reports
{
    public partial class CaretakerBookingReport : System.Web.UI.Page
    {
        PCMSLogger Logger = new PCMSLogger();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    rptCaretakerBookingReport.Visible = false;
                    rptCaretakerBookingReport.ShowRefreshButton = false;
                    rptCaretakerBookingReport.ProcessingMode = ProcessingMode.Local;
                    rptCaretakerBookingReport.LocalReport.DisplayName = "UserBookingReport";
                    rptCaretakerBookingReport.LocalReport.ReportPath = Server.MapPath("~/Reports/CaretakerBookingReport.rdlc");

                    CaretakerBookingReportModel searchInputs = new CaretakerBookingReportModel();
                    List<CaretakerBookingReportView> bookingReport = new List<CaretakerBookingReportView>();

                    searchInputs.FromDate = (Request.QueryString["fromdate"] != "null") ? (DateTime?)Convert.ToDateTime(Request.QueryString["fromdate"]) : null;
                    searchInputs.ToDate = (Request.QueryString["todate"] != "null") ? (DateTime?)Convert.ToDateTime(Request.QueryString["todate"]) : null;
                    searchInputs.CategoryId = (Request.QueryString["categoryId"] != "null") ? (int?)Convert.ToInt32(Request.QueryString["categoryId"]) : null;//clienmt id is passing to storeprocedure in caretaker property
                    searchInputs.ServiceId = (Request.QueryString["serviceId"] != "null") ? (int?)Convert.ToInt32(Request.QueryString["serviceId"]) : null;
                    //searchInputs.CaretakerName = (Request.QueryString["caretakerName"] != "null" )? (int?)(Convert.ToInt32(Request.QueryString["caretakerName"])) : null;
                    searchInputs.Year = (Request.QueryString["year"] != "null") ? (int?)Convert.ToInt32(Request.QueryString["year"]) : null;
                    searchInputs.Month = (Request.QueryString["month"] != "null") ? (int?)Convert.ToInt32(Request.QueryString["month"]) : null;

                    string ServiceName = Request.QueryString["Service"].ToString();

                    Service service = new Service();
                    string api = "Admin/GetBookingHistoryReport";
                    var advancedSearchInputModel = JsonConvert.SerializeObject(searchInputs);
                    var result = service.PostAPIWithData(advancedSearchInputModel, api);
                    bookingReport = JsonConvert.DeserializeObject<List<CaretakerBookingReportView>>(result.Result);
                    if (bookingReport.Count != 0)
                    {
                        rptCaretakerBookingReport.Visible = true;
                    }
                    else
                    {
                        lblmessage.Visible = true;

                    }
                    int year = 0; int month = 0; DateTime fromdate = DateTime.MinValue;
                    year = (Request.QueryString["year"] != "null")? Convert.ToInt32(Request.QueryString["year"]) : 0;
                    month = (Request.QueryString["month"] != "null") ? Convert.ToInt32(Request.QueryString["month"]) : 0;
                    fromdate = (Request.QueryString["fromdate"] != "null") ? Convert.ToDateTime(Request.QueryString["fromdate"]) : DateTime.MinValue;

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

                    //List<BookingHistory> scheduleDetailsListFilterd = new List<BookingHistory>();
                    //scheduleDetailsListFilterd = scheduleDetailsList.ToList();


                    ReportDataSource datasource = new ReportDataSource("BookingList", bookingReport);
                    rptCaretakerBookingReport.LocalReport.DataSources.Clear();
                    rptCaretakerBookingReport.LocalReport.DataSources.Add(datasource);
                   
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

                    reportParameters.Add(new ReportParameter("ServiceName", ServiceName.ToString()));
                    this.rptCaretakerBookingReport.LocalReport.SetParameters(reportParameters);
                    this.rptCaretakerBookingReport.LocalReport.Refresh();

                    List<CompanyProfile> companyProfile = new List<CompanyProfile>();
                    string apia = "Admin/GetCompanyProfiles/0";
                    var results = service.GetAPI(apia);
                    CompanyProfile listCompanyProfile = JsonConvert.DeserializeObject<CompanyProfile>(results);
                    companyProfile.Add(listCompanyProfile);
                    ReportDataSource datasourceCompanyProfile = new ReportDataSource("CompanyProfile", companyProfile);
                    rptCaretakerBookingReport.LocalReport.DataSources.Add(datasourceCompanyProfile);
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