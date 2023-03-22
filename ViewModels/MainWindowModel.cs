using MyMemo.Common.Extendsions;
using MyMemo.Common.Model;
using MyMemo.Common.service;
using MyMemo.Common.service.request;
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
using System.Windows.Controls;

namespace MyMemo.ViewModels
{
    public class MainWindowModel : BindableBase, IConfigureService
    {

        private readonly IEventAggregator aggregator;
        public ObservableCollection<Workspace> workSpaceList;

        private WorkSpaceService workSpaceService;

        private readonly IRegionManager regionManager;

        private static Workspace selectWorkspace;
        public DelegateCommand<ComboBox> WorkspaceSelectCommand { get; set; }

        public ObservableCollection<Workspace> WorkSpaceList
        {
            get { return workSpaceList; }
            set { workSpaceList = value; RaisePropertyChanged(); }
        }

        public static Workspace SelectWorkspace { get => selectWorkspace; set => selectWorkspace = value; }

        public MainWindowModel(IRegionManager regionManager, IEventAggregator aggregator)
        {
            this.aggregator = aggregator;
            this.regionManager = regionManager;
            WorkSpaceList = new ObservableCollection<Workspace>();
            workSpaceService = new WorkSpaceService();
            WorkspaceSelectCommand = new DelegateCommand<ComboBox>(workspaceChage);
            GetWorkSpaceList();

            aggregator.ResgiterFlash(arg =>
            {
                if ("Workspace".Equals(arg.Name))
                {
                    GetWorkSpaceList();
                }
            });
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
                    aggregator.SendMessage("网络错误");
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
