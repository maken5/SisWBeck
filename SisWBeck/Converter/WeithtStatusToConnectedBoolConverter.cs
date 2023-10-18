using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MKDComm.communication.devices.weightscales.BalancaWBeck;

namespace SisWBeck.Converter
{
    public class WeithtStatusToConnectedBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is WeightStats)
            {
                WeightStats st = (WeightStats)value;
                return st != WeightStats.Desconectado && st != WeightStats.Iniciando; 
            }
            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
