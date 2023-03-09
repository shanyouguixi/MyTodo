using Microsoft.Web.WebView2.WinForms;
using MyTodo.Common.Model;
using MyTodo.Common.service;
using MyTodo.Common.service.request;
using Prism.Commands;
using Prism.Mvvm;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using static System.Net.Mime.MediaTypeNames;

namespace MyTodo.ViewModels
{
    public class MemosViewModel : BindableBase
    {
        private MemoService memoService;
        private TagService tagService;

        public Memo tempMemo { get; set; }
        public ObservableCollection<Memo> memoList;
        public ObservableCollection<Tag> tagList;
        public DelegateCommand<ComboBox> tagSelectCommand;
        public DelegateCommand<ListBox> memoSelectCommand;
        public DelegateCommand<string> searchWordChangedCommand;
        public int selectTag = -1;
        private string searchWord = "";



        public DelegateCommand<ComboBox> TagSelectCommand
        {
            get { return tagSelectCommand; }
            set { tagSelectCommand = value; RaisePropertyChanged(); }
        }
        public DelegateCommand<ListBox> MemoSelectCommand
        {
            get { return memoSelectCommand; }
            set { memoSelectCommand = value; RaisePropertyChanged(); }
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

        public MemosViewModel()
        {
            memoList = new ObservableCollection<Memo>();
            tagList = new ObservableCollection<Tag>();
            memoService = new MemoService();
            tagService = new TagService();
            TagSelectCommand = new DelegateCommand<ComboBox>(tagChage);
            MemoSelectCommand = new DelegateCommand<ListBox>(listBoxChange);
            SearchWordChangedCommand = new DelegateCommand<string>(SearchWordChanged);
            getMemoList(1, 10);
            getTagList(1, 10);
        }

        private void listBoxChange(ListBox obj)
        {
            tempMemo = (Memo)obj.SelectedItem;
        }

        public void tagChage(ComboBox comboBox)
        {
            Tag item = (Tag)comboBox.SelectedItem;
            if (item == null)
            {
                SelectTag = -1;
                memoList.Clear();
                getMemoList(1, 10);
            }
            else
            {
                SelectTag = item.id;
                getMemoList(1, 10);
            }
        }

        public void SearchWordChanged(string textBox)
        {
            //if (string.IsNullOrEmpty(searchWord))
            //{
            //    return;
            //}
            getMemoList(1, 10);
        }

        /// <summary>
        /// 查询备忘录
        /// </summary>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <param name="tagId"></param>
        /// <param name="searchWord"></param>
        public async void getMemoList(int pageNum, int pageSize, string searchWord = null)
        {
            try
            {
                JsonObject param = new JsonObject();
                param.Add("pageNum", pageNum);
                param.Add("pageSize", pageSize);
                if (SelectTag != -1)
                {
                    param.Add("tagId", SelectTag);
                }
                if (SearchWord != "")
                {
                    param.Add("searchWord", SearchWord);
                }
                var obj = await memoService.MemoList(param);
                if(pageNum == 1)
                {
                    memoList.Clear();
                }
                foreach (Memo item in obj.list)
                {
                    MemoList.Add(item);
                }
            }
            finally
            {

            }
        }

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
            finally
            {

            }
        }

    }





}
