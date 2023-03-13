namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;
    public partial class PSHU_NEW
    {
        public PSHU_NEW()
        {
            PSHUData_news = new HashSet<PSHUData_new>();
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<PSHUData_new> PSHUData_news { get; set; }
    }
}
