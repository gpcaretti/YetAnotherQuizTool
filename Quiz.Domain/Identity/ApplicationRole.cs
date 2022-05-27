using Microsoft.AspNetCore.Identity;

namespace Quiz.Domain.Identity {
    public class ApplicationRole : IdentityRole {
        public ApplicationRole() {
        }

        public ApplicationRole(Guid id) {
            Id = id.ToString();
        }

        public ApplicationRole(string roleName)
            : this(Guid.NewGuid(), roleName) {
        }

        public ApplicationRole(Guid id, string roleName) {
            Id = id.ToString();
            Name = roleName;
            NormalizedName = roleName?.ToUpperInvariant();
        }
    }
}