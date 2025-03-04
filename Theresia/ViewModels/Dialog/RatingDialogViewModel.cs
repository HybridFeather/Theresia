using System.Diagnostics;
using GalaSoft.MvvmLight.Command;
using HandyControl.Tools.Extension;
using Prism.Mvvm;

namespace Theresia.ViewModels.Dialog
{
    public class RatingDialogViewModel : BindableBase, IDialogResultable<double>
    {
        private double _result;

        public double Result
        {
            get => _result;
            set
            {
                SetProperty(ref _result, value);
            }
        }
        public Action CloseAction { get; set; }
        public RelayCommand CloseCmd => new(() => CloseAction?.Invoke());


    }
}
