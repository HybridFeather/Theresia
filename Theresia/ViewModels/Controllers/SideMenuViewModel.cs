using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Data;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace Theresia.ViewModels.Controllers
{
    public class SideMenuViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        // 命令，用于触发页面切换
        public ICommand NavigateCommand { get; private set; }
        public SideMenuViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            // 初始化命令
            NavigateCommand = new DelegateCommand<string>(OnNavigate);
        }

        // 页面切换逻辑
        private void OnNavigate(string viewName)
        {
            // 通过 RegionManager 激活对应的视图
            _regionManager.RequestNavigate("MainRegion", viewName);
        }

        public RelayCommand<FunctionEventArgs<object>> SwitchItemCmd => new(SwitchItem);

        private void SwitchItem(FunctionEventArgs<object> info)
        {
            //Growl.Info((info.Info as SideMenuItem)?.Header.ToString());
            //Growl.Success("测试TEST");
        } 

        public RelayCommand<string> SelectCmd => new(Select);

        private void Select(string header) => Growl.Success(header);
    }
}
