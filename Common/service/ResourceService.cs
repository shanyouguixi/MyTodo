using MyMemo.Common.service.request;
using MyTodo.Common.Model;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
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


        public async Task<ApiResponse<ResponseData<UserResource>>> GetResourceList(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/api/resource/getResource");
            ApiResponse<ResponseData<UserResource>> response = await httpRestClient.ExcuteAsync<ResponseData<UserResource>>(new BaseRequest() { Method = Method.Get, Parameter = param });
            var obj = response.data;
            return response;
        }

        public async Task<ApiResponse<ResponseData<ResourcesType>>> GetResourceTypeList(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/api/resource/getResourceType");
            ApiResponse<ResponseData<ResourcesType>> response = await httpRestClient.ExcuteAsync<ResponseData<ResourcesType>>(new BaseRequest() { Method = Method.Get, Parameter = param });
            var obj = response.data;
            return response;
        }


        public async Task<ApiResponse> UpdateResource(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/api/resource/updateResource");
            return await httpRestClient.ExcuteAsync(new BaseRequest() { Method = Method.Post, Parameter = param });
        }

        public async Task<ApiResponse> UploadTypeFile(JsonObject param, string[] filePaths)
        {
            httpRestClient = new HttpRestClient("/api/resource/saveResource");
            return await httpRestClient.ExcuteUploadAsync(new BaseRequest() { Method = Method.Post, Parameter = param, ContentType = "multipart/form-data" }, filePaths);
        }

        public async Task<ApiResponse> DelResource(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/api/resource/delResource");
            return await httpRestClient.ExcuteAsync(new BaseRequest() { Method = Method.Delete, Parameter = param });
        }


        public async Task<ApiResponse> UpdateResourceType(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/api/resource/updateResourceType");
            return await httpRestClient.ExcuteAsync(new BaseRequest() { Method = Method.Post, Parameter = param });
        }

        public async Task<ApiResponse> AddResourceType(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/api/resource/addResourceType");
            return await httpRestClient.ExcuteAsync(new BaseRequest() { Method = Method.Post, Parameter = param });
        }
        public async Task<ApiResponse> DelResourceType(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/api/resource/delResourceType");
            return await httpRestClient.ExcuteAsync(new BaseRequest() { Method = Method.Delete, Parameter = param });
        }
    }
}
