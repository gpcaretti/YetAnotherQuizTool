using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatenteN.Quiz.Domain {
    public class BaseEntity {

        [Column(TypeName = "datetime")]
        [DataType(DataType.DateTime)]
        public DateTime? CreatedOn { get; set; }

        [Column(TypeName = "datetime")]
        [DataType(DataType.DateTime)]
        public DateTime? ModifiedOn { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string CreatedBy { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
