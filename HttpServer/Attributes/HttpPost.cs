using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Attributes
{
    public class HttpPost: Attribute
    {
        public string MethodURI { get; set; }
        public HttpPost(string methodURI)
        {
            MethodURI = methodURI;
        }
    }
}
