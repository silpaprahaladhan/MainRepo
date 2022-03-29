using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class CustomerPaymentModel
    {
        /// <summary>
        /// Get or Set payment id
        /// </summary>
        public int PaymentId { get; set; }

        /// <summary>
        /// Get or Set payment date
        /// </summary>
        public DateTime PaymentDate { get; set; }

        /// <summary>
        /// Get or Set user id
        /// </summary>
        public int PaymentByUserId { get; set; }

        /// <summary>
        /// Get or Set user name
        /// </summary>
        public string PaymentByUserName { get; set; }

        /// <summary>
        /// Get or Set caretaker id
        /// </summary>
        public int PaymentToCareTakerId { get; set; }

        /// <summary>
        /// Get or Set care taker name
        /// </summary>
        public string PaymentToCareTakerName { get; set; }

        /// <summary>
        /// Get or Set payment type id
        /// </summary>
        public int PaymentTypeId { get; set; }

        /// <summary>
        /// Get or Set payment type mode
        /// </summary>
        public string PaymentTypeMode { get; set; }

        /// <summary>
        /// Get or Set the amount
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Get or Set the remark
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// Get or Set sevice id
        /// </summary>
        public int ServiceId { get; set; }

        /// <summary>
        /// Get or Set service name
        /// </summary>
        public string ServiceName { get; set; }
    }
}