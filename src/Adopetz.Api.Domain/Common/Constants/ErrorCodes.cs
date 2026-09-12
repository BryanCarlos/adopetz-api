namespace Adopetz.Api.Domain.Common.Constants
{
    public static class ErrorCodes
    {
        public static class Common
        {
            public const string NotFound = "NOT_FOUND";

            public static class Security
            {
                public const string InvalidCredentials = "INVALID_CREDENTIALS";
            }
        }

        public static class User
        {
            public const string EmailAlreadyInUse = "USER_EMAIL_ALREADY_IN_USE";
            
            public static class Validation
            {
                public const string InvalidEmail = "USER_INVALID_EMAIL";
                public const string InvalidPassword = "USER_INVALID_PASSWORD";
                public const string PasswordTooShort = "USER_PASSWORD_TOO_SHORT";
                public const string PasswordsMustNotMatch = "USER_PASSWORDS_MUST_NOT_MATCH";
            }
        }

        public static class Pet
        {
            public const string AlreadyAdopted = "PET_ALREADY_ADOPTED";
        }
    }
}
