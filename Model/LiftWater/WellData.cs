namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;

    public partial class WellData
    {
        public int Id { get; set; }

        public int WellId { get; set; }

        public int Level { get; set; }

        public double PressCamera { get; set; }

        public double Press { get; set; }

        public float FlowWater { get; set; }

        public float FlowWaterTotal { get; set; }

        public DateTime Sysdate { get; set; }

        public bool IsUps { get; set; }

        public bool IsDoorOpen { get; set; }

        public bool IsAuto { get; set; }

        public bool IsLevel { get; set; }

        public bool IsPumpLink { get; set; }

        public bool IsFlowWaterLink { get; set; }

        public bool IsWellLink { get; set; }

        public virtual Well Well { get; set; }

        public string ToSql()
        {
            return string.Format("INSERT INTO WellData (WellId, Level, PressCamera, Press, FlowWater, FlowWaterTotal, Sysdate, IsUps, IsDoorOpen, IsAuto, IsLevel, IsPumpLink, IsFlowWaterLink, IsWellLink)" +
                "VALUES ({0}, {1}, {2}, {3}, {4}, {5}, '{6:yyyyMMdd HH:mm:ss}', {7}, {8}, {9}, {10}, {11}, {12}, {13});", WellId, Level, PressCamera, Press, FlowWater, FlowWaterTotal, Sysdate, IsUps ? 1 : 0, IsDoorOpen ? 1 : 0, IsAuto ? 1 : 0, IsLevel ? 1 : 0, IsPumpLink ? 1 : 0, IsFlowWaterLink ? 1 : 0, IsWellLink ? 1 : 0);
        }
    }
}
