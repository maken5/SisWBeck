using Modelo.Entidades;
using SisWBeck.DB;
using SisWBeck.Modelo;

namespace SisWBeck.Views
{
    public partial class PesagemPage : ContentPage
    {
        PesagemViewModel model;
        public PesagemPage(PesagemViewModel viewModel)
        {
            InitializeComponent(); 
            BindingContext = viewModel;
            this.model = viewModel;
            DeviceDisplay.Current.KeepScreenOn = true;
        }

        public void SetLote(Lotes lote)
        {
            if (model!=null)
                model.SetLote(lote);
        }

        protected override bool OnBackButtonPressed()
        {
            if (model.Finalizar())
                return base.OnBackButtonPressed();
            return false;
        }



        protected override void OnDisappearing()
        {
            if (model.Balanca != null)
                try { model.Balanca.Stop(); }catch (Exception) { }
            base.OnDisappearing();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            //workaround bug MAUI - Input nem sempre esconde teclado quando perde focus
            Platforms.KeyboardHelper.HideKeyboard();
        }
    }
}
