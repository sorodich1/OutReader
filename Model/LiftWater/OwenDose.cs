namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;
    public partial class OwenDose
    {
        
        public int Id { get; set; }

        public DateTime Sysdate { get; set; }
        public decimal V1Cur { get; set; }
        public decimal V2Cur { get; set; }
        public decimal H1Cur { get; set; }
        public decimal H2Cur { get; set; }
        public decimal CanFillV1 { get; set; }
        public decimal CanFillV2 { get; set; }
        public decimal Flow1 { get; set; }
        public decimal Flow2 { get; set; }
        public int FillTime1 { get; set; }
        public int FillTime2 { get; set; }
        public int MY8 { get; set; }
        public int MB16 { get; set; }
        public decimal V1CurNew { get; set; }
        public decimal V2CurNew { get; set; }
        public string ToSql()
        {
            return string.Format("INSERT OwenDoses (Sysdate, V1Cur, V2Cur, H1Cur, H2Cur, CanFillV1, CanFillV2, Flow1, Flow2, FillTime1, FillTime2, MY8, MB16, V1CurNew, V2CurNew)" +
                "VALUES ('{0:yyyyMMdd HH:mm:ss}', {1}, {2}, {3}, {4}, {5}, {6}, {7},{8}, {9}, {10}, {11}, {12}, {13},{14});", Sysdate, V1Cur, V2Cur, H1Cur, H2Cur, CanFillV1, CanFillV2, Flow1, Flow2, FillTime1, FillTime2, MY8, MB16, V1CurNew, V2CurNew);
        }
    }
}
