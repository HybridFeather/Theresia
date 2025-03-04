using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Theresia.Convert
{
    /// <summary>
    /// 分辨率底色转换器
    /// </summary>
    public class ResolutionBackgroundConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string resolution)
            {
                SolidColorBrush brush = resolution switch
                {
                    "8K" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD700")),
                    "4K" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#34B3A0")),
                    "2K" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6A4CFF")),
                    "1080P" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6F61")),
                    "720P" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A9E1D4")),
                    //"480P" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3C4E64")),
                    _ => Brushes.DimGray
                };
                return brush;
            }
            return Brushes.DimGray;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
