using MyMemo.Common.DialogUtils;
using MyMemo.Common.Extendsions;
using MyMemo.Common.Model;
using MyMemo.Common.service;
using MyMemo.ViewModels;
using MyTodo.Common.Cache;
using MyTodo.Common.Events;
using MyTodo.Common.Model;
using MyTodo.Common.service;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyTodo.ViewModels
{
    public class IndexViewModel : NavigationViewModel
    {
        private readonly IDialogHostService dialogHost;
        private readonly IEventAggregator aggregator;
        private ResourceService resourceService;
        private UserService userService;
        private TodoService todoService;
        private User userInfo;
        private ObservableCollection<Todo> todoList;


        private Workspace workspaceLocal;

        private string title;

        private int selectIndex = 0;

        private DelegateCommand<string> toUpdateUserInfoCommon;
        private DelegateCommand updateUserInfoCommon;
        private DelegateCommand uploadFileCommand;
        private DelegateCommand flashClockCommand;


        public Workspace WorkspaceLocal { get => workspaceLocal; set => workspaceLocal = value; }
        public ObservableCollection<Todo> TodoList { get => todoList; set { todoList = value; RaisePropertyChanged(); } }

        public IndexViewModel(IEventAggregator aggregator, IContainerProvider provider) : base(provider)
        {
            dialogHost = provider.Resolve<IDialogHostService>();
            this.aggregator = aggregator;
            ToUpdateUserInfoCommon = new DelegateCommand<string>(toUpdateUserInfo);
            UpdateUserInfoCommon = new DelegateCommand(updateUserInof);
            UploadFileCommand = new DelegateCommand(uploadFile);
            FlashClockCommand = new DelegateCommand(getImportantTodoList);
            resourceService = new ResourceService();
            userService = new UserService();
            todoService = new TodoService();
            TodoList = new ObservableCollection<Todo>();

            WorkspaceLocal = MainWindowModel.SelectWorkspace;

            this.aggregator.SetFlash("Workspace");
            aggregator.ResgiterFlash(arg =>
            {
                if ("TodoClock".Equals(arg.Name))
                {
                    getImportantTodoList();
                }
            });
            flashUserInfo();
            getImportantTodoList();
        }



        public User UserInfo { get => userInfo; set { userInfo = value; RaisePropertyChanged(); } }
        public string Title { get => title; set { title = value; RaisePropertyChanged(); } }

        public int SelectIndex { get => selectIndex; set { selectIndex = value; RaisePropertyChanged(); } }

        public DelegateCommand<string> ToUpdateUserInfoCommon { get => toUpdateUserInfoCommon; set => toUpdateUserInfoCommon = value; }
        public DelegateCommand UpdateUserInfoCommon { get => updateUserInfoCommon; set => updateUserInfoCommon = value; }
        public DelegateCommand UploadFileCommand { get => uploadFileCommand; set => uploadFileCommand = value; }
        public DelegateCommand FlashClockCommand { get => flashClockCommand; set => flashClockCommand = value; }

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


        private async void uploadFile()
        {

            string filePath = selectFile();
            if (filePath == null)
            {
                aggregator.SendMessage("请选择文件");
                return;
            }
            var apiResponse = await resourceService.UploadFile(null, filePath);
            if (apiResponse.code == 0)
            {
                User user = Application.Current.Properties["user"] as User;
                Resource resource = apiResponse.data;
                user.avatar = resource.fileUrl;
                UserInfo = user;

            }
            else
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
            if (updateRes.code == 0)
            {
                UserInfo = updateRes.data;
                Application.Current.Properties["user"] = UserInfo;
                string userJson = JsonConvert.SerializeObject(UserInfo);
                CacheManager.IniWritevalue("USERINFO", "user", userJson);
                aggregator.SendMessage("更新成功");
                aggregator.SetFlash("UserInfo");
            }
            else
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



        public async void getImportantTodoList()
        {
            if (WorkspaceLocal == null)
            {
                return;
            }
            try
            {
                JsonObject param = new JsonObject();
                //param.Add("workspaceId", WorkspaceLocal.id);
                param.Add("pageNum", 1);
                param.Add("pageSize", 10);
                var res = await todoService.getImportantTodoList(param);
                var obj = res.data;
                if (res.code != 0)
                {
                    aggregator.SendMessage(res.msg);
                }

                await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    TodoList.Clear();

                    foreach (Todo item in obj.list)
                    {
                        TodoList.Add(item);
                    }

                    if (TodoList.Count > 0 && TodoList[0].remarkDate != null)
                    {
                        Todo temp = TodoList[0];
                        AlarmClock alarmClock = new AlarmClock(this.aggregator, temp.remarkDate, temp.content);
                        alarmClock.SetAlarm();
                    }
                }));

            }
            catch (Exception e)
            {
                aggregator.SendMessage("网络错误");
            }
            finally
            {

            }
        }









    }



}
