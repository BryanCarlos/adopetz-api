namespace Adopetz.Api.Infrastructure.Authentication.Models
{
    internal class RefreshTokenSettings
    {
        public const string SectionName = "RefreshTokenSettings";

        public byte TokenBytes { get; set; }
        public int ExpiryDays { get; set; }
    }
}
