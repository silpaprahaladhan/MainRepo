using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class QuestionsModel
    {
        /// <summary>
        /// Get or Set the question id
        /// </summary>
        public int QuestionId { get; set; }

        /// <summary>
        /// Get or Set user id
        /// </summary>
        public int CreatedUserId { get; set; }

        /// <summary>
        /// Get or Set updated user id
        /// </summary>
        public int UpdatedUserId { get; set; }

        /// <summary>
        /// Get or Set the questions
        /// </summary>
        public string Questions { get; set; }

        /// <summary>
        /// Get or Set answer
        /// </summary>
        public string Answer { get; set; }
    }
}