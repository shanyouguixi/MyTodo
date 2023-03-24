using MyMemo.Common.service.request;
using MyTodo.Common.Model;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace MyTodo.Common.service
{
    public class ResourceService
    {
        private HttpRestClient httpRestClient;
        public ResourceService() { }

        public async Task<ApiResponse<Resource>> UploadFile(JsonObject param,string filePath)
        {
            httpRestClient = new HttpRestClient("/api/resource/upload");
            return await httpRestClient.UplodFile<Resource>(new BaseRequest() { Method = Method.Post, Parameter = param ,ContentType= "multipart/form-data" },filePath);
        }
    }
}
