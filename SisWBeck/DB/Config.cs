using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisWBeck.DB
{
    public class Config
    {
        public string Balanca
        {
            get => Preferences.Default.Get("Balança", "");
            set => Preferences.Default.Set("Balança", value);
        }
        public bool UsarTecladoNumerido
        {
            get => Preferences.Default.Get("UsarTecladoNumerico", "S") == "S";
            set => Preferences.Default.Set("UsarTecladoNumerico", value);
        }
        public bool UsarPontoVirgula
        {
            get => Preferences.Default.Get("UsarPontoVirgula", "S") == "S";
            set => Preferences.Default.Set("UsarPontoVirgula", value);
        }


    }
}
