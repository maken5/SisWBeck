using Modelo.Tipos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisWBeck
{
    public class DialogService : IDialogService
    {
        public async Task<string> InputDialog(string title, string message)
        {
            string result = await Application.Current.MainPage.DisplayPromptAsync(title, message);
            return result;
        }

        public async Task<bool> InputAlert(string title, string message)
        {
            bool? resposta = await Application.Current.MainPage.DisplayAlert(title, message, "SIM", "NÃO");
            return resposta ?? false;
        }
    }
}
