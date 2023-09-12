using mkdinfo.communication.media;
using mkdinfo.communication.protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Communication.src.communication.media;
using Communication.src.communication.protocol;

namespace Communication.src.communication.devices.readers
{
    public class IntermecRFIDReader : ReaderBase
    {

        public IntermecRFIDReader(HALCommMediaBase communicationMedia)
            : base(communicationMedia)
        {

            this.protocol = new IntermecBRI();
        }

        public IntermecRFIDReader(string ip, int port)
        {
            TCPClientObject o = new TCPClientObject();
            KeyValuePair<object, object>[] data = new KeyValuePair<object, object>[2];
            data[0] = new KeyValuePair<object, object>(Communication.src.communication.media.TCPClientObject.TCPClientParam.IP, ip);
            data[1] = new KeyValuePair<object, object>(Communication.src.communication.media.TCPClientObject.TCPClientParam.Port, port);
            o.setParameter(data);
            this.media = o;
            this.protocol = new IntermecBRI();
        }

        public IntermecRFIDReader(string ip_port)
        {
            string[] field;
            int port;
            if (String.IsNullOrEmpty(ip_port))
                throw new Exception("Endereço IP e porta inválido");
            field = ip_port.Split(':');
            if (field.Length == 2)
            {
                try
                {
                    port = Convert.ToInt32(field[1]);
                }
                catch { throw new Exception("Porta Inválida"); }

                TCPClientObject o = new TCPClientObject();
                KeyValuePair<object, object>[] data = new KeyValuePair<object, object>[2];
                data[0] = new KeyValuePair<object, object>(Communication.src.communication.media.TCPClientObject.TCPClientParam.IP, field[0]);
                data[1] = new KeyValuePair<object, object>(Communication.src.communication.media.TCPClientObject.TCPClientParam.Port, port);
                try
                {
                    o.setParameter(data);
                    this.media = o;
                }
                catch (Exception ex)
                {
                    throw new Exception("Não foi possível criar conexão com " + ip_port, ex);
                }
                
            }
            else
                throw new Exception("Formato inválido de endereço/porta de comunicação");
            
            this.protocol = new IntermecBRI();
        }
       
    }
}
