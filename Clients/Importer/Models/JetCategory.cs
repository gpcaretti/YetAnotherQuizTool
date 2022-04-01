namespace PatenteN.Importer.Models {
    public partial class JetCategory {
        public JetCategory()
        {
            InverseAncestor = new HashSet<JetCategory>();
            Questions = new HashSet<JetQuestion>();
        }

        public int Id { get; set; }
        public int? AncestorId { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;

        public virtual JetCategory? Ancestor { get; set; }
        public virtual ICollection<JetCategory> InverseAncestor { get; set; }
        public virtual ICollection<JetQuestion> Questions { get; set; }
    }
}
