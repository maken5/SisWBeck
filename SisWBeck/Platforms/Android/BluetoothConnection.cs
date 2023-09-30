using Android.Bluetooth;
using Java.IO;
using Java.Util;
using mkdinfo.communication.media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisWBeck.Comm
{
    public class BluetoothConnection : HALCommMediaBase, IDisposable
    {
        InputStreamReader inputStramReader = null;
        Thread receiveThread = null;
        bool runReceiveThread = false;
        object semaphore = new object();
        private BluetoothSocket btClienttSocket = null;
        string bluetoothName;
        UUID uuid = UUID.FromString("00001101-0000-1000-8000-00805f9b34fb");


        private BluetoothSocket BtClientSocket
        {
            get
            {
                if (btClienttSocket == null)
                {
                    btClienttSocket = CreateSocket(bluetoothName);
                }
                return btClienttSocket;
            }
        }


        public BluetoothConnection(string blutoothName)
        {
            this.bluetoothName = blutoothName;
            //BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            //var d = adapter.BondedDevices.Where(w => w.Name == blutoothName).FirstOrDefault();

            //if (d == null)
            //{
            //    throw new Exception(String.Format("Dispositivo bluetooth {0} não encontrado", blutoothName));
            //}

            ////btClientSocket = d.CreateInsecureRfcommSocketToServiceRecord(uuid);
            //btClientSocket = d.CreateRfcommSocketToServiceRecord(uuid);
        }

        private BluetoothSocket CreateSocket(string blutoothName)
        {
            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            var d = adapter.BondedDevices.Where(w => w.Name == this.bluetoothName).FirstOrDefault();

            if (d == null)
            {
                throw new Exception(String.Format("Dispositivo bluetooth {0} não encontrado", blutoothName));
            }

            //btClientSocket = d.CreateInsecureRfcommSocketToServiceRecord(uuid);
            //BtClientSocket = d.CreateRfcommSocketToServiceRecord(uuid);
            return d.CreateRfcommSocketToServiceRecord(uuid);
        }
        public override bool close()
        {
            runReceiveThread = false;
            try
            {
                inputStramReader.Close();
            }
            catch
            {

            }

            

            try
            {
                BtClientSocket.Close();
                return true;
            }
            catch
            {
                return false;
            }

            finally
            {
                btClienttSocket = null;
                inputStramReader = null;
                receiveThread = null;
            }

        }

        public override string getNameComm()
        {
            return bluetoothName;
        }

        public override Type getType()
        {
            return BtClientSocket == null ? this.GetType() : BtClientSocket.GetType();
        }

        public override bool isOpen()
        {
            return BtClientSocket != null && BtClientSocket.IsConnected;
        }

        public override void open()
        {


            if (!this.isOpen() && !runReceiveThread)
            {
                runReceiveThread = true;
                receiveThread = new Thread(receiveWorkThread);
                receiveThread.Start();
            }
        }

        public override void open(KeyValuePair<object, object>[] parameter)
        {
            this.open();
        }


        public override void send(byte[] data)
        {
            if (data == null || data.Length < 1)
                return;
            lock (semaphore)
            {
                if (isOpen())
                {
                    try
                    {
                        BtClientSocket.OutputStream.Write(data, 0, data.Length);
                    }
                    catch (Exception ex)
                    {
                        if (onCommError != null)
                            onCommError(ex);
                    }
                }
            }
        }

        public override void setParameter(KeyValuePair<object, object>[] parameter)
        {
        }


        public override void updateParameters()
        {

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //liberar todas as conexões e etc
                this.close();
            }
        }

        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            base.Dispose();
        }

        private void receiveWorkThread()
        {
            try
            {
                BtClientSocket.Connect();
                inputStramReader = new InputStreamReader(BtClientSocket.InputStream);
                while (runReceiveThread)
                {
                    if (inputStramReader.Ready())
                    {
                        int x = inputStramReader.Read();

                        if (receive != null)
                        {
                            receive((byte)x);
                        }
                    }
                    else
                    {
                        Thread.Sleep(2);
                    }
                }

                inputStramReader.Close();
            }
            catch (Exception ex)
            {
                this.close();

            }

            inputStramReader = null;

            //    SerialReceiverBuffer sb = null;
            //    while (runReceiveThread && serialPort != null)
            //    {
            //        lock (sem)
            //        {
            //            if (serialPort.IsOpen && serialPort.BytesToRead > 0)
            //            {
            //                sb = new SerialReceiverBuffer((uint)serialPort.BytesToRead);
            //                byte[] data = new byte[serialPort.BytesToRead];
            //                sb.len = serialPort.Read(sb.serialRxBuffer, 0, sb.serialRxBuffer.Length);
            //            }
            //        }
            //        if (runReceiveThread && sb != null)
            //        {
            //            handleReceivedBytes(sb);
            //            sb = null;
            //        }
            //        else
            //        {
            //            Thread.Sleep(10);
            //        }
            //}
        }

    }
}
