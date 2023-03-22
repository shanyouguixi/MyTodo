using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace MyMemo.Common.service.request
{
    public class BaseRequest
    {
        public Method Method { get; set; }
        public string Url { get; set; } = "http://localhost:8099/";
        public string ContentType { get; set; } = "application/json";
        public List<string> Headers { get; set; }

        public JsonObject Parameter { get; set; }

    }
}
