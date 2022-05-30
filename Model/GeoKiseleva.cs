using Modbus.Device;
using OutReader.Helper;
using OutReader.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OutReader.Model
{
    public class GeoKiseleva:ObjectReader
    {
        public GeoKiseleva(byte mModbusId1 = 5)
        {
            _mModbusId1 = mModbusId1;
            Us = new Us();
        }
        private readonly byte _mModbusId1;
        public Us Us { get; private set; }
        public M M1 { get; set; }
        public bool IsM1 { get { return M1 != null; } }
        public decimal PressIn{get;set;}
        public decimal PressOut{get;set;}
        public decimal PressOut2 { get; set; }
        public override bool NotConnection { get { return M1==null || IsExceptionClient; } }
        public override void SaveToScada()
        {

            try
            {
                if (!IsExceptionClient)
                    ScadaHelper.SetGeoKiselev(this);
            }
            catch (Exception ex)
            {
                ExceptionMessage = (string.IsNullOrEmpty(ExceptionMessage) ? "" : "\r\n") + "ErrorScada: " + ex.Message;
            }
        }
        public override void Run()
        {
            RunWithScada();
            Us.Run();
        }
        public override void Update()
        {            
            ExceptionMessage = "";
            try
            {
                using (TcpClient client = new TcpClient(Config.Default.GeoKiselevaIp, Config.Default.GeoKiselevaPort))
                {
                    client.SendTimeout = 15000;
                    client.ReceiveTimeout = 15000;
                    M1 = ModbusHelper.ReadM(client, _mModbusId1);
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

        public override void ConvertM()
        {
            PressIn = IsM1 ? ConverterHelper.ToPress10(M1.A1) : 0;
            PressOut = IsM1 ? ConverterHelper.ToPress10(M1.A2) : 0;
            PressOut2 = IsM1 ? ConverterHelper.ToPress10(M1.A3) : 0; 
        }
        public override string ToString()
        {
            return string.Format("GeoKiseleva    {0}, M1: {1}, {2}, {3}, {4}", Us, PressIn, PressOut, PressOut2, M1 != null ? M1.DiToString() : "M1 = null");
        }
    }
}
