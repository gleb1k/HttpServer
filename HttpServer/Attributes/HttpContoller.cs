using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Attributes
{
    public class HttpContoller : Attribute
    {
        public string ControllerName { get; set; }
        public HttpContoller(string controllerName)
        {
            ControllerName = controllerName;
        }
    }
}
