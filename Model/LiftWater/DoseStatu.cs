namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;

    public partial class DoseStatu
    {
        public int Id { get; set; }

        public DateTime Sysdate { get; set; }

        public bool IsActive { get; set; }

        public bool IsAlarm { get; set; }

        public bool IsMode1 { get; set; }

        public bool IsMode2 { get; set; }

        public bool IsOpen289 { get; set; }

        public bool IsOpen2811 { get; set; }

        public bool IsLevelNaOCl1 { get; set; }

        public bool IsLevelWater1 { get; set; }

        public bool IsLevelNaOCl2 { get; set; }

        public bool IsLevelwater2 { get; set; }
        public decimal Job1017 { get; set; }
        public decimal Job1014 { get; set; }
        public string ToSql()
        {
            return string.Format("INSERT DoseStatus (Sysdate, IsActive, IsAlarm, IsMode1, IsMode2, IsOpen289, IsOpen2811, IsLevelNaOCl1, IsLevelWater1, IsLevelNaOCl2, IsLevelwater2, Job1017, Job1014)" +
                "VALUES ('{0:yyyyMMdd HH:mm:ss}', {1}, {2}, {3}, {4}, {5}, {6}, {7},{8}, {9}, {10}, {11}, {12});", Sysdate, IsActive ? 1 : 0, IsAlarm ? 1 : 0, IsMode1 ? 1 : 0, IsMode2 ? 1 : 0, IsOpen289 ? 1 : 0, IsOpen2811 ? 1 : 0, IsLevelNaOCl1 ? 1 : 0, IsLevelWater1 ? 1 : 0, IsLevelNaOCl2 ? 1 : 0, IsLevelwater2 ? 1 : 0, Job1017, Job1014);
        }
    }
}
