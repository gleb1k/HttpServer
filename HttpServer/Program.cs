using System.Text;

namespace HttpServer
{
    internal class Program
    {
        private static bool _appIsRunning = true;
        static void Main(string[] args)
        {
            var settings = new ServerSettings(7700, "\\site");

            var httpserver = new HttpServer(settings);
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