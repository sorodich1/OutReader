namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;

    public partial class FlowMeterData
    {
        public int Id { get; set; }

        public DateTime Sysdate { get; set; }

        public int FlowMeterId { get; set; }

        public bool IsConnected { get; set; }

        public float Intake { get; set; }

        public float Total { get; set; }

        public virtual FlowMeter FlowMeter { get; set; }
        public string ToSql()
        {
            return string.Format("INSERT FlowMeterData (Sysdate, FlowMeterId, IsConnected, Intake, Total)" +
                "VALUES ('{0:yyyyMMdd HH:mm:ss}', {1}, {2}, {3}, {4});", Sysdate, FlowMeterId, IsConnected ? 1 : 0, Intake, Total);
        }
    }
}
