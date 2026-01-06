using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tovia.Models;

namespace Tovia.interfaces
{
    public interface IAuthProvider
    {
        string? AccessToken { get; }
        UserProfile? User { get; }

        Task SignInAsync();
        Task SignOutAsync();
    }
}
