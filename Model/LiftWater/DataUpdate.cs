namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;

    public partial class DataUpdate
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }
        public string ToSql()
        {
            return string.Format("INSERT DataUpdates (CreatedAt)" +
                "VALUES ('{0:yyyyMMdd HH:mm:ss}');", CreatedAt);
        }
    }
}
