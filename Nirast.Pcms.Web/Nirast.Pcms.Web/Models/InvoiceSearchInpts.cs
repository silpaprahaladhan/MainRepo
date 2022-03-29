using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class InvoiceSearchInpts
    {
        public int InvoiceSearchInputId { get; set; }
        public int InvoiceNumber { get; set; }
        public int BookingId { get; set; }
        public string InvoicePrefix { get; set; }
        public int ClientId { get; set; }
        public int PublicUserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Mode { get; set; }    
        public int? Year { get; set; }
        public int? Month { get; set; }
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public bool Seperateinvoice { get; set; }
        public string Description { get; set; }
       public byte[] PdfFile { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string Category { get; set; }
        public string Service { get; set; }
        public string PdfFilePath { get; set; }
        public float TotalPayingAmount { get; set; }
    }
}