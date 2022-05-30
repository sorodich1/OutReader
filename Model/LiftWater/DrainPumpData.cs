namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;

    public partial class DrainPumpData
    {
        public int Id { get; set; }

        public int DrainPumpId { get; set; }

        public DateTime Sysdate { get; set; }

        public bool IsAuto { get; set; }

        public bool IsDist { get; set; }

        public bool IsActive { get; set; }

        public bool IsAlarm { get; set; }

        public bool IsLevelLL { get; set; }

        public int OperaionTime { get; set; }

        public virtual DrainPump DrainPump { get; set; }
    }
}
