using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HttpServer
{
    public class Files
    {
        public static byte[] GetFile(string rawUrl)
        {
            byte[] buffer = null;
            var settings = ServerSettings.Deserialize();
            var filePath = settings.Path + rawUrl;

            if (Directory.Exists(filePath))
            {
                //Каталог
                filePath = filePath + "/index.html";
                if (File.Exists(filePath))
                {
                    buffer = File.ReadAllBytes(filePath);
                }
            }
            else if (File.Exists(filePath))
            {
                //Файл
                buffer = File.ReadAllBytes(filePath);
            }
            return buffer;    
        }
    }
}
