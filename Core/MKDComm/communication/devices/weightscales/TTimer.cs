using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace mkdinfo.communication.devices.weightscales
{
    class TTimer
    {
        public delegate void OnTime();
        public OnTime onTime = null;

        protected object timerLock = new object();
        protected Thread timerThread = null;
        protected bool timerThreadRun = false;
        protected bool timerThreadRunning = false;
        protected int timerThreadCounter = 0;
        protected int timerThreadLimit = 10000;

        protected void timerWorkThread()
        {
            int t;
            timerThreadRunning = true;
            while (timerThreadRun)
            {
                Thread.Sleep(1);
                lock (timerLock)
                {
                    timerThreadCounter++;
                    t = timerThreadCounter;
                }
                if (t >= timerThreadLimit)
                {
                    timerThreadRun = false;
                    if (onTime != null)
                        onTime();
                }
            }
            timerThreadRunning = false;
        }

        public TTimer() 
        { 
        }
        public bool isRunning()
        {
            return timerThreadRunning;
        }
        public void reset()
        {
            lock (timerLock)
            {
                timerThreadCounter = 0;
            }
        }
        public void stop()
        {
            if (timerThreadRunning)
            {
                lock (timerLock)
                {
                    timerThreadRun = false;
                }
            }
            if (timerThread != null)
            {
                if (timerThread.IsAlive)
                {
                    if (timerThread.Join(30))
                        timerThread.Abort();
                }
                timerThread = null;
            }
        }
        public void start(int timeInMillis)
        {
            stop();
            timerThreadRun = true;
            timerThreadLimit = timeInMillis;
            timerThreadCounter = 0;
            timerThread = new Thread(new ThreadStart(timerWorkThread));
            timerThread.Start();
        }

    }
}
