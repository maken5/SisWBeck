using mkdinfo.communication.media;
using mkdinfo.communication.protocol;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace mkdinfo.communication.protocol
{
    public class ProtocolSMA1 : ProtocolBase
    {
        #region defs
        const char CHAR_SEPARATOR = ':';

        protected List<Type> responseTypes = new List<Type>();


        #region Response Types

        public class StandardResponseMessage : ResponseProtocolBase
        {
            #region defs
            public enum ScaleStatus
            {
                CenterOfZero        = 'Z',
                OverCapacity        = 'O',
                UnderCapacity       = 'U',
                ZeroError           = 'E',  //peso com '-'
                InitialZeroError    = 'I',  //peso com '-'
                TareError           = 'T', //peso com '-'
                None                = ' '   //none of the above conditions
            };
            public enum GrossNetStatus
            {
                GrossNormalWeight           = 'G',
                TareWeight                  = 'T',
                NetNormalWeight             = 'N',
                GrossWeightHighResolution   = 'g',
                NetWeightHighResolution     = 'n'
            };
            public enum MotionStatus
            {
                ScaleInMotion       = 'M',
                ScaleNotInMotion    = ' '
            };
            #endregion

            #region public attributes
            public override ResponseType responseType
            {
                get
                {
                    return ResponseType.Weight;
                }
            }
            public ScaleStatus status;
            public int range;
            public GrossNetStatus grossNetStatus;
            public MotionStatus motion;
            public char reserved;
            public float weight;
            public string unit;
            public bool noWeightData = false;
            #endregion

            protected static bool isHyphen(string str)
            {
                if (str!=null)
                foreach (Char c in str)
                {
                    if (c != '-')
                        return false;
                }
                return true;
            }

            public static StandardResponseMessage parse(string protocolData)
            {
                StandardResponseMessage srm = new StandardResponseMessage();
                try
                {
                    srm.status = (ScaleStatus)protocolData[0];
                    srm.range = Convert.ToInt32(protocolData.Substring(1, 1));
                    srm.grossNetStatus = (GrossNetStatus)protocolData[2];
                    srm.motion = (MotionStatus)protocolData[3];
                    srm.reserved = protocolData[4];
                    
                    if (srm.status != ScaleStatus.ZeroError && srm.status != ScaleStatus.InitialZeroError && srm.status != ScaleStatus.TareError)
                    {
                        string number = protocolData.Substring(5, 10).Trim();
                        number = number.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator).Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                        if (String.IsNullOrEmpty(number) || isHyphen(number))
                            srm.noWeightData = true;
                        else
                            srm.weight = (float)Convert.ToDouble(protocolData.Substring(5, 10).Trim());
                    }
                    else
                        srm.weight = 0;
                    srm.unit = protocolData.Substring(15, 3);
                }
                catch 
                {
                    return null;
                }
                return srm;
            }
        };

        public class ErrorResponse : ResponseProtocolBase
        {
            public enum ErrorCode
            {
                UnrecognizedCommand = '?',
                CommunicationError = '!',
                RamOrRomError = 'R',
                EerpomError = 'E',
                CalibrationError = 'C'
            };
            public override ResponseType responseType
            {
                get
                {
                    return ResponseType.Error;
                }
            }

            public List<ErrorCode> erros = new List<ErrorCode>();
            public char manufacturerError;

            public static ErrorResponse parse(string line)
            {
                if (line == "?")
                {
                    ErrorResponse er = new ErrorResponse();
                    er.erros.Add(ErrorCode.UnrecognizedCommand);
                    return er;
                }
                if (line == "!")
                {
                    ErrorResponse er = new ErrorResponse();
                    er.erros.Add(ErrorCode.CommunicationError);
                    return er;
                }
                if (line != null && line.Length == 4)
                {
                    ErrorResponse er = new ErrorResponse();
                    if (line[0] == 'R') er.erros.Add(ErrorCode.RamOrRomError);
                    else if (line[0] != ' ') return null;
                    if (line[1] == 'E') er.erros.Add(ErrorCode.EerpomError);
                    else if (line[1] != ' ') return null;
                    if (line[2] == 'C') er.erros.Add(ErrorCode.CalibrationError);
                    else if (line[2] != ' ') return null;
                    er.manufacturerError = line[3];
                    return er;
                }
                return null;
            }
        };

        public class EndInformationCommandResponse : ResponseProtocolBase
        {
            public override ResponseType responseType
            {
                get
                {
                    return ResponseType.Information;
                }
            }

            public static EndInformationCommandResponse parse(string line)
            {
                if (line == "END:")
                    return new EndInformationCommandResponse();
                return null;
            }
        }

        public class SMACLevelommandResonse : ResponseProtocolBase
        {
            public override ResponseType responseType
            {
                get
                {
                    return ResponseType.Information;
                }
            }
            public string value;
            public int complianceLevel;
            public float revision;

            public static SMACLevelommandResonse parse(string line)
            {
                try
                {
                    string[] fields = line.Split(CHAR_SEPARATOR);
                    if (fields.Length == 2)
                    {
                        if (fields[0] == "SMA")
                        {
                            SMACLevelommandResonse r = new SMACLevelommandResonse();
                            r.value = fields[1];
                            fields = fields[1].Split('/');
                            r.complianceLevel = Convert.ToInt32(fields[0]);
                            fields[1] = fields[1].Replace(".", CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator).Replace(",", CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator);
                            r.revision = (float)Convert.ToDouble(fields[1]);
                            return r;
                        }

                    }
                }
                catch { }
                return null;
            }
            
        }

        public class AboutCommandResponse : ResponseProtocolBase
        {

            public enum FieldType
            {
                Manufacturer ,
                Model,
                Revision,
                SerialNumber,
                OP1,
                OP2,
                OP3,
                OP4,
                OP5,
                OP6,
                OP7,
                OP8,
                OP9
            };

            protected static Dictionary<string, FieldType> _map = null;

            protected static Dictionary<string, FieldType> map
            {
                get
                {
                    if (_map == null)
                        _map = new Dictionary<string, FieldType>()
                        {
				            {"MFG",FieldType.Manufacturer},
                            {"MOD",FieldType.Model       },
                            {"REV",FieldType.Revision    },
                            {"SN_",FieldType.SerialNumber},
                            {"SN ",FieldType.SerialNumber},
                            {"OP1",FieldType.OP1         },
                            {"OP2",FieldType.OP2         },
                            {"OP3",FieldType.OP3         },
                            {"OP4",FieldType.OP4         },
                            {"OP5",FieldType.OP5         },
                            {"OP6",FieldType.OP6         },
                            {"OP7",FieldType.OP7         },
                            {"OP8",FieldType.OP8         },
                            {"OP9",FieldType.OP9         }
                        };
                    return _map;
                }
            }


            public override ResponseType responseType
            {
                get
                {
                    return ResponseType.Information;
                }
            }
            public FieldType fieldType;
            public string value = null;

            public static AboutCommandResponse parse(string line)
            {
                AboutCommandResponse r = null;
                try
                {
                    string[] fields = line.Split(CHAR_SEPARATOR);
                    if (fields.Length == 2)
                    {
                        if (map.ContainsKey(fields[0]))
                        {
                            r = new AboutCommandResponse();
                            if (map.TryGetValue(fields[0], out r.fieldType))
                            {
                                r.value = fields[1];
                                return r;
                            }
                        }

                    }
                }
                catch { }
                return r;
            }

        };

        public class InformationCommandResponse : ResponseProtocolBase
        {
            public enum FieldType
            {
                Manufacturer    ,
                Model           ,
                CAP             
            };

            protected static Dictionary<string, FieldType> _map = null;
            protected static Dictionary<string, FieldType> map
            {
                get
                {
                    if (_map == null)
                    {
                        _map = new Dictionary<string, FieldType>()
                        {   
                            {"TYP",FieldType.Manufacturer},
                            {"CMD",FieldType.Model       },
                            {"CAP",FieldType.CAP         }
                        };
                    }
                    return _map;
                }
            }


            public override ResponseType responseType
            {
                get
                {
                    return ResponseType.Information;
                }
            }
            public FieldType fieldType;
            public string value = null;

            public static InformationCommandResponse parse(string line)
            {
                InformationCommandResponse r = null;
                try
                {
                    string[] fields = line.Split(CHAR_SEPARATOR);
                    if (fields.Length == 2)
                    {
                        if (map.ContainsKey(fields[0]))
                        {
                            r = new InformationCommandResponse();
                            if (map.TryGetValue(fields[0], out r.fieldType))
                            {
                                r.value = fields[1];
                                return r;
                            }
                        }

                    }
                }
                catch { }
                return r;
            }
        }

        public enum SMACommands
        {
            RequestDisplayWeight,
            RequestHighResolutionWeight,
            RequestDisplayedWeightAfterStability,
            RequestHighResolutionWeightAfterStability,
            RequestScaleToZero,
            RequestScaleToTare,
            SetScaleTareWeight,
            ReturnTareWeight,
            ClearScaleTareWeight,
            ChangeUnitsOfMeasureToggleOrScroll,
            SetUnitsOfMeasure,
            InvokeScaleDiagnostics,
            AboutScaleFirstLine,
            AboutScaleScroll,
            ScaleInformation,
            ScaleInformationScroll,
            ExtendedCommandSet,
            AbortCommand,
            RepeatDisplayedWeightContinuously,
            RepeatHighResolutionWeightContinuously
        };

        #endregion

        protected static Dictionary<char, SMACommands> _SMACommandsMap = null;


        public static Dictionary<char, SMACommands> SMACommandsMap
        {
            get
            {
                if (_SMACommandsMap == null)
                {
                    _SMACommandsMap = new Dictionary<char, SMACommands>()
                    {
                        {'W', SMACommands.RequestDisplayWeight},
                        {'H', SMACommands.RequestHighResolutionWeight},
                        {'P', SMACommands.RequestDisplayedWeightAfterStability},
                        {'Q', SMACommands.RequestHighResolutionWeightAfterStability},
                        {'Z', SMACommands.RequestScaleToZero},
                        {'T', SMACommands.RequestScaleToTare},
                        {'T', SMACommands.SetScaleTareWeight},
                        {'M', SMACommands.ReturnTareWeight},
                        {'C', SMACommands.ClearScaleTareWeight},
                        {'U', SMACommands.ChangeUnitsOfMeasureToggleOrScroll},
                        {'U', SMACommands.SetUnitsOfMeasure},
                        {'D', SMACommands.InvokeScaleDiagnostics},
                        {'A', SMACommands.AboutScaleFirstLine},
                        {'B', SMACommands.AboutScaleScroll},
                        {'I', SMACommands.ScaleInformation},
                        {'N', SMACommands.ScaleInformationScroll},
                        {'X', SMACommands.ExtendedCommandSet},
                        {'\x1b', SMACommands.AbortCommand},
                        {'R', SMACommands.RepeatDisplayedWeightContinuously},
                        {'S', SMACommands.RepeatHighResolutionWeightContinuously}
                    };
                }
                return _SMACommandsMap;
            }
        }

                                                                         
        #endregion

        #region protected attributes
        protected bool commandStarted = false;
        protected StringBuilder sb = new StringBuilder();
        #endregion

        #region class


        #endregion

        #region interface

        protected override void receive(byte data)
        {
            resetTimerCounter();
            if (onNewResponse != null)
            {
                char chr = Convert.ToChar(data);
                switch (chr)
                {
                    case '\r':
                        string line = sb.ToString();
                        sb.Clear();
                        //Console.WriteLine(line);
                        commandStarted = false;
                        onNewLine(line);
                        break;
                    case '\n':
                        //Console.WriteLine();
                        sb.Clear();
                        commandStarted = true;
                        break;
                    default:
                        if (sb.Length < 128 && commandStarted)
                        {
                            //Console.WriteLine(chr);
                            sb.Append(chr);
                            //Console.Write(chr);
                        }
                        else
                        {
                            commandStarted = false;
                        }
                        break;
                }
            }
        }

        protected virtual ResponseProtocolBase decode(string line)
        {
            ResponseProtocolBase er = ErrorResponse.parse(line);
            if (er != null) 
                return er;
            er = StandardResponseMessage.parse(line);
            if (er != null)
                return er;
            er = AboutCommandResponse.parse(line);
            if (er != null)
                return er;
            er = InformationCommandResponse.parse(line);
            if (er != null)
                return er;
            er = SMACLevelommandResonse.parse(line);
            if (er != null) 
                return er;
            er = EndInformationCommandResponse.parse(line);
            if (er != null)
                return er;
            foreach (Type t in responseTypes)
            {
                MethodInfo method = t.GetMethod("parse");
                if (method != null)
                {
                    er = (ResponseProtocolBase)method.Invoke(null, new object[]{line});
                    if (er != null)
                        return er;
                }
            }
            return null;
        }

        protected virtual void onNewLine(String line)
        {
            if (onNewResponse != null)
                onNewResponse(decode(line));
        }

        

        #endregion

    }
}
