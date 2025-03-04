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
    /// 清晰度字体颜色转换
    /// </summary>
    public class ResolutionFontColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string resolution)
            {
                SolidColorBrush brush = resolution switch
                {
                    "8K" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000")),
                    "4K" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D8D8D8")),
                    "2K" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C6B9E3")),
                    "1080P" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF")),
                    "720P" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D3A49")),
                    //"480P" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1")),
                    _ => Brushes.White
                };
                return brush;
            }
            return Brushes.White;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
