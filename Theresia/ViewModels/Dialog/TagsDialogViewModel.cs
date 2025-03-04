using GalaSoft.MvvmLight.Command;
using HandyControl.Collections;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows;
using Theresia.Common;
using Theresia.Entity;
using Theresia.Repositories.Interfaces;

namespace Theresia.ViewModels.Dialog
{
    public class TagsDialogViewModel : BindableBase, IDialogResultable<string>
    {
        private List<TagEntity> movieTags;
        private readonly ITagRepository tagRepository;
        private readonly IMovieTagsRepository movieTagsRepository;

        public ObservableCollection<Tag> DataList { get; set; }
        public ManualObservableCollection<TagEntity> TagList { get; set; }


        private string _result;
        public string Result
        {
            get => _result;
            set { SetProperty(ref _result, value); }
        }

        private string _tagName;
        public string TagName
        {
            get => _tagName;
            set
            {
                SetProperty(ref _tagName, value);
                FilterTags(value);
            }
        }

        public TagsDialogViewModel(ITagRepository _tagRepository,IMovieTagsRepository _movieTagsRepository)
        {
            tagRepository = _tagRepository;
            movieTagsRepository = _movieTagsRepository;

            DataList = new ObservableCollection<Tag>();
            TagList = new ManualObservableCollection<TagEntity>(CommonCache.TAG_CACHE);
        }

        public void Initialize(string code)
        {
            Result = code;
            RefreshCache();
        }

        private void RefreshCache()
        {
            movieTags = tagRepository.GetTagByMovieCode(Result);
            List<Tag> tempList = new List<Tag>(movieTags.Count);
            for (var i = 0; i < movieTags.Count; i++)
            {
                TagEntity tagEntity = movieTags[i];
                Tag t = new Tag();
                t.Content = tagEntity.Name;
                t.ShowCloseButton = true;
                t.IsSelected = i % 2 == 0;
                t.Closed += Tag_Closed;
                tempList.Add(t);
            }
            DataList.Clear();
            DataList.AddRange(tempList);
        }

        private void Tag_Closed(object sender, EventArgs e)
        {
            var tag = sender as Tag;
            if (tag != null)
            {
                var result = HandyControl.Controls.MessageBox.Ask($"确定要删除番号 [{Result}] 的 [{tag.Content}] 标签吗？", "删除确认");

                if (result == MessageBoxResult.OK)
                {
                    TagEntity? entity = CommonCache.TAG_CACHE.FirstOrDefault(t => t.Name == tag.Content.ToString());
                    if (entity != null)
                    {
                        movieTagsRepository.RemoveMovieTag(Result, entity.Id);
                    }
                }
            }
            HandyControl.Controls.MessageBox.Show($"删除成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            RefreshCache();
        }



        private void FilterTags(string key)
        {
            TagList.CanNotify = false;

            TagList.Clear();

            foreach (var data in CommonCache.TAG_CACHE)
            {
                if (data.Name.ToLower().Contains(key.ToLower()))
                {
                    TagList.Add(data);
                }
            }
            TagList.CanNotify = true;
        }


        public Action CloseAction { get; set; }

        public RelayCommand CloseCmd => new(() =>
        {
            CloseAction?.Invoke();
        });


        public RelayCommand AddItemCmd => new(() =>
        {
            if (string.IsNullOrEmpty(TagName))
            {
                HandyControl.Controls.MessageBox.Show($"标签名不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                string str = TagName.Trim();
                TagEntity? movieTag = movieTags.FirstOrDefault(mt => mt.Name == str);
                if (movieTag == null)
                {
                    TagEntity? tag = CommonCache.TAG_CACHE.FirstOrDefault(t => t.Name == str);
                    if (tag == null)
                    {
                        tag = new TagEntity
                        {
                            Name = str
                        };
                        tag = tagRepository.AddTag(tag);
                        CommonCache.TAG_CACHE.Add(tag);
                    }
                    movieTagsRepository.AddMovieTag(Result, tag.Id);
                }
            }
            HandyControl.Controls.MessageBox.Show($"添加成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            RefreshCache();
        });
    }
}
