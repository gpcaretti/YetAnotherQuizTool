using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Quiz.Domain {

    public abstract class Entity {

        protected Entity() {
            CreatedOn = DateTimeOffset.Now;
        }

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

    public abstract class Entity<TPrimaryKey> : Entity where TPrimaryKey : IEquatable<TPrimaryKey> {

        protected Entity()  {
        }

        public Entity(TPrimaryKey id) : this() {
            Id = id;
        }

        public TPrimaryKey Id { get; private set; }
    }
}
