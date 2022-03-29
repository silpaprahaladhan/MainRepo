using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class EmailConfiguration
    {
        public int ConfigId { get; set; }
        public string MailHost { get; set; }
        public int MailPort { get; set; }
        public bool SSL { get; set; }
        public bool IsDefault { get; set; }
        public string ConfigName { get; set; }
    }

    public class EmailTypeConfiguration
    {
        public int ConfigId { get; set; }
        public string Emailtype { get; set; }
        public int EmailtypeId { get; set; }
        public string FromEmail { get; set; }
        public string Password { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
    }
}
