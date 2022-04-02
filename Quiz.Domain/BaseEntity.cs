using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PatenteN.Quiz.Domain {

    public abstract class BaseEntity<TPrimaryKey> {

        public BaseEntity(TPrimaryKey id) {
            Id = id;
        }

        // TODO: mettere private il setter
        public TPrimaryKey Id { get; private set; }

        //[Column(TypeName = "datetime")]
        //[DataType(DataType.DateTime)]
        public DateTimeOffset? CreatedOn { get; set; }

        public DateTimeOffset? ModifiedOn { get; set; }

        [MaxLength(256)]
        public string? CreatedBy { get; set; }

        [MaxLength(256)]
        public string? ModifiedBy { get; set; }

        [DefaultValue(false)]
        public bool? IsDeleted { get; set; } = false;
    }
}
