using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class LoggedInUser
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string FirstName { get; set; }
        public byte[] ProfilePicByte { get; set; }
        public string LastName { get; set; }
        public string UserType { get; set; }
        public int UserStatus { get; set; }
        public bool IsVerified { get; set; }
        public string EmailAddress { get; set; }
        public string LoginName { get; set; }
        public string Location { get; set; }
        public string Password { get; set; }
        public string ProfilePicPath { get; set; }
        public int CityId { get; set; }
        public string EmployeeNumber { get; set; }

        public int WorkRoleId { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }

        public int BranchId { get; set; }

        public string cityname { get; set; }
        public string statename { get; set; }
        public string countryname { get; set; }
    }

    public class UserCredential
    {
        public string LoginName { get; set; }
        public string Password{ get; set; }
    }
}
