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
    public partial class CaretakerReportClientWise : System.Web.UI.Page
    {
        PCMSLogger Logger = new PCMSLogger();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {

                    ReportViewer1.Visible = false;

                    ReportViewer1.ProcessingMode = ProcessingMode.Local;
                    ReportViewer1.ShowRefreshButton = false;
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/ClientCaretakerAvailablity.rdlc");

                    DateTime fromdate = DateTime.MinValue; DateTime todate = DateTime.MinValue;
                    int clientId = 0;
                    string clientText = "";
                    clientText = (Request.QueryString["clientText"]);
                    Service service = new Service();

                    if (Request.QueryString["fromdate"] != "")
                    {
                        fromdate = Convert.ToDateTime(Request.QueryString["fromdate"]);
                    }
                    if (Request.QueryString["todate"] != "")
                    {
                        todate = Convert.ToDateTime(Request.QueryString["todate"]);
                    }
                    if (Request.QueryString["clientId"] != "")
                    {
                        clientId = Convert.ToInt32(Request.QueryString["clientId"]);
                    }


                    PaymentAdvancedSearch searchInputs = new PaymentAdvancedSearch();
                    searchInputs.FromDate = fromdate;
                    searchInputs.ToDate = todate;
                    searchInputs.ClientId = clientId;



                    string api = "CareTaker/GetAvailableCareTakerListReport";
                    var advancedSearchInputModel = JsonConvert.SerializeObject(searchInputs);
                    var result = service.PostAPIWithData(advancedSearchInputModel, api);
                    List<CaretakerAvailableReport> lstCaretakers = new List<CaretakerAvailableReport>();
                    lstCaretakers = JsonConvert.DeserializeObject<List<CaretakerAvailableReport>>(result.Result);
                    if (lstCaretakers.Count != 0)
                    {
                        if (0 != (lstCaretakers.Where(x => x.ClientId != 0).Count()))
                        {
                            ReportViewer1.Visible = true;
                            lblmessage.Visible = false;
                        }
                        else
                        {
                            lblmessage.Visible = true;
                            ReportViewer1.Visible = false;
                        }
                    }
                    else
                    {
                        lblmessage.Visible = true;
                        ReportViewer1.Visible = false;

                    }
                    if (lstCaretakers != null)
                    {
                        ReportDataSource datasource = new ReportDataSource("CareTakerAvailable", lstCaretakers);
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportViewer1.LocalReport.DataSources.Add(datasource);

                        ReportParameterCollection reportParameters = new ReportParameterCollection();
                        int year = 0;
                        string monthText = "--Select Month--";
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
                        reportParameters.Add(new ReportParameter("Client", clientText.ToString()));

                        this.ReportViewer1.LocalReport.SetParameters(reportParameters);
                        this.ReportViewer1.LocalReport.Refresh();
                        List<CompanyProfile> listCompanyProfile = new List<CompanyProfile>();
                        CompanyProfile companyProfile = new CompanyProfile();
                        string apia = "Admin/GetCompanyProfiles/0";
                        var results = service.GetAPI(apia);
                        companyProfile = JsonConvert.DeserializeObject<CompanyProfile>(results);
                        listCompanyProfile.Add(companyProfile);
                        ReportDataSource datasourceCompanyProfile = new ReportDataSource("CompanyProfile", listCompanyProfile);
                        ReportViewer1.LocalReport.DataSources.Add(datasourceCompanyProfile);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error occurred in Admin Controller-Interview");
                }

            }


        }
    }
}