using MyTodo.Common.Events;
using MyTodo.Common.Extendsions;
using MyTodo.Common.Model;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace MyTodo.Views
{
    /// <summary>
    /// MemosView.xaml 的交互逻辑
    /// </summary>
    public partial class MemosView : UserControl
    {
        private readonly IEventAggregator aggregator;
        public MemosView(IEventAggregator aggregator)
        {
            this.aggregator = aggregator;
            InitializeComponent();
            InitializeAsync();
            aggregator.ResgiterMemo(arg =>
            {
                loadMemo(arg.Value);
            });
        }

        private async void InitializeAsync()
        {
            //await webView.EnsureCoreWebView2Async(null);
            //需要是同xcopy把html复制到编译的目录下
            string filepath = System.IO.Path.Combine(Environment.CurrentDirectory, "Html", "index.html");
            webView.Source = new Uri(filepath);
            webView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
        }

        private void WebView_CoreWebView2InitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {

            if (webView != null && webView.CoreWebView2 != null)
            {

                //注册csobj脚本c#互操作
                webView.CoreWebView2.AddHostObjectToScript("csobj", new ScriptCallbackObject());
                //注册全局变量csobj
                webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("var csobj = window.chrome.webview.hostObjects.csobj;");
                webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("var csobj_sync= window.chrome.webview.hostObjects.sync.csobj;");

            }
        }

        /// <summary>
        /// 删除ComboBox已选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            tagCombo.SelectedIndex = -1;
        }

        private void menuBar_Selected(object sender, SelectionChangedEventArgs e)
        {
            Memo selectedItem = (Memo)memoList.SelectedItem;
            if (selectedItem != null)
            {
                loadMemo(selectedItem);
            }
        }

        private void loadMemo(Memo memo)
        {
            if (webView.CoreWebView2 != null)
            {
                webView.CoreWebView2.PostWebMessageAsJson(JsonSerializer.Serialize(memo));
            }
        }
    }
}
