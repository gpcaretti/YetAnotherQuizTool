namespace PatenteN.Importer.Models {
    public partial class Question {
        public Question() {
            Answers = new HashSet<Answer>();
        }

        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string Statement { get; set; } = null!;
        public string? Image { get; set; }
        public string? Comment { get; set; }
        public string? ImageComment { get; set; }
        public string? Code { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<Answer> Answers { get; set; }
    }
}
