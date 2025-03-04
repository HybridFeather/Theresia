using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Prism.Mvvm;
using Theresia.Common;
using Theresia.Entity;
using Theresia.Enums;
using Theresia.Repositories.Interfaces;

namespace Theresia.ViewModels.Controllers.Settings
{
    public class SystemSettingViewModel : BindableBase
    {
        private readonly ISettingRepository settingRepository;

        /// <summary>
        /// 最小化选项的值
        /// </summary>
        private bool _closeMinimizesValue = false;
        public bool CloseMinimizesValue
        {
            get => _closeMinimizesValue;
            set => SetProperty(ref _closeMinimizesValue, value);
        }

        /// <summary>
        /// 最小化选项开关
        /// </summary>
        private bool _closeMinimizesEnable = true;
        public bool CloseMinimizesEnable
        {
            get => _closeMinimizesEnable;
            set => SetProperty(ref _closeMinimizesEnable, value);
        }

        /// <summary>
        /// 最小化按钮变动事件
        /// </summary>
        public ICommand CloseMinimizesChangedCmd { get; set; }


        public SystemSettingViewModel(ISettingRepository _settingRepository)
        {
            settingRepository = _settingRepository;

            CloseMinimizesChangedCmd = new RelayCommand(CloseMinimizesChangedEvent);


            Initialize();
        }

        private async void Initialize()
        {
            List<SettingEntity> list = await settingRepository.GetSettingsByTypeAsync(SettingTypeEnum.System);
            SettingEntity closeMinimize = list.FirstOrDefault(s => s.Key == AppConstant.CLOSE_MINIMIZES);
            CloseMinimizesValue = closeMinimize.Value == "1";
        }

        private async void CloseMinimizesChangedEvent()
        {
            try
            {
                CloseMinimizesEnable = false;
                SettingEntity closeMinimize = await settingRepository.GetSettingByKey(AppConstant.CLOSE_MINIMIZES);
                closeMinimize.Value = CloseMinimizesValue ? "1" : "0";
                await settingRepository.UpdateAsync(closeMinimize);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex}");
            }
            finally
            {
                CloseMinimizesEnable = true;
            }
        }
    }
}
