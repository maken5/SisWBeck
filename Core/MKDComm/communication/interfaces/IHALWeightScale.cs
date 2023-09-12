using mkdinfo.communication.devices.weightscales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mkdinfo.communication.protocol;

namespace mkdinfo.communication.interfaces
{
    public interface IHALWeightScale
    {
        List<ProtocolSMA1.SMACommands> getSupportedCommands();

        bool start();
        bool stop();

        bool RequestDisplayWeight();
        bool RequestHighResolutionWeight();
        bool RequestDisplayedWeightAfterStability();
        bool RequestHighResolutionWeightAfterStability();
        bool RequestScaleToZero();
        bool RequestScaleToTare();
        bool SetScaleTareWeight(float tare);
        bool ReturnTareWeight();
        bool ClearScaleTareWeight();
        bool ChangeUnitsOfMeasureToggleOrScroll();
        bool SetUnitsOfMeasure(string unit);
        bool InvokeScaleDiagnostics();
        bool AboutScaleFirstLine();
        bool AboutScaleScroll();
        bool ScaleInformation();
        bool ScaleInformationScroll();
        bool AbortCommand();
        bool RepeatDisplayedWeightContinuously();
        bool RepeatHighResolutionWeightContinuously();
        bool ExtendedCommandSet(string commandAndParameters);
        bool sendExtendedCommand(string command, object[] parameters);
    
    }
}
