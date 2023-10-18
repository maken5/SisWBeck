using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisWBeck.Converter
{
    public class IndexToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var listview = parameter as ListView;
            if (listview.SelectedItem != null && listview.SelectedItem == value)
                return Colors.Orange;
            var index = IndexOf(listview.ItemsSource, value);
            
            return index % 2 == 0 ? Colors.LightGray : Colors.White;
        }

        private int IndexOf(System.Collections.IEnumerable obj, object value)
        {
            if (obj != null && value !=null)
            {
                int idx = 0;
                foreach(var item in obj)
                {
                    if (item == value)
                    {
                        return idx;
                    }
                    idx++;
                }
            }
            return -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
}
