namespace Donatas.Core.Authorization
{
    public interface ICoreAuthorizationService
    {
        /// <summary>
        /// Returns the Azure Active Directory ID of the caller
        /// </summary>
        /// <returns></returns>
        string? AadId();

        /// <summary>
        /// Returns the Access token from the request
        /// </summary>
        /// <returns>string</returns>
        string? AccessToken();

        /// <summary>
        /// Caller client id from Azure active directory
        /// </summary>
        /// <returns>string</returns>
        string? ClientId();

        /// <summary>
        /// Returns the client IP from the request
        /// </summary>
        /// <returns>string</returns>
        string? ClientIp();

        /// <summary>
        /// Returns the email address of the caller
        /// </summary>
        /// <returns></returns>
        string? Email();

        /// <summary>
        /// Returns the indicator is user is assigned to the Azure AD role
        /// </summary>
        /// <param name="role">Name of the role</param>
        /// <returns>bool</returns>
        bool HasRole(string role);

        /// <summary>
        /// Returns the hosting environment name from the request
        /// </summary>
        /// <returns>string</returns>
        string? Host();

        /// <summary>
        /// Returns the ID token from the request
        /// </summary>
        /// <returns>string</returns>
        string? IdToken();

        /// <summary>
        /// Inidicates if the request was made by user or application
        /// </summary>
        /// <returns>bool</returns>
        bool IsUser();

        /// <summary>
        /// Returns the logic app block nam that triggered the request
        /// </summary>
        /// <returns>string</returns>
        string? LaActionName();

        /// <summary>
        /// Returns the logic app name that triggered the request
        /// </summary>
        /// <returns>string</returns>
        string? LaName();

        /// <summary>
        /// Returns the logic app resource group name that triggered the request
        /// </summary>
        /// <returns>string</returns>
        string? LaRGName();

        /// <summary>
        /// Returns the logic app run id that triggered the request
        /// </summary>
        /// <returns>string</returns>
        string? LaRunId();

        /// <summary>
        /// Returns the name of the caller
        /// </summary>
        /// <returns></returns>
        string? Name();

        /// <summary>
        /// Returns a list of roles assigned to the caller
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetRoles();
    }
}
