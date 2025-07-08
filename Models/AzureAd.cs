namespace ToDoListPlus.Models
{
    public class AzureAd
    {
        public string ClientId { get; set; }
        public string Tenant { get; set; }
        public string Instance { get; set; }
        public string[] Scopes { get; set; }
    }
}
