namespace Quiz.Application.Exams {

    // FIXME!
    public class ExamSessionDtoZZZ : BaseEntityDto<Guid> {
        public Guid? AncestorId { get; private set; }

        public string Name { get; set; }

        public string? Code { get; set; }

        /// <summary>True if choices for questions will be randomized when presented to the candidate</summary>
        public bool RandomChoicesAllowed { get; set; } = false;

        public int Duration { get; set; } = 0;

        public decimal FullMarks { get; set; } = 100;
    }
}
