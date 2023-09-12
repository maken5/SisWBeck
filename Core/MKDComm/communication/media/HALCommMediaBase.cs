using mkdInfo.communication.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mkdinfo.communication.media
{
    public abstract class HALCommMediaBase : IHALCommMedia, IDisposable
    {
        public delegate void OnDataReceived(byte data);
        public delegate void OnCommError(Exception ex);

        //private List<KeyValuePair<Object, Object>> paramns = new List<KeyValuePair<object, object>>();
        private Dictionary<object, object> paramns = new Dictionary<object, object>();

        protected Dictionary<object, object> Paramns
        {
            get { return paramns; }
            set { paramns = value; }
        }

        public OnDataReceived receive = null;
        public OnCommError onCommError = null;

        public abstract string getNameComm();
        public abstract void setParameter(KeyValuePair<Object, Object>[] parameter);
        public abstract void updateParameters();
        public abstract bool isOpen();
        public abstract Type getType();
        public abstract void open();
        public abstract void open(KeyValuePair<Object, Object>[] parameter);
        public abstract bool close();
        public abstract void send(byte[] data);
        public void send(string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                //Encoding e = System.Text.Encoding.GetEncoding(1252);
                Encoding e = System.Text.Encoding.GetEncoding("iso-8859-1");
                send(e.GetBytes(data));
            }
        }

        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool disposing);

    }
}
