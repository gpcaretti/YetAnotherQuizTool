namespace PatenteN.Importer.Models {
    public partial class JetQuestion
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Statement { get; set; } = null!;
        public string? Image { get; set; }
        public string? Comment { get; set; }
        public string? ImageComment { get; set; }
    }
}
