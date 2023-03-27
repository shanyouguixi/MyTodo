using MyMemo.Common.DialogUtils;
using MyMemo.Common.Extendsions;
using MyMemo.Common.Model;
using MyMemo.Common.service;
using MyMemo.Common.service.request;
using MyMemo.ViewModels;
using MyTodo.Common.Events;
using MyTodo.Common.Model;
using MyTodo.Common.service;
using MyTodo.Common.Util;
using MyTodo.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MyTodo.ViewModels
{
    public class TodoViewModel : NavigationViewModel
    {
        private readonly IDialogHostService dialogHost;
        private readonly IEventAggregator aggregator;


        private Workspace workspaceLocal;

        private TagService tagService;
        private ObservableCollection<Tag> tagList;

        private TodoService todoService;
        private ObservableCollection<Todo> todoList;

        private int selectTag = -1;
        private string searchWord = "";
        public static int todoPageNum = 1;
        public static int todoPageSize = 10;
        public static int todosTotal = 0;
        public static int todoPages = 0;

        private int selectListBoxIndex = 0;

        private int startTime;
        private int endTime;

        private Todo tempTodo;


        private DelegateCommand<string> searchWordChangedCommand;

        private DelegateCommand<ComboBox> tagSelectCommand;
        private DelegateCommand<ComboBox> timeSelectCommand;

        private DelegateCommand resetTagCommand;

        private DelegateCommand addTodoCommand;
        private DelegateCommand updateTodoCommand;

        private DelegateCommand<ListBoxItem> delTodoCommand;
        private DelegateCommand<Todo> todoSelectedCommand;


        private DelegateCommand preTodoPage;
        private DelegateCommand nextTodoPage;



        public int TodoPageNum { get => todoPageNum; set { todoPageNum = value; RaisePropertyChanged(); } }
        public int TodoPageSize { get => todoPageSize; set => todoPageSize = value; }
        public int TodosTotal { get => todosTotal; set { todosTotal = value; RaisePropertyChanged(); } }
        public int TodoPages { get => todoPages; set { todoPages = value; RaisePropertyChanged(); } }
        public int SelectTag { get => selectTag; set => selectTag = value; }
        public string SearchWord { get => searchWord; set { searchWord = value; RaisePropertyChanged(); } }

        public ObservableCollection<Tag> TagList { get => tagList; set { tagList = value; RaisePropertyChanged(); } }

        public ObservableCollection<Todo> TodoList { get => todoList; set { todoList = value; RaisePropertyChanged(); } }

        public Workspace WorkspaceLocal { get => workspaceLocal; set => workspaceLocal = value; }
        public DelegateCommand<string> SearchWordChangedCommand { get => searchWordChangedCommand; set => searchWordChangedCommand = value; }
        public DelegateCommand<ComboBox> TagSelectCommand { get => tagSelectCommand; set => tagSelectCommand = value; }
        public DelegateCommand ResetTagCommand { get => resetTagCommand; set => resetTagCommand = value; }
        public DelegateCommand AddTodoCommand { get => addTodoCommand; set => addTodoCommand = value; }
        public DelegateCommand<ListBoxItem> DelTodoCommand { get => delTodoCommand; set => delTodoCommand = value; }
        public DelegateCommand PreTodoPage { get => preTodoPage; set => preTodoPage = value; }
        public DelegateCommand NextTodoPage { get => nextTodoPage; set => nextTodoPage = value; }
        public DelegateCommand<Todo> TodoSelectedCommand { get => todoSelectedCommand; set => todoSelectedCommand = value; }
        public Todo TempTodo { get => tempTodo; set { tempTodo = value; RaisePropertyChanged(); } }

        public DelegateCommand UpdateTodoCommand { get => updateTodoCommand; set => updateTodoCommand = value; }
        public DelegateCommand<ComboBox> TimeSelectCommand { get => timeSelectCommand; set => timeSelectCommand = value; }
        public int StartTime { get => startTime; set => startTime = value; }
        public int EndTime { get => endTime; set => endTime = value; }
        public int SelectListBoxIndex { get => selectListBoxIndex; set => selectListBoxIndex = value; }

        public TodoViewModel(IEventAggregator aggregator, IContainerProvider provider) : base(provider)
        {
            dialogHost = provider.Resolve<IDialogHostService>();
            this.aggregator = aggregator;
            WorkspaceLocal = MainWindowModel.SelectWorkspace;
            tagService = new TagService();
            todoService = new TodoService();
            TagList = new ObservableCollection<Tag>();
            TodoList = new ObservableCollection<Todo>();

            SearchWordChangedCommand = new DelegateCommand<string>(SearchWordChanged);

            TagSelectCommand = new DelegateCommand<ComboBox>(tagChage);
            TimeSelectCommand = new DelegateCommand<ComboBox>(selectTimeChage);
            ResetTagCommand = new DelegateCommand(resetPage);
            AddTodoCommand = new DelegateCommand(addNewTodo);
            NextTodoPage = new DelegateCommand(getNextTodoPage);
            PreTodoPage = new DelegateCommand(getPreTodoPage);
            DelTodoCommand = new DelegateCommand<ListBoxItem>(delTodo);
            TodoSelectedCommand = new DelegateCommand<Todo>(tempTodoSelected);
            UpdateTodoCommand = new DelegateCommand(updateTempTodo);
            aggregatorSet(this.aggregator);
            initBaseData();
        }



        private async void updateTempTodo()
        {
            try
            {
                if (TempTodo == null)
                {
                    return;
                }
                JsonObject param = new JsonObject();
                param.Add("id", TempTodo.id);
                int selectIndex = TempTodo.tagIndex;
                if (selectIndex == -1)
                {
                    param.Add("tagId", -1);
                }
                else
                {
                    param.Add("tagId", TagList[TempTodo.tagIndex].id);
                }
                param.Add("title", TempTodo.title);
                param.Add("content", TempTodo.content);
                param.Add("remarkDate", TempTodo.remarkDate.ToString());
                ApiResponse apiResponse = await todoService.UpdateTodo(param);
                if (apiResponse.code == 0)
                {
                    aggregator.SendMessage(apiResponse.msg);
                    getTodoList();
                    this.aggregator.SetFlash("TodoClock");
                }
                else
                {
                    aggregator.SendMessage("更新失败");
                }
            }
            catch (Exception e)
            {
                aggregator.SendMessage("网络错误");
            }
        }

        private void tempTodoSelected(Todo obj)
        {
            if (obj == null)
            {
                return;
            }
            int tempIndex = 0;
            foreach (Todo item in TodoList)
            {
                if (item.id == obj.id)
                {
                    SelectListBoxIndex = tempIndex;
                    break;
                }
                tempIndex++;
            }
            TempTodo = obj;

        }

        public async void initBaseData()
        {
            var task = await getTagList(1, 10);
            if (task)
            {
                getTodoList();
            }
        }

        private void aggregatorSet(IEventAggregator aggregator)
        {
            aggregator.ResgiterWorkspace(arg =>
            {
                WorkspaceLocal = arg.Value;
                resetPage();
                getTodoList();
            });
            aggregator.ResgiterFlash(arg =>
            {
                if ("Memo".Equals(arg.Name))
                {
                    resetPage();
                    getTodoList();
                }
            });

        }


        private async void addNewTodo()
        {
            try
            {

                JsonObject param = new JsonObject();
                param.Add("workspaceId", WorkspaceLocal.id);
                param.Add("tagId", SelectTag);
                param.Add("title", "新备便签");
                param.Add("content", "山有鬼兮");
                ApiResponse apiResponse = await todoService.SaveTodo(param);
                if (apiResponse.code == 0)
                {
                    aggregator.SendMessage(apiResponse.msg);
                    resetPage();
                    getTodoList();
                    this.aggregator.SetFlash("TodoClock");
                }
            }
            catch (Exception e)
            {
                aggregator.SendMessage("网络错误");
            }
        }

        private async void delTodo(ListBoxItem boxItem)
        {
            try
            {
                Todo todo = boxItem.DataContext as Todo;
                var dialogResult = await dialogHost.Question("温馨提示", $"确认删除:{todo.title} ?");
                if (dialogResult.Result != Prism.Services.Dialogs.ButtonResult.OK) return;
                UpdateLoading(true);
                JsonObject param = new JsonObject();
                param.Add("id", todo.id);
                ApiResponse apiResponse = await todoService.DelTodo(param);
                if (apiResponse.code == 0)
                {
                    aggregator.SendMessage("删除成功");
                    getTodoList();
                    this.aggregator.SetFlash("TodoClock");
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

        public void SearchWordChanged(string textBox)
        {
            resetPage();
            getTodoList();
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
                EndTime = TimeUtil.getSecond(DateTime.Now);
            }
            else
            {
                StartTime = TimeUtil.getSecond(DateTime.Now.Date.AddDays(-6));
                EndTime = TimeUtil.getSecond(DateTime.Now);
            }
            getTodoList();
        }
        public async void getTodoList(string searchWord = null)
        {
            if (WorkspaceLocal == null)
            {
                return;
            }
            try
            {
                JsonObject param = new JsonObject();
                param.Add("workspaceId", WorkspaceLocal.id);
                param.Add("pageNum", TodoPageNum);
                param.Add("pageSize", TodoPageSize);


                if (StartTime > 0)
                {
                    param.Add("startDate", StartTime);
                }
                if (EndTime > 0)
                {
                    param.Add("endDate", EndTime);
                }
                if (SelectTag != -1)
                {
                    param.Add("tagId", SelectTag);
                }
                if (SearchWord != "")
                {
                    param.Add("searchWord", SearchWord);
                }
                var res = await todoService.TodoList(param);
                var obj = res.data;
                if (res.code != 0)
                {
                    aggregator.SendMessage(res.msg);
                    return;
                }
                TodoList.Clear();
                TodosTotal = obj.total;
                TodoPages = obj.pages;
                foreach (Todo item in obj.list)
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
                    TodoList.Add(item);
                }
                if (TodoList.Count > 0)
                {
                    TempTodo = TodoList[SelectListBoxIndex];
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

        public void tagChage(ComboBox comboBox)
        {
            resetPage();
            Tag item = (Tag)comboBox.SelectedItem;
            if (item == null)
            {
                SelectTag = -1;
                getTodoList();
            }
            else
            {
                SelectTag = item.id;

                getTodoList();
            }
        }

        private void resetPage()
        {
            TodoPageNum = 1;
            TodoPageSize = 10;
        }

        private void getNextTodoPage()
        {
            if (TodoPageNum * todoPageSize <= TodosTotal)
            {
                TodoPageNum++;
                getTodoList();
            }
        }

        private void getPreTodoPage()
        {
            if (TodoPageNum > 1)
            {
                TodoPageNum--;
                getTodoList();
            }
        }

    }
}
