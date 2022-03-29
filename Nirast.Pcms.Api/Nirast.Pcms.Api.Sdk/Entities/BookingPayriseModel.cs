using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Entities
{
   public class BookingPayriseModel
    {
        public int BookingPayriseId { get; set; }
        public DateTime? EffectiveFromDate { get; set; }
        public int CaretakerId { get; set; }
        public string CaretakerName { get; set; }
        public int ServiceId { get; set; }
        public string Service { get; set; }
        public float DisplayRate { get; set; }
        public float PayingRate { get; set; }
        public string Currency { get; set; }
        public string CurrencySymbol { get; set; }

    }
}
