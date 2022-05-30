namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;

    public partial class FlowMeter
    {
       public FlowMeter()
        {
            FlowMeterDatas = new HashSet<FlowMeterData>();
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }

        
        public virtual ICollection<FlowMeterData> FlowMeterDatas { get; set; }
    }
}
