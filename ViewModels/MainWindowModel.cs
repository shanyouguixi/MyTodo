using MyMemo.Common.DialogUtils;
using MyMemo.Common.Extendsions;
using MyMemo.Common.Model;
using MyMemo.Common.service;
using MyMemo.Common.service.request;
using MyTodo.Common.Cache;
using MyTodo.Common.Model;
using MyTodo.Common.service;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
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

        #region 资源管理相关
        private ResourceService resourceService;
        private int resourcePageNum = 1;
        private int resourcePageSize = 15;
        private int resourceTotal;
        private int resourcePages;
        private int resourceTypeId = 0;
        private int resourceSelectIndex = 0;
        private string resourceFileName;
        private DelegateCommand loadResourceCommand;
        private ObservableCollection<UserResource> userResources;
        private ObservableCollection<ResourcesType> userResourcesType;
        private ResourcesType tempResourceType;
        private DelegateCommand<ListBoxItem> copyResourceCommand;
        private DelegateCommand<ListBoxItem> delResourceCommand;
        private DelegateCommand<Object> selectResourceTypeCommand;
        private DelegateCommand<Object> selectResourceCommand;
        private DelegateCommand<string> resourceTypeExcuteCommand;
        private DelegateCommand resourceExcuteCommand;
        private UserResource tempUserResource;
        #endregion
        public ObservableCollection<Workspace> WorkSpaceList
        {
            get { return workSpaceList; }
            set { workSpaceList = value; RaisePropertyChanged(); }
        }

        public static Workspace SelectWorkspace { get => selectWorkspace; set => selectWorkspace = value; }
        public User UserInfo { get => userInfo; set { userInfo = value; RaisePropertyChanged(); } }

        public DelegateCommand<string> Logout { get => logout; set => logout = value; }



        public int ResourcePageNum { get => resourcePageNum; set => resourcePageNum = value; }
        public int ResourcePageSize { get => resourcePageSize; set => resourcePageSize = value; }
        public int ResourceTotal { get => resourceTotal; set => resourceTotal = value; }
        public int ResourcePages { get => resourcePages; set => resourcePages = value; }
        public int ResourceTypeId { get => resourceTypeId; set => resourceTypeId = value; }
        public int ResourceSelectIndex { get => resourceSelectIndex; set { resourceSelectIndex = value; RaisePropertyChanged(); } }
        public string ResourceFileName { get => resourceFileName; set => resourceFileName = value; }
        public DelegateCommand LoadResourceCommand { get => loadResourceCommand; set => loadResourceCommand = value; }
        public ObservableCollection<UserResource> UserResources { get => userResources; set => userResources = value; }
        public ObservableCollection<ResourcesType> UserResourcesType { get => userResourcesType; set => userResourcesType = value; }
        public ResourcesType TempResourceType { get => tempResourceType; set { tempResourceType = value; RaisePropertyChanged(); } }
        public DelegateCommand<ListBoxItem> CopyResourceCommand { get => copyResourceCommand; set => copyResourceCommand = value; }
        public DelegateCommand<object> SelectResourceTypeCommand { get => selectResourceTypeCommand; set => selectResourceTypeCommand = value; }
        public UserResource TempUserResource { get => tempUserResource; set { tempUserResource = value; RaisePropertyChanged(); } }
        public DelegateCommand<object> SelectResourceCommand { get => selectResourceCommand; set => selectResourceCommand = value; }
        public DelegateCommand<string> ResourceTypeExcuteCommand { get => resourceTypeExcuteCommand; set => resourceTypeExcuteCommand = value; }
        public DelegateCommand ResourceExcuteCommand { get => resourceExcuteCommand; set => resourceExcuteCommand = value; }
        public DelegateCommand<ListBoxItem> DelResourceCommand { get => delResourceCommand; set => delResourceCommand = value; }

        public MainWindowModel(IRegionManager regionManager, IEventAggregator aggregator, IDialogHostService dialogHostService)
        {
            this.dialogHostService = dialogHostService;
            this.aggregator = aggregator;
            this.regionManager = regionManager;
            WorkSpaceList = new ObservableCollection<Workspace>();
            workSpaceService = new WorkSpaceService();
            WorkspaceSelectCommand = new DelegateCommand<ComboBox>(workspaceChage);
            Logout = new DelegateCommand<string>(userLogout);

            resourceService = new ResourceService();
            UserResources = new ObservableCollection<UserResource>();
            UserResourcesType = new ObservableCollection<ResourcesType>();
            LoadResourceCommand = new DelegateCommand(getResource);
            CopyResourceCommand = new DelegateCommand<ListBoxItem>(copyResourName);
            DelResourceCommand = new DelegateCommand<ListBoxItem>(delResourName);
            SelectResourceTypeCommand = new DelegateCommand<object>(selectResourceType);
            SelectResourceCommand = new DelegateCommand<object>(selectResource);
            ResourceTypeExcuteCommand = new DelegateCommand<string>(ResourceTypeExcute);
            ResourceExcuteCommand = new DelegateCommand(UpdateUserResource);



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
                UserInfo = user;
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


        private void selectResourceType(object obj)
        {
            ListView resourceTypes = obj as ListView;
            ResourcesType resourceType = resourceTypes.SelectedItem as ResourcesType;
            if (resourceType != null)
            {
                ResourceSelectIndex = 0;
                TempResourceType = resourceType;
                getResource();
            }
        }

        private void selectResource(object obj)
        {
            ListView resources = obj as ListView;
            UserResource temp = resources.SelectedItem as UserResource;
            if (temp != null)
            {
                TempUserResource = temp;
                ResourceSelectIndex = 1;
            }
        }

        private void copyResourName(ListBoxItem obj)
        {
            if (obj == null)
            {
                return;
            }
            UserResource resource = obj.DataContext as UserResource;
            Clipboard.SetText($"![{resource.fileName}]({resource.url})");
            this.aggregator.SendMessage("图片地址已复制");
        }

        private async void delResourName(ListBoxItem obj)
        {
            UserResource resource = obj.DataContext as UserResource;
            JsonObject param = new JsonObject();
            param.Add("id", resource.id);
            ApiResponse apiResponse = await resourceService.DelResource(param);
            if (apiResponse.code == 0)
            {
                getResource();
                aggregator.SendMessage("删除成功");
            }
            else
            {
                aggregator.SendMessage("删除失败");
            }
        }

        public async void getUserResource()
        {
            JsonObject param = new JsonObject();
            param.Add("pageNum", ResourcePageNum);
            param.Add("pageSize", ResourcePageSize);
            if (TempResourceType != null)
            {
                param.Add("typeId", TempResourceType.id);
            }
            if (!string.IsNullOrWhiteSpace(ResourceFileName))
            {
                param.Add("fileName", ResourceFileName);
            }
            ApiResponse<ResponseData<UserResource>> apiResponse = await resourceService.GetResourceList(param);
            if (apiResponse.code == 0)
            {
                var data = apiResponse.data;
                ResourceTotal = data.total;
                ResourcePages = data.pages;
                var list = data.list;
                UserResources.Clear();
                foreach (UserResource item in list)
                {
                    UserResources.Add(item);
                }
            }
        }
        public async void getUserResourceType()
        {
            JsonObject param = new JsonObject();
            param.Add("pageNum", 1);
            param.Add("pageSize", 10);
            ApiResponse<ResponseData<ResourcesType>> apiResponse = await resourceService.GetResourceTypeList(param);
            if (apiResponse.code == 0)
            {
                var data = apiResponse.data;
                ResourceTotal = data.total;
                ResourcePages = data.pages;
                var list = data.list;
                UserResourcesType.Clear();
                foreach (ResourcesType item in list)
                {
                    UserResourcesType.Add(item);
                }
            }
        }
        public async void UpdateUserResource()
        {
            if (TempUserResource == null)
            {
                return;
            }
            JsonObject param = new JsonObject();
            param.Add("id", TempUserResource.id);
            param.Add("fileName", TempUserResource.fileName);
            ApiResponse apiResponse = await resourceService.UpdateResource(param);
            if (apiResponse.code == 0)
            {
                getResource();
                aggregator.SendMessage("编辑成功");
            }
            else
            {
                aggregator.SendMessage("编辑失败");
            }
        }

        private void ResourceTypeExcute(string obj)
        {
            if ("add".Equals(obj))
            {
                AddUserResourceType();
            }
            else if ("edit".Equals(obj))
            {
                UpdateUserResourceType();

            }
            else if("del".Equals(obj))
            {
                DelUserResourceType();
            } else
            {
                UploadFile();
            }
        }
        public async void UpdateUserResourceType()
        {
            if (TempResourceType == null)
            {
                return;
            }
            JsonObject param = new JsonObject();
            param.Add("id", TempResourceType.id);
            param.Add("typeName", TempResourceType.typeName);
            ApiResponse apiResponse = await resourceService.UpdateResourceType(param);
            if (apiResponse.code == 0)
            {
                getUserResourceType();
                aggregator.SendMessage("编辑成功");
            }
            else
            {
                aggregator.SendMessage("编辑失败");
            }
        }
        public async void AddUserResourceType()
        {

            ApiResponse apiResponse = await resourceService.AddResourceType(null);
            if (apiResponse.code == 0)
            {
                getUserResourceType();
                aggregator.SendMessage("新增成功");
            }
            else
            {
                aggregator.SendMessage("新增失败");
            }
        }

        public async void DelUserResourceType()
        {
            if (TempResourceType == null)
            {
                aggregator.SendMessage("选择分类"); return;
            }
            JsonObject param = new JsonObject();
            param.Add("id", TempResourceType.id);
            ApiResponse apiResponse = await resourceService.DelResourceType(param);
            if (apiResponse.code == 0)
            {
                getUserResourceType();
                aggregator.SendMessage("新增成功");
            }
            else
            {
                aggregator.SendMessage("新增失败");
            }
        }
        public async void UploadFile()
        {
            if (TempResourceType == null)
            {
                aggregator.SendMessage("选择分类"); return;
            }
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "图片|*.gif;*.jpg;*.jpeg;*.bmp;*.jfif;*.png;",//限制只能选择这几种图片格式
                Multiselect = true
               
            };
            string[] fileNames = null;
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                fileNames = openFileDialog.FileNames;
            }

            JsonObject param = new JsonObject();
            param.Add("typeId", TempResourceType.id);
            ApiResponse apiResponse = await resourceService.UploadTypeFile(param, fileNames);
            if (apiResponse.code == 0)
            {
                getUserResourceType();
                aggregator.SendMessage("上传成功");
            }
            else
            {
                aggregator.SendMessage("上传失败");
            }
        }


        public async void getResource()
        {
            getUserResourceType();
            getUserResource();
        }
    }
}
