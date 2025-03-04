using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theresia.Common;
using Theresia.Entity;
using Theresia.Enums;
using Theresia.Repositories;
using Theresia.Services.Interfaces;

namespace Theresia.Services
{
    public class DirectoryService : IDirectoryService
    {
        private SettingRepository SettingRepository;
        private static Dictionary<string, SettingEntity> dictionary = new Dictionary<string, SettingEntity>();
        public string GetCastCrewDirectory()
        {
            SettingEntity entity = dictionary[AppConstant.CAST_CREW_PHOTO_DIRECOTRY];
            if (string.IsNullOrEmpty(entity.Value))
            {
                return Path.Combine(GetRootDirectory(), entity.Default);
            }
            else
            {
                return entity.Value;
            }
        }

        public string GetMovieCoverDirectory()
        {
            SettingEntity entity = dictionary[AppConstant.MOVIE_COVER_DIRECTORY];
            if (string.IsNullOrEmpty(entity.Value))
            {
                return Path.Combine(GetMovieDirectory(),entity.Default);
            }
            else
            {
                return entity.Value;
            }
        }

        public string GetMovieDirectory()
        {
            SettingEntity entity = dictionary[AppConstant.MOVIE_DIRECTORY];
            if (string.IsNullOrEmpty(entity.Value))
            {
                return Path.Combine(GetRootDirectory(), entity.Default);
            }
            else
            {
                return entity.Value;
            }
        }

        public string GetRootDirectory()
        {
            SettingEntity entity = dictionary[AppConstant.ROOT_DIRECTORY];
            if (string.IsNullOrEmpty(entity.Value))
            {
                return string.Empty;
            }
            else
            {
                return entity.Value;
            }
        }

        public string GetVideoCoverDirectory()
        {
            SettingEntity entity = dictionary[AppConstant.VIDEO_COVER_DIRECOTRY];
            if (string.IsNullOrEmpty(entity.Value))
            {
                return Path.Combine(GetVideoDirectory(), entity.Default);
            }
            else
            {
                return entity.Value;
            }
        }

        public string GetVideoDirectory()
        {
            SettingEntity entity = dictionary[AppConstant.VIDEO_DIRECOTRY];
            if (string.IsNullOrEmpty(entity.Value))
            {
                return Path.Combine(GetRootDirectory(), entity.Default);
            }
            else
            {
                return entity.Value;
            }
        }

        public void Initialize()
        {
            dictionary.Clear();
            List<SettingEntity> list = SettingRepository.GetSettingsByTypeAsync(SettingTypeEnum.Path).Result;
            SettingEntity? rootEntity = list.FirstOrDefault(e => e.Key == AppConstant.ROOT_DIRECTORY);
            SettingEntity? movieDirectoryEntity = list.FirstOrDefault(e => e.Key == AppConstant.MOVIE_DIRECTORY);
            SettingEntity? movieCoverDirectoryEntity = list.FirstOrDefault(e => e.Key == AppConstant.MOVIE_COVER_DIRECTORY);
            SettingEntity? vidoeDirectoryEntity = list.FirstOrDefault(e => e.Key == AppConstant.VIDEO_DIRECOTRY);
            SettingEntity? videoCoverDirectoryEntity = list.FirstOrDefault(e => e.Key == AppConstant.VIDEO_COVER_DIRECOTRY);
            SettingEntity? castCrewDirectory = list.FirstOrDefault(e => e.Key == AppConstant.CAST_CREW_PHOTO_DIRECOTRY);
            dictionary.Add(AppConstant.ROOT_DIRECTORY, rootEntity);
            dictionary.Add(AppConstant.MOVIE_DIRECTORY, movieDirectoryEntity);
            dictionary.Add(AppConstant.MOVIE_COVER_DIRECTORY, movieCoverDirectoryEntity);
            dictionary.Add(AppConstant.VIDEO_DIRECOTRY, vidoeDirectoryEntity);
            dictionary.Add(AppConstant.VIDEO_COVER_DIRECOTRY, videoCoverDirectoryEntity);
            dictionary.Add(AppConstant.CAST_CREW_PHOTO_DIRECOTRY, castCrewDirectory);
        }

        public DirectoryService(SettingRepository _SettingRepository)
        {
            SettingRepository = _SettingRepository;
        }
    }
}
