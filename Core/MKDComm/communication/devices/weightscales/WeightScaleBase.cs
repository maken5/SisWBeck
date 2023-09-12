using mkdinfo.communication.interfaces;
using mkdinfo.communication.media;
using mkdinfo.communication.protocol;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace mkdinfo.communication.devices.weightscales
{

    public abstract class WeightScaleBase : IHALWeightScale, IDisposable
    {

        #region static

        //private static bool _loaded = false;

        public static string getCommandDescription(ProtocolSMA1.SMACommands command)
        {
            FieldInfo fi = command.GetType().GetField(command.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            return null;
        }

        //public static bool isLibraryLoaded()
        //{
        //    return _loaded;
        //}

        //public static void load(string loadCode)
        //{

        //    string h = Process.GetCurrentProcess().ProcessName.Replace(".vshost", "") + "3242311";

        //    if (loadCode == calculateCr(h))
        //    {
        //        _loaded = true;
        //    }

            
        //}

        public static string code()
        {
            //System.AppDomain.CurrentDomain.scar
            //return Process.GetCurrentProcess().ProcessName.Replace(".vshost", "") + "3242311";
            return null;
        }

        private static string calculateCr(string f)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(f);
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
       
        #endregion

        #region declarations
        public delegate void OnResponse(ResponseProtocolBase rp);
        public delegate void OnError(Exception ex);
        public OnResponse onResponse = null;
        public OnError onError = null;

        #endregion
        
        #region protected
        protected int resolution = 1;
        protected ProtocolBase prot = null;
        protected HALCommMediaBase med = null;
        protected float ultimoPesoRecebido = Int32.MinValue;
       
        #endregion

        #region public attributes

        public abstract Dictionary<ProtocolSMA1.SMACommands, byte[]> simpleCommands { get; }
        public abstract Dictionary<ProtocolSMA1.SMACommands, string> parametrizedCommands { get; }
        public abstract Dictionary<string, string> extendedSimpleCommands { get; }
        public abstract Dictionary<string, string> extendedParametrizedCommands { get; }

        public ProtocolBase protocol
        {
            get { return prot; }
            set
            {
                //if (!WeightScaleBase.isLibraryLoaded())
                //    return;
                if (prot != null)
                {
                    try
                    {
                        prot.onNewResponse -= new mkdInfo.communication.interfaces.OnNewResponse(onNewResponse);
                        prot.onError -= new mkdInfo.communication.interfaces.OnError(onErrorReceived);
                    }
                    catch { }
                    prot = null;
                }
                prot = value;
                if (prot != null)
                {
                    prot.onError += new mkdInfo.communication.interfaces.OnError(onErrorReceived);
                    prot.onNewResponse += new mkdInfo.communication.interfaces.OnNewResponse(onNewResponse);
                    if (med != null)
                        prot.media = med;
                }
            }
        }

        public HALCommMediaBase media
        {
            get { return med; }
            set
            {
                med = value;
                if (prot!=null)
                    prot.media = value;
            }
        }

        #endregion

        #region class

        public WeightScaleBase()
        {

            //if (!WeightScaleBase.isLibraryLoaded())
            //    throw new Exception();
        }

        public WeightScaleBase(HALCommMediaBase communicationMedia)
            : this()
        {

            //if (!WeightScaleBase.isLibraryLoaded())
            //    throw new Exception();
            media = communicationMedia;
        }

        #endregion

        #region protected
        protected bool sendCommand(ProtocolSMA1.SMACommands cmd)
        {
            if (simpleCommands.ContainsKey(cmd))
            {
                byte[] dataToSend = simpleCommands[cmd];
                if (dataToSend != null)
                    protocol.send(dataToSend);
            }
            return false;
        }

        protected bool sendParammetrizedCommand(ProtocolSMA1.SMACommands cmd, object[] parameters)
        {
            if (parametrizedCommands.ContainsKey(cmd))
            {
                string formatString = parametrizedCommands[cmd];
                if (formatString != null)
                {
                    string dataToSend = String.Format(formatString, parameters);
                    protocol.send(System.Text.Encoding.ASCII.GetBytes(dataToSend));
                    return true;
                }
            }
            return false;
        }

        protected virtual bool onNewResponseProcess(ResponseProtocolBase rp)
        {
            return true;
        }

        protected void onNewResponse(ResponseProtocolBase rp)
        {

            if (onResponse != null && rp != null)
            {
                if (onNewResponseProcess(rp))
                    onResponse(rp);
            }
        }
        protected void onErrorReceived(Exception ex)
        {
            if (onError != null)
                onError(ex);
        }
        #endregion

        #region interface
        public virtual List<ProtocolSMA1.SMACommands> getSupportedCommands()
        {
            return simpleCommands.Keys.ToList<ProtocolSMA1.SMACommands>();
        }
        public virtual bool start()
        {
            if (prot != null)
                prot.start();
            //return sendCommand(ProtocolSMA1.SMACommands.RepeatDisplayedWeightContinuously);
            return true;
        }
        public virtual bool stop()
        {
            //sendCommand(ProtocolSMA1.SMACommands.AbortCommand);
            if (protocol != null)
                protocol.stop();
            return protocol != null;
        }
        public virtual bool RequestScaleToZero()
        {
            return sendCommand(ProtocolSMA1.SMACommands.RequestScaleToZero);
        }
        public virtual bool RequestScaleToTare()
        {
            return sendCommand(ProtocolSMA1.SMACommands.RequestScaleToTare);
        }
        public virtual bool RequestDisplayWeight()
        {
            return sendCommand(ProtocolSMA1.SMACommands.RequestDisplayWeight);
        }
        public virtual bool RequestHighResolutionWeight()
        {
            return sendCommand(ProtocolSMA1.SMACommands.RequestHighResolutionWeight);
        }
        public virtual bool RequestDisplayedWeightAfterStability()
        {
            return sendCommand(ProtocolSMA1.SMACommands.RequestDisplayedWeightAfterStability);
        }
        public virtual bool RequestHighResolutionWeightAfterStability()
        {
            return sendCommand(ProtocolSMA1.SMACommands.RequestHighResolutionWeightAfterStability);
        }
        public virtual bool ReturnTareWeight()
        {
            return sendCommand(ProtocolSMA1.SMACommands.ReturnTareWeight);
        }
        public virtual bool ClearScaleTareWeight()
        {
            return sendCommand(ProtocolSMA1.SMACommands.ClearScaleTareWeight);
        }
        public virtual bool ChangeUnitsOfMeasureToggleOrScroll()
        {
            return sendCommand(ProtocolSMA1.SMACommands.ChangeUnitsOfMeasureToggleOrScroll);
        }
        public virtual bool InvokeScaleDiagnostics()
        {
            return sendCommand(ProtocolSMA1.SMACommands.InvokeScaleDiagnostics);
        }
        public virtual bool AboutScaleFirstLine()
        {
            return sendCommand(ProtocolSMA1.SMACommands.AboutScaleFirstLine);
        }
        public virtual bool AboutScaleScroll()
        {
            return sendCommand(ProtocolSMA1.SMACommands.AboutScaleScroll);
        }
        public virtual bool ScaleInformation()
        {
            return sendCommand(ProtocolSMA1.SMACommands.ScaleInformation);
        }
        public virtual bool ScaleInformationScroll()
        {
            return sendCommand(ProtocolSMA1.SMACommands.ScaleInformationScroll);
        }
        public virtual bool SetUnitsOfMeasure(string unit)
        {
            return sendParammetrizedCommand(ProtocolSMA1.SMACommands.SetUnitsOfMeasure, new object[] { unit });
        }
        public virtual bool SetScaleTareWeight(float tare)
        {
            return sendParammetrizedCommand(ProtocolSMA1.SMACommands.SetScaleTareWeight, new object[] { tare });
        }
        public virtual bool AbortCommand()
        {
            return sendCommand(ProtocolSMA1.SMACommands.AbortCommand);
        }
        public virtual bool RepeatDisplayedWeightContinuously()
        {
            return sendCommand(ProtocolSMA1.SMACommands.RepeatDisplayedWeightContinuously);
        }
        public virtual bool RepeatHighResolutionWeightContinuously()
        {
            return sendCommand(ProtocolSMA1.SMACommands.RepeatHighResolutionWeightContinuously);
        }
        public virtual bool ExtendedCommandSet(string commandAndParameters)
        {   
            if (protocol != null && commandAndParameters!=null)
            {
                string commandToSend = String.Format("\nX{0}\r", commandAndParameters);
                protocol.send(System.Text.Encoding.ASCII.GetBytes(commandToSend));
                return true;
            }
            return false;
        }

        public virtual void sendNullData()
        {
            if (protocol != null)
                protocol.send(new byte[1] { 0 });
        }

        public virtual bool sendExtendedCommand(string command, object[] parameters = null)
        {
            if (protocol != null)
            {
                if (parameters == null || parameters.Length < 1)
                {
                    string dataToSend;
                    if (extendedSimpleCommands.TryGetValue(command, out dataToSend))
                    {
                        protocol.send(System.Text.Encoding.ASCII.GetBytes(dataToSend));
                        return true;
                    }
                }
                else
                {
                    string formatString;
                    if (extendedParametrizedCommands.TryGetValue(command, out formatString))
                    {
                        string dataToSend = String.Format(formatString, parameters);
                        protocol.send(System.Text.Encoding.ASCII.GetBytes(dataToSend));
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual void Dispose()
        {
            if (protocol != null)
            {
                try
                {
                    protocol.stop();
                }
                catch { }
                protocol.media = null;
            }
            if (media != null)
                media.Dispose();
        }

        #endregion


        
    }
}
