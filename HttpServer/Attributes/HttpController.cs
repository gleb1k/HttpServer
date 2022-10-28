using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Attributes
{
    public class HttpController : Attribute
    {
        public string ControllerName { get; set; }
        public HttpController(string controllerName)
        {
            ControllerName = controllerName;
        }
    }
}
