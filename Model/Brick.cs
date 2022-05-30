using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using Modbus.Device;
using OutReader.Helper;

namespace OutReader.Model
{
    public class Brick:IObject
    {
        private readonly byte _mModbusId1;
        private readonly byte _mModbusId2;
        private readonly byte _tnModbusId;
        private readonly byte _apbModbusId;
        public Brick(byte tnModbusId = 7, byte mModbusId1 = 3, byte mModbusId2 = 5, byte apbModbusId = 11)
        {
            _mModbusId1 = mModbusId1;
            _mModbusId2 = mModbusId2;
            _tnModbusId = tnModbusId;
            _apbModbusId = apbModbusId;
            Mass1 = -1;
        }

        public DateTime LastUpdate { get; set; }
        public DateTime LastSaveToScada { get; set; }
        public bool NotConnection { get { return (!IsM1 && !IsM2 && !IsTn && !IsPulsar) || IsExceptionClient; } }
        public bool IsExceptionClient { get; set; }
        public string ExceptionMessage { get; set; }
        /// <summary>
        /// DI
        /// d1 - работа упс (0 - вкл)
        /// </summary>
        public Tn Tn { get; set; }
        /// <summary>
        /// Analog
        /// a1 - Манометр 3-1 (in)
        /// a2 - Манометр 3-2 (out) 
        /// a3 - Манометр 2-2
        /// a4 - Манометр 2-1
        /// DI
        /// d1 - дверь (1 - закрыта)
        /// d2 - питание/авария в шкафу ЩУНС колодка X3 (0 - авария питания)
        /// d3 - питание/авария в шкафу ЩУРС реле K1 (0 - авария питания)
        /// d4 - Насос 1 Вкл (0 - вкл)
        /// d5 - Насос 1 авария (0 - авария)
        /// d6 - насос 2 вкл (0 - вкл)
        /// d7 - Насос 2 авария (0 - авария)
        /// d8 - Промывочный насос вкл (0 - вкл)
        /// </summary>
        public M M1 { get; set; }
        /// <summary>
        /// Analog
        /// a1 - Манометр 1-1 (in)
        /// a2 - Манометр 1-2 (out) 
        /// a3 - входное давление станции (6 бар макс)
        /// a4 - выходное давление станции (6 бар макс)
        /// DI
        /// d1 - Сухой ход (??)
        /// d2 - Неизвестный сигнал
        /// d3 - 
        /// d4 - РЧВ (низкий)
        /// d5 - РЧВ (средний)
        /// d6 - РЧВ (высокий)
        /// d7 - 
        /// d8 - 
        /// </summary>
        public M M2 { get; set; }
        public Pulsar Pulsar { get; set; }
        public Apb Apb { get; set; }
        public PR200 PR200 { get; set; }
        public ROC ROC { get; set; }
        public SI30 Si1 { get; set; }
        public SI30 Si2 { get; set; }
        public bool IsM1 { get { return M1 != null; } }
        public bool IsM2 { get { return M2 != null; } }
        public bool IsTn { get { return Tn != null; } }
        public bool IsApb { get { return Apb != null; } }
        public bool IsPulsar { get { return Pulsar != null; } }
        public bool IsMass1 { get { return Mass1 != -1; } }
        public bool IsPR200 { get { return PR200 != null; } }
        public bool IsROC { get { return ROC !=null; } }
        public bool IsSi1 { get { return Si1 != null; } }
        public bool IsSi2 { get { return Si2 != null; } }
        public double PressIn3 { get; set; }
        public double PressOut3 { get; set; }
        public double PressIn2 { get; set; }
        public double PressOut2 { get; set; }
        public double PressIn1 { get; set; }
        public double PressOut1 { get; set; }
        public double PressIn { get; set; }
        public double PressOut { get; set; }
        /// <summary>
        /// Масса гипохлорита
        /// </summary>
        public decimal Mass1 { get; set; }
        /// <summary>
        /// Масса коагулянта для осмоса
        /// </summary>
        public decimal Mass2 { get; set; }

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
                ScadaHelper.SetBrickClient(this);
            }
            catch (Exception ex)
            {
                ExceptionMessage = (string.IsNullOrEmpty(ExceptionMessage) ? "" : "\r\n") + "ErrorScada: " + ex.Message;
            }
            LastSaveToScada = DateTime.Now;
        }

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
            PressIn3 = IsM1 ? ConverterHelper.ToPress6(M1.A1) : 0;
            PressOut3 = IsM1 ? ConverterHelper.ToPress6(M1.A2) : 0;
            //PressIn2 = IsM1 ? ConverterHelper.ToPress6(M1.A4) : 0;
            //PressOut2 = IsM1 ? ConverterHelper.ToPress6(M1.A3) : 0;
            PressIn1 = IsM2 ? ConverterHelper.ToPress6(M2.A1) : 0;
            PressOut1 = IsM2 ? ConverterHelper.ToPress6(M2.A2) : 0;
            PressIn = IsM1 ? ConverterHelper.ToPress6(M1.A3) : 0;
            PressOut = IsM1 ? ConverterHelper.ToPress6(M1.A4) : 0;
        }

        public void Update()
        {
            ExceptionMessage = "";
            //Pulsar = ModbusHelper.ReadPulsar(ModbusHelper.BRICK_PORT, true);
            IsExceptionClient = false;
            try
            {
                using (TcpClient client = new TcpClient(ModbusHelper.IP, ModbusHelper.BRICK_PORT))
                {
                    client.SendTimeout = 4000;
                    client.ReceiveTimeout = 8000;
                    ModbusIpMaster master = ModbusIpMaster.CreateIp(client);
                    master.Transport.Retries = 5;
                    master.Transport.WaitToRetryMilliseconds = 2000;
                    Apb = ModbusHelper.ReadApbAll(master, _apbModbusId);
                    Tn = ModbusHelper.ReadTn(master, _tnModbusId);
                    M1 = ModbusHelper.ReadM(master, _mModbusId1);
                    M2 = ModbusHelper.ReadM(master, _mModbusId2);

                    ConvertM();
                }
            }
            catch (Exception ex)
            {
                IsExceptionClient = true;
                ExceptionMessage = "ErrorUpdateClient: " + ex.Message;
            }
            try
            {
                using (TcpClient client = new TcpClient("192.168.8.99", 21708))
                {
                    client.SendTimeout = 4000;
                    client.ReceiveTimeout = 8000;
                    Si1 = ModbusHelper.ReadSI30(client, 26);
                    Si2 = ModbusHelper.ReadSI30(client, 27);
                    PR200 = ModbusHelper.ReadPR200(client, 20);
                    

                }
                Thread.Sleep(1000);
                using (TcpClient client = new TcpClient("192.168.8.99", 21708))
                {
                    client.SendTimeout = 4000;
                    client.ReceiveTimeout = 8000;
                    Mass1 = ModbusHelper.ReadMass(client);


                }
            }
            catch (Exception ex)
            {
                IsExceptionClient = true;
                ExceptionMessage = "ErrorUpdateClient: " + ex.Message;
            }
            LastUpdate = DateTime.Now;
        }
        
        public override string ToString()
        {
            var res = string.Format("Brick: {0},{1},", LastUpdate, LastSaveToScada);
            //Pulsar
            //res += string.Format(" Flow1 = {0:0.00}, Flow2 = {1:0.00},", isP ? Pulsar.Flow1 : 0, isP ? Pulsar.Flow2 : 0);
            //M1
            res += string.Format(" PIn2 = {0:0.00}, POut2 = {1:0.00}, POut3 = {2:0.00}, PIn3 = {3:0.00}, DI = {4},", PressIn2, PressOut2, PressOut3, PressIn3, IsM1 ? M1.DiToString() : "");
            //M2
            res += string.Format(" PressOut = {0:0.00}, PressIn = {1:0.00}, POut2 = {2:0.00}, PIn1 = {3:0.00}, DI = {4},", PressOut, PressIn, PressOut2, PressIn2, IsM2 ? M2.DiToString() : "");
            //Tn
            res += string.Format(" U1 = {0:0.00}, U2 = {1:0.00}, U3 = {2:0.00}, DI = {3}", IsTn ? Tn.Au1 : 0, IsTn ? Tn.Au2 : 0, IsTn ? Tn.Au3 : 0, IsTn ? Tn.DiToString() : "");
            //Apb
            res += string.Format(" PR200:{1}  Mass1={2}  Si1:{3}  Si2: {4} Apb = {0}", IsApb ? Apb.AllToString() : "", PR200,Mass1, Si1, Si2);

            if(!string.IsNullOrEmpty(ExceptionMessage))
                res += "\r\n" + ExceptionMessage;
            return res;
        }
    }
}
