using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using Microsoft.Win32;
using Theresia.Common;
using Theresia.Entity;
using Theresia.Repositories.Interfaces;
using Theresia.Services.Interfaces;

namespace Theresia.ViewModels.Controllers.Settings
{
    public class PathSettingViewModel : BindableBase
    {
        /// <summary>
        /// 根目录
        /// </summary>
        private string _rootDirectory = "";

        /// <summary>
        /// 电影目录
        /// </summary>
        private string _movieDirecotry = "";

        /// <summary>
        /// 电影封面目录
        /// </summary>
        private string _movieCoverDirecotry = "";

        /// <summary>
        /// 演职人员照片目录
        /// </summary>
        private string _castCrewPhotoDirectory = "";

        /// <summary>
        /// 视频目录
        /// </summary>
        private string _videoDirectory = "";

        /// <summary>
        /// 视频封面目录
        /// </summary>
        private string _videoCoverDirectory = "";

        private ISettingRepository SettingRepository;
        private IDirectoryService directoryService;

        public string RootDirectory
        {
            get => _rootDirectory;
            set => SetProperty(ref _rootDirectory, value);
        }

        public string MovieDirectory
        {
            get => _movieDirecotry;
            set => SetProperty(ref _movieDirecotry, value);
        }

        public string MovieCoverDirectory
        {
            get => _movieCoverDirecotry;
            set => SetProperty(ref _movieCoverDirecotry, value);
        }

        public string CastCrewPhotoDirectory
        {
            get => _castCrewPhotoDirectory;
            set => SetProperty(ref _castCrewPhotoDirectory, value);
        }

        public string VideoDirectory
        {
            get => _videoDirectory;
            set => SetProperty(ref _videoDirectory, value);
        }

        public string VideoCoverDirectory
        {
            get => _videoCoverDirectory;
            set => SetProperty(ref _videoCoverDirectory, value);
        }

        public RelayCommand<string> SetDirectoryCmd => new((string type) =>
        {
            string directory;
            OpenFolderDialog openFolderDialog = new OpenFolderDialog
            {
                Title = "选择目录",
                Multiselect = false // 只允许选择一个文件
            };

            if (openFolderDialog.ShowDialog() == true) // 用户点击“确定”时
            {
                directory = openFolderDialog.FolderName;
                _ = type switch
                {
                    "Root" => RootDirectory = directory,
                    "MovieDirectory" => MovieDirectory = directory,
                    "MovieCover" => MovieCoverDirectory = directory,
                    "CastCrewPhoto" => CastCrewPhotoDirectory = directory,
                    "VideoDirectory" => VideoDirectory = directory,
                    "VideoCover" => VideoCoverDirectory = directory,
                    _ => null
                };
            }
        });


        public RelayCommand SaveSettingCmd => new(async () =>
        {
            var result = HandyControl.Controls.MessageBox.Ask("确定要保存设置吗", "保存设置");

            if (result == MessageBoxResult.OK)
            {
                Dictionary<string, string> dictionary = new()
                {
                    { AppConstant.ROOT_DIRECTORY, RootDirectory },
                    { AppConstant.MOVIE_DIRECTORY, MovieDirectory },
                    { AppConstant.MOVIE_COVER_DIRECTORY, MovieCoverDirectory },
                    { AppConstant.CAST_CREW_PHOTO_DIRECOTRY, CastCrewPhotoDirectory },
                    { AppConstant.VIDEO_DIRECOTRY, VideoDirectory },
                    { AppConstant.VIDEO_COVER_DIRECOTRY, VideoCoverDirectory }
                };
                bool succeed = await SettingRepository.UpdateSetting(dictionary);
                if (succeed)
                {
                    Growl.Success("保存成功");
                    directoryService.Initialize();
                }
                else
                {
                    Growl.Error("保存失败");
                }
            }
        });

        public PathSettingViewModel(ISettingRepository _SettingRepository,
            IDirectoryService _directoryService)
        {
            SettingRepository = _SettingRepository;
            directoryService = _directoryService;
            List<SettingEntity> list = SettingRepository.GetSettings().Result;
            RootDirectory = list.FirstOrDefault(s => s.Key == AppConstant.ROOT_DIRECTORY)?.Value ?? "";
            MovieDirectory = list.FirstOrDefault(s => s.Key == AppConstant.MOVIE_DIRECTORY)?.Value ?? "";
            MovieCoverDirectory = list.FirstOrDefault(s => s.Key == AppConstant.MOVIE_COVER_DIRECTORY)?.Value ?? "";
            CastCrewPhotoDirectory =
                list.FirstOrDefault(s => s.Key == AppConstant.CAST_CREW_PHOTO_DIRECOTRY)?.Value ?? "";
            VideoDirectory = list.FirstOrDefault(s => s.Key == AppConstant.VIDEO_DIRECOTRY)?.Value ?? "";
            VideoCoverDirectory = list.FirstOrDefault(s => s.Key == AppConstant.VIDEO_COVER_DIRECOTRY)?.Value ?? "";
        }
    }
}
