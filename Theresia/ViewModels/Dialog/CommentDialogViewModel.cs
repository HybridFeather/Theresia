using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Tools.Extension;
using Prism.Mvvm;

namespace Theresia.ViewModels.Dialog
{
    public class CommentDialogViewModel : BindableBase, IDialogResultable<string>
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
        public RelayCommand CloseCmd => new(() => CloseAction?.Invoke());


    }
}
