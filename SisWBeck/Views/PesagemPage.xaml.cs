using Microsoft.Maui.Animations;
using Modelo.Entidades;
using SisWBeck.DB;
using SisWBeck.Modelo;
using Microsoft.Maui.Controls;

namespace SisWBeck.Views
{
    public partial class PesagemPage : ContentPage
    {
        PesagemViewModel model;

        bool IsBalancaPropertiesVisible = false;
        bool IsBalancaPropertiesAnimationRunning = false;
        Microsoft.Maui.Controls.Animation BalancaShowPropertiesAnimation = null;
        Microsoft.Maui.Controls.Animation BalancaHidePropertiesAnimation = null;

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

        private void ButtonBalanca_Clicked(object sender, EventArgs e)
        {
            if (BalancaShowPropertiesAnimation == null)
            {
                BalancaShowPropertiesAnimation = new Microsoft.Maui.Controls.Animation(v => SessaoPesagem.Height = v, 270, 330);
            }
            if (BalancaHidePropertiesAnimation == null)
            {
                BalancaHidePropertiesAnimation = new Microsoft.Maui.Controls.Animation(v => SessaoPesagem.Height = v, 330, 270);
            }
            if (!IsBalancaPropertiesAnimationRunning)
            {
                IsBalancaPropertiesAnimationRunning = true;
                if (!IsBalancaPropertiesVisible)
                {
                    BalancaShowPropertiesAnimation.Commit(this,
                                                          "ShowBalancaProperties",
                                                          16,
                                                          250,
                                                          Easing.Linear,
                                                          (v, c) => { IsBalancaPropertiesAnimationRunning = false;IsBalancaPropertiesVisible = true; }) ;
                }
                else
                {
                    BalancaHidePropertiesAnimation.Commit(this,
                                                          "HideBalancaProperties",
                                                          16,
                                                          250,
                                                          Easing.Linear,
                                                          (v, c) => { IsBalancaPropertiesAnimationRunning = false; IsBalancaPropertiesVisible = false; });
                }
            }
            //if (SessaoPesagem.Height.Value == 270)
            //{
            //    SessaoPesagem.Height = new GridLength(350);
            //}
            //else
            //{
            //    SessaoPesagem.Height = new GridLength(270);
            //}
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            //workaround bug MAUI - Input nem sempre esconde teclado quando perde focus
            Platforms.KeyboardHelper.HideKeyboard();
        }
    }
}
