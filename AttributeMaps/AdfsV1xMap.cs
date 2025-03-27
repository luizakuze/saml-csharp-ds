namespace SamlCsharp.AttributeMaps
{
    public static class AdfsV1xMap
    {
        public const string Identifier = "urn:oasis:names:tc:SAML:2.0:attrname-format:unspecified";

        public static readonly Dictionary<string, string> From = new()
        {
            { "http://schemas.xmlsoap.org/claims/commonname", "commonName" },
            { "http://schemas.xmlsoap.org/claims/emailaddress", "emailAddress" },
            { "http://schemas.xmlsoap.org/claims/group", "group" },
            { "http://schemas.xmlsoap.org/claims/upn", "upn" }
        };

        public static readonly Dictionary<string, string> To = new()
        {
            { "commonName", "http://schemas.xmlsoap.org/claims/commonname" },
            { "emailAddress", "http://schemas.xmlsoap.org/claims/emailaddress" },
            { "group", "http://schemas.xmlsoap.org/claims/group" },
            { "upn", "http://schemas.xmlsoap.org/claims/upn" }
        };
    }
}