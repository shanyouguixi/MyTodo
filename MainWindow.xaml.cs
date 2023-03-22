using MaterialDesignThemes.Wpf;
using MyMemo.Common.DialogUtils;
using MyMemo.Common.Extendsions;
using MyMemo.Common.Model;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Windows.Threading;

namespace MyMemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IDialogHostService dialogHostService;

        private PackIcon menuPackIcon;
        private PackIcon windowPackIcon;

        private bool menuOpen = true;
        private DispatcherTimer leftMenuTimer;

        private bool timerFlag = false;
        private int leftMenuMaxWidth = 150;
        private int leftMenuMinWidth = 50;
        private readonly IRegionManager regionManager;

        public bool MenuOpen
        {
            get { return menuOpen = true; }
            set { menuOpen = value; }
        }

        public PackIcon MenuPackIcon { get => menuPackIcon; set => menuPackIcon = value; }
        public PackIcon WindowPackIcon { get => windowPackIcon; set => windowPackIcon = value; }
        public DispatcherTimer LeftMenuTimer { get => leftMenuTimer; set => leftMenuTimer = value; }
        public bool TimerFlag { get => timerFlag; set => timerFlag = value; }

        public MainWindow(IEventAggregator aggregator, IDialogHostService dialogHostService, IRegionManager regionManager)
        {
            InitializeComponent();
            this.dialogHostService = dialogHostService;
            MenuPackIcon = new PackIcon();
            WindowPackIcon = new PackIcon();
            LeftMenuTimer = new DispatcherTimer();
            leftMenuTimer.Interval = TimeSpan.FromMilliseconds(1);
            leftMenuTimer.Tick += timer_Tick;
            aggregator.ResgiterMessage(arg =>
            {
                if("Main".Equals(arg.Filter))
                {
                    Snackbar.MessageQueue.Enqueue(arg.Message);
                }
                
            });
            aggregator.ResgiterFlash(arg =>
            {
                if ("GlogalWorkspace".Equals(arg.Name))
                {
                    tagCombo.SelectedIndex = 0;
                }
            });

            btnClose.Click += async (s, e) =>
            {
                var dialogResult = await dialogHostService.Question("温馨提示", "确认退出系统?");
                if (dialogResult.Result != Prism.Services.Dialogs.ButtonResult.OK) return;
                this.Close();
            };
            this.regionManager = regionManager;

            ColorZone.MouseMove += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    this.DragMove();
            };
            ColorZone.MouseDoubleClick += (s, e) =>
            {
                if (this.WindowState == WindowState.Normal)
                {
                    this.WindowState = WindowState.Maximized;
                    WindowPackIcon.Kind = PackIconKind.DockWindow;
                }

                else
                {
                    this.WindowState = WindowState.Normal;
                    WindowPackIcon.Kind = PackIconKind.WindowMaximize;
                }
                btnMax.Content = WindowPackIcon;

            };

        }



        /// <summary>
        /// 打开/关闭菜单栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void topMenu_Click(object sender, RoutedEventArgs e)
        {
            if (menuOpen)
            {
                MenuPackIcon.Kind = PackIconKind.MenuOpen;
                topMenu.Content = MenuPackIcon;
                MenuPackIcon.Width = 30;
                MenuPackIcon.Height = 30;
                //LeftMenu.Width = 0;
            }
            else
            {
                MenuPackIcon.Kind = PackIconKind.Menu;
                topMenu.Content = MenuPackIcon;
                MenuPackIcon.Width = 30;
                MenuPackIcon.Height = 30;
                //LeftMenu.Width= 200;
            }
            timerFlag = false;
            leftMenuTimer.Start();
        }

        /// <summary>
        /// 定时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_Tick(object sender, EventArgs e)
        {
            double tempLen = LeftMenu.Width;
            if (timerFlag)
            {
                leftMenuTimer.Stop();
                return;
            }
            int tempWidth = 15;
            if (menuOpen)
            {
                tempWidth = -15;
            }
            tempLen += tempWidth;
            if (tempLen > leftMenuMaxWidth)
            {
                LeftMenu.Width = leftMenuMaxWidth;
                timerFlag = true;
                menuOpen = true;
            }
            else if (tempLen < leftMenuMinWidth)
            {
                LeftMenu.Width = leftMenuMinWidth;
                timerFlag = true;
                menuOpen = false;
            }
            else
            {
                LeftMenu.Width = tempLen;
            }
        }



        /// <summary>
        /// 窗口最小化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        /// <summary>
        /// 窗口最大化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMax_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                WindowPackIcon.Kind = PackIconKind.WindowMaximize;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                WindowPackIcon.Kind = PackIconKind.DockWindow;
            }
            btnMax.Content = WindowPackIcon;
        }

    }
}
