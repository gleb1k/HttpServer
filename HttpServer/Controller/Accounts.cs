﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using HttpServer.Attributes;
using Microsoft.Data.SqlClient;


namespace HttpServer.Controller
{
    [HttpController("accounts")]
    public class Accounts
    {
        ////GetAccounts, GetAccountById и SaveAccount 
        [HttpGET("getaccountbyid")]
        public Account GetAccountById(int id)
        {

            Account account = null;
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True";


            string sqlExpression = "select * from [dbo].[Table]";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                //если есть данные 
                if (reader.HasRows)
                {
                    //Построчно считываем данные
                    while (reader.Read())
                    {
                        if (reader.GetInt32(0) == id)
                        {
                            account = new Account
                         (
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetString(2));
                            break;
                        }
                    }
                }
                reader.Close();
            }
            return account;
        }

        [HttpGET("getaccounts")]
        public List<Account> GetAccounts()
        {
            List<Account> accounts = new List<Account>();
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True";

            string sqlExpression = "select * from [dbo].[Table]";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                //если есть данные 
                if (reader.HasRows)
                {
                    //Построчно считываем данные
                    while (reader.Read())
                    {
                        accounts.Add(new Account
                        (reader.GetInt32(0),
                         reader.GetString(1),
                         reader.GetString(2))
                                );
                    }
                }
                reader.Close();
            }
            return accounts;
        }

        [HttpPOST("saveaccount")]
        public static async void SaveAccount(string login = "test3", string password = "test3")
        {
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True";

            // выражение SQL для добавления данных
            string sqlExpression = "INSERT INTO [dbo].[Table] (Login, Password) VALUES (@login, @password)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                SqlCommand command = new SqlCommand(sqlExpression, connection);

                // создаем параметр для логина
                SqlParameter loginParam = new SqlParameter("@login", login);
                // добавляем параметр к команде
                command.Parameters.Add(loginParam);
                // создаем параметр для пароля
                SqlParameter passwordParam = new SqlParameter("@password", password);
                // добавляем параметр к команде
                command.Parameters.Add(passwordParam);

                // записываем строку в бд
                await command.ExecuteNonQueryAsync();
            }
        }
        [HttpPOST("login")]
        public static bool LoginPOST(string login = "test3", string password = "test3")
        {
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True";

            string sqlExpression = "select * from [dbo].[Table]";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                //если есть данные 
                if (reader.HasRows)
                {
                    //Построчно считываем данные
                    while (reader.Read())
                    {
                        if (reader.GetString(1) == login && reader.GetString(2) == password)
                        {
                            return true;
                        }
                    }
                }
                reader.Close();
            }
            return false;
        }

    }
}
