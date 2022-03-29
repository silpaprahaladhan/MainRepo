using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Entities
{
   public class PaymentReport
    {
        /// <summary>
        /// Get or Set the Caretaker Type
        /// </summary>
        public int? CaretakerType { get; set; }

        /// <summary>
        /// Get or Set the Caretaker Type
        /// </summary>
        public int? CareTaker { get; set; }

        /// <summary>
        /// Get or Set from date
        /// </summary>

        public DateTime? FromDate { get; set; }

        /// <summary>
        /// Get or Set to date
        /// </summary>

        public DateTime? ToDate { get; set; }

        /// <summary>
        /// Get or Set the Year
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// Get or Set the Month
        /// </summary>
        public int? Month { get; set; }

        /// <summary>
        /// Get or Set the Month
        /// </summary>
        public int? TransactionStatus { get; set; }
        public int? ServiceType { get; set; }
        public int? SearchType { get; set; }
    }
}
