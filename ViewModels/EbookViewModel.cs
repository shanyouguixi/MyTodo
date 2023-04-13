using MaterialDesignThemes.Wpf;
using MyMemo.Common.DialogUtils;
using MyMemo.Common.Extendsions;
using MyMemo.Common.Model;
using MyMemo.Common.service.request;
using MyMemo.Common.service;
using MyMemo.ViewModels;
using MyTodo.Common.Model;
using MyTodo.Common.service;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Diagnostics;
using Prism.Commands;
using System.IO;
using System.ComponentModel.Design;
using Microsoft.VisualBasic.ApplicationServices;
using System.Security.Policy;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;

namespace MyTodo.ViewModels
{
    public class EbookViewModel : NavigationViewModel
    {
        private readonly IDialogHostService dialogHost;
        private readonly IEventAggregator aggregator;



        private EbookService ebookService;
        private ResourceService resourceService;
        private ObservableCollection<Ebook> ebookList;
        private ObservableCollection<EbookTag> ebookTagList;
        private int ebookPageNum = 1;
        private int ebookPageSize = 10;
        private int ebooksTotal = 0;
        private int ebookPages = 0;
        private int selectTag = -1;
        private string searchWord;
        private int selecListBoxIndex;
        private int selectListBoxIndex;
        public Ebook tempEbook;
        private int updateSelectIndex = 0;



        private DelegateCommand<string> searchWordChangedCommand;
        private DelegateCommand preEbookPage;
        private DelegateCommand nextEbookPage;
        private DelegateCommand<ComboBox> tagSelectCommand;
        private DelegateCommand<Ebook> ebookSelectedCommand;
        private DelegateCommand<ListBoxItem> delTodoCommand;
        private DelegateCommand<ListBoxItem> uploadTodoCommand;
        private DelegateCommand<Object> imageSelectCommand;
        private DelegateCommand<string> toUpdateEbookCommand;
        private DelegateCommand addNewEbookCommand;


        public ObservableCollection<Ebook> EbookList { get => ebookList; set => ebookList = value; }
        public ObservableCollection<EbookTag> EbookTagList { get => ebookTagList; set => ebookTagList = value; }
        public int EbookPageNum { get => ebookPageNum; set => ebookPageNum = value; }
        public int EbookPageSize { get => ebookPageSize; set => ebookPageSize = value; }
        public int EbooksTotal { get => ebooksTotal; set => ebooksTotal = value; }
        public int EbookPages { get => ebookPages; set => ebookPages = value; }
        public int SelectTag { get => selectTag; set => selectTag = value; }
        public string SearchWord { get => searchWord; set => searchWord = value; }
        public Ebook TempEbook { get => tempEbook; set { tempEbook = value; RaisePropertyChanged(); } }
        public int SelecListBoxIndex { get => selecListBoxIndex; set => selecListBoxIndex = value; }
        public DelegateCommand<string> SearchWordChangedCommand { get => searchWordChangedCommand; set => searchWordChangedCommand = value; }
        public DelegateCommand<ComboBox> TagSelectCommand { get => tagSelectCommand; set => tagSelectCommand = value; }
        public DelegateCommand PreEbookPage { get => preEbookPage; set => preEbookPage = value; }
        public DelegateCommand NextEbookPage { get => nextEbookPage; set => nextEbookPage = value; }
        public DelegateCommand<Ebook> EbookSelectedCommand { get => ebookSelectedCommand; set => ebookSelectedCommand = value; }
        public int SelectListBoxIndex { get => selectListBoxIndex; set => selectListBoxIndex = value; }
        public DelegateCommand<ListBoxItem> DelTodoCommand { get => delTodoCommand; set => delTodoCommand = value; }
        public DelegateCommand<Object> ImageSelectCommand { get => imageSelectCommand; set => imageSelectCommand = value; }
        public int UpdateSelectIndex { get => updateSelectIndex; set { updateSelectIndex = value; RaisePropertyChanged(); } }

        public DelegateCommand<string> ToUpdateEbookCommand { get => toUpdateEbookCommand; set => toUpdateEbookCommand = value; }
        public DelegateCommand AddNewEbookCommand { get => addNewEbookCommand; set => addNewEbookCommand = value; }
        public DelegateCommand<ListBoxItem> UploadTodoCommand { get => uploadTodoCommand; set => uploadTodoCommand = value; }

        public EbookViewModel(IEventAggregator aggregator, IContainerProvider provider) : base(provider)
        {
            dialogHost = provider.Resolve<IDialogHostService>();
            this.aggregator = aggregator;
            ebookService = new EbookService();
            resourceService = new ResourceService();
            ebookList = new ObservableCollection<Ebook>();
            ebookTagList = new ObservableCollection<EbookTag>();
            SearchWordChangedCommand = new DelegateCommand<string>(SearchWordChanged);
            EbookSelectedCommand = new DelegateCommand<Ebook>(tempEbookSelected);
            DelTodoCommand = new DelegateCommand<ListBoxItem>(delEbook);
            UploadTodoCommand = new DelegateCommand<ListBoxItem>(uploadPdf);
            TagSelectCommand = new DelegateCommand<ComboBox>(tagChage);
            ImageSelectCommand = new DelegateCommand<Object>(changeEbookImage);
            ToUpdateEbookCommand = new DelegateCommand<string>(toUpdateEbook);
            AddNewEbookCommand = new DelegateCommand(addNewEbook);
            initBaseData();

            aggregator.ResgiterFlash((arg) =>
            {
                if ("EbookTag".Equals(arg.Name))
                {
                    getEbookTagList();
                }
            });
        }



        public async void initBaseData()
        {

            getEbookTagList();
            getEbookList();

        }
        private void resetPage()
        {
            EbookPageNum = 0;
            EbookPageSize = 10;
        }
        public void SearchWordChanged(string textBox)
        {
            resetPage();
            getEbookList();
        }

        public void tagChage(ComboBox comboBox)
        {
            resetPage();
            EbookTag item = (EbookTag)comboBox.SelectedItem;
            if (item == null)
            {
                SelectTag = -1;
                getEbookList();
            }
            else
            {
                SelectTag = item.id;
                getEbookList();
            }
        }

        private void tempEbookSelected(Ebook obj)
        {
            if (obj == null)
            {
                return;
            }
            //int tempIndex = 0;
            //foreach (Ebook item in EbookList)
            //{
            //    if (item.id == obj.id)
            //    {
            //        SelectListBoxIndex = tempIndex;
            //        break;
            //    }
            //    tempIndex++;
            //}
            TempEbook = obj;

        }

        public async void getEbookTagList()
        {
            try
            {
                var res = await ebookService.GetEbookTag(null);
                var obj = res.data;
                if (res.code != 0)
                {
                    aggregator.SendMessage(res.msg);
                    return;
                }
                EbookTagList.Clear();
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

        public async void getEbookList(string searchWord = null)
        {
            try
            {
                JsonObject param = new JsonObject();
                param.Add("pageNum", EbookPageNum);
                param.Add("pageSize", EbookPageSize);
                if (SelectTag != -1)
                {
                    param.Add("tagId", SelectTag);
                }
                if (SearchWord != "")
                {
                    param.Add("searchWord", SearchWord);
                }

                var res = await ebookService.GetEbook(param);
                var obj = res.data;
                if (res.code != 0)
                {
                    aggregator.SendMessage(res.msg);
                    return;
                }
                EbookList.Clear();
                EbooksTotal = obj.total;
                EbookPages = obj.pages;
                foreach (Ebook item in obj.list)
                {
                    int itemTagIndex = -1;
                    int i = 0;
                    foreach (EbookTag itemTag in EbookTagList)
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
                    EbookList.Add(item);
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

        private async void uploadPdf(ListBoxItem boxItem)
        {
            Ebook ebook = boxItem.DataContext as Ebook;
            var apiResponse = await resourceService.UploadFile(null, ebook.path);
            if (apiResponse.code == 0)
            {
                Resource resource = apiResponse.data;
                string url = resource.fileUrl;
                JsonObject param = new JsonObject();
                param.Add("id", ebook.id);
                param.Add("url", url);
                ApiResponse saveResponse = await ebookService.SaveEbook(param);
                if (saveResponse.code == 0)
                {
                    aggregator.SendMessage(saveResponse.msg);
                    resetPage();
                    getEbookList();
                }
                else
                {
                    aggregator.SendMessage("修改失败");
                }
            }
        }


        private async void delEbook(ListBoxItem boxItem)
        {
            try
            {
                Ebook ebook = boxItem.DataContext as Ebook;
                var dialogResult = await dialogHost.Question("温馨提示", $"确认删除:{ebook.name} ?");
                if (dialogResult.Result != Prism.Services.Dialogs.ButtonResult.OK) return;
                UpdateLoading(true);
                JsonObject param = new JsonObject();
                param.Add("id", ebook.id);
                ApiResponse apiResponse = await ebookService.DelEbook(param);
                if (apiResponse.code == 0)
                {
                    aggregator.SendMessage("删除成功");
                    getEbookList();
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
        private async void addNewEbook()
        {
            try
            {
                if (SelectTag <= 0)
                {
                    aggregator.SendMessage("请选择分类");
                    return;
                }
                string filePath = selectFile("PDF|*.pdf;");
                if (filePath == null)
                {
                    aggregator.SendMessage("请选择文件");
                    return;
                }
                //var apiResponse = await resourceService.UploadFile(null, filePath);
                int index = filePath.LastIndexOf("\\");
                string fileName = filePath.Substring(index + 1, filePath.Length - index - 1);
                //if (apiResponse.code == 0)
                //{
                //}
                //Resource resource = apiResponse.data;
                //    string url = resource.fileUrl;
                JsonObject param = new JsonObject();
                param.Add("tagId", SelectTag);
                param.Add("path", filePath);
                param.Add("name", fileName);
                ApiResponse saveResponse = await ebookService.SaveEbook(param);
                if (saveResponse.code == 0)
                {
                    aggregator.SendMessage(saveResponse.msg);
                    resetPage();
                    getEbookList();
                }
                else
                {
                    aggregator.SendMessage("添加失败");
                }



            }
            catch (Exception e)
            {
                aggregator.SendMessage("网络错误");
            }

        }
        private async void updateEbookInfo()
        {
            try
            {
                if (TempEbook == null)
                {
                    return;
                }
                JsonObject param = new JsonObject();
                param.Add("tagId", EbookList[TempEbook.tagIndex].id);
                param.Add("name", TempEbook.name);
                param.Add("path", TempEbook.path);
                param.Add("image", TempEbook.image);
                param.Add("url", TempEbook.url);
                param.Add("id", TempEbook.id);
                ApiResponse apiResponse = await ebookService.UpdateEbook(param);
                if (apiResponse.code == 0)
                {
                    aggregator.SendMessage(apiResponse.msg);
                    getEbookList();
                }
            }
            catch (Exception e)
            {
                aggregator.SendMessage("网络错误");
            }

        }

        private async void changeEbookImage(Object id)
        {
            if (id == null)
            {
                return;
            }
            string filePath = selectFile("图片|*.gif;*.jpg;*.jpeg;*.bmp;*.jfif;*.png;");
            if (filePath == null)
            {
                aggregator.SendMessage("请选择文件");
                return;
            }
            var apiResponse = await resourceService.UploadFile(null, filePath);
            if (apiResponse.code == 0)
            {
                Resource resource = apiResponse.data;
                string url = resource.fileUrl;
                JsonObject param = new JsonObject();
                param.Add("id", (int)id);
                param.Add("image", url);
                var updateRes = await ebookService.UpdateEbook(param);
                if (updateRes.code == 0)
                {
                    getEbookList();
                }
                else
                {
                    aggregator.SendMessage("上传失败");
                }
            }
            else
            {
                aggregator.SendMessage("上传失败");
            }
        }

        private async void toUpdateEbook(string obj)
        {
            if ("edit".Equals(obj))
            {
                UpdateSelectIndex = 1;
            }
            else if ("cancel".Equals(obj))
            {
                UpdateSelectIndex = 0;
            }
            else
            {
                JsonObject param = new JsonObject();
                param.Add("id", TempEbook.id);
                param.Add("path", TempEbook.path);
                param.Add("name", TempEbook.name);
                param.Add("desc", TempEbook.desc);
                var updateRes = await ebookService.UpdateEbook(param);
                if (updateRes.code == 0)
                {
                    aggregator.SendMessage("编辑成功");
                    getEbookList();
                    UpdateSelectIndex = 0;
                }
                else
                {
                    aggregator.SendMessage("编辑失败");
                }
            }
        }


        private string selectFile(string Filter)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = Filter //限制只能选择这几种图片格式
            };
            string file = null;
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                file = openFileDialog.FileName;
            }
            return file;
        }


    }
}
