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
    public class Us:IObject
    {
        
        public DateTime LastUpdate { get; set; }

        public bool NotConnection
        {
            get { return IsExceptionClient; }
        }

        public bool IsExceptionClient { get; set; }

        public string ExceptionMessage { get; set; }
        public double FW1 { get; set; }
        public double FW2 { get; set; }
        public double FWIndex1 { get; set; }
        public double FWIndex2 { get; set; }

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
                ScadaHelper.SetUsCLient(this);
            }
            catch (Exception ex)
            {
                ExceptionMessage = (string.IsNullOrEmpty(ExceptionMessage) ? "" : "\r\n") + "ErrorScada: " + ex.Message;
            }
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
            throw new NotImplementedException();
        }

        public void Update()
        {
            ExceptionMessage = "";
            //IsExceptionClient = false;
            try
            {
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.Connect(Config.Default.GeoKiselevaIp, Config.Default.GeoKiselevaPort);
                sock.ReceiveTimeout = 45000;
                var fData = new float[6];
                byte[] resCmd = new byte[30];
                for (int i = 0; i < 6; i++)
                {
                    try
                    {
                        //var id = __id > 9 ? __id.ToString() : "0" + __id.ToString();
                        var crc = Encoding.Default.GetBytes("#")[0] + Encoding.Default.GetBytes("0")[0] +
                                  Encoding.Default.GetBytes(3.ToString())[0] + Encoding.Default.GetBytes(i.ToString())[0];
                        var t = string.Format("#03{0}{1:X}\r", i, crc);
                        var reqCmd = Encoding.ASCII.GetBytes(t);
                        sock.Send(reqCmd);
                        sock.Receive(resCmd, resCmd.Length, System.Net.Sockets.SocketFlags.None);

                        var str = Encoding.ASCII.GetString(resCmd);
                        str = str.Substring(1, str.IndexOf("\r") > 8 ? 7 : str.IndexOf("\r") - 1).Replace('.', ',');
                        if (string.IsNullOrEmpty(str))
                            str = "0";
                        float val = float.Parse(str);

                        if (i == 3 || i == 5)
                        {
                            val = fData[i - 1] * 10000 + val * 0.1f;
                            fData[i - 1] = val;
                            if (i == 5)
                            {
                                fData[3] = val;
                                fData[4] = 0;
                            }
                        }
                        else
                            fData[i] = val;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                        ExceptionMessage = "ErrorUpdateClient: " + ex.Message;
                    }
                }
                sock.Close();
                FW1 = fData[0];
                FWIndex1 = fData[2];
            }
            catch (Exception ex) { 
                Console.WriteLine(ex.StackTrace);
                ExceptionMessage = "ErrorUpdateClient: " + ex.Message;
                //IsExceptionClient = true;
            }
            try
            {
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.Connect(Config.Default.GeoKiselevaIp, Config.Default.GeoKiselevaPort);
                sock.ReceiveTimeout = 45000;
                var fData = new float[6];
                byte[] resCmd = new byte[30];
                for (int i = 0; i < 6; i++)
                {
                    try
                    {
                        //var id = __id > 9 ? __id.ToString() : "0" + __id.ToString();
                        var crc = Encoding.Default.GetBytes("#")[0] + Encoding.Default.GetBytes("0")[0] +
                                  Encoding.Default.GetBytes(2.ToString())[0] + Encoding.Default.GetBytes(i.ToString())[0];
                        var t = string.Format("#02{0}{1:X}\r", i, crc);
                        var reqCmd = Encoding.ASCII.GetBytes(t);
                        sock.Send(reqCmd);
                        sock.Receive(resCmd, resCmd.Length, System.Net.Sockets.SocketFlags.None);

                        var str = Encoding.ASCII.GetString(resCmd);
                        str = str.Substring(1, str.IndexOf("\r") > 8 ? 7 : str.IndexOf("\r") - 1).Replace('.', ',');
                        if (string.IsNullOrEmpty(str))
                            str = "0";
                        float val = float.Parse(str);

                        if (i == 3 || i == 5)
                        {
                            val = fData[i - 1] * 10000 + val * 0.1f;
                            fData[i - 1] = val;
                            if (i == 5)
                            {
                                fData[3] = val;
                                fData[4] = 0;
                            }
                        }
                        else
                            fData[i] = val;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                        ExceptionMessage = "ErrorUpdateClient: " + ex.Message;
                    }
                }
                sock.Close();
                FW2 = fData[0];
                FWIndex2 = fData[2];
            }
            catch (Exception ex) { 
                Console.WriteLine(ex.StackTrace);
                ExceptionMessage = "ErrorUpdateClient: " + ex.Message;
            }
        }
        public override string ToString()
        {
            return string.Format("Us: {4} FW1:{0} FW2:{1} FWIndex1:{2} FWIndex2:{3}", FW1, FW2, FWIndex1, FWIndex2, LastUpdate);
        }
    }
}
