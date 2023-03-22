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
    public class TodoService
    {
        private HttpRestClient httpRestClient;
        public TodoService() { }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="param"></param>
       /// <returns></returns>
        public async Task<ApiResponse<ResponseData<Todo>>> TodoList(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/api/todo/getTodoList");
            ApiResponse<ResponseData<Todo>> response = await httpRestClient.ExcuteAsync<ResponseData<Todo>>(new BaseRequest() { Method = Method.Get, Parameter = param });
            var obj = response.data;
            return response;
        }

        public async Task<ApiResponse> SaveTodo(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/api/todo/addTodo");
            return await httpRestClient.ExcuteAsync(new BaseRequest() { Method = Method.Post, Parameter = param });
        }

        public async Task<ApiResponse> UpdateTodo(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/api/todo/updateTodo");
            return await httpRestClient.ExcuteAsync(new BaseRequest() { Method = Method.Post, Parameter = param });
        }

        public async Task<ApiResponse> DelTodo(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/api/todo/delTodo");
            return await httpRestClient.ExcuteAsync(new BaseRequest() { Method = Method.Delete, Parameter = param });
        }
    }

}
