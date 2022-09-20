namespace Quiz.Domain.Identity {

    /// <summary>
    ///     Store candidate's info
    /// </summary>
    public class Candidate : Entity<Guid> {

        // only for EF
        private Candidate() {
        }

        public Candidate(Guid id)
            : base(id) {
        }
    }
}
