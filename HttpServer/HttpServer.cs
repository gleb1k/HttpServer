using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace HttpServer
{
    public class HttpServer : IDisposable
    {
        public ServerStatus Status = ServerStatus.Stop;
        
        private ServerSettings _serverSettings;
        private readonly HttpListener _httpListener;
        public HttpServer()
        {
            _serverSettings = ServerSettings.Deserialize();
            _httpListener = new HttpListener();
        }

        public void Start()
        {
            if (Status == ServerStatus.Start)
            {
                Console.WriteLine("Сервер уже запущен!");
            }
            else
            {
                Console.WriteLine("Запуск сервера...");

                _serverSettings = ServerSettings.Deserialize();
                _httpListener.Prefixes.Clear();
                _httpListener.Prefixes.Add($"http://localhost:" + _serverSettings.Port + "/");

                _httpListener.Start();
                Console.WriteLine("Ожидание подключений...");
                Status = ServerStatus.Start;
            }

            Listening();
        }
        public void Stop()
        {
            if (Status == ServerStatus.Start)
            {
                _httpListener.Stop();
                Status = ServerStatus.Stop;
                Console.WriteLine("Обработка подключений завершена");
            }
            else
                Console.WriteLine("Сервер уже остановлен");
        }

        private void Listening()
        {
            _httpListener.BeginGetContext(new AsyncCallback(ListenerCallback), _httpListener);
        }

        private void ListenerCallback(IAsyncResult result)
        {
            try
            {
                if (_httpListener.IsListening)
                {
                    var context = _httpListener.EndGetContext(result);

                    //объект ответа
                    var response = context.Response;

                    var request = context.Request;

                    byte[] buffer;

                    var rawurl = request.RawUrl;

                    buffer = Files.GetFile(rawurl.Replace("%20", " "));

                    //Задаю расширения для файлов
                    Files.GetExtension(ref response, "." + rawurl);

                    //Неправильно задан запрос / не найдена папка
                    if (buffer == null)
                    {
                        response.Headers.Set("Content-Type", "text/html");
                        response.StatusCode = 404;
                        response.ContentEncoding = Encoding.UTF8;
                        string err = "<h1>404<h1><h2>The resource can not be found.<h2>";
                        buffer = Encoding.UTF8.GetBytes(err);
                    }
                    Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);

                    //закрываем поток
                    output.Close();

                    Listening();
                }
            }
            catch 
            {
                //костыль
                Console.WriteLine("костыль");
                _httpListener.Stop();
            }
        }
        public void Dispose()
        {
            Stop();
        }
    }
}
