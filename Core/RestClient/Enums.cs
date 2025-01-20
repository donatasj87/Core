namespace Donatas.Core.RestClient
{
    public enum HttpRequestStatus
    {
        Success,
        Fail
    }

    public enum BodyType
    {
        raw,
        typed,
        x_www_form_urlencoded
    }

    /// <summary>
    /// https://docs.microsoft.com/en-us/azure/active-directory/develop/msal-net-differences-adal-net
    /// </summary>
    public enum AuthenticationType
    {
        Adal,
        Msal,
        ManagedIdentity
    }
}
