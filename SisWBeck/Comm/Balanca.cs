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
    public class Balanca : INotifyPropertyChanged, IDisposable
    {
        private bool disposedValue;
        private BluetoothHelper bluetoothHelper;
        private Config config;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null && !String.IsNullOrWhiteSpace(propertyName))
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                });
        }

        public Balanca(BluetoothHelper bluetoothHelper, Config config)
        {
            this.bluetoothHelper = bluetoothHelper; 
            this.config = config;
        }
        public string Nome { get; }
        public int Peso{get;}
        public string PesoStr { get; }
        public bool Estavel { get; }
        public void Zerar()
        {

        }
        public void Reconectar()
        {

        }
        public void Stop()
        {

        }

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
            //Device.BeginInvokeOnMainThread(new Action(() =>
            //{
            //    if (canSend)
            //    {
            //        canSend = false;

            //        try
            //        {

            //            TextAlertVisibility = false;
            //            TextAlert = "";
            //            WeightVisibility = true;
            //            EnabledConnectingFlag = true;

            //            Weight = peso.ToString();
            //            WeightStats = weightStats;
            //            SetAutozeroColor();


            //            if (WeightStats == WeightStats.Desconectado)
            //            {
            //                EnabledConnectingFlag = true;
            //                WeightVisibility = false;
            //                Weight = "";
            //                TextAlert = "Desconectado";
            //                TextAlertVisibility = true;
            //                WeightStats = WeightStats.Desconectado;
            //                WeightStatus = "Reconectar";
            //                WeightTextColor = "White";
            //                WeightBackgroundColor = "Red";
            //                SaveButtonColor = "Red";
            //                KgVisibility = false;
            //                SetAutozeroColor();
            //                Saved = false;
            //                CodeColor = Color.Transparent;
            //                CodeTextColor = Color.Black;
            //            }
            //            else if (WeightStats == WeightStats.Estavel)
            //            {
            //                string codeText = View.getEntryText();
            //                if (NewOx == true && Saved == true)
            //                {
            //                    CodeColor = Color.LimeGreen;
            //                    CodeTextColor = Color.White;
            //                }
            //                else
            //                {
            //                    CodeColor = Color.Transparent;
            //                    CodeTextColor = Color.Black;
            //                }

            //                WeightStatus = "Gravar";
            //                SaveButtonColor = "LimeGreen";
            //                WeightBackgroundColor = "LimeGreen";
            //                WeightTextColor = "White";
            //                KgVisibility = true;
            //            }
            //            else if (WeightStats == WeightStats.Iniciando)
            //            {
            //                WeightStatus = WeightStats.ToString();
            //                SaveButtonColor = "LightGray";
            //                WeightBackgroundColor = "Transparent";
            //                WeightTextColor = "Black";
            //                KgVisibility = false;
            //            }
            //            else if (WeightStats == WeightStats.Pesando)
            //            {

            //                Saved = false;
            //                CodeColor = Color.Transparent;
            //                CodeTextColor = Color.Black;

            //                //string codeText = View.getEntryText();
            //                //if (Weight != null && codeText != null && Items != null &&
            //                //    Items.Select(s => s.Code).Contains(codeText) && Convert.ToInt32(Weight) < 30)
            //                if (Weight != null && NewOx == true && Convert.ToInt32(Weight) < 30)
            //                {
            //                    View.setEntryText();
            //                    NewOx = false;
            //                }

            //                WeightStatus = WeightStats.ToString();
            //                SaveButtonColor = "LightGray";
            //                WeightBackgroundColor = "Transparent";
            //                WeightTextColor = "Black";
            //                KgVisibility = false;
            //            }
            //            else if (WeightStats == WeightStats.Zerando)
            //            {
            //                WeightStatus = WeightStats.ToString();
            //                WeightBackgroundColor = "Transparent";
            //                SaveButtonColor = "LightGray";
            //                WeightTextColor = "Black";
            //                KgVisibility = false;
            //            }
            //        }
            //        catch
            //        {

            //        }
            //        finally
            //        {
            //            canSend = true;
            //        }
            //    }
            //}));


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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Balanca()
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
