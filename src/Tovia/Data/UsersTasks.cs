using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tovia.Data
{
    public class UsersTasks
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime LastTimeModified { get; set; }
        public string TaskId { get; set; }
        public string? EventId { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public bool SoftDelete { get; set; }
        public int UserId { get; set; }
        public Users User { get; set; }

    }
}
