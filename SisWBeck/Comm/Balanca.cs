using MKDComm.communication.devices.weightscales;
using mkdinfo.communication.media;
using mkdinfo.communication.protocol;
using SisWBeck.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MKDComm.communication.devices.weightscales.BalancaWBeck;

namespace SisWBeck.Comm
{
    public class Balanca : ObservableObject, INotifyPropertyChanged, IDisposable
    {

        #region ctor ---------------------------------------------------------------------

        

        public Balanca(BluetoothHelper bluetoothHelper, Config config)
        {
            this.bluetoothHelper = bluetoothHelper;
            this.config = config;
            if (!String.IsNullOrWhiteSpace(config.Balanca))
            {
                comm = bluetoothHelper.getBluetoothConnection(config.Balanca);
                if (comm != null) {
                    wbeck = new BalancaWBeck(comm);
                    wbeck.onDisconnect += new OnDisconnect(this.OnDisconnect);
                    wbeck.onReadConfigEnd += new OnReadConfigEnd(this.OnConfigEnd);
                    wbeck.onErrorReceived += new OnError(this.OnErrorReceive);
                    wbeck.onWeightStatusReceived += new OnWeightStatusReceived(this.OnNewWeight);
                }
            }
        }

        #endregion

        #region Atributos e métodos privados ---------------------------------------------
        
        //Objetos (serviços e dados)
        private BluetoothHelper bluetoothHelper;
        private Config config;
        private HALCommMediaBase comm = null;
        private BalancaWBeck wbeck = null;

        //atributos de controle interno
        private bool disposedValue;
        private bool lerConfiguracoes=true;
        private object _lock = new object();
        private bool ForceUpdate = false;
        private int reconexoes = 0;
        private readonly int MAX_RECONEXOES = 5;

        // Métodos internos  de notificação
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (!String.IsNullOrWhiteSpace(propertyName))
                MainThread.BeginInvokeOnMainThread(() => {
                    if (ForceUpdate)
                    {
                        ForceUpdate = false;
                        OnPropertyChanged(nameof(Peso));
                        OnPropertyChanged(nameof(PesoStr));
                        OnPropertyChanged(nameof(Status));
                    }
                    OnPropertyChanged(propertyName);
                });
        }

        private void NotifyPesoByStatus(WeightStats old_status, WeightStats new_status)
        {
            switch (old_status)
            {
                case WeightStats.Iniciando:
                case WeightStats.Desconectado:
                    if (new_status != WeightStats.Iniciando && new_status != WeightStats.Desconectado)
                    {
                        RaisePropertyChanged(nameof(PesoPositivo));
                        RaisePropertyChanged(nameof(PesoStr));
                    }
                    break;
                case WeightStats.Estavel:
                case WeightStats.Pesando:
                case WeightStats.Zerando:
                    if (new_status == WeightStats.Iniciando || new_status == WeightStats.Desconectado)
                    {
                        RaisePropertyChanged(nameof(PesoPositivo));
                        RaisePropertyChanged(nameof(PesoStr));
                    }else if (new_status == WeightStats.Zerando)
                        RaisePropertyChanged(nameof(PesoPositivo));
                    break;
            }
        }

        protected bool Set<T>(ref T prop, T value, [CallerMemberName] string propertyName = null)
            where T : struct
        {
            if (!prop.Equals(value))
            {
                prop = value;
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }
        protected T Get<T>(ref T prop)
            where T : struct
        {
            lock (_lock)
            {
                return prop;
            }
        }
        #endregion

        #region Propriedades públicas ----------------------------------------------------

        #region propriedades principais de controle --------------------------------------
        private int _peso;
        private WeightStats _status = WeightStats.Iniciando;

        public int Peso
        {
            get => Get(ref _peso);
            private set
            {
                int old_peso = 0;
                lock (_lock)
                {
                    old_peso = _peso;
                    if (_peso != value)
                        _peso = value;
                }
                if (old_peso != value || (_status!= WeightStats.Pesando && _status!= WeightStats.Estavel))
                {
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(PesoStr));
                    if ((old_peso <= 0 && value > 0) || (old_peso > 0 && value <= 0))
                        RaisePropertyChanged(nameof(PesoPositivo));
                }
            }
        }
        public WeightStats Status
        {
            get => Get(ref _status);
            set
            {
                WeightStats? old_status = null;
                lock (_lock)
                {
                    old_status = _status;
                    if (_status != value)
                        _status = value;
                }
                RaisePropertyChanged();
                NotifyPesoByStatus(old_status.Value, value);
                if (value == WeightStats.Estavel && Peso>0)
                    PesoEstavelParaRegistrar = true;
                else PesoEstavelParaRegistrar = false;
            }
        }
        #endregion

        #region propriedades auxiliares de controle --------------------------------------
        private bool pesoEstavelRegistrado = false;
        public bool PesoEstavelParaRegistrar
        {
            get => Get(ref pesoEstavelRegistrado);
            set => Set(ref pesoEstavelRegistrado, value);
        }
        public string Nome => this.config.Balanca;
        public string PesoStr
        {
            get
            {
                switch (Status)
                {
                    case WeightStats.Iniciando:
                        return "Conect";
                    case WeightStats.Pesando:
                    case WeightStats.Estavel:
                        return Peso.ToString(); 
                    case WeightStats.Zerando:
                        return "Zero";
                    case WeightStats.Desconectado:
                    default:
                        return "---";
                }
            }
        }
        public bool PesoPositivo => Peso>0 && (Status == WeightStats.Pesando || Status == WeightStats.Estavel);
        #endregion

        #endregion

        #region Métodos públicos ---------------------------------------------------------
        public bool Zerar()
        {
            if (wbeck != null)
                try
                {
                    wbeck.Zerar();
                    Status = WeightStats.Zerando;
                    return true;
                }
                catch { return false; };
            return false;
        }
        public void Reconectar()
        {
            if (wbeck!=null && reconexoes< MAX_RECONEXOES)
            {
                Stop();
                reconexoes++;
                Status = WeightStats.Iniciando;
                Thread.Sleep(250);
                Start();
            }
        }
        public void Start()
        {
            ForceUpdate = true;
            if (wbeck != null)
            {
                lerConfiguracoes = true;
                try { wbeck.Start(); } catch { }
            }
        }
        public void Stop()
        {
            if (wbeck != null)
            {
                try { wbeck.Stop(); } catch { }
            }
        }

        #endregion

        #region Métodos callback ---------------------------------------------------------

        public void OnErrorReceive(Exception ex)
        {
            Console.WriteLine("OnErrorReceive");
            Reconectar();
        }
        protected virtual void OnNewWeight(int? peso, WeightStats weightStats)
        {
            if (peso != null)
                Peso = peso.Value;
            Status = weightStats;
            if (reconexoes > 0) reconexoes = 0;
        }

        protected virtual void OnDisconnect()
        {
            Console.WriteLine("OnDisconect");
        }

        private void OnConfigEnd(ProtocoloModuloPesagemSMAX.SensorCalibrationResponse[] calibracoes, int? calibarcaoIdx, bool autozero, string numeroSerie, bool requireLicenseKey)
        {
            //EnabledConnectingFlag = true;
            //WeightVisibility = false;
            //Weight = "";
            //TextAlert = "Leitura de Configuração finalizada";
            //TextAlertVisibility = true;
            //WeightStats = WeightStats.Iniciando;
            //WeightStatus = "Confgiuração Finalizada";
            //WeightTextColor = "Black";
            //WeightBackgroundColor = "White";
            //SaveButtonColor = "LightGray";
            //KgVisibility = false;

        }

        private void ReadingInfos(int step, string name)
        {
            lerConfiguracoes = false;
            //EnabledConnectingFlag = true;
            //WeightVisibility = false;
            //Weight = "";
            //TextAlert = String.Format("Passo {0}: {1}", step, name);
            //TextAlertVisibility = true;
            //WeightStats = WeightStats.Iniciando;
            //WeightStatus = "Lendo";
            //WeightTextColor = "Black";
            //WeightBackgroundColor = "White";
            //SaveButtonColor = "LightGray";
            //KgVisibility = false;
        }

        #endregion

        #region IDisposable --------------------------------------------------------------

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if ( wbeck!=null)
                    {
                        try
                        {
                            wbeck.Stop();
                        }
                        catch { }
                        wbeck.onDisconnect -= new OnDisconnect(this.OnDisconnect);
                        wbeck.onReadConfigEnd -= new OnReadConfigEnd(this.OnConfigEnd);
                        wbeck.onErrorReceived -= new OnError(this.OnErrorReceive);
                        wbeck.onWeightStatusReceived -= new OnWeightStatusReceived(this.OnNewWeight);
                    }
                    // TODO: dispose managed state (managed objects)
                    if (comm != null)
                    {
                        try
                        {
                            comm.close();
                            comm.Dispose();
                        }catch { }
                        comm = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        //~Balanca()
        //{
        //    // Não altere este código. Coloque o código de limpeza no método 'Dispose(bool disposing)'
        //    Dispose(disposing: false);
        //}

        public void Dispose()
        {
            // Não altere este código. Coloque o código de limpeza no método 'Dispose(bool disposing)'
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
