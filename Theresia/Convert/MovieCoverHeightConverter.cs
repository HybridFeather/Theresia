using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Theresia.Convert
{
    class MovieCoverHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var image = value as Image;
            if (image != null)
            {
                //double containerWidth = image.ActualWidth;
                if (image.Source == null)
                {
                    return 0;
                }
                else
                {
                    double imageHeight = image.Source.Height;
                    return -(imageHeight / 27);
                }
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
