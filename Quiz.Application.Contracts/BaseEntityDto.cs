
namespace PatenteN.Quiz.Domain {

    public abstract class BaseEntityDto<TPrimaryKey> {
        public TPrimaryKey? Id { get; set; }

        public DateTimeOffset? CreatedOn { get; set; }

        public DateTimeOffset? ModifiedOn { get; set; }

        public string? CreatedBy { get; set; }

        public string? ModifiedBy { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
