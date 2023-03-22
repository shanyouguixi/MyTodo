using MyMemo.Common.DialogUtils;
using MyMemo.ViewModels;
using MyMemo.Views;
using MyMemo.Views.Dialog;
using Prism.DryIoc;
using Prism.Ioc;
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

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<IndexView>();
            containerRegistry.RegisterForNavigation<MemosView, MemosViewModel>();
  
            containerRegistry.RegisterForNavigation<SettingView,SettingViewModel>();
            containerRegistry.RegisterForNavigation<MainWindow, MainWindowModel>();
            containerRegistry.RegisterForNavigation<MsgView, MsgViewModel>();
            containerRegistry.Register<IDialogHostService, DialogHostService>();
        }


        protected override void OnInitialized() {
            var service = App.Current.MainWindow.DataContext as IConfigureService;
            if (service != null)
                service.Configure();

            base.OnInitialized();
        }
    }
}
