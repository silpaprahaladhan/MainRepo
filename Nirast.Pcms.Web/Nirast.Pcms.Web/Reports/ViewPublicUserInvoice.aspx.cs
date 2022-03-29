using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Nirast.Pcms.Web.Reports
{
    public partial class ViewPublicUserInvoice : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //    ValidationSettings.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
            //    try
            //    {
            //        Service service = new Service();
            //        int fileId = Convert.ToInt32(Request.QueryString["fileId"]);
            //        InvoiceSearchInpts scheduleDetailsListFilterd = new InvoiceSearchInpts();
            //        string api = "Client/GetInvoiceHistoryById/" + fileId;
            //        var result = service.GetAPI(api);
            //        scheduleDetailsListFilterd = JsonConvert.DeserializeObject<List<InvoiceSearchInpts>>(result).ToList().FirstOrDefault();
            //        if (scheduleDetailsListFilterd != null)
            //        {
            //            //byte[] bytes = scheduleDetailsListFilterd.PdfFile;
            //            //string filenname = Server.MapPath("~/PCMS/Invoice/Client/") + "Invoice For " + scheduleDetailsListFilterd.ClientName +"_" + scheduleDetailsListFilterd.InvoicePrefix + ".pdf";
            //            //if (!Directory.Exists(Server.MapPath("~/PCMS/Invoice/Client/")))
            //            //{
            //            //    Directory.CreateDirectory(Server.MapPath("~/PCMS/Invoice/Client/"));
            //            //}
            //            //System.IO.File.WriteAllBytes(filenname, bytes);
            //            //ifrmpdfshow.Src = "~/PCMS/Invoice/Client/" + "Invoice For " + scheduleDetailsListFilterd.ClientName + "_" + scheduleDetailsListFilterd.InvoicePrefix + ".pdf"; 
            //            ifrmpdfshow.Src = scheduleDetailsListFilterd.PdfFilePath;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        pCMSLogger.Error(ex, "Error occurred in ShowClientInvoiceReport-Page_Load");
            //    }
            //}

        }
    }
}
