using MyMemo.Common.Extendsions;
using MyMemo.Common.service.request;
using MyMemo.Common.service;
using MyTodo.Common.service;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using MyTodo.Common.Model;
using System.Windows;
using MyTodo.Common.Cache;
using Newtonsoft.Json;

namespace MyTodo.ViewModels
{
    public class LoginViewModel : BindableBase, IDialogAware
    {


        private readonly IEventAggregator aggregator;
        private UserService userService;

        private UserDto userDto;
        public string Title { get; set; } = "ToDo";
        private string userName;
        private string password;
        private int selectIndex = 0;

        public DelegateCommand<string> ExecuteCommand { get; private set; }
        public LoginViewModel(IEventAggregator aggregator)
        {
            this.aggregator = aggregator;
            ExecuteCommand = new DelegateCommand<string>(Execute);
            userService = new UserService();
            UserDto = new UserDto();
        }





        public string UserName
        {
            get { return userName; }
            set { userName = value; RaisePropertyChanged(); }
        }
        public string Password
        {
            get { return password; }
            set { password = value; RaisePropertyChanged(); }
        }

        public int SelectIndex { get => selectIndex; set { selectIndex = value; RaisePropertyChanged(); } }

        public UserDto UserDto { get => userDto; set { userDto = value; RaisePropertyChanged(); } }

        private void Execute(string obj)
        {
            switch (obj)
            {
                case "Login":
                    Login();
                    break;
                case "LogOut":
                    LogOut();
                    break;
                case "ToRegister":
                    SelectIndex = 1;
                    break;
                case "ToLogin":
                    SelectIndex = 0;
                    break;
                case "Register":
                    Register();
                    break;
            }
        }

        private void LogOut()
        {
            
        }

        private async void Register()
        {
            try
            {
                if (string.IsNullOrEmpty(UserDto.userName) || 
                    string.IsNullOrWhiteSpace(UserDto.password) || 
                    string.IsNullOrWhiteSpace(UserDto.newPassword) || 
                    UserDto.password != UserDto.newPassword)
                {
                    aggregator.SendMessage("请输入正确的账号密码", "Login");
                    return;
                }
                JsonObject param = new JsonObject();
                param.Add("userName", UserDto.userName);
                param.Add("password", UserDto.password);
                param.Add("newPassword", UserDto.newPassword);
                ApiResponse apiResponse = await userService.Register(param);
                if (apiResponse.code == 0)
                {
                    SelectIndex = 0;
                    aggregator.SendMessage(apiResponse.msg, "Login");
                }


            }
            catch (Exception e)
            {
                aggregator.SendMessage("网络错误");
            }
        }

        private async void Login()
        {
            try
            {
                if (string.IsNullOrEmpty(UserName) || string.IsNullOrWhiteSpace(Password))
                {
                    aggregator.SendMessage("请输入正确的账号密码", "Login");
                    MessageBox.Show("请输入正确的账号密码");
                    return;
                }
                JsonObject param = new JsonObject();
                param.Add("userName", UserName);
                param.Add("password", Password);
                ApiResponse<UserInfo> apiResponse = await userService.Login(param);
                if (apiResponse.code == 0)
                {
                    UserInfo userinfo = apiResponse.data;
                    if (userinfo == null)
                    {
                        aggregator.SendMessage("登录失败", "Login");
                        MessageBox.Show("登录失败");
                        return;
                    }
                    string userJson = JsonConvert.SerializeObject(userinfo.user);                    
                    CacheManager.DeleteSection("USERINFO");
                    CacheManager.IniWritevalue("USERINFO", "user", userJson);
                    CacheManager.IniWritevalue("USERINFO", "JSESSIONID", userinfo.JSESSIONID);

                    Application.Current.Properties["JSESSIONID"] = userinfo.JSESSIONID;
                    Application.Current.Properties["user"] = userinfo.user;
                    aggregator.SetFlash("UserInfo");

                    aggregator.SendMessage(apiResponse.msg);
                    RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
                }
                else
                {
                    aggregator.SendMessage(apiResponse.msg, "Login");
                    MessageBox.Show(apiResponse.msg);
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("网络错误");
                aggregator.SendMessage("网络错误");
            }
        }


        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {

        }
    }
}
