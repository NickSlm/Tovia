using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
