using mkdinfo.communication.media;
using mkdinfo.communication.protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace mkdinfo.communication.devices.weightscales
{
    public class BalancaSMA
    {
        const int MaxConnectionTime = 20000;
        const int MaxResponseTime = 1000;
        const int ResponseTimeInterval = 100;
        const int MaxReconnections = 5;

        public enum Status{
            Conectando,
            ErroConnectando,
            Pesando,
            Zerando,
            PesoEstavel,
            PerdaConexao,
            Reconectando,
            RespostaDesconhecida
        };

        public class StatusEvent{
            public int value = 0;
            public Status status;

            public StatusEvent() { }
            public StatusEvent(int peso, Status status)
            {
                this.value = peso;
                this.status = status;
            }
        };

        public delegate void OnNewStatus(StatusEvent e);

        public OnNewStatus onNewStatus = null;


        protected enum ConnectionStatus
        {
            Conectando,
            Conectado,
            Desconectado
        };
        protected ConnectionStatus status = ConnectionStatus.Desconectado;
        protected int reconnections = 0;
        protected WeightScaleSMA weightScale = new WeightScaleSMA();
        protected HALCommMediaBase comm;
        TTimer watchDogTimer = new TTimer();
        TTimer requestTimer = new TTimer();

        protected void watchDogTimeOut()
        {
            switch (status)
            {
                case ConnectionStatus.Conectando:
                    parar();
                    sendMessage(Status.ErroConnectando);
                    break;
                case ConnectionStatus.Conectado:
                    if (reconnections < MaxReconnections)
                    {
                        stop();
                        reconnections++;
                        sendMessage(Status.Reconectando, reconnections);
                        start();
                    }
                    else
                    {
                        parar();
                        sendMessage(Status.PerdaConexao);
                        status = ConnectionStatus.Desconectado;
                    }
                    
                    
                    break;
                case ConnectionStatus.Desconectado:
                    requestTimer.stop();
                    watchDogTimer.stop();
                    break;
                default:
                    break;
            }
        }

        protected void requestTimerWork()
        {
            if (weightScale!=null)
                try
                {
                    weightScale.RequestDisplayWeight();
                }
                catch { }
        }

        protected void onError(Exception ex)
        {

        }

        protected void sendMessage(Status status, int peso = 0)
        {
            if (onNewStatus != null)
                onNewStatus(new StatusEvent(peso, status));
        }

        protected void onReport(ResponseProtocolBase resp)
        {
            watchDogTimer.reset();
            requestTimer.reset();
            reconnections = 0;
            if (status == ConnectionStatus.Conectando)
            {
                watchDogTimer.start(MaxResponseTime);
                requestTimer.start(ResponseTimeInterval);
                status = ConnectionStatus.Conectado;
            }
            weightScale.RequestDisplayWeight();
            if (resp.responseType == ResponseProtocolBase.ResponseType.Weight)
            {
                Status st = Status.Pesando;
                ProtocolSMA1.StandardResponseMessage sm = (ProtocolSMA1.StandardResponseMessage)resp;
                if (sm.status == ProtocolSMA1.StandardResponseMessage.ScaleStatus.CenterOfZero)
                    st = Status.Zerando;
                else if (sm.motion == ProtocolSMA1.StandardResponseMessage.MotionStatus.ScaleNotInMotion)
                    st = Status.PesoEstavel;
                sendMessage(st, (int)sm.weight);
            }
            else 
            {
                sendMessage(Status.RespostaDesconhecida);
            }
        }

        protected bool start()
        {
            bool r = false;
            try
            {
                r = weightScale.start();
                Thread.Sleep(100);
                weightScale.RequestDisplayWeight();
            }
            catch { }
            watchDogTimer.start(MaxConnectionTime);
            requestTimer.start(MaxResponseTime);
            return r;
        }

        protected void stop()
        {
            requestTimer.stop();
            watchDogTimer.stop();
            try
            {
                weightScale.stop();
                comm.close();
            }
            catch { }
        }

        public BalancaSMA(HALCommMediaBase commMedia)
        {
            this.comm = commMedia;
            
            weightScale.media = this.comm;
            weightScale.protocol = new ProtocolSMA1();
            weightScale.onError += new WeightScaleBase.OnError(onError);
            weightScale.onResponse += new WeightScaleBase.OnResponse(onReport);
            watchDogTimer.onTime += new TTimer.OnTime(watchDogTimeOut);
            requestTimer.onTime += new TTimer.OnTime(requestTimerWork);
        }

        public bool zerar()
        {
            try
            {
                return weightScale.RequestScaleToZero();
            }
            catch { }
            return false;
        }

        public bool iniciar()
        {
            bool r = false;
            parar();
            status = ConnectionStatus.Conectando;
            reconnections = 0;
            start();
            return r;
        }

        public void parar()
        {
            status = ConnectionStatus.Desconectado;
            stop();
        }

    }
}
