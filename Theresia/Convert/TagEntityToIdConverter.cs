using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Theresia.Entity;

namespace Theresia.Convert
{
    public class TagEntityToIdConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ICollection<int>)
            {
                ObservableCollection<int> list = (ObservableCollection<int>)value;
                //List<int> result = new List<int>(value.coun)
                return list;  // 返回 Id
            }
            return null;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 如果需要返回 TagEntity 对象，可以创建它
            if (value is TagEntity entity)
            {
                return entity.Id;  // 这里可以根据需要创建 TagEntity 对象
            }
            return null;
        }
    }

}
