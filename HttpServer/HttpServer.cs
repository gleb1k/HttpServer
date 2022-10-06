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
        
        public ServerSettings ServerSettings;
        private readonly HttpListener _httpListener;
        public HttpServer(ServerSettings serverSettings)
        {
            ServerSettings = serverSettings;
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add($"http://localhost:" + serverSettings.Port + "/");
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
            if (_httpListener.IsListening)
            {
                var _httpContext = _httpListener.EndGetContext(result);

                //объект ответа
                var response = _httpContext.Response;

                var request = _httpContext.Request;

                if (Directory.Exists(Path.GetFullPath("site")))
                {
                    byte[] buffer;
                    if (File.Exists("." + request.RawUrl))
                    {
                        buffer = File.ReadAllBytes("." + request.RawUrl);
                        //получаем поток и записываем в него ответ
                        response.ContentLength64 = buffer.Length;

                        Stream output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);

                        //закрываем поток
                        output.Close();
                    }
                    response.Close();

                }
                else
                {
                    response.Headers.Set("Content-Type", "text/plain");

                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    string err = "404 - not found";

                    byte[] buffer = Encoding.UTF8.GetBytes(err);
                    Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);

                    //закрываем поток
                    output.Close();
                }

                Listening();
            }
        }
        public void Dispose()
        {
            Stop();
        }
    }
}
