
using mkdinfo.communication.media;
using SisWBeck.Comm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisWBeck.Comm
{
    public partial class BluetoothHelper
    {
        ObservableCollection<string> bluetoothDevices = new ObservableCollection<string>();

        public ObservableCollection<string> GetDevicesNames()
        {

            return new ObservableCollection<string>();
        }


        public HALCommMediaBase getBluetoothConnection(string name)
        {
            return null;
        }

    }
}
