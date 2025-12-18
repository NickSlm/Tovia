using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tovia.Data;

namespace Tovia.interfaces
{
    public interface ILocalDBService
    {
        Task SaveTask(UsersTasks task);
    }
}
