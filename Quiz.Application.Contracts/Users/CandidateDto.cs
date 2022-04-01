namespace PatenteN.Quiz.Domain.Users {

    public class CandidateDto {

        public int Sl_No { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Candidate_ID { get; set; }

        public string Roles { get; set; }
        public string Password { get; set; }

        public string ImgFile { get; set; }

        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }
}
