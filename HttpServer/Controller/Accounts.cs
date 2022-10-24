using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpServer.Attributes;

namespace HttpServer.Controller
{
    [HttpContoller("accounts")]
    public class Accounts
    {
        //GetAccounts, GetAccountById и SaveAccount 
        public Account GetAccountById(int id)
        {
            throw new NotImplementedException();
        }
        public List<Account> GetAccounts()
        {
            string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=SteamDB;Integrated Security = True";

            string sqlExpression = "SELECT * FROM Accounts";
            using (sqlConnection connection = new sqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                //если есть данные 
                if (reader.HasRows)
                {
                    //Выводим названия столбцов
                    Console.WriteLine($"{reader.GetName(0)} \t {reader.GetName(1)} \t {reader.GetName(2)}");

                    //Построчно считываем данные
                    while (reader.Read())
                    {
                        object id = reader.GetValue(0);
                        object login = reader.GetValue(1);
                        object password = reader.GetValue(2);

                        Console.WriteLine($"{id} \t {login} \t {password}");
                    }
                }
                reader.Close();
            }
        }
        public void SaveAccount()
        {

        }

    }
}
