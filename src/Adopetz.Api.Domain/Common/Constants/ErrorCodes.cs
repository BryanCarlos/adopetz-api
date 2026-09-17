namespace Adopetz.Api.Domain.Common.Constants
{
    public static class ErrorCodes
    {
        public static class Common
        {
            public const string Required = "REQUIRED";
            public const string MaximumLength = "MAXIMUM_LENGTH";
            public const string InvalidRange = "INVALID_RANGE";
            public const string InvalidValue = "INVALID_VALUE";
            public const string ForbiddenAccess = "FORBIDDEN_ACCESS";
            public const string ExternalServiceProblem = "EXTERNAL_SERVICE_PROBLEM";

            public static class Security
            {
                public const string InvalidCredentials = "INVALID_CREDENTIALS";
            }
        }

        public static class User
        {
            public const string EmailAlreadyInUse = "USER_EMAIL_ALREADY_IN_USE";
            public const string NotFound = "USER_NOT_FOUND";

            public static class Validation
            {
                public const string InvalidEmail = "USER_INVALID_EMAIL";
                public const string InvalidPassword = "USER_INVALID_PASSWORD";
                public const string PasswordTooShort = "USER_PASSWORD_TOO_SHORT";
                public const string PasswordsMustNotMatch = "USER_PASSWORDS_MUST_NOT_MATCH";
                public const string PasswordMustHaveUppercase = "USER_PASSWORDS_MUST_HAVE_UPPERCASE";
                public const string PasswordMustHaveLowercase = "USER_PASSWORDS_MUST_HAVE_LOWERCASE";
                public const string PasswordMustHaveNumber = "USER_PASSWORDS_MUST_HAVE_NUMBER";
            }
        }

        public static class Pet
        {
            public const string NotFound = "PET_NOT_FOUND";
            public const string AlreadyAdopted = "PET_ALREADY_ADOPTED";
            

            public static class Vaccine
            {
                public const string NotFound = "VACCINE_NOT_FOUND";
                public const string InvalidVaccineDate = "INVALID_VACCINE_DATE";
            }

            public static class Image
            {
                public const string NotFound = "IMAGE_NOT_FOUND";
                public const string ImageTooLarge = "PET_IMAGE_TOO_LARGE";
                public const string MaxImagesLimit = "PET_MAX_IMAGES_EXCEEDED";
                public const string FailedUpload = "PET_IMAGE_FAILED_UPLOAD";
                public const string InvalidFormat = "PET_IMAGE_INVALID_FORMAT";
            }
        }
    }
}
