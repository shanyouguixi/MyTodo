using MyTodo.Common.Model;
using MyTodo.Common.service.request;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace MyTodo.Common.service
{
    public class MemoService
    {
        private HttpRestClient httpRestClient;
       
        public MemoService()
        {
            
        }

        /// <summary>
        /// 查询备忘录
        /// </summary>
        /// <param name="">int pageSize, int pageNum, int? tagId,string? searchWord</param>
        /// <returns></returns>
        public async Task<ResponseData<Memo>> MemoList(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/api/memo/getMemoList");
            ApiResponse<ResponseData<Memo>> response = await httpRestClient.ExcuteAsync<ResponseData<Memo>>(new BaseRequest() { Method = Method.Get, Parameter = param });
            var obj = response.data;
            return response.data;
        }

        public async Task<ApiResponse> SaveMemo(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/api/memo/addMemo");
            return await httpRestClient.ExcuteAsync(new BaseRequest() { Method = Method.Post, Parameter = param });

        }

        public async Task<ApiResponse> UpdateMemo(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/api/memo/updateMemo");
            return await httpRestClient.ExcuteAsync(new BaseRequest() { Method = Method.Post, Parameter = param });
        }
    }
}
