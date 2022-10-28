using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Attributes
{
    public class HttpGET : Attribute
    {
        public string MethodURI { get; set; }
        public HttpGET(string methodURI)
        {
            MethodURI = methodURI;
        }
    }
}
