using MKDComm.communication.interfaces;
using Modelo.Tipos;
using SisWBeck.DB;
using System.Windows.Input;

namespace SisWBeck.ViewModels
{
    public partial class ConfiguracaoViewModel : BaseViewModel
    {
        IHALCommFactory CommFactory;
        Config config = new Config();
        public ConfiguracaoViewModel(SISWBeckContext context,
                                     IDialogService dialogService,
                                     IHALCommFactory CommFactory) : base(context, dialogService)
        {
            this.CommFactory = CommFactory;
            List<string> lista = CommFactory.GetDevices();
            foreach (string device in lista)
            {
                Balancas.Add(device);
            }
        }


        private ObservableCollection<string> _Balancas = new ObservableCollection<string>();
        public ObservableCollection<string> Balancas
        {
            get => _Balancas;
            set => Set(ref _Balancas, value);
        }

        private string _Balanca;
        public string Balanca
        {
            get => _Balanca;
            set => Set(ref _Balanca, value);
        }

        private bool _usarTecladoNumerico;
        public bool UsarTecladoNumerico
        {
            get => _usarTecladoNumerico;
            set => Set(ref _usarTecladoNumerico, value);
        }


        private bool _UsarPontoEVirgula;
        public bool UsarPontoEVirgula
        {
            get => _UsarPontoEVirgula;
            set => Set(ref _UsarPontoEVirgula, value);
        }

        public ICommand SaveCommand => new RelayCommand(() =>
        {
            IsModificado = false;
        });

    }
}
