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
    public class UserService
    {
        private HttpRestClient httpRestClient;

        public UserService()
        {

        }

        public async Task<ApiResponse<UserInfo>> Login(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/login");
            return await httpRestClient.ExcuteAsync<UserInfo>(new BaseRequest() { Method = Method.Post, Parameter = param });
        }

        public async Task<ApiResponse> LogOut(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/logout");
            return await httpRestClient.ExcuteAsync(new BaseRequest() { Method = Method.Post, Parameter = param });
        }

        public async Task<ApiResponse> Register(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/register");
            return await httpRestClient.ExcuteAsync(new BaseRequest() { Method = Method.Post, Parameter = param });
        }
        public async Task<ApiResponse<User>> UdateUser(JsonObject param)
        {
            httpRestClient = new HttpRestClient("/api/user/updateUser");
            return await httpRestClient.ExcuteAsync<User>(new BaseRequest() { Method = Method.Post, Parameter = param });
        }
    }
}
