namespace Adopetz.Api.Application.Common.Constants
{
    public static class ImageConstants
    {
        public static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg",
            "image/png",
            "image/webp",
            "image/avif"
        };

        public const int MaxImageSizeInBytes = 10 * 1024 * 1024;
    }
}
