namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;

    public partial class DoseData
    {
        public int Id { get; set; }

        public int DoseId { get; set; }

        public DateTime Sysdate { get; set; }

        public bool IsControllerAct { get; set; }

        public bool IsActive { get; set; }

        public bool IsDist { get; set; }

        public bool IsAlarm { get; set; }

        public bool IsAuto { get; set; }

        public bool IsOff { get; set; }

        public bool IsCF { get; set; }

        public bool IsMembrF { get; set; }

        public bool IsLL { get; set; }

        public bool IsLLL { get; set; }

        public bool IsHolo { get; set; }

        public bool IsHC { get; set; }

        public bool IsPmax5 { get; set; }

        public bool IsPmax { get; set; }

        public float Deb { get; set; }

        public float DebSP { get; set; }

        public float DebSum { get; set; }

        public int OperationTime { get; set; }

        public virtual Dos Dos { get; set; }
        public string ToSql()
        {
            return string.Format("INSERT DoseData (DoseId, Sysdate, IsControllerAct, IsActive, IsDist, IsAlarm, IsAuto, IsOff, IsCF, IsMembrF, IsLL, IsLLL, IsHolo, IsHC, IsPmax5, IsPmax, Deb, DebSP, DebSum, OperationTime) " +
                "VALUES ({0}, '{1:yyyyMMdd HH:mm:ss}', {2}, {3}, {4}, {5}, {6}, {7},{8}, {9}, {10}, {11}, {12}, {13}, {14},{15},{16}, {17}, {18}, {19});", DoseId, Sysdate, IsControllerAct? 1 : 0, IsActive? 1 : 0, IsDist? 1 : 0, IsAlarm? 1 : 0, IsAuto? 1 : 0, IsOff? 1 : 0, IsCF? 1 : 0, IsMembrF? 1 : 0, IsLL? 1 : 0, IsLLL? 1 : 0, IsHolo? 1 : 0, IsHC? 1 : 0, IsPmax5? 1 : 0, IsPmax? 1 : 0, Deb, DebSP, DebSum, OperationTime);
        }
    }
}
