namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;

    public partial class KNSPumpData
    {
        public int Id { get; set; }

        public int KNSPumpId { get; set; }

        public DateTime Sysdate { get; set; }

        public bool IsPresent { get; set; }

        public bool IsRunning { get; set; }

        public bool IsAlarm { get; set; }

        public bool IsCommFault { get; set; }

        public byte[] OperatingMode { get; set; }

        public int Current { get; set; }

        public int Frequency { get; set; }

        public decimal RunTime { get; set; }
        public string Alarm { get; set; }

        public virtual KNSPump KNSPump { get; set; }
        public string ToSql()
        {
            return string.Format("INSERT KNSPumpData (KNSPumpId, Sysdate, IsPresent, IsRunning, IsAlarm, IsCommFault, OperatingMode, [Current], Frequency, RunTime, Alarm)" +
                "VALUES ({0}, '{1:yyyyMMdd HH:mm:ss}', {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, N'{10}');", KNSPumpId, Sysdate, IsPresent? 1 : 0, IsRunning? 1 : 0, IsAlarm? 1 : 0, IsCommFault? 1 : 0, "0", Current, Frequency, RunTime, Alarm);
        }
    }
}
