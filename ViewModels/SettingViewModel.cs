using MaterialDesignThemes.Wpf;
using MyMemo.Common.DialogUtils;
using MyMemo.Common.Extendsions;
using MyMemo.Common.Model;
using MyMemo.Common.service;
using MyMemo.Common.service.request;
using MyTodo.Common.Model;
using MyTodo.Common.service;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using System;
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
    public class SettingViewModel : NavigationViewModel
    {
        private readonly IEventAggregator aggregator;
        private readonly IDialogHostService dialogHost;
        private TagService tagService;
        public ObservableCollection<Tag> tagList;
        private DelegateCommand<ListBoxItem> tagSelectCommand;
        private DelegateCommand updateTag;
        private DelegateCommand addTagCommand;
        private DelegateCommand<ListBoxItem> delTagCommand;


        private WorkSpaceService workSpaceService;
        private ObservableCollection<Workspace> workspaceList;
        private DelegateCommand<ListBoxItem> workspaceSelectCommand;
        private DelegateCommand updateWorkspace;
        private DelegateCommand addWorkspaceCommand;
        private DelegateCommand<ListBoxItem> delWorkspaceCommand;
        private Workspace tempWorkspace;

        private EbookService ebookService;
        private ObservableCollection<EbookTag> ebookTagList;
        private DelegateCommand<ListBoxItem> ebookTagSelectCommand;
        private DelegateCommand updateEbookTag;
        private DelegateCommand addEbookTagCommand;
        private DelegateCommand<ListBoxItem> ebookDelTagCommand;
        private EbookTag tempEbookTag;



        private int tagNum = 1;
        private int tagSize = 10;
        private int tagTotal = 0;
        private int tagPages = 0;
        public Tag tempTag;



        public ObservableCollection<Tag> TagList
        {
            get { return tagList; }
            set { tagList = value; RaisePropertyChanged(); }
        }

        public DelegateCommand<ListBoxItem> TagSelectCommand { get => tagSelectCommand; set => tagSelectCommand = value; }
        public Tag TempTag { get => tempTag; set { tempTag = value; RaisePropertyChanged(); } }

        public DelegateCommand UpdateTag { get => updateTag; set => updateTag = value; }
        public DelegateCommand<ListBoxItem> DelTagCommand { get => delTagCommand; set => delTagCommand = value; }
        public int TagNum { get => tagNum; set => tagNum = value; }
        public int TagSize { get => tagSize; set => tagSize = value; }
        public int TagTotal { get => tagTotal; set => tagTotal = value; }
        public int TagPages { get => tagPages; set => tagPages = value; }
        public DelegateCommand AddTagCommand { get => addTagCommand; set => addTagCommand = value; }
        public ObservableCollection<Workspace> WorkspaceList { get => workspaceList; set{ workspaceList = value; RaisePropertyChanged(); } }
        public DelegateCommand<ListBoxItem> WorkspaceSelectCommand { get => workspaceSelectCommand; set => workspaceSelectCommand = value; }
        public DelegateCommand UpdateWorkspace { get => updateWorkspace; set => updateWorkspace = value; }
        public DelegateCommand AddWorkspaceCommand { get => addWorkspaceCommand; set => addWorkspaceCommand = value; }
        public DelegateCommand<ListBoxItem> DelWorkspaceCommand { get => delWorkspaceCommand; set => delWorkspaceCommand = value; }
        public Workspace TempWorkspace { get => tempWorkspace; set { tempWorkspace = value; RaisePropertyChanged(); } }

        public ObservableCollection<EbookTag> EbookTagList { get => ebookTagList; set => ebookTagList = value; }
        public DelegateCommand<ListBoxItem> EbookTagSelectCommand { get => ebookTagSelectCommand; set => ebookTagSelectCommand = value; }
        public DelegateCommand UpdateEbookTag { get => updateEbookTag; set => updateEbookTag = value; }
        public DelegateCommand AddEbookTagCommand { get => addEbookTagCommand; set => addEbookTagCommand = value; }
        public DelegateCommand<ListBoxItem> EbookDelTagCommand { get => ebookDelTagCommand; set => ebookDelTagCommand = value; }
        public EbookTag TempEbookTag { get => tempEbookTag; set { tempEbookTag = value; RaisePropertyChanged(); } }

        public SettingViewModel(IEventAggregator aggregator, IContainerProvider provider) : base(provider) {
            this.aggregator = aggregator;
            dialogHost = provider.Resolve<IDialogHostService>();
            tagService = new TagService();
            tagList = new ObservableCollection<Tag>();
            TagSelectCommand = new DelegateCommand<ListBoxItem>(TagSelect);
            DelTagCommand = new DelegateCommand<ListBoxItem>(DelTag);
            UpdateTag = new DelegateCommand(UpdateTagMoel);
            AddTagCommand = new DelegateCommand(AddTagModel);

            WorkspaceList = new ObservableCollection<Workspace>();
            workSpaceService = new WorkSpaceService();
            WorkspaceSelectCommand = new DelegateCommand<ListBoxItem>(WorkspaceSelect);
            DelWorkspaceCommand = new DelegateCommand<ListBoxItem>(DelWorkspace);
            UpdateWorkspace = new DelegateCommand(UpdateWorkspaceMoel);
            AddWorkspaceCommand = new DelegateCommand(AddWorkspaceModel);

            ebookService = new EbookService();
            EbookTagList = new ObservableCollection<EbookTag>();
            EbookTagSelectCommand = new DelegateCommand<ListBoxItem>(EbookTagSelect);
            EbookDelTagCommand = new DelegateCommand<ListBoxItem>(EbookDelWorkspace);
            UpdateEbookTag = new DelegateCommand(updateEbookTagInfo);
            AddEbookTagCommand = new DelegateCommand(addEbookInfoInfo);
            initBaseData();
        }

        public async void initBaseData()
        {
            var task = await getTagList();
            GetWorkSpaceList();
            GetEbookTagList();


        }


        private void AddWorkspaceModel()
        {
            addWorkspaceInfo();
        }

        private void UpdateWorkspaceMoel()
        {
            updateWorkspaceInfo();
        }

        private void WorkspaceSelect(ListBoxItem obj)
        {
            TempWorkspace = obj.DataContext as Workspace;
        }

        private void EbookTagSelect(ListBoxItem obj)
        {
            TempEbookTag = obj.DataContext as EbookTag;
        }




        private void TagSelect(ListBoxItem obj)
        {
            TempTag = obj.DataContext as Tag;
        }

        private void AddTagModel()
        {
            addMemoInfo();
        }

        private async void DelTag(ListBoxItem obj)
        {
           
            try
            {
                Tag tag = obj.DataContext as Tag;
                var dialogResult = await dialogHost.Question("温馨提示", $"确认删除:{tag.name} ?");
                if (dialogResult.Result != Prism.Services.Dialogs.ButtonResult.OK) return;
                JsonObject param = new JsonObject();
                param.Add("id", tag.id);
                ApiResponse apiResponse = await tagService.DelTag(param);
                if (apiResponse.code == 0)
                {
                    aggregator.SendMessage("删除成功");
                    getTagList();
                }
                else
                {
                    aggregator.SendMessage("删除失败");
                }
            }
            catch (Exception e)
            {
                aggregator.SendMessage("删除失败");
            }
            finally
            {
                
            }
        }

        private void UpdateTagMoel()
        {
            updateMemoInfo();
        }

        /// <summary>
        /// 请求标签
        /// </summary>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        public async Task<bool> getTagList()
        {
            try
            {
                JsonObject param = new JsonObject();
                param.Add("pageNum", TagNum);
                param.Add("pageSize", TagSize);
                var obj = await tagService.GetTagList(param);
                if(TagNum == 1)
                {
                    TagList.Clear();
                }
                foreach (Tag item in obj.list)
                {
                    TagList.Add(item);
                }
                return true;
            }
            catch (Exception e)
            {
                aggregator.SendMessage("网络错误");
                return false;
            }
            finally
            {

            }
        }

        private async void updateMemoInfo()
        {
            try
            {

                JsonObject param = new JsonObject();
                param.Add("id", TempTag.id);
                param.Add("name", TempTag.name);
                param.Add("sort", TempTag.sort);
                param.Add("color", TempTag.color);
                ApiResponse apiResponse = await tagService.UpdateTag(param);
                if (apiResponse.code == 0)
                {
                    aggregator.SendMessage(apiResponse.msg);
                    getTagList();
                }
            }
            catch (Exception e)
            {
                aggregator.SendMessage("网络错误");
            }

        }
        private async void addMemoInfo()
        {
            try
            {
                JsonObject param = new JsonObject();
                param.Add("name", "新标签");
                param.Add("sort", "100");
                param.Add("color", "green");
                ApiResponse apiResponse = await tagService.SaveTag(param);
                if (apiResponse.code == 0)
                {
                    aggregator.SendMessage(apiResponse.msg);
                    getTagList();
                }
            }
            catch (Exception e)
            {
                aggregator.SendMessage("网络错误");
            }

        }

        //=======================================
        /// <summary>
        /// 查询工作空净
        /// </summary>
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
                WorkspaceList.Clear();
                var obj = res.data;
                foreach (Workspace item in obj.list)
                {
                    WorkspaceList.Add(item);
                }
            }
            catch (Exception e)
            {
                aggregator.SendMessage("网络错误");
            }
            finally
            {

            }
        }
        /// <summary>
        /// 编辑工作空净
        /// </summary>
        private async void updateWorkspaceInfo()
        {
            try
            {

                JsonObject param = new JsonObject();
                param.Add("id", TempWorkspace.id);
                param.Add("spaceName", TempWorkspace.spaceName);
                ApiResponse apiResponse = await workSpaceService.UpdateWorkspace(param);
                if (apiResponse.code == 0)
                {
                    aggregator.SendMessage(apiResponse.msg);
                    GetWorkSpaceList();
                    aggregator.SetFlash("Workspace");
                }
            }
            catch (Exception e)
            {
                aggregator.SendMessage("网络错误");
            }

        }
        /// <summary>
        /// 添加工作空净
        /// </summary>
        private async void addWorkspaceInfo()
        {
            try
            {
                JsonObject param = new JsonObject();
                param.Add("spaceName", "新工作空净");
                ApiResponse apiResponse = await workSpaceService.SaveWorkspace(param);
                if (apiResponse.code == 0)
                {
                    aggregator.SendMessage(apiResponse.msg);
                    GetWorkSpaceList();
                    aggregator.SetFlash("Workspace");
                }
            }
            catch (Exception e)
            {
                aggregator.SendMessage("网络错误");
            }

        }

        private async void DelWorkspace(ListBoxItem obj)
        {

            try
            {
                Workspace workspace = obj.DataContext as Workspace;
                var dialogResult = await dialogHost.Question("温馨提示", $"确认删除:{workspace.spaceName} ? 该工作空净下的数据都将会被删除！");
                if (dialogResult.Result != Prism.Services.Dialogs.ButtonResult.OK) return;
                JsonObject param = new JsonObject();
                param.Add("id", workspace.id);
                ApiResponse apiResponse = await workSpaceService.DelWorkspace(param);
                if (apiResponse.code == 0)
                {
                    aggregator.SendMessage("删除成功");
                    GetWorkSpaceList();
                    aggregator.SetFlash("Workspace");
                }
                else
                {
                    aggregator.SendMessage("删除失败");
                }
            }
            catch (Exception e)
            {
                aggregator.SendMessage("删除失败");
            }
            finally
            {

            }
        }



        public async void GetEbookTagList()
        {
            try
            {
   
                var res = await ebookService.GetEbookTag(null);
                if (res.code != 0)
                {
                    aggregator.SendMessage("网络错误");
                    return;
                }
                EbookTagList.Clear();
                var obj = res.data;
                foreach (EbookTag item in obj.list)
                {
                    EbookTagList.Add(item);
                }
            }
            catch (Exception e)
            {
                aggregator.SendMessage("网络错误");
            }
            finally
            {

            }
        }

        private async void updateEbookTagInfo()
        {
            try
            {

                JsonObject param = new JsonObject();
                param.Add("id", TempEbookTag.id);
                param.Add("tagName", TempEbookTag.tagName);
                ApiResponse apiResponse = await ebookService.UpdateEbookTag(param);
                if (apiResponse.code == 0)
                {
                    aggregator.SendMessage(apiResponse.msg);
                    GetEbookTagList();
                    aggregator.SetFlash("EbookTag");
                }
            }
            catch (Exception e)
            {
                aggregator.SendMessage("网络错误");
            }

        }

        private async void addEbookInfoInfo()
        {
            try
            {
                JsonObject param = new JsonObject();
                param.Add("tagName", "新工分类");
                ApiResponse apiResponse = await ebookService.SaveEbookTag(param);
                if (apiResponse.code == 0)
                {
                    aggregator.SendMessage(apiResponse.msg);
                    GetEbookTagList();
                    aggregator.SetFlash("EbookTag");
                }
            }
            catch (Exception e)
            {
                aggregator.SendMessage("网络错误");
            }

        }

        private async void EbookDelWorkspace(ListBoxItem obj)
        {

            try
            {
                EbookTag ebookTag = obj.DataContext as EbookTag;
                var dialogResult = await dialogHost.Question("温馨提示", $"确认删除:{ebookTag.tagName} ? 该工作分类下的数据都将会被删除！");
                if (dialogResult.Result != Prism.Services.Dialogs.ButtonResult.OK) return;
                JsonObject param = new JsonObject();
                param.Add("id", ebookTag.id);
                ApiResponse apiResponse = await ebookService.DelEbookTag(param);
                if (apiResponse.code == 0)
                {
                    aggregator.SendMessage("删除成功");
                    GetEbookTagList();
                    aggregator.SetFlash("EbookTag");
                }
                else
                {
                    aggregator.SendMessage("删除失败");
                }
            }
            catch (Exception e)
            {
                aggregator.SendMessage("删除失败");
            }
            finally
            {

            }
        }
    }
}
