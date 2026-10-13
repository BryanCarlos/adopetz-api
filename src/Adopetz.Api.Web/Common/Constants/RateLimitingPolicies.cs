namespace Adopetz.Api.Web.Common.Constants
{
    public static class RateLimitingPolicies
    {
        public const string LoginStrict = "LoginStrictPolicy";
        public const string SessionRefresh = "SessionRefreshPolicy";
        public const string PublicQueryStrict = "PublicQueryPolicy";
        public const string PetRegisterStrict = "PetRegisterPolicy";
        public const string PetImageRegisterStrict = "PetImageRegisterPolicy";
    }
}
