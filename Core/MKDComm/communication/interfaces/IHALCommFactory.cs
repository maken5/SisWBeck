using System;
using System.Collections.Generic;
using System.Text;

namespace MKDComm.communication.interfaces
{
    public interface IHALCommFactory 
    {
        List<string> GetDevices();
    }
}
