using mkdinfo.communication.media;
using mkdinfo.communication.protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mkdinfo.communication.devices.weightscales
{
    public class ModuloPesagemSMAX : WeightScaleBase
    {

        protected int _memoria = -1;

        public int memoria
        {
            get { return _memoria; }
        }

        //public class CalibrationData
        //{
        //    int id;
        //    int nrPontos;
        //    int pesoPadrao;
        //    int fundoEscala;
        //    string nomeSensor;
        //};

        public class WeightScaleInformation
        {
            public int SMALevel = 0;
            public float SMACompliance = 0;
            public string manufacturer = null;
            public string model = null;
            public string revision = null;
            public string serialNumber = null;
            public string operation = null;
        };

        private WeightScaleInformation ws = new WeightScaleInformation();

        public WeightScaleInformation Information
        {
            get { return ws; }
        }


        public ProtocoloModuloPesagemSMAX.SensorCalibrationResponse[] calibracoes = new ProtocoloModuloPesagemSMAX.SensorCalibrationResponse[5];

        public override Dictionary<ProtocolSMA1.SMACommands, byte[]> simpleCommands { get { return _simpleCommands; } }
        public override Dictionary<ProtocolSMA1.SMACommands, string> parametrizedCommands
        {
            get { return _parametrizedCommands; }
        }
        public override Dictionary<string, string> extendedSimpleCommands { get { return _extendendSimpleCommands; } }
        public override Dictionary<string, string> extendedParametrizedCommands { get { return _extendedParametrizedCommands; } }



        protected Dictionary<ProtocolSMA1.SMACommands, byte[]> _simpleCommands = new Dictionary<ProtocolSMA1.SMACommands, byte[]>(){
                                                    {ProtocolSMA1.SMACommands.RequestDisplayWeight, System.Text.Encoding.ASCII.GetBytes("\nW\r")},
                                                    {ProtocolSMA1.SMACommands.RequestScaleToZero,  System.Text.Encoding.ASCII.GetBytes("\nZ\r")},                   // Envia comando de zero
                                                    {ProtocolSMA1.SMACommands.RepeatDisplayedWeightContinuously,  System.Text.Encoding.ASCII.GetBytes("\nR\r")},    // Envia comando para requerimento de peso contínuo
                                                    {ProtocolSMA1.SMACommands.ScaleInformation,  System.Text.Encoding.ASCII.GetBytes("\nI\r")},                     // Envia comando de requerimento de informação da balança
                                                    {ProtocolSMA1.SMACommands.ScaleInformationScroll,  System.Text.Encoding.ASCII.GetBytes("\nN\r")},               // Envia comando de requerimento de informação da balançca, próxima linha
                                                    {ProtocolSMA1.SMACommands.AboutScaleFirstLine,  System.Text.Encoding.ASCII.GetBytes("\nA\r")},                  // Envia comando de info da balançca, primeira linha
                                                    {ProtocolSMA1.SMACommands.AboutScaleScroll,  System.Text.Encoding.ASCII.GetBytes("\nB\r")},                     // Envia comando de info da balançca, próxima linha
                                                    {ProtocolSMA1.SMACommands.RequestDisplayedWeightAfterStability,  System.Text.Encoding.ASCII.GetBytes("\nQ\r")}  // Envia comando de requerimento de peso após establização
                                                };

        protected Dictionary<ProtocolSMA1.SMACommands, string> _parametrizedCommands = new Dictionary<ProtocolSMA1.SMACommands, string>()
        {
            //{Commands.ExtendedCommandSetAutozeroValue, "\nXV({0})\r"}
        };


        protected Dictionary<string, string> _extendendSimpleCommands = new Dictionary<string, string>()
        {
            {"AutozeroOn",          "\nXA\r"},  //Liga autozero
            {"AutozeroOff",         "\nXa\r"},  //Desliga autozero
            {"GetAutozeroValue",    "\nXv\r"},  //Envia requisição de valor do autozero
            {"GetCalibData",        "\nXc\r"},  //Envia requisição de valor de calibração
            {"GetSensorName",       "\nXn\r"},  //Envia requisição de nome de sensor
        };
        protected Dictionary<string, string> _extendedParametrizedCommands = new Dictionary<string, string>()
        {
            {"SetAutozero",     "\nXV:{0}\r"},                      //SetAutozero(autozeroValue)
            {"Write",           "\nXW:{0}\r"},                      //Write(writePassord)
            {"SetCalibData",    "\nXC:{0}:{1}:{2}:{3}:{4}:{5}\r"},  //SetCalibData(idx,nrPontos, pesoPadrao, fundoEscala, sensorName, senhaSetCalibData)
            {"Calibrar",        "\nXR:{0}:{1}:{2}\r"},              //Calibrar(fundoEscala,pesoPadrao,senhaCalibrar)
            {"SetMemory",       "\nXM:{0}:{1}\r"},                  //SetMemory(memory,senhaSetMemory)
            {"SetSerialNumber", "\nXS:{0}:{1}:{2}\r"},              //SetSerialNumber(serialNumber,keyLicense,senhaSetSerialNumber)
            {"SetSensorName",   "\nXN:{0}:{1}\r"},                  //SetSensorName(especificar)
            {"SetLicenseKey",   "\nXL:{0}:{1}\r"}                   //
        };


        //public CalibrationData[] sensores = new CalibrationData[5] { null, null, null, null, null };

        #region classe


        protected override bool onNewResponseProcess(ResponseProtocolBase rp)
        {   
            return true;
        }

        public ModuloPesagemSMAX(HALCommMediaBase communicationMedia)
            : base(communicationMedia)
        {
            //if (!WeightScaleBase.isLibraryLoaded())
            //    throw new Exception();
            this.protocol = new ProtocoloModuloPesagemSMAX();
        }

        public ModuloPesagemSMAX()
            : base()
        {
            //if (!WeightScaleBase.isLibraryLoaded())
            //    throw new Exception();
        }

        public void setAutozero(bool ligar)
        {
            if (ligar)
                sendExtendedCommand("AutozeroOn", null);
            else
                sendExtendedCommand("AutozeroOff", null);
        }

        public void setAutozero(int valor)
        {
            if (valor < 1) valor = 1;
            if (valor > 9) valor = 9;
            sendExtendedCommand("SetAutozero", new object[] { valor });
        }

        public void getAutozero()
        {
            sendExtendedCommand("GetAutozeroValue", null);
        }

        public void getCalibration()
        {
            sendExtendedCommand("GetCalibData", null);
        }

        public void getSensorName()
        {
            sendExtendedCommand("GetSensorName", null);
        }

        public void save(int senha)
        {
            sendExtendedCommand("Write", new object[] { senha });
        }

        public void setCalibData(int idx, int nrPontos, int pesoPadrao, int fundoEscala, string sensorName, int senha)
        {
            sendExtendedCommand("SetCalibData", new object[] { idx, nrPontos, pesoPadrao, fundoEscala, sensorName, senha });
        }

        public void calibrar(int fundoEscala, int pesoPadrao, int senha)
        {
            sendExtendedCommand("Calibrar", new object[] { fundoEscala, pesoPadrao, senha });
        }

        //----------------------

        public void setMemory(int memory, int senha)
        {
            sendExtendedCommand("SetMemory", new object[] { memory, senha });
        }


        public void setSerialNumber(int numeroSerie, int chave, int senha)
        {
            sendExtendedCommand("SetSerialNumber", new object[] { numeroSerie, chave, senha });
        }

        public void setSensorName(string sensorName, int senha)
        {
            sendExtendedCommand("SetSensorName", new object[] { sensorName, senha });
        }

        public void setLicenseKey(int chave, int senha)
        {
            sendExtendedCommand("SetLicenseKey", new object[] { chave, senha });
        }

        public void updateScaleInformation()
        {
            sendCommand(ProtocolSMA1.SMACommands.AboutScaleFirstLine);
        }

        public override bool start()
        {
            base.start();
            //updateScaleInformation();
            return true;
        }

        public bool IsOpen()
        {
            return this.protocol != null && this.protocol.media != null && this.protocol.media.isOpen();
        }

        #endregion

        #region interface



        #endregion



        const ushort POLYNOMIAL = 0x00d8;
        const int WIDTH = (8 * sizeof(ushort));
        const ushort TOPBIT = (ushort)(1 << (WIDTH - 1));
        private static ushort crc16(uint valor)
        {
            ushort remainder = 0;
            int bytev;
            byte bit;
            byte msg;
            /*
                * Perform modulo-2 division, a byte at a time.
                */
            for (bytev = 0; bytev < sizeof(uint); bytev++)
            {
                msg = (byte)(valor >> (8 * bytev));
                /*
                    * Bring the next byte into the remainder.
                    */
                remainder ^= (ushort)(msg << ((8 * sizeof(ushort)) - 8));

                /*
                    * Perform modulo-2 division, a bit at a time.
                    */
                for (bit = 8; bit > 0; --bit)
                {
                    /*
                        * Try to divide the current data bit.
                        */
                    if ((remainder & TOPBIT) != 0)
                    {
                        remainder = (ushort)((remainder << 1) ^ POLYNOMIAL);
                    }
                    else
                    {
                        remainder = (ushort)(remainder << 1);
                    }
                }
            }

            /*
                * The final remainder is the CRC result.
                */
            return (remainder);

        }   /* crcSlow() */


        public static ushort licenseKey(uint numSerie)
        {
            //if (WeightScaleBase.isLibraryLoaded())
            //    return crc16((uint)numSerie * 7 / 3 + 54323);
            return 0;
        }

    }
}
