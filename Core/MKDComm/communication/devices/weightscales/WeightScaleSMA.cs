using mkdinfo.communication.media;
using mkdinfo.communication.protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mkdinfo.communication.devices.weightscales
{
    public class WeightScaleSMA : WeightScaleBase
    {
        public override Dictionary<ProtocolSMA1.SMACommands, byte[]> simpleCommands { get { return _simpleCommands; } }
        public override Dictionary<ProtocolSMA1.SMACommands, string> parametrizedCommands { get { return _parametrizedCommands; }}
        public override Dictionary<string, string> extendedSimpleCommands { get {return _extendendSimpleCommands; } }
        public override Dictionary<string, string> extendedParametrizedCommands { get {return _extendedParametrizedCommands; } }

        protected Dictionary<ProtocolSMA1.SMACommands, byte[]> _simpleCommands;
        protected Dictionary<ProtocolSMA1.SMACommands, string> _parametrizedCommands;
        protected Dictionary<string, string> _extendendSimpleCommands;
        protected Dictionary<string, string> _extendedParametrizedCommands;

        protected void initData()
        {
            _simpleCommands = new Dictionary<ProtocolSMA1.SMACommands, byte[]>(){
                                                    {ProtocolSMA1.SMACommands.RequestDisplayWeight, System.Text.Encoding.ASCII.GetBytes("\nW\r")},
                                                    {ProtocolSMA1.SMACommands.RequestScaleToZero,  System.Text.Encoding.ASCII.GetBytes("\nZ\r")},
                                                    {ProtocolSMA1.SMACommands.RepeatDisplayedWeightContinuously,  System.Text.Encoding.ASCII.GetBytes("\nR\r")}
             };
            _parametrizedCommands = new Dictionary<ProtocolSMA1.SMACommands, string>();
            _extendendSimpleCommands = new Dictionary<string, string>();
            _extendedParametrizedCommands = new Dictionary<string, string>();
        }

        public WeightScaleSMA() : base()
        {
            initData();
        }


        public WeightScaleSMA(HALCommMediaBase communicationMedia):base(communicationMedia)
        {
            initData();
        }
    }
}
