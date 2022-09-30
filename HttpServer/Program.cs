using System.Text;

namespace MyApp 
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Гугл находится в HttpServer/bin/Debug/net6.0

            var httpserver = new HttpServer.HttpServer("http://localhost:8888/connection/");
            httpserver.Start();

        }
    }
}