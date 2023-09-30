namespace SisWBeck.Views
{
    public partial class ConfiguracaoPage : ContentPage
    {
        public ConfiguracaoPage(ConfiguracaoViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }


        public void Teste()
        {
        }
    }
}
