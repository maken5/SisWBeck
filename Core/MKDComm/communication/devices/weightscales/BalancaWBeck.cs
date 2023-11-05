using mkdinfo.communication.devices.weightscales;
using mkdinfo.communication.media;
using mkdinfo.communication.protocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using static mkdinfo.communication.protocol.ProtocolSMA1;

namespace MKDComm.communication.devices.weightscales
{
    public class BalancaWBeck : IDisposable
    {
        #region Definições----------------------------------------------------------------------------
        const int MaxCommConnectTime = 10000;
        const int MaxCommTime = 1000;

        /// <summary>
        /// Delegate para definição de função a ser chamada quando um peso e/ou status é recebido.
        /// Esta função é chamaa toda vez que um peso ou status é recebido da balança.
        /// </summary>
        /// <param name="peso">Peso em Kg.</param>
        /// <param name="status">Status da balança</param>
        public delegate void OnWeightStatusReceived(int? peso, WeightStats status);
        /// <summary>
        /// Delegate para definição de função a ser chamada quando alguma propriedade da classe é modificada.
        /// </summary>
        /// <param name="Property">Nome da propriedade modificada.</param>
        public delegate void OnPropertyChanged(String Property);
        /// <summary>
        /// Delegate para definição de função a ser chamada quando a leitura de configurações da balança é recebida.
        /// </summary>
        /// <param name="calibracoes">Memórias de calibrações da balança</param>
        /// <param name="calibarcaoIdx"Índice da memória sendo utilizada.</param>
        /// <param name="autozero">Função autozero ligado ou desligado</param>
        /// <param name="numeroSerie">Número de serie da balança</param>
        /// <param name="requireLicenseKey">Se a balança requer licença para destravar a comunicação</param>
        public delegate void OnReadConfigEnd(ProtocoloModuloPesagemSMAX.SensorCalibrationResponse[] calibracoes, int? calibarcaoIdx, bool autozero, string numeroSerie, bool requireLicenseKey);
        /// <summary>
        /// Delegate para definição de função a ser chamada na ocorrência de um erro.
        /// </summary>
        /// <param name="ex">Exceção com mensagem de erro recebida ou gerada.</param>
        public delegate void OnError(Exception ex);
        /// <summary>
        /// Delegate para definição de função a ser chamada na ocorrência de desconexão.
        /// </summary>
        public delegate void OnDisconnect();
        /// <summary>
        /// Delegate para definição de função a ser chamada na ocorrência de início de leitura ou recepção de informação.
        /// </summary>
        /// <param name="step">número do passo</param>
        /// <param name="name">Nome do passo</param>
        public delegate void OnReadingInformation(int step, string name);

        /// <summary>
        /// Tipo de status a ser reportado nas funções de callbak.
        /// Representa o status da balança.
        /// </summary>
        public enum WeightStats
        {
            /// <summary>
            /// Iniciando comunicação com a balança.
            /// </summary>
            Iniciando,
            /// <summary>
            /// Balança pesando, peso não estável.
            /// </summary>
            Pesando,
            /// <summary>
            /// Balança efetuando referência de zero.
            /// </summary>
            Zerando,
            /// <summary>
            /// Balança pesando, peso estável.
            /// </summary>
            Estavel,
            /// <summary>
            /// Balança desconectada.
            /// </summary>
            Desconectado
        };

        /// <summary>
        /// Tipo de status da biblioteca.
        /// </summary>
        public enum StatusModulo
        {
            /// <summary>
            /// Desconectado.
            /// </summary>
            IDDLE,
            /// <summary>
            /// Iniciando conexão com a balança.
            /// </summary>
            Iniciando,
            /// <summary>
            /// Lendo informações complementares da balança.
            /// </summary>
            LendoSMAAboutScroll,
            /// <summary>
            /// Lendo memória de calibração da balança.
            /// </summary>
            LendoMemoria,
            /// <summary>
            /// Lendo informações sobre a função de Autozero.
            /// </summary>
            LendoAutozero,
            /// <summary>
            /// Lendo calbiração da balança.
            /// </summary>
            LendoCalibracoes,
            /// <summary>
            /// Lendo pesagem continuamente.
            /// </summary>
            Pesando,
            /// <summary>
            /// Conexões encerradas.
            /// </summary>
            Desconecatdo
        };

        #endregion

        #region atributos internos--------------------------------------------------------------------

        //Atributos de chave -------------------------------------------------------------------------
        const ushort POLYNOMIAL = 0x00d8;
        const int WIDTH = (8 * sizeof(ushort));
        const ushort TOPBIT = (ushort)(1 << (WIDTH - 1));

        //Atributos internos -------------------------------------------------------------------------
        //Timer timer = null;

        protected HALCommMediaBase Comm = null;
        protected int tentativas_retransmissao_comando = 0;
        protected ModuloPesagemSMAX modulo;
        protected StatusModulo statusDoModulo = StatusModulo.IDDLE;
        protected bool disposedValue;
        bool inicializar = true;
        int? autoZeroValue = null;
        bool? autozeroStatus = null;
        string manufacturer;
        string model;
        string revision;
        string serialNumber;
        int? peso = 0;
        string op1;
        int? memoria = null;
        //int timerCount = 0;
        WeightStats ultimoEstado = WeightStats.Iniciando;
        DateTime lastDataUpdate = new DateTime(2000, 1, 1);
        ProtocoloModuloPesagemSMAX.SensorCalibrationResponse[] calibracoes =
                new ProtocoloModuloPesagemSMAX.SensorCalibrationResponse[] { null, null, null, null, null };
        #endregion

        #region atributos publicos--------------------------------------------------------------------

        /// <summary>
        /// Função a ser chamada quando um peso e/ou status é recebido.
        /// Esta função é chamaa toda vez que um peso ou status é recebido da balança.
        /// </summary>
        /// <param name="peso">Peso em Kg.</param>
        /// <param name="status">Status da balança</param>
        public OnWeightStatusReceived onWeightStatusReceived = null;
        /// <summary>
        /// Função a ser chamada quando a leitura de configurações da balança é recebida.
        /// </summary>
        /// <param name="calibracoes">Memórias de calibrações da balança</param>
        /// <param name="calibarcaoIdx"Índice da memória sendo utilizada.</param>
        /// <param name="autozero">Função autozero ligado ou desligado</param>
        /// <param name="numeroSerie">Número de serie da balança</param>
        /// <param name="requireLicenseKey">Se a balança requer licença para destravar a comunicação</param>
        public OnReadConfigEnd onReadConfigEnd = null;
        /// <summary>
        /// Função a ser chamada quando alguma propriedade da classe é modificada.
        /// </summary>
        /// <param name="Property">Nome da propriedade modificada.</param>
        public OnPropertyChanged onPropertyChanged = null;
        /// <summary>
        /// Função a ser chamada na ocorrência de um erro.
        /// </summary>
        /// <param name="ex">Exceção com mensagem de erro recebida ou gerada.</param>
        public OnError onErrorReceived = null;
        /// <summary>
        /// Função a ser chamada na ocorrência de uma desconexão.
        /// </summary>
        public OnDisconnect onDisconnect = null;
        /// <summary>
        /// Função a ser chamada na ocorrência de início de leitura ou recepção de informação.
        /// </summary>
        public OnReadingInformation onReadingInformation = null;

        /// <summary>
        /// Fabricante da balança
        /// </summary>
        public string Manufacturer
        {
            get { return manufacturer; }
            private set {
                if (manufacturer != value)
                {
                    manufacturer = value;
                    PropertyChanged("Manufacturer");
                }
            }
        }
        /// <summary>
        /// Modelo da balança
        /// </summary>
        public string Model
        {
            get { return model; }
            private set
            {
                if (model != value)
                {
                    model = value;
                    PropertyChanged("Model");
                }
            }
        }
        /// <summary>
        /// Revisão da balança
        /// </summary>
        public string Revision
        {
            get { return revision; }
            private set
            {
                if (revision != value)
                {
                    revision = value;
                    PropertyChanged("Revision");
                }
            }
        }
        /// <summary>
        /// Número serial da balança
        /// </summary>
        public string SerialNumber
        {
            get { return serialNumber; }
            private set
            {
                if (serialNumber != value)
                {
                    serialNumber = value;
                    PropertyChanged("SerialNumber");
                }
            }
        }
        /// <summary>
        /// Status do módulo de conexão.
        /// </summary>
        public StatusModulo StatusDoModulo
        {
            get { return statusDoModulo; }
            private set
            {
                if (statusDoModulo != value)
                {
                    statusDoModulo = value;
                    PropertyChanged("StatusDoModulo");
                }
            }
        }
        /// <summary>
        /// Calibrações da balança
        /// </summary>
        public ProtocoloModuloPesagemSMAX.SensorCalibrationResponse[] Calibracoes
        {
            get { return calibracoes; }
            set { calibracoes = value; }
        }
        /// <summary>
        /// Status de comunicação (locked/unlocked)
        /// </summary>
        public string Op1
        {
            get { return op1; }
            private set
            {
                if (op1 != value)
                {
                    op1 = value;
                    if (!String.IsNullOrEmpty(op1))
                    {
                        if (String.Compare(op1, "unlocked", true) == 0)//TODO: verificar locked e enviar comando de UNLOCK
                        {
                        }
                        else
                        {
                        }
                    }
                    PropertyChanged("Op1");
                }
            }
        }
        /// <summary>
        /// Valor do autozero.
        /// </summary>
        public int? AutoZeroValue
        {
            get { return autoZeroValue; }
            set {
                if (autoZeroValue != value)
                {
                    autoZeroValue = value;
                    PropertyChanged("AutoZeroValue");
                }
            }
        }
        /// <summary>
        /// Status de ligado/desligado da função de autozero da balança.
        /// </summary>
        public bool? AutoZeroStatus
        {
            get { return autozeroStatus; }
            protected set
            {
                if (autozeroStatus != value)
                {
                    autozeroStatus = value;
                    PropertyChanged("AutoZeroStatus");
                }
            }
        }


        public string AutoZeroStatusName
        {
            get { return autozeroStatus == null ? "Não definido" : autozeroStatus == true ? "Ligado" : "Desligado"; }
        }
        /// <summary>
        /// último valor de peso recebido.
        /// </summary>
        public int? Peso {
            get { return peso; }
            private set {
                if (peso != value)
                {
                    peso = value;
                    PropertyChanged("Peso");
                }
            }

        }
        /// <summary>
        /// Índice de memória utilizada.
        /// </summary>
        public int? Memoria
        {
            get { return memoria; }
            private set
            {
                if (memoria != value)
                {
                    memoria = value;
                    PropertyChanged("Memoria");
                }
            }
        }



        #endregion

        #region classe -------------------------------------------------------------------------------
        protected static BalancaWBeck balancaWBeck { get; set; }
        public static BalancaWBeck Get(HALCommMediaBase com, bool forceNew = false)
        {
            if (balancaWBeck != null && com != null && balancaWBeck.Comm.getNameComm() != com.getNameComm())
            {
                forceNew = true;
            }

            if ((balancaWBeck == null || forceNew) && com != null)
            {
                if (balancaWBeck != null)
                {
                    try
                    {
                        balancaWBeck.Stop();
                    }
                    catch
                    {

                    }

                    balancaWBeck = null;
                }

                balancaWBeck = new BalancaWBeck(com);
            }
            return balancaWBeck;
        }
        public BalancaWBeck(HALCommMediaBase comm)
        {
            this.Comm = comm;
            modulo = new ModuloPesagemSMAX(comm);
            modulo.onResponse += new WeightScaleBase.OnResponse(onResponse);
            modulo.onError += new WeightScaleBase.OnError(onError);
        }

        ~BalancaWBeck()
        {
            try
            {
                modulo.onResponse -= new WeightScaleBase.OnResponse(onResponse);
                modulo.onError -= new WeightScaleBase.OnError(onError);
                modulo.stop();
                modulo.Dispose();
                modulo = null;
            }
            catch { }
            Dispose(disposing: false);
        }

        #endregion

        #region callback comm balança-----------------------------------------------------------------
        private void onError(Exception ex)
        {

            if (tentativas_retransmissao_comando < 3 && this.Comm.isOpen())
            {
                tentativas_retransmissao_comando++;
                switch (StatusDoModulo)
                {
                    case StatusModulo.IDDLE:
                        StatusDoModulo = StatusModulo.Iniciando;
                        modulo.AboutScaleFirstLine();
                        break;
                    case StatusModulo.Iniciando:
                        modulo.AboutScaleFirstLine();
                        break;
                    //case StatusModulo.LendoAbout:
                    //    modulo.AboutScaleFirstLine();
                    //    break;
                    case StatusModulo.LendoSMAAboutScroll:
                        modulo.AboutScaleScroll();
                        break;
                    case StatusModulo.LendoMemoria:
                        modulo.sendExtendedCommand("GetSensorName", null);
                        break;
                    case StatusModulo.LendoAutozero:
                        modulo.sendExtendedCommand("GetAutozeroValue", null);
                        break;
                    case StatusModulo.LendoCalibracoes:
                        modulo.sendExtendedCommand("GetCalibData", null);
                        break;
                    case StatusModulo.Pesando:
                        modulo.RepeatDisplayedWeightContinuously();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Stop();
                if (onErrorReceived != null)
                    onErrorReceived(ex);
                if (onDisconnect != null)
                    onDisconnect();
            }
        }
        private void onResponse(ResponseProtocolBase rp)
        {
            if (rp != null)
            {

                ResetTimer();
                tentativas_retransmissao_comando = 0;
                switch (StatusDoModulo)
                {
                    case StatusModulo.IDDLE:
                        modulo.AboutScaleFirstLine();
                        break;
                    case StatusModulo.Iniciando:
                        if (rp.GetType() == typeof(ProtocolSMA1.SMACLevelommandResonse))
                        {
                            modulo.AboutScaleScroll();
                            StatusDoModulo = StatusModulo.LendoSMAAboutScroll;
                            SendOnReadingInformation(1, "Lendo INF");
                        }
                        else
                        {
                            modulo.AboutScaleFirstLine();
                        }
                        break;
                    case StatusModulo.LendoSMAAboutScroll:
                        if (rp.responseType == ResponseProtocolBase.ResponseType.Information)
                        {
                            if (rp.GetType() == typeof(ProtocolSMA1.AboutCommandResponse))
                            {
                                ProtocolSMA1.AboutCommandResponse a = (ProtocolSMA1.AboutCommandResponse)rp;
                                switch (a.fieldType)
                                {
                                    case ProtocolSMA1.AboutCommandResponse.FieldType.Manufacturer:
                                        Manufacturer = a.value;
                                        break;
                                    case ProtocolSMA1.AboutCommandResponse.FieldType.Model:
                                        Model = a.value;
                                        break;
                                    case ProtocolSMA1.AboutCommandResponse.FieldType.Revision:
                                        Revision = a.value;
                                        break;
                                    case ProtocolSMA1.AboutCommandResponse.FieldType.SerialNumber:
                                        SerialNumber = a.value;
                                        break;
                                    case ProtocolSMA1.AboutCommandResponse.FieldType.OP1:
                                        Op1 = a.value;
                                        break;
                                    default:
                                        break;
                                }
                                modulo.AboutScaleScroll();
                            }
                            else if (rp.GetType() == typeof(ProtocolSMA1.EndInformationCommandResponse))
                            {
                                StatusDoModulo = StatusModulo.LendoMemoria;
                                modulo.sendExtendedCommand("GetSensorName", null);
                                SendOnReadingInformation(1, "Lendo Memória");
                            }
                        }
                        else
                        {
                            StatusDoModulo = StatusModulo.Iniciando;
                            modulo.AboutScaleFirstLine();
                        }
                        break;
                    case StatusModulo.LendoMemoria:
                        if (rp.GetType() == typeof(ProtocoloModuloPesagemSMAX.SensorNameResponse))
                        {
                            memoria = ((ProtocoloModuloPesagemSMAX.SensorNameResponse)rp).id;
                            StatusDoModulo = StatusModulo.LendoAutozero;
                            modulo.sendExtendedCommand("GetAutozeroValue", null);
                        }

                        break;
                    case StatusModulo.LendoAutozero:
                        if (rp.GetType() == typeof(ProtocoloModuloPesagemSMAX.AutozeroResponse))
                        {
                            AutoZeroStatus = ((ProtocoloModuloPesagemSMAX.AutozeroResponse)rp).active;
                            AutoZeroValue = ((ProtocoloModuloPesagemSMAX.AutozeroResponse)rp).value;
                            StatusDoModulo = StatusModulo.LendoCalibracoes;
                            modulo.sendExtendedCommand("GetCalibData", null);
                            SendOnReadingInformation(1, "Lendo CAL");
                        }
                        else
                        {
                            modulo.sendExtendedCommand("GetAutozeroValue", null);
                        }
                        break;
                    case StatusModulo.LendoCalibracoes:
                        if (rp.GetType() == typeof(ProtocoloModuloPesagemSMAX.SensorCalibrationResponse))
                        {
                            int i = ((ProtocoloModuloPesagemSMAX.SensorCalibrationResponse)rp).id;
                            if (i >= 0 && i < 5)
                            {
                                calibracoes[i] = (ProtocoloModuloPesagemSMAX.SensorCalibrationResponse)rp;
                                for (i = 0; i < calibracoes.Length; i++)
                                {
                                    if (calibracoes[i] == null)
                                        return;
                                }
                                if (onReadConfigEnd != null)
                                {
                                    bool balancaTravada = String.Compare(Op1, "locked", true) == 0;
                                    if (balancaTravada && this.SerialNumber != null)
                                    {
                                        try
                                        {
                                            uint serial = Convert.ToUInt32(this.SerialNumber);
                                            int license = (int)calculateKey(serial);
                                            SetLicense(license);
                                            SendOnReadingInformation(1, "Liberando Licença");
                                        }
                                        catch { }
                                    }
                                    onReadConfigEnd(calibracoes,
                                                    memoria,
                                                    autoZeroValue == 1,
                                                    serialNumber,
                                                    balancaTravada);
                                }
                                SendOnReadingInformation(1, "Inicio Pesagem");
                                StatusDoModulo = StatusModulo.Pesando;
                                modulo.RepeatDisplayedWeightContinuously();
                            }
                        } else
                        {
                            modulo.sendExtendedCommand("GetCalibData", null);
                        }
                        break;
                    case StatusModulo.Pesando:
                        if (rp.responseType == ResponseProtocolBase.ResponseType.Weight)
                        {
                            if (rp.GetType() == typeof(mkdinfo.communication.protocol.ProtocolSMA1.StandardResponseMessage))
                            {
                                mkdinfo.communication.protocol.ProtocolSMA1.StandardResponseMessage r =
                                    (mkdinfo.communication.protocol.ProtocolSMA1.StandardResponseMessage)rp;
                                if (r.noWeightData)
                                {
                                    inicializar = true;
                                }
                                else
                                {
                                    int p = (int)r.weight;
                                    bool pesoDiferente = false;
                                    WeightStats newStats = WeightStats.Iniciando;
                                    if (r.status == ProtocolSMA1.StandardResponseMessage.ScaleStatus.CenterOfZero)
                                        newStats = WeightStats.Zerando;
                                    else if (r.motion == ProtocolSMA1.StandardResponseMessage.MotionStatus.ScaleInMotion)
                                        newStats = WeightStats.Pesando;
                                    else if (r.motion == ProtocolSMA1.StandardResponseMessage.MotionStatus.ScaleNotInMotion)
                                        newStats = WeightStats.Estavel;
                                    if (Peso != p || inicializar || newStats != ultimoEstado || DateTime.Now.Subtract(lastDataUpdate).TotalMilliseconds > 333)
                                    {
                                        lastDataUpdate = DateTime.Now;
                                        inicializar = false;
                                        Op1 = "unlocked";
                                        pesoDiferente = true;
                                        Peso = p;
                                        //if (peso > 0)
                                        //    PesoSimbolo = "kg";
                                        //else
                                        //    PesoSimbolo = "";
                                    }

                                    if (newStats != ultimoEstado || pesoDiferente)
                                    {
                                        ultimoEstado = newStats;
                                        if (onWeightStatusReceived != null)
                                            onWeightStatusReceived(Peso, ultimoEstado);
                                    }
                                }
                            }
                        }
                        else if (rp.GetType() == typeof(ProtocoloModuloPesagemSMAX.AutozeroResponse))
                        {
                            AutoZeroStatus = ((ProtocoloModuloPesagemSMAX.AutozeroResponse)rp).active;
                            AutoZeroValue = ((ProtocoloModuloPesagemSMAX.AutozeroResponse)rp).value;
                            modulo.RepeatDisplayedWeightContinuously();
                        }
                        else if (rp.GetType() == typeof(ProtocoloModuloPesagemSMAX.SensorNameResponse))
                        {
                            memoria = ((ProtocoloModuloPesagemSMAX.SensorNameResponse)rp).id;
                            modulo.RepeatDisplayedWeightContinuously();
                        } else
                        {
                            modulo.RepeatDisplayedWeightContinuously();
                        }
                        break;
                    default:
                        modulo.AboutScaleFirstLine();
                        ultimoEstado = WeightStats.Iniciando;
                        if (onWeightStatusReceived != null)
                        {
                            onWeightStatusReceived(null, this.ultimoEstado);
                        }
                        break;
                }
            }
        }

        #endregion

        #region funções API --------------------------------------------------------------------------

        /// <summary>
        /// Envia o comando de zerar para a balança.
        /// só pode ser chamado quando o StatusDoModulo for StatusModulo.Pesando
        /// </summary>
        /// <returns>True se o comando foi enviado com sucesso, false caso contrário</returns>
        public bool Zerar() {
            if (StatusDoModulo != StatusModulo.Pesando)
                return false;
            try
            {
                modulo.RequestScaleToZero();
                return true;
            }
            catch (Exception ex)
            {
                if (onErrorReceived != null)
                    onErrorReceived(ex);
            }
            return false;
        }

        /// <summary>
        /// Inicia a conexão e operação de pesagem com a balança.
        /// Só pode ser executado com StatusDoModulo em IDDLE.
        /// </summary>
        /// <returns>True se o comando foi enviado com sucesso, false caso contrário</returns>
        public bool Start() {
            if (this.StatusDoModulo == StatusModulo.IDDLE)
            {
                ResetValues();
                this.StatusDoModulo = StatusModulo.Iniciando;
                new Thread(new ThreadStart(() => {
                    try
                    {
                        modulo.start();
                        modulo.AboutScaleFirstLine();
                        //this.timer = new Timer(_ => OnTimer(), null, 1000, 1000); 
                    }
                    catch (Exception ex)
                    {
                        if (onErrorReceived != null)
                            onErrorReceived(ex);
                    }
                }
                )).Start();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Fecha as operações com a balança.
        /// </summary>
        public void Stop() {
            ReleaseTimer();
            try
            {
                modulo.stop();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
            try
            {
                Comm.close();
            }
            catch { }
            this.StatusDoModulo = StatusModulo.IDDLE;
            this.ultimoEstado = WeightStats.Desconectado;
            if (onWeightStatusReceived != null)
            {
                onWeightStatusReceived(null, ultimoEstado);
            }
        }

        /// <summary>
        /// Efetua a calibração na memória atual da balança do peso atual para ser o peso padrão, configurando o fundo de escala.
        /// Requer uma chamada a write?
        /// </summary>
        /// <param name="pesoPadrao">Peso de referência ou calibração presente na plataforma de pesagem</param>
        /// <param name="fundoEscala">Limite de pesagem</param>
        /// <returns></returns>
        public bool CalibrarPesoAtualComo(int pesoPadrao, int fundoEscala)
        {
            if (modulo.IsOpen() && pesoPadrao > 50 && fundoEscala >= pesoPadrao)
            {
                modulo.sendExtendedCommand("Calibrar", new object[] { fundoEscala, pesoPadrao, 2014 });
                Thread t = new Thread(new ThreadStart(() =>
                {
                    Thread.Sleep(7000);
                    modulo.sendExtendedCommand("Write", new object[] { 2014 });
                }));
                t.Start();
                return true;
            }
            return false;
        }
        /// <summary>
        /// Envia a calibração de uma memória. Requer uma chamada a write para salvar.
        /// </summary>
        /// <param name="idx">índice da memória [0, 4]</param>
        /// <param name="nrPontos">Número de pontos do ADC</param>
        /// <param name="pesoPadrao">Peso padrão que o nr de pontos representa</param>
        /// <param name="fundoEscala">Fundo de escala da balançca</param>
        /// <param name="sensorName">Nome do sensor</param>
        /// <returns></returns>
        public bool SetCalibracaoDeMemoria(int idx, int nrPontos, int pesoPadrao, int fundoEscala, string sensorName)
        {
            if (modulo.IsOpen() && idx >= 0 && idx < 5 && pesoPadrao > 50 && fundoEscala > pesoPadrao && !String.IsNullOrWhiteSpace(sensorName))
            {
                modulo.sendExtendedCommand("SetCalibData",
                    new object[] { idx, nrPontos, pesoPadrao, fundoEscala, sensorName, 2014 });

                return true;
            }
            return false;
        }

        /// <summary>
        /// Escreve o valor de calibração da memória
        /// </summary>
        /// <returns></returns>
        public bool WriteMemory()
        {
            if (modulo.IsOpen())
            {
                modulo.sendExtendedCommand("Write", new object[] { 2014 });
                return true;
            }
            return false;
        }

        /// <summary>
        /// Configura o nome do sensor atual
        /// Requer uma chamada a write?
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool SetSensorName(string name)
        {
            if (modulo.IsOpen() && !String.IsNullOrWhiteSpace(name))
            {
                modulo.sendExtendedCommand("SetSensorName", new object[] { name.Trim(), 2014 });
                this.WriteMemory();
                return true;
            }
            return false;
        }

        /// <summary>
        /// configura a licença para liberar a comunicação.
        /// Requer uma chamada a write?
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool SetLicense(int key)
        {
            if (modulo.IsOpen() && key >= 0)
            {
                modulo.sendExtendedCommand("SetLicenseKey", new object[] { key, 2014 });
                this.WriteMemory();
                return true;
            }
            return false;
        }


        /// <summary>
        /// Envia o comando para ligar ou desligar a função de autozero da balança.
        /// só pode ser chamado quando o StatusDoModulo for StatusModulo.Pesando
        /// </summary>
        /// <param name="ligar">Valor true para ligar, false para desligar</param>
        /// <returns>True se o comando foi enviado com sucesso, caso contrário, false</returns>
        public bool SetAutozero(bool ligar) {
            if (StatusDoModulo == StatusModulo.Pesando)
                try
                {
                    modulo.sendExtendedCommand(ligar ? "AutozeroOn" : "AutozeroOff");
                    return true;
                } catch (Exception ex)
                {
                    if (onErrorReceived != null)
                        onErrorReceived(ex);
                }
            return false;
        }

        /// <summary>
        /// Envia o comando para alterar o valor da função de autozero da balança.
        /// só pode ser chamado quando o StatusDoModulo for StatusModulo.Pesando
        /// </summary>
        /// <param name="valorAutozero">Valor do autozero, no intervalo (0, 10) (intervalo aberto) </param>
        /// <returns>True se o comando foi enviado com sucesso, caso contrário, false</returns>
        public bool SetAutozero(int valorAutozero)
        {
            if (StatusDoModulo == StatusModulo.Pesando && valorAutozero > 0 && valorAutozero < 10)
                try
                {
                    modulo.sendExtendedCommand("SetAutozero", new object[] { valorAutozero });
                    return true;
                }
                catch (Exception ex)
                {
                    if (onErrorReceived != null)
                        onErrorReceived(ex);
                }
            return false;
        }

        /// <summary>
        /// Envia o comando de troca de memória de calibração da balança.
        /// só pode ser chamado quando o StatusDoModulo for StatusModulo.Pesando
        /// Valor permitido de memória é [0, 4]
        /// </summary>
        /// <param name="memoria">código da memória,valor de 0 a 4</param>
        /// <returns>True se o comando foi enviado com sucesso, false caso contrário</returns>
        public bool SetMemoria(int memoria)
        {
            if (StatusDoModulo != StatusModulo.Pesando)
                return false;
            if (memoria >= 0 && memoria < 5)
            {
                try
                {
                    modulo.sendExtendedCommand("SetMemory", new object[] { memoria, 2014 });
                    return true;
                }
                catch (Exception ex)
                {
                    if (onErrorReceived != null)
                        onErrorReceived(ex);
                }
                return false;
            }
            else
            {
                if (onErrorReceived != null)
                    onErrorReceived(new Exception("Valor de índice de memória deve estar no intervalo [0, 4]"));
                return false;
            }
        }

        public bool SendCommandReadMemoria()
        {
            if (StatusDoModulo == StatusModulo.Pesando )
                try
                {
                    modulo.sendExtendedCommand("GetCalibData");//GetSensorName
                    return true;
                }
                catch (Exception ex)
                {
                    if (onErrorReceived != null)
                        onErrorReceived(ex);
                }
            return false;
        }

        #endregion

        #region Métodos internos----------------------------------------------------------------------

        //void OnTimer()
        //{
        //    if(timerCount < 30){
        //        timerCount++;
        //    }
        //    else
        //    {
                
        //        this.Stop();
        //    }
        //}

        void ResetTimer()
        {
            //timerCount = 0;
        }
        void ReleaseTimer()
        {
            //if (timer != null)
            //{
            //    try
            //    {
            //        timer.Change(Timeout.Infinite, Timeout.Infinite);
            //        timer.Dispose();
            //    }
            //    catch { }
            //    timer = null;
            //}
        }
        void ResetValues()
        {
            
            ResetTimer();
            ReleaseTimer();
            tentativas_retransmissao_comando = 0;
            Manufacturer = null;
            Model = null;
            Revision = null;
            SerialNumber = null;
            StatusDoModulo = StatusModulo.IDDLE;
            Op1 = null;
            AutoZeroStatus = null;
            AutoZeroValue = null;
            Peso = null;
            calibracoes =
                new ProtocoloModuloPesagemSMAX.SensorCalibrationResponse[] { null, null, null, null, null };
        }
        void PropertyChanged(String propertyName)
        {
            if (onPropertyChanged != null)
                onPropertyChanged(propertyName);
        }

        void SendOnReadingInformation(int passo, string nome)
        {
            if (onReadingInformation != null)
            {
                onReadingInformation(passo, nome);
            }   
        }

        //Métodos de cálculo de chave  ---------------------------------------------------------------
        ushort crcSlow(uint valor)
        {
            ushort remainder = 0;
            int bytev;
            byte bit;
            byte msg;
            /*
                * Perform modulo-2 division, a byte at a time.
                */
            for (bytev = 0; bytev < sizeof(uint); bytev++)
            {
                msg = (byte)(valor >> (8 * bytev));
                /*
                    * Bring the next byte into the remainder.
                    */
                remainder ^= (ushort)(msg << ((8 * sizeof(ushort)) - 8));

                /*
                    * Perform modulo-2 division, a bit at a time.
                    */
                for (bit = 8; bit > 0; --bit)
                {
                    /*
                        * Try to divide the current data bit.
                        */
                    if ((remainder & TOPBIT) != 0)
                    {
                        remainder = (ushort)((remainder << 1) ^ POLYNOMIAL);
                    }
                    else
                    {
                        remainder = (ushort)(remainder << 1);
                    }
                }
            }

            /*
                * The final remainder is the CRC result.
                */
            return (remainder);

        }   /* crcSlow() */

        ushort calculateKey(uint numSerie)
        {
            return crcSlow(numSerie * 7 / 3 + 54323);
        }

        #endregion 

        #region IDisposable---------------------------------------------------------------------------
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        modulo.stop();
                    }
                    catch { }
                    modulo = null;
                    try
                    {
                        Comm.close();
                    }
                    catch { }
                }

                // Tarefa pendente: liberar recursos não gerenciados (objetos não gerenciados) e substituir o finalizador
                // Tarefa pendente: definir campos grandes como nulos
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
