using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class MakePaymentViewModel : PaymentViewModel
    {
        public string CareTakerName { get; set; }
        public string ServiceFromDate { get; set; }

        public string ServiceFromTime { get; set; }

        public string ServiceToDate { get; set; }

        public string ServiceToTime { get; set; }
        public float ServiceRate { get; set; }
        public string ServiceName { get; set; }
        public string Address { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string ZipCode { get; set; }
        public string PhoneNo { get; set; }
        public string CardType { get; set; }
        public string CardHolderName { get; set; }

        public string CardNo { get; set; }

        public int CardExpiryMonth { get; set; }

        public int CardExpiryYear { get; set; }
        public bool IsSameAddress { get; set; }

    }
}