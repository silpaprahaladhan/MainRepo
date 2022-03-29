using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class InvoicePayriseModel
    {
        public int InvoicePayriseId { get; set; }
        public DateTime? EffectiveFromDate { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public int CaretakerId { get; set; }
        public string CaretakerName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public float Rate { get; set; }
        public string Currency { get; set; }
        public string CurrencySymbol { get; set; }
    }
}
