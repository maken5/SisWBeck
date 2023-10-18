using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MKDComm.communication.devices.weightscales.BalancaWBeck;

namespace SisWBeck.Converter
{
    public class BoolToPesoEstavelParaSalvarColorConverter : IValueConverter
    {
        private static Color CorInativo = Colors.LightGray;
        private static Color CorAtivo = Colors.LightGreen;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !targetType.IsAssignableFrom(typeof(Color)))
            {
                return CorInativo;
            }
            if (value is bool)
                return ((bool)value) ? CorAtivo : CorInativo;
            return true;
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
