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
        private bool disposedValue;
        private BluetoothHelper bluetoothHelper;
        private Config config;
        private HALCommMediaBase comm = null;

        private int _peso;
        private bool _estavel;
        private WeightStats _status = WeightStats.Desconectado;
        private object _lock = new object();
        private BalancaWBeck wbeck = null;
        private Color CorLaranja = Color.FromRgb(255, 165, 0);
        private Color CorVermelho = Color.FromRgb(255, 0, 0);
        private Color CorVerde = Color.FromRgb(0, 128, 0);
        private Color CorPreto = Color.FromRgb(0, 0, 0);

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

        public string Nome => this.config.Balanca;
        public int Peso
        {
            get => Get(ref _peso);
            private set
            {
                if (Set(ref _peso, value))
                {
                    OnPropertyChanged(nameof(PesoStr));
                    OnPropertyChanged(nameof(PesoPositivo));
                    OnPropertyChanged(nameof(PesoCor));
                }

            }
        }
        public string PesoStr
        {
            get
            {
                switch (Status)
                {
                    case WeightStats.Iniciando:
                        return "Conectando";
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
        public Color PesoCor
        {
            get
            {
                switch (Status)
                {
                    case WeightStats.Iniciando:
                        return CorLaranja;
                    case WeightStats.Pesando:
                        return Peso < 0 ? CorVermelho : CorPreto;
                    case WeightStats.Zerando:
                        return CorPreto;
                    case WeightStats.Estavel:
                        return CorVerde;
                    case WeightStats.Desconectado:
                    default:
                        return CorVermelho;
                }
            }
        }
        public bool Estavel
        {
            get => Get(ref _estavel);
            private set
            {
                Set(ref _estavel, value);
                OnPropertyChanged(nameof(PesoStr));
            }
        }
        public WeightStats Status
        {
            get => Get(ref _status);
            set
            {
                if (Set(ref _status, value))
                {
                    OnPropertyChanged(nameof(IsContectado));
                    OnPropertyChanged(nameof(Estavel));
                    OnPropertyChanged(nameof(PesoStr));
                    OnPropertyChanged(nameof(IsContectado));
                    OnPropertyChanged(nameof(PesoCor));
                }
            }
        }
        public bool PesoPositivo => Peso>0 && (Status == WeightStats.Pesando || Status == WeightStats.Estavel);
        public bool IsContectado => Status != WeightStats.Desconectado && Status != WeightStats.Iniciando;

        #endregion

        #region Métodos públicos ---------------------------------------------------------
        public void Zerar()
        {

        }
        public void Reconectar()
        {
            if (wbeck != null)
                try
                {
                    wbeck.Start();
                } catch (Exception ex)
                {

                }
        }

        public void Start()
        {
            if (wbeck != null)
            {
                wbeck.Start();
            }
        }
        public void Stop()
        {
            if (wbeck != null)
            {
                wbeck.Stop();
            }
        }

        #endregion

        #region Métodos callback ---------------------------------------------------------

        public void OnErrorReceive(Exception ex)
        {
            //try
            //{
            //    //Balanca.onWeightStatusReceived -= new BalancaWBeck.OnWeightStatusReceived(OnNewWeight);
            //    //Balanca.onErrorReceived -= new BalancaWBeck.OnError(OnErrorReceive);
            //    this.StopCommunication();
            //    EnabledConnectingFlag = true;
            //    WeightVisibility = false;
            //    TextAlert = ex.Message;
            //    TextAlertVisibility = true;
            //    Weight = "";
            //    WeightStats = WeightStats.Desconectado;
            //    WeightStatus = "Reconectar";
            //    WeightTextColor = "White";
            //    WeightBackgroundColor = "Red";
            //    SaveButtonColor = "Red";
            //    KgVisibility = false;
            //    //WeightStats = WeightStats.Desconectado;
            //    //WeightStatus = "Reconectar";
            //    //SaveButtonColor = "Red";
            //    //KgVisibility = false;
            //    SetAutozeroColor();
            //    Saved = false;
            //    CodeColor = Color.Transparent;
            //    CodeTextColor = Color.Black;
            //}
            //catch
            //{

            //}
        }
        protected virtual void OnNewWeight(int? peso, WeightStats weightStats)
        {
            if (peso != null)
            {
                Peso = peso.Value;
                Status = weightStats;
            }
        }

        protected virtual void OnDisconnect()
        {
            //try
            //{
            //    //Balanca.onWeightStatusReceived -= new BalancaWBeck.OnWeightStatusReceived(OnNewWeight);
            //    //Balanca.onErrorReceived -= new BalancaWBeck.OnError(OnErrorReceive);
            //    EnabledConnectingFlag = true;
            //    WeightVisibility = false;
            //    Weight = "";
            //    TextAlert = "Desconectado";
            //    TextAlertVisibility = true;
            //    WeightStats = WeightStats.Desconectado;
            //    WeightStatus = "Reconectar";
            //    WeightTextColor = "White";
            //    WeightBackgroundColor = "Red";
            //    SaveButtonColor = "Red";
            //    KgVisibility = false;
            //    SetAutozeroColor();
            //    Saved = false;
            //    CodeColor = Color.Transparent;
            //    CodeTextColor = Color.Black;
            //}
            //catch
            //{

            //}
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
