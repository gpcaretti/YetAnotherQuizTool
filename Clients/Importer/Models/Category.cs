namespace PatenteN.Importer.Models {
    public partial class Category {
        public Category() {
            InverseAncestor = new HashSet<Category>();
            Questions = new HashSet<Question>();
        }

        public Guid Id { get; set; }
        public Guid? AncestorId { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;

        public virtual Category? Ancestor { get; set; }
        public virtual ICollection<Category> InverseAncestor { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
    }
}
