namespace Quiz.Domain.Exams {

    public class ChoiceDto : BaseEntityDto<Guid> {
        public Guid QuestionId { get; set; }
        public string Statement { get; set; }

        /// <summary>true if the right answer</summary>
        public bool IsCorrect { get; set; }

        /// <summary>Relative position of the choice (from 0)</summary>
        public int? Position { get; set; }
    }
}
