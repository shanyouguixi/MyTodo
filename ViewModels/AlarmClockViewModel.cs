using MyTodo.Common.Model;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MyTodo.ViewModels
{
    public class AlarmClockViewModel : BindableBase
    {
        private bool isOpen;

        public DateTime SelectedDateTime;

        private string _selectedDateTimeStr = "2023-03-25 19:02:00";

        public string SelectedDateTimeStr
        {
            get { return _selectedDateTimeStr; }
            set { _selectedDateTimeStr = value; RaisePropertyChanged(); }
        }


        public DelegateCommand SetAlarmCommand { get => _setAlarmCommand; set => _setAlarmCommand = value; }
        public DelegateCommand CancelAlarmCommand { get => _cancelAlarmCommand; set => _cancelAlarmCommand = value; }
        public bool IsOpen { get => isOpen; set { isOpen = value; RaisePropertyChanged(); } }

        private DelegateCommand _setAlarmCommand;


        private DelegateCommand _cancelAlarmCommand;


        private BackgroundWorker _alarmWorker;
        public AlarmClockViewModel()
        {
            SetAlarmCommand = new DelegateCommand(SetAlarm);
            CancelAlarmCommand = new DelegateCommand(CancelAlarm);
        }

        private void SetAlarm()
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
            SelectedDateTime = strToTime(SelectedDateTimeStr);
            return SelectedDateTime > DateTime.Now;
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
                // 播放闹钟音乐
                MessageBox.Show("闹钟时间到了。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private DateTime strToTime(string dateString)
        {
            DateTime date = DateTime.ParseExact(dateString, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
            return date;
        }
    }
}
