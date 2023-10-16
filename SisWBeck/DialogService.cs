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
    public class DialogService : IDialogServicePesagem, IMainNavigationService
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

        public async Task<bool> InputAlert(string title, string message, string confirmar = "SIM", string cancelar = "NÃO")
        {
            bool? resposta = await Shell.Current.CurrentPage.DisplayAlert(title, message, confirmar, cancelar);
            return resposta ?? false;
        }

        public async Task MessageError(string title, string message)
        {
            await Shell.Current.CurrentPage.DisplayAlert(title, message, "OK");
        }

        public async Task NavigateBack()
        {
            await Shell.Current.CurrentPage.Navigation.PopModalAsync();
        }

        public async Task ShowPesagem(Lotes lote)
        {
            if (lote != null)
            {
                PesagemPage pesagem = serviceProvider.GetServices<PesagemPage>().FirstOrDefault();
                if (pesagem != null)
                {
                    pesagem.SetLote(lote);
                    await Shell.Current.CurrentPage.Navigation.PushModalAsync(pesagem);
                }
            }
        }


        public async Task NavigateToMain()
        {
            await Shell.Current.GoToAsync("//inicio");
        }
    }
}
