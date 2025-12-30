using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tovia.Models
{
    public class GoogleAuth
    {
        public required string ClientId { get; set; }
        public required string ClientSecret { get; set; }
        public required string[] Scopes { get; set; }
    }
}
