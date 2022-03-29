using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class SearchViewModel
    {
        public int CaretakerId { get; set; }

        public string CaretakerFirstName { get; set; }

        public string CaretakerLastName { get; set; }

        public int Rate{ get; set; }
    }
}