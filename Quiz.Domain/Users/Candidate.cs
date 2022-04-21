using System;
using System.ComponentModel.DataAnnotations;

namespace Quiz.Domain.Users {

    public class Candidate : Entity<Guid> {

        //public Candidate()
        //    : this(Guid.Empty) {
        //}

        public Candidate(Guid id)
            : base(id) {
        }

        [MaxLength(256)]
        public string Name { get; set; }

        [MaxLength(256)]
        public string Email { get; set; }

        [MaxLength(24)]
        public string? Phone { get; set; }

        [MaxLength(128)]
        public string Roles { get; set; }

        [MaxLength(256)]
        public string Password { get; set; }

        [MaxLength(256)]
        public string? ImgFile { get; set; }
    }
}
