namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;

    public partial class ValveData
    {
        public int Id { get; set; }

        public int ValveId { get; set; }

        public DateTime Sysdate { get; set; }

        public bool IsOpen { get; set; }

        public bool IsClosed { get; set; }

        public bool IsAlarm { get; set; }

        public bool IsAuto { get; set; }

        public bool IsNotOpenAlarm { get; set; }

        public bool IsNotClosedAlarm { get; set; }

        public virtual Valve Valve { get; set; }
        public string ToSql()
        {
            return string.Format("INSERT ValveData (ValveId, Sysdate, IsOpen, IsClosed, IsAlarm, IsAuto, IsNotOpenAlarm, IsNotClosedAlarm)" +
                "VALUES ({0}, '{1:yyyyMMdd HH:mm:ss}', {2}, {3}, {4}, {5}, {6}, {7});", ValveId, Sysdate, IsOpen ? 1 : 0, IsClosed ? 1 : 0, IsAlarm ? 1 : 0, IsAuto ? 1 : 0, IsNotOpenAlarm ? 1 : 0, IsNotClosedAlarm ? 1 : 0);
        }
    }
}
