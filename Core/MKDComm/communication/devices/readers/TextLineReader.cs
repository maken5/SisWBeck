using mkdinfo.communication.media;
using mkdinfo.communication.protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Communication.src.communication.protocol;

namespace Communication.src.communication.devices.readers
{
    public class TextLineReader : ReaderBase
    {


        public TextLineReader(HALCommMediaBase communicationMedia)
            : base(communicationMedia)
        {
            
            protocol = new TextLineProtocol();
        }
    }
}
