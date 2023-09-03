using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Modelo.Tipos;
using SisWBeck.DB;
using System.ComponentModel;

namespace SisWBeck.ViewModels
{
    public partial class PesagemViewModel : BaseViewModel
    {
        public PesagemViewModel(SISWBeckContext context, IDialogService dialogService) : base(context, dialogService)
        {
        }
    }
}
