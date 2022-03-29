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
    public partial class ClientScheduleReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PCMSLogger Logger = new PCMSLogger();
            if (!IsPostBack)
            {
                try
                {

                    ReportViewer1.Visible = false;

                    ReportViewer1.ProcessingMode = ProcessingMode.Local;
                    ReportViewer1.ShowRefreshButton = false;
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/ClientScheduleReport.rdlc");

                    PaymentAdvancedSearch searchInputs = new PaymentAdvancedSearch();
                    List<ScheduledData> scheduleDetailsList = new List<ScheduledData>();
                    DateTime fromdate = DateTime.MinValue, todate = DateTime.MinValue;
                    int caretaker = 0, year = 0, month = 0, workmode = 0, category = 0, client = 0;

                    if (Request.QueryString["clientId"] != "null" && Request.QueryString["clientId"] != "")
                    {
                        client = Convert.ToInt32(Request.QueryString["clientId"]);
                    }

                    if (Request.QueryString["caretaker"] != "null" && Request.QueryString["caretaker"] != "")
                    {
                        caretaker = Convert.ToInt32(Request.QueryString["caretaker"]);
                    }
                    if (Request.QueryString["year"] != "null" && Request.QueryString["year"] != "")
                    {
                        year = Convert.ToInt32(Request.QueryString["year"]);
                    }
                    if (Request.QueryString["month"] != "null" && Request.QueryString["month"] != "")
                    {
                        month = Convert.ToInt32(Request.QueryString["month"]);
                    }
                    if (Request.QueryString["work"] != "null" && Request.QueryString["work"] != "")
                    {
                        workmode = Convert.ToInt32(Request.QueryString["work"]);
                    }
                    if (Request.QueryString["category"] != "null" && Request.QueryString["category"] != "")
                    {
                        category = Convert.ToInt32(Request.QueryString["category"]);
                    }

                    if (Request.QueryString["fromdate"] != "")
                    {
                        fromdate = Convert.ToDateTime(Request.QueryString["fromdate"]);
                    }
                    if (Request.QueryString["todate"] != "")
                    {
                        todate = Convert.ToDateTime(Request.QueryString["todate"]);
                    }

                    searchInputs.Service = workmode;
                    searchInputs.Category = category;
                    searchInputs.CareTakerId = caretaker;
                    searchInputs.FromDate = fromdate;
                    searchInputs.ToDate = todate;
                    searchInputs.Year = year;
                    searchInputs.Month = month;
                    searchInputs.ClientId = client;

                    Service service = new Service();
                    string api = "Home/SearchClientScheduleReoprt";
                    var advancedSearchInputModel = JsonConvert.SerializeObject(searchInputs);
                    var result = service.PostAPIWithData(advancedSearchInputModel, api);
                    scheduleDetailsList = JsonConvert.DeserializeObject<List<ScheduledData>>(result.Result);

                    //Service service = new Service();
                    //string api = "Client/GetAllScheduledetails/0";
                    //var result = service.GetAPI(api);
                    //scheduleDetailsList = JsonConvert.DeserializeObject<List<ScheduledData>>(result);

                    List<ScheduledData> scheduleDetailsListFilterd = new List<ScheduledData>();
                    scheduleDetailsListFilterd = scheduleDetailsList.ToList();

                    //if (clientId == 0)
                    //{
                    //    scheduleDetailsListFilterd = scheduleDetailsList.ToList();
                    //}
                    //else
                    //{
                    //    scheduleDetailsListFilterd = scheduleDetailsList.Where(a => a.ClientId == clientId).ToList();
                    //}

                    if (scheduleDetailsListFilterd.Count != 0)
                    {
                        ReportViewer1.Visible = true;
                    }
                    else
                    {
                        lblmessage.Visible = true;

                    }


                    ReportDataSource datasource = new ReportDataSource("Schedule", scheduleDetailsListFilterd);
                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportViewer1.LocalReport.DataSources.Add(datasource);
                    //ReportViewer1.SizeToReportContent = true;

                    List<CompanyProfile> companyProfile = new List<CompanyProfile>();
                    string apia = "Admin/GetCompanyProfiles/0";
                    var results = service.GetAPI(apia);
                    CompanyProfile listCompanyProfile = JsonConvert.DeserializeObject<CompanyProfile>(results);
                    companyProfile.Add(listCompanyProfile);
                    ReportDataSource datasourceCompanyProfile = new ReportDataSource("CompanyProfile", companyProfile);

                    ReportViewer1.LocalReport.DataSources.Add(datasourceCompanyProfile);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error occurred in Admin Controller-Interview");
                }
            }

        }
    }
}