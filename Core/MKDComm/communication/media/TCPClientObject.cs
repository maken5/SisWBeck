using mkdinfo.communication.devices.weightscales;
using mkdinfo.communication.media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Communication.src.communication.media
{
    public class TCPClientObject : HALCommMediaBase, IDisposable
    {
        #region defs

        const string DefaultIp = "192.168.25.200";
        const int DefaultPort = 2189;

        public enum TCPClientParam
        {
            IP,
            Port
        }

        #endregion


        #region private attributes
        string ip_port = "";
        TcpClient socket = null;
        Stream stream = null;
        readonly object semaph = new object();
        #endregion

        #region properties

        #endregion

        #region class


        public TCPClientObject(string ip_port)
        {
            //if (!WeightScaleBase.isLibraryLoaded())
            //    throw new Exception();
            if (String.IsNullOrWhiteSpace(ip_port))
                throw new Exception("Endereço de IP e porta não pode estar em branco");
            string[] field = ip_port.Split(':');
            int port = Convert.ToInt32(field[1]);
            KeyValuePair<object, object>[] pars = new KeyValuePair<object, object>[2]{
                new KeyValuePair<object,object>(TCPClientParam.IP, field[0]),
                new KeyValuePair<object,object>(TCPClientParam.Port, port),
            };
            setParameter(pars);
        }

        public TCPClientObject()
        {
            //if (!WeightScaleBase.isLibraryLoaded())
            //    throw new Exception();
        }

        #endregion

        #region api

        #endregion

        #region private methods
        void closeSocket()
        {
            if (socket != null)
            {
                socket.Close();
            }
        }

        String getIpAddr()
        {
            if (Paramns.ContainsKey(TCPClientParam.IP))
                return (String) Paramns[TCPClientParam.IP];
            return DefaultIp;
        }

        int getPortAddr()
        {
            if (Paramns.ContainsKey(TCPClientParam.Port))
                return (int)Paramns[TCPClientParam.Port];
            return DefaultPort;
        }

        private void startReceive()
        {
            byte[] buffer = new byte[2048];

            try
            {
                socket.Client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, dataReceiver, buffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro iniciando recepção de dados TCP: " + ex.Message);
                close();
            }
        }

        private void dataReceiver(IAsyncResult ar)
        {
            if (socket == null || socket.Connected != true)
                return;
            if (receive != null)
            {
                byte[] byteData = ar.AsyncState as byte[];
                foreach (byte b in byteData)
                {
                    if (b > 0)
                    {
                        Char c = Convert.ToChar(b);
                        Console.WriteLine(c);
                        receive(b);
                    }
                }
            }
            startReceive();
        }

        #endregion


        #region interface
        public override string getNameComm()
        {
            return ip_port;
        }

        public override void setParameter(KeyValuePair<object, object>[] parameter)
        {
            this.Paramns.Clear();
            foreach (KeyValuePair<object, object> v in parameter)
            {
                if (v.Key.GetType() == typeof(TCPClientParam))
                {
                    switch ((TCPClientParam)v.Key)
                    {
                        case TCPClientParam.IP:
                            if (!Paramns.ContainsKey(TCPClientParam.IP))
                                Paramns.Remove(TCPClientParam.IP);
                            Paramns.Add(TCPClientParam.IP, v.Value);
                            break;
                        case TCPClientParam.Port:
                            if (!Paramns.ContainsKey(TCPClientParam.Port))
                                Paramns.Remove(TCPClientParam.Port);
                            Paramns.Add(TCPClientParam.Port, v.Value);
                            break;
                    }
                }
            }

        }

        public override void updateParameters()
        {
            if (socket != null) close();
            socket = new TcpClient();
        }

        public override bool isOpen()
        {
            return socket != null && socket.Connected;
        }

        public override Type getType()
        {
            return typeof(System.Net.Sockets.TcpClient);
        }

        public override void open()
        {
            close();
            if (this.socket == null)
            {
                updateParameters();
            }
            try
            {
                IPEndPoint adr = new IPEndPoint(IPAddress.Parse(getIpAddr()), getPortAddr());
                socket.Connect(adr);
                if (socket.Connected)
                {
                    byte[] buffer = new byte[1024];
                    //socket.Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.KeepAlive, true);
                    socket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                    
                    stream = socket.GetStream();
                    //SetTcpKeepAlive(socket.Client, 20000, 20000);

                    startReceive();
                }
            }
            catch (Exception ex)
            {
                if (onCommError != null)
                    onCommError(ex);
            }

        }

        public override void open(KeyValuePair<object, object>[] parameter)
        {
            setParameter(parameter);
            open();
        }

        public override bool close()
        {
            lock (semaph)
            {
                if (socket != null && socket.Connected)
                {
                    try
                    {
                        socket.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return false;
                    }
                }
                socket = null;
            }
            return true;
        }

        public override void send(byte[] data)
        {
            if (socket != null && socket.Connected && data != null && data.Length>0 && stream!=null)
            {
                try
                {
                    lock (semaph)
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }
                catch (Exception ex)
                {
                    close();
                    if (onCommError != null)
                        onCommError(ex);
                }
                //startReceive();
            }
        }

        protected override void Dispose(bool disposing)
        {

            if (disposing)
            {
                close();
                socket = null;
            }
        }



        public static void SetTcpKeepAlive(Socket socket, uint keepaliveTime, uint keepaliveInterval)
        {
            /* the native structure
            struct tcp_keepalive {
            ULONG onoff;
            ULONG keepalivetime;
            ULONG keepaliveinterval;
            };
            */

            // marshal the equivalent of the native structure into a byte array
            uint dummy = 0;
            byte[] inOptionValues = new byte[Marshal.SizeOf(dummy) * 3];
            BitConverter.GetBytes((uint)(keepaliveTime)).CopyTo(inOptionValues, 0);
            BitConverter.GetBytes((uint)keepaliveTime).CopyTo(inOptionValues, Marshal.SizeOf(dummy));
            BitConverter.GetBytes((uint)keepaliveInterval).CopyTo(inOptionValues, Marshal.SizeOf(dummy) * 2);

            // write SIO_VALS to Socket IOControl
            socket.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);
        }
        #endregion
    }
}
