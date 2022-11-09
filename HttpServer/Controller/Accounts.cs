using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using HttpServer.Attributes;
using Microsoft.Data.SqlClient;
using HttpServer.Server;

namespace HttpServer.Controller
{
    [HttpController("accounts")]
    public class Accounts
    {

        private const string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True";

        [HttpGET("getaccountbyid")]
        public Account GetAccountById(int id)
        {
            var myORM = new MyORM(_connectionString);
            return myORM.AddParameter("@id", id).ExecuteQuery<Account>("select * from [dbo].[Table] where Id=@id").FirstOrDefault();
        }

        [HttpGET("getaccounts")]
        public List<Account> GetAccounts()
        {
            var myORM = new MyORM(_connectionString);
            return myORM.ExecuteQuery<Account>("select * from [dbo].[Table]").ToList();
        }

        [HttpPOST("saveaccount")]
        public static void SaveAccount(string login, string password)
        {
            var myORM = new MyORM(_connectionString);
            myORM.AddParameter("@Login", login)
               .AddParameter("@Password", password).ExecuteNonQuery("insert into [dbo].[Table] values (@Login,@Password)");
        }

        [HttpPOST("login")]
        public static bool LoginPOST(string login, string password)
        {
            var myORM = new MyORM(_connectionString);
            int count = 0;
            count += myORM.AddParameter("@login", login).AddParameter("@password", password)
                .ExecuteNonQuery("select * from [dbo].[Table] where Login='@login' and Password='@password'");
            if (count > 0)
                return true;
            else
                return false;
        }

    }
}
