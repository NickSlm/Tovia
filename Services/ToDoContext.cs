using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ToDoListPlus.Services
{

    public class ToDoContext: DbContext
    {

    public DbSet<ToDoItem> ToDoItems { get; set; }


    public ToDoContext(DbContextOptions<ToDoContext> options): base(options)
        {
            
        }


    }
}
