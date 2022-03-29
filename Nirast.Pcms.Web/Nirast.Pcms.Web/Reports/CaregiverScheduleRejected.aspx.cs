using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using Nirast.Pcms.Web.Helpers;
using Nirast.Pcms.Web.Models;
using Nirast.Pcms.Web.Logger;

namespace Nirast.Pcms.Web.Reports
{
    public partial class CaregiverScheduleRejected : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PCMSLogger pCMSLogger = new PCMSLogger();
            if (!IsPostBack)
            {
                try
                {
                    scheduleRejectedReport.Visible = false;
                    scheduleRejectedReport.ShowRefreshButton = false;
                    scheduleRejectedReport.ProcessingMode = ProcessingMode.Local;
                    scheduleRejectedReport.LocalReport.DisplayName = "CaregiverScheduleRejected";
                    scheduleRejectedReport.LocalReport.ReportPath = Server.MapPath("~/Reports/CaregiverScheduleRejected.rdlc");

                    BookingHistorySearch searchInputs = new BookingHistorySearch();
                    List<RejectedCaretaker> bookingRejectedReport = new List<RejectedCaretaker>();
                    int year = DateTime.Now.Year;
                    DateTime firstDay = new DateTime(year, 1, 1);
                    DateTime lastDay = new DateTime(year, 12, 31);
                    if (Request.QueryString["fromdate"] == "")
                    {
                        searchInputs.FromDate = firstDay;
                    }
                    else
                    {
                        searchInputs.FromDate = (Request.QueryString["fromdate"] != "null") ? (DateTime?)Convert.ToDateTime(Request.QueryString["fromdate"]) : null;
                    }
                    if (Request.QueryString["todate"] == "")
                    {
                        searchInputs.ToDate = lastDay;
                    }
                    else
                    {
                        searchInputs.ToDate = (Request.QueryString["todate"] != "null") ? (DateTime?)Convert.ToDateTime(Request.QueryString["todate"]) : null;

                    }
                    searchInputs.Caretaker = (Request.QueryString["caretakerName"]) != "" ? (Request.QueryString["caretakerName"]) : "--Select--";




                    Service service = new Service();
                    
                    string api = "Client/GetAllScheduleRejectedCaretaker";
                    var advancedSearchInputModel = JsonConvert.SerializeObject(searchInputs);
                    var result = service.PostAPIWithData(advancedSearchInputModel, api);
                    bookingRejectedReport = JsonConvert.DeserializeObject<List<RejectedCaretaker>>(result.Result);
                    if (bookingRejectedReport.Count != 0)
                    {
                        scheduleRejectedReport.Visible = true;
                    }
                    else
                    {
                        lblmessage.Visible = true;

                    }
                    

                    

                    ReportDataSource datasource = new ReportDataSource("BookingRejectedList", bookingRejectedReport);
                    scheduleRejectedReport.LocalReport.DataSources.Clear();
                    scheduleRejectedReport.LocalReport.DataSources.Add(datasource);


                    ReportParameterCollection reportParameters = new ReportParameterCollection();

                    reportParameters.Add(new ReportParameter("CaregiverName", Convert.ToString(searchInputs.Caretaker).ToString()));
                    reportParameters.Add(new ReportParameter("FromDate", Convert.ToDateTime(searchInputs.FromDate).ToString("dd MMM yyyy")));
                    reportParameters.Add(new ReportParameter("Todate", Convert.ToDateTime(searchInputs.ToDate).ToString("dd MMM yyyy")));



                    this.scheduleRejectedReport.LocalReport.SetParameters(reportParameters);
                    this.scheduleRejectedReport.LocalReport.Refresh();

                    List<CompanyProfile> companyProfile = new List<CompanyProfile>();
                    string apia = "Admin/GetCompanyProfiles/0";
                    var results = service.GetAPI(apia);
                    CompanyProfile listCompanyProfile = JsonConvert.DeserializeObject<CompanyProfile>(results);
                    companyProfile.Add(listCompanyProfile);
                    ReportDataSource datasourceCompanyProfile = new ReportDataSource("CompanyProfile", companyProfile);
                    scheduleRejectedReport.LocalReport.DataSources.Add(datasourceCompanyProfile);
                    //ReportViewer1.SizeToReportContent = true;
                }
                catch (Exception ex)
                {
                    pCMSLogger.Error(ex, "Error occurred in Admin Controller-Interview");
                }
            }
        }

    }
}
