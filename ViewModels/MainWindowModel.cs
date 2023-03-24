using MyMemo.Common.DialogUtils;
using MyMemo.Common.Extendsions;
using MyMemo.Common.Model;
using MyMemo.Common.service;
using MyMemo.Common.service.request;
using MyTodo.Common.Cache;
using MyTodo.Common.Model;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MyMemo.ViewModels
{
    public class MainWindowModel : BindableBase, IConfigureService
    {

        private readonly IEventAggregator aggregator;
        public ObservableCollection<Workspace> workSpaceList;

        private WorkSpaceService workSpaceService;
        private IDialogHostService dialogHostService;
        private User userInfo;

        private readonly IRegionManager regionManager;

        private static Workspace selectWorkspace;
        public DelegateCommand<ComboBox> WorkspaceSelectCommand { get; set; }

        private DelegateCommand<string> logout;



        public ObservableCollection<Workspace> WorkSpaceList
        {
            get { return workSpaceList; }
            set { workSpaceList = value; RaisePropertyChanged(); }
        }

        public static Workspace SelectWorkspace { get => selectWorkspace; set => selectWorkspace = value; }
        public User UserInfo { get => userInfo; set { userInfo = value; RaisePropertyChanged(); } }

        public DelegateCommand<string> Logout { get => logout; set => logout = value; }

        public MainWindowModel(IRegionManager regionManager, IEventAggregator aggregator, IDialogHostService dialogHostService)
        {
            this.dialogHostService = dialogHostService;
            this.aggregator = aggregator;
            this.regionManager = regionManager;
            WorkSpaceList = new ObservableCollection<Workspace>();
            workSpaceService = new WorkSpaceService();
            WorkspaceSelectCommand = new DelegateCommand<ComboBox>(workspaceChage);
            Logout = new DelegateCommand<string>(userLogout);
            aggregator.ResgiterFlash(arg =>
            {
                if ("Workspace".Equals(arg.Name))
                {
                    GetWorkSpaceList();
                }
            });
            aggregator.ResgiterFlash(arg =>
            {
                if ("UserInfo".Equals(arg.Name))
                {
                    flashUserInfo();
                }
            });
            readuserInfo();
        }

        private async void userLogout(string arg)
        {
            if ("exit".Equals(arg))
            {
                var dialogResult = await dialogHostService.Question("温馨提示", "确认退出系统?");
                if (dialogResult.Result != Prism.Services.Dialogs.ButtonResult.OK) return;
                Application.Current.Shutdown();
            }
            else
            {
                CacheManager.DeleteSection("USERINFO");
                clearUserInfo();
                Application.Current.Shutdown();
            }
            
        }


        private void readuserInfo()
        {
            string userString = CacheManager.IniReadvalue("USERINFO", "user");
            string tokenString = CacheManager.IniReadvalue("USERINFO", "JSESSIONID");
            if (!string.IsNullOrWhiteSpace(userString) && !string.IsNullOrWhiteSpace(tokenString))
            {
                User user = JsonConvert.DeserializeObject<User>(userString);
                Application.Current.Properties["JSESSIONID"] = tokenString;
                Application.Current.Properties["user"] = user;
                aggregator.SetFlash("UserInfo");
                GetWorkSpaceList();                
            }

        }

        private void flashUserInfo()
        {
            User user = Application.Current.Properties["user"] as User;
            if (user != null)
            {
                UserInfo= user;
            }
        }

        private void clearUserInfo()
        {
            Application.Current.Properties["JSESSIONID"] = null;
            Application.Current.Properties["user"] = null;
            aggregator.SetFlash("UserInfo");
        }

        public void Configure()
        {
            regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate("IndexView");
        }

        public void workspaceChage(ComboBox comboBox)
        {
            SelectWorkspace = (Workspace)comboBox.SelectedItem;
            aggregator.SetWorkspace(SelectWorkspace);
        }




        public async void GetWorkSpaceList()
        {
            try
            {
                JsonObject param = new JsonObject();
                param.Add("pageNum", 1);
                param.Add("pageSize", 100);
                var res = await workSpaceService.GetWorkSpaceList(param);
                if (res.code != 0)
                {
                    aggregator.SendMessage(res.msg);
                    return;
                }
                WorkSpaceList.Clear();
                var obj = res.data;
                foreach (Workspace item in obj.list)
                {
                    WorkSpaceList.Add(item);
                }
                aggregator.SetFlash("GlogalWorkspace");
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
