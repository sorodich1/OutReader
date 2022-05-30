//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Net.Sockets;
//using System.Threading;
//using Modbus.Device;
//using OutReader.Helper;
//using OutReader.Properties;

//namespace OutReader.Model
//{
    //public class Merkur:IObject
    //{
//        private readonly int _id;
//        private readonly int _port;
//        private readonly string _ip;
//        private readonly byte _modbusId;

//        public Merkur(int id, byte modbusId, int port, string ip)
//        {
//            _id = id;
//            _modbusId = modbusId;
//            _port = port;
//            _ip = ip;
//        }
//        public int Id
//        {
//            get { return _id; }
//        }
//        public int ModbusId
//        {
//            get { return _modbusId; }
//        }
//        public DateTime LastUpdate { get; set; }
//        public DateTime LastSaveToScada { get; set; }
//        public bool NotConnection { get { return (U == null && I == null && P == null && Energies == null) || IsExceptionClient; } }
//        public bool IsExceptionClient { get; set; }
//        public string ExceptionMessage { get; set; }
//        public List<decimal> U { get; set; }
//        public List<decimal> I { get; set; }
//        public List<decimal> P { get; set; }
//        public List<int> Energies { get; set; }
//        public decimal Fq{ get; set; }
       
//        public void Run()
//        {
//            Update();
//            if (string.IsNullOrEmpty(ExceptionMessage))
//                SaveToSql();
//            SaveLogToSql();
//        }

//        public void SaveToScada()
//        {
//            //try
//            //{
//                //ScadaHelper.SetBrickClient(this);
//            //}
//            //catch (Exception ex)
//            //{
//            //    ExceptionMessage = (string.IsNullOrEmpty(ExceptionMessage) ? "" : "\r\n") + "ErrorScada: " + ex.Message;
//            //}
//            LastSaveToScada = DateTime.Now;
//        }

//        public void SaveToSql()
//        {
//            DbHelper.SetMerkur(this);
//        }

//        public void SaveLogToSql()
//        {
//            if (!string.IsNullOrEmpty(ExceptionMessage))
//                DbHelper.SetLog(ExceptionMessage, this.GetType().Name);
//            DbHelper.SetLog(ToString(), this.GetType().Name, false);
//        }

//        public void ConvertM()
//        {
           
//        }

//        public void Update()
//        {
//            ExceptionMessage = "";
//            //try{
//                using (TcpClient client = new TcpClient())
//                {
//                    client.SendTimeout = 10000;
//                    client.ReceiveTimeout = 10000;
//                    client.Connect(_ip, _port);
//                    //if (MerkurHelper.IsExists(client, _modbusId))
//                    //{
//                    //var i = 0;
//                    //while (i < 6)
//                    //{
//                        if (MerkurHelper.Open(client, _modbusId))
//                        {
//                            //U = MerkurHelper.U(client, _modbusId);
//                            //I = MerkurHelper.I(client, _modbusId);
//                            //P = MerkurHelper.P(client, _id);
//                            Energies = MerkurHelper.Energies(client, _modbusId);
//                            ExceptionMessage = "";
//                            //Fq = MerkurHelper.Fq(client, _modbusId);
//                            //break;
//                            //MerkurHelper.Close(client, _modbusId);
//                        }
//                        else
//                        {
//                            ExceptionMessage = "Не удалось открыть канал связи.";
//                        }
//                        Thread.Sleep(1000);
//                    //    i++;
//                    //}
//                    //}
//                    //else
//                    //{
//                    //    ExceptionMessage = "Счётчик не доступен";
//                    //}

//                    //if(_modbusId == 88)
                       
//                }
//            //}
//            //catch (Exception ex)
//            //{
//            //    IsExceptionClient = true;
//            //    ExceptionMessage = "ErrorUpdateClient: " + ex.Message;
//            //}
//            LastUpdate = DateTime.Now;
//        }
        
//        public override string ToString()
//        {
//            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-GB");
//            var res = string.Format("Merkur: ModbusId:{0}, Port:{1}, {2}, ", _modbusId, _port, LastUpdate);
//            if (U != null && U.Count > 2)
//                res += string.Format("{0:0.00}, {1:0.00}, {2:0.00}, ", U[0], U[1], U[2]);
//            else
//                res += "0,0,0,";
//            if (I != null && I.Count > 2)
//                res += string.Format("{0:0.00}, {1:0.00}, {2:0.00}, ", I[0], I[1], I[2]);
//            else
//                res += "0,0,0,";
//            if (P != null && P.Count > 2)
//                res += string.Format("{0:0.00}, {1:0.00}, {2:0.00}, ", P[0], P[1], P[2]);
//            else
//                res += "0,0,0,";
//            if (Energies != null && Energies.Count > 2)
//                res += string.Format("{0}, {1}, {2},", Energies[0], Energies[1], Energies[2]);
//            else
//                res += "0,0,0,";
//            res += Fq.ToString();
//            return res;
//        }
//    }
//}
