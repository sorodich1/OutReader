namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    public partial class LiftWaterPumpData
    {
        public LiftWaterPumpData()
        {
            AlarmCode = new byte[2];
            WarningCode = new byte[2];
            StateCode = new byte[2];
        }
        public int Id { get; set; }

        public DateTime Sysdate { get; set; }

        public int PumpId { get; set; }

        public bool IsAccessMode { get; set; }
        public bool IsCommFault { get; set; }

        public bool IsActive { get; set; }

        public bool IsPumpAlarm { get; set; }

        public byte[] AlarmCode { get; set; }

        public decimal OperationTime { get; set; }

        public int Speed { get; set; }
        public string Alarm { get; set; }

        public decimal Press { get; set; }
        public byte[] WarningCode { get; set; }
        public byte[] StateCode { get; set; }
        public virtual LiftWaterPump LiftWaterPump { get; set; }
        public string ToSql()
        {
            return string.Format("INSERT LiftWaterPumpData (Sysdate, PumpId, IsAccessMode, IsActive, IsPumpAlarm, AlarmCode, OperationTime, Speed, IsCommFault, Alarm, Press, WarningCode, StateCode)" +
                "VALUES ('{0:yyyyMMdd HH:mm:ss}', {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, N'{9}', {10}, {11}, {12});", Sysdate, PumpId, IsAccessMode ? 1 : 0, IsActive ? 1 : 0, IsPumpAlarm ? 1 : 0, "0", OperationTime, Speed, IsCommFault ? 1 : 0, Alarm, Press, "0", "0");
        }
    }
}
