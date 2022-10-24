using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Attributes
{
    public class HttpGet : Attribute
    {
        public string MethodURI { get; set; }
        public HttpGet(string methodURI)
        {
            MethodURI = methodURI;
        }
    }
}
