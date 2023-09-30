using Android.Bluetooth;
using Android.Content;
using AndroidX.Core.Content;
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

            if (BluetoothAdapter.DefaultAdapter != null && BluetoothAdapter.DefaultAdapter.IsEnabled)
            {
                foreach (var pairedDevice in BluetoothAdapter.DefaultAdapter.BondedDevices)
                {
                    bluetoothDevices.Add(pairedDevice.Name);
                }

            }

            return bluetoothDevices;
        }


        public HALCommMediaBase getBluetoothConnection(string name)
        {
            return new BluetoothConnection(name);
        }

    }
}
