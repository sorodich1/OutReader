namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;

    public partial class ReactionChamber
    {
        public ReactionChamber()
        {
            ReactionChamberDatas = new HashSet<ReactionChamberData>();
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<ReactionChamberData> ReactionChamberDatas { get; set; }
    }
}
