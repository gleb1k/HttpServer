using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Reflection;
using System.Text.Json;
using HttpServer.Attributes;
using HttpServer.Controller;

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
                        string err = "<h1>404<h1> <h2>The resource can not be found.<h2>";
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
                // ???
                Console.WriteLine("Возникла ошибка. Сервер остановлен");
                Stop();
            }
        }
        private bool MethodHandler(HttpListenerContext _httpContext)
        {
            // объект запроса
            HttpListenerRequest request = _httpContext.Request;

            // объект ответа
            HttpListenerResponse response = _httpContext.Response;

            if (_httpContext.Request.Url.Segments.Length < 2) return false;

            string controllerName = _httpContext.Request.Url.Segments[1].Replace("/", "");

            string[] strParams = _httpContext.Request.Url
                                    .Segments
                                    .Skip(2)
                                    .Select(s => s.Replace("/", ""))
                                    .ToArray();

            var assembly = Assembly.GetExecutingAssembly();

            var controller = assembly.GetTypes().Where(t => Attribute.IsDefined(t, typeof(HttpController))).FirstOrDefault(c => c.Name.ToLower() == controllerName.ToLower());

            if (controller == null) return false;

            var test = typeof(HttpController).Name;
            var method = controller.GetMethods().Where(t => t.GetCustomAttributes(true)
                                                              .Any(attr => attr.GetType().Name == $"Http{_httpContext.Request.HttpMethod}"))
                                                 .FirstOrDefault();

            if (method == null) return false;

            object[] queryParams = method.GetParameters()
                                .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
                                .ToArray();

            var ret = method.Invoke(Activator.CreateInstance(controller), queryParams);

            response.ContentType = "Application/json";

            byte[] buffer = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(ret));
            response.ContentLength64 = buffer.Length;

            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);

            output.Close();

            return true;
        }
        public void Dispose()
        {
            Stop();
        }
    }
}
