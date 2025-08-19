namespace ToDoListPlus.Models
{
    public class AzureAd
    {
        public required string ClientId;
        public required string Tenant;
        public required string Instance;
        public required string[] Scopes;
    }
}
