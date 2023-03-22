using MyMemo.Common.Model;
using MyMemo.Common.service.request;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace MyMemo.Common.service
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

        public async Task<ApiResponse> SaveTag(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/api/tag/addTag");
            return await httpRestClient.ExcuteAsync(new BaseRequest() { Method = Method.Post, Parameter = param });

        }

        public async Task<ApiResponse> UpdateTag(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/api/tag/updateTag");
            return await httpRestClient.ExcuteAsync(new BaseRequest() { Method = Method.Post, Parameter = param });
        }

        public async Task<ApiResponse> DelTag(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/api/tag/delTag");
            return await httpRestClient.ExcuteAsync(new BaseRequest() { Method = Method.Delete, Parameter = param });
        }
    }
}
