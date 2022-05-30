namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;

    public partial class KNSPump
    {
        public KNSPump()
        {
            KNSPumpDatas = new HashSet<KNSPumpData>();
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<KNSPumpData> KNSPumpDatas { get; set; }
    }
}
