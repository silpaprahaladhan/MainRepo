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
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Nirast.Pcms.Web.Reports
{
    public partial class SaveClientInvoiceReport : System.Web.UI.Page
    {
        PCMSLogger pCMSLogger = new PCMSLogger();
        List<string> myFiles = new List<string>();
        List<InvoiceSearchInpts> listInvoiceInputs = new List<InvoiceSearchInpts>();
        DateTime fromdate = DateTime.MinValue, todate = DateTime.MinValue;
        string monthText;
        PaymentAdvancedSearch searchInputs = new PaymentAdvancedSearch();
        int year;
        string invoiceAddress = "";
        List<ScheduledData> scheduleDetailsList = new List<ScheduledData>();
        List<ScheduledData> scheduleDetailsListSeperateInvoice = new List<ScheduledData>();
        List<ScheduledData> scheduleDetailsListFilterd = new List<ScheduledData>();
        List<ScheduledData> invoiceDetailsList = new List<ScheduledData>();
        string InvoicePrefix;
        int NextInvoiceNumber, InvoiceNumber;
        protected void Page_Load(object sender, EventArgs e)
        {
            ValidationSettings.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
            //  div1.Visible = false;
            if (!IsPostBack)
            {
                try
                {

                    string script = "$(document).ready(function () { $('[id*=btnSubmit]').click(); });";
                    ClientScript.RegisterStartupScript(this.GetType(), "load", script, true);

                    ReportViewer1.ProcessingMode = ProcessingMode.Local;
                    ReportViewer1.ShowRefreshButton = false;
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/ClientInvoice.rdlc");
                    ReportViewer1.Visible = false;
                    ReportViewer2.Visible = false;

                    int clientId = Convert.ToInt32(Request.QueryString["clientId"]);
                    bool isOrientation = Convert.ToBoolean(Request.QueryString["isOrientation"]);
                    year = Convert.ToInt32(Request.QueryString["year"]);
                    int month = Convert.ToInt32(Request.QueryString["month"]);
                    int mode = Convert.ToInt32(Request.QueryString["Mode"]);
                    int category = Convert.ToInt32(Request.QueryString["category"]);
                    monthText = (Request.QueryString["monthText"]);
                    if (Request.QueryString["fromdate"] != "")
                    {
                        fromdate = Convert.ToDateTime(Request.QueryString["fromdate"]);
                    }
                    if (Request.QueryString["todate"] != "")
                    {
                        todate = Convert.ToDateTime(Request.QueryString["todate"]);
                    }
                    DateTime invoiceDate = Convert.ToDateTime(Request.QueryString["invoiceDate"]);
                    searchInputs.ClientId = clientId;
                    searchInputs.CareTakerId = 0;
                    searchInputs.FromDate = fromdate;
                    searchInputs.ToDate = todate;
                    searchInputs.Category = category;
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

                    searchInputs.IsOrientation = isOrientation;
                    Service service = new Service();
                    string api = "Home/GetClientScheduledDetails";
                    var advancedSearchInputModel = JsonConvert.SerializeObject(searchInputs);
                    var result = service.PostAPIWithData(advancedSearchInputModel, api);
                    scheduleDetailsList = JsonConvert.DeserializeObject<List<ScheduledData>>(result.Result);

                    if (scheduleDetailsList.Count!=0)
                    {
                        invoiceAddress = scheduleDetailsList[0].InvoiceAddress;

                    }                    
                    if (null==invoiceAddress)
                    {
                        invoiceAddress = "";
                    }



                    string api2 = "Home/GetInvoiceGenerationDetails";
                    var invoiceDetails = JsonConvert.SerializeObject(searchInputs);
                    var invoiceResult = service.PostAPIWithData(invoiceDetails, api2);
                    invoiceDetailsList = JsonConvert.DeserializeObject<List<ScheduledData>>(invoiceResult.Result);


                    api = "/Admin/GetCategory?flag=*&value=''";
                    List<CategoryViewModel> categoryList = new List<CategoryViewModel>();
                    var resultList = service.GetAPI(api);
                    categoryList = JsonConvert.DeserializeObject<List<CategoryViewModel>>(resultList);

                    List<int> categoryIdList = new List<int>();

                    if (category != 0)
                    {

                        if (searchInputs.FromDate != DateTime.MinValue)
                        {
                            scheduleDetailsList = scheduleDetailsList.Where(a => Convert.ToDateTime(a.Startdate).Date <= Convert.ToDateTime(searchInputs.ToDate).Date).Where(a => Convert.ToDateTime(a.Startdate).Date >= Convert.ToDateTime(searchInputs.FromDate).Date).ToList();
                            scheduleDetailsList = scheduleDetailsList.Where(x => (x.IsSeparateInvoice || x.CareTakerType == category) ).ToList();
                        }
                    }
                    else
                    {
                        foreach (var item in categoryList)
                        {
                            if (!invoiceDetailsList.Any(x => x.CareTakerType == item.CategoryId))
                            {
                                categoryIdList.Add(item.CategoryId);
                            }
                        }

                        if (searchInputs.FromDate != DateTime.MinValue)
                        {
                            scheduleDetailsList = scheduleDetailsList.Where(a => Convert.ToDateTime(a.Startdate).Date <= Convert.ToDateTime(searchInputs.ToDate).Date).Where(a => Convert.ToDateTime(a.Startdate).Date >= Convert.ToDateTime(searchInputs.FromDate).Date).ToList();
                            scheduleDetailsList = scheduleDetailsList.FindAll(x => categoryIdList.Contains(x.CareTakerType)).ToList();
                        }
                    }

                    if (invoiceDetailsList.Count == 0)
                    {
                        if (searchInputs.FromDate != DateTime.MinValue)
                        {
                            scheduleDetailsList = scheduleDetailsList.Where(a => Convert.ToDateTime(a.Startdate).Date <= Convert.ToDateTime(searchInputs.ToDate).Date).Where(a => Convert.ToDateTime(a.Startdate).Date >= Convert.ToDateTime(searchInputs.FromDate).Date).ToList();
                        }
                        InvoiceGeneration(service);
                    }
                    else
                    {
                        if ((invoiceDetailsList.All(inv => inv.IsSeparateInvoice == true)) && (scheduleDetailsList.Count != 0))
                        {
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

                            scheduleDetailsListFilterd = scheduleDetailsList.Where(a => a.IsSeparateInvoice == false).ToList();
                            List<ScheduledData> list1 = scheduleDetailsListFilterd;
                            list1 = list1.OrderBy(x => x.StartDateTime).ThenBy(x => x.TimeIn).ToList();
                            list1 = list1.GroupBy(p => p.ServiceTypeName).Select(g => g.First()).OrderBy(x => x.StartDateTime).ThenBy(x => x.TimeIn).ToList();
                            list1 = list1.OrderBy(x => x.ServiceTypeName).ToList();



                            for (int i = 0; i < list1.Count(); i++)
                            {
                                foreach (var item in scheduleDetailsListFilterd.Where(w => w.ServiceTypeName == list1[i].ServiceTypeName))
                                {
                                    item.InvoiceNumber = item.InvoiceNumber + i;
                                    InvoiceNumber = item.InvoiceNumber;
                                }
                            }
                            ReportDataSource datasource = new ReportDataSource("ClientInvoice", scheduleDetailsListFilterd);
                            if (scheduleDetailsListFilterd.Count != 0)
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
                            reportParameters.Add(new ReportParameter("InvoiceAddress", invoiceAddress.ToString()));
                           
                            if (monthText == "--Select Month--")
                            {
                                reportParameters.Add(new ReportParameter("FromDate", Convert.ToDateTime(searchInputs.FromDate).ToString("dd MMM yyyy")));
                                reportParameters.Add(new ReportParameter("Todate", Convert.ToDateTime(searchInputs.ToDate).ToString("dd MMM yyyy")));
                                reportParameters.Add(new ReportParameter("InvoiceDate", Convert.ToDateTime(searchInputs.InvoiceDate).ToString("dd MMM yyyy")));
                                ReportViewer1.LocalReport.DisplayName = "ClientInvoice" + "  " + fromdate.ToString("dd MMM yyyy") + " to " + todate.ToString("dd MMM yyyy");
                            }
                            else
                            {
                                reportParameters.Add(new ReportParameter("monthText", monthText.ToString()));
                                reportParameters.Add(new ReportParameter("InvoiceDate", Convert.ToDateTime(searchInputs.InvoiceDate).ToString("dd MMM yyyy")));
                                ReportViewer1.LocalReport.DisplayName = "ClientInvoice" + " " + monthText.ToString() + " " + year.ToString();

                            }

                            this.ReportViewer1.LocalReport.SetParameters(reportParameters);
                            this.ReportViewer1.LocalReport.Refresh();
                            if (scheduleDetailsListSeperateInvoice.Count == 0)
                            {
                                lblmessagesplit.Text = "The Invoice Already exists with same parameters ";
                                lblmessagesplit.Visible = true;
                            }

                        }
                        else if ((invoiceDetailsList.All(inv => inv.IsSeparateInvoice == false)) && (scheduleDetailsList.Count != 0))
                        {

                            ReportViewer2.ProcessingMode = ProcessingMode.Local;
                            ReportViewer2.ShowRefreshButton = false;
                            ReportViewer2.LocalReport.ReportPath = Server.MapPath("~/Reports/ClientInvoiceSeperately.rdlc");
                            scheduleDetailsListSeperateInvoice = scheduleDetailsList.Where(a => a.IsSeparateInvoice == true).ToList();

                            List<ScheduledData> list = scheduleDetailsListSeperateInvoice;
                            list = list.OrderBy(x => x.StartDateTime).ThenBy(x => x.TimeIn).ToList();
                            list = list.GroupBy(p => new { p.Description, p.ServiceTypeName }).Select(g => g.First()).OrderBy(x => x.StartDateTime).ThenBy(x => x.TimeIn).ToList();
                            if (list.Count() == 0)
                            {
                                NextInvoiceNumber = InvoiceNumber;
                            }
                            for (int i = 0; i < list.Count(); i++)
                            {
                                if (scheduleDetailsListFilterd.Count > 0)
                                {
                                    foreach (var item in scheduleDetailsListSeperateInvoice.Where(w => w.Description == list[i].Description && w.ServiceTypeName == list[i].ServiceTypeName))
                                    {
                                        if (scheduleDetailsListFilterd.Count() == 0)
                                        {
                                            InvoiceNumber = item.InvoiceNumber;
                                        }
                                        item.InvoiceNumber = InvoiceNumber + 1 + i;
                                        NextInvoiceNumber = item.InvoiceNumber;
                                    }
                                }
                                else
                                {
                                    foreach (var item in scheduleDetailsListSeperateInvoice.Where(w => w.Description == list[i].Description && w.ServiceTypeName == list[i].ServiceTypeName))
                                    {
                                        if (scheduleDetailsListFilterd.Count() == 0)
                                        {
                                            InvoiceNumber = item.InvoiceNumber;
                                        }
                                        item.InvoiceNumber = InvoiceNumber + i;
                                        NextInvoiceNumber = item.InvoiceNumber;
                                    }
                                }
                            }


                            ReportDataSource datasourceNew = new ReportDataSource("ClientInvoice", scheduleDetailsListSeperateInvoice);
                            if (scheduleDetailsListSeperateInvoice.Count != 0)
                            {
                                ReportViewer2.Visible = true;
                            }
                            else
                            {
                                lblmessagesplit.Visible = true;

                            }


                            ReportViewer2.LocalReport.DataSources.Clear();
                            ReportViewer2.LocalReport.DataSources.Add(datasourceNew);
                            ReportParameterCollection reportParameters2 = new ReportParameterCollection();
                            reportParameters2.Add(new ReportParameter("Year", year.ToString()));
                            reportParameters2.Add(new ReportParameter("InvoiceAddress", invoiceAddress.ToString()));
                            reportParameters2.Add(new ReportParameter("InvoiceDate", Convert.ToDateTime(searchInputs.InvoiceDate).ToString("dd MMM yyyy")));
                            if (monthText == "--Select Month--")
                            {
                                reportParameters2.Add(new ReportParameter("FromDate", Convert.ToDateTime(searchInputs.FromDate).ToString("dd MMM yyyy")));
                                reportParameters2.Add(new ReportParameter("Todate", Convert.ToDateTime(searchInputs.ToDate).ToString("dd MMM yyyy")));
                                ReportViewer2.LocalReport.DisplayName = "ClientInvoice" + "  " + fromdate.ToString("dd MMM yyyy") + " to " + todate.ToString("dd MMM yyyy");
                            }
                            else
                            {
                                reportParameters2.Add(new ReportParameter("monthText", monthText.ToString()));
                                ReportViewer2.LocalReport.DisplayName = "ClientInvoice" + " " + monthText.ToString() + " " + year.ToString();

                            }

                            this.ReportViewer2.LocalReport.SetParameters(reportParameters2);
                            this.ReportViewer2.LocalReport.Refresh();
                            if (scheduleDetailsListFilterd.Count == 0)
                            {
                                lblmessage.Text = "The Invoice Already exists with same parameters ";
                                lblmessage.Visible = true;
                            }
                        }
                        else
                        {
                            lblmessage.Text = "The Invoice Already exists with same parameters ";
                            lblmessage.Visible = true;
                            lblmessagesplit.Text = "The Invoice Already exists with same parameters";
                            lblmessagesplit.Visible = true;

                        }
                        List<CompanyProfile> companyProfile = new List<CompanyProfile>();
                        string apia = "Admin/GetCompanyProfiles/0";
                        var results = service.GetAPI(apia);
                        CompanyProfile listCompanyProfile = JsonConvert.DeserializeObject<CompanyProfile>(results);
                        companyProfile.Add(listCompanyProfile);
                        ReportDataSource datasourceCompanyProfile = new ReportDataSource("CompanyProfile", companyProfile);

                        ReportViewer1.LocalReport.DataSources.Add(datasourceCompanyProfile);
                        ReportViewer2.LocalReport.DataSources.Add(datasourceCompanyProfile);

                        GenerateInvoice();
                        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "script", "<script type='text/javascript'>hideProgress();</script>", false);
                        ScriptManager.RegisterClientScriptBlock(this, GetType(), "none", "<script>hideProgress();</script>", false);
                    }
                }

                catch (Exception ex)
                {
                    pCMSLogger.Error(ex, "Error occurred in Admin Controller-Interview");
                }
            }
        }

        private void InvoiceGeneration(Service service)
        {
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

            scheduleDetailsListFilterd = scheduleDetailsList.Where(a => a.IsSeparateInvoice == false).ToList();
            List<ScheduledData> list1 = scheduleDetailsListFilterd;
            list1 = list1.OrderBy(x => x.StartDateTime).ThenBy(x => x.TimeIn).ToList();
            list1 = list1.GroupBy(p => p.ServiceTypeName).Select(g => g.First()).OrderBy(x => x.StartDateTime).ThenBy(x => x.TimeIn).ToList();
            list1 = list1.OrderBy(x => x.ServiceTypeName).ToList();



            for (int i = 0; i < list1.Count(); i++)
            {
                foreach (var item in scheduleDetailsListFilterd.Where(w => w.ServiceTypeName == list1[i].ServiceTypeName))
                {
                    item.InvoiceNumber = item.InvoiceNumber + i;
                    InvoiceNumber = item.InvoiceNumber;
                }

            }
            ReportDataSource datasource = new ReportDataSource("ClientInvoice", scheduleDetailsListFilterd);
            if (scheduleDetailsListFilterd.Count != 0)
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
            reportParameters.Add(new ReportParameter("InvoiceAddress", invoiceAddress.ToString()));
            if (monthText == "--Select Month--")
            {
                reportParameters.Add(new ReportParameter("FromDate", Convert.ToDateTime(searchInputs.FromDate).ToString("dd MMM yyyy")));
                reportParameters.Add(new ReportParameter("Todate", Convert.ToDateTime(searchInputs.ToDate).ToString("dd MMM yyyy")));
                reportParameters.Add(new ReportParameter("InvoiceDate", Convert.ToDateTime(searchInputs.InvoiceDate).ToString("dd MMM yyyy")));
                ReportViewer1.LocalReport.DisplayName = "ClientInvoice" + "  " + fromdate.ToString("dd MMM yyyy") + " to " + todate.ToString("dd MMM yyyy");
            }
            else
            {
                reportParameters.Add(new ReportParameter("monthText", monthText.ToString()));
                reportParameters.Add(new ReportParameter("InvoiceDate", Convert.ToDateTime(searchInputs.InvoiceDate).ToString("dd MMM yyyy")));
                ReportViewer1.LocalReport.DisplayName = "ClientInvoice" + " " + monthText.ToString() + " " + year.ToString();

            }

            this.ReportViewer1.LocalReport.SetParameters(reportParameters);
            this.ReportViewer1.LocalReport.Refresh();


            ReportViewer2.ProcessingMode = ProcessingMode.Local;
            ReportViewer2.ShowRefreshButton = false;
            ReportViewer2.LocalReport.ReportPath = Server.MapPath("~/Reports/ClientInvoiceSeperately.rdlc");
            //List<ScheduledData> scheduleDetailsListSeperateInvoice = new List<ScheduledData>();
            scheduleDetailsListSeperateInvoice = scheduleDetailsList.Where(a => a.IsSeparateInvoice == true).ToList();

            List<ScheduledData> list = scheduleDetailsListSeperateInvoice;
            list = list.OrderBy(x => x.StartDateTime).ThenBy(x => x.TimeIn).ToList();
            list = list.GroupBy(p => new { p.Description, p.ServiceTypeName }).Select(g => g.First()).OrderBy(x => x.StartDateTime).ThenBy(x => x.TimeIn).ToList();
            if (list.Count() == 0)
            {
                NextInvoiceNumber = InvoiceNumber;
            }
            for (int i = 0; i < list.Count(); i++)
            {
                if (scheduleDetailsListFilterd.Count > 0)
                {
                    foreach (var item in scheduleDetailsListSeperateInvoice.Where(w => w.Description == list[i].Description && w.ServiceTypeName == list[i].ServiceTypeName))
                    {
                        if (scheduleDetailsListFilterd.Count() == 0)
                        {
                            InvoiceNumber = item.InvoiceNumber;
                        }
                        item.InvoiceNumber = InvoiceNumber + 1 + i;
                        NextInvoiceNumber = item.InvoiceNumber;
                    }
                }
                else
                {
                    foreach (var item in scheduleDetailsListSeperateInvoice.Where(w => w.Description == list[i].Description && w.ServiceTypeName == list[i].ServiceTypeName))
                    {
                        if (scheduleDetailsListFilterd.Count() == 0)
                        {
                            InvoiceNumber = item.InvoiceNumber;
                        }
                        item.InvoiceNumber = InvoiceNumber + i;
                        NextInvoiceNumber = item.InvoiceNumber;
                    }
                }
            }


            ReportDataSource datasourceNew = new ReportDataSource("ClientInvoice", scheduleDetailsListSeperateInvoice);
            if (scheduleDetailsListSeperateInvoice.Count != 0)
            {
                ReportViewer2.Visible = true;
            }
            else
            {
                lblmessagesplit.Visible = true;

            }
            ReportViewer2.LocalReport.DataSources.Clear();
            ReportViewer2.LocalReport.DataSources.Add(datasourceNew);
            ReportParameterCollection reportParameters2 = new ReportParameterCollection();
            reportParameters2.Add(new ReportParameter("Year", year.ToString()));
            reportParameters2.Add(new ReportParameter("InvoiceAddress", invoiceAddress.ToString()));
            reportParameters2.Add(new ReportParameter("InvoiceDate", Convert.ToDateTime(searchInputs.InvoiceDate).ToString("dd MMM yyyy")));
            if (monthText == "--Select Month--")
            {
                reportParameters2.Add(new ReportParameter("FromDate", Convert.ToDateTime(searchInputs.FromDate).ToString("dd MMM yyyy")));
                reportParameters2.Add(new ReportParameter("Todate", Convert.ToDateTime(searchInputs.ToDate).ToString("dd MMM yyyy")));
                ReportViewer2.LocalReport.DisplayName = "ClientInvoice" + "  " + fromdate.ToString("dd MMM yyyy") + " to " + todate.ToString("dd MMM yyyy");
            }
            else
            {
                reportParameters2.Add(new ReportParameter("monthText", monthText.ToString()));
                ReportViewer2.LocalReport.DisplayName = "ClientInvoice" + " " + monthText.ToString() + " " + year.ToString();

            }

            this.ReportViewer2.LocalReport.SetParameters(reportParameters2);
            this.ReportViewer2.LocalReport.Refresh();

            List<CompanyProfile> companyProfile = new List<CompanyProfile>();
            string apia = "Admin/GetCompanyProfiles/0";
            var results = service.GetAPI(apia);
            CompanyProfile listCompanyProfile = JsonConvert.DeserializeObject<CompanyProfile>(results);
            companyProfile.Add(listCompanyProfile);
            ReportDataSource datasourceCompanyProfile = new ReportDataSource("CompanyProfile", companyProfile);

            ReportViewer1.LocalReport.DataSources.Add(datasourceCompanyProfile);
            ReportViewer2.LocalReport.DataSources.Add(datasourceCompanyProfile);

            GenerateInvoice();
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "script", "<script type='text/javascript'>hideProgress();</script>", false);
            ScriptManager.RegisterClientScriptBlock(this, GetType(), "none", "<script>hideProgress();</script>", false);
        }

        //generate invoice buttuon click
        protected void Button2_Click(object sender, EventArgs e)
        {

            GenerateInvoice();
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "script", "<script type='text/javascript'>hideProgress();</script>", false);
            ScriptManager.RegisterClientScriptBlock(this, GetType(), "none", "<script>hideProgress();</script>", false);

        }
        [System.Web.Services.WebMethod]
        public void GenerateInvoice()
        {
            try
            {
                ClientModel clientModelObj = new ClientModel();
                Service service = new Service();
                int clientId = Convert.ToInt32(Request.QueryString["clientId"]);
                string api = "Client/GetAllClientDetailsById?clientId=" + clientId;
                var result = service.GetAPI(api);
                clientModelObj = JsonConvert.DeserializeObject<ClientModel>(result);

                InvoicePrefix = clientModelObj.InvoicePrefix;
                int InvoiceNumber = clientModelObj.InvoiceNumber;

                DateTime fromdate = DateTime.MinValue, todate = DateTime.MinValue;


                int month = Convert.ToInt32(Request.QueryString["month"]);
                int mode = Convert.ToInt32(Request.QueryString["Mode"]);

                string monthText = (Request.QueryString["monthText"]);
                if (Request.QueryString["fromdate"] != "")
                {
                    fromdate = Convert.ToDateTime(Request.QueryString["fromdate"]);
                }
                if (Request.QueryString["todate"] != "")
                {
                    todate = Convert.ToDateTime(Request.QueryString["todate"]);
                }
                int year = Convert.ToInt32(Request.QueryString["year"]);
                // Variables
                Warning[] warnings;
                string[] streamIds;
                string mimeType = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;
                iTextSharp.text.Image image;
                string filePathSeparate = string.Empty;
                string filePath = string.Empty;
                iTextSharp.text.Font blackFont = iTextSharp.text.FontFactory.GetFont("Tohama", 12, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
                string randomName = "Client Invoice";
                List<ScheduledData> listFloor = ReportViewer1.LocalReport.DataSources[0].Value as List<ScheduledData>;
                List<ScheduledData> listSeperate = ReportViewer2.LocalReport.DataSources[0].Value as List<ScheduledData>;

                string WatermarkLocation = Server.MapPath("~/images/") + "logo_Cut.jpg";
                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(WatermarkLocation);
                img.SetAbsolutePosition(750, 100);
                img.ScaleAbsolute(320f, 255.25f);



                if (listFloor != null && listFloor.Count > 0)
                {
                    //reading sperate invoice =false report viewer and adding invoice number  dynamically and storing to temp folder
                    byte[] bytes = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                    if (monthText == "--Select Month--")
                    {
                        randomName = "ClientInvoice for " + clientModelObj.ClientName + "  " + fromdate.ToString("dd MMM yyyy") + " to " + todate.ToString("dd MMM yyyy") + " " + DateTime.Now.Ticks.ToString();
                    }
                    else
                    {
                        randomName = "ClientInvoice for " + clientModelObj.ClientName + " " + monthText.ToString() + " " + year.ToString() + " " + DateTime.Now.Ticks.ToString();
                    }

                    filePath = Server.MapPath("~/PCMS/Invoice/Client/") + randomName + ".pdf";
                    if (!Directory.Exists(Server.MapPath("~/PCMS/Invoice/Client/")))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/PCMS/Invoice/Client/"));
                    }

                    string returnFileName = randomName + ".pdf";
                    System.IO.File.WriteAllBytes(filePath, bytes);

                    myFiles.Add(filePath);

                }
                if (listSeperate != null && listSeperate.Count > 0)
                {
                    //reading sperate invoice=true report viewer and adding invoice number  and storing to temp folder
                    byte[] bytesSeprate = this.ReportViewer2.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                    string randomNameNew = "";
                    if (monthText == "--Select Month--")
                    {
                        randomNameNew = "ClientInvoice for " + clientModelObj.ClientName + "  " + fromdate.ToString("dd MMM yyyy") + " to " + todate.ToString("dd MMM yyyy") + " " + DateTime.Now.Ticks.ToString();
                    }
                    else
                    {
                        randomNameNew = "ClientInvoice for " + clientModelObj.ClientName + " " + monthText.ToString() + " " + year.ToString() + " " + DateTime.Now.Ticks.ToString();
                    }
                    filePathSeparate = Server.MapPath("~/PCMS/Invoice/Client/") + randomNameNew + " SeperateInvoice.pdf";
                    string outputPath = Server.MapPath("~/PCMS/Invoice/Client/");
                    if (!Directory.Exists(Server.MapPath("~/PCMS/Invoice/Client/")))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/PCMS/Invoice/Client/"));
                    }
                    System.IO.File.WriteAllBytes(filePathSeparate, bytesSeprate);
                    myFiles.Add(filePathSeparate);
                }

                //merge
                string mergedFile = Server.MapPath("~/PCMS/Invoice/Client/") + clientModelObj.ClientName + ".pdf";
                if (mergedFile != null)
                {
                    Button2.Visible = false;
                    div1.Visible = true;
                    emailTxt.Text = clientModelObj.EmailId;
                }
                MergePDFs(mergedFile, myFiles);
                //showing all pdf as singel pdf in oframe    
                ifrmpdfshow.Src = "~/PCMS/Invoice/Client/" + clientModelObj.ClientName + ".pdf";
                divNotSeperate.Visible = false;
                divSeperate.Visible = false;
                divShowpdf.Visible = true;

                RefreshDeafaultInvoiceReportviewr();
                refreshReportviewr();

                //save client invoice search details and pdf to table
                foreach (var item in listInvoiceInputs)
                {
                    api = "Client/SaveClientInvoiceDetails";
                    var ClientInvoiceDataDetails = JsonConvert.SerializeObject(item);
                    var resultSave = service.PostAPIWithData(ClientInvoiceDataDetails, api);
                    int SaveId = Convert.ToInt32(resultSave.Result);
                }
                //updating next invoice number in table
                api = "Client/UpdateClientInvoiceNumber/" + clientId + "/" + (NextInvoiceNumber + 1);
                var serviceContent = JsonConvert.SerializeObject(clientId);
                HttpStatusCode results = service.PostAPI(serviceContent, api);
            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in aspx client Invoice Report!");
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "script", "<script type='text/javascript'>hideProgress();</script>", false);
                ScriptManager.RegisterClientScriptBlock(this, GetType(), "none", "<script>hideProgress();</script>", false);
            }

        }

        private void RefreshDeafaultInvoiceReportviewr()
        {
            int month = Convert.ToInt32(Request.QueryString["month"]);
            int mode = Convert.ToInt32(Request.QueryString["Mode"]);
            int clientId = Convert.ToInt32(Request.QueryString["clientId"]);

            List<ScheduledData> list = scheduleDetailsListFilterd;
            list = list.OrderBy(x => x.StartDateTime).ThenBy(x => x.TimeIn).ToList();
            list = list.GroupBy(p => p.InvoiceNumber).Select(g => g.First()).OrderBy(x => x.StartDateTime).ThenBy(x => x.TimeIn).ToList();
            List<ScheduledData> initialList = scheduleDetailsListFilterd;
            List<List<ScheduledData>> listOfList = initialList.GroupBy(item => item.InvoiceNumber).Select(group => group.ToList<ScheduledData>()).ToList();
            foreach (var item in listOfList)
            {
                Warning[] warnings;
                string[] streamIds;
                string mimeType = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;

                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/ClientInvoice.rdlc");

                ReportDataSource datasourceNew = new ReportDataSource("ClientInvoice", item);
                if (scheduleDetailsListFilterd.Count != 0)
                {
                    ReportViewer1.Visible = true;
                }
                else
                {
                    lblmessagesplit.Visible = true;

                }
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(datasourceNew);
                ReportParameterCollection reportParameters1 = new ReportParameterCollection();
                reportParameters1.Add(new ReportParameter("Year", year.ToString()));
                if (monthText == "--Select Month--")
                {
                    reportParameters1.Add(new ReportParameter("FromDate", Convert.ToDateTime(searchInputs.FromDate).ToString("dd MMM yyyy")));
                    reportParameters1.Add(new ReportParameter("Todate", Convert.ToDateTime(searchInputs.ToDate).ToString("dd MMM yyyy")));
                    reportParameters1.Add(new ReportParameter("InvoiceDate", Convert.ToDateTime(searchInputs.InvoiceDate).ToString("dd MMM yyyy")));
                    ReportViewer1.LocalReport.DisplayName = "ClientInvoice" + "  " + fromdate.ToString("dd MMM yyyy") + " to " + todate.ToString("dd MMM yyyy");
                }
                else
                {
                    reportParameters1.Add(new ReportParameter("monthText", monthText.ToString()));
                    ReportViewer1.LocalReport.DisplayName = "ClientInvoice" + " " + monthText.ToString() + " " + year.ToString();

                }

                this.ReportViewer1.LocalReport.SetParameters(reportParameters1);
                this.ReportViewer1.LocalReport.Refresh();

                Service service = new Service();
                List<CompanyProfile> companyProfile = new List<CompanyProfile>();
                string apia = "Admin/GetCompanyProfiles/0";
                var results = service.GetAPI(apia);
                CompanyProfile listCompanyProfile = JsonConvert.DeserializeObject<CompanyProfile>(results);
                companyProfile.Add(listCompanyProfile);
                ReportDataSource datasourceCompanyProfile = new ReportDataSource("CompanyProfile", companyProfile);

                // ReportViewer1.LocalReport.DataSources.Add(datasourceCompanyProfile);
                ReportViewer1.LocalReport.DataSources.Add(datasourceCompanyProfile);
                byte[] bytesSeprate = this.ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                string siteUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/";

                string filePath = Server.MapPath("~/PCMS/Invoice/Client/") + item[0].InvoiceNumber + ".pdf";
                {
                    Directory.CreateDirectory(Server.MapPath("~/PCMS/Invoice/Client/"));
                }
                System.IO.File.WriteAllBytes(filePath, bytesSeprate);
                filePath = siteUrl + "PCMS/Invoice/Client/" + item[0].InvoiceNumber + ".pdf";

                DateTime invoiceDate = Convert.ToDateTime(Request.QueryString["invoiceDate"]);

                InvoiceSearchInpts invoicesearchinputs = new InvoiceSearchInpts();
                invoicesearchinputs.InvoiceNumber = item[0].InvoiceNumber;
                invoicesearchinputs.InvoicePrefix = InvoicePrefix;
                invoicesearchinputs.ClientId = clientId;
                invoicesearchinputs.StartDate = fromdate;
                invoicesearchinputs.Category = item[0].ServiceTypeName;
                invoicesearchinputs.EndDate = todate;
                invoicesearchinputs.Mode = mode;
                invoicesearchinputs.Year = year;
                invoicesearchinputs.Month = month;
                invoicesearchinputs.Seperateinvoice = false;
                invoicesearchinputs.Description = item[0].Description;
                invoicesearchinputs.PdfFile = bytesSeprate;
                invoicesearchinputs.PdfFilePath = filePath;
                invoicesearchinputs.InvoiceDate = invoiceDate;
                listInvoiceInputs.Add(invoicesearchinputs);
            }
        }

        public void refreshReportviewr()
        {
            int month = Convert.ToInt32(Request.QueryString["month"]);
            int mode = Convert.ToInt32(Request.QueryString["Mode"]);
            int clientId = Convert.ToInt32(Request.QueryString["clientId"]);

            List<ScheduledData> list = scheduleDetailsListSeperateInvoice;
            list = list.OrderBy(x => x.StartDateTime).ThenBy(x => x.TimeIn).ToList();
            list = list.GroupBy(p => p.InvoiceNumber).Select(g => g.First()).OrderBy(x => x.StartDateTime).ThenBy(x => x.TimeIn).ToList();
            List<ScheduledData> initialList = scheduleDetailsListSeperateInvoice;
            List<List<ScheduledData>> listOfList = initialList.GroupBy(item => item.InvoiceNumber).Select(group => group.ToList<ScheduledData>()).ToList();
            foreach (var item in listOfList)
            {
                Warning[] warnings;
                string[] streamIds;
                string mimeType = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;

                ReportViewer2.ProcessingMode = ProcessingMode.Local;
                ReportViewer2.ShowRefreshButton = false;
                ReportViewer2.LocalReport.ReportPath = Server.MapPath("~/Reports/ClientInvoiceSeperately.rdlc");

                ReportDataSource datasourceNew = new ReportDataSource("ClientInvoice", item);
                if (scheduleDetailsListSeperateInvoice.Count != 0)
                {
                    ReportViewer2.Visible = true;
                }
                else
                {
                    lblmessagesplit.Visible = true;

                }
                ReportViewer2.LocalReport.DataSources.Clear();
                ReportViewer2.LocalReport.DataSources.Add(datasourceNew);
                ReportParameterCollection reportParameters2 = new ReportParameterCollection();
                reportParameters2.Add(new ReportParameter("Year", year.ToString()));
                if (monthText == "--Select Month--")
                {
                    reportParameters2.Add(new ReportParameter("FromDate", Convert.ToDateTime(searchInputs.FromDate).ToString("dd MMM yyyy")));
                    reportParameters2.Add(new ReportParameter("Todate", Convert.ToDateTime(searchInputs.ToDate).ToString("dd MMM yyyy")));
                    reportParameters2.Add(new ReportParameter("InvoiceDate", Convert.ToDateTime(searchInputs.InvoiceDate).ToString("dd MMM yyyy")));
                    ReportViewer2.LocalReport.DisplayName = "ClientInvoice" + "  " + fromdate.ToString("dd MMM yyyy") + " to " + todate.ToString("dd MMM yyyy");
                }
                else
                {
                    reportParameters2.Add(new ReportParameter("monthText", monthText.ToString()));
                    ReportViewer2.LocalReport.DisplayName = "ClientInvoice" + " " + monthText.ToString() + " " + year.ToString();

                }

                this.ReportViewer2.LocalReport.SetParameters(reportParameters2);
                this.ReportViewer2.LocalReport.Refresh();

                Service service = new Service();
                List<CompanyProfile> companyProfile = new List<CompanyProfile>();
                string apia = "Admin/GetCompanyProfiles/0";
                var results = service.GetAPI(apia);
                CompanyProfile listCompanyProfile = JsonConvert.DeserializeObject<CompanyProfile>(results);
                companyProfile.Add(listCompanyProfile);
                ReportDataSource datasourceCompanyProfile = new ReportDataSource("CompanyProfile", companyProfile);

                ReportViewer2.LocalReport.DataSources.Add(datasourceCompanyProfile);
                byte[] bytesSeprate = this.ReportViewer2.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                string siteUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/";
                string filePath = Server.MapPath("~/PCMS/Invoice/Client/") + item[0].InvoiceNumber + ".pdf";
                {
                    Directory.CreateDirectory(Server.MapPath("~/PCMS/Invoice/Client/"));
                }
                System.IO.File.WriteAllBytes(filePath, bytesSeprate);
                filePath = siteUrl + "PCMS/Invoice/Client/" + item[0].InvoiceNumber + ".pdf";
                DateTime invoiceDate = Convert.ToDateTime(Request.QueryString["invoiceDate"]);

                InvoiceSearchInpts invoicesearchinputs = new InvoiceSearchInpts();
                invoicesearchinputs.InvoiceDate = invoiceDate;
                invoicesearchinputs.InvoiceNumber = item[0].InvoiceNumber;
                invoicesearchinputs.InvoicePrefix = InvoicePrefix;
                invoicesearchinputs.ClientId = clientId;
                invoicesearchinputs.StartDate = fromdate;
                invoicesearchinputs.EndDate = todate;
                invoicesearchinputs.Mode = mode;
                invoicesearchinputs.Year = year;
                invoicesearchinputs.Category = item[0].ServiceTypeName;
                invoicesearchinputs.Month = month;
                invoicesearchinputs.Seperateinvoice = true;
                invoicesearchinputs.Description = item[0].Description;
                invoicesearchinputs.PdfFile = bytesSeprate;
                invoicesearchinputs.PdfFilePath = filePath;
                listInvoiceInputs.Add(invoicesearchinputs);
            }
        }

        protected void btnSeperateFiles_Click(object sender, EventArgs e)
        {

            string pdfFilePath = @"C:\PdfFiles\sample.pdf";
            string outputPath = @"C:\SplitedPdfFiles";
            int interval = 10;
            int pageNameSuffix = 0;

            // Intialize a new PdfReader instance with the contents of the source Pdf file:  
            PdfReader reader = new PdfReader(pdfFilePath);

            FileInfo file = new FileInfo(pdfFilePath);
            string pdfFileName = file.Name.Substring(0, file.Name.LastIndexOf(".")) + "-";

            for (int pageNumber = 1; pageNumber <= reader.NumberOfPages; pageNumber += interval)
            {
                pageNameSuffix++;
                string newPdfFileName = string.Format(pdfFileName + "{0}", pageNameSuffix);
                SplitAndSaveInterval(pdfFilePath, outputPath, pageNumber, interval, newPdfFileName);
            }
        }

        private void SplitAndSaveInterval(string pdfFilePath, string outputPath, int startPage, int interval, string pdfFileName)
        {
            using (PdfReader reader = new PdfReader(pdfFilePath))
            {
                Document document = new Document();
                PdfCopy copy = new PdfCopy(document, new FileStream(outputPath + "\\" + pdfFileName + ".pdf", FileMode.Create));

                document.Open();

                for (int pagenumber = startPage; pagenumber < (startPage + interval); pagenumber++)
                {
                    if (reader.NumberOfPages >= pagenumber)
                    {
                        copy.AddPage(copy.GetImportedPage(reader, pagenumber));
                    }
                    else
                    {
                        break;
                    }

                }

                document.Close();
            }
        }

        private void MergePDFs(string outPutFilePath, List<string> filesPath)
        {
            List<PdfReader> readerList = new List<PdfReader>();
            foreach (string filePath in filesPath)
            {
                PdfReader pdfReader = new PdfReader(filePath);
                readerList.Add(pdfReader);
            }

            //Define a new output document and its size, type

            iTextSharp.text.Rectangle pagesize = new iTextSharp.text.Rectangle(612, 892);
            Document document = new Document(PageSize.LETTER, 0, 0, 0, 0);
            //Create blank output pdf file and get the stream to write on it.
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outPutFilePath, FileMode.Create));
            document.Open();

            foreach (PdfReader reader in readerList)
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    PdfImportedPage page = writer.GetImportedPage(reader, i);
                    document.Add(iTextSharp.text.Image.GetInstance(page));
                }
            }
            document.Close();
        }

        protected void Btnsend_Click(object sender, EventArgs e)
        {
            ClientModel clientModelObj = new ClientModel();
            Service service = new Service();
            int clientId = Convert.ToInt32(Request.QueryString["clientId"]);
            string api = "Client/GetAllClientDetailsById?clientId=" + clientId;
            var result = service.GetAPI(api);
            clientModelObj = JsonConvert.DeserializeObject<ClientModel>(result);
            EmailInput emailinputs = new EmailInput
            {
                EmailType = Enums.EmailType.Invoice,
                Body = GetEmailBody(clientModelObj),
                Subject = "Invoice ",
                EmailId = emailTxt.Text,
                Attachments = Server.MapPath("~/PCMS/Invoice/Client/") + clientModelObj.ClientName + ".pdf",
                UserId = clientId
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

        private string GetEmailBody(ClientModel clientModelObj)
        {
            try
            {
                string WelcomeMsg = "Client Invoice.";
                string MailMsg = "Invoice Details.<br/>";
                string Mailcontent = "Thank you for choosing to work with us.<br/> We are attaching the copy of the invoice.";
                string ContactNo = "1-800-892-6066";
                string RegardsBy = "Tranquil Care Inc.";
                string siteUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/";
                string CompanyName_TagLine = "Tranquil Care - Delivering Care Excellence.";
                string CompanyName = "Tranquil Care Inc.";
                string body = "";
                string sd = AppDomain.CurrentDomain.BaseDirectory + "EmailTemplates/EmailCommon.html";
                var sr = new StreamReader(sd);
                body = sr.ReadToEnd();
                body = string.Format(body, WelcomeMsg, clientModelObj.ClientName, MailMsg, Mailcontent, ContactNo, RegardsBy, siteUrl, CompanyName_TagLine, CompanyName);
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
