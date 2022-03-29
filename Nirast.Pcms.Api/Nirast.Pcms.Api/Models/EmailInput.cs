
namespace Nirast.Pcms.Api.Models
{
    public class EmailInput
    {
        public int UserId
        {
            get;
            set;
        }

        public string UserName
        {
            get;
            set;
        }
        public string EmailId
        {
            get;
            set;
        }

        public string Subject
        {
            get;
            set;
        }

        public string Body
        {
            get;
            set;
        }
        public string Attachments
        {
            get;
            set;
        }
    }
}