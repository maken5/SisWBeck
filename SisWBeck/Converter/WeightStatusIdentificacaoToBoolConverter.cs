using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MKDComm.communication.devices.weightscales.BalancaWBeck;

namespace SisWBeck.Converter
{
    public class WeightStatusIdentificacaoToBoolConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || !targetType.IsAssignableFrom(typeof(bool)))
            {
                return false;
            }
            WeightStats status = WeightStats.Iniciando;
            string identificacao = null;
            //bool identificacaoJaSalvo = false;
            foreach (var value in values)
            {
                if (value is WeightStats)
                    status = (WeightStats)value;
                else if (value is string)
                    identificacao = (string)value;
                //else if (value is bool) 
                //    identificacaoJaSalvo = (bool)value;
            }
            //return status == WeightStats.Estavel &&
            //    !String.IsNullOrWhiteSpace(identificacao);
            return true;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
