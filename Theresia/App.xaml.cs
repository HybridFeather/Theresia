using System.Windows;
using Prism.Ioc;
using Prism.Unity;
using Theresia.Views.Controllers;
using Theresia.Services;
using Theresia.Views;
using Microsoft.EntityFrameworkCore;
using Theresia.Config;
using Theresia.Services.Interfaces;
using Theresia.Repositories.Interfaces;
using Theresia.Repositories;
using Theresia.Views.MediaManagement;
using Theresia.Scraper.Interfaces;
using Theresia.Scraper.Movie;
using Theresia.Views.Controllers.Settings;
using Microsoft.Extensions.Logging;

namespace Theresia
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {

        public static ILoggerFactory loggerFactory { get; private set; }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // 配置全局日志工厂
            loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddConsole()       // 控制台输出
                    .AddDebug()         // 调试输出（Visual Studio 的输出窗口）
                    .SetMinimumLevel(LogLevel.Debug); // 设置日志级别为 Debug
            });

            // 使用日志工厂来创建一个日志记录器
            var logger = loggerFactory.CreateLogger<App>();
            logger.LogInformation("应用程序启动");

            //注册数据库
            containerRegistry.Register<AppDbContext>(provider =>
            {
                var options = new DbContextOptionsBuilder<AppDbContext>()
                    .Options;
                return new AppDbContext(options);
            });

            //注册Repository
            containerRegistry.Register<ITagRepository, TagRepository>();
            containerRegistry.Register<ISettingRepository, SettingRepository>();
            containerRegistry.Register<IMovieRepository, MovieRepository>();
            containerRegistry.Register<IMovieTagsRepository, MovieTagsRepository>();
            containerRegistry.Register<IMovieCastRepository, MovieCastRepository>();
            containerRegistry.Register<ICastCrewRepository, CastCrewRepository>();
            containerRegistry.Register<ICastCrewPhotoRepository, CastCrewPhotoRepository>();
            containerRegistry.Register<ISeriesRepository, SeriesRepository>();
            containerRegistry.Register<IMediaMetadataRepository, MediaMetadataRepository>();
            // 注册服务和类型
            containerRegistry.Register<IMovieService, MovieService>();
            containerRegistry.Register<IDirectoryService, DirectoryService>();
            containerRegistry.Register<IMovieScraper, JavBusScraper>("JavBus");

            // 注册对话框
            //containerRegistry.RegisterForNavigation<TagsDialog>();

            // 注册侧边菜单栏 UserControl
            containerRegistry.RegisterForNavigation<SideMenu>();

            // 注册页面
            containerRegistry.RegisterForNavigation<HomePage>();
            containerRegistry.RegisterForNavigation<TagManagement>();
            containerRegistry.RegisterForNavigation<Settings>();
            containerRegistry.RegisterForNavigation<PathSetting>();
            containerRegistry.RegisterForNavigation<SystemSetting>();
            containerRegistry.RegisterForNavigation<MovieList>();
        }

        protected override Window CreateShell()
        {
            // 创建主窗口
            return Container.Resolve<MainWindow>();
        }

        protected override IContainerExtension CreateContainerExtension()
        {
            return new UnityContainerExtension();
        }
    }

}
