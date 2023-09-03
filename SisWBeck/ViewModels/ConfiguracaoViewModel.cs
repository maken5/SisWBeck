using Modelo.Tipos;
using SisWBeck.DB;

namespace SisWBeck.ViewModels
{
    public partial class ConfiguracaoViewModel : BaseViewModel
    {
        public ConfiguracaoViewModel(SISWBeckContext context, 
                                     IDialogService dialogService) : base(context, dialogService)
        {
        }
    }
}
