using MaterialDesignThemes.Wpf;
using Microsoft.Web.WebView2.WinForms;
using MyMemo.Common.service;
using MyMemo.Common.DialogUtils;
using MyMemo.Common.Extendsions;
using MyMemo.Common.Model;
using MyMemo.Common.service;
using MyMemo.Common.service.request;
using MyMemo.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using static System.Net.Mime.MediaTypeNames;
using MyTodo.Common.Util;

namespace MyMemo.ViewModels
{
    public class MemosViewModel : NavigationViewModel
    {
        private readonly IDialogHostService dialogHost;
        private readonly IEventAggregator aggregator;

        private MemoService memoService;
        private TagService tagService;

        public Memo tempMemo;
        public ObservableCollection<Memo> memoList;
        public ObservableCollection<Tag> tagList;
        public DelegateCommand<ComboBox> tagSelectCommand;
        public DelegateCommand<ListBoxItem> memoSelectCommand;
        private DelegateCommand<ListBox> memoSelectChangeCommand;
        private DelegateCommand<ComboBox> timeSelectCommand;

        public DelegateCommand<string> searchWordChangedCommand;
        private DelegateCommand<Object> updateMemoTitleAndTag;
        private DelegateCommand addMemoCommand;
        private DelegateCommand resetTagCommand;
        private DelegateCommand updateMemo;
        private DelegateCommand preMemoPage;
        private DelegateCommand nextMemoPage;
        public DelegateCommand<ListBoxItem> delMemoCommand;
        private DelegateCommand<Memo> memoSelectedCommand;
        public static int selectTag = -1;
        private string searchWord = "";
        private Workspace workspaceLocal;
        public static int memoPageNum = 1;
        public static int memoPageSize = 10;
        public static int memosTotal = 0;
        public static int memoPages = 0;

        private int selecListBoxIndex = -1;

        private int startTime;
        private int endTime;


        public DelegateCommand<ComboBox> TagSelectCommand
        {
            get { return tagSelectCommand; }
            set { tagSelectCommand = value; RaisePropertyChanged(); }
        }
        public DelegateCommand<ListBoxItem> MemoSelectCommand
        {
            get { return memoSelectCommand; }
            set { memoSelectCommand = value; RaisePropertyChanged("count"); }
        }
        public DelegateCommand<Memo> LoadContentCommand { get; set; }
        public ObservableCollection<Memo> MemoList
        {
            get { return memoList; }
            set { memoList = value; RaisePropertyChanged(); }
        }
        public ObservableCollection<Tag> TagList
        {
            get { return tagList; }
            set { tagList = value; RaisePropertyChanged(); }
        }

        public int SelectTag
        {
            get { return selectTag; }
            set { selectTag = value; RaisePropertyChanged(); }
        }
        public string SearchWord
        {
            get { return searchWord; }
            set { searchWord = value; RaisePropertyChanged(); }
        }
        public DelegateCommand<string> SearchWordChangedCommand
        {
            get { return searchWordChangedCommand; }
            set { searchWordChangedCommand = value; RaisePropertyChanged(); }
        }

        public Workspace WorkspaceLocal { get => workspaceLocal; set => workspaceLocal = value; }
        public DelegateCommand AddMemoCommand { get => addMemoCommand; set => addMemoCommand = value; }
        public DelegateCommand<ListBoxItem> DelMemoCommand { get => delMemoCommand; set => delMemoCommand = value; }
        public int MemoPageNum { get => memoPageNum; set { memoPageNum = value; RaisePropertyChanged(); } }
        public int MemoPageSize { get => memoPageSize; set => memoPageSize = value; }
        public int MemosTotal { get => memosTotal; set { memosTotal = value; RaisePropertyChanged(); } }
        public int MemoPages { get => memoPages; set { memoPages = value; RaisePropertyChanged(); } }
        public DelegateCommand ResetTagCommand { get => resetTagCommand; set => resetTagCommand = value; }

        public DelegateCommand PreMemoPage { get => preMemoPage; set => preMemoPage = value; }
        public DelegateCommand NextMemoPage { get => nextMemoPage; set => nextMemoPage = value; }
        public DelegateCommand<object> UpdateMemoTitleAndTag { get => updateMemoTitleAndTag; set => updateMemoTitleAndTag = value; }
        public Memo TempMemo { get => tempMemo; set { tempMemo = value; RaisePropertyChanged(); } }

        public DelegateCommand UpdateMemo { get => updateMemo; set => updateMemo = value; }
        public DelegateCommand<ListBox> MemoSelectChangeCommand { get => memoSelectChangeCommand; set => memoSelectChangeCommand = value; }

        public int StartTime { get => startTime; set => startTime = value; }
        public int EndTime { get => endTime; set => endTime = value; }
        public DelegateCommand<ComboBox> TimeSelectCommand { get => timeSelectCommand; set => timeSelectCommand = value; }
        public int SelecListBoxIndex { get => selecListBoxIndex; set => selecListBoxIndex = value; }
        public DelegateCommand<Memo> MemoSelectedCommand { get => memoSelectedCommand; set => memoSelectedCommand = value; }

        public MemosViewModel(IEventAggregator aggregator, IContainerProvider provider) : base(provider)
        {
            dialogHost = provider.Resolve<IDialogHostService>();
            this.aggregator = aggregator;
            memoList = new ObservableCollection<Memo>();
            tagList = new ObservableCollection<Tag>();
            memoService = new MemoService();
            tagService = new TagService();
            AddMemoCommand = new DelegateCommand(addNewMemo);
            UpdateMemo = new DelegateCommand(updateMemoInfo);
            ResetTagCommand = new DelegateCommand(resetPage);
            PreMemoPage = new DelegateCommand(getPreMemoPage);
            NextMemoPage = new DelegateCommand(getNextMemoPage);
            DelMemoCommand = new DelegateCommand<ListBoxItem>(delMemo);
            TagSelectCommand = new DelegateCommand<ComboBox>(tagChage);
            MemoSelectCommand = new DelegateCommand<ListBoxItem>(listBoxChange);
            MemoSelectChangeCommand = new DelegateCommand<ListBox>(memoListBoxChange);
            SearchWordChangedCommand = new DelegateCommand<string>(SearchWordChanged);
            UpdateMemoTitleAndTag = new DelegateCommand<Object>(UpdateMemoTitleAndTagMethed);
            TimeSelectCommand = new DelegateCommand<ComboBox>(selectTimeChage);
            MemoSelectedCommand = new DelegateCommand<Memo>(memoSelect);
            WorkspaceLocal = MainWindowModel.SelectWorkspace;
            aggregatorSet(this.aggregator);
            initBaseData();
        }



        private void UpdateMemoTitleAndTagMethed(object obj)
        {

        }

        public async void initBaseData()
        {
            var task = await getTagList(1, 10);
            if (task)
            {
                getMemoList();
            }
        }

        private void selectTimeChage(ComboBox obj)
        {
            // 全部 今天 昨天 进7天
            int selectIndex = obj.SelectedIndex;
            if (selectIndex == -1)
            {
                return;
            }
            else if (selectIndex == 0)
            {
                StartTime = -1;
                EndTime = -1;
            }
            else if (selectIndex == 1)
            {
                StartTime = TimeUtil.getSecond(DateTime.Now.Date);
                EndTime = TimeUtil.getSecond(DateTime.Now);
            }
            else if (selectIndex == 2)
            {
                StartTime = TimeUtil.getSecond(DateTime.Now.Date.AddDays(-1));
                EndTime = StartTime - 24 * 3600;
            }
            else
            {
                StartTime = TimeUtil.getSecond(DateTime.Now.Date.AddDays(-6));
                EndTime = TimeUtil.getSecond(DateTime.Now);
            }
            getMemoList();
        }

        private void aggregatorSet(IEventAggregator aggregator)
        {
            aggregator.ResgiterWorkspace(arg =>
            {
                WorkspaceLocal = arg.Value;
                resetPage();
                getMemoList();
            });
            aggregator.ResgiterFlash(arg =>
            {
                if ("Memo".Equals(arg.Name))
                {
                    resetPage();
                    getMemoList();
                }
            });

        }
        /// <summary>
        /// 删除备忘录
        /// </summary>
        /// <param name="boxItem"></param>
        private async void delMemo(ListBoxItem boxItem)
        {
            try
            {
                Memo memo = boxItem.DataContext as Memo;
                var dialogResult = await dialogHost.Question("温馨提示", $"确认删除:{memo.title} ?");
                if (dialogResult.Result != Prism.Services.Dialogs.ButtonResult.OK) return;
                UpdateLoading(true);
                JsonObject param = new JsonObject();
                param.Add("id", memo.id);
                ApiResponse apiResponse = await memoService.DelMemo(param);
                if (apiResponse.code == 0)
                {
                    aggregator.SendMessage("删除成功");
                    getMemoList();
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
                UpdateLoading(false);
            }
        }


        /// <summary>
        /// 添加备忘录
        /// </summary>
        private async void addNewMemo()
        {
            try
            {

                JsonObject param = new JsonObject();
                param.Add("workspaceId", WorkspaceLocal.id);
                param.Add("tagId", SelectTag);
                param.Add("title", "新备忘录");
                param.Add("content", "山有鬼兮");
                ApiResponse apiResponse = await memoService.SaveMemo(param);
                if (apiResponse.code == 0)
                {
                    aggregator.SendMessage(apiResponse.msg);
                    resetPage();
                    getMemoList();
                } else
                {
                    aggregator.SendMessage("添加失败");
                }
            }
            catch (Exception e)
            {
                aggregator.SendMessage("网络错误");
            }

        }
        private async void updateMemoInfo()
        {
            try
            {
                if (TempMemo == null)
                {
                    return;
                }

                JsonObject param = new JsonObject();
                param.Add("tagId", TagList[TempMemo.tagIndex].id);
                param.Add("title", TempMemo.title);
                param.Add("id", TempMemo.id);
                ApiResponse apiResponse = await memoService.UpdateMemo(param);
                if (apiResponse.code == 0)
                {
                    aggregator.SendMessage(apiResponse.msg);
                    getMemoList();
                }
            }
            catch (Exception e)
            {
                aggregator.SendMessage("网络错误");
            }

        }

        private void listBoxChange(ListBoxItem obj)
        {
            Memo temp = obj.DataContext as Memo;
            int selectIndex = 0;
            foreach (Memo item in MemoList)
            {
                if (item.id == temp.id)
                {
                    if (SelecListBoxIndex != selectIndex) { 
                        TempMemo = temp;
                    }
                    SelecListBoxIndex = selectIndex;
                    break;
                }
                selectIndex++;
            }
        }

        private void memoSelect(Memo obj)
        {
            if (obj == null)
            {
                return;
            }
            Memo temp = obj as Memo;
            int selectIndex = -1;
            foreach (Memo item in MemoList)
            {
                if (item.id == temp.id)
                {
                    if (SelecListBoxIndex != selectIndex)
                    {
                        TempMemo = temp;
                    }
                    SelecListBoxIndex = selectIndex;
                    break;
                }
                selectIndex++;
            }
        }

        private void memoListBoxChange(ListBox obj)
        {
            TempMemo = (Memo)obj.SelectedItem;
        }
        /// <summary>
        /// 切换标签
        /// </summary>
        /// <param name="comboBox"></param>
        public void tagChage(ComboBox comboBox)
        {
            resetPage();
            Tag item = (Tag)comboBox.SelectedItem;
            if (item == null)
            {
                SelectTag = -1;
                getMemoList();
            }
            else
            {
                SelectTag = item.id;

                getMemoList();
            }
        }
        /// <summary>
        /// 搜索框文字改变
        /// </summary>
        /// <param name="textBox"></param>
        public void SearchWordChanged(string textBox)
        {
            //if (string.IsNullOrEmpty(searchWord))
            //{
            //    return;
            //}
            resetPage();
            getMemoList();
        }

        /// <summary>
        /// 查询备忘录
        /// </summary>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <param name="tagId"></param>
        /// <param name="searchWord"></param>
        public async void getMemoList(string searchWord = null)
        {
            if (WorkspaceLocal == null)
            {

                return;
            }
            try
            {
                JsonObject param = new JsonObject();
                param.Add("workspaceId", WorkspaceLocal.id);
                param.Add("pageNum", MemoPageNum);
                param.Add("pageSize", MemoPageSize);
                if (SelectTag != -1)
                {
                    param.Add("tagId", SelectTag);
                }
                if (SearchWord != "")
                {
                    param.Add("searchWord", SearchWord);
                }
                if (StartTime > 0)
                {
                    param.Add("startDate", StartTime);
                }
                if (EndTime > 0)
                {
                    param.Add("endDate", EndTime);
                }
 
                var res = await memoService.MemoList(param);
                var obj = res.data;
                if (res.code != 0)
                {
                    aggregator.SendMessage(res.msg);
                    return;
                }
                MemoList.Clear();
                MemosTotal = obj.total;
                MemoPages = obj.pages;
                foreach (Memo item in obj.list)
                {
                    int itemTagIndex = -1;
                    int i = 0;
                    foreach (Tag itemTag in TagList)
                    {
                        if (item.tagId <= 0)
                        {
                            break;
                        }
                        if (item.tagId == itemTag.id)
                        {
                            itemTagIndex = i;
                            break;
                        }
                        i++;
                    }
                    item.tagIndex = itemTagIndex;
                    MemoList.Add(item);
                }
                if (MemoList.Count > 0)
                {
                    if(SelecListBoxIndex == -1)
                    {
                        aggregator.SetMemo(MemoList[0]);
                    } else
                    {
                        aggregator.SetMemo(MemoList[SelecListBoxIndex]);
                    }
                    
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

        private void getNextMemoPage()
        {
            if (MemoPageNum * MemoPageSize <= MemosTotal)
            {
                MemoPageNum++;
                getMemoList();
            }
        }

        private void getPreMemoPage()
        {
            if (MemoPageNum > 1)
            {
                MemoPageNum--;
                getMemoList();
            }
        }
        /// <summary>
        /// 请求标签
        /// </summary>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        public async Task<bool> getTagList(int pageNum, int pageSize)
        {
            try
            {
                JsonObject param = new JsonObject();
                param.Add("pageNum", pageNum);
                param.Add("pageSize", pageSize);
                var obj = await tagService.GetTagList(param);
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
        /// <summary>
        /// 充值页码
        /// </summary>
        private void resetPage()
        {
            MemoPageNum = 1;
            MemoPageSize = 10;
        }

        public void flashMemo()
        {
            getMemoList();
            MemoList.Clear();
        }

    }







}
