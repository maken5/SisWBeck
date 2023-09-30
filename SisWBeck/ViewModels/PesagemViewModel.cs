using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui;
using MKDComm.communication.devices.weightscales;
using mkdinfo.communication.media;
using mkdinfo.communication.protocol;
using Modelo.Tipos;
using SisWBeck.Comm;
using SisWBeck.DB;
using System.ComponentModel;
using static MKDComm.communication.devices.weightscales.BalancaWBeck;

namespace SisWBeck.ViewModels
{
    public partial class PesagemViewModel : BaseViewModel, IDisposable
    {
        private BluetoothHelper bluetoothHelper;
        private bool disposedValue;
        private Balanca balanca;
        public Balanca Balanca
        {
            get
            {
                if (balanca == null)
                {
                    balanca = new Balanca(bluetoothHelper, context.Config);
                }
                return balanca;
            }
            private set => SetProperty(ref balanca, value);
        }
        public PesagemViewModel(SISWBeckContext context, 
                                IDialogService dialogService,
                                BluetoothHelper bluetoothHelper) : base(context, dialogService)
        {
            this.bluetoothHelper = bluetoothHelper;
        }

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
    }
}
