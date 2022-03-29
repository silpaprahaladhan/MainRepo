using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class QuestionareModel
    {
        /// <summary>
        /// Get or Set the question id
        /// </summary>
        public int QuestionId { get; set; }

        ///// <summary>
        ///// Get or Set user id
        ///// </summary>
        //public int CreatedUserId { get; set; }

        /// <summary>
        /// Get or Set the questions
        /// </summary>
        public string Questions { get; set; }

        /// <summary>
        /// Get or Set updated SortOrder
        /// </summary>
        public int SortOrder { get; set; }


    }
}
