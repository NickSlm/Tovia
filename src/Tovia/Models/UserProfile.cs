using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Tovia.Models
{
    public class UserProfile
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; }
        public BitmapImage? Pfp { get; set; }
        public string? TaskListId { get; set; }
    }
}
