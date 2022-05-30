namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public partial class LiftWaterStatu
    {
        public LiftWaterStatu()
        {
            ControlMode  = new byte[2];
            OperationMode = new byte[2];
        }
        public int Id { get; set; }

        public DateTime Sysdate { get; set; }

        public bool IsAccessMode { get; set; }

        public bool IsActive { get; set; }

        public bool IsAlarm { get; set; }

        public bool IsWarning { get; set; }

        public bool IsPumpAct { get; set; }

        public bool IsSpeedMin { get; set; }

        public bool IsStandBy { get; set; }

        public bool IsSpeedMax { get; set; }

        public bool IsResetAlarmAck { get; set; }

        public bool IsSetpointAct { get; set; }

        public bool IsPowerMax { get; set; }

        public bool IsRotation { get; set; }

        public bool IsDirection { get; set; }

        public int Feedback { get; set; }
        
        public byte[] ControlMode { get; set; }

        public byte[] OperationMode { get; set; }

        public double Press { get; set; }
        public double PressSetPoint { get; set; }
        public double Chlorine { get; set; }
        public double Temp { get; set; }
        public string ToSql()
        {
            return string.Format("INSERT LiftWaterStatus (Sysdate, IsAccessMode, IsActive, IsAlarm, IsWarning, IsPumpAct, IsSpeedMin, IsStandBy, IsSpeedMax, IsResetAlarmAck, IsSetpointAct, IsPowerMax, IsRotation, IsDirection, Feedback, ControlMode, OperationMode, Press, PressSetPoint, Chlorine, Temp)"+
                "VALUES ('{0:yyyyMMdd HH:mm:ss}', {1}, {2}, {3}, {4}, {5}, {6}, {7},{8}, {9}, {10}, {11}, {12}, {13}, {14},{15},{16}, {17}, {18}, {19}, {20});", Sysdate, IsAccessMode ? 1 : 0, IsActive ? 1 : 0, IsAlarm ? 1 : 0, IsWarning ? 1 : 0, IsPumpAct ? 1 : 0, IsSpeedMin ? 1 : 0, IsStandBy ? 1 : 0, IsSpeedMax ? 1 : 0, IsResetAlarmAck ? 1 : 0, IsSetpointAct ? 1 : 0, IsPowerMax ? 1 : 0, IsRotation ? 1 : 0, IsDirection ? 1 : 0, Feedback, /*String.Join(String.Empty, Array.ConvertAll(ControlMode, x => x.ToString("X2")))*/"0", "0", Press, PressSetPoint, Chlorine, Temp);
        }
    }
}
