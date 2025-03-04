using System.ComponentModel;

namespace Theresia.ViewModels.MediaManagement
{
    public class MovieModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 标题
        /// </summary>
        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }
        /// <summary>
        /// 番号
        /// </summary>
        private string _code;
        public string Code
        {
            get { return _code; }
            set
            {
                if (_code != value)
                {
                    _code = value;
                    OnPropertyChanged(nameof(Code));
                }
            }
        }
        /// <summary>
        /// 封面
        /// </summary>
        private string _cover;
        public string Cover
        {
            get { return _cover; }
            set
            {
                if (_cover != value)
                {
                    _cover = value;
                    OnPropertyChanged(nameof(Cover));
                }
            }
        }
        /// <summary>
        /// 分辨率
        /// </summary>
        private string _resolution;
        public string Resolution
        {
            get { return _resolution; }
            set
            {
                if (_resolution != value)
                {
                    _resolution = value;
                    OnPropertyChanged(nameof(Resolution));
                }
            }
        }
        /// <summary>
        /// 评分
        /// </summary>
        private double _rating;
        public double Rating
        {
            get { return _rating; }
            set
            {
                if (_rating != value)
                {
                    _rating = value;
                    OnPropertyChanged(nameof(Rating));
                }
            }
        }

        /// <summary>
        /// 评论
        /// </summary>
        private string _comment;
        public string Comment
        {
            get { return _comment; }
            set
            {
                if (_comment != value)
                {
                    _comment = value;
                    OnPropertyChanged(nameof(Comment));
                }
            }
        }
        /// <summary>
        /// 是否加载
        /// </summary>
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                }
            }
        }

        /// <summary>
        /// 喜欢
        /// </summary>
        private int _like;
        public int Like
        {
            get { return _like; }
            set
            {
                if (_like != value)
                {
                    _like = value;
                    OnPropertyChanged(nameof(Like));
                }
            }
        }

        /// <summary>
        /// 发布日期
        /// </summary>
        private string _releseDate;

        public string ReleaseDate
        {
            get { return _releseDate; }
            set
            {
                if (_releseDate != value)
                {
                    _releseDate = value;
                    OnPropertyChanged(nameof(ReleaseDate));
                }
            }
        }

        /// <summary>
        /// 是否有中文字幕
        /// </summary>
        private bool _hasCaption;
        public bool HasCaption
        {
            get { return _hasCaption; }
            set
            {
                if (_hasCaption != value)
                {
                    _hasCaption = value;
                    OnPropertyChanged(nameof(HasCaption));
                }
            }
        }

        /// <summary>
        /// 是否无修正
        /// </summary>
        private bool _isUncensored;
        public bool IsUncensored
        {
            get { return _isUncensored; }
            set
            {
                if (_isUncensored != value)
                {
                    _isUncensored = value;
                    OnPropertyChanged(nameof(IsUncensored));
                }
            }
        }

        /// <summary>
        /// 文件路径
        /// </summary>
        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                if (_filePath != value)
                {
                    _filePath = value;
                    OnPropertyChanged(nameof(FilePath));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}