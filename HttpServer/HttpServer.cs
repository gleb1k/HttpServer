using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace HttpServer
{
    internal class HttpServer
    {
        public static void Run()
        {
            HttpListener listener = new HttpListener();
            // установка адресов прослушки
            listener.Prefixes.Add("http://localhost:8888/connection/");
            listener.Start();
            Console.WriteLine("Ожидание подключений...");

            // метод GetContext блокирует текущий поток, ожидая получение запроса 
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;

            // получаем объект ответа
            HttpListenerResponse response = context.Response;

            // создаем ответ в виде кода html
            string responseStr = "<html><head><meta charset='utf8'></head><body>Привет мир!</body></html>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseStr);

            // получаем поток ответа и пишем в него ответ
            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);

            // закрываем поток
            output.Close();

            // останавливаем прослушивание подключений
            listener.Stop();
            Console.WriteLine("Обработка подключений завершена");
            Console.Read();
        }
    }
}
