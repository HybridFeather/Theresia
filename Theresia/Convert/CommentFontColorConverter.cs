using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Theresia.Convert
{
    /// <summary>
    /// 评论字体颜色转换
    /// </summary>
    public class CommentFontColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string comment)
            {
                if (string.IsNullOrEmpty(comment))
                {
                    return Brushes.Gainsboro;
                }
            }
            return Brushes.CornflowerBlue;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
