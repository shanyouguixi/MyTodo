using MyMemo.Common.DialogUtils;
using MyMemo.ViewModels;
using MyMemo.Views;
using MyMemo.Views.Dialog;
using MyTodo.ViewModels;
using MyTodo.Views;
using Prism.DryIoc;
using Prism.Ioc;
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



        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<IndexView,IndexViewModel>();
            containerRegistry.RegisterForNavigation<MemosView, MemosViewModel>();
  
            containerRegistry.RegisterForNavigation<TodoView,TodoViewModel>();
            containerRegistry.RegisterForNavigation<SettingView,SettingViewModel>();
            containerRegistry.RegisterForNavigation<MainWindow, MainWindowModel>();
            containerRegistry.RegisterForNavigation<MsgView, MsgViewModel>();
            containerRegistry.Register<IDialogHostService, DialogHostService>();

            containerRegistry.RegisterDialog<LoginView,LoginViewModel>();
        }


 
    }
}
