using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoListPlus.Services
{
    public class DatabaseInitializer: IDatabaseInitializer
    {


        private readonly ToDoContext _dbContext;    


        public DatabaseInitializer(ToDoContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Initialize()
        {
            _dbContext.Database.EnsureCreated();
        }
    }
}
