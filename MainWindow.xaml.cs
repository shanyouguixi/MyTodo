using MaterialDesignThemes.Wpf;
using MyMemo.Common.DialogUtils;
using MyMemo.Common.Extendsions;
using MyMemo.Common.Model;
using MyTodo.Common.Model;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
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
        private static NotifyIcon _notifyIcon = null;

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
                InitialTray();
                this.WindowState = WindowState.Minimized;
                this.Visibility = Visibility.Hidden;

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

        #region 最小化系统托盘
        private void InitialTray()
        {
            //隐藏主窗体
            this.Visibility = Visibility.Hidden;
            //设置托盘的各个属性
            if(_notifyIcon == null)
            {
                _notifyIcon = new NotifyIcon();
            }
            _notifyIcon.BalloonTipText = "最小化到托盘";//托盘气泡显示内容
            _notifyIcon.Text = "Todo";
            _notifyIcon.Visible = true;//托盘按钮是否可见
            _notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath); //托盘中显示的图标
            _notifyIcon.ShowBalloonTip(500);//托盘气泡显示时间
            _notifyIcon.MouseDoubleClick += notifyIcon_MouseDoubleClick;
            _notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(notifyIcon_MouseClick);
        }
        #endregion


        /// <summary>  
        /// 鼠标单击  
        /// </summary>  
        /// <param name="sender"></param>  
        /// <param name="e"></param>  
        private void notifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //如果鼠标左键单击  
            if (e.Button == MouseButtons.Right)
            {
                System.Windows.Controls.ContextMenu NotifyIconMenu = (System.Windows.Controls.ContextMenu)this.FindResource("NotifyIconMenu");
                NotifyIconMenu.IsOpen = true;
            }
        }


        #region 托盘图标鼠标单击事件
        private void notifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (this.Visibility == Visibility.Visible)
                {
                    this.Visibility = Visibility.Hidden;
                }
                else
                {
                    this.Visibility = Visibility.Visible;
                    this.WindowState = WindowState.Normal;
                    this.Activate();
                }
            }
        }
        #endregion



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

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            if (this.Visibility == Visibility.Visible)
            {
                this.Visibility = Visibility.Hidden;
            }
            else
            {
                this.Visibility = Visibility.Visible;
                this.WindowState = WindowState.Normal;
                this.Activate();
            }
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }


   
}
