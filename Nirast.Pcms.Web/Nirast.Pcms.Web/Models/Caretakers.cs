using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class Caretakers
    {
        public string CareTakerName { get; set; }
        public int CaretakerId { get; set; }
        public string ProfileId { get; set; }
        public string Phone1 { get; set; }
        public string TypeName { get; set; }
        public int RejectedId { get; set; }
        public string rejectedstatus { get; set; }
    }
}