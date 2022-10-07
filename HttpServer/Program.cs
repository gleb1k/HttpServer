using System.Text;

namespace HttpServer
{
    internal class Program
    {
        private static bool _appIsRunning = true;
        static void Main(string[] args)
        {
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
        static async void Handler(string command, HttpServer httpserver)
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
                    //Без ожидания все падает. Не знаю как фиксить(
                    Thread.Sleep(50);
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