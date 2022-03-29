using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Helpers
{
    public class DateTimeHelper
    {
        public Dictionary<int, string> GetMonthList()
        {
            var months = new Dictionary<int, string>()
           {
                { 0, "--Select--" },
                { 1, "January" },
                { 2, "February" },
                { 3, "March" },
                { 4, "April" },
                { 5, "May" },
                { 6, "June" },
                { 7, "July" },
                { 8, "August" },
                { 9, "September" },
                { 10, "October" },
                { 11, "November" },
                { 12, "December" },
            };
            return months;
        }
        public List<int> GetYearList()
        {
            var year = new List<int>();
            for (int i = DateTime.Now.Year; i < DateTime.Now.Year+10; i++)
            {
                year.Add(i);
            }
           // var year = new Dictionary<int, int>()
           //{
           //     { 0,DateTime.Now.Year+10}
               
           // };
            return year;
        }
    }
}