namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;

    public partial class ReactionChamberData
    {
        public int Id { get; set; }

        public int ReactionChamberId { get; set; }

        public DateTime Sysdate { get; set; }

        public bool IsLevelHH { get; set; }

        public bool IsLevelH { get; set; }

        public bool IsLevelL { get; set; }

        public bool IsLevelLL { get; set; }

        public bool IsSA { get; set; }

        public bool IsBobber { get; set; }

        public double Level { get; set; }

        public virtual ReactionChamber ReactionChamber { get; set; }
        public decimal LL { get; set; }
        public decimal L { get; set; }
        public decimal H { get; set; }
        public decimal HH { get; set; }

        public string ToSql()
        {
            return string.Format("INSERT ReactionChamberData (ReactionChamberId, Sysdate, IsLevelHH, IsLevelH, IsLevelL, IsLevelLL, IsSA, IsBobber, Level, LL, L, H, HH)" +
                "VALUES ({0}, '{1:yyyyMMdd HH:mm:ss}', {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12});", ReactionChamberId, Sysdate, IsLevelHH ? 1 : 0, IsLevelH ? 1 : 0, IsLevelL ? 1 : 0, IsLevelLL ? 1 : 0, IsSA ? 1 : 0, IsBobber ? 1 : 0, Level, LL, L, H, HH);
        }
    }
}
