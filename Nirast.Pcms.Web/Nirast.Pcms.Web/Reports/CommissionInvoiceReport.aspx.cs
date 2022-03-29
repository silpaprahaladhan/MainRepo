using Ionic.Zip;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using Nirast.Pcms.Web.Helpers;
using Nirast.Pcms.Web.Logger;
using Nirast.Pcms.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Nirast.Pcms.Web.Reports
{
    public partial class CommissionInvoiceReport : System.Web.UI.Page
    {
        PCMSLogger pCMSLogger = new PCMSLogger();
        List<string> myFiles = new List<string>();
        List<InvoiceSearchInpts> listInvoiceInputs = new List<InvoiceSearchInpts>();
        DateTime fromdate = DateTime.MinValue, todate = DateTime.MinValue;
        string monthText;
        PaymentAdvancedSearch searchInputs = new PaymentAdvancedSearch();
        int year;
        int NextInvoiceNumber, InvoiceNumber;
        List<ScheduledData> scheduleDetailsList = new List<ScheduledData>();
        List<ScheduledData> scheduleDetailsListSeperateInvoice = new List<ScheduledData>();
        string InvoicePrefix;
        string invoiceAddress = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            ValidationSettings.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
            if (!IsPostBack)
            {
                try
                {
                    ReportViewer1.ProcessingMode = ProcessingMode.Local;
                    ReportViewer1.ShowRefreshButton = false;
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/CommissionInvoice.rdlc");
                    ReportViewer1.Visible = false;



                    PaymentAdvancedSearch searchInputs = new PaymentAdvancedSearch();
                    List<ScheduledData> scheduleDetailsList = new List<ScheduledData>();
                    DateTime fromdate = DateTime.MinValue, todate = DateTime.MinValue;
                    int BranchId=Convert.ToInt32(Request.QueryString["BranchId"]);
                    int clientId = Convert.ToInt32(Request.QueryString["clientId"]);
                    int category = Convert.ToInt32(Request.QueryString["category"]);
                    bool isOrientation = Convert.ToBoolean(Request.QueryString["isOrientation"]);
                    int year = Convert.ToInt32(Request.QueryString["year"]);
                    int month = Convert.ToInt32(Request.QueryString["month"]);
                    string monthText = (Request.QueryString["monthText"]);
                    if (Request.QueryString["fromdate"] != "")
                    {
                        fromdate = Convert.ToDateTime(Request.QueryString["fromdate"]);
                    }
                    if (Request.QueryString["todate"] != "")
                    {
                        todate = Convert.ToDateTime(Request.QueryString["todate"]);
                    }
                    searchInputs.ClientId = clientId;
                    searchInputs.CareTakerId = 0;
                    searchInputs.FromDate = fromdate;
                    searchInputs.ToDate = todate;
                    searchInputs.Category = category;
                    searchInputs.BranchId = BranchId;
                    if (month == 0)
                    {
                        searchInputs.Year = 0;
                    }
                    else
                    {
                        searchInputs.Year = 0;
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


                    searchInputs.IsOrientation = isOrientation;
                    Service service = new Service();
                    string api = "Home/GetClientScheduledDetailsByBranchWise";
                    var advancedSearchInputModel = JsonConvert.SerializeObject(searchInputs);
                    var result = service.PostAPIWithData(advancedSearchInputModel, api);
                    scheduleDetailsList = JsonConvert.DeserializeObject<List<ScheduledData>>(result.Result);

                    CommissionInputs commissionInputs = new CommissionInputs()
                    {
                        BranchId = BranchId,
                        Date = DateTime.UtcNow
                    };

                    string api2 = "Admin/GetCommission";
                    var commission = JsonConvert.SerializeObject(commissionInputs);
                    var result1 = service.PostAPIWithData(commission, api2);


                    if (scheduleDetailsList.Count != 0)
                    {
                        invoiceAddress = scheduleDetailsList[0].InvoiceAddress;

                    }
                    if (null == invoiceAddress)
                    {
                        invoiceAddress = "";
                    }

                    if (searchInputs.FromDate != DateTime.MinValue)
                    {
                        scheduleDetailsList = scheduleDetailsList.Where(a => Convert.ToDateTime(a.Startdate).Date <= Convert.ToDateTime(searchInputs.ToDate).Date).ToList();


                        //scheduleDetailsList = scheduleDetailsList.Where(a => Convert.ToDateTime(a.Startdate).Date <= Convert.ToDateTime(searchInputs.ToDate).Date).Where(a => Convert.ToDateTime(a.Startdate).Date >= Convert.ToDateTime(searchInputs.FromDate).Date).ToList();
                    }

                    List<ScheduledData> holidaySchedules = new List<ScheduledData>();
                    holidaySchedules = scheduleDetailsList.Where(x => x.HoildayHours != 0).ToList();


                    scheduleDetailsList = scheduleDetailsList.Where(x => x.HoildayHours == 0).GroupBy(l => l.SchedulingId)
                                                         .Select(cl => new ScheduledData
                                                         {

                                                             SchedulingId = cl.First().SchedulingId,
                                                             ClientName = cl.First().ClientName,
                                                             CurrencySymbol = cl.First().CurrencySymbol,
                                                             StartDateTime = cl.First().StartDateTime,
                                                             BuildingName = cl.First().BuildingName,
                                                             StateName = cl.First().StateName,
                                                             Zipcode = cl.First().Zipcode,
                                                             Phone1 = cl.First().Phone1,
                                                             Startdate = cl.First().Startdate,
                                                             Enddate = cl.First().Enddate,
                                                             Description = cl.First().Description,
                                                             TimeIn = cl.First().TimeIn,
                                                             TimeOut = cl.First().EndDateTime > Convert.ToDateTime(searchInputs.ToDate).AddDays(1).Date ? "12:00 AM" : cl.First().EndDateTime.ToString("hh:mm tt"),
                                                             WorkTimeName = cl.First().WorkTimeName,
                                                             CareTakerName = cl.First().CareTakerName,
                                                             ServiceTypeName = cl.First().ServiceTypeName,
                                                             WorkModeName = cl.First().WorkModeName,
                                                             IsSeparateInvoice = cl.First().IsSeparateInvoice,
                                                             Rate = cl.First().Rate,
                                                             Amount = cl.First().Amount,
                                                             StateId = cl.First().StateId,
                                                             HoildayHours = cl.First().HoildayHours,
                                                             HoildayAmout = cl.First().HoildayAmout,
                                                             HolidayPayValue = cl.First().HolidayPayValue,
                                                             CountryId = cl.First().CountryId,
                                                             Hours = cl.Sum(k => Convert.ToDecimal(k.Hours)).ToString(),
                                                             Total = cl.Sum(k => k.Total),
                                                             HST = cl.Sum(k => k.HST),
                                                             InvoiceNumber = cl.First().InvoiceNumber,
                                                             InvoicePrefix = cl.First().InvoicePrefix
                                                         }).ToList();

                    scheduleDetailsList.AddRange(holidaySchedules);

                    ReportDataSource datasource = new ReportDataSource("ClientInvoice", scheduleDetailsList);
                    if (scheduleDetailsList.Count != 0)
                    {
                        ReportViewer1.Visible = true;
                    }
                    else
                    {
                        lblmessage.Visible = true;

                    }
                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportViewer1.LocalReport.DataSources.Add(datasource);
                    ReportParameterCollection reportParameters = new ReportParameterCollection();
                    reportParameters.Add(new ReportParameter("Year", year.ToString()));
                    reportParameters.Add(new ReportParameter("Commission", result1.Result.ToString()));
                    if (monthText == "--Select Month--" || monthText == null)
                    {
                        reportParameters.Add(new ReportParameter("FromDate", Convert.ToDateTime(searchInputs.FromDate).ToString("dd MMM yyyy")));
                        reportParameters.Add(new ReportParameter("Todate", Convert.ToDateTime(searchInputs.ToDate).ToString("dd MMM yyyy")));
                        // reportParameters.Add(new ReportParameter("InvoiceDate", Convert.ToDateTime(DateTime.Now).ToString("dd MMM yyyy")));
                        ReportViewer1.LocalReport.DisplayName = "ClientInvoice" + "  " + Convert.ToDateTime(searchInputs.FromDate).ToString("dd MMM yyyy") + " to " + Convert.ToDateTime(searchInputs.ToDate).ToString("dd MMM yyyy");
                    }
                    else
                    {
                        reportParameters.Add(new ReportParameter("monthText", monthText.ToString()));
                        ReportViewer1.LocalReport.DisplayName = "ClientInvoice" + " " + monthText.ToString() + " " + year.ToString();

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



                }
                catch (Exception ex)
                {
                    pCMSLogger.Error(ex, "Error occurred in Admin Controller-ClientInvoiceReport.cs");
                }

            }

        }
    }

}