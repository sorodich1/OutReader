namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;

    public partial class PSHUData
    {
        public int Id { get; set; }

        public DateTime Sysdate { get; set; }

        public int PSHUId { get; set; }

        public bool IsOpen { get; set; }

        public bool IsClosed { get; set; }

        public bool IsPress { get; set; }

        public bool IsAuto { get; set; }

        public virtual PSHU PSHU { get; set; }

        public string ToSql()
        {
            return string.Format("INSERT PSHUData (Sysdate, PSHUId, IsOpen, IsClosed, IsPress, IsAuto)" +
                "VALUES ('{0:yyyyMMdd HH:mm:ss}', {1}, {2}, {3}, {4}, {5});", Sysdate, PSHUId, IsOpen ? 1 : 0, IsClosed ? 1 : 0, IsPress ? 1 : 0, IsAuto ? 1 : 0);
        }
    }
}
