namespace PatenteN.Importer.Models {
    public partial class Answer
    {
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }
        public string Statement { get; set; } = null!;
        public bool IsCorrect { get; set; }
        public int Position { get; set; }

        public virtual Question Question { get; set; } = null!;
    }
}
