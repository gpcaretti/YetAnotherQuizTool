using System.ComponentModel.DataAnnotations;

namespace PatenteN.Quiz.Application.Dtos {
    public class PagedAndSortedResultRequestDto {

        [Range(1, int.MaxValue)]
        public virtual int MaxResultCount { get; set; } = 20;

        [Range(0, int.MaxValue)]
        public virtual int SkipCount { get; set; } = 0;

        public virtual string? Sorting { get; set; }
    }
}
