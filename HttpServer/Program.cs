using HttpServer.Controller;
using System.Text;

namespace HttpServer
{
    internal class Program
    {
        private static bool _appIsRunning = true;
        static void Main(string[] args)
        {
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True";

            var result = new MyORM(connectionString).AddParameter("@Login", "aboba")
                .AddParameter("Password", "12345678").ExecuteNonQuery("insert into [dbo].[Table] values (@Login,@Password)"); 








            var af = Directory.Exists("\\site\\index.html" );

            //--Работа с найстройками сервера (сериализация и десериализация json)--
            var settings = new ServerSettings();
            settings.Serialize();
            var settingsDeserialized = ServerSettings.Deserialize();


            //Запуск сервера
            var httpserver = new HttpServer();
            using (httpserver)
            {
                httpserver.Start();

                while (_appIsRunning)
                {
                    Handler(Console.ReadLine()?.ToLower(), httpserver);
                }
            }
        }
        static void Handler(string command, HttpServer httpserver)
        {
            switch (command)
            {
                case "stop":
                    httpserver.Stop();
                    break;
                case "start":
                    httpserver.Start();
                    break;
                case "restart":
                    httpserver.Stop();
                    httpserver.Start();
                    break;
                case "status":
                    Console.WriteLine(httpserver.Status);
                    break;
                case "exit":
                    _appIsRunning = false;
                    break;
            }
        }

    }
}