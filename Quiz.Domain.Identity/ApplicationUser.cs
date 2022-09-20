using Microsoft.AspNetCore.Identity;

namespace Quiz.Domain.Identity {
    public class ApplicationUser : IdentityUser {

        public ApplicationUser() {
        }

        public ApplicationUser(Guid id) {
            Id = id.ToString();
        }

        public ApplicationUser(string userName)
            : this(Guid.NewGuid(), userName) {
        }

        public ApplicationUser(Guid id, string userName) : this() {
            Id = id.ToString();
            UserName = userName;
            NormalizedUserName = userName?.ToUpperInvariant();
        }

    }
}