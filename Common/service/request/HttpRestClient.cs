using MyMemo.Common.Model;
using MyTodo.Common.Model;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;

namespace MyMemo.Common.service.request
{
    public class HttpRestClient
    {
        private string apiUrl;
        private string JSESSIONID;

        protected readonly RestClient restClient;

        public HttpRestClient(string apiUrl)
        {
            this.apiUrl = apiUrl;
            restClient = new RestClient();
            JSESSIONID = Application.Current.Properties["JSESSIONID"] as string;

        }


        public async Task<ApiResponse<T>> ExcuteAsync<T>(BaseRequest baseRequest)
        {
            RestRequest request = new RestRequest();
            request.AddHeader("Content-type", baseRequest.ContentType);
            request.Method = baseRequest.Method;
            if (!string.IsNullOrEmpty(JSESSIONID))
            {
                request.AddHeader("Authorization", JSESSIONID);
            }
            if (baseRequest.Parameter != null)
            {
                if (baseRequest.Method == Method.Get)
                {
                    string paramStr = ObjectToGetParam(baseRequest.Parameter);
                    apiUrl += paramStr;
                }
                else
                {
                    request.AddBody(baseRequest.Parameter);
                }
            }
            restClient.Options.BaseUrl = new Uri(baseRequest.Url + apiUrl);
            RestResponse response = await restClient.ExecuteAsync(request);
            if (response == null)
            {
                return new ApiResponse<T>()
                {
                    code = 1,
                    msg = "请求失败"
                };
            }
            ApiResponse<T> apiResponse = null;
            try
            {
                apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(response.Content);
            }
            catch (Exception ex)
            {
                return new ApiResponse<T>()
                {
                    code = 1,
                    msg = "请求失败"
                };
            }
            if (response == null || response.StatusCode != HttpStatusCode.OK)
            {
                MessageBox.Show("请求失败");
                return new ApiResponse<T>()
                {
                    code = 1,
                    msg = "请求失败"
                };
            }
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (apiResponse.code == 886)
                {
                    return new ApiResponse<T>()
                    {
                        code = 886,
                        msg = "请求失败"
                    };
                }
                else if (apiResponse.code == 1000000)
                {
                    return new ApiResponse<T>()
                    {
                        code = 1000000,
                        msg = "请登录"
                    };
                }
                else
                {
                    return apiResponse;
                }
            }
            else
            {
                return new ApiResponse<T>()
                {
                    code = 1,
                    msg = "请求失败"
                };
            }
        }

        public async Task<ApiResponse> ExcuteAsync(BaseRequest baseRequest)
        {
            RestRequest request = new RestRequest();
            request.AddHeader("Content-type", baseRequest.ContentType);
            request.Method = baseRequest.Method;
            if (!string.IsNullOrEmpty(JSESSIONID))
            {
                request.AddHeader("Authorization", JSESSIONID);
            }
            if (baseRequest.Parameter != null)
            {
                if (baseRequest.Method == Method.Get || baseRequest.Method == Method.Delete)
                {
                    string paramStr = ObjectToGetParam(baseRequest.Parameter);
                    apiUrl += paramStr;
                }
                else
                {
                    request.AddBody(baseRequest.Parameter);
                }
            }
            restClient.Options.BaseUrl = new Uri(baseRequest.Url + apiUrl);
            RestResponse response = await restClient.ExecuteAsync(request);
            if (response == null || response.StatusCode != HttpStatusCode.OK)
            {
                MessageBox.Show("请求失败");
                return new ApiResponse()
                {
                    code = 1,
                    msg = "请求失败"
                };
            }
            ApiResponse apiResponse = null;
            try
            {
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(response.Content);
            }
            catch (Exception ex)
            {
                return new ApiResponse()
                {
                    code = 1,
                    msg = "请求失败"
                };

            }
            if (apiResponse == null)
            {
                return new ApiResponse()
                {
                    code = 1,
                    msg = "请求失败"
                };
            }
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (apiResponse.code == 886)
                {
                    return new ApiResponse()
                    {
                        code = 886,
                        msg = "请求失败"
                    };
                }
                else if (apiResponse.code == 1000000)
                {
                    MessageBox.Show("请登录");
                    return new ApiResponse()
                    {
                        code = 1000000,
                        msg = "请登录"
                    };
                }
                else
                {
                    return apiResponse;
                }
            }
            else
            {
                MessageBox.Show("请求失败");
                return new ApiResponse()
                {
                    code = 1,
                    msg = "请求失败"
                };
            }
        }

        public async Task<ApiResponse> ExcuteUploadAsync(BaseRequest baseRequest, string[] filePaths)
        {
            RestRequest request = new RestRequest();
            request.AddHeader("Content-type", baseRequest.ContentType);
            request.Method = baseRequest.Method;
            if (!string.IsNullOrEmpty(JSESSIONID))
            {
                request.AddHeader("Authorization", JSESSIONID);
            }
            request.AddHeader("Content-Type", "multipart/form-data");
            foreach (string filePath in filePaths)
            {
                request.AddFile("file", filePath);
            }
            if (baseRequest.Parameter != null)
            {
                string paramStr = ObjectToGetParam(baseRequest.Parameter);
                apiUrl += paramStr;
            }
            restClient.Options.BaseUrl = new Uri(baseRequest.Url + apiUrl);
            RestResponse response = await restClient.ExecuteAsync(request);
            if (response == null || response.StatusCode != HttpStatusCode.OK)
            {
                MessageBox.Show("请求失败");
                return new ApiResponse()
                {
                    code = 1,
                    msg = "请求失败"
                };
            }
            ApiResponse apiResponse = null;
            try
            {
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(response.Content);
            }
            catch (Exception ex)
            {
                return new ApiResponse()
                {
                    code = 1,
                    msg = "请求失败"
                };

            }
            if (apiResponse == null)
            {
                return new ApiResponse()
                {
                    code = 1,
                    msg = "请求失败"
                };
            }
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (apiResponse.code == 886)
                {
                    return new ApiResponse()
                    {
                        code = 886,
                        msg = "请求失败"
                    };
                }
                else if (apiResponse.code == 1000000)
                {
                    MessageBox.Show("请登录");
                    return new ApiResponse()
                    {
                        code = 1000000,
                        msg = "请登录"
                    };
                }
                else
                {
                    return apiResponse;
                }
            }
            else
            {
                MessageBox.Show("请求失败");
                return new ApiResponse()
                {
                    code = 1,
                    msg = "请求失败"
                };
            }
        }


        public async Task<ApiResponse<T>> UplodFile<T>(BaseRequest baseRequest, string filePath)
        {
            RestRequest request = new RestRequest();
            request.AddHeader("Content-type", baseRequest.ContentType);
            request.Method = baseRequest.Method;
            if (!string.IsNullOrEmpty(JSESSIONID))
            {
                request.AddHeader("Authorization", JSESSIONID);
            }
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddFile("file", filePath);
            if (baseRequest.Parameter != null)
            {
                request.AddBody(baseRequest.Parameter);
            }
            restClient.Options.BaseUrl = new Uri(baseRequest.Url + apiUrl);
            RestResponse response = await restClient.ExecuteAsync(request);
            if (response == null || response.StatusCode != HttpStatusCode.OK)
            {
                MessageBox.Show("请求失败");
                return new ApiResponse<T>()
                {
                    code = 1,
                    msg = "请求失败"
                };
            }
            ApiResponse<T> apiResponse = null;
            try
            {
                apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(response.Content);
            }
            catch (Exception ex)
            {
                return new ApiResponse<T>()
                {
                    code = 1,
                    msg = "请求失败"
                };

            }
            if (apiResponse == null)
            {
                return new ApiResponse<T>()
                {
                    code = 1,
                    msg = "请求失败"
                };
            }
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (apiResponse.code == 886)
                {
                    return new ApiResponse<T>()
                    {
                        code = 886,
                        msg = "请求失败"
                    };
                }
                else if (apiResponse.code == 1000000)
                {
                    MessageBox.Show("请登录");
                    return new ApiResponse<T>()
                    {
                        code = 1000000,
                        msg = "请登录"
                    };
                }
                else
                {
                    return apiResponse;
                }
            }
            else
            {
                MessageBox.Show("请求失败");
                return new ApiResponse<T>()
                {
                    code = 1,
                    msg = "请求失败"
                };
            }
        }


        public static string ObjectToGetParam(JsonObject param)
        {
            StringBuilder strBui = new StringBuilder();

            foreach (var item in param)
            {
                object value = item.Value;
                if (value != null)
                {

                    if (strBui.Length < 1)
                    {
                        strBui.Append("?");
                    }
                    else
                    {
                        strBui.Append("&");
                    }
                    string temp = string.Format("{0}={1}", item.Key, item.Value);
                    strBui.Append(temp);
                }
            }
            return strBui.ToString();
        }
    }



}
