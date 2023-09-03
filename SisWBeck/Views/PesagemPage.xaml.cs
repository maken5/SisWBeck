namespace SisWBeck.Views
{
    public partial class PesagemPage : ContentPage
    {
        public PesagemPage(PesagemViewModel viewModel)
        {
            InitializeComponent(); 
            BindingContext = viewModel;
        }
    }
}
