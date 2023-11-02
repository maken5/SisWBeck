using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisWBeck.Converter
{
    public class BoolToIdentificaoSalvaColorConverter : IValueConverter
    {
        private static Color CorNaoSalva = Colors.White;
        private static Color CorSalva = Colors.LightGreen;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !targetType.IsAssignableFrom(typeof(Color)))
            {
                return CorNaoSalva;
            }
            if (value is bool)
                return ((bool)value) ? CorSalva : CorNaoSalva;
            return true;
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}