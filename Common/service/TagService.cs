using MyTodo.Common.Model;
using MyTodo.Common.service.request;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace MyTodo.Common.service
{
    public class TagService
    {
        private HttpRestClient httpRestClient;

        public TagService()
        {

        }

        public async Task<ResponseData<Tag>> GetTagList(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/api/tag/getTagList");
            ApiResponse<ResponseData<Tag>> response = await httpRestClient.ExcuteAsync<ResponseData<Tag>>(new BaseRequest() { Method = Method.Get, Parameter = param });
            var obj = response.data;
            return response.data;
        }
    }
}
