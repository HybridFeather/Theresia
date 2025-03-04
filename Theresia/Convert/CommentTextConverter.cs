using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Theresia.Convert
{
    /// <summary>
    /// 评论内容转换器
    /// </summary>
    public class CommentTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string comment)
            {
                if (string.IsNullOrEmpty(comment))
                {
                    return "未评论";
                }
                else
                {
                    if (comment.Length > 8)
                    {
                        comment = comment[..8] + "...";
                    }
                    return $"\"{comment}\"";
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
