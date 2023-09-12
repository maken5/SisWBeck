using mkdinfo.communication.protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Communication.src.communication.protocol
{
    public class TextResponseProtocol : ResponseProtocolBase
    {
        string txt = "";

        public string Txt
        {
            get { return txt; }
            set { txt = value; }
        }

        public override ResponseType responseType { get { return ResponseType.Text; } }

        public TextResponseProtocol(string txt = "")
        {
            this.txt = txt;
        }
    }
}
