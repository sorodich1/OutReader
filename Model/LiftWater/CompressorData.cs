namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;

    public partial class CompressorData
    {
        public int Id { get; set; }

        public int CompressorId { get; set; }

        public DateTime Sysdate { get; set; }

        public bool IsActive { get; set; }

        public int Coils { get; set; }

        public decimal Press { get; set; }

        public decimal Temp { get; set; }

        public decimal TempOutput { get; set; }

        public decimal TempDew { get; set; }

        public int OperationTime { get; set; }

        public int OperationTimeLoad { get; set; }

        public int StartCount { get; set; }

        public decimal FlowAir { get; set; }

        public int FlowEnegry { get; set; }

        public virtual Compressor Compressor { get; set; }
        public bool IsAlarmButton { get; set; }
        public bool IsLinearStarter { get; set; }
        public bool IsStarStarter { get; set; }
        public bool IsTriangleStarter { get; set; }
        public bool IsLoading { get; set; }
        public bool IsDehumidifierMotor { get; set; }
        public int MMS { get; set; }
        public int MCM { get; set; }
        public int GS { get; set; }
        public bool IsAlarmDI { get; set; }
        public bool IsOverloadDI { get; set; }
        public bool IsDrainageDI { get; set; }
        public bool IsRotationProtection { get; set; }
        public bool IsDehumidifierProtaction { get; set; }
        public bool IsAlarmPress { get; set; }
        public int LoadRelay { get; set; }
        public int StartDehumidifier { get; set; }
        public int HoursControl { get; set; }

        public string ToSql()
        {
            return string.Format("INSERT CompressorData (CompressorId, Sysdate, IsActive, Coils, Press, Temp, TempOutput, TempDew, OperationTime, OperationTimeLoad, StartCount, FlowAir, FlowEnegry, IsAlarmButton, IsLinearStarter, IsStarStarter, IsTriangleStarter, IsLoading, IsDehumidifierMotor, MMS, MCM, GS, IsAlarmDI, IsOverloadDI, IsDrainageDI, IsRotationProtection, IsDehumidifierProtaction, IsAlarmPress, LoadRelay, StartDehumidifier, HoursControl)" + //30
                "VALUES ({0}, '{1:yyyyMMdd HH:mm:ss}', {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}, {21}, {22}, {23}, {24}, {25}, {26}, {27}, {28}, {29}, {30});", CompressorId, Sysdate, IsActive ? 1 : 0, Coils, Press, Temp, TempOutput, TempDew, OperationTime, OperationTimeLoad, StartCount, FlowAir, FlowEnegry, IsAlarmButton ? 1 : 0, IsLinearStarter ? 1 : 0, IsStarStarter ? 1 : 0, IsTriangleStarter ? 1 : 0, IsLoading ? 1 : 0, IsDehumidifierMotor ? 1 : 0, MMS, MCM, GS, IsAlarmDI ? 1 : 0, IsOverloadDI ? 1 : 0, IsDrainageDI ? 1 : 0, IsRotationProtection ? 1 : 0, IsDehumidifierProtaction ? 1 : 0, IsAlarmPress ? 1 : 0, LoadRelay, StartDehumidifier, HoursControl);
        }
    }
}
