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
    public class OwenLiftWater : IObject
        {
        public OwenLiftWater()
            {
            }
        public DateTime LastUpdate { get; set; }
        public DateTime LastSaveToScada { get; set; }
        public bool NotConnection { get { return IsExceptionClient; } }
        public bool IsExceptionClient { get; set; }
        public string ExceptionMessage { get; set; }

        private string Query { get; set; }


        public void Run()
            {
            Update();
            if (!string.IsNullOrEmpty(ToString()))
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

        public void Update()
            {
            //        try
            //        {
            //            ModbusHelper.IPOWEN = OutReader.Properties.Config.Default.OwenLiftWaterIp;
            //            var dt = DateTime.Now;
            //            OwenDose owenDose = null;
            //            List<CompressorData> cds = new List<CompressorData>();

            //            //try
            //            //{
            //            //    var bytesOwen = ModbusHelper.ReadInputRegisters(0, 5);
            //            //    bytesOwen = ModbusHelper.ReadInputRegisters(0, 88);
            //            //    owenDose = ModbusHelper.GetOwenDose(bytesOwen.GetRange(0, 20), dt, bytesOwen.GetRange(84, 4));
            //            //    cds = ModbusHelper.GetCompressoDatas(bytesOwen.GetRange(20, 64), dt);
            //            //}
            //            //catch (Exception ex)
            //            //{
            //            //    ExceptionMessage += string.Format(" Error Owen Message: {0} \r\n StackTrace:{1}", ex.Message, ex.StackTrace);
            //            //}
            //            var query = "";
            //            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            //            foreach (var compressorData in cds)
            //                query += compressorData.ToSql();

            //            if (owenDose != null)
            //                query += owenDose.ToSql();
            //            Query = query;
            //        }
            //        catch (Exception ex)
            //        {
            //            ExceptionMessage += string.Format(" Error Update Message: {0} \r\n StackTrace:{1}", ex.Message, ex.StackTrace);
            //        }
            //    }

            //    public override string ToString()
            //    {
            //        return string.Format("OwenLiftWater - {0}", DateTime.Now);
            //    }
            }
        }
    }
