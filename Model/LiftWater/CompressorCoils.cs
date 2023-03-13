namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;

    public partial class CompressorCoils
    {
        public int Id { get; set; }
        public int CompressorId { get; set; }

        public bool IsActive { get; set; }

        public bool IsAlarmButton { get; set; }
        public bool IsLinearStarter { get; set; }
        public bool IsStarStarter { get; set; }
        public bool IsTriangleStarter { get; set; }
        public bool IsLoading { get; set; }
        public bool IsDehumidifierMotor { get; set; }
        public bool IsAlarmDI { get; set; }
        public bool IsOverloadDI { get; set; }
        public bool IsDrainageDI { get; set; }
        public bool IsRotationProtection { get; set; }
        public bool IsDehumidifierProtaction { get; set; }
        public bool IsAlarmPress { get; set; }        
    }
}
