using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Quiz.Domain.Identity {

    public class QuizIdentityDBContext
        //: IdentityDbContext<ApplicationUser, ApplicationRole, string> {
        : ApiAuthorizationDbContext<ApplicationUser> {

        private const string ADMIN_ROLE_ID = "9a8dc12f-c862-e400-6e11-3a0355e4c36f";
        private const string MANAGER_ROLE_ID = "1a5442a6-de43-d043-8b50-3a0356472148";
        private const string CANDIDATE_ROLE_ID = "8d29bb6f-d47c-ac17-0256-3a035ce14eb3";

        private const string ADMIN_EMAIL = "admin@gpsoftware.it";
        private const string ADMIN_USERNAME = "admin@gpsoftware.it";
        private const string ADMIN_PASSWORD = "mypassword";
        private const string ADMIN_ID = "36bace61-5b6a-6bd6-6050-3a030665f9f7";

        protected QuizIdentityDBContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options, operationalStoreOptions) {
        }

        public QuizIdentityDBContext(
            DbContextOptions<QuizIdentityDBContext> options,
            IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options, operationalStoreOptions) {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            DoSeedData(modelBuilder);
        }

        private void DoSeedData(ModelBuilder modelBuilder) {
            // roles
            modelBuilder.Entity<ApplicationRole>()
                .HasData(
                         new ApplicationRole (new Guid(ADMIN_ROLE_ID), "Admin") {
                             NormalizedName = "ADMIN",
                             ConcurrencyStamp = new DateTime(2022, 04, 25).ToString(),
                         },
                         new ApplicationRole (new Guid(MANAGER_ROLE_ID), "Manager") {
                             NormalizedName = "MANAGER",
                             ConcurrencyStamp = new DateTime(2022, 04, 25, 0, 0, 0, DateTimeKind.Utc).ToString(),
                         },
                         new ApplicationRole(new Guid(CANDIDATE_ROLE_ID), "Candidate") {
                             NormalizedName = "CANDIDATE",
                             ConcurrencyStamp = new DateTime(2022, 04, 25, 0, 0, 0, DateTimeKind.Utc).ToString(),
                         }
                     );

            // users
            var hasher = new PasswordHasher<ApplicationUser>();
            modelBuilder.Entity<ApplicationUser>()
                .HasData(
                         new ApplicationUser (new Guid(ADMIN_ID), ADMIN_USERNAME) {
                             Email = ADMIN_EMAIL,
                             NormalizedEmail = ADMIN_EMAIL.ToUpperInvariant(),
                             PasswordHash = hasher.HashPassword(null, ADMIN_PASSWORD),
                             EmailConfirmed = true,
                             SecurityStamp = new DateTime(2022, 04, 25, 0, 0, 0, DateTimeKind.Utc).ToString()
                         }
                     );

            modelBuilder.Entity<IdentityUserRole<string>>()
                .HasData(
                    new IdentityUserRole<string> {
                        RoleId = ADMIN_ROLE_ID,
                        UserId = ADMIN_ID
                    }
                );
        }

    }
}
