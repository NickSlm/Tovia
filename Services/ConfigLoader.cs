using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ToDoListPlus.Models;

namespace ToDoListPlus.Services
{
    public class ConfigLoader
    {

        public static AuthConfig Load(string path) 
        {

            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<AuthConfig>(json);

        }

    }
}
