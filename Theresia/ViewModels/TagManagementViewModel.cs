using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows;
using Theresia.Entity;
using Theresia.Repositories.Interfaces;

namespace Theresia.ViewModels
{
    internal class TagManagementViewModel : BindableBase
    {

        private string _tagName;
        private string _searchKey;

        private ITagRepository tagRepository;

        public ObservableCollection<Tag> DataList { get; set; }
        public string TagName
        {
            get => _tagName;
            set => SetProperty(ref _tagName, value);
        }
        public string SearchKey
        {
            get => _searchKey;
            set => SetProperty(ref _searchKey, value);
        }

        /// <summary>
        /// 添加按钮事件
        /// </summary>
        public RelayCommand AddItemCmd => new(() =>
        {
            if (string.IsNullOrEmpty(TagName))
            {
                Growl.Warning($"标签名不能为空");
                return;
            }
            TagEntity? tag = tagRepository.AddTag(new TagEntity
            {
                Name = TagName,
            });

            if (tag != null)
            {
                Growl.Success($"标签名[{TagName}]添加成功！");
            }
            else
            {
                Growl.Error($"标签名[{TagName}]已存在！");
            }
            TagName = "";
            SearchKey = "";
            SearchTag(SearchKey);
        });

        /// <summary>
        /// 搜索
        /// </summary>
        public RelayCommand SearchCmd => new(() =>
        {
            SearchTag(SearchKey);
        });

        private async void Tag_Closed(object sender, EventArgs e)
        {
            var tag = sender as Tag;
            if (tag != null)
            {
                var result = HandyControl.Controls.MessageBox.Ask($"确定要删除 {tag.Content} 吗？", "删除确认");

                if (result == MessageBoxResult.OK)
                {
                    await tagRepository.RmTagByNameAsync(tag.Content.ToString());
                    Growl.Success($"标签: {tag.Content}已删除");
                    SearchKey = "";
                }
                SearchTag(SearchKey);
            }
        }

        /// <summary>
        /// 查询标签
        /// </summary>
        /// <param name="name">关键字</param>
        private async void SearchTag(string name)
        {
            DataList.Clear();
            List<TagEntity> tags = await tagRepository.GetTagsByNameAsync(name);
            foreach (var item in tags)
            {
                Tag t = new Tag();
                t.Content = item.Name;
                t.ShowCloseButton = true;
               // t.IsSelected = DataList.Count % 2 == 0;
                t.Closed += Tag_Closed;
                t.Header = "1";
                DataList.Insert(0,t);
            }
        }

        public TagManagementViewModel(ITagRepository _tagRepository) {
            tagRepository = _tagRepository;

            DataList = new ObservableCollection<Tag>();
            SearchTag("");
        }

        
        public TagManagementViewModel() { }
    }
}
