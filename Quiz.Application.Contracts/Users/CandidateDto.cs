namespace Quiz.Domain.Users {

    public class CandidateDto : BaseEntityDto<Guid> {
        public string Name { get; set; }

        public string Email { get; set; }

        public string? Phone { get; set; }

        public string Roles { get; set; }
        public string Password { get; set; }

        public string? ImgFile { get; set; }

    }
}
