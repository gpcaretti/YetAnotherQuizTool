namespace PatenteN.Importer.Models {
    public partial class Sorgente {
        public Sorgente() {
        }

        public Sorgente(string[] line) {
            Code = line[0];
            IsCorrect = line[1]?.Trim().ToUpperInvariant();
            Position = line[2]?.Trim().ToLowerInvariant();
            Statement = line[3]?.Trim();
        }

        public string Id { get; set; } = null!;
        public string? Code { get; set; }
        public string? IsCorrect { get; set; }
        public string? Position { get; set; }
        public string? Statement { get; set; }
    }
}
