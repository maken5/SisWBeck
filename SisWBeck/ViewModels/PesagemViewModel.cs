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
        private DateTime dtUltimaPesagem = DateTime.MinValue;
        
        private ControleLotes _controleLote;
        private int UltimoPesoRegistrado = 0;
        private bool executandoAltercacoConfiguracaoBalanca = false;

        private WeightStats lastWeightscaleStatus = WeightStats.Iniciando;
        private  bool PodeExecutarAlteracaoConfigBalanca() => !executandoAltercacoConfiguracaoBalanca &&
                                                          (balanca.Status == WeightStats.Pesando || balanca.Status == WeightStats.Estavel);


        private void Balanca_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Balanca.Peso) && UltimoPesoRegistrado > 0)
            {
                if (Balanca.Peso < UltimoPesoRegistrado / 2)
                {
                    UltimoPesoRegistrado = 0;
                    Identificacao = "";
                }
            }
        }
        private void Balanca_OnStatusChanged(WeightStats status)
        {

            if (lastWeightscaleStatus != status)
            {
                lastWeightscaleStatus = status;
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ToogleAutozeroCommand.NotifyCanExecuteChanged();
                    SelecionaMemoriaCommand.NotifyCanExecuteChanged();
                });
            }
        }
        private void Balanca_OnReadConfigEnd()
        {
            executandoAltercacoConfiguracaoBalanca = false;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                ToogleAutozeroCommand.NotifyCanExecuteChanged();
                SelecionaMemoriaCommand.NotifyCanExecuteChanged();
            });

        }
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
            this.balanca.OnStatusChanged += Balanca_OnStatusChanged;
            this.Balanca.OnReadConfigEnd += Balanca_OnReadConfigEnd;
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

        [RelayCommand(CanExecute = nameof(PodeExecutarAlteracaoConfigBalanca))]
        void ToogleAutozero()
        {
            executandoAltercacoConfiguracaoBalanca = true;
            ToogleAutozeroCommand.NotifyCanExecuteChanged();
            SelecionaMemoriaCommand.NotifyCanExecuteChanged();
            if (Balanca != null && 
                (Balanca.Status == WeightStats.Estavel || Balanca.Status== WeightStats.Pesando))
            {
                if (Balanca.ToogleAutoZero())
                    return;
            }
            executandoAltercacoConfiguracaoBalanca = false;
            ToogleAutozeroCommand.NotifyCanExecuteChanged();
            SelecionaMemoriaCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute =nameof(PodeExecutarAlteracaoConfigBalanca))]
        async Task SelecionaMemoria()
        {
            executandoAltercacoConfiguracaoBalanca = true;
            SelecionaMemoriaCommand.NotifyCanExecuteChanged();
            ToogleAutozeroCommand.NotifyCanExecuteChanged();
            string s = Balanca.MemoriaStr;
            if (Balanca.Calibracoes!=null && Balanca.Calibracoes.Any())
            {
                List<string> memorias = Balanca.Calibracoes.Select(x => x.Nome).OrderBy(x=>x).ToList();
                string str = await dialogService.SelectDialog("Selecione a Calibração", memorias);
                if (!String.IsNullOrWhiteSpace(str))
                {
                    bool r = !Balanca.SelecionarMemoriaCalibracao(str);
                    if (r != executandoAltercacoConfiguracaoBalanca)
                    {
                        executandoAltercacoConfiguracaoBalanca = r;
                        SelecionaMemoriaCommand.NotifyCanExecuteChanged();
                        ToogleAutozeroCommand.NotifyCanExecuteChanged();
                    }
                    return;
                }
            }
            executandoAltercacoConfiguracaoBalanca = false;
            SelecionaMemoriaCommand.NotifyCanExecuteChanged();
            ToogleAutozeroCommand.NotifyCanExecuteChanged();
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
                        this.balanca.PropertyChanged -= Balanca_PropertyChanged;
                        this.balanca.OnStatusChanged -= Balanca_OnStatusChanged;
                        this.Balanca.OnReadConfigEnd -= Balanca_OnReadConfigEnd;
                        try { balanca.Stop(); } catch { }

                    }
                    balanca = null;
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Não altere este código. Coloque o código de limpeza no método 'Dispose(bool disposing)'
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
