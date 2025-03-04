using HandyControl.Data;
using System.Windows;
using Theresia.Common;
using Theresia.Entity;
using Theresia.Enums;
using Theresia.Repositories.Interfaces;

namespace Theresia.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ISettingRepository settingRepository;
        public MainWindow(ISettingRepository _settingRepository)
        {
            InitializeComponent();
            settingRepository = _settingRepository;
            //var viewModel = (MainWindowViewModel)this.DataContext;
            //viewModel.OnAppStart();
        }

        // 处理窗口关闭事件
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 取消窗口关闭操作，防止程序退出
            e.Cancel = true;
            List<SettingEntity> list = settingRepository.GetSettingsByTypeAsync(SettingTypeEnum.System).Result;
            SettingEntity closeMinimize = list.FirstOrDefault(s => s.Key == AppConstant.CLOSE_MINIMIZES);
            SettingEntity closeOperation = list.FirstOrDefault(s => s.Key == AppConstant.CLOSE_OPERATE_INITIALIZE);
            if (closeOperation.Value == "0")
            {
                var result = HandyControl.Controls.MessageBox.Show($"正在进行关闭操作，是否要最小化到系统托盘？（该选项可在系统设置里进行修改）", "提示", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    closeOperation.Value = "1";
                    closeMinimize.Value = "1";
                    bool a =settingRepository.UpdateAsync(closeOperation).Result;
                    bool b = settingRepository.UpdateAsync(closeMinimize).Result;
                    this.Hide();  // 窗口最小化时隐藏窗口
                    TrayIcon.ShowBalloonTip("应用最小化", "程序已最小化到系统托盘", NotifyIconInfoType.Info);
                } else if (result == MessageBoxResult.Cancel)
                {
                    
                } else if (result == MessageBoxResult.No)
                {
                    closeOperation.Value = "1";
                    bool a = settingRepository.UpdateAsync(closeOperation).Result;
                    this.Close();
                }
            }
            else
            {
                if (closeMinimize.Value == "1")
                {
                    this.Hide();  // 窗口最小化时隐藏窗口
                    TrayIcon.ShowBalloonTip("应用最小化", "程序已最小化到系统托盘", NotifyIconInfoType.Info);
                }
                else
                {
                    TrayIcon.Dispose();  // 退出前清理托盘图标
                    Application.Current.Shutdown();  // 关闭程序
                }
            }
        }

        // 处理窗口状态改变事件
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.Hide();  // 窗口最小化时隐藏窗口
                TrayIcon.ShowBalloonTip("应用最小化", "程序已最小化到系统托盘", NotifyIconInfoType.Info);
            }
        }

        // 处理托盘图标的双击事件
        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
        }
    }
}