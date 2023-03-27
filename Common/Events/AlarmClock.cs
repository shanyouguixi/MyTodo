using MyMemo.Common.Extendsions;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace MyTodo.Common.Events
{
    public class AlarmClock
    {
        private readonly IEventAggregator aggregator;
        public DateTime? SelectedDateTime;
        private string Message;
        private BackgroundWorker _alarmWorker;
        private NotifyIcon _notifyIcon = null;
        public AlarmClock(IEventAggregator aggregator,DateTime? dateTime, string message = null)
        {
            this.aggregator = aggregator;
            SelectedDateTime = dateTime;
            Message = message;
        }
        public void SetAlarm()
        {
            if (!CanSetAlarm())
            {
                return;
            }
            if (_alarmWorker != null && _alarmWorker.IsBusy)
            {
                _alarmWorker.CancelAsync();
            }

            _alarmWorker = new BackgroundWorker();
            _alarmWorker.DoWork += OnAlarmDoWork;
            _alarmWorker.WorkerSupportsCancellation = true;
            _alarmWorker.RunWorkerAsync();
        }

        private void CancelAlarm()
        {
            if (_alarmWorker != null && _alarmWorker.IsBusy)
            {
                _alarmWorker.CancelAsync();
            }
        }

        private bool CanSetAlarm()
        {
            if(aggregator == null)
            {
                return false;
            }
            bool flag = SelectedDateTime > DateTime.Now;
            return flag;
        }

        private bool CanCancelAlarm()
        {
            return _alarmWorker != null && _alarmWorker.IsBusy;
        }

        private void OnAlarmDoWork(object sender, DoWorkEventArgs e)
        {
            while (DateTime.Now < SelectedDateTime)
            {
                if (_alarmWorker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
            }

            if (!e.Cancel)
            {
                _notifyIcon = new NotifyIcon();
                _notifyIcon.BalloonTipText = Message;//托盘气泡显示内容
                _notifyIcon.Text = "Todo";
                _notifyIcon.Visible = true;//托盘按钮是否可见
                _notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath); //托盘中显示的图标
                _notifyIcon.ShowBalloonTip(2000);//托盘气泡显示时间
                
                this.aggregator.SetFlash("TodoClock");
            }
        }

        private DateTime strToTime(string dateString)
        {
            DateTime date = DateTime.ParseExact(dateString, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
            return date;
        }
    }

}
