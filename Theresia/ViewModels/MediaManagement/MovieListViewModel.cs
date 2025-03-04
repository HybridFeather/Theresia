using GalaSoft.MvvmLight.Command;
using HandyControl.Collections;
using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Tools.Extension;
using Prism.Events;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Theresia.Common;
using Theresia.DO;
using Theresia.DO.Option;
using Theresia.Entity;
using Theresia.Enums;
using Theresia.Repositories.Interfaces;
using Theresia.Services.Interfaces;
using Theresia.ViewModels.Dialog;
using Theresia.ViewModels.MediaManagement.Event;
using Theresia.Views.Dialog;

namespace Theresia.ViewModels.MediaManagement
{
    public class MovieListViewModel : BindableBase
    {
        private readonly IMovieService movieService;
        private readonly IDirectoryService directoryService;

        private readonly IMovieRepository movieRepository;
        private readonly IMediaMetadataRepository mediaMetadataRepository;

        private readonly IEventAggregator eventAggregator;

        /// <summary>
        /// 目标模糊值
        /// </summary>
        private readonly static double TARGET_BLUR_RADIUS = 50;

        /// <summary>
        /// 缓存
        /// </summary>
        private Dictionary<string, MovieEntity> MOVIE_CACHE;
        /// <summary>
        /// 每次加载的个数
        /// </summary>
        private readonly int SCAN_BANTCH_NUM = 4;

        /// <summary>
        /// 每页数量
        /// </summary>
        private readonly int PAGE_SIZE = 100;
        /// <summary>
        /// 对页面展示的所有数据
        /// </summary>
        private List<MovieModel> _dataList;
        /// <summary>
        /// 对页面展示的分页后数据
        /// </summary>
        public ManualObservableCollection<MovieModel> DataList { get; set; }
        /// <summary>
        /// 查询结果，用以分页
        /// </summary>
        private List<MovieEntity> QueryResult { get; set; }
        /// <summary>
        /// 标签列表
        /// </summary>
        private List<TagEntity> _tagOptionList { get; set; }
        public ObservableCollection<TagEntity> TagOptionList { get; set; }
        /// <summary>
        /// 已选择的标签
        /// </summary>
        public List<int> SelectedTagOptions { get; set; }

        /// <summary>
        /// 系列
        /// </summary>
        public ManualObservableCollection<SeriesEntity> SeriesOptionList { get; set; }
        /// <summary>
        /// 演员
        /// </summary>
        public ManualObservableCollection<CastCrewEntity> ActorOptionList { get; set; }
        public ManualObservableCollection<CastCrewEntity> DirectorOptionList { get; set; }
        /// <summary>
        /// 罩杯
        /// </summary>
        public ObservableCollection<string> CupOptionList { get; set; }
        /// <summary>
        /// 是否喜欢
        /// </summary>
        public ObservableCollection<ComboBoxLikeOption> LikeOptions { get; set; }
        /// <summary>
        /// 页数
        /// </summary>
        private int _pageCount = 1;
        public int PageCount
        {
            get => _pageCount;
            set
            {
                SetProperty(ref _pageCount, value);
            }
        }

        /// <summary>
        /// 当前页
        /// </summary>
        private int _pageIndex = 1;
        public int PageIndex
        {
            get => _pageIndex;
            set => SetProperty(ref _pageIndex, value);
        }
        /// <summary>
        /// 模糊开关
        /// </summary>
        private bool _blurSwitch = true;
        public bool BlurSwitch
        {
            get => _blurSwitch;
            set
            {
                SetProperty(ref _blurSwitch, value);
            }
        }
        /// <summary>
        /// 模糊值
        /// </summary>
        private double _blurRadius = TARGET_BLUR_RADIUS;
        public double BlurRadius
        {
            get => _blurRadius;
            set
            {
                SetProperty(ref _blurRadius, value);
            }
        }
        /// <summary>
        /// 进度条进度
        /// </summary>
        private int _progressValue;
        public int ProgressValue
        {
            get => _progressValue;
            set
            {
                SetProperty(ref _progressValue, value);
            }
        }

        /// <summary>
        /// 刷新按钮
        /// </summary>
        private bool _refreshEnable = true;
        public bool RefreshEnable
        {
            get => _refreshEnable;
            set
            {
                SetProperty(ref _refreshEnable, value);
            }
        }
        /// <summary>
        /// 下载按钮
        /// </summary>
        private bool _downloadEnable = true;
        public bool DownLoadEnable
        {
            get => _downloadEnable;
            set
            {
                SetProperty(ref _downloadEnable, value);
            }
        }

        /// <summary>
        /// 关键词
        /// </summary>
        private string _keyWordText;
        public string KeyWordText
        {
            get => _keyWordText;
            set
            {
                SetProperty(ref _keyWordText, value);
            }
        }
        /// <summary>
        /// 系列文本
        /// </summary>
        private string _seriesText;
        public string SeriesText
        {
            get => _seriesText;
            set
            {
                SetProperty(ref _seriesText, value);
                FilterSeries(value);
            }
        }

        /// <summary>
        /// 演员文本
        /// </summary>
        private string _actorText;
        public string ActorText
        {
            get => _actorText;
            set
            {
                SetProperty(ref _actorText, value);
                FilterActors(value);
            }
        }
        /// <summary>
        /// 导演文本
        /// </summary>
        private string _directorText;
        public string DirectorText
        {
            get => _directorText;
            set
            {
                SetProperty(ref _directorText, value);
                FilterDirector(value);
            }
        }

        /// <summary>
        /// 罩杯文本
        /// </summary>
        private string _cupText;
        public string CupText
        {
            get => _cupText;
            set
            {
                SetProperty(ref _cupText, value);
            }
        }

        /// <summary>
        /// 是否有字幕
        /// </summary>
        private int _hasCaption;
        public int HasCaption
        {
            get => _hasCaption;
            set
            {
                SetProperty(ref _hasCaption, value);
            }
        }

        /// <summary>
        /// 是否有收藏
        /// </summary>
        private int _hasLike = 2;
        public int HasLike
        {
            get => _hasLike;
            set
            {
                SetProperty(ref _hasLike, value);
            }
        }

        /// <summary>
        /// 最小评分
        /// </summary>
        private double _minRating = 0;
        public double MinRating
        {
            get => _minRating;
            set
            {
                SetProperty(ref _minRating, value);
            }
        }

        /// <summary>
        /// 最大评分
        /// </summary>
        private double _maxRating = 5;
        public double MaxRating
        {
            get => _maxRating;
            set
            {
                SetProperty(ref _maxRating, value);
            }
        }

        /// <summary>
        /// 最小发行日期
        /// </summary>
        private DateTime? _minReleaseDate = DateTime.MinValue;

        public DateTime? MinReleaseDate
        {
            get => _minReleaseDate;
            set
            {
                if (value == null)
                {
                    _minReleaseDate = DateTime.MinValue;
                }
                SetProperty(ref _minReleaseDate, value);
            }
        }

        /// <summary>
        /// 最大发行日期
        /// </summary>
        private DateTime? _maxReleaseDate = DateTime.MaxValue;
        public DateTime? MaxReleaseDate
        {
            get => _maxReleaseDate;
            set
            {
                if (value == null)
                {
                    _maxReleaseDate = DateTime.MaxValue;
                }
                SetProperty(ref _maxReleaseDate, value);
            }
        }

        /// <summary>
        /// 标签搜索框变动开关
        /// </summary>
        private bool SeriesTextSearchEnable = true;
        /// <summary>
        /// 喜欢按钮开关
        /// </summary>
        private bool LikeBtnEnable = true;

        /// <summary>
        /// 模糊开关控制
        /// </summary>
        private bool _blurSwitchEnable = true;
        public bool BlurSwitchEnable
        {
            get => _blurSwitchEnable;
            set
            {
                SetProperty(ref _blurSwitchEnable, value);
            }
        }

        /// <summary>
        /// 下载按钮
        /// </summary>
        public ICommand DownloadBtnCmd { get; set; }
        /// <summary>
        /// 刷新按钮事件
        /// </summary>
        public ICommand RefreshBtnCmd { set; get; }
        /// <summary>
        /// 标签多选事件
        /// </summary>
        public ICommand TagSelectedCmd { set; get; }
        /// <summary>
        /// 标签搜索栏变动事件
        /// </summary>
        public ICommand TagSearchTextChangedCmd { set; get; }
        /// <summary>
        /// 搜索按钮事件
        /// </summary>
        public ICommand SearchBtnCmd { get; set; }
        /// <summary>
        /// 播放按钮事件
        /// </summary>
        public ICommand PlayBtnClickCmd { get; set; }

        /// <summary>
        /// 详情页面
        /// </summary>
        public ICommand InfoBtnCmd { get; set; }
        /// <summary>
        /// 评分按钮事件
        /// </summary>
        public ICommand RatingBtnCmd { get; set; }
        /// <summary>
        /// 评论按钮事件
        /// </summary>
        public ICommand CommentBtnCmd { get; set; }
        /// <summary>
        /// 重新爬取按钮
        /// </summary>
        public ICommand ReScrapeBtnCmd { get; set; }
        /// <summary>
        /// 显示标签按钮
        /// </summary>
        public ICommand ShowTagsBtnCmd { get; set; }
        /// <summary>
        /// 重命名按钮
        /// </summary>
        public ICommand RenameBtnCmd { get; set; }
        /// <summary>
        /// 喜欢按钮
        /// </summary>
        public ICommand LikeBtnCmd { get; set; }
        /// <summary>
        /// 删除按钮
        /// </summary>
        public ICommand DeleteBtnCmd { get; set; }
        /// <summary>
        /// 模糊开关
        /// </summary>
        public ICommand BlurSwitchCmd { get; set; }
        /// <summary>
        /// 翻页
        /// </summary>
        public ICommand PageUpdatedCmd { get; set; }
        public MovieListViewModel(IMovieService _movieService, IDirectoryService _directoryService,IMovieRepository _movieRepository,
            IEventAggregator _eventAggregator,IMediaMetadataRepository _mediaMetadataRepository)
        {
            this.movieService = _movieService;
            this.directoryService = _directoryService;

            this.movieRepository = _movieRepository;
            this.mediaMetadataRepository = _mediaMetadataRepository;

            this.eventAggregator = _eventAggregator;

            LikeOptions = [
                new ComboBoxLikeOption { Value = 2, Name = "不限" },
                new ComboBoxLikeOption { Value = 1, Name = "是" },
                new ComboBoxLikeOption { Value = 0, Name = "否" }
            ];

            Initialize();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        private async Task Initialize()
        {
            _dataList = new();
            DataList = new();
            QueryResult = new List<MovieEntity>();

            _tagOptionList = new List<TagEntity>();
            TagOptionList = new ObservableCollection<TagEntity>();
            SelectedTagOptions = new List<int>();

            SeriesOptionList = new ManualObservableCollection<SeriesEntity>();

            DirectorOptionList = new ManualObservableCollection<CastCrewEntity>();

            ActorOptionList = new ManualObservableCollection<CastCrewEntity>();

            CupOptionList = new ObservableCollection<string>();

            DownloadBtnCmd = new RelayCommand(DownloadBtnEvent);
            RefreshBtnCmd = new RelayCommand<bool>(RefreshBtnEvent);
            TagSelectedCmd = new RelayCommand<object>(TagSelectedEvent);
            TagSearchTextChangedCmd = new RelayCommand<object>(TagSearchTextChangedEvent);
            SearchBtnCmd = new RelayCommand(SearchBtnEvent);
            PlayBtnClickCmd = new RelayCommand<string>(PlayBtnClickEvent);
            BlurSwitchCmd = new RelayCommand(BlurSwitchBtnEvent);
            PageUpdatedCmd = new RelayCommand(PageUpdatedEvent);

            //右键菜单
            InfoBtnCmd = new RelayCommand<string>(InfoBtnEvent); 
            RatingBtnCmd = new RelayCommand<string>(RatingBtnEvent);
            CommentBtnCmd = new RelayCommand<string>(CommentBtnEvent);
            ReScrapeBtnCmd = new RelayCommand<string>(ReScrapeBtnEvent);
            LikeBtnCmd = new RelayCommand<string>(LikeBtnEvent);
            ShowTagsBtnCmd = new RelayCommand<string>(ShowTagsBtnEvent);
            RenameBtnCmd = new RelayCommand<string>(RenameBtnEvent);
            DeleteBtnCmd = new RelayCommand<string>(DeleteBtnEvent);
            await InitializeCache();
            await ScanDirectory();
            RefreshBtnEvent(false);
        }

        
        /// <summary>
        /// 刷新各种缓存
        /// </summary>
        private async Task InitializeCache()
        {
            //全部电影的缓存
            List<MovieEntity> list = await movieRepository.QueryByConditions(new MovieQueryDTO{PageIndex = 1,PageSize = Int32.MaxValue});
            MOVIE_CACHE = new Dictionary<string, MovieEntity>(list.Count);
            foreach (MovieEntity entity in list)
            {
                MOVIE_CACHE.Add(entity.Code,entity);
            }
            //标签
            _tagOptionList.Clear();
            TagOptionList.Clear();
            _tagOptionList.AddRange(CommonCache.TAG_CACHE);
            TagOptionList.AddRange(_tagOptionList);
            //系列
            SeriesOptionList.Clear();
            SeriesOptionList.AddRange(CommonCache.SERIES_CACHE.Where(s => s.Type == (int)SeriesTypeEnum.MOVIE));

            //卡斯
            DirectorOptionList.Clear();
            ActorOptionList.Clear();
            CupOptionList.Clear();
            DirectorOptionList.AddRange(CommonCache.DIRECTOR_CACHE);
            ActorOptionList.AddRange(CommonCache.ACTOR_CACHE);
            CupOptionList.AddRange(ActorOptionList.Select(a => a.Cup).Distinct().ToList());
        }

        /// <summary>
        /// 扫描目录
        /// </summary>
        /// <returns></returns>
        private async Task ScanDirectory()
        {
            Growl.Info("开始扫描目录，这可能需要一些时间，请耐心等待");
            DataList.Clear();
            _dataList.Clear();
            //使用异步方式执行
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                OnStartLoading();
            });
            string movieDirectory = directoryService.GetMovieDirectory();
            string movieCoverDirectory = directoryService.GetMovieCoverDirectory();

            // 创建目录
            Directory.CreateDirectory(movieDirectory);
            Directory.CreateDirectory(movieCoverDirectory);

            int completedScan = 0;//已完成扫描

            // 进度条
            IProgress<int> progress = new Progress<int>(percentage =>
            {
                // 更新进度条
                ProgressValue = percentage;
            });

            // 扫描到的文件列表
            List<string> fileList = [.. Directory.EnumerateFiles(movieDirectory, "*", SearchOption.TopDirectoryOnly)];
            //番号与文件路径的对应
            Dictionary<string, string> pathCodeMap = new Dictionary<string, string>(fileList.Count);
            foreach (string file in fileList)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                pathCodeMap.TryAdd(file, MediaUtils.GetMovieCode(fileName));
            }
            //异步添加
            Task scanTask = Task.Run(async () =>
            {
                int index = 0;
                //缓存，用以批量更新提升性能
                //List<MovieModel> cacheList = new List<MovieModel>(SCAN_BANTCH_NUM);
                
                foreach (string filePath in pathCodeMap.Keys)
                {
                    string fileName = Path.GetFileName(filePath);
                    if (!MediaUtils.IsMediaFile(fileName))
                    {
                        Debug.WriteLine($"文件[{filePath}]不是媒体文件，跳过");
                        continue;
                    }
                    try
                    {
                        string movieCode = pathCodeMap[filePath];
                        // 解析分辨率（非常耗时）
                        //MediaMetadata metadata = MediaUtils.GetMetadata(filePath);
                        Debug.WriteLine($"番号{movieCode}查找数据库中的记录");
                        MovieEntity? entity = MOVIE_CACHE.TryGetValue(movieCode, out MovieEntity? value) ? value : null;
                        //视图对象
                        MovieModel movieModel;
                        //根据这个来获取视频元数据
                        string sha256 = MediaUtils.GetFileSha256(filePath);
                        MediaMetadataEntity? metadata = CommonCache.MEDIA_METADATA_CACHE.ContainsKey(sha256) ? CommonCache.MEDIA_METADATA_CACHE[sha256]:null;
                        if (metadata == null)
                        {
                            metadata = MediaUtils.GetMetadata(filePath);
                            await mediaMetadataRepository.AddAsync(metadata);
                            CommonCache.MEDIA_METADATA_CACHE.TryAdd(sha256, metadata);
                        }
                        if (entity == null)
                        {
                            Debug.WriteLine($"番号{movieCode}在数据库中不存在，将进行数据爬取");
                            movieModel = new MovieModel
                            {
                                Title = fileName,
                                Code = movieCode,
                                Resolution = MediaUtils.GetResolution(metadata.Height),
                                IsLoading = true,
                                Cover = AppConstant.QUESTION_MARK_PATH,
                                FilePath = filePath,
                                HasCaption = MediaUtils.HasCaption(fileName),
                                IsUncensored = MediaUtils.IsUncensored(fileName)
                            };
                        }
                        else
                        {
                            Debug.WriteLine($"番号{movieCode}查找到数据库中的数据");
                            movieModel = new MovieModel
                            {
                                Title = entity.Title ?? "",
                                Code = entity.Code,
                                Resolution = MediaUtils.GetResolution(metadata.Height),
                                Cover = Path.Combine(movieCoverDirectory, entity.Code + ".jpg"),
                                IsLoading = entity.Status != 1,
                                Rating = entity.Rating,
                                Comment = entity.Comment ?? "",
                                Like = entity.Like,
                                FilePath = filePath,
                                ReleaseDate = entity.ReleaseDate ?? "",
                                HasCaption = MediaUtils.HasCaption(fileName),
                                IsUncensored = MediaUtils.IsUncensored(fileName)
                            };
                        }
                        _dataList.Add(movieModel);
                        /* ----------下面这段因为解决了媒体元数据读取问题，性能得以巨大提升，且由于查询方式的改变所以不用这么搞了---------- */
                        //添加到缓存队列
                        /*cacheList.Add(movieModel);
                        // 如果当前缓存的数量已经达到批处理的大小，或者剩余数据小于批处理的大小时，触发一次批量添加
                        if (cacheList.Count == SCAN_BANTCH_NUM || pathCodeMap.Count - index < SCAN_BANTCH_NUM)
                        {
                            // UI 线程更新数据，逐步显示解析的电影
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                DataList.AddRange(cacheList);
                            });
                            // 添加完后直接清理掉缓存队列
                            cacheList.Clear();
                        }*/

                        // 计算进度
                        completedScan++;
                        int percentage = (int)((double)completedScan / fileList.Count * 100);
                        // 使用 IProgress 更新 UI 线程上的进度
                        progress.Report(percentage);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"文件名{Path.GetFileName(filePath)}解析有误，异常信息{ex.Message}");
                    }
                    index++;
                }
            });

            // 这里不需要等待所有任务完成后才更新 UI，而是让 UI 逐步更新
            await scanTask;
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                OnStopLoading();
            });
            Growl.Success($"扫描完成，共有{_dataList.Count}部电影");
        }

        /// <summary>
        /// 翻页
        /// </summary>
        private void PageUpdatedEvent()
        {
            //页数总数
            Task.Run(() =>
            {
                PageCount = QueryResult.Count % PAGE_SIZE == 0
                    ? QueryResult.Count / PAGE_SIZE
                    : (QueryResult.Count / PAGE_SIZE) + 1;
                List<MovieEntity> takenResult = QueryResult.Skip((PageIndex - 1) * PAGE_SIZE)
                    .Take(PAGE_SIZE)
                    .ToList();
                List<MovieModel> sortedList;
                if (takenResult.Count != 0)
                {
                    sortedList = new List<MovieModel>(PAGE_SIZE);
                    foreach (var item in takenResult)
                    {
                        List<MovieModel> queryList = _dataList.Where(m => m.Code == item.Code).ToList();
                        sortedList.AddRange(queryList);
                    }
                    PageCount = QueryResult.Count % PAGE_SIZE == 0
                        ? QueryResult.Count / PAGE_SIZE
                        : (QueryResult.Count / PAGE_SIZE) + 1;
                }
                else
                {
                    List<MovieModel> modelList = _dataList.Skip((PageIndex - 1) * PAGE_SIZE)
                        .Take(PAGE_SIZE)
                        .ToList();
                    sortedList = new(modelList);
                    PageCount = _dataList.Count % PAGE_SIZE == 0
                        ? _dataList.Count / PAGE_SIZE
                        : (_dataList.Count / PAGE_SIZE) + 1;
                }
                //计算页数
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    if (PageCount == 1)
                    {
                        HidePagination();
                    }
                    else
                    {
                        ShowPagination();
                    }
                    //要添加上那些没被识别出来的到第一页
                    DataList.Clear();
                    if (PageIndex == 1 && takenResult.Count != 0)
                    {
                        List<MovieModel> loadingList = _dataList.Where(m => m.IsLoading).ToList();
                        DataList.CanNotify = false;
                        DataList.AddRange(loadingList);
                    }
                    DataList.AddRange(sortedList);
                    DataList.CanNotify = true;
                });
            });
        }

        private async Task BatchScrape()
        {
            Growl.Info("现在开始爬取电影信息");
            string movieCoverDirectory = directoryService.GetMovieCoverDirectory();
            foreach (MovieModel item in DataList)
            {
                if(item.IsLoading == true)
                {
                    try
                    {
                        Growl.Info($"开始获取番号{item.Code}的电影信息");
                        MovieScraperResult result = await movieService.ScrapeMovieInfoAsync(item.Code);
                        item.Title = result.Title;
                        item.IsLoading = false;
                        item.Cover = Path.Combine(movieCoverDirectory, item.Code + ".jpg");
                        item.ReleaseDate = result.ReleaseDate;
                        Growl.Success($"番号{item.Code}的电影信息获取完成");
                        await Task.Delay(TimeSpan.FromSeconds(5));
                    }
                    catch(Exception ex)
                    {
                        Growl.Error(new GrowlInfo()
                        {
                            Message = $"获取番号{item.Code}的电影失败",
                            WaitTime = 5,
                            IsCustom = true
                        });
                        Debug.WriteLine($"获取番号{item.Code}的电影失败，异常信息{ex.Message}");
                    }
                }
            }
            await InitializeCache();
            Growl.Success("电影信息爬取完成");
        }
        /// <summary>
        /// 搜索按钮事件
        /// </summary>
        private async void SearchBtnEvent()
        {
            RefreshEnable = false;
            try
            {
                string code = MediaUtils.GetMovieCode(KeyWordText);
                string title = string.IsNullOrEmpty(code) ? KeyWordText : "";
                SeriesEntity? series = SeriesOptionList.FirstOrDefault(s => s.Name == SeriesText);
                int seriesId = series == null ? 0 : series.Id;
                CastCrewEntity? actor = ActorOptionList.FirstOrDefault(a => a.OriginalName == ActorText);
                int actorId = actor == null ? 0 : actor.Id;
                CastCrewEntity? director = DirectorOptionList.FirstOrDefault(a => a.OriginalName == DirectorText);
                int directorId = director == null ? 0 : director.Id;
                QueryResult = await movieRepository.QueryByConditions(new MovieQueryDTO()
                {
                    Code = code,
                    Title = title,
                    SeriesId = seriesId,
                    ActorId = actorId,
                    DirectId = directorId,
                    Like = HasLike,
                    MinRating = MinRating,
                    MaxRating = MaxRating,
                    MinReleaseDate = MinReleaseDate?.ToString("yyyy-MM-dd") ?? "",
                    MaxReleaseDate = MaxReleaseDate?.ToString("yyyy-MM-dd") ?? "",
                    Tags = SelectedTagOptions,
                    PageIndex = PageIndex,
                    PageSize = PAGE_SIZE
                });
                PageUpdatedEvent();
            }
            finally
            {
                RefreshEnable = true;
            }
        }

        /// <summary>
        /// 下载按钮
        /// </summary>
        private async void DownloadBtnEvent()
        {
            RefreshEnable = false;
            DownLoadEnable = false;
            try
            {
                await BatchScrape();
            }
            finally
            {
                RefreshEnable = true;
                DownLoadEnable = true;
            }
        }

        /// <summary>
        /// 刷新按钮事件
        /// </summary>
        private async void RefreshBtnEvent(bool scan)
        {
            RefreshEnable = false;
            DownLoadEnable = false;
            try
            {
                if (scan)
                {
                    await ScanDirectory();
                }
            }
            finally
            {
                SearchBtnEvent();
                RefreshEnable = true;
                DownLoadEnable = true;
            }
        }

        /// <summary>
        /// 标签选择框变动事件
        /// </summary>
        /// <param name="parameter"></param>
        private void TagSelectedEvent(object parameter)
        {
            SelectedTagOptions.Clear();
            var checkbox = parameter as CheckComboBox; // 获取控件本身
            if (checkbox != null)
            {
                var list = checkbox.SelectedItems;
                foreach (var item in list)
                {
                    SelectedTagOptions.Add(((TagEntity)item).Id);
                }
            }
        }
        /// <summary>
        /// 标签搜索栏变动事件
        /// </summary>
        private async void TagSearchTextChangedEvent(object parameter)
        {
            if (!SeriesTextSearchEnable)
            {
                return;
            }
            SeriesTextSearchEnable = false;
            await Task.Delay(TimeSpan.FromSeconds(1));
            var textBox = parameter as HandyControl.Controls.TextBox;
            string value = textBox.Text;
            if (string.IsNullOrEmpty(value))
            {
                TagOptionList.AddRange(_tagOptionList);
            }
            else
            {
                TagOptionList.Clear();
                TagOptionList.AddRange(_tagOptionList.Where(t => t.Name.Contains(value)));
            }
            SeriesTextSearchEnable = true;
        }

        /// <summary>
        /// 播放按钮事件
        /// </summary>
        private async void PlayBtnClickEvent(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                HandyControl.Controls.MessageBox.Show($"文件[{filePath}]不存在", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (!File.Exists(filePath))
            {
                HandyControl.Controls.MessageBox.Show($"文件[{filePath}]不存在", "提示", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                DataList.Remove(DataList.First(m => m.FilePath == filePath));
                _dataList.Remove(_dataList.First(m => m.FilePath == filePath));
            }
            else
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true // 使用默认程序打开文件
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    HandyControl.Controls.MessageBox.Show($"未知错误，错误内容[{ex}]", "提示", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
                
        }

        /// <summary>
        /// 详情按钮
        /// </summary>
        /// <param name="code"></param>
        private async void InfoBtnEvent(string code)
        {

        }

        /// <summary>
        /// 评分按钮
        /// </summary>
        /// <param name="code"></param>
        private async void RatingBtnEvent(string code)
        {
            MovieEntity? movie = MOVIE_CACHE[code];
            if (movie == null)
            {
                HandyControl.Controls.MessageBox.Show($"番号[{code}]在数据库中不存在，请尝试重新抓取", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            double result = await HandyControl.Controls.Dialog.Show<RatingDialog>()
                .Initialize<RatingDialogViewModel>(vm => vm.Result = movie.Rating)
                .GetResultAsync<double>();
            movie.Rating = result;
            try
            {
                await movieRepository.UpdateAsync(movie);
                Growl.Success($"番号{code}评分保存成功");
                MovieModel? model = DataList.FirstOrDefault(m => m.Code == code);
                if (model != null)
                {
                    model.Rating = result;
                }
            }
            catch (Exception ex)
            {
                Growl.Error(new GrowlInfo()
                {
                    Message = $"番号{code}的电影评分保存失败",
                    WaitTime = 5,
                    IsCustom = true
                });
                Debug.WriteLine($"番号{code}的电影评分保存失败，异常信息{ex}");
            }
        }

        /// <summary>
        /// 评论事件
        /// </summary>
        /// <param name="code"></param>
        private async void CommentBtnEvent(string code)
        {
            MovieEntity? movie = MOVIE_CACHE[code];
            if (movie == null)
            {
                HandyControl.Controls.MessageBox.Show($"番号[{code}]在数据库中不存在，请尝试重新抓取", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string result  = await HandyControl.Controls.Dialog.Show<CommentDialog>()
                .Initialize<CommentDialogViewModel>(vm => vm.Result = movie.Comment ?? "")
                .GetResultAsync<string>();
            movie.Comment = result;
            try
            {
                await movieRepository.UpdateAsync(movie);
                Growl.Success($"番号{code}评论保存成功");
                MovieModel? model = DataList.FirstOrDefault(m => m.Code == code);
                if (model != null)
                {
                    model.Comment = result;
                }
            }
            catch (Exception ex)
            {
                Growl.Error(new GrowlInfo()
                {
                    Message = $"番号{code}的电影评论保存失败",
                    WaitTime = 5,
                    IsCustom = true
                });
                Debug.WriteLine($"番号{code}的电影评论保存失败，异常信息{ex}");
            }

        }

        /// <summary>
        /// 重新抓取
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private async void ReScrapeBtnEvent(string code)
        {
            var result = HandyControl.Controls.MessageBox.Show($"确定要重新抓取番号[{code}]的电影吗", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await movieService.ScrapeMovieInfoAsync(code);
                    MovieEntity? entity = await movieRepository.GetMovieByCodeAsync(code);
                    if (entity != null)
                    {
                        MOVIE_CACHE.Remove(code);
                        MOVIE_CACHE.Add(code,entity);
                        MovieModel? model = DataList.FirstOrDefault(m => m.Code == code);
                        if (model != null)
                        {
                            string coverPath = directoryService.GetMovieCoverDirectory();
                            model.Code = entity.Code;
                            model.Title = entity.Code;
                            model.Cover = Path.Combine(coverPath, entity.Code + ".jpg");
                            model.Comment = entity.Comment??"";
                            model.IsLoading = entity.Status != 1;
                            model.Rating = entity.Rating;
                            model.Like = entity.Like;
                            model.Like = entity.Like;
                            model.ReleaseDate = entity.ReleaseDate ?? "";
                        }
                    }
                }
                catch (Exception ex)
                {
                    Growl.Error(new GrowlInfo()
                    {
                        Message = $"{ex.Message}",
                        WaitTime = 5,
                        IsCustom = true
                    });
                }
            }
        }

        /// <summary>
        /// 删除按钮
        /// </summary>
        /// <param name="filePath"></param>
        private async void DeleteBtnEvent(string filePath)
        {
            if (!File.Exists(filePath))
            {
                HandyControl.Controls.MessageBox.Show($"文件[{filePath}]不存在", "提示", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                DataList.Remove(DataList.First(m => m.FilePath == filePath));
                _dataList.Remove(_dataList.First(m => m.FilePath == filePath));
            }
            try
            {
                var result = HandyControl.Controls.MessageBox.Show($"确定要删除该视频文件吗？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    File.Delete(filePath);
                    DataList.Remove(DataList.First(m => m.FilePath == filePath));
                    _dataList.Remove(_dataList.First(m => m.FilePath == filePath));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                HandyControl.Controls.MessageBox.Show($"未知错误，错误内容[{ex}]", "提示", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 喜欢按钮
        /// </summary>
        /// <param name="code"></param>
        private async void LikeBtnEvent(string code)
        {
            if (!LikeBtnEnable)
            {
                Growl.Warning("操作速度过快，请重新操作");
            }
            else
            {
                LikeBtnEnable = false;
                MovieEntity? movie = MOVIE_CACHE[code];
                if (movie == null)
                {
                    HandyControl.Controls.MessageBox.Show($"番号[{code}]在数据库中不存在，请尝试重新抓取", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                try
                {
                    int like = movie.Like == 1 ? 0 : 1;
                    movie.Like = like;
                    await movieRepository.UpdateAsync(movie);
                    MovieModel? model = DataList.FirstOrDefault(m => m.Code == code);
                    if (model != null)
                    {
                        model.Like = like;
                    }
                }
                catch (Exception ex)
                {
                    Growl.Error(new GrowlInfo()
                    {
                        Message = $"番号{code}的电影收藏失败",
                        WaitTime = 5,
                        IsCustom = true
                    });
                    Debug.WriteLine($"番号{code}的电影收藏失败，异常信息{ex}");
                }
                finally
                {
                    LikeBtnEnable = true;
                }
            }
        }


        /// <summary>
        /// 显示标签
        /// </summary>
        /// <param name="code"></param>
        private async void ShowTagsBtnEvent(string code)
        {
            MovieEntity? movie = MOVIE_CACHE[code];
            if (movie == null)
            {
                HandyControl.Controls.MessageBox.Show($"番号[{code}]在数据库中不存在，请尝试重新抓取", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string result = await HandyControl.Controls.Dialog.Show<TagsDialog>()
                .Initialize<TagsDialogViewModel>(vm => vm.Initialize(movie.Code ?? ""))
                .GetResultAsync<string>();
        }

        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="filePath"></param>
        private async void RenameBtnEvent(string filePath)
        {
            if (!File.Exists(filePath))
            {
                HandyControl.Controls.MessageBox.Show($"文件[{filePath}]不存在", "提示", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                DataList.Remove(DataList.First(m => m.FilePath == filePath));
                _dataList.Remove(_dataList.First(m => m.FilePath == filePath));
            }
            try
            {
                string fileName = Path.GetFileName(filePath);
                string directory = Path.GetDirectoryName(filePath);
                string result = await HandyControl.Controls.Dialog.Show<RenameDialog>()
                    .Initialize<RenameDialogViewModel>(vm => vm.Result = fileName ?? "")
                    .GetResultAsync<string>();
                if (result != fileName)
                {
                    string newPath = Path.Combine(directory, result);
                    if(File.Exists(newPath))
                    {
                        HandyControl.Controls.MessageBox.Show($"文件[{newPath}]已存在", "提示", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                    else
                    {
                        File.Move(filePath,newPath);
                        MovieModel? model = DataList.FirstOrDefault(m => m.FilePath == filePath);
                        if (model != null)
                        {
                            model.Title = model.IsLoading ? result: model.Title;
                            model.Code = MediaUtils.GetMovieCode(result);
                            model.FilePath = newPath;
                            model.IsUncensored = MediaUtils.IsUncensored(result);
                            model.HasCaption = MediaUtils.HasCaption(result);
                        }
                        Growl.Success($"文件重命名成功");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                HandyControl.Controls.MessageBox.Show($"未知错误，错误内容[{ex}]", "提示", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 模糊开关
        /// </summary>
        private async void BlurSwitchBtnEvent()
        {
            try
            {
                BlurSwitchEnable = !BlurSwitchEnable;
                if (BlurSwitch)
                {
                    await AnimateBlurRadiusAsync(0, TARGET_BLUR_RADIUS, 300);
                }
                else
                {
                    await AnimateBlurRadiusAsync(TARGET_BLUR_RADIUS, 0, 300);
                }
            }
            finally
            {
                BlurSwitchEnable = !BlurSwitchEnable;
            }
        }
        /// <summary>
        /// 简便方法
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        private async Task AnimateBlurRadiusAsync(double from, double to, int duration)
        {
            var stepCount = 20; // 渐变的步数
            var stepDuration = duration / stepCount; // 每一步的持续时间
            var stepSize = (to - from) / stepCount; // 每一步变化的值

            for (int i = 0; i < stepCount; i++)
            {
                BlurRadius = from + stepSize * i; // 每次变化 BlurRadius
                await Task.Delay(stepDuration); // 延迟一下，模拟动画效果
            }

            BlurRadius = to; // 确保最终值为目标值
        }

        private void FilterSeries(string key)
        {
            if (key != "*")
            {
                SeriesOptionList.CanNotify = false;

                SeriesOptionList.Clear();

                foreach (var data in CommonCache.SERIES_CACHE)
                {
                    if (data.Name.ToLower().Contains(key.ToLower()))
                    {
                        SeriesOptionList.Add(data);
                    }
                }

                SeriesOptionList.CanNotify = true;
            }
        }

        private void FilterActors(string key)
        {
            if (key != "*")
            {
                ActorOptionList.CanNotify = false;

                ActorOptionList.Clear();

                foreach (var data in CommonCache.ACTOR_CACHE)
                {
                    if (data.OriginalName.ToLower().Contains(key.ToLower()))
                    {
                        ActorOptionList.Add(data);
                    }
                }

                ActorOptionList.CanNotify = true;
            }
        }

        private void FilterDirector(string key)
        {
            if (key != "*")
            {
                DirectorOptionList.CanNotify = false;

                DirectorOptionList.Clear();

                foreach (var data in CommonCache.DIRECTOR_CACHE)
                {
                    if (data.OriginalName.ToLower().Contains(key.ToLower()))
                    {
                        DirectorOptionList.Add(data);
                    }
                }

                DirectorOptionList.CanNotify = true;
            }
        }


        // 启动进度条显示事件
        private void OnStartLoading()
        {
            eventAggregator.GetEvent<ShowProgressBarEvent>().Publish();
        }

        // 停止进度条显示事件
        private void OnStopLoading()
        {
            eventAggregator.GetEvent<HideProgressBarEvent>().Publish();
        }

        /// <summary>
        /// 显示页码条
        /// </summary>
        private void ShowPagination()
        {
            eventAggregator.GetEvent<ShowPaginationEvent>().Publish();
        }

        /// <summary>
        /// 隐藏页码条
        /// </summary>
        private void HidePagination()
        {
            eventAggregator.GetEvent<HidePaginationEvent>().Publish();
        }

    }
}
