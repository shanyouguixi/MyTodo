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
    public class WorkSpaceService
    {
        private HttpRestClient httpRestClient;

        public WorkSpaceService()
        {

        }

        public async Task<ApiResponse<ResponseData<Workspace>>> GetWorkSpaceList(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/api/workspace/getWorkspaceList");
            ApiResponse<ResponseData<Workspace>> response = await httpRestClient.ExcuteAsync<ResponseData<Workspace>>(new BaseRequest() { Method = Method.Get, Parameter = param });
            var obj = response.data;
            return response;
        }

        public async Task<ApiResponse> SaveWorkspace(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/api/workspace/addWorkspace");
            return await httpRestClient.ExcuteAsync(new BaseRequest() { Method = Method.Post, Parameter = param });
        }

        public async Task<ApiResponse> UpdateWorkspace(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/api/workspace/updateWorkspace");
            return await httpRestClient.ExcuteAsync(new BaseRequest() { Method = Method.Post, Parameter = param });
        }

        public async Task<ApiResponse> DelWorkspace(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/api/workspace/delWorkspace");
            return await httpRestClient.ExcuteAsync(new BaseRequest() { Method = Method.Delete, Parameter = param });
        }
    }
}
