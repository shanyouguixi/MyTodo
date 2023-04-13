using MyMemo.Common.DialogUtils;
using MyMemo.Common.Extendsions;
using MyMemo.ViewModels;
using MyMemo.Views;
using MyMemo.Views.Dialog;
using MyTodo.Common.Cache;
using MyTodo.Common.Model;
using MyTodo.ViewModels;
using MyTodo.Views;
using MyTodo.Views.Dialog;
using Newtonsoft.Json;
using Prism.DryIoc;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyMemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }
        protected override void OnInitialized()
        {
            if (isLogin())
            {
                base.OnInitialized();
            }
            else
            {
                var dialog = Container.Resolve<IDialogService>();
                dialog.ShowDialog("LoginView", callback =>
                {
                    if (callback.Result != ButtonResult.OK)
                    {
                        Environment.Exit(0);
                        return;
                    }

                    var service = App.Current.MainWindow.DataContext as IConfigureService;
                    if (service != null)
                        service.Configure();
                    base.OnInitialized();
                });
            }
        }

        private bool isLogin()
        {
            string userString = CacheManager.IniReadvalue("USERINFO", "user");
            string tokenString = CacheManager.IniReadvalue("USERINFO", "JSESSIONID");
            if (!string.IsNullOrWhiteSpace(userString) && !string.IsNullOrWhiteSpace(tokenString))
            {
                User user = JsonConvert.DeserializeObject<User>(userString);
                Application.Current.Properties["JSESSIONID"] = tokenString;
                Application.Current.Properties["user"] = user;
                return true;
            }
            else
            {
                return false;
            }
        }


        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<IndexView, IndexViewModel>();
            containerRegistry.RegisterForNavigation<MemosView, MemosViewModel>();

            containerRegistry.RegisterForNavigation<TodoView, TodoViewModel>();
            containerRegistry.RegisterForNavigation<SettingView, SettingViewModel>();
            containerRegistry.RegisterForNavigation<MainWindow, MainWindowModel>();
            containerRegistry.RegisterForNavigation<MsgView, MsgViewModel>();
            containerRegistry.RegisterForNavigation<AlarmClockView, AlarmClockViewModel>();
            containerRegistry.RegisterForNavigation<EbookView, EbookViewModel>();
            containerRegistry.Register<IDialogHostService, DialogHostService>();

            containerRegistry.RegisterDialog<LoginView, LoginViewModel>();
        }



    }
}
