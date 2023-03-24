using MyMemo.Common.DialogUtils;
using MyMemo.Common.Extendsions;
using MyMemo.ViewModels;
using MyTodo.Common.Model;
using MyTodo.Common.service;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MyTodo.ViewModels
{
    public class IndexViewModel : NavigationViewModel
    {
        private readonly IDialogHostService dialogHost;
        private readonly IEventAggregator aggregator;
        private ResourceService resourceService;
        private UserService userService;
        private User userInfo;

        private string title;

        private int selectIndex = 0;

        private DelegateCommand<string> toUpdateUserInfoCommon;
        private DelegateCommand updateUserInfoCommon;
        private DelegateCommand uploadFileCommand;


        public IndexViewModel(IEventAggregator aggregator, IContainerProvider provider) : base(provider)
        {
            dialogHost = provider.Resolve<IDialogHostService>();
            this.aggregator = aggregator;
            ToUpdateUserInfoCommon = new DelegateCommand<string>(toUpdateUserInfo);
            UpdateUserInfoCommon = new DelegateCommand(updateUserInof);
            UploadFileCommand = new DelegateCommand(uploadFile);
            resourceService = new ResourceService();
            userService = new UserService();
            this.aggregator.SetFlash("Workspace");
            flashUserInfo();


        }

        private async void uploadFile()
        {

            string filePath = selectFile();
            if(filePath == null)
            {
                aggregator.SendMessage("请选择文件");
                return;
            }
            var apiResponse = await resourceService.UploadFile(null, filePath);
            if(apiResponse.code == 0)
            {
                User user = Application.Current.Properties["user"] as User;
                Resource resource = apiResponse.data;
                user.avatar = resource.fileUrl;
                UserInfo = user;
                
            } else
            {
                aggregator.SendMessage("上传失败");
            }
        }

        private async void updateUserInof()
        {
            SelectIndex = 0;
            JsonObject param = new JsonObject();
            param.Add("userName", UserInfo.userName);
            param.Add("email", UserInfo.email);
            param.Add("avatar", UserInfo.avatar);

            var updateRes = await userService.UdateUser(param);
            if(updateRes.code == 0)
            {                
                UserInfo = updateRes.data;
                Application.Current.Properties["user"] = UserInfo;
                aggregator.SendMessage("更新成功");
                aggregator.SetFlash("UserInfo");
            } else
            {
                aggregator.SendMessage("更新失败");
            }
        }

        private void toUpdateUserInfo(string obj)
        {
            if ("view".Equals(obj))
            {
                SelectIndex = 0;
            }
            else
            {
                SelectIndex = 1;
            }
        }

        public User UserInfo { get => userInfo; set { userInfo = value;RaisePropertyChanged(); } }
        public string Title { get => title; set { title = value; RaisePropertyChanged(); } }

        public int SelectIndex { get => selectIndex; set { selectIndex = value; RaisePropertyChanged(); } }

        public DelegateCommand<string> ToUpdateUserInfoCommon { get => toUpdateUserInfoCommon; set => toUpdateUserInfoCommon = value; }
        public DelegateCommand UpdateUserInfoCommon { get => updateUserInfoCommon; set => updateUserInfoCommon = value; }
        public DelegateCommand UploadFileCommand { get => uploadFileCommand; set => uploadFileCommand = value; }

        private void flashUserInfo()
        {
            User user = Application.Current.Properties["user"] as User;
            if (user != null)
            {
                UserInfo = user;
                Title = $"你好，{DateTime.Now.GetDateTimeFormats('D')[1].ToString()}";
            }
        }


        private string selectFile()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "图片|*.gif;*.jpg;*.jpeg;*.bmp;*.jfif;*.png;"//限制只能选择这几种图片格式
            };
            string file = null;
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                file = openFileDialog.FileName;
            }
            return file;
        }

    }



}
