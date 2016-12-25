using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Poc.ProgressBar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double TASKBAR_PROGRESS_VALUE_MAX = 1.0;
        private const int PROGRESS_BAR_PERCENTAGE_MIN = 0;
        private const int PROGRESS_BAR_PERCENTAGE_MAX = 100;
        private const int THREAD_DEFAULT_SLEEP_MILLIAN_SECOND = 100;

        private BackgroundWorker _testWorker = new BackgroundWorker();
        private bool _isPause = false;
        private bool _isCancel = false;

        public MainWindow()
        {
            InitializeComponent();

            _testWorker.WorkerReportsProgress = true;
            _testWorker.WorkerSupportsCancellation = true;
            _testWorker.DoWork += _testWorker_DoWork;
            _testWorker.ProgressChanged += _testWorker_ProgressChanged;
            _testWorker.RunWorkerCompleted += _testWorker_RunWorkerCompleted;
            _testWorker.RunWorkerAsync();
        }



        private void _testWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int i = 0;
            while ((i <= PROGRESS_BAR_PERCENTAGE_MAX) && (!_isPause) && (!_isCancel))
            {
                i++;
                System.Threading.Thread.Sleep(THREAD_DEFAULT_SLEEP_MILLIAN_SECOND);
                _testWorker.ReportProgress(i);
            }
        }
        private void _testWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progTest.Value = e.ProgressPercentage;
            TaskbarItemInfo.ProgressValue = e.ProgressPercentage / PROGRESS_BAR_PERCENTAGE_MAX;
        }

        private void _testWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_isCancel)
            {
                
                TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Error;
                if (e.Error != null)
                {
                    MessageBox.Show(e.Error.Message);
                }
                MessageBox.Show("Application Cancel.");
                Application.Current.Shutdown();
            }
            else if (_isPause)
            {
                TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Paused;
                MessageBox.Show("Application Pause.");
            }
            else
            {
                TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Indeterminate;
                if (MessageBox.Show("Load completed, close application now?", "Exit Application", MessageBoxButton.YesNo)
                    .Equals(MessageBoxResult.Yes))
                {
                    Application.Current.Shutdown();
                }
            }
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            if (_isPause)
            {
                _isPause = false;
                TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
            }
            else
            {
                _isPause = true;
                TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Paused;
            }
        }

        private void btnError_Click(object sender, RoutedEventArgs e)
        {
            TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Error;
            _isCancel = true;
            _testWorker.CancelAsync();

        }
    }
}
