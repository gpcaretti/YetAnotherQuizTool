namespace Quiz.Application {

    public static class QuizConstants {
        public static class Roles {
            public const string Admin = "Admin";
            public const string Manager = "Manager";
            public const string Candidate = "Candidate";
        }

        public static class ClientNames {
            public const string Standard = "Quiz.Blazor.Host.ServerAPI";
            public const string Anonymous = "ServerAPI.NoAuthenticationClient";
        }

    }
}
