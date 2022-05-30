using System;

namespace OutReader.Model.LiftWater
{
    public partial class CompressorStatu
    {
        public int Id { get; set; }

        public decimal Press { get; set; }
        public decimal PressSetPoint { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime Sysdate { get; set; }
        public string ToSql()
        {
            return string.Format("INSERT CompressorStatus (Press, PressSetPoint, CreatedAt, Sysdate)" +
                "VALUES ({0}, {1}, DEFAULT,'{2:yyyyMMdd HH:mm:ss}');", Press, PressSetPoint, Sysdate);
        }
    }
}
