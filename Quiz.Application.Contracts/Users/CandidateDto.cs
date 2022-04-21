namespace Quiz.Application.Users {

    public class BasicCandidateDto : BaseEntityDto<Guid> {
        public string Name { get; set; }
    }

    public class CandidateDto : BasicCandidateDto {

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Roles { get; set; }
        public string Password { get; set; }

        public string ImgFile { get; set; }

    }
}
