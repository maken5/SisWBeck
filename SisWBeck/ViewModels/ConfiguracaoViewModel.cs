using MKDComm.communication.interfaces;
using Modelo.Tipos;
using SisWBeck.DB;
using System.Windows.Input;

namespace SisWBeck.ViewModels
{
    public partial class ConfiguracaoViewModel : BaseViewModel
    {
        IHALCommFactory CommFactory;
        new IMainNavigationService dialogService;

        public Config Cfg { get; set; }
        public ConfiguracaoViewModel(SISWBeckContext context,
                                     IMainNavigationService dialogService,
                                     IHALCommFactory CommFactory) : base(context, dialogService)
        {
            this.CommFactory = CommFactory;
            this.dialogService = dialogService;
            Cfg = context.GetConfig();
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

        public ICommand SaveCommand => new RelayCommand(async () =>
        {
            IsModificado = false;
            try
            {
                context.UpdateConfig(Cfg);
                
            }catch (Exception ex)
            {
                await dialogService.MessageError("Erro salvando configurações", ex.Message);
            }
            await this.dialogService.NavigateToMain();
        });

    }
}
