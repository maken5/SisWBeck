using mkdinfo.communication.devices.weightscales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mkdinfo.communication.protocol;

namespace mkdinfo.communication.protocol
{

    public abstract class SMAProtocolWeightScaleResponseBase
    { 

        public enum UnityOfMeasure
        {
            Custom,
            Pounds ,
            Ounces,
            PoundsToOunces,
            Kilograms,
            Grams,
            TroyOunces,
            Carats,
            HongKongTaels,
            SingaporeTaels,
            TaiwaneseTaels,
            Grains,
            Pennyweights,
            Milligrams,
            PartsPerPounds,
            ChineseTaels,
            Mommes,
            AustrianCarats,
            Tola,
            Bath,
            Mesghal,
            MetricTon,
            AvoirTon,
            Microgram,
            Tael_nonspecific,
            Percent,
            NoUnits
        };

        public enum ScaleStatus
        {
            CenterOfZero                = 'Z',
            OverCapacity                = 'O',
            UnderCapacity               = 'U',
            ZeroError                   = 'E',
            InitialZeroError            = 'I',
            TareError                   = 'T',
            None                        = ' '
        };

        public enum GrossNetStatus
        {
            GrossNormalHeight           = 'G',
            TareWeight                  = 'T',
            NetNormalWeight             = 'N',
            GrossWeightHighResolution   = 'g',
            NetWeightHighResolution     = 'n',
            Unknow                      = '\n'
        };

        public enum MotionStatus
        {
            ScaleInMotion               = 'M',
            ScaleNotInMotion            = ' ',
            Future                      = 'f'
        };

        public enum Erros
        {
            UnrecognizedCommand = '?',
            CommError           = '!',
            RamOrRomError       = 'R',
            EepromError         = 'E',
            CalibrationError    = 'C',
            ManufacuterError    = 'M'
        };

        public enum DeviceType
        {
            Scale = 'S',
            Classifier = 'C',
            Unknow =    ' '
        };

        #region static
        public static Dictionary<string, UnityOfMeasure> Units = new Dictionary<string, UnityOfMeasure>()
        {
            {"lb ", UnityOfMeasure.Pounds            },
            {"oz ", UnityOfMeasure.Ounces            },
            {"l/o", UnityOfMeasure.PoundsToOunces    },
            {"kg ", UnityOfMeasure.Kilograms         },
            {"g  ", UnityOfMeasure.Grams             },
            {"ozt", UnityOfMeasure.TroyOunces        },
            {"ct ", UnityOfMeasure.Carats            },
            {"tlh", UnityOfMeasure.HongKongTaels     },
            {"tls", UnityOfMeasure.SingaporeTaels    },
            {"tlt", UnityOfMeasure.TaiwaneseTaels    },
            {"gn ", UnityOfMeasure.Grains            },
            {"dwt", UnityOfMeasure.Pennyweights      },
            {"mg ", UnityOfMeasure.Milligrams        },
            {"/lb", UnityOfMeasure.PartsPerPounds    },
            {"tlc", UnityOfMeasure.ChineseTaels      },
            {"mom", UnityOfMeasure.Mommes            },
            {"k  ", UnityOfMeasure.AustrianCarats    },
            {"tol", UnityOfMeasure.Tola              },
            {"bat", UnityOfMeasure.Bath              },
            {"ms ", UnityOfMeasure.Mesghal           },
            {"t  ", UnityOfMeasure.MetricTon         },
            {"ton", UnityOfMeasure.AvoirTon          },
            {"ug ", UnityOfMeasure.Microgram         },
            {"tl ", UnityOfMeasure.Tael_nonspecific  },
            {"%  ", UnityOfMeasure.Percent           },
            {"   ", UnityOfMeasure.NoUnits           }
        };

        protected static Dictionary<char, Type> firstChar = new Dictionary<char, Type>()
        {
            {'!', typeof(SMAProtocolWeigthScaleResponseErrorDiagnostic)},
            {'?', typeof(SMAProtocolWeigthScaleResponseErrorDiagnostic)},
            {'I', typeof(SMAProtocolWeightScaleResponse)},
            {'U', typeof(SMAProtocolWeightScaleResponse)},
            {'Z', typeof(SMAProtocolWeightScaleResponse)}
        };

        protected static SMAProtocolWeightScaleResponseAboutOrInformation _aboutOrInfo = new SMAProtocolWeightScaleResponseAboutOrInformation();
        protected static SMAProtocolWeightScaleResponseAboutOrInformation aboutOrInfo
        {
            get
            {
                if (_aboutOrInfo == null || _aboutOrInfo.end)
                    _aboutOrInfo = new SMAProtocolWeightScaleResponseAboutOrInformation();
                return _aboutOrInfo;
            }
        }
        public static SMAProtocolWeightScaleResponseBase build(string data, Type waitingType)
        {
            if (!String.IsNullOrEmpty(data))
            {
                if (firstChar.ContainsKey(data[0]))
                {
                    Type t = firstChar[data[0]];
                    SMAProtocolWeightScaleResponseBase ws = (SMAProtocolWeightScaleResponseBase)Activator.CreateInstance(t);
                    ws.parse(data);
                    return ws;
                }
                else  if (data.Length>3)
                {
                    if (data.Length == 4)
                    {
                        SMAProtocolWeigthScaleResponseErrorDiagnostic ws = new SMAProtocolWeigthScaleResponseErrorDiagnostic();
                        ws.parse(data);
                        return ws;
                    }
                    string s = data.Substring(0, 3);
                    if (waitingType == typeof(SMAProtocolWeightScaleResponse))
                    {
                        try
                        {
                            SMAProtocolWeightScaleResponse ws = new SMAProtocolWeightScaleResponse();
                            ws.parse(data);
                            return ws;
                        }
                        catch { }
                    }
                    switch (s)
                    {
                        case "SMA":
                        case "MFG":
                        case "MOD":
                        case "REV":
                        case "SN ":
                        case "OP1":
                        case "OP2":
                        case "OP3":
                        case "OP4":
                        case "OP5":
                        case "OP6":
                        case "OP7":
                        case "OP8":
                        case "OP9":
                        case "TYPE":
                        case "CAP":
                        case "CMD":
                        case "END":
                            aboutOrInfo.parse(data, s);
                            return aboutOrInfo;
                        default:
                            try
                            {
                                SMAProtocolWeightScaleResponse ws = new SMAProtocolWeightScaleResponse();
                                ws.parse(data);
                                return ws;
                            }
                            catch { }
                            break;
                    }
                    
                }
            }
            return null;
        }

        #endregion

        protected abstract void parse(string data, string prefix = null);
        protected abstract void clear();
    }

    public class SMAProtocolWeightScaleResponse : SMAProtocolWeightScaleResponseBase
    {
        public ScaleStatus scaleStatus;
        public ushort range;
        public GrossNetStatus grossNetStatus;
        public MotionStatus motionStatus;
        public float weight;
        public UnityOfMeasure unit;
        public string strUnit;

        protected override void parse(string data, string prefix = null)
        {
            scaleStatus = (ScaleStatus)data[0];
            if (data[1] != ' ')
                range = (ushort)Convert.ToInt16(data[1]);
            grossNetStatus = (GrossNetStatus)data[2];
            if (data[3] == 'M' || data[3] == ' ')
                motionStatus = (MotionStatus)data[3];
            weight = (float)Convert.ToDouble(data.Substring(4, 10).Replace(',', '.').Trim());
            string strUnit = data.Substring(14, 3);
            if (Units.ContainsKey(strUnit))
                unit = Units[strUnit];
            else
                unit = UnityOfMeasure.Custom;
        }

        protected override void clear()
        {
            scaleStatus = ScaleStatus.None;
            range = 0;
            grossNetStatus = GrossNetStatus.Unknow;
            weight = 0;
            unit = UnityOfMeasure.Kilograms;
            strUnit = "kg ";
        }

    }

    public class SMAProtocolWeigthScaleResponseErrorDiagnostic : SMAProtocolWeightScaleResponseBase
    {
        public HashSet<Erros> erros = new HashSet<Erros>();
        public char manufacturerError = ' ';
        protected override void parse(string data, string prefix = null)
        {
            if (data[0] != ' ')
                erros.Add((Erros)data[0]);
            if (data.Length == 4)
            {
                if (data[1] != ' ')
                    erros.Add((Erros)data[1]);
                if (data[2] != ' ')
                    erros.Add((Erros)data[2]);
                if (data[3] != ' ')
                    erros.Add((Erros)data[3]);
            }
        }
        protected override void clear()
        {
            erros.Clear();
            manufacturerError = ' ';
        }
    }


    public class SMAProtocolWeightScaleResponseAboutOrInformation : SMAProtocolWeightScaleResponseBase
    {
        public class About
        {
            public string manufacturerName = null;
            public string model = null;
            public string revision = null;
            public string serialNumber = null;
            public string[] optional = new string[10] { null, null, null, null, null, null, null, null, null, null };
        }
        public class Information
        {
            public class Capacity
            {
                public UnityOfMeasure unit;
                public string customUnit;
                public float totalCapacity;
                public int resolution;
                public int decimalPointPosition;
            };
            public DeviceType deviceType;
            public List<Capacity> capacit = new List<Capacity>();
            public List<ProtocolSMA1.SMACommands> commands = new List<ProtocolSMA1.SMACommands>()
            {
                ProtocolSMA1.SMACommands.RequestDisplayWeight,
                ProtocolSMA1.SMACommands.RequestScaleToZero,
                ProtocolSMA1.SMACommands.InvokeScaleDiagnostics,
                ProtocolSMA1.SMACommands.AboutScaleFirstLine,
                ProtocolSMA1.SMACommands.AboutScaleScroll,
                ProtocolSMA1.SMACommands.AbortCommand
            };
        }

        public int SMAComplianceLevel = 1;
        public float SMAComplianceRevision = 1.0f;

        public About about = null;
        public Information information = null;

        public bool end = false;

        protected override void parse(string data, string prefix = null)
        {
            string posfix = null;
            try
            {
                if (prefix == null)
                    prefix = data.Substring(0, 3);
                if (data.Length>3)
                    posfix = data.Substring(4);

                switch (prefix)
                {
                    case "SMA":
                        string[] fields = posfix.Split('/');
                        SMAComplianceLevel = Convert.ToInt32(fields[0]);
                        SMAComplianceRevision = (float)Convert.ToDouble(fields[1]);
                        break;
                    case "MFG":
                        if (about == null) about = new About();
                        about.manufacturerName = posfix;
                        break;
                    case "MOD":
                        if (about == null) about = new About();
                        about.model = posfix;
                        break;
                    case "REV":
                        if (about == null) about = new About();
                        about.revision= posfix;
                        break;
                    case "SN ":
                        if (about == null) about = new About();
                        about.serialNumber= posfix;
                        break;
                    case "OP1":
                        if (about == null) about = new About();
                        about.optional[0] = posfix;
                        break;
                    case "OP2":
                        if (about == null) about = new About();
                        about.optional[1] = posfix;
                        break;
                    case "OP3":
                        if (about == null) about = new About();
                        about.optional[2] = posfix;
                        break;
                    case "OP4":
                        if (about == null) about = new About();
                        about.optional[3] = posfix;
                        break;
                    case "OP5":
                        if (about == null) about = new About();
                        about.optional[4] = posfix;
                        break;
                    case "OP6":
                        if (about == null) about = new About();
                        about.optional[5] = posfix;
                        break;
                    case "OP7":
                        if (about == null) about = new About();
                        about.optional[6] = posfix;
                        break;
                    case "OP8":
                        if (about == null) about = new About();
                        about.optional[7] = posfix;
                        break;
                    case "OP9":
                        if (about == null) about = new About();
                        about.optional[8] = posfix;
                        break;
                    case "TYPE":
                        if (information == null)
                            information = new Information();
                        information.deviceType = (DeviceType)posfix[0];
                        break;
                    case "CAP":
                        if (information == null)
                            information = new Information();
                        
                        break;
                    case "CMD":
                        if (information == null)
                            information = new Information();
                        foreach (Char c in posfix)
                        {
                            if (ProtocolSMA1.SMACommandsMap.ContainsKey(c))
                                information.commands.Add(ProtocolSMA1.SMACommandsMap[c]);
                        }
                        break;
                    case "END":
                        end = true;
                        break;
                }
            }
            catch { }
        }

        protected override void clear()
        {
            SMAComplianceLevel = 1;
            SMAComplianceRevision = 1.0f;
            about = null;
            information = null;
            end = false;
        }
    }

}
