using mkdinfo.communication.devices.weightscales;
using mkdinfo.communication.media;
using mkdinfo.communication.protocol;
using mkdInfo.communication.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Communication.src.communication.devices.readers
{
    public abstract class ReaderBase
    {
        #region declarations
        public delegate void OnResponse(ResponseProtocolBase response);
        public delegate void OnError(Exception ex);
        public OnResponse onResponse = null;
        public OnError onError = null;

        #endregion

        #region protected
        protected int resolution = 1;
        protected IHALCommProtocol prot = null;
        protected HALCommMediaBase med = null;
        protected float ultimoPesoRecebido = Int32.MinValue;

        #endregion

        #region public attributes
        public IHALCommProtocol protocol
        {
            get { return prot; }
            set
            {
                //if (!WeightScaleBase.isLibraryLoaded())
                //    return;
                if (prot != null)
                {
                    try
                    {
                        prot.onNewResponse -= new mkdInfo.communication.interfaces.OnNewResponse(onNewResponse);
                        prot.onError -= new mkdInfo.communication.interfaces.OnError(onErrorReceived);
                    }
                    catch { }
                    prot = null;
                }
                prot = value;
                if (prot != null)
                {
                    prot.onError += new mkdInfo.communication.interfaces.OnError(onErrorReceived);
                    prot.onNewResponse += new mkdInfo.communication.interfaces.OnNewResponse(onNewResponse);
                    if (med != null)
                        prot.media = med;
                }
            }
        }

        public HALCommMediaBase media
        {
            get { return med; }
            set
            {
                med = value;
                if (prot != null)
                {
                    prot.media = value;

                }
            }
        }

        protected virtual bool onNewResponseProcess(ResponseProtocolBase rp)
        {
            return true;
        }

        protected void onNewResponse(ResponseProtocolBase rp)
        {

            if (onResponse != null && rp != null)
            {
                if (onNewResponseProcess(rp))
                    onResponse(rp);
            }
        }
        protected void onErrorReceived(Exception ex)
        {
            if (onError != null)
                onError(ex);
        }

        #endregion

        public ReaderBase(HALCommMediaBase communicationMedia)
        {
            //if (!WeightScaleBase.isLibraryLoaded())
            //    throw new Exception();
            this.media = communicationMedia;
        }

        public ReaderBase()
        {
            //if (!WeightScaleBase.isLibraryLoaded())
            //    throw new Exception();
        }

        public void start()
        {
            if (protocol != null) protocol.start();
        }

        public void stop()
        {
            if (protocol != null) protocol.stop();
        }
    }
}
