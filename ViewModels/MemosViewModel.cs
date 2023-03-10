using MaterialDesignThemes.Wpf;
using Microsoft.Web.WebView2.WinForms;
using MyTodo.Common.DialogUtils;
using MyTodo.Common.Extendsions;
using MyTodo.Common.Model;
using MyTodo.Common.service;
using MyTodo.Common.service.request;
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
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using static System.Net.Mime.MediaTypeNames;

namespace MyTodo.ViewModels
{
    public class MemosViewModel : NavigationViewModel
    {
        private readonly IDialogHostService dialogHost;

        private MemoService memoService;
        private TagService tagService;

        public Memo tempMemo { get; set; }
        public ObservableCollection<Memo> memoList;
        public ObservableCollection<Tag> tagList;
        public DelegateCommand<ComboBox> tagSelectCommand;
        public DelegateCommand<ListBox> memoSelectCommand;
        public DelegateCommand<string> searchWordChangedCommand;
        private DelegateCommand addMemoCommand;
        private DelegateCommand resetTagCommand;
        private DelegateCommand preMemoPage;
        private DelegateCommand nextMemoPage;
        public DelegateCommand<ListBoxItem> delMemoCommand;
        public static int selectTag = -1;
        private string searchWord = "";
        private readonly IEventAggregator aggregator;
        private Workspace workspaceLocal;
        private int memoPageNum = 1;
        private int memoPageSize = 10;
        private int memosTotal = 0;
        private int memoPages = 0;



        public DelegateCommand<ComboBox> TagSelectCommand
        {
            get { return tagSelectCommand; }
            set { tagSelectCommand = value; RaisePropertyChanged(); }
        }
        public DelegateCommand<ListBox> MemoSelectCommand
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
        public DelegateCommand ResetTagCommand { get => resetTagCommand; set => resetTagCommand = value; }
        public int MemoPages { get => memoPages; set { memoPages = value; RaisePropertyChanged(); } }

        public DelegateCommand PreMemoPage { get => preMemoPage; set => preMemoPage = value; }
        public DelegateCommand NextMemoPage { get => nextMemoPage; set => nextMemoPage = value; }

        public MemosViewModel(IEventAggregator aggregator, IContainerProvider provider) : base(provider)
        {
            dialogHost = provider.Resolve<IDialogHostService>();
            this.aggregator = aggregator;
            memoList = new ObservableCollection<Memo>();
            tagList = new ObservableCollection<Tag>();
            memoService = new MemoService();
            tagService = new TagService();
            AddMemoCommand = new DelegateCommand(addNewMemo);
            ResetTagCommand = new DelegateCommand(resetPage);
            PreMemoPage = new DelegateCommand(getPreMemoPage);
            NextMemoPage = new DelegateCommand(getNextMemoPage);
            DelMemoCommand = new DelegateCommand<ListBoxItem>(delMemo);
            TagSelectCommand = new DelegateCommand<ComboBox>(tagChage);
            MemoSelectCommand = new DelegateCommand<ListBox>(listBoxChange);
            SearchWordChangedCommand = new DelegateCommand<string>(SearchWordChanged);
            WorkspaceLocal = MainWindowModel.SelectWorkspace;
            getMemoList();
            getTagList(1, 10);
            aggregatorSet(this.aggregator);
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
                if ("Memo".Equals(arg))
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
                }
            }
            catch (Exception e)
            {
                aggregator.SendMessage("网络错误");
            }

        }

        private void listBoxChange(ListBox obj)
        {
            tempMemo = (Memo)obj.SelectedItem;
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
                var res = await memoService.MemoList(param);
                var obj = res.data;
                if (res.code != 0)
                {
                    aggregator.SendMessage(res.msg);
                }
                memoList.Clear();
                MemosTotal = obj.total;
                MemoPages = obj.pages;
                foreach (Memo item in obj.list)
                {
                    MemoList.Add(item);
                }
                if (MemoList.Count > 0)
                {
                    aggregator.SetMemo(MemoList[0]);
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
        public async void getTagList(int pageNum, int pageSize)
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
