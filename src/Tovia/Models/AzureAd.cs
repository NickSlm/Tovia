namespace Tovia.Models
{
    public class AzureAd
    {
        public required string ClientId { get; set; }
        public required string Tenant { get; set; }
        public required string Instance { get; set; }
        public required string[] Scopes { get; set; }
    }
}
