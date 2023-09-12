using mkdinfo.communication.protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.src.communication.protocol
{
    public class IntermecBRI : ProtocolBase
    {
        StringBuilder sb = new StringBuilder();
        DateTime lastReceivedData = DateTime.Now;


        protected override void receive(byte data)
        {
            resetTimerCounter();
            lastReceivedData = DateTime.Now;
            if (onNewResponse != null)
            {
                char chr = Convert.ToChar(data);
                switch (chr)
                {
                    case '\r':
                        string line = sb.ToString();
                        sb.Clear();
                        onNewLine(line);
                        break;
                    case '\n':
                        sb.Clear();
                        break;
                    default:
                        if (sb.Length < 128)
                        {
                            sb.Append(chr);
                        }
                        break;
                }
            }
        }

        protected override void onCommError(Exception ex)
        {
            stop();
            if (this.onError != null)
            {
                this.onError(new Exception("Erro no link de comunicação", ex));
            }
        }

        protected override void handleTimeout()
        {
            if (DateTime.Now.Subtract(lastReceivedData).TotalMilliseconds > 5000)
            {
                
                if (onError != null)
                {
                    onError(new Exception("Communication time out"));
                }
                Console.WriteLine("----------------> time out <-----------------");
                lastReceivedData = DateTime.Now;
                resetTimerCounter();
            }
            else
            {
                if (comm != null)
                {
                    comm.send("read report=event\r\n");
                }
            }
        }


        protected virtual void onNewLine(String line)
        {
            if (onNewResponse != null)
            {
                onNewResponse(new RFIDProtocolResponse(line));
            }
        }




        public override void start()
        {
            base.start();
            base._startTimeOut = 10000;
            base._timeOut = 1000;
            comm.send("read report=event\r\n");
        }
    }
}
