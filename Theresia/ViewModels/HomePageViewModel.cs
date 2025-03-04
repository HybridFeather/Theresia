using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using Theresia.Services;
using Theresia.Services.Interfaces;

namespace Theresia.ViewModels
{
    internal class HomePageViewModel : BindableBase
    {
        private IMovieService excelService;
        const string UploadingContent = "解析中...";
        const string UploadedContent = "上传";
        private bool _isUploading = false;//上传状态
        private bool _uploadButtonEnable = true;//按钮状态
        private string _buttonContent = UploadedContent;//上传按钮文本
        private int _numberPreRow = 5;//每行座位数
        private string _textBoxContent = "李炫君,赵礼杰,李汭粲,朴到贤,田野,余峻嘉|Canyon,Showmaker,Ghost,Beryl,测试,Naguri" +
                                                                  "\n姜承録,高振宁,宋义进,喻文波,王柳亦|高天亮,金泰相,林伟祥,刘青松";//文本框内容
        private string _textBoxPlaceHolder = "请输入座位顺序，格式:\n李炫君,赵礼杰,李汭粲,朴到贤,田野,余峻嘉|Canyon,Showmaker,Ghost,Beryl,测试,Naguri" +
                                                                  "\n姜承録,高振宁,宋义进,喻文波,王柳亦|高天亮,金泰相,林伟祥,刘青松";//文本框提示词
        public bool IsUploading
        {
            get { return _isUploading; }
            set { SetProperty(ref _isUploading, value); }
        }

        public string ButtonContent
        {
            get { return _buttonContent; }
            set { SetProperty(ref _buttonContent, value); }
        }

        public bool UploadButtonEnable
        {
            get { return _uploadButtonEnable; }
            set { SetProperty(ref _uploadButtonEnable, value); }
        }

        public string TextBoxContent
        {
            get { return _textBoxContent; }
            set { SetProperty(ref _textBoxContent, value); }
        }

        public string TextBoxPlaceHoder
        {
            get { return _textBoxPlaceHolder; }
            set { SetProperty(ref _textBoxPlaceHolder, value); }
        }
        public int NumberPreRow
        {
            get { return _numberPreRow; }
            set { SetProperty(ref _numberPreRow, value); }
        }
        /// <summary>
        /// 上传按钮事件
        /// </summary>
        public DelegateCommand UploadButtonClickEvent { get; }
        /// <summary>
        /// 
        /// </summary>
        public DelegateCommand CreateButtonClickEvent { get; }

        private async void _uploadButtonClickEvent()
        {
            string fileName;
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "选择文件",
                Filter = "Excel文件 (*.xlsx;*.xls)|*.xlsx;*.xls;",
                Multiselect = false // 只允许选择一个文件
            };

            if (openFileDialog.ShowDialog() == true) // 用户点击“确定”时
            {
                ButtonContent = UploadingContent;
                IsUploading = true;
                UploadButtonEnable = false;
                await Task.Delay(5000);
                try
                {

                    await analyzeExcel();
                }
                finally
                {
                    ButtonContent = UploadedContent;
                    IsUploading = false;
                    UploadButtonEnable = true;
                }
                fileName = openFileDialog.FileName;
                Debug.WriteLine(openFileDialog);
                Growl.Success($"选中的文件: {fileName}");
            }
        }

        /// <summary>
        /// 生成按钮
        /// </summary>
        private void _createButtonClickEvent()
        {
            if(_textBoxContent.Length == 0)
            {
                MessageBox.Show("文本不能为空", "提示");
                return;
            }
            //excelService.createExcel(_textBoxContent, _numberPreRow, "");
            //MessageBox.Show(excelService.createExcel(_textBoxContent,_numberPreRow,""));

        }

        /// <summary>
        /// 解析
        /// </summary>
        private async Task analyzeExcel()
        {

        }


        public HomePageViewModel(IMovieService _excelService)
        {
            excelService = _excelService;
            UploadButtonClickEvent = new DelegateCommand(_uploadButtonClickEvent);
            CreateButtonClickEvent = new DelegateCommand(_createButtonClickEvent);
        }
    }
}
