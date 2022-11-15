using HttpServer.Controller;
using HttpServer.Server;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.ORM
{
    public class AccountDAO
    {
        private const string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True";
        public static List<Account> GetAll()
        {
            var myORM = new MyORM(_connectionString);
            return myORM.ExecuteQuery<Account>("select * from [dbo].[Table]").ToList();
        }

        public static Account GetEntityById(int id)
        {
            var myORM = new MyORM(_connectionString);
            return myORM.AddParameter("@id", id).ExecuteQuery<Account>("select * from [dbo].[Table] where Id=@id").FirstOrDefault();
        }
        //Изменить пароль
        public static Account Update(string login, string password)
        {
            var myORM = new MyORM(_connectionString);
            myORM.AddParameter("@Login", login).AddParameter("@Password", password).ExecuteQuery<Account>
                ("UPDATE [dbo].[Table] SET Password = @Password" +
                "\r\nFROM" +
                "\r\n(SELECT * FROM [dbo].[Table] WHERE Login=@Login) AS Selected" +
                "\r\nWHERE [dbo].[Table].Id = Selected.Id");

            return myORM.ExecuteQuery<Account>("select * from [dbo].[Table] where Login=@Login").FirstOrDefault();
        }
        public static bool Delete(int id)
        {
            var myORM = new MyORM(_connectionString);
            myORM.AddParameter("@Id", id).ExecuteQuery<Account>("UPDATE [dbo].[Table] SET Login = 'updated'\r\nFROM\r\n(SELECT * FROM [dbo].[Table] WHERE ID='@Id') AS Selected\r\nWHERE [dbo].[Table].Id = Selected.Id");
            //TODO
            return false;
        }
        public static bool Create(string login, string password)
        {
            var myORM = new MyORM(_connectionString);
            int count = 0;
            count += myORM.AddParameter("@login", login).AddParameter("@password", password)
                .ExecuteNonQuery("select * from [dbo].[Accounts] where Login=@login and Password=@password");
            if (count > 0)
            {
                myORM.AddParameter("@Login", login)
                   .AddParameter("@Password", password).ExecuteNonQuery("insert into [dbo].[Accounts] values (@Login,@Password)");
                return true;
            }
            else
                return false;
        }
        
    }
}
