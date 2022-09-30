using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace HttpServer
{
    public class HttpServer
    {
        private readonly string _url;
        private readonly HttpListener _listener;
        private HttpListenerContext _httpContext;

        public HttpServer(string url)
        {
            _url = url;
            _listener = new HttpListener();

            _listener.Prefixes.Add(_url);
        }

        public void Start()
        {
            _listener.Start();
            Console.WriteLine("Ожидание подключений...");

            Listener();
        }
        private void Listener()
        {
            try
            {
                while (true)
                {
                    // метод GetContext блокирует текущий поток, ожидая получение запроса 
                    _httpContext = _listener.GetContext();
                    HttpListenerRequest request = _httpContext.Request;

                    // получаем объект ответа
                    HttpListenerResponse response = _httpContext.Response;

                    FileStream fstream = File.OpenRead(@"C:\Users\gleb\Desktop\Прога\2КУРС\WEB\HttpServer\HttpServer\web\google.html");
                    byte[] buffer = new byte[fstream.Length];
                    // считываем данные
                    fstream.Read(buffer, 0, buffer.Length);
                    // получаем поток ответа и пишем в него ответ
                    response.ContentLength64 = buffer.Length;
                    Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    // закрываем поток
                    output.Close();

                }
            }
            //Если задаем неправильный путь к google.html, то нам выкидывает ошибку и мы можем выбрать команды. Иначе сервер работает бессконечно и ничего нового не происходит
            //Не знаю как тут это колхозить без использования потоков(

            catch (Exception FileNotFoundException)
            {
                Console.WriteLine(FileNotFoundException.Message);
                Console.WriteLine("Server is dead :с. Commands: \"Stop\", \"Restart\", \"~/google/\" ");

                switch (Console.ReadLine())
                {
                    case "Stop":
                        Stop();
                        break;
                    case "Restart":
                        {
                            Stop();
                            Start();
                        }
                        break;
                    case "~/google/":
                        {
                            // метод GetContext блокирует текущий поток, ожидая получение запроса 
                            _httpContext = _listener.GetContext();
                            HttpListenerRequest request = _httpContext.Request;

                            // получаем объект ответа
                            HttpListenerResponse response = _httpContext.Response;

                            FileStream fstream = File.OpenRead(@"C:\Users\gleb\Desktop\Прога\2КУРС\WEB\HttpServer\HttpServer\web\google.html");
                            byte[] buffer = new byte[fstream.Length];
                            // считываем данные
                            fstream.Read(buffer, 0, buffer.Length);
                            // получаем поток ответа и пишем в него ответ
                            response.ContentLength64 = buffer.Length;
                            Stream output = response.OutputStream;
                            output.Write(buffer, 0, buffer.Length);
                            // закрываем поток
                            output.Close();
                        }
                        break;
                }
            }
        }
        public void Stop()
        {
            _listener.Stop();
            Console.WriteLine("Обработка подключений завершена");
        }
    }
}
