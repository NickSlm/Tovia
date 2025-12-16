using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tovia.Models;

namespace Tovia.Data
{
    public class Item
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public TaskState Status { get; set; }
        public DateTime DueDate { get; set; }
        public string Priority { get; set; }
        public string? TaskId { get; set; }
        public string? EventId { get; set; }
        public DateTime LastTimeModified { get; set; }
        public bool SoftDelete { get; set; } = false;
    }
}
