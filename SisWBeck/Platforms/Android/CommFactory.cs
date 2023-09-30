using Android.App.AppSearch;
using Android.Bluetooth;
using MKDComm.communication.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisWBeck.Platforms
{
    public class CommFactory : IHALCommFactory
    {
        [Obsolete]
        public List<string> GetDevices()
        {
            
            List<string> devices = new List<string>();
            if (BluetoothAdapter.DefaultAdapter != null && BluetoothAdapter.DefaultAdapter.IsEnabled)
            {
                foreach (var pairedDevice in BluetoothAdapter.DefaultAdapter.BondedDevices)
                {
                    Console.WriteLine($"Found device with name: {pairedDevice.Name} and MAC address: {pairedDevice.Address}");
                    if (("" + pairedDevice.Name).ToUpper().StartsWith("WBECK"))
                    {
                        devices.Add(pairedDevice.Name);
                    }
                }
            }
            return devices;
        }

        private async Task Permissao()
        {
            //PermissionStatus status = await Permissions.CheckStatusAsync<Android.Manifest.Permission.BluetoothConnect>();
            var x = Android.Manifest.Permission.BluetoothConnect;
        }

    }
}
