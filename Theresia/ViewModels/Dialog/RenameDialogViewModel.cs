using HandyControl.Tools.Extension;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using NStandard;
using System.Windows;

namespace Theresia.ViewModels.Dialog
{
    public class RenameDialogViewModel : BindableBase, IDialogResultable<string>
    {
        private string _result;

        public string Result
        {
            get => _result;
            set
            {
                SetProperty(ref _result, value);
            }
        }
        public Action CloseAction { get; set; }
        public RelayCommand CloseCmd => new(() =>
        {
            if (string.IsNullOrEmpty(Result))
            {
                HandyControl.Controls.MessageBox.Show($"文件名不能为空", "提示", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            else
            {
                CloseAction?.Invoke();
            }
            
        });


    }
}
