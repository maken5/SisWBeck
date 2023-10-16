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
        }

        public void SetLote(Lotes lote)
        {
            if (model!=null)
                model.SetLote(lote);
        }

    }
}
