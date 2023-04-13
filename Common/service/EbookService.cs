using MyMemo.Common.Model;
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
    public class EbookService
    {
        public EbookService() { }
        private HttpRestClient httpRestClient;

        public async Task<ApiResponse<ResponseData<Ebook>>> GetEbook(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/api/ebook/getEbook");
            ApiResponse<ResponseData<Ebook>> response = await httpRestClient.ExcuteAsync<ResponseData<Ebook>>(new BaseRequest() { Method = Method.Get, Parameter = param });
            var obj = response.data;
            return response;
        }


        public async Task<ApiResponse> SaveEbook(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/api/ebook/saveEbook");
            return await httpRestClient.ExcuteAsync(new BaseRequest() { Method = Method.Post, Parameter = param });

        }

        public async Task<ApiResponse> UpdateEbook(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/api/ebook/updateEbook");
            return await httpRestClient.ExcuteAsync(new BaseRequest() { Method = Method.Post, Parameter = param });
        }

        public async Task<ApiResponse> DelEbook(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/api/ebook/delEbook");
            return await httpRestClient.ExcuteAsync(new BaseRequest() { Method = Method.Delete, Parameter = param });
        }




        public async Task<ApiResponse<ResponseData<EbookTag>>> GetEbookTag(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/api/ebook/getEbookTag");
            ApiResponse<ResponseData<EbookTag>> response = await httpRestClient.ExcuteAsync<ResponseData<EbookTag>>(new BaseRequest() { Method = Method.Get, Parameter = param });
            var obj = response.data;
            return response;
        }


        public async Task<ApiResponse> SaveEbookTag(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/api/ebook/saveEbookTag");
            return await httpRestClient.ExcuteAsync(new BaseRequest() { Method = Method.Post, Parameter = param });

        }

        public async Task<ApiResponse> UpdateEbookTag(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/api/ebook/updateEbookTag");
            return await httpRestClient.ExcuteAsync(new BaseRequest() { Method = Method.Post, Parameter = param });
        }

        public async Task<ApiResponse> DelEbookTag(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/api/ebook/delEbookTag");
            return await httpRestClient.ExcuteAsync(new BaseRequest() { Method = Method.Delete, Parameter = param });
        }

    }
}
