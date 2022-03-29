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
    public partial class ClientInvoiceReport : System.Web.UI.Page
    {
        PCMSLogger pCMSLogger = new PCMSLogger();
        List<string> myFiles = new List<string>();
        List<InvoiceSearchInpts> listInvoiceInputs = new List<InvoiceSearchInpts>();
        DateTime fromdate = DateTime.MinValue, todate = DateTime.MinValue;
        string monthText;
        PaymentAdvancedSearch searchInputs = new PaymentAdvancedSearch();
        int year;
        int NextInvoiceNumber,InvoiceNumber;
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

                    string script = "$(document).ready(function () { $('[id*=btnSubmit]').click(); });";
                    ClientScript.RegisterStartupScript(this.GetType(), "load", script, true);

                    ReportViewer1.ProcessingMode = ProcessingMode.Local;
                    ReportViewer1.ShowRefreshButton = false;
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/ClientInvoice.rdlc");
                    ReportViewer1.Visible = false;
                    ReportViewer2.Visible = false;

                    PaymentAdvancedSearch searchInputs = new PaymentAdvancedSearch();
                    List<ScheduledData> scheduleDetailsList = new List<ScheduledData>();
                    DateTime fromdate = DateTime.MinValue, todate = DateTime.MinValue;
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
                    searchInputs.ToDate   = todate;
                    searchInputs.Category = category;
                    if (month == 0)
                    {
                        searchInputs.Year = 0;
                    }
                    else
                    {
                        searchInputs.Year = 0;
                    }
                    searchInputs.Month = 0;
                    if(year !=0 &&  month !=0)
                    {
                        searchInputs.FromDate = new DateTime(year, month, 1);   //new DateTime(year, month, 1);
                        searchInputs.ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

                    }
                    if(year !=0 && month ==0 && fromdate == DateTime.MinValue)
                    {
                        searchInputs.FromDate = new DateTime(year, 1, 1);
                        searchInputs.ToDate  = new DateTime(year, 12, 31);
                    }


                    searchInputs.IsOrientation = isOrientation;
                    Service service = new Service();
                    string api = "Home/GetClientScheduledDetails";
                    var advancedSearchInputModel = JsonConvert.SerializeObject(searchInputs);
                    var result = service.PostAPIWithData(advancedSearchInputModel, api);
                    scheduleDetailsList = JsonConvert.DeserializeObject<List<ScheduledData>>(result.Result);

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
                        scheduleDetailsList = scheduleDetailsList.Where(a => Convert.ToDateTime(a.Startdate).Date <= Convert.ToDateTime(searchInputs.ToDate).Date).Where(a => Convert.ToDateTime(a.Startdate).Date >= Convert.ToDateTime(searchInputs.FromDate).Date).ToList();
                    }

                    List<ScheduledData> holidaySchedules = new List<ScheduledData>();
                    holidaySchedules = scheduleDetailsList.Where(x => x.HoildayHours != 0).ToList();


                    scheduleDetailsList = scheduleDetailsList.Where(x => x.HoildayHours == 0).GroupBy(l => l.SchedulingId)
                                                         .Select(cl =>  new ScheduledData
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
                                                             Hours = cl.Sum(k=>Convert.ToDecimal(k.Hours)).ToString(),
                                                             Total = cl.Sum(k => k.Total),
                                                             HST = cl.Sum(k => k.HST),
                                                             InvoiceNumber=cl.First().InvoiceNumber,
                                                             InvoicePrefix= cl.First().InvoicePrefix 
                                                         }).ToList();

                    scheduleDetailsList.AddRange(holidaySchedules);


                    List<ScheduledData> scheduleDetailsListFilterd = new List<ScheduledData>();
                    scheduleDetailsListFilterd = scheduleDetailsList.Where(a => a.IsSeparateInvoice == false).ToList();
                    List<ScheduledData> list1 = scheduleDetailsListFilterd;
                    list1 = list1.OrderBy(x => x.StartDateTime).ThenBy(x => x.TimeIn).ToList();
                    list1 = list1.GroupBy(p => p.ServiceTypeName).Select(g => g.First()).OrderBy(x => x.StartDateTime).ThenBy(x => x.TimeIn).ToList();
                    list1 = list1.OrderBy(x => x.ServiceTypeName).ToList();

                    for (int i = 0; i < list1.Count(); i++)
                    {
                        if (scheduleDetailsListFilterd.Count > 0)
                        {
                            foreach (var item in scheduleDetailsListFilterd.Where(w => w.ServiceTypeName == list1[i].ServiceTypeName))
                            {
                                item.InvoiceNumber = item.InvoiceNumber + i;
                                InvoiceNumber = item.InvoiceNumber;
                            }
                        }
                        else
                        {
                            foreach (var item in scheduleDetailsListFilterd.Where(w => w.ServiceTypeName == list1[i].ServiceTypeName))
                            {
                                item.InvoiceNumber = item.InvoiceNumber + i;
                                InvoiceNumber = item.InvoiceNumber;
                            }
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
                    reportParameters.Add(new ReportParameter("InvoiceDate", Convert.ToDateTime(DateTime.Now).ToString("dd MMM yyyy")));
                    reportParameters.Add(new ReportParameter("InvoiceAddress", invoiceAddress.ToString()));
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


                    ReportViewer2.ProcessingMode = ProcessingMode.Local;
                    ReportViewer2.ShowRefreshButton = false;
                    ReportViewer2.LocalReport.ReportPath = Server.MapPath("~/Reports/ClientInvoiceSeperately.rdlc");
                   // List<ScheduledData> scheduleDetailsListSeperateInvoice = new List<ScheduledData>();
                    scheduleDetailsListSeperateInvoice = scheduleDetailsList.Where(a => a.IsSeparateInvoice == true).ToList();

                    List<ScheduledData> list = scheduleDetailsListSeperateInvoice;
                    list = list.OrderBy(x => x.StartDateTime).ThenBy(x => x.TimeIn).ToList();
                    list = list.GroupBy(p => new { p.Description, p.ServiceTypeName }).Select(g => g.First()).OrderBy(x => x.StartDateTime).ThenBy(x => x.TimeIn).ToList();

                    for (int i = 0; i < list.Count(); i++)
                    {
                        if (scheduleDetailsListSeperateInvoice.Count > 0)
                        {
                            foreach (var item in scheduleDetailsListSeperateInvoice.Where(w => w.Description == list[i].Description && w.ServiceTypeName == list[i].ServiceTypeName))
                            {
                                item.InvoiceNumber = InvoiceNumber + 1 + i;
                                NextInvoiceNumber = item.InvoiceNumber;
                            }
                        }
                        else
                        {
                            foreach (var item in scheduleDetailsListSeperateInvoice.Where(w => w.Description == list[i].Description && w.ServiceTypeName == list[i].ServiceTypeName))
                            {
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
                    reportParameters2.Add(new ReportParameter("InvoiceDate", Convert.ToDateTime(DateTime.Now).ToString("dd MMM yyyy")));
                    reportParameters2.Add(new ReportParameter("Year", year.ToString()));
                    reportParameters.Add(new ReportParameter("InvoiceAddress", invoiceAddress.ToString()));
                    if (monthText == "--Select Month--" || monthText == null)
                    {
             
                        reportParameters2.Add(new ReportParameter("FromDate", Convert.ToDateTime(searchInputs.FromDate).ToString("dd MMM yyyy")));
                        reportParameters2.Add(new ReportParameter("Todate", Convert.ToDateTime(searchInputs.ToDate).ToString("dd MMM yyyy")));
                       
                        //ReportViewer2.LocalReport.DisplayName = "ClientInvoice" + "  " + fromdate.ToString("dd MMM yyyy") + " to " + todate.ToString("dd MMM yyyy");
                        ReportViewer2.LocalReport.DisplayName = "ClientInvoice" + "  " + Convert.ToDateTime(searchInputs.FromDate).ToString("dd MMM yyyy") + " to " + Convert.ToDateTime(searchInputs.ToDate).ToString("dd MMM yyyy");

                    }
                    else
                    {
                        reportParameters2.Add(new ReportParameter("monthText", monthText.ToString()));
                        ReportViewer2.LocalReport.DisplayName = "ClientInvoice" + " " + monthText.ToString() + " " + year.ToString();

                    }

                    this.ReportViewer2.LocalReport.SetParameters(reportParameters2);
                    this.ReportViewer2.LocalReport.Refresh();

                    //foreach (var item in scheduleDetailsListSeperateInvoice)
                    //{
                    //    List<ScheduledData> test = new List<ScheduledData>();
                    //    test.Add(item);
                    //    ReportDataSource datasourceNew = new ReportDataSource("ClientInvoice", test);
                    //    this.ReportViewer1.LocalReport.DataSources.Add(datasourceNew);
                    //}
                    List<CompanyProfile> companyProfile = new List<CompanyProfile>();
                    string apia = "Admin/GetCompanyProfiles/0";
                    var results = service.GetAPI(apia);
                    CompanyProfile listCompanyProfile = JsonConvert.DeserializeObject<CompanyProfile>(results);
                    companyProfile.Add(listCompanyProfile);
                    ReportDataSource datasourceCompanyProfile = new ReportDataSource("CompanyProfile", companyProfile);

                    ReportViewer1.LocalReport.DataSources.Add(datasourceCompanyProfile);
                    ReportViewer2.LocalReport.DataSources.Add(datasourceCompanyProfile);

                    

                }
                catch (Exception ex)
                {
                    pCMSLogger.Error(ex, "Error occurred in Admin Controller-ClientInvoiceReport.cs");
                }

            }

        }


        public void refreshReportviewr()
        {
            int month = Convert.ToInt32(Request.QueryString["month"]);
            int mode = Convert.ToInt32(Request.QueryString["Mode"]);

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
                reportParameters2.Add(new ReportParameter("InvoiceAddress", invoiceAddress.ToString()));
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

                Service service = new Service();
                List<CompanyProfile> companyProfile = new List<CompanyProfile>();
                string apia = "Admin/GetCompanyProfiles/0";
                var results = service.GetAPI(apia);
                CompanyProfile listCompanyProfile = JsonConvert.DeserializeObject<CompanyProfile>(results);
                companyProfile.Add(listCompanyProfile);
                ReportDataSource datasourceCompanyProfile = new ReportDataSource("CompanyProfile", companyProfile);

               // ReportViewer1.LocalReport.DataSources.Add(datasourceCompanyProfile);
                ReportViewer2.LocalReport.DataSources.Add(datasourceCompanyProfile);
                   byte[] bytesSeprate = this.ReportViewer2.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                //InvoiceSearchInpts invoicesearchinputs = new InvoiceSearchInpts();
                //invoicesearchinputs.InvoiceNumber = item[0].InvoiceNumber;
                //invoicesearchinputs.InvoicePrefix = InvoicePrefix + " " + Convert.ToString(item[0].InvoiceNumber - 1);
                //invoicesearchinputs.ClientId = item[0].ClientId;
                //invoicesearchinputs.StartDate = fromdate;
                //invoicesearchinputs.EndDate = todate;
                //invoicesearchinputs.Mode = mode;
                //invoicesearchinputs.Year = year;
                //invoicesearchinputs.Month = month;
                //invoicesearchinputs.Seperateinvoice = true;
                //invoicesearchinputs.Description = item[0].Description;
                //invoicesearchinputs.PdfFile = bytesSeprate;
                //listInvoiceInputs.Add(invoicesearchinputs);
            }
        }

        public void ExportToPdf(DataTable dt)
        {
            Document document = new Document();
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream("F://sample.pdf", FileMode.Create));
            document.Open();
            iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 5);

            PdfPTable table = new PdfPTable(dt.Columns.Count);
            PdfPRow row = null;
            float[] widths = new float[] { 4f, 4f, 4f, 4f };

            table.SetWidths(widths);

            table.WidthPercentage = 100;
            int iCol = 0;
            string colname = "";
            PdfPCell cell = new PdfPCell(new Phrase("Products"));

            cell.Colspan = dt.Columns.Count;

            foreach (DataColumn c in dt.Columns)
            {

                table.AddCell(new Phrase(c.ColumnName, font5));
            }

            foreach (DataRow r in dt.Rows)
            {
                if (dt.Rows.Count > 0)
                {
                    table.AddCell(new Phrase(r[0].ToString(), font5));
                    table.AddCell(new Phrase(r[1].ToString(), font5));
                    table.AddCell(new Phrase(r[2].ToString(), font5));
                    table.AddCell(new Phrase(r[3].ToString(), font5));
                }
            }
            document.Add(table);
            document.Close();
        }

        public class ListtoDataTable
        {
            public DataTable ToDataTable<T>(List<T> items)
            {
                DataTable dataTable = new DataTable(typeof(T).Name);
                //Get all the properties by using reflection   
                PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo prop in Props)
                {
                    //Setting column names as Property names  
                    dataTable.Columns.Add(prop.Name);
                }
                foreach (T item in items)
                {
                    var values = new object[Props.Length];
                    for (int i = 0; i < Props.Length; i++)
                    {

                        values[i] = Props[i].GetValue(item, null);
                    }
                    dataTable.Rows.Add(values);
                }

                return dataTable;
            }
        }

        //generate invoice buttuon click
        protected void Button2_Click(object sender, EventArgs e)
        {
            #region pdfgeneration

            //try
            //{


            //    ClientModel clientModelObj = new ClientModel();
            //    Service service = new Service();
            //    int clientId = Convert.ToInt32(Request.QueryString["clientId"]);
            //    string api = "Client/GetAllClientDetailsById?clientId=" + clientId;
            //    var result = service.GetAPI(api);
            //    clientModelObj = JsonConvert.DeserializeObject<ClientModel>(result);

            //    string InvoicePrefix = clientModelObj.InvoicePrefix;
            //    int InvoiceNumber = clientModelObj.InvoiceNumber;

            //    DateTime fromdate = DateTime.MinValue, todate = DateTime.MinValue;
            //    string monthText = (Request.QueryString["monthText"]);
            //    if (Request.QueryString["fromdate"] != "")
            //    {
            //        fromdate = Convert.ToDateTime(Request.QueryString["fromdate"]);
            //    }
            //    if (Request.QueryString["todate"] != "")
            //    {
            //        todate = Convert.ToDateTime(Request.QueryString["todate"]);
            //    }
            //    int year = Convert.ToInt32(Request.QueryString["year"]);
            //    // Variables
            //    Warning[] warnings;
            //    string[] streamIds;
            //    string mimeType = string.Empty;
            //    string encoding = string.Empty;
            //    string extension = string.Empty;
            //    iTextSharp.text.Image image;
            //    string filennameNew = string.Empty;
            //    string filenname = string.Empty;
            //    iTextSharp.text.Font blackFont = iTextSharp.text.FontFactory.GetFont("Tohama", 12, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
            //    string randomName = "Client Invoice";
            //    List<ScheduledData> listFloor = ReportViewer1.LocalReport.DataSources[0].Value as List<ScheduledData>;
            //    List<ScheduledData> listSeperate = ReportViewer2.LocalReport.DataSources[0].Value as List<ScheduledData>;

            //    string WatermarkLocation = Server.MapPath("~/images/") + "logo_Cut.jpg";
            //    iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(WatermarkLocation);
            //    img.SetAbsolutePosition(750, 100);
            //    img.ScaleAbsolute(320f,255.25f);


            //    if (listFloor.Count > 0)
            //    {
            //        //reading sperate invoice =false report viewer and adding invoice number  dynamically and storing to temp folder
            //        byte[] bytes = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);                    
            //        using (MemoryStream stream = new MemoryStream())
            //        {
            //            PdfReader reader = new PdfReader(bytes);
            //            PdfStamper stamper = new PdfStamper(reader, stream);
            //            int pages = reader.NumberOfPages;
            //            PdfContentByte waterMark;
            //            for (int i = 1; i <= pages; i++)
            //            {
            //                if (i == 1)
            //                {
            //                    ColumnText.ShowTextAligned(stamper.GetUnderContent(i), iTextSharp.text.Element.ALIGN_RIGHT, new iTextSharp.text.Phrase('#' + InvoicePrefix + '-' + InvoiceNumber.ToString(), blackFont), 1042f, 2658f, 0);
            //                    PdfContentByte pbunder = stamper.GetUnderContent(i);
            //                    InvoiceNumber++;
            //                }


            //                waterMark = stamper.GetUnderContent(i);
            //                waterMark.AddImage(img);
            //            }
            //            stamper.Close();
            //            bytes = stream.ToArray();
            //        }                    
            //        if (monthText == "--Select Month--")
            //        {
            //            randomName = "ClientInvoice for " + clientModelObj.ClientName + "  " + fromdate.ToString("dd MMM yyyy") + " to " + todate.ToString("dd MMM yyyy") + " " + DateTime.Now.Ticks.ToString();
            //        }
            //        else
            //        {
            //            randomName = "ClientInvoice for " + clientModelObj.ClientName + " " + monthText.ToString() + " " + year.ToString() + " " + DateTime.Now.Ticks.ToString();
            //        }
            //         filenname = Server.MapPath("~/Temp/") + randomName + ".pdf";
            //        string returnFileName = randomName + ".pdf";
            //        System.IO.File.WriteAllBytes(filenname, bytes);
            //        //return returnFileName;
            //    }
            //    if (listSeperate.Count > 0)
            //    {
            //       //reading sperate invoice=true report viewer and adding invoice number and invoice for (residents name) dynamically and storing to temp folder
            //        byte[] bytesSeprate = ReportViewer2.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
            //        using (MemoryStream stream = new MemoryStream())
            //        {
            //            PdfReader reader = new PdfReader(bytesSeprate);
            //            PdfStamper stamper = new PdfStamper(reader, stream);
            //            int pages = reader.NumberOfPages;
            //            PdfContentByte waterMark;
            //            for (int i = 1; i <= pages; i++)
            //            {
            //                List<ScheduledData> list = ReportViewer2.LocalReport.DataSources[0].Value as List<ScheduledData>;
            //                list = list.GroupBy(p => p.Description).Select(g => g.First()).OrderBy(x => x.StartDateTime).ThenBy(x=>x.TimeIn). ToList();
            //                string DataInvoicefor = list[i - 1].Description;
            //                string WorkShiftname = list[i - 1].WorkModeName;
            //                ColumnText.ShowTextAligned(stamper.GetUnderContent(i), iTextSharp.text.Element.ALIGN_RIGHT, new iTextSharp.text.Phrase('#' + InvoicePrefix + '-' + InvoiceNumber.ToString(), blackFont), 1067f, 2664f, 0);
            //                PdfContentByte pbunder = stamper.GetUnderContent(i);
            //                string pdftext = PdfTextExtractor.GetTextFromPage(reader, i);
            //                string[] words = pdftext.Split('\n');
            //                string[] tableFirstRow = words[11].Split(' ');
            //                string InvoiceFor = tableFirstRow[0];
            //                ColumnText.ShowTextAligned(stamper.GetUnderContent(i), iTextSharp.text.Element.ALIGN_RIGHT, new iTextSharp.text.Phrase(WorkShiftname,  blackFont), 753f, 2552f, 0);
            //                ColumnText.ShowTextAligned(stamper.GetUnderContent(i), iTextSharp.text.Element.ALIGN_RIGHT, new iTextSharp.text.Phrase(DataInvoicefor, blackFont), 754f, 2570f, 0);
            //                PdfContentByte pbunderInvoiceFor = stamper.GetUnderContent(i);
            //                waterMark = stamper.GetUnderContent(i);
            //                waterMark.AddImage(img);
            //                InvoiceNumber++;
            //            }
            //            stamper.Close();
            //            bytesSeprate = stream.ToArray();
            //        }
            //        string randomNameNew = "";
            //        if (monthText == "--Select Month--")
            //        {
            //            randomNameNew = "ClientInvoice for " + clientModelObj.ClientName + "  " + fromdate.ToString("dd MMM yyyy") + " to " + todate.ToString("dd MMM yyyy") + " " + DateTime.Now.Ticks.ToString();
            //        }
            //        else
            //        {
            //            randomNameNew = "ClientInvoice for " + clientModelObj.ClientName + " " + monthText.ToString() + " " + year.ToString() + " " + DateTime.Now.Ticks.ToString();
            //        }
            //         filennameNew = Server.MapPath("~/Temp/") + randomNameNew + " SeperateInvoice.pdf";
            //        System.IO.File.WriteAllBytes(filennameNew, bytesSeprate);
            //    }
            //    api = "Client/UpdateClientInvoiceNumber/" + clientId + "/" + InvoiceNumber;
            //    var serviceContent = JsonConvert.SerializeObject(clientId);
            //    HttpStatusCode results = service.PostAPI(serviceContent, api);

            //    //dowload to client browser
            //    // Now that you have all the bytes representing the PDF report, buffer it and send it to the client.
            //    //Response.Buffer = true;
            //    //Response.Clear();
            //    //Response.ContentType = mimeType;
            //    //Response.AddHeader("content-disposition", "attachment; filename=ClientInvoice." + extension);
            //    //Response.BinaryWrite(bytes);            
            //    //Response.Flush(); // send it to the client to download

            //    //add created file to list and create a list 
            //    List<string> files = new List<string>();
            //    if (filenname != "")
            //    {
            //        files.Add(filenname);
            //    }
            //    if (filennameNew != "")
            //    {
            //        files.Add(filennameNew);
            //    }
            //    //zip the files in list
            //    if(files.Count>0)
            //    {
            //        using (ZipFile zip = new ZipFile())
            //        {
            //            zip.AlternateEncodingUsage = ZipOption.AsNecessary;
            //            zip.AddDirectoryByName(randomName);
            //            foreach (string fl in files)
            //            {

            //                string filePath = fl;
            //                zip.AddFile(filePath, randomName);

            //            }
            //            Response.Clear();
            //            Response.BufferOutput = true;
            //            string zipName = String.Format("Zip_{0}.zip", DateTime.Now.Ticks.ToString());
            //            Response.ContentType = "application/zip";
            //            Response.AddHeader("content-disposition", "attachment; filename=" + zipName);
            //            zip.Save(Response.OutputStream);
            //            Response.End();
            //        }
            //    }


            //}
            //catch (Exception ex)
            //{
            //    pCMSLogger.Error(ex, "Error occurred in aspx client Invoice Report!");

            //}
            #endregion

            //System.Threading.Thread.Sleep(5000);
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

                string InvoicePrefix = clientModelObj.InvoicePrefix;
                int InvoiceNumber = clientModelObj.InvoiceNumber;

                DateTime fromdate = DateTime.MinValue, todate = DateTime.MinValue;
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
                string filennameNew = string.Empty;
                string filenname = string.Empty;
                iTextSharp.text.Font blackFont = iTextSharp.text.FontFactory.GetFont("Tohama", 12, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
                string randomName = "Client Invoice";
                List<ScheduledData> listFloor = ReportViewer1.LocalReport.DataSources[0].Value as List<ScheduledData>;
                List<ScheduledData> listSeperate = ReportViewer2.LocalReport.DataSources[0].Value as List<ScheduledData>;

                string WatermarkLocation = Server.MapPath("~/images/") + "logo_Cut.jpg";
                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(WatermarkLocation);
                img.SetAbsolutePosition(750, 100);
                img.ScaleAbsolute(320f, 255.25f);


                if (listFloor.Count > 0)
                {
                    //reading sperate invoice =false report viewer and adding invoice number  dynamically and storing to temp folder
                    byte[] bytes = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        PdfReader reader = new PdfReader(bytes);
                        PdfStamper stamper = new PdfStamper(reader, stream);
                        int pages = reader.NumberOfPages;
                        PdfContentByte waterMark;
                        for (int i = 1; i <= pages; i++)
                        {
                            if (i == 1)
                            {
                                ColumnText.ShowTextAligned(stamper.GetUnderContent(i), iTextSharp.text.Element.ALIGN_RIGHT, new iTextSharp.text.Phrase('#' + InvoicePrefix + '-' + InvoiceNumber.ToString(), blackFont), 1042f, 2658f, 0);
                                PdfContentByte pbunder = stamper.GetUnderContent(i);
                                InvoiceNumber++;
                            }


                            waterMark = stamper.GetUnderContent(i);
                            waterMark.AddImage(img);
                        }
                        stamper.Close();
                        bytes = stream.ToArray();
                    }
                    if (monthText == "--Select Month--")
                    {
                        randomName = "ClientInvoice for " + clientModelObj.ClientName + "  " + fromdate.ToString("dd MMM yyyy") + " to " + todate.ToString("dd MMM yyyy") + " " + DateTime.Now.Ticks.ToString();
                    }
                    else
                    {
                        randomName = "ClientInvoice for " + clientModelObj.ClientName + " " + monthText.ToString() + " " + year.ToString() + " " + DateTime.Now.Ticks.ToString();
                    }
                    filenname = Server.MapPath("~/Temp/") + randomName + ".pdf";
                    string returnFileName = randomName + ".pdf";
                    System.IO.File.WriteAllBytes(filenname, bytes);

                    myFiles.Add(filenname);
                    //return returnFileName;
                }
                if (listSeperate.Count > 0)
                {
                    //reading sperate invoice=true report viewer and adding invoice number and invoice for (residents name) dynamically and storing to temp folder
                    byte[] bytesSeprate = ReportViewer2.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        PdfReader reader = new PdfReader(bytesSeprate);
                        PdfStamper stamper = new PdfStamper(reader, stream);
                        int pages = reader.NumberOfPages;
                        PdfContentByte waterMark;
                        for (int i = 1; i <= pages; i++)
                        {
                            List<ScheduledData> list = ReportViewer2.LocalReport.DataSources[0].Value as List<ScheduledData>;
                            list = list.OrderBy(x => x.StartDateTime).ThenBy(x => x.TimeIn).ToList();
                            list = list.GroupBy(p => p.Description).Select(g => g.First()).OrderBy(x => x.StartDateTime).ThenBy(x => x.TimeIn).ToList();
                            string DataInvoicefor = list[i - 1].Description;
                            string WorkShiftname = list[i - 1].WorkModeName;
                            ColumnText.ShowTextAligned(stamper.GetUnderContent(i), iTextSharp.text.Element.ALIGN_RIGHT, new iTextSharp.text.Phrase('#' + InvoicePrefix + '-' + InvoiceNumber.ToString(), blackFont), 1067f, 2664f, 0);
                            PdfContentByte pbunder = stamper.GetUnderContent(i);
                            string pdftext = PdfTextExtractor.GetTextFromPage(reader, i);
                            string[] words = pdftext.Split('\n');
                            string[] tableFirstRow = words[11].Split(' ');
                            string InvoiceFor = tableFirstRow[0];
                            ColumnText.ShowTextAligned(stamper.GetUnderContent(i), iTextSharp.text.Element.ALIGN_RIGHT, new iTextSharp.text.Phrase(WorkShiftname, blackFont), 753f, 2552f, 0);
                            ColumnText.ShowTextAligned(stamper.GetUnderContent(i), iTextSharp.text.Element.ALIGN_RIGHT, new iTextSharp.text.Phrase(DataInvoicefor, blackFont), 754f, 2570f, 0);
                            PdfContentByte pbunderInvoiceFor = stamper.GetUnderContent(i);
                            waterMark = stamper.GetUnderContent(i);
                            waterMark.AddImage(img);
                            InvoiceNumber++;
                        }
                        stamper.Close();
                        bytesSeprate = stream.ToArray();
                    }
                    string randomNameNew = "";
                    if (monthText == "--Select Month--")
                    {
                        randomNameNew = "ClientInvoice for " + clientModelObj.ClientName + "  " + fromdate.ToString("dd MMM yyyy") + " to " + todate.ToString("dd MMM yyyy") + " " + DateTime.Now.Ticks.ToString();
                    }
                    else
                    {
                        randomNameNew = "ClientInvoice for " + clientModelObj.ClientName + " " + monthText.ToString() + " " + year.ToString() + " " + DateTime.Now.Ticks.ToString();
                    }
                    filennameNew = Server.MapPath("~/Temp/") + randomNameNew + " SeperateInvoice.pdf";
                    string outputPath = Server.MapPath("~/Temp/");
                    System.IO.File.WriteAllBytes(filennameNew, bytesSeprate);

                    //split multiple page pdf in to seperate file
                    int interval = 1;
                    int pageNameSuffix = 0;
                    PdfReader readerNew = new PdfReader(filennameNew);
                    FileInfo file = new FileInfo(filennameNew);
                    string pdfFileName = file.Name.Substring(0, file.Name.LastIndexOf(".")) + "-";
                    for (int pageNumber = 1; pageNumber <= readerNew.NumberOfPages; pageNumber += interval)
                    {
                        pageNameSuffix++;
                        string newPdfFileName = string.Format(pdfFileName + "{0}", pageNameSuffix);

                        SplitAndSaveInterval(filennameNew, outputPath, pageNumber, interval, newPdfFileName);
                        myFiles.Add(outputPath + newPdfFileName + ".pdf");

                    }
                    //split end

                    //merge
                    string mergedFile = Server.MapPath("~/Temp/") + "Merged.pdf";
                    MergePDFs(mergedFile, myFiles);
                    //

                }
                api = "Client/UpdateClientInvoiceNumber/" + clientId + "/" + InvoiceNumber;
                var serviceContent = JsonConvert.SerializeObject(clientId);
                HttpStatusCode results = service.PostAPI(serviceContent, api);

                //dowload to client browser
                // Now that you have all the bytes representing the PDF report, buffer it and send it to the client.
                //Response.Buffer = true;
                //Response.Clear();
                //Response.ContentType = mimeType;
                //Response.AddHeader("content-disposition", "attachment; filename=ClientInvoice." + extension);
                //Response.BinaryWrite(bytes);            
                //Response.Flush(); // send it to the client to download

                //add created file to list and create a list 
                List<string> files = new List<string>();
                if (filenname != "")
                {
                    files.Add(filenname);
                }
                if (filennameNew != "")
                {
                    files.Add(filennameNew);
                }
                //zip the files in list
                if (files.Count > 0)
                {
                    using (ZipFile zip = new ZipFile())
                    {
                        zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                        zip.AddDirectoryByName(randomName);
                        foreach (string fl in files)
                        {

                            string filePath = fl;
                            zip.AddFile(filePath, randomName);

                        }
                        Response.Clear();
                        Response.BufferOutput = true;
                        string zipName = String.Format("Zip_{0}.zip", DateTime.Now.Ticks.ToString());
                        Response.ContentType = "application/zip";
                        Response.AddHeader("content-disposition", "attachment; filename=" + zipName);
                        zip.Save(Response.OutputStream);
                        Response.End();
                    }
                }


            }
            catch (Exception ex)
            {
                pCMSLogger.Error(ex, "Error occurred in aspx client Invoice Report!");
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "script", "<script type='text/javascript'>hideProgress();</script>", false);
                ScriptManager.RegisterClientScriptBlock(this, GetType(), "none", "<script>hideProgress();</script>", false);
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

            iTextSharp.text.Rectangle pagesize = new iTextSharp.text.Rectangle(2160, 2880);
            Document document = new Document(pagesize);
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
    }

}