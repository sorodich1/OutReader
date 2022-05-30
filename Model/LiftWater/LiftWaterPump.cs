namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;

    public partial class LiftWaterPump
    {
        public LiftWaterPump()
        {
            LiftWaterPumpDatas = new HashSet<LiftWaterPumpData>();
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<LiftWaterPumpData> LiftWaterPumpDatas { get; set; }
    }
}
