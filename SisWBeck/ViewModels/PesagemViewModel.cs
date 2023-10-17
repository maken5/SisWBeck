using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui;
using MKDComm.communication.devices.weightscales;
using mkdinfo.communication.media;
using mkdinfo.communication.protocol;
using Modelo.Entidades;
using Modelo.Tipos;
using SisWBeck.Comm;
using SisWBeck.DB;
using SisWBeck.Modelo;
using System.ComponentModel;
using static MKDComm.communication.devices.weightscales.BalancaWBeck;

namespace SisWBeck.ViewModels
{
    public partial class PesagemViewModel : BaseViewModel, IDisposable
    {

        #region Atributos e métodos privados ---------------------------------------------
        private BluetoothHelper bluetoothHelper;
        private bool disposedValue;
        private Balanca balanca;
        private Config config;
        
        private ControleLotes _controleLote;
        #endregion
        #region ctor ---------------------------------------------------------------------
        public PesagemViewModel(SISWBeckContext context,
                                IDialogService dialogService,
                                BluetoothHelper bluetoothHelper) : base(context, dialogService)
        {
            this.bluetoothHelper = bluetoothHelper;
            this.config = context.GetConfig();
            this.balanca = new Balanca(bluetoothHelper, config);
        }
        #endregion

        #region Propriedades públicas ----------------------------------------------------
        public ControleLotes Lote
        {
            get => _controleLote; 
            set => Set(ref _controleLote, value);
        }
        public Keyboard TipoTeclado => this.config.UsarTecladoNumerico ?
                                            Keyboard.Numeric : Keyboard.Default;

        
        public Balanca Balanca
        {
            get
            {
                if (balanca == null)
                {
                    balanca = new Balanca(bluetoothHelper, context.GetConfig());
                }
                return balanca;
            }
            private set => SetProperty(ref balanca, value);
        }
        #endregion

        #region Métodos públicos ---------------------------------------------------------
        public void SetLote(Lotes lote)
        {
            Lote = new ControleLotes(lote, context);
        }
        public bool Finalizar()
        {
            try
            {
                balanca.Stop();
            }
            catch { }
            return true;
        }

        #endregion


        #region Command ------------------------------------------------------------------
        [RelayCommand]
        public async Task Voltar()
        {
            if ((Balanca?.IsContectado ?? false) &&
                Lote !=null)
            {
                bool voltar = await dialogService.InputAlert("Sair da pesagem?", 
                    $"Deseja sair da pesagem do lote {Lote.Nome}?");
                if (!voltar) return;
            }
            try
            {
                balanca.Stop();
                balanca.Dispose();
            }
            catch { }
            balanca = null;
            await dialogService.NavigateBack();
        }

        [RelayCommand]
        void Appearing()
        {
            balanca.Start();
        }

        [RelayCommand]
        void Disapearing()
        {
            balanca.Stop();
        }


        #endregion


        #region IDisposable --------------------------------------------------------------
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (balanca != null)
                    {
                        try { balanca.Stop(); } catch { }
                    }
                    balanca = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PesagemViewModel()
        // {
        //     // Não altere este código. Coloque o código de limpeza no método 'Dispose(bool disposing)'
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Não altere este código. Coloque o código de limpeza no método 'Dispose(bool disposing)'
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
