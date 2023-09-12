using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mkdinfo.communication.protocol
{
    public abstract class ResponseProtocolBase
    {
        public enum ResponseType
        {
            Weight,
            Information,
            Error,
            Text,
            Unknow
        };

        public abstract ResponseType responseType {get;}

    }
}
