namespace Quiz.Application {

    public abstract class BaseEntityDto<TPrimaryKey> where TPrimaryKey : IEquatable<TPrimaryKey> {
        public TPrimaryKey? Id { get; set; }

        public DateTimeOffset? CreatedOn { get; set; }

        public DateTimeOffset? ModifiedOn { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
