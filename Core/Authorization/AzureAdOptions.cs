namespace Donatas.Core.Authorization
{
    public class AzureAdOptions
    {
        public string Instance { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public string TenantId { get; set; } = "{TenantId}.onmicrosoft.com";
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string CallbackPath { get; set; } = string.Empty;
        public string SignedOutCallbackPath { get; set; } = string.Empty;
    }
}
