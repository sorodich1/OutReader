namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;

   
    public partial class KNSData
    {
        public int Id { get; set; }

        public DateTime Sysdate { get; set; }

        public bool IsLevelHH { get; set; }

        public bool IsLevelH { get; set; }

        public bool IsLevelL { get; set; }

        public bool IsLevelLL { get; set; }

        public double Level { get; set; }

        public bool IsRemoteAccess { get; set; }

        public bool IsResetAlarmAck { get; set; }

        public bool IsAutoPitAck { get; set; }

        public bool IsInterlockPitAck { get; set; }

        public bool IsCustomRelayPulseAck { get; set; }

        public bool IsPitMode { get; set; }

        public byte[] OperationMode { get; set; }

        public decimal LL { get; set; }
        public decimal L { get; set; }
        public decimal H { get; set; }
        public decimal HH { get; set; }
        public string ToSql()
        {
            return string.Format("INSERT KNSData (Sysdate, IsLevelHH, IsLevelH, IsLevelL, IsLevelLL, Level, IsRemoteAccess, IsResetAlarmAck, IsAutoPitAck, IsInterlockPitAck, IsCustomRelayPulseAck, IsPitMode, OperationMode, LL, L, H, HH)" +
                "VALUES ('{0:yyyyMMdd HH:mm:ss}', {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14},{15},{16});", Sysdate, IsLevelHH ? 1 : 0, IsLevelH ? 1 : 0, IsLevelL ? 1 : 0, IsLevelLL ? 1 : 0, Level, IsRemoteAccess ? 1 : 0, IsResetAlarmAck ? 1 : 0, IsAutoPitAck ? 1 : 0, IsInterlockPitAck ? 1 : 0, IsCustomRelayPulseAck ? 1 : 0, IsPitMode ? 1 : 0, "0", LL, L, H, HH);
        }
    }
}
