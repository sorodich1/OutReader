using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Modbus.Device;
using Modbus.IO;
using OutReader.Helper;
using OutReader.Properties;
using ScxV6DbClient;

namespace OutReader.Model
{
    public class Elizarovo:IObject
    {
        private readonly byte _mModbusId1;
        private readonly byte _mModbusId2;
        private readonly byte _tnModbusId;
        public Elizarovo(byte tnModbusId = 1, byte mModbusId1 = 3, byte mModbusId2 = 5)
        {
            _mModbusId1 = mModbusId1;
            _mModbusId2 = mModbusId2;
            _tnModbusId = tnModbusId;
        }

        public DateTime LastUpdate { get; set; }
        public bool NotConnection { get { return (!IsM1 && !IsM2 && !IsTn && !IsPulsar) || IsExceptionClient; } }
        public bool IsExceptionClient { get; set; }
        public string ExceptionMessage { get; set; }
        public DateTime LastSaveToScada { get; set; }
        public Tn Tn { get; set; }
        public M M1 { get; set; }
        public M M2 { get; set; }
        public Pulsar Pulsar { get; set; }
        public S1200 S1200 { get; set; }
        public double PressIn { get; set; }
        public double PressOut { get; set; }
        public double Fq1 { get; set; }
        public double Fq2 { get; set; }
        public double Fq3 { get; set; }
        public double Fq4 { get; set; }
        public double Fq5 { get; set; }
        public double Fq6 { get; set; }

        public bool IsM1 { get { return M1 != null; } }
        public bool IsM2 { get { return M2 != null; } }
        public bool IsTn { get { return Tn != null; } }
        public bool IsPulsar { get { return Pulsar != null; } }


        public void SaveToSql()
        {
            throw new NotImplementedException();
        }

        public void SaveLogToSql()
        {
            if (!string.IsNullOrEmpty(ExceptionMessage))
                DbHelper.SetLog(ExceptionMessage, this.GetType().Name);
            DbHelper.SetLog(ToString(), this.GetType().Name, false);
        }

        public void ConvertM()
        {
            PressIn = IsM1 ? ConverterHelper.ToPress6(M1.A2) : 0;
            PressOut = IsM1 ? ConverterHelper.ToPress6(M1.A1) : 0;
            Fq1 = IsM2 && M2.A4 > 0 ? Convert.ToDouble(M2.A4 * 5m)*100 : 0;
            Fq2 = IsM2 && M2.A3 > 0 ? Convert.ToDouble(M2.A3 * 5m)*100 : 0;
            Fq3 = IsM2 && M2.A2 > 0 ? Convert.ToDouble(M2.A2 * 5m)*100 : 0;
            Fq4 = IsM2 && M2.A1 > 0 ? Convert.ToDouble(M2.A1 * 5m)*100 : 0;
            Fq5 = IsM1 && M1.A4 > 0 ? Convert.ToDouble(M1.A4 * 5m)*100 : 0;
            Fq6 = IsM1 && M1.A3 > 0 ? Convert.ToDouble(M1.A3 * 5m)*100 : 0;
        }

        public void Update()
        {
            Pulsar = ModbusHelper.ReadPulsar(Config.Default.ElizarovoPort);
            ExceptionMessage = "";
            try
            {
                using (TcpClient client = new TcpClient(ModbusHelper.IP, Config.Default.ElizarovoPort))
                {
                    client.SendTimeout = 4000;
                    client.ReceiveTimeout = 8000;
                    ModbusIpMaster master = ModbusIpMaster.CreateIp(client);
                    master.Transport.Retries = 5;
                    master.Transport.WaitToRetryMilliseconds = 2000;
                    Tn = ModbusHelper.ReadTn(master, _tnModbusId);
                    M1 = ModbusHelper.ReadM(master, _mModbusId1);
                    M2 = ModbusHelper.ReadM(master, _mModbusId2);
                    ConvertM();
                    IsExceptionClient = false;
                }
            }
            catch (Exception ex)
            {
                IsExceptionClient = true;
                ExceptionMessage = "ErrorUpdateClient: " + ex.Message;
            }
            LastUpdate = DateTime.Now;
        }

        public void Run()
        {
            Update();
            SaveToScada();
            SaveLogToSql();
        }

        public void SaveToScada()
        {
            try
            {
                ScadaHelper.SetElizarovoCLient(this);
            }
            catch (Exception ex)
            {
                ExceptionMessage = (string.IsNullOrEmpty(ExceptionMessage) ? "" : "\r\n") + "ErrorScada: " + ex.Message;
            }
            LastSaveToScada = DateTime.Now;
        }

        public override string ToString()
        {
            var res = string.Format("Elizarovo: {0},{1},", LastUpdate, LastSaveToScada);
            var isP = Pulsar != null;
            //Pulsar
            res += string.Format(" Flow1 = {0:0.00}, Flow2 = {1:0.00},", isP ? Pulsar.Flow1 : 0, isP ? Pulsar.Flow2 : 0);
            //M2
            res += string.Format(" Fq1 = {0:0.00}, Fq2 = {1:0.00}, Fq3 = {2:0.00}, Fq4 = {3:0.00}, DI={4},", Fq1, Fq2, Fq3, Fq4, IsM2 ? M2.DiToString() : "");
            //M1
            res += string.Format(" Fq5 = {0:0.00}, Fq6 = {1:0.00}, PressIn = {2:0.00}, PressOut = {3:0.00}, DI={4},", Fq5, Fq6, PressIn, PressOut, IsM1 ? M1.DiToString() : "");
            //Tn
            res += string.Format(" U1 = {0:0.00}, U2 = {1:0.00}, U3 = {2:0.00}, DI = {3}", IsTn ? Tn.Au1 : 0, IsTn ? Tn.Au2 : 0, IsTn ? Tn.Au3 : 0, IsTn ? Tn.DiToString() : "");
            return res;
        }
    }
}
