namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;
    public partial class PSHU
    {
        public PSHU()
        {
            PSHUDatas = new HashSet<PSHUData>();
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<PSHUData> PSHUDatas { get; set; }
    }
}
