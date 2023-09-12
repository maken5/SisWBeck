using mkdinfo.communication.media;
using mkdInfo.communication.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mkdinfo.communication.protocol
{
    public class ProtocoloModuloPesagemSMAX : ProtocolSMA1
    {

        public class SensorNameResponse : ResponseProtocolBase
        {
            public override ResponseType responseType
            {
                get
                {
                    return ResponseType.Information;
                }
            }

            public string name = "";
            public int id = -1;

            public static SensorNameResponse parse(string line)
            {
                if (line != null && line.StartsWith("Ynam:"))
                {
                    SensorNameResponse r = new SensorNameResponse();
                    String[] lines = line.Split(':');
                    if (lines.Length > 2)
                    {
                        r.id = Convert.ToInt32(lines[1]);
                        r.name = lines[2].Trim().ToUpper();
                    }
                    return r;
                }
                return null;
            }
        }

        public class SensorCalibrationResponse : ResponseProtocolBase
        {
            public override ResponseType responseType
            {
                get
                {
                    return ResponseType.Information;
                }
            }

            public string name = "";
            public int id = -1;
            public int nrPontos = 0;
            public int pesoPadrao = 0;
            public int fundoEscala = 0;

            public static SensorCalibrationResponse parse(string line)
            {
                if (line != null && line.StartsWith("Ycal:"))
                {
                    SensorCalibrationResponse r = new SensorCalibrationResponse();
                    String[] lines = line.Split(':');
                    if (lines.Length > 5)
                    {
                        r.id = Convert.ToInt32(lines[1]);
                        r.pesoPadrao = Convert.ToInt32(lines[2]);
                        r.fundoEscala = Convert.ToInt32(lines[3]);
                        r.nrPontos = Convert.ToInt32(lines[4]);
                        r.name = lines[5];
                    }
                    return r;
                }
                return null;
            }
        }

        public class AutozeroResponse : ResponseProtocolBase
        {
            public override ResponseType responseType
            {
                get
                {
                    return ResponseType.Information;
                }
            }
            public int value = 5;
            public bool active = false;

            public static AutozeroResponse parse(string line)
            {
                try
                {
                    
                    string[] lines = line.Split(':');
                    if (lines.Length == 3 && String.Compare(lines[0], "Yaz", true)==0)
                    {
                        AutozeroResponse r = new AutozeroResponse();
                        r.value = Convert.ToInt32(lines[1]);
                        r.active = String.Compare(lines[2], "1", true) == 0;
                        return r;
                    }
                }
                catch { }

                return null;
            }
        }

        #region public attributes
        #endregion


        #region class
        #endregion

        #region interface
        protected override ResponseProtocolBase decode(string line)
        {
            ResponseProtocolBase rb = base.decode(line);
            if (rb != null)
                return rb;
            rb = SensorNameResponse.parse(line);
            if (rb != null)
                return rb;
            rb = SensorCalibrationResponse.parse(line);
            if (rb != null)
                return rb;
            rb = AutozeroResponse.parse(line);
            return rb;
        }
        #endregion
    }
}
