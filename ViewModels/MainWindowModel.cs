using MyTodo.Common.Extendsions;
using MyTodo.Common.Model;
using MyTodo.Common.service;
using Prism.Commands;
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
using System.Windows.Controls;

namespace MyTodo.ViewModels
{
    public class MainWindowModel:BindableBase, IConfigureService
    {

        public ObservableCollection<Workspace> workSpaceList;

        private WorkSpaceService workSpaceService;

        private readonly IRegionManager regionManager;
        public DelegateCommand<ComboBox> WorkspaceSelectCommand { get; set; }

        public ObservableCollection<Workspace> WorkSpaceList {
            get { return workSpaceList; }
            set { workSpaceList = value; RaisePropertyChanged(); }
        }


        public MainWindowModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
            WorkSpaceList = new ObservableCollection<Workspace>();
            workSpaceService = new WorkSpaceService();
            WorkspaceSelectCommand = new DelegateCommand<ComboBox>(workspaceChage);
            GetWorkSpaceList(1, 10);
        }

        

        public void Configure()
        {
            regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate("IndexView");
        }

        public void workspaceChage(ComboBox comboBox)
        {

        }

        public async void GetWorkSpaceList(int pageNum, int pageSize)
        {
            try
            {
                JsonObject param = new JsonObject();
                param.Add("pageNum", pageNum);
                param.Add("pageSize", pageSize);
                var obj = await workSpaceService.GetWorkSpaceList(param);
                foreach (Workspace item in obj.list)
                {
                    WorkSpaceList.Add(item);
                }
            }
            finally
            {

            }
        }

    }
}
