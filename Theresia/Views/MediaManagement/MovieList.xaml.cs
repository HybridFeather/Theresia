using HandyControl.Controls;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Theresia.ViewModels.MediaManagement.Event;

namespace Theresia.Views.MediaManagement
{
    /// <summary>
    /// MovieList.xaml 的交互逻辑
    /// </summary>
    public partial class MovieList : UserControl
    {
        private readonly IEventAggregator eventAggregator;

        public MovieList(IEventAggregator _eventAggregator)
        {
            InitializeComponent();
            eventAggregator = _eventAggregator;
            // 订阅事件
            _eventAggregator.GetEvent<ShowProgressBarEvent>().Subscribe(ProgressBar_Show);
            _eventAggregator.GetEvent<HideProgressBarEvent>().Subscribe(ProgressBar_Hide);
            _eventAggregator.GetEvent<ShowPaginationEvent>().Subscribe(Pagination_Show);
            _eventAggregator.GetEvent<HidePaginationEvent>().Subscribe(Pagination_Hide);
        }
        // 展开时触发的事件
        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            var storyboard = (Storyboard)this.Resources["ExpandStoryboard"];
            storyboard.Begin();
        }

        // 收起时触发的事件
        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            var storyboard = (Storyboard)this.Resources["CollapseStoryboard"];
            storyboard.Begin();
        }

        private void ProgressBar_Show()
        {
            var storyboard = (Storyboard)this.Resources["ShowProgressBarStoryboard"];
            storyboard.Begin();
        }

        // 收起时触发的事件
        private void ProgressBar_Hide()
        {
            var storyboard = (Storyboard)this.Resources["HideProgressBarStoryboard"];
            storyboard.Begin();
        }
        /// <summary>
        /// 页码条出现
        /// </summary>
        private void Pagination_Show()
        {
            var storyboard = (Storyboard)this.Resources["ShowPaginationStoryboard"];
            storyboard.Begin();
        }
        /// <summary>
        /// 页码条隐藏
        /// </summary>
        private void Pagination_Hide()
        {
            var storyboard = (Storyboard)this.Resources["HidePaginationStoryboard"];
            storyboard.Begin();
        }

        private void Card_MouseEnter(object sender, MouseEventArgs e)
        {
            var storyboard = this.Resources["CardHoverStoryboard"] as Storyboard;
            if (sender is HandyControl.Controls.Card card)
            {
                storyboard.Begin(card);
            }
        }

        private void Card_MouseLeave(object sender, MouseEventArgs e)
        {
            var storyboard = this.Resources["CardLeaveStoryboard"] as Storyboard;
            if (sender is HandyControl.Controls.Card card)
            {
                storyboard.Begin(card);
            }
        }

        // MinRating 的事件处理
        private void MinRating_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 如果是小数点，允许输入
            if (e.Text == "." && !((HandyControl.Controls.TextBox)sender).Text.Contains("."))
            {
                return; // 允许小数点输入
            }

            // 如果是数字，允许输入
            e.Handled = !IsInputValid(e.Text);
        }

        private void MinRating_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            // 禁止空格和回车键
            if (e.Key == Key.Space || e.Key == Key.Enter)
            {
                e.Handled = true;
            }
        }

        private void MinRating_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            // 进行最大值和最小值限制
            if (double.TryParse(MinRating.Text, out double value))
            {
                if (value > 5)
                {
                    MinRating.Text = "5"; // 设置为最大值
                }
                else if (value < 0)
                {
                    MinRating.Text = "0"; // 设置为最小值
                }
            }
        }

        // MaxRating 的事件处理
        private void MaxRating_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 如果是小数点，允许输入
            if (e.Text == "." && !((HandyControl.Controls.TextBox)sender).Text.Contains("."))
            {
                return; // 允许小数点输入
            }

            // 如果是数字，允许输入
            e.Handled = !IsInputValid(e.Text);
        }

        private void MaxRating_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            // 禁止空格和回车键
            if (e.Key == Key.Space || e.Key == Key.Enter)
            {
                e.Handled = true;
            }
        }

        private void MaxRating_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            // 进行最大值和最小值限制
            if (double.TryParse(MaxRating.Text, out double value))
            {
                if (value > 5)
                {
                    MaxRating.Text = "5"; // 设置为最大值
                }
                else if (value < 0)
                {
                    MaxRating.Text = "0"; // 设置为最小值
                }
            }
        }

        // 验证输入是否为数字或小数点
        private bool IsInputValid(string text)
        {
            // 只允许输入数字
            return double.TryParse(text, out _);
        }

    }
}
