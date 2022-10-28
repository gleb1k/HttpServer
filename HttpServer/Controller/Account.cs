using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpServer.Attributes;

namespace HttpServer.Controller
{
    
    public class Account
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public int Id { get; set; }
        public Account(int id, string login, string password)
        {
            Id = id;
            Login = login;
            Password = password;
            
        }
    }
}
