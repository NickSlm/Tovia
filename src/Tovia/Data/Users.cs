using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tovia.Data
{
    public class Users
    {
        public int Id { get; set; }
        public DateTime LastLogin { get; set; }
        public string AccountName { get; set; }
        public List<UsersTasks> Tasks { get; set; }
        public string MicrosoftOid { get; set; }

    }
}
