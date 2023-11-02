using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui;
using MKDComm.communication.devices.weightscales;
using mkdinfo.communication.media;
using mkdinfo.communication.protocol;
using Modelo.Entidades;
using Modelo.Tipos;
using SisWBeck.Comm;
using SisWBeck.Converter;
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
        private int UltimoPesoRegistrado = 0;
        #endregion
        #region ctor ---------------------------------------------------------------------
        public PesagemViewModel(SISWBeckContext context,
                                IDialogService dialogService,
                                BluetoothHelper bluetoothHelper) : base(context, dialogService)
        {
            this.bluetoothHelper = bluetoothHelper;
            this.config = context.GetConfig();
            this.balanca = new Balanca(bluetoothHelper, config);
            this.balanca.PropertyChanged += Balanca_PropertyChanged;
        }

        private void Balanca_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Balanca.Peso) && UltimoPesoRegistrado>0)
            {
                if (Balanca.Peso < UltimoPesoRegistrado / 2)
                {
                    UltimoPesoRegistrado = 0;
                    Identificacao = "";
                }
            }
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

        private string _identificacao;
        public string Identificacao
        {
            get => _identificacao;
            set
            {
                if (Set(ref _identificacao, value))
                {
                    IsIdentificacaoSalva = false;
                }
            }
        }

        private bool _IsIdentificacaoSalva = false;
        public bool IsIdentificacaoSalva
        {
            get => _IsIdentificacaoSalva;
            private set =>Set(ref _IsIdentificacaoSalva, value);
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
                Balanca.Stop();
            }
            catch { }
            return true;
        }

        #endregion

        
        #region Command ------------------------------------------------------------------
        [RelayCommand]
        public async Task Voltar()
        {
            bool voltar = await dialogService.InputAlert("Sair da pesagem?",
                    $"Deseja sair da pesagem do lote {Lote.Nome}?");
            if (!voltar) return;
            try
            {
                Balanca.Stop();
                Balanca.Dispose();
            }
            catch { }
            Balanca = null;
            await dialogService.NavigateBack();
        }

        [RelayCommand]
        void Appearing()
        {
            if (Balanca != null) Balanca.Start();
        }

        [RelayCommand]
        void Disapearing()
        {
            if (balanca != null) balanca.Stop();
        }

        [RelayCommand]
        async Task ApagarPesagem()
        {
            if (Lote.PesagemSelecionada == null)
                await dialogService.MessageError("Nenhuma pesagem selecionada!", "Selecione um registro de pesagem para ser removido");
            else
            {
                if (await dialogService.InputAlert("Deseja apagar a seguinte pesagem?", $"Identificação: {Lote.PesagemSelecionada.Codigo}\nData: {Lote.PesagemSelecionada.Data}\nPeso:{Lote.PesagemSelecionada.Peso} Kg"))
                {
                    try
                    {
                        await Lote.RemovePesagem();
                    }catch (Exception ex)
                    {
                        await dialogService.MessageError("Erro removendo pesagem", ex.Message);
                    }
                }
            }
        }

        [RelayCommand]
        void OutrasOpcoes()
        {
        }

        [RelayCommand]
        void Zerar()
        {
            if (Balanca != null) Balanca.Zerar();
        }

        [RelayCommand]
        async Task Registrar()
        {
            if (!string.IsNullOrWhiteSpace(Identificacao) &&
                (Balanca.Status  == WeightStats.Estavel) &&
                Balanca.Peso > 0)
            {
                if (Lote.IdentificacaoJaSalva(Identificacao))
                {
                    bool salvar = await dialogService.InputAlert("Pesagem já salva!",
                                        $"O animal {Identificacao} já foi pesado na pesagem {Lote.NrPesagem}, atualizar o peso?");
                    if (!salvar) return;
                }
                UltimoPesoRegistrado = Balanca.Peso;
                await Lote.SavePesagem(Identificacao, UltimoPesoRegistrado);
                IsIdentificacaoSalva = true;
            }
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
                        balanca.PropertyChanged -= Balanca_PropertyChanged;
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
