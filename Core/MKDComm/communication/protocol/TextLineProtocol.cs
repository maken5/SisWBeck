using mkdinfo.communication.protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Communication.src.communication.protocol
{
    public class TextLineProtocol : TextLineBaseProtocol
    {
        protected override void onNewLine(String line)
        {
            if (onNewResponse != null)
            {
                onNewResponse(new TextResponseProtocol(line));
            }
        }

    }
}
