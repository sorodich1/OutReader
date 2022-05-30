namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;

    public partial class Well
    {

        public Well()
        {
            WellDatas = new HashSet<WellData>();
            WellPumps = new HashSet<WellPump>();
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<WellData> WellDatas { get; set; }

        public virtual ICollection<WellPump> WellPumps { get; set; }
    }
}
