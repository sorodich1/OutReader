namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;

    public partial class WellPumpData
    {
        public int Id { get; set; }

        public DateTime Sysdate { get; set; }

        public int WellPumpId { get; set; }

        public bool IsAuto { get; set; }

        public bool IsDist { get; set; }

        public bool IsActive { get; set; }

        public bool IsAlarm { get; set; }

        public double Speed { get; set; }

        public double Current { get; set; }
        public int OperationTime { get; set; }
        public string Alarm { get; set; }
        public decimal Temp { get; set; }
        public decimal Humidity { get; set; }
        public decimal DewPoint { get; set; }
        public virtual WellPump WellPump { get; set; }
        public string ToSql()
        {
            return string.Format("INSERT INTO WellPumpData (Sysdate, WellPumpId, IsAuto, IsDist, IsActive, IsAlarm, Speed, [Current], OperationTime, Alarm, Temp, Humidity, DewPoint)"+
                "VALUES ('{0:yyyyMMdd HH:mm:ss}', {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, N'{9}', {10}, {11}, {12});",Sysdate, WellPumpId, IsAuto?1:0, IsDist?1:0, IsActive?1:0, IsAlarm?1:0, Speed, Current, OperationTime, Alarm, Temp, Humidity, DewPoint);
        }
    }
}
