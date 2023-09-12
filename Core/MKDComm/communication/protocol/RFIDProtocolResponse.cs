using mkdinfo.communication.protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.src.communication.protocol
{
    public class RFIDProtocolResponse : ResponseProtocolBase
    {
        string tag = "";

        public string Tag
        {
          get { return tag; }
          set { tag = value; }
        }

        public override ResponseType responseType { get { return ResponseType.Text; } }

        public RFIDProtocolResponse(string tag)
        {
            this.tag = tag;
        }

        public RFIDProtocolResponse() { }

        
    }
}
