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
    public class WorkSpaceService
    {
        private HttpRestClient httpRestClient;

        public WorkSpaceService()
        {

        }

        public async Task<ResponseData<Workspace>> GetWorkSpaceList(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/api/workspace/getWorkspaceList");
            ApiResponse<ResponseData<Workspace>> response = await httpRestClient.ExcuteAsync<ResponseData<Workspace>>(new BaseRequest() { Method = Method.Get, Parameter = param });
            var obj = response.data;
            return response.data;
        }
    }
}
