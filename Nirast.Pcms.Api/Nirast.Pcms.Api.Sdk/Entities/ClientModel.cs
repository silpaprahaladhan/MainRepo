using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Entities
{
   //public class ClientModel
   // {
   //      public int ClientId { get; set; }

   //     public int CountryId1 { get; set; }
   //     public int CountryId2 { get; set; }

   //     [Required]
   //     public string ClientName { get; set; }

   //     public string Country { get; set; }

   //     public string State { get; set; }

   //     public string City { get; set; }

   //     public string Location { get; set; }

   //     [Required]
   //     public string Address1 { get; set; }

   //     [Required]
   //     public int ProvinceId1 { get; set; }

   //     public string Province1 { get; set; }

   //     [Required]
   //     public int StateId1 { get; set; }

   //     public string State1 { get; set; }

   //     [Required]
   //     public int CityId1 { get; set; }

   //     public string City1 { get; set; }

   //     public string PhoneNo1 { get; set; }

   //     public string ZipCode1 { get; set; }

   //     public string Address2 { get; set; }

   //     public int ProvinceId2 { get; set; }

   //     public string Province2 { get; set; }

   //     public int StateId2 { get; set; }

   //     public string State2 { get; set; }

   //     public int CityId2 { get; set; }

   //     public string City2 { get; set; }

   //     public string PhoneNo2 { get; set; }

   //     public string ZipCode2 { get; set; }

   //     public bool TaxApplicable { get; set; }

   //     public string EmailAddress { get; set; }

   //     public string WebsiteAddress { get; set; }

   //     public List<ClientCategoryRate> ClientCategoryRateList { get; set; }

   //     public List<OrientationModel> Orientations { get; set; }

   //     public List<CareTakerRegistrationModel> CareTakers { get; set; }

   //     public List<ClientCaretakers> ClientCaretakers { get; set; }

   //     public List<ClientShiftDetails> ClientShiftList { get; set; }

   //     public int ClientStatus { get; set; }

   //     public List<CareTakerRegistrationModel> RegistredCaretakers { get; set; }

   // }

    public class CaretakerScheduling : ClientDetails
    {
        public int CaretakerId { get; set; }

        public string CaretakerName { get; set; }

        public List<string> ServicesByCaretaker { get; set; }

        public string CaretakerCategory { get; set; }
    }

    //public class ClientCategoryRate
    //{
    //    public int CategoryId { get; set; }

    //    public string CategoryName { get; set; }

    //    public float Rate { get; set; }

    //    public bool IsTaxApplicable { get; set; }

    //}

    public class OrientationModel
    {
        public int OrientationId { get; set; }

        [Required]
        public string Orientation { get; set; }
    }

    public class ScheduleDetailModel: CaretakerScheduling
    {
        public string CareShift { get; set; }

        public string WorkShift { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public String TimeIn { get; set; }

        public string TimeOut { get; set; }
    }
    
}
