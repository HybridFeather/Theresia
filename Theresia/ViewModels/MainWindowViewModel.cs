using System.Diagnostics;
using System.IO;
using System.Windows;
using HandyControl.Controls;
using Microsoft.Win32;
using Prism.Mvvm;
using Theresia.Common;
using Theresia.Config;
using Theresia.Entity;
using Theresia.Repositories;
using Theresia.Repositories.Interfaces;
using Theresia.Services;
using Theresia.Services.Interfaces;
using Theresia.ViewModels.Controllers;

namespace Theresia.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Theresia";//标题
        private double _screenWidth = SystemParameters.PrimaryScreenWidth;//用户屏幕宽度
        private double _screenHeight = SystemParameters.PrimaryScreenHeight;//用户屏幕高度
        private IDirectoryService directoryService;
        private ISettingRepository settingRepository;
        private AppDbContext context;

        public string title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }//set这么写是因为了Prism框架绑定数据,这样就能实时变动数据
        }

        public double screenWidth
        {
            get { return _screenWidth * 0.8; }
            set { SetProperty(ref _screenWidth, value); }
        }

        public double screenHeight
        {
            get { return _screenHeight * 0.7; }
            set { SetProperty(ref _screenHeight, value); }
        }

        public MainWindowViewModel(IDirectoryService _directoryService, AppDbContext _context,ISettingRepository _settingRepository)
        {
            context = _context;
            directoryService = _directoryService;
            settingRepository = _settingRepository;

            OnAppStart();
        }

        // 启动时自动执行
        public void OnAppStart()
        {
            InitializeCache();
            InitializeRootDirectory();
        }

        /// <summary>
        /// 初始化根目录
        /// </summary>
        private void InitializeRootDirectory()
        {
            string rootDirectory = directoryService.GetRootDirectory();
            if (string.IsNullOrEmpty(rootDirectory))
            {
                var result = HandyControl.Controls.MessageBox.Show($"请先设置根目录\n点取消则退出本程序", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.OK)
                {
                    OpenFolderDialog openFolderDialog = new OpenFolderDialog
                    {
                        Title = "选择目录",
                        Multiselect = false // 只允许选择一个文件
                    };

                    if (openFolderDialog.ShowDialog() == true) // 用户点击“确定”时
                    {
                        Dictionary<string, string> dictionary = new()
                        {
                            { AppConstant.ROOT_DIRECTORY, openFolderDialog.FolderName }
                        };
                        settingRepository.UpdateSetting(dictionary);
                        directoryService.Initialize();
                        string movieDirectory = directoryService.GetMovieDirectory();
                        Directory.CreateDirectory(movieDirectory);
                        HandyControl.Controls.MessageBox.Show($"设置成功，将影片放到{movieDirectory}则可自动识别", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    Application.Current.Shutdown();  // 关闭程序
                }
            }
        }

        
        /// <summary>
        /// 初始化一些缓存
        /// </summary>
        private void InitializeCache()
        {
            directoryService.Initialize();
            foreach (var item in context.Tag.ToList())
            {
                CommonCache.TAG_CACHE.Add(item);
            }
            foreach (var item in context.CastCrew.ToList())
            {
                CommonCache.CAST_CACHE.Add(item);
            }
            foreach (var item in context.Series.ToList())
            {
                CommonCache.SERIES_CACHE.Add(item);
            }
            foreach (var item in context.MediaMetadata.ToList())
            {
                CommonCache.MEDIA_METADATA_CACHE.TryAdd(item.Sha256, item);
            }

            CommonCache.RefreshCastCache();
        }
    }
}
