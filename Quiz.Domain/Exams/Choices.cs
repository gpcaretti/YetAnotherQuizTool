using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatenteN.Quiz.Domain.Exams
{
    public class Choice:BaseEntity
    {
        [Key]
        public int ChoiceID { get; set; }
        public int QuestionID { get; set; }      
        public string DisplayText { get; set; }               
    }
}
