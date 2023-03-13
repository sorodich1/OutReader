using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OutReader.Model.LiftWater;
using System.Threading;
using OutReader.Helper;
using ModbusHelper = OutReader.Model.LiftWater.ModbusHelper;

namespace OutReader.Model
{
    public class LW : IObject
    {
        public LW()
        {
            Wells = new List<Well>();
        }
        public DateTime LastUpdate { get; set; }
        public DateTime LastSaveToScada { get; set; }
        public bool NotConnection { get { return  IsExceptionClient; } }
        public bool IsExceptionClient { get; set; }
        public string ExceptionMessage { get; set; }

        public List<Well> Wells { get; set; }
        public List<ReactionChamber> Chambers { get; set; }
        public List<FlowMeter> FlowMeters { get; set; }
        public List<LiftWaterPump> StationPumps { get; set; }
        public Tank Tank { get; set; }
        public LiftWaterStatu LiftWaterStatus { get; set; }
        public DoseStatu DoseStatus { get; set; }
        public List<Alarm> Alarms { get; set; }
        public List<Dos> Doses { get; set; }
        public KNSData Kns { get; set; }
        public List<KNSPump> KnsPumps { get; set; }
        public List<Valve> Valves { get; set; }
        public List<PSHU> Pshus { get; set; }
        public List<PSHU_NEW> Pshu_news { get; set; }
        //public List<cds> cds { get; set; }
    private string Query { get; set; }


        public void Run()
        {
            Update();
            SaveToSql();
            SaveLogToSql();
        }

        public void SaveToScada()
        {
            throw new NotImplementedException();
        }

        public void SaveToSql()
        {
            DbHelper.SetQueryLiftWater(Query);
        }

        public void SaveLogToSql()
        {
            if (!string.IsNullOrEmpty(ExceptionMessage))
                DbHelper.SetLog(ExceptionMessage, this.GetType().Name);
            DbHelper.SetLog(ToString(), this.GetType().Name, false);
        }

        public void ConvertM()
        {
            throw new NotImplementedException();
        }

        public void Update ()
        {
            try
            {
                //WinSCPHelper.Download();
                ModbusHelper.IP = OutReader.Properties.Config.Default.LiftWaterIp;
                ModbusHelper.IPOWEN = OutReader.Properties.Config.Default.OwenLiftWaterIp;
                var ipEnergo = "192.168.22.149";
                var dt = DateTime.Now;

                var bytes54 = ModbusHelper.ReadRegisters ( 0, 54 );
                Wells = ModbusHelper.GetWells ( bytes54.GetRange ( 0, 28 ), dt );
                Chambers = ModbusHelper.GetReactionChambers ( bytes54.GetRange ( 28, 5 ), dt );
                Tank = ModbusHelper.GetTank ( bytes54.GetRange ( 32, 2 ), dt );
                FlowMeters = ModbusHelper.GetFlowMeters ( bytes54.GetRange ( 35, 17 ), dt );
                Thread.Sleep ( 700 );
                var bytes55_100 = ModbusHelper.ReadRegisters ( 52, 50 );
                LiftWaterStatus = ModbusHelper.GetLiftWaterStatus ( bytes55_100.GetRange ( 0, 5 ), dt );
                StationPumps = ModbusHelper.GetLiftWaterPumps ( bytes55_100.GetRange ( 5, 16 ), dt );
                Kns = ModbusHelper.GetKNS ( bytes55_100.GetRange ( 21, 3 ), dt );
                KnsPumps = ModbusHelper.GetKNSPumps ( bytes55_100.GetRange ( 24, 15 ), dt );
                Valves = ModbusHelper.GetValves ( bytes55_100.GetRange ( 39, 10 ), dt );
                Thread.Sleep ( 700 );
                var bytes101_151 = ModbusHelper.ReadRegisters ( 101, 55 );
                Pshus = ModbusHelper.GetPSHUs(bytes101_151.GetRange(0, 6), dt);
                DoseStatus = ModbusHelper.GetDoseStatus ( bytes101_151.GetRange ( 12, 1 ), dt );
                Doses = ModbusHelper.GetDoses ( bytes101_151.GetRange ( 13, 42 ), dt );
                //var bytesOwen = ModbusHelper.ReadInputRegisters2(0, 10);
                ////owenDose = ModbusHelper.GetOwenDose(bytesOwen.GetRange(0, 20), dt, bytesOwen.GetRange(84, 4));
                //cds= ModbusHelper.GetCompressoDatas(bytesOwen.GetRange(0, 10), dt);

                Thread.Sleep ( 700 );

                var bytes211 = ModbusHelper.ReadRegisters(210, 1);
                Pshu_news = ModbusHelper.GetPSHU_NEWs(bytes211.GetRange(0, 1), dt);
                var bytes154 = ModbusHelper.ReadRegisters ( 154, 48 );
                Alarms = ModbusHelper.GetAlarms ( bytes154.GetRange ( 0, 2 ), DateTime.Now );
                ModbusHelper.GetDoseJobM3 ( bytes154.GetRange ( 6, 2 ), DoseStatus );
                ModbusHelper.GetReactionChambersLevels ( bytes154.GetRange ( 8, 8 ), Chambers );
                ModbusHelper.GetTankLevels ( bytes154.GetRange ( 16, 4 ), Tank );
                ModbusHelper.GetKNSLevels ( bytes154.GetRange ( 20, 4 ), Kns );
                var alarms2 = ModbusHelper.GetAlarms2 ( bytes154.GetRange ( 24, 2 ), DateTime.Now );
                Alarms.AddRange ( alarms2 );
                ModbusHelper.GetLiftWaterPressSetPoint ( bytes154 [26], LiftWaterStatus );
                var compressorStatus = ModbusHelper.GetCompressorStatus ( bytes154.GetRange ( 27, 2 ), dt );
                //40184-40185
                ModbusHelper.GetChlorine ( LiftWaterStatus, bytes154.GetRange ( 29, 2 ) );
                //Pump29.1 AlarmCod            - 40186,40187,40188,40189
                //Pump29.2 AlarmCod            - 40190,40191,40192,40193
                //Pump29.3 AlarmCod            - 40194,40195,40196,40197
                ModbusHelper.GetAlarmsKNSPumps ( KnsPumps, bytes154.GetRange ( 31, 12 ) );
                ModbusHelper.GetAlarmsLiftWaterPumps ( StationPumps, bytes154.GetRange ( 43, 4 ) );
                //Pump12.2.1 AlarmCod             -40198
                //Pump12.2.2 AlarmCod             -40199
                //Pump12.2.3 AlarmCod             -40200
                //Pump12.2.4 AlarmCod             -40201
                //Аварийно высокий ост. хлор(х100)     -40202

                Thread.Sleep ( 700 );
                var bytes203 = ModbusHelper.ReadRegisters ( 203, 7 );
                ModbusHelper.GetTempWellPumpData ( Wells, bytes203.GetRange ( 0, 7 ) );
                try
                {
                    //PromEnergo
                    ModbusHelper.IP = ipEnergo;
                    var bEnergoPressSet = ModbusHelper.ReadRegisters(32, 2);
                    var bEnergoData = ModbusHelper.ReadRegisters(2059, 7);
                    var bEnergoAlarm = ModbusHelper.ReadRegisters(1200, 3);
                    Thread.Sleep(200);
                    var bEnergoWarning = ModbusHelper.ReadRegisters(1300, 2);
                    var bEnergoStatePump = ModbusHelper.ReadRegisters(2000, 8);
                    var lf2 = ModbusHelper.GetLiftWaterPumpsPromEnergo(bEnergoData, bEnergoStatePump, bEnergoAlarm,
                        bEnergoWarning, dt);
                    StationPumps.AddRange(lf2);
                    LiftWaterStatus.IsActive = StationPumps.Any(x => x.LiftWaterPumpDatas.Any(z => z.IsActive));
                    LiftWaterStatus.IsAlarm = StationPumps.All(x => x.LiftWaterPumpDatas.Any(z => z.IsPumpAlarm));
                    LiftWaterStatus.IsWarning = LiftWaterStatus.IsWarning || StationPumps.Any(x =>
                                            x.LiftWaterPumpDatas.Any(
                                                z => z.WarningCode != null && z.WarningCode.Any(p => Convert.ToInt32(p) > 0)));
                    LiftWaterStatus.PressSetPoint = LiftWater.ConverterHelper.ByteToReal(bEnergoPressSet[1], bEnergoPressSet[0]);
                    LiftWaterStatus.IsAccessMode = StationPumps.Any(x => x.LiftWaterPumpDatas.Any(z => z.IsAccessMode));
                }
                catch (Exception ex) { ExceptionMessage += DateTime.Now.ToString("G") + "- не доступен " + ipEnergo + "!!!"; }

                Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
                var query = "";
                foreach (var well in Wells)
                {
                    foreach (var wellData in well.WellDatas)
                        query += wellData.ToSql ();
                    foreach (var wellPump in well.WellPumps)
                    {
                        foreach (var wellPumpData in wellPump.WellPumpDatas)
                            query += wellPumpData.ToSql ();
                    }
                }
                foreach (var reactionChamber in Chambers)
                {
                    foreach (var reactionChamberData in reactionChamber.ReactionChamberDatas)
                    {
                        query += reactionChamberData.ToSql ();
                    }
                }
                foreach (var tankData in Tank.TankDatas)
                {
                    query += tankData.ToSql ();
                }
                foreach (var flowMeter in FlowMeters)
                {
                    foreach (var flowMeterData in flowMeter.FlowMeterDatas)
                    {
                        query += flowMeterData.ToSql ();
                    }
                }
                query += LiftWaterStatus.ToSql ();
                foreach (var liftWaterPump in StationPumps)
                {
                    foreach (var liftWaterPumpData in liftWaterPump.LiftWaterPumpDatas)
                    {
                        query += liftWaterPumpData.ToSql ();
                    }
                }

                //lw.SaveChanges();
                query += Kns.ToSql ();
                foreach (var knsPump in KnsPumps)
                {
                    foreach (var knsPumpData in knsPump.KNSPumpDatas)
                    {
                        query += knsPumpData.ToSql ();
                    }
                }
                foreach (var valve in Valves)
                {
                    foreach (var valveData in valve.ValveDatas)
                    {
                        query += valveData.ToSql ();
                    }
                }
                foreach (var pshu in Pshus)
                {
                    foreach (var pshuData in pshu.PSHUDatas)
                    {
                        query += pshuData.ToSql ();
                    }
                }
                foreach (var pshu_new in Pshu_news)
                {
                    foreach (var pshuData_new in pshu_new.PSHUData_news)
                    {
                        query += pshuData_new.ToSql();
                    }
                }
                query += DoseStatus.ToSql ();
                foreach (var dose in Doses)
                {
                    foreach (var doseData in dose.DoseDatas)
                    {
                        query += doseData.ToSql ();
                    }
                }
                //query += new DataUpdate() { CreatedAt = DateTime.Now }.ToSql();
                //foreach (var compressorData in cds)
                //{
                //    query += compressorData.ToSql();
                //}


                //alarms
                foreach (var alarm in Alarms)
                {
                    query += alarm.ToSql ();
                }
                query += compressorStatus.ToSql ();
                //if (owenDose != null)
                //    query += owenDose.ToSql();
                Query = query;
            }
            catch (Exception ex)
            {
                ExceptionMessage += string.Format ( " Error Update Message: {0} \r\n StackTrace:{1}", ex.Message, ex.StackTrace );
            }
        }

        public override string ToString ()
        {
            return string.Format ( "LiftWater - {0}", DateTime.Now );
        }
    }
}
