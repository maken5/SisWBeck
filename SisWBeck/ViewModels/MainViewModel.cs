using System.Windows.Input;

namespace SisWBeck.ViewModels;

public partial class MainViewModel : BaseViewModel
{


    public ICommand CmdConfiguracao => new RelayCommand(async () => { await Shell.Current.GoToAsync("//config"); });

    public MainViewModel()
    {
    }

}
