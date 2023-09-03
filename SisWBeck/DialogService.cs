using Modelo.Entidades;
using Modelo.Tipos;
using SisWBeck.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisWBeck
{
    public class DialogService : IDialogServicePesagem
    {
        IServiceProvider serviceProvider;


        public DialogService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task<string> InputDialog(string title, string message)
        {
            string result = await Shell.Current.CurrentPage.DisplayPromptAsync(title, message);
            return result;
        }

        public async Task<bool> InputAlert(string title, string message)
        {
            bool? resposta = await Shell.Current.CurrentPage.DisplayAlert(title, message, "SIM", "NÃO");
            return resposta ?? false;
        }

        public async Task MessageError(string title, string message)
        {
            await Shell.Current.CurrentPage.DisplayAlert(title, message, "OK");
        }


        void IDialogServicePesagem.ShowPesagem(Lotes lote)
        {
            PesagemPage pesagem = serviceProvider.GetServices<PesagemPage>().FirstOrDefault();
            if (pesagem!=null)
                Shell.Current.CurrentPage.Navigation.PushModalAsync(pesagem);
        }
    }
}
