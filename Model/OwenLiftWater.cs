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
            try
            {
                ModbusHelper.IPOWEN = OutReader.Properties.Config.Default.OwenLiftWaterIp;
                var dt = DateTime.Now;
                OwenDose owenDose = null;
                List<CompressorData> cds = new List<CompressorData>();
                List<CompressorCoils> cds_c = new List<CompressorCoils>();
                List<CompressorData_2> cds_2 = new List<CompressorData_2>();

                try
                {
                    var bytesOwen = ModbusHelper.ReadInputRegisters(0, 10);
                    //owenDose = ModbusHelper.GetOwenDose(bytesOwen.GetRange(0, 20), dt, bytesOwen.GetRange(84, 4));
                    cds = ModbusHelper.GetCompressorDatas(bytesOwen.GetRange(0, 10), dt);
                    var bytesOwen_2 = ModbusHelper.ReadInputRegisters_2(0, 10);
                    cds_2 = ModbusHelper.GetCompressorDatas_2(bytesOwen_2.GetRange(0, 10), dt);
                    var bytesCds = ModbusHelper.ReadCoils(0, 10);
                    //cds = ModbusHelper.GetCompressorDatas(bytesCds.GetRange(0, 10), dt);
                    cds_c = ModbusHelper.GetCoilsData(bytesCds.GetRange(0, 10), dt);
                }
                catch (Exception ex)
                {
                    ExceptionMessage += string.Format(" Error Owen Message: {0} \r\n StackTrace:{1}", ex.Message, ex.StackTrace);
                }
                //Присвоение списка дискретных значений к списку аналоговых значений
                foreach (var compressorData in cds)
                {
                    foreach (var compressorCoils in cds_c)
                    {
                        if (compressorData.CompressorId == compressorCoils.CompressorId)
                        {
                            compressorData.IsActive = compressorCoils.IsActive;
                        }
                    }
                }
                var query = "";
                Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
                foreach (var compressorData in cds)
                    query += compressorData.ToSql();
                //foreach (var compressorData in ncds)
                //    query += compressorData.ToSql();
                foreach (var compressorData_2 in cds_2)
                    query += compressorData_2.ToSql();

                if (owenDose != null)
                    query += owenDose.ToSql();
                Query = query;
            }
            catch (Exception ex)
            {
                ExceptionMessage += string.Format(" Error Update Message: {0} \r\n StackTrace:{1}", ex.Message, ex.StackTrace);
            }
        }

        public override string ToString()
        {
            return string.Format("OwenLiftWater - {0}", DateTime.Now);
        }
    }
        }
