public static class SameSiteSupport
{
    public static void CheckSameSite(HttpContext context, CookieOptions options)
    {
        if (options.SameSite == SameSiteMode.None)
        {
            var userAgent = context.Request.Headers["User-Agent"].ToString();

            if (DisallowsSameSiteNone(userAgent))
            {
                options.SameSite = (SameSiteMode)(-1); 
            }
        }
    }

    private static bool DisallowsSameSiteNone(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
            return false;

        // iOS 12 e Safari no macOS Mojave
        if (userAgent.Contains("CPU iPhone OS 12") || userAgent.Contains("iPad; CPU OS 12"))
            return true;

        if (userAgent.Contains("Macintosh; Intel Mac OS X 10_14") &&
            userAgent.Contains("Version/") && userAgent.Contains("Safari"))
            return true;

        // Chrome 50 a 69
        if (userAgent.Contains("Chrome/5") || userAgent.Contains("Chrome/6"))
            return true;

        return false;
    }
}