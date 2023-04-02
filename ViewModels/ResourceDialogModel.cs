using MyMemo.Common.Extendsions;
using MyMemo.Common.service.request;
using MyMemo.ViewModels;
using MyTodo.Common.Model;
using MyTodo.Common.service;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MyTodo.ViewModels
{
    public class ResourceDialogModel : NavigationViewModel
    {
        private readonly IEventAggregator aggregator;
        private ResourceService resourceService;
        private int resourcePageNum = 1;
        private int resourcePageSize = 10;
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
        private DelegateCommand<Object> selectResourceTypeCommand;
        public ResourceDialogModel(IEventAggregator aggregator, IContainerProvider provider) : base(provider)
        {
            this.aggregator = aggregator;
            resourceService = new ResourceService();
            UserResources = new ObservableCollection<UserResource>();
            UserResourcesType = new ObservableCollection<ResourcesType>();
            LoadResourceCommand = new DelegateCommand(getResource);
            CopyResourceCommand = new DelegateCommand<ListBoxItem>(copyResourName);
            SelectResourceTypeCommand = new DelegateCommand<object>(selectResourceType);
        }

        public int ResourcePageNum { get => resourcePageNum; set => resourcePageNum = value; }
        public int ResourcePageSize { get => resourcePageSize; set => resourcePageSize = value; }
        public int ResourceTotal { get => resourceTotal; set => resourceTotal = value; }
        public int ResourcePages { get => resourcePages; set => resourcePages = value; }
        public int ResourceTypeId { get => resourceTypeId; set => resourceTypeId = value; }
        public int ResourceSelectIndex { get => resourceSelectIndex; set => resourceSelectIndex = value; }
        public string ResourceFileName { get => resourceFileName; set => resourceFileName = value; }
        public DelegateCommand LoadResourceCommand { get => loadResourceCommand; set => loadResourceCommand = value; }
        public ObservableCollection<UserResource> UserResources { get => userResources; set => userResources = value; }
        public ObservableCollection<ResourcesType> UserResourcesType { get => userResourcesType; set => userResourcesType = value; }
        public ResourcesType TempResourceType { get => tempResourceType; set => tempResourceType = value; }
        public DelegateCommand<ListBoxItem> CopyResourceCommand { get => copyResourceCommand; set => copyResourceCommand = value; }
        public DelegateCommand<object> SelectResourceTypeCommand { get => selectResourceTypeCommand; set => selectResourceTypeCommand = value; }



        private void selectResourceType(object obj)
        {
            ResourcesType resourceType = obj as ResourcesType;
            if (resourceType != null)
            {
                TempResourceType = resourceType;
                getResource();
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
            aggregator.SendMessage("图片地址已复制");
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


        public async void getResource()
        {
            getUserResourceType();
            getUserResource();
        }
    }
}
