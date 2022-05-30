﻿using Modbus.Device;
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
            TERs = new List<TER>();
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
            TERs = new List<TER>();
            SBIs = new List<SBI>();
            ME3m = new List<ME3M>();
        }
        public MB16D MB16D { get; private set; }
        public MB8A MB8A { get; private set; }
        public Tn Tn { get; set; }
        public ME3M ME3M { get; set; }
        public string KNSId { get { return _id; } }
        public List<KnsDevice> Devices { get; set; }
        public List<MB16D> MB16Ds { get; private set; }
        public List<MB8A> MB8As { get; private set; }
        public List<TER> TERs { get; private set; }
        public List<SBI> SBIs { get; private set; }
        public List<ME3M> ME3m { get; private set; }
        public override bool NotConnection { get { return MB16D==null || IsExceptionClient; } }

        public override void SaveToSql ()
        {
            if (MB8As.Count > 0 || MB16Ds.Count > 0)
            {

                DbHelper.SetKns ( this );
            }
            else
            {
                DbHelper.SetKNSAlehinaAlarm ( _id );
            }
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
                        else if (device.IsMB8A)
                        {
                            var mb8a = ModbusHelper.ReadM8A(client, device.ModbusId);
                            if (mb8a != null)
                            {
                                MB8As.Add(mb8a);
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
        }

        public override string ToString()
        {
            var res = string.Format("KNS: {0} {1} -  MB16Ds: {2} ", KNSId, LastUpdate, string.Join(" ", MB16Ds));

            if (MB8As != null && MB8As.Count > 0)
                res += string.Format("MB8As:({0}) ", string.Join(" ", MB8As));

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
