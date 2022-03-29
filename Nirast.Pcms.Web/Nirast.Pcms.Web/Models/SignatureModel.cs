using System;
using System.ComponentModel.DataAnnotations;

namespace Nirast.Pcms.Web.Models
{
    public class SignatureModel
    {
        public int CaretakerId { get; set; }

        public string Caretaker { get; set; }

        public string policyContent { get; set; }

        [UIHint("SignaturePad")]
        public byte[] CaretakerSignature { get; set; }

        public DateTime AgreedDate { get; set; }

    }
}