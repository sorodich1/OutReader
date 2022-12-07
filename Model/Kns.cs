using Modbus.Device;
using OutReader.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace OutReader.Model
{
    public class Kns:ObjectReader
    {
        private readonly int _port;
        private readonly string _ip;
        private readonly string _id;
        private readonly string _title;

        public Kns(int port, string ip, string id)
        {
            _port = port;
            _ip = ip;
            _id = id;
            Devices = new List<KnsDevice>();
            MB16Ds = new List<MB16D>();
            MB8As = new List<MB8A>();
            MB8A_OBEHs = new List<MB8A_OBEH>();
            MB8A_KRESTYs = new List<MB8A_KRESTY>();
            MB8A_GKNSs = new List<MB8A_GKNS>();
            TERs = new List<TER>();
            OBEHs = new List<OBEH>();
            OBEH_2s = new List<OBEH_2>();
            OBEH_3s = new List<OBEH_3>();
            OBEH_VRUs = new List<OBEH_VRU>();
            OBEH_Alarms = new List<OBEH_Alarm>();
            OBEH_levels = new List<OBEH_level>();
            GKNSs = new List<GKNS>();
            GKNS_2s = new List<GKNS_2>();
            GKNS_3s = new List<GKNS_3>();
            GKNS_4s = new List<GKNS_4>();
        }

        public Kns(int port, string ip, string id, string title)
        {
            _port = port;
            _ip = ip;
            _id = id;
            _title = title;
            Devices = new List<KnsDevice>();
            MB16Ds = new List<MB16D>();
            MB8As = new List<MB8A>();
            MB8A_KRESTYs = new List<MB8A_KRESTY>();
            MB8A_GKNSs = new List<MB8A_GKNS>();
            MB8A_OBEHs = new List<MB8A_OBEH>();
            TERs = new List<TER>();
            SBIs = new List<SBI>();
            ME3m = new List<ME3M>();
            OBEHs = new List<OBEH>();
            OBEH_2s = new List<OBEH_2>();
            OBEH_3s = new List<OBEH_3>();
            OBEH_VRUs = new List<OBEH_VRU>();
            OBEH_Alarms = new List<OBEH_Alarm>();
            OBEH_levels = new List<OBEH_level>();
            GKNSs = new List<GKNS>();
            GKNS_2s = new List<GKNS_2>();
            GKNS_3s = new List<GKNS_3>();
            GKNS_4s = new List<GKNS_4>();
        }
        public MB16D MB16D { get; private set; }
        public MB8A MB8A { get; private set; }
        public MB8A_KRESTY MB8A_KRESTY { get; private set; }
        public MB8A_GKNS MB8A_GKNS { get; private set; }
        public MB8A_OBEH MB8A_OBEH { get; private set; }
        public Tn Tn { get; set; }
        public ME3M ME3M { get; set; }
        public OBEH OBEH { get; set; }
        public OBEH_2 OBEH_2 { get; set; }
        public OBEH_3 OBEH_3 { get; set; }
        public OBEH_VRU OBEH_VRU { get; set; }
        public OBEH_Alarm OBEH_Alarm { get; set; }
        public OBEH_level OBEH_level { get; set; }
        public GKNS GKNS { get; set; }
        public GKNS_2 GKNS_2 { get; set; }
        public GKNS_3 GKNS_3 { get; set; }
        public GKNS_4 GKNS_4 { get; set; }

        public string KNSId { get { return _id; } }
        public List<KnsDevice> Devices { get; set; }
        public List<MB16D> MB16Ds { get; private set; }
        public List<MB8A> MB8As { get; private set; }
        public List<MB8A_KRESTY> MB8A_KRESTYs { get; private set; }
        public List<MB8A_GKNS> MB8A_GKNSs { get; private set; }
        public List<MB8A_OBEH> MB8A_OBEHs { get; private set; }
        public List<TER> TERs { get; private set; }
        public List<SBI> SBIs { get; private set; }
        public List<ME3M> ME3m { get; private set; }
        public List<OBEH> OBEHs { get; private set; }
        public List<OBEH_2> OBEH_2s { get; private set; }
        public List<OBEH_3> OBEH_3s { get; private set; }
        public List<OBEH_VRU> OBEH_VRUs { get; private set; }
        public List<OBEH_Alarm> OBEH_Alarms { get; private set; }
        public List<OBEH_level> OBEH_levels { get; private set; }
        public List<GKNS> GKNSs { get; private set; }
        public List<GKNS_2> GKNS_2s { get; private set; }
        public List<GKNS_3> GKNS_3s { get; private set; }
        public List<GKNS_4> GKNS_4s { get; private set; }

        public override bool NotConnection { get { return MB16D==null || IsExceptionClient; } }

        public override void SaveToSql ()
        {
            if (MB8As.Count > 0 || MB16Ds.Count > 0 || OBEHs.Count > 0 || OBEH_2s.Count > 0 || OBEH_3s.Count > 0 || OBEH_VRUs.Count > 0 || OBEH_Alarms.Count > 0 || OBEH_levels.Count > 0 || GKNSs.Count > 0 || GKNS_2s.Count > 0 || GKNS_3s.Count > 0 || GKNS_4s.Count > 0)
            {

                DbHelper.SetKns ( this );
            }
            //else
            //{
            //    DbHelper.SetKNSAlehinaAlarm ( _id );
            //}
        }

        public override void SaveLogToSql()
        {
            if (!string.IsNullOrEmpty(ExceptionMessage))
                DbHelper.SetLog(ExceptionMessage, this.GetType().Name);
            DbHelper.SetLog(ToString(), this.GetType().Name, false, _id);
        }

        public override void Update()
        {
            ExceptionMessage = "";
            LastUpdate = DateTime.Now;
            using (TcpClient client = new TcpClient(_ip, _port))
            {
                client.SendTimeout = 10000;
                client.ReceiveTimeout = 10000;
                foreach (var device in Devices)
                {
                    try
                    {
                        if (device.IsMB16D)
                        {
                            var mb16d = ModbusHelper.ReadM16D(client, device.ModbusId);
                            if (mb16d != null) MB16Ds.Add(mb16d);
                        }
                        else if (device.IsOBEH) //true
                        {
                            var obeh = ModbusHelper.ReadOBEH(client, device.ModbusId);
                            if (obeh != null)
                            {
                                OBEHs.Add(obeh);
                            }
                        }
                        else if (device.IsOBEH_2) //true
                        {
                            var obeh_2 = ModbusHelper.ReadOBEH_2(client, device.ModbusId);
                            if (obeh_2 != null)
                            {
                                OBEH_2s.Add(obeh_2);
                            }
                        }
                        else if (device.IsOBEH_3) //true
                        {
                            var obeh_3 = ModbusHelper.ReadOBEH_3(client, device.ModbusId);
                            if (obeh_3 != null)
                            {
                                OBEH_3s.Add(obeh_3);
                            }
                        }
                        else if (device.IsOBEH_VRU) //true
                        {
                            var obeh_vru = ModbusHelper.ReadOBEH_VRU(client, device.ModbusId);
                            if (obeh_vru != null)
                            {
                                OBEH_VRUs.Add(obeh_vru);
                            }
                        }
                        else if (device.IsOBEH_Alarm) //true
                        {
                            var obeh_alarm = ModbusHelper.ReadOBEH_Alarm(client, device.ModbusId);
                            if (obeh_alarm != null)
                            {
                                OBEH_Alarms.Add(obeh_alarm);
                            }
                        }
                        else if (device.IsOBEH_level) //true
                        {
                            var obeh_level = ModbusHelper.ReadOBEH_level(client, device.ModbusId);
                            if (obeh_level != null)
                            {
                                OBEH_levels.Add(obeh_level);
                            }
                        }
                        else if (device.IsGKNS) //true
                        {
                            var gkns = ModbusHelper.ReadGKNS(client, device.ModbusId);
                            if (gkns != null)
                            {
                                GKNSs.Add(gkns);
                            }
                        }
                        else if (device.IsGKNS_2) //true
                        {
                            var gkns_2 = ModbusHelper.ReadGKNS_2(client, device.ModbusId);
                            if (gkns_2 != null)
                            {
                                GKNS_2s.Add(gkns_2);
                            }
                        }
                        else if (device.IsGKNS_3) //true
                        {
                            var gkns_3 = ModbusHelper.ReadGKNS_3(client, device.ModbusId);
                            if (gkns_3 != null)
                            {
                                GKNS_3s.Add(gkns_3);
                            }
                        }
                        else if (device.IsGKNS_4) //true
                        {
                            var gkns_4 = ModbusHelper.ReadGKNS_4(client, device.ModbusId);
                            if (gkns_4 != null)
                            {
                                GKNS_4s.Add(gkns_4);
                            }
                        }
                        else if (device.IsMB8A)
                        {
                            var mb8a = ModbusHelper.ReadM8A(client, device.ModbusId);
                            if (mb8a != null)
                            {
                                MB8As.Add(mb8a);
                            }
                        }
                        else if (device.IsMB8A_KRESTY)
                        {
                            var mb8a_kresty = ModbusHelper.ReadMB8A_KRESTY(client, device.ModbusId);
                            if (mb8a_kresty != null)
                            {
                                MB8A_KRESTYs.Add(mb8a_kresty);
                            }
                        }
                        else if (device.IsMB8A_GKNS)
                        {
                            var mb8a_gkns = ModbusHelper.ReadMB8A_GKNS(client, device.ModbusId);
                            if (mb8a_gkns != null)
                            {
                                MB8A_GKNSs.Add(mb8a_gkns);
                            }
                        }
                        else if (device.IsMB8A_OBEH)
                        {
                            var mb8a_obeh = ModbusHelper.ReadMB8A_OBEH(client, device.ModbusId);
                            if (mb8a_obeh != null)
                            {
                                MB8A_OBEHs.Add(mb8a_obeh);
                            }
                        }
                        else if (device.IsTER)
                        {
                            var ter = ModbusHelper.ReadTER(client, device.ModbusId);
                            if (ter != null) TERs.Add(ter);
                        }
                        else if (device.IsMR)
                        {
                            var ter = ModbusHelper.ReadMR(client, device.ModbusId);
                            if (ter != null) TERs.Add(ter);
                        }
                        else if (device.IsSBI)
                        {
                            var sbi = ModbusHelper.ReadSBI(client, device.ModbusId, KNSId, LastUpdate);
                            if (sbi != null) SBIs.Add(sbi);
                        }
                        else if (device.IsTn4017)
                        {
                            var tn = ModbusHelper.ReadTn(client, (byte)device.ModbusId);
                            if (tn != null) Tn = tn;
                        }
                        else if (device.IsMe3M)
                            {
                            var me3m = ModbusHelper.ReadME3M(client, (byte)device.ModbusId);
                            if (ME3m != null)
                                ME3m.Add(me3m);
                            ME3M = me3m;
                            }
                    }
                    catch (Exception ex)
                    {
                        //IsExceptionClient = true;
                        ExceptionMessage = "ErrorUpdate: " + device.Name + " Id:" + device.ModbusId + " Message:" +
                                           ex.Message;
                    }
                }
            }
            
            if (SBIs != null && SBIs.Count > 0)
            {
                for(var i=0; i<SBIs.Count; i++)
                {
                    if (MB16Ds != null && MB16Ds.Count > 0)
                    {
                        MB16Ds[0].DI[8 + i] = SBIs[i].IsRun;
                        MB16Ds[0].DI[11 + i] = SBIs[i].IsError;
                    }
                    if (MB8As != null && MB8As.Count > 0)
                    {
                        if(i==0) MB8As[0].A1 = SBIs[i].I;
                        else if (i == 1) MB8As[0].A2 = SBIs[i].I;
                        else if (i == 2) MB8As[0].A3 = SBIs[i].I;
                    }
                }
            }
            
        }

        public override void Run()
        {
            base.Run();
            if (string.IsNullOrEmpty(ExceptionMessage))
            {
                SaveToScada();
            }

        }
        public override void SaveToScada()
        {
            //base.SaveToScada();
            if (MB8As.Count > 0 && (KNSId == "KNS21" || KNSId == "KNS03" || KNSId == "KNS06" || KNSId == "KNS14" || KNSId == "KNS08" || KNSId == "KNS07" || KNSId == "KNS11" || KNSId == "Unit_5249" || KNSId == "KNS04" || KNSId == "KNS19" || KNSId == "KNS23" || KNSId == "RNS") && string.IsNullOrEmpty(ExceptionMessage))
            {
                ScadaHelper.SetKnsPress(MB8As[0].A5, KNSId, Tn);
            }
            else if (KNSId == "KNS18" && string.IsNullOrEmpty(ExceptionMessage))
            {
                ScadaHelper.SetKnsPress(MB8A_OBEHs[0].A5, KNSId, Tn);
            }
        }

        public override string ToString()
        {
            var res = string.Format("KNS: {0} {1} -  MB16Ds: {2} ", KNSId, LastUpdate, string.Join(" ", MB16Ds));

            if (MB8As != null && MB8As.Count > 0)
                res += string.Format("MB8As:({0}) ", string.Join(" ", MB8As));
            if (MB8A_KRESTYs != null && MB8A_KRESTYs.Count > 0)
                res += string.Format("MB8A_KRESTYs:({0}) ", string.Join(" ", MB8A_KRESTYs));
            if (MB8A_GKNSs != null && MB8A_GKNSs.Count > 0)
                res += string.Format("MB8A_GKNSs:({0}) ", string.Join(" ", MB8A_GKNSs));
            if (MB8A_OBEHs != null && MB8A_OBEHs.Count > 0)
                res += string.Format("MB8A_OBEHs:({0}) ", string.Join(" ", MB8A_OBEHs));
            if (OBEHs != null && OBEHs.Count > 0)
                res += string.Format("OBEH:({0}) ", string.Join(" ", OBEHs));
            if (OBEH_2s != null && OBEH_2s.Count > 0)
                res += string.Format("OBEH_2:({0}) ", string.Join(" ", OBEH_2s));
            if (OBEH_3s != null && OBEH_3s.Count > 0)
                res += string.Format("OBEH_3:({0}) ", string.Join(" ", OBEH_3s));
            if (OBEH_VRUs != null && OBEH_VRUs.Count > 0)
                res += string.Format("OBEH_VRU:({0}) ", string.Join(" ", OBEH_VRUs));
            if (OBEH_Alarms != null && OBEH_Alarms.Count > 0)
                res += string.Format("OBEH_Alarm:({0}) ", string.Join(" ", OBEH_Alarms));
            if (OBEH_levels != null && OBEH_levels.Count > 0)
                res += string.Format("OBEH_level:({0}) ", string.Join(" ", OBEH_levels));
            if (GKNSs != null && GKNSs.Count > 0)
                res += string.Format("GKNS:({0}) ", string.Join(" ", GKNSs));
            if (GKNS_2s != null && GKNS_2s.Count > 0)
                res += string.Format("GKNS_2:({0}) ", string.Join(" ", GKNS_2s));
            if (GKNS_3s != null && GKNS_3s.Count > 0)
                res += string.Format("GKNS_3:({0}) ", string.Join(" ", GKNS_3s));
            if (GKNS_4s != null && GKNS_4s.Count > 0)
                res += string.Format("GKNS_4:({0}) ", string.Join(" ", GKNS_4s));

            if (TERs != null && TERs.Count > 0)
                res += string.Format("TERs:({0}) ", string.Join(" ", TERs));

            if (SBIs != null && SBIs.Count > 0)
                res += string.Format("SBIs:({0}) ", string.Join(" ", SBIs));

            if (Tn != null )
                res += string.Format("Tn:({0}) ", Tn);

            if (ME3m != null && ME3m.Count > 0)
                {
                res += string.Format("ME3m: ({0}) ", string.Join(" ", ME3m));
                }

            if (!string.IsNullOrEmpty(ExceptionMessage))
                res += "\r\n" + ExceptionMessage;
            return res;
        }
    }
}
