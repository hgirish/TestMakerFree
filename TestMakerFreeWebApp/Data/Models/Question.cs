using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TestMakerFreeWebApp.Data.Models
{
    public class Question
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public int QuizId { get; set; }
        [Required]
        public string Text { get; set; }
        public string Notes { get; set; }
        [DefaultValue(0)]
        public int Type { get; set; }
        [DefaultValue(0)]
        public int Flags { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public DateTime LastModifiedDate { get; set; }

        /// <summary>
        /// The parent quiz.
        /// </summary>
        [ForeignKey("QuizId")]
        public virtual Quiz Quiz { get; set; }
        /// <summary>
        /// A list containing all the answer related to this question.
        /// </summary>
        public virtual List<Answer> Answers { get; set; }

    }
}
