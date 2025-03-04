using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Theresia.Convert
{
    public class NegateBoolToVisibilityConverter : IValueConverter
    {
        // 将 bool 值取反后转换为 Visibility
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool booleanValue)
            {
                // 取反 bool 值再转换为 Visibility
                return booleanValue ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Visible; // 默认显示
        }

        // ConvertBack 是可选的，可以在需要双向绑定时使用
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                // 将 Visibility 取反后转换回 bool
                return visibility == Visibility.Collapsed;
            }
            return false; // 默认返回 false
        }
    }

}
