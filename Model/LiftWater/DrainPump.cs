namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;

    public partial class DrainPump
    {
        public DrainPump()
        {
            DrainPumpDatas = new HashSet<DrainPumpData>();
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<DrainPumpData> DrainPumpDatas { get; set; }
    }
}
