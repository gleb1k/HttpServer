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
using Azure;
using System.Net.Http;
using HttpServer.Server;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;

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
        //todo
        //public HttpServer(string str)
        //{
        //    _serverSettings = ServerSettings.Deserialize();
        //    _httpListener = new HttpListener();
        //    var context = _httpListener.GetContext();
        //    context.Response.Headers.Add("@#!elephant=&.ha-ha", str);
        //}
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

        private async void Listening()
        {
            while (_httpListener.IsListening)
            {
                //_httpListener.BeginGetContext(new AsyncCallback(ListenerCallback), _httpListener);
                var context = await _httpListener.GetContextAsync();                

                if (MethodHandler(context)) return;


                StaticFiles(context.Response, context.Request);

                Kr(context);
            }

        }
        private void Kr(HttpListenerContext _httpContext)
        {
            // объект запроса
            HttpListenerRequest request = _httpContext.Request;
            // объект ответа
            HttpListenerResponse response = _httpContext.Response;

            var headers = request.Headers;
            //@#!elephant=&.ha-ha  -  НЕ РАБОТАЕТ, ИНВАЛИДНЫЕ ЗНАЧЕНИЯ
            headers["test"] = "абв"; //Добавляем значение в headers

            IEnumerable<string> headerValues = request.Headers.GetValues("test");
            var headerValue = headerValues.FirstOrDefault();
            var charArr = headerValue.ToCharArray();

            var dict = MakeDict();

            var newWordList = new List<string>();

            for (int i=0; i< charArr.Length; i++)
            {
                if (dict.ContainsKey(charArr[i]))
                {
                    string value;
                    bool hasValue = dict.TryGetValue(charArr[i], out value);
                    if (hasValue)
                    {
                        newWordList.Add(value);
                    }
                    else
                    {
                        // do something when the value is not there
                    }
                }
            }
            var newWord = string.Join("", newWordList);

            newWord = Cesar(newWord, dict, 3);

            headers.Add("resultMyVoid", newWord);

        }
        private string Cesar(string str,Dictionary<char, string> dict, int bias)
        {
            int nomer; // Номер в алфавите
            int d; // Смещение
            string s; //Результат
            int j; // Переменная для циклов

            var massage = str.ToCharArray().Select(c => c.ToString()).ToArray(); ; // Превращаем строку в массив символов.

            var temp = new List<string>();
            foreach (var item in dict)
            {
                temp.Add(item.Value);
            }
            var alfavit = temp.ToArray();

            //char[] alfavit = { 'а', 'б', 'в', 'г', 'д', 'е', 'ё', 'ж', 'з', 'и', 'й', 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ъ', 'ы', 'ь', 'э', 'ю', 'я' };

            // Перебираем каждый символ сообщения
            for (int i = 0; i < massage.Length; i++)
            {
                // Ищем индекс буквы
                for (j = 0; j < alfavit.Length; j++)
                {
                    if (massage[i] == alfavit[j])
                    {
                        break;
                    }
                }

                if (j != 33) // Если j равно 33, значит символ не из алфавита
                {
                    nomer = j; // Индекс буквы
                    d = nomer + bias; // Делаем смещение

                    // Проверяем, чтобы не вышли за пределы алфавита
                    if (d > 32)
                    {
                        d = d - 33;
                    }

                    massage[i] = alfavit[d]; // Меняем букву
                }
            }
            s = string.Join("", massage); // Собираем символы обратно в строку.
            return s;
        }
        private Dictionary<char,string> MakeDict()
        {
            var dictionary = new Dictionary<char, string>();
            dictionary.Add('а', "a");
            dictionary.Add('б', "b");
            dictionary.Add('в', "v");
            dictionary.Add('г', "g");
            dictionary.Add('д', "d");

            dictionary.Add('е', "e");
            dictionary.Add('ё', "yo");
            dictionary.Add('ж', "j");
            dictionary.Add('з', "z");
            dictionary.Add('и', "i");

            dictionary.Add('й', "j");
            dictionary.Add('к', "k");
            dictionary.Add('л', "l");
            dictionary.Add('м', "m");
            dictionary.Add('н', "n");

            dictionary.Add('о', "o");
            dictionary.Add('п', "p");
            dictionary.Add('р', "r");
            dictionary.Add('с', "s");
            dictionary.Add('т', "t");

            dictionary.Add('у', "u");
            dictionary.Add('ф', "f");
            dictionary.Add('х', "h");
            dictionary.Add('ц', "ts");
            dictionary.Add('ч', "ch");

            dictionary.Add('ш', "sh");
            dictionary.Add('щ', "csh");
            dictionary.Add('ъ', "'");
            dictionary.Add('ы', "bI");
            dictionary.Add('ь', "'");

            dictionary.Add('э', "e");
            dictionary.Add('ю', "yu");
            dictionary.Add('я', "ya");
            return dictionary;
        }
        private void StaticFiles(HttpListenerResponse response, HttpListenerRequest request)
        {
            try
            {
                byte[] buffer;

                var rawurl = request.RawUrl;

                buffer = Files.GetFile(rawurl.Replace("%20", " "));

                //Задаю расширения для файлов
                Files.GetExtension(ref response, "." + rawurl);

                //Неправильно задан запрос / не найдена папка
                if (buffer != null)
                {
                    Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    //закрываем поток
                    output.Close();
                }
                else
                    Show404(ref response, ref buffer);
                Listening();
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

            //пустой url
            if (_httpContext.Request.Url.Segments.Length < 2) return false;

            string controllerName = _httpContext.Request.Url.Segments[1].Replace("/", "");

            var assembly = Assembly.GetExecutingAssembly();

            // ищет контроллер accounts
            var controller = assembly.GetTypes().Where(t => Attribute.IsDefined(t, typeof(HttpController))).FirstOrDefault(c => c.Name.ToLower() == controllerName.ToLower());

            if (controller == null) return false;

            string[] strParams = _httpContext.Request.Url
                                    .Segments
                                    .Skip(2)
                                    .Select(s => s.Replace("/", ""))
                                    .ToArray();

            if (strParams.Length == 0) return false;

            string methodURI = strParams[0];

            var methods = controller.GetMethods().Where(t => t.GetCustomAttributes(true)
                                                              .Any(attr => attr.GetType().Name == $"Http{_httpContext.Request.HttpMethod}"));

            var method = methods.FirstOrDefault(x => _httpContext.Request.HttpMethod switch
            {
                "GET" => x.GetCustomAttribute<HttpGET>()?.MethodURI == methodURI,
                "POST" => x.GetCustomAttribute<HttpPOST>()?.MethodURI == methodURI
            });

            object[] queryParams = null;

            object[] queryParams2 = strParams.Skip(1).Select(x => (object)x).ToArray();

            if (_httpContext.Request.HttpMethod.Equals(HttpReqests.GET))
            {
                //GET
                queryParams = strParams.Skip(1).Select(x => (object)x).ToArray();

                var result = method.Invoke(Activator.CreateInstance(controller), queryParams2);

                response.ContentType = "Application/json";

                byte[] buffer = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(result));
                response.ContentLength64 = buffer.Length;

                Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);

                output.Close();
            }
            else
            {
                //POST
                queryParams = GetRequestData(request);



                response.Headers.Set("Content-Type", "text/html");
                response.StatusCode = 201;
                response.ContentEncoding = Encoding.UTF8;

                string message = $"<h1>201<h1> <h2>{queryParams[0]} {queryParams[1]}<h2>";
                var buffer = Encoding.UTF8.GetBytes(message);

                Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);

                output.Close();
            }
            //switch (methodURI)
            //{
            //    case "getaccounts":
            //        //параметров нет
            //        break;
            //    case "getaccountbyid":
            //        object[] temp = new object[1] { Convert.ToInt32(strParams[1]) };
            //        queryParams = temp;
            //        break;
            //    case "saveaccount":
            //        //колхоз, как красиво написать?? (чтобы не переименовывать переменную)
            //        object[] temp1 = GetRequestData(request);
            //        queryParams = temp1;
            //        break;
            //}

            Listening();

            return true;
        }
        //прием данных с полей логин и пароль (ретурнуть словарь)
        public string[] GetRequestData(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                Console.WriteLine("No client data was sent with the request.");
                return null;
            }
            System.IO.Stream body = request.InputStream;
            System.Text.Encoding encoding = request.ContentEncoding;
            System.IO.StreamReader reader = new System.IO.StreamReader(body, encoding);
            string s = reader.ReadToEnd();
            Console.WriteLine(s);
            body.Close();
            reader.Close();

            var charLogin = s.ToCharArray().Skip(6).TakeWhile(item => item != '&').ToArray();
            string login = new string(charLogin);

            var charPassword = s.SkipWhile(item => item != '&').Skip(10).ToArray();
            string password = new string(charPassword);

            string[] strParams = new string[] { login, password };
            return strParams;
        }

        //Выводит ошибку
        private void Show404(ref HttpListenerResponse response, ref byte[] buffer)
        {
            response.Headers.Set("Content-Type", "text/html");
            response.StatusCode = 404;
            response.ContentEncoding = Encoding.UTF8;
            string err = "<h1>404<h1> <h2>The resource can not be found.<h2>";
            buffer = Encoding.UTF8.GetBytes(err);
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            //закрываем поток
            output.Close();
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
