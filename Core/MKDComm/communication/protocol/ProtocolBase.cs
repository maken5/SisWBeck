using mkdinfo.communication.devices.weightscales;
using mkdinfo.communication.media;
using mkdInfo.communication.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace mkdinfo.communication.protocol
{
    public abstract class ProtocolBase : IHALCommProtocol
    {   
        #region atributos protegidos
        protected HALCommMediaBase comm = null;
        protected int _timeOut = 3000;
        protected int _startTimeOut = 6000;
        protected bool _firstTime = true;
        protected Thread timer = null;
        protected bool _runTimer = false;
        protected DateTime _resetTimer = DateTime.Now;
        //protected uint elapsedTime = 0;
        protected readonly object sem = new object();
        #endregion

        #region atributos publicos
        //public delegate void OnNewResponse(ResponseProtocolBase response);
        //public delegate void OnError(Exception ex);

        public int timeOut { get; set; }
        public int startTimeOut { get; set; }

        public OnNewResponse onNewResponseField = null;
        public OnError onErrorField = null;
        public OnNewResponse onNewResponse
        {
            get
            {
                return onNewResponseField;
            }
            set
            {
                onNewResponseField += value;
            }
        }
        public OnError onError
        {
            get
            {
                return onErrorField;
            }
            set
            {
                onErrorField += value;
            }
        }
        public IHALCommMedia media
        {
            get { return comm; }
            set
            {
                if (comm != null)
                {
                    try
                    {
                        comm.receive -= new HALCommMediaBase.OnDataReceived(receive);
                        comm.onCommError -= new HALCommMediaBase.OnCommError(onCommError);
                        comm.close();
                    }
                    catch (Exception ex)
                    {
                        if (onError != null)
                            onError(ex);
                    }
                }
                comm = (HALCommMediaBase) value;
                if (comm != null)
                {
                    comm.receive += new HALCommMediaBase.OnDataReceived(receive);
                    comm.onCommError += new HALCommMediaBase.OnCommError(onCommError);
                }
            }
        }

        
        #endregion

        #region class
        public ProtocolBase()
        {
            //if (!WeightScaleBase.isLibraryLoaded())
            //    throw new Exception();
        }

        public ProtocolBase(HALCommMediaBase mediaBase)
        {
            media = mediaBase;
        }

        #endregion

        #region protected

        protected void resetTimerCounter()
        {
            lock (sem)
            {
                _resetTimer = DateTime.Now;
                //elapsedTime = 0;
                _firstTime = false;
            }
        }

        protected void startTimer()
        {
            
            if (_timeOut > 0)
            {
                if (timer != null && timer.IsAlive)
                {
                    timer.Abort();
                }
                _runTimer = true;
                _firstTime = true;
                _resetTimer = DateTime.Now;
                //elapsedTime = 0;
                timer = new Thread(new ThreadStart(timerWorkThread));
                timer.Start();
            }
        }

        protected virtual void handleTimeout()
        {
            //_runTimer = false;
            if (onError != null)
            {
                onError(new Exception("Communication time out"));
            }
            resetTimerCounter();
        }

        protected void timerWorkThread()
        {
            bool call = false;
            while (_runTimer)
            {
                Thread.Sleep(10);
                lock (sem)
                {
                    //elapsedTime += 10;
                    if (_firstTime)
                    {
                        //if (elapsedTime > _timeOut + _startTimeOut)
                        if (DateTime.Now.Subtract(_resetTimer).TotalMilliseconds > _timeOut + _startTimeOut)
                        {
                            call = true;

                        }
                    }
                    //else if (elapsedTime > _timeOut)
                    else if (DateTime.Now.Subtract(_resetTimer).TotalMilliseconds > _timeOut)
                    {
                        call = true;
                    }
                }
                if (call)
                {
                    handleTimeout();
                    call = false;
                }
            }

        }

        protected void stopTimer()
        {
            _runTimer = false;
            Thread.Sleep(10);
            //if (timer != null && timer.IsAlive)
            //    timer.Abort();
            timer = null;
            //elapsedTime = 0;
            _resetTimer = DateTime.Now;
        }


        protected abstract void receive(byte data);
        protected virtual void onCommError(Exception ex)
        {
            if (this.onError != null)
            {
                this.onError(new Exception("Erro no link de comunicação", ex));
            }
        }

        #endregion

        #region interface
        public virtual void send(byte[] sendData)
        {
            if (comm != null && comm.isOpen())
                comm.send(sendData);
        }

        public virtual void start()
        {
            if (comm != null)
            {
                if (!comm.isOpen())
                    comm.open();
                startTimer();
            }
        }

        public virtual void stop()
        
        {
            stopTimer();
            if (comm != null)
            {
                try
                {
                    comm.close();
                }
                catch { }

            }
        }
        #endregion
        
    }
}
