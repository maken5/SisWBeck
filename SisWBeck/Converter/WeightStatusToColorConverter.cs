using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MKDComm.communication.devices.weightscales.BalancaWBeck;

namespace SisWBeck.Converter
{
    public class WeightStatusToColorConverter : IMultiValueConverter
    {
        private static Color CorIniciando = Color.FromRgb(255, 165, 0);
        private static Color CorZerando = Color.FromRgb(255, 165, 0);
        private static Color CorPesoNegativo = Color.FromRgb(255, 0, 0);
        private static Color CorDesconectado = Color.FromRgb(255, 0, 0);
        private static Color CorPesoEstavel = Colors.LightGreen;
        private static Color CorPesando = Colors.White;
        private static Color CorPadrao = Colors.LightGray;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || !targetType.IsAssignableFrom(typeof(Color)))
            {
                return CorPadrao;
            }
            int peso=0;
            WeightStats status = WeightStats.Iniciando;
            foreach (var value in values)
            {
                if (value is int)
                    peso = (int)value;
                else if (value is WeightStats)
                    status = (WeightStats)value;
            }
            switch (status)
            {
                case WeightStats.Iniciando:
                    return CorIniciando;
                case WeightStats.Pesando:
                    if (peso < 0)
                        return CorPesoNegativo;
                    return CorPesando;
                case WeightStats.Zerando:
                    return CorZerando;
                case WeightStats.Estavel:
                    return CorPesoEstavel;
                case WeightStats.Desconectado:
                default:
                    return CorDesconectado;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
