using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tovia.interfaces;

namespace Tovia.Models
{
    public class AuthSession
    {
        public string AccessToken { get; init; }
        public UserProfile User { get; init; }
        public ITaskProvider TaskProvider { get; init; }



    }
}
