namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;
    public partial class WellPump
    {
        public WellPump()
        {
            WellPumpDatas = new HashSet<WellPumpData>();
        }

        public int Id { get; set; }

        public int WellId { get; set; }

        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<WellPumpData> WellPumpDatas { get; set; }

        public virtual Well Well { get; set; }
    }
}
