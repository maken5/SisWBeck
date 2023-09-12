using mkdinfo.communication.devices.weightscales;
using mkdinfo.communication.media;
using mkdinfo.communication.protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mkdinfo.communication.protocol
{
    public abstract class TextLineBaseProtocol : ProtocolBase
    {
        #region private attributes
        private StringBuilder sb = new StringBuilder();
        private int maxLen = 16;

        public int MaxLen
        {
            get { return maxLen; }
            set { if (value>0) maxLen = value; }
        }
        #endregion


        #region class
        public TextLineBaseProtocol(int maxLen = 16) : base()
        {
            //if (!WeightScaleBase.isLibraryLoaded())
            //    throw new Exception();
            this.maxLen = maxLen;
        }

        public TextLineBaseProtocol(HALCommMediaBase commMedia) : base(commMedia)
        {
            //if (!WeightScaleBase.isLibraryLoaded())
            //    throw new Exception();
           
        }
        #endregion

        #region interface

        protected override void receive(byte data)
        {
            if (onNewResponse != null)
            {
                char chr = Convert.ToChar(data);
                switch (chr)
                {
                    case '\n':
                        string line = sb.ToString();
                        sb.Clear();
                        onNewLine(line);
                        break;
                    case '\r':
                        break;
                    default:
                        if (sb.Length < maxLen)
                            sb.Append(chr);
                        break;
                }
            }
        }

        protected abstract void onNewLine(String line);

        #endregion
    }
}
