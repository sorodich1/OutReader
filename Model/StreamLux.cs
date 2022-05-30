using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using OutReader.Helper;
using OutReader.Properties;

namespace OutReader.Model
    {
    public class StreamLux : IObject
        {
        /// <summary>
        /// DQD
        /// </summary>
        public double FlowWaterDay { get; set; }
        /// <summary>
        /// DQH
        /// </summary>
        public double FlowWaterHour { get; set; }
        /// <summary>
        /// DQM
        /// </summary>
        public double FlowWaterMinute { get; set; }
        /// <summary>
        /// DV
        /// </summary>
        public double FlowWaterCurrent { get; set; }
        /// <summary>
        /// DI+
        /// </summary>
        public double FlowWaterPos { get; set; }
        /// <summary>
        /// DI-
        /// </summary>
        public double FlowWaterNeg { get; set; }
        /// <summary>
        /// DIN
        /// </summary>
        public double FlowWaterNet { get; set; }
        /// <summary>
        /// DL
        /// </summary>
        public string SignalQuality { get; set; }
        /// <summary>
        /// DC
        /// </summary>
        public string ErrorCode { get; set; }
        /// <summary>
        /// DA
        /// </summary>
        public bool? IsErrorOCT { get; set; }
        /// <summary>
        /// DA
        /// </summary>
        public bool? IsErrorRelay { get; set; }
        /// <summary>
        /// LCD
        /// </summary>
        public string MonitorValue { get; set; }
        /// <summary>
        /// DT
        /// </summary>
        public string Dt { get; set; }
        public double Level { get; set; }
        public Tn Tn { get; set; }
        public MB16D Mb16D { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool NotConnection { get; private set; }
        public bool IsExceptionClient { get; set; }
        public string ExceptionMessage { get; set; }
        public void Run()
            {
            Update();
            SaveToScada();
            if (string.IsNullOrEmpty(ExceptionMessage))
                {
                SaveToSql();

                }

            SaveLogToSql();
            }

        public void SaveToScada()
            {
            ScadaHelper.SetStreamLux(this);
            }

        public void SaveToSql()
            {
            DbHelper.SetStreamLuxLog(this);
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

            try
                {
                using (TcpClient client = new TcpClient(Config.Default.StreamLuxIP, Config.Default.StreamLuxPort))
                    {
                    client.SendTimeout = 20000;
                    client.ReceiveTimeout = 16000;
                    Mb16D = ModbusHelper.ReadM16D(client, 5);
                    Level = ModbusHelper.TRM200(client, 2, FlowWaterHour);
                    //client.Close();
                    }
                }
            catch (Exception ex)
                {
                IsExceptionClient = true;
                ExceptionMessage = "ErrorUpdateClient: " + ex.Message;
                }
            Thread.Sleep(15000);
            Tn = new Tn();
            try
                {

                using (TcpClient client = new TcpClient("192.168.8.99", 21724))
                    {
                    client.SendTimeout = 20000;
                    client.ReceiveTimeout = 16000;
                    //Mb16D = ModbusHelper.ReadM16D(client, 5);

                    //Tn = ModbusHelper.ReadTn(client, 3);
                    //Thread.Sleep(200);

                    //Thread.Sleep(200);

                    var str = _sendRequest(client, "DQD\r");
                    //str = _sendRequest(client, "M<&M1&M1&LCD\r");
                    //str = _sendRequest(client, "M=&M8&M5&M1&M=\r"); //&LCD
                    //str = _sendRequest(client, "DQD\r");
                    FlowWaterDay = _stringToFlowWater(str[0]);
                    Thread.Sleep(200);
                    str = _sendRequest(client, "DQH\r");
                    FlowWaterHour = _stringToFlowWater(str[0]);

                    Thread.Sleep(200);
                    str = _sendRequest(client, "DQM&DV\r");
                    var i = 0;
                    foreach (var s in str)
                        {
                        var val = _stringToFlowWater(s);
                        switch (i)
                            {
                            case 0:
                                    {
                                    FlowWaterMinute = val;
                                    break;
                                    }
                            case 1:
                                    {
                                    FlowWaterCurrent = val;
                                    break;
                                    }
                            }
                        i++;
                        }

                    Thread.Sleep(100);
                    str = _sendRequest(client, "DI+&DI-\r");
                    i = 0;
                    foreach (var s in str)
                        {
                        var val = _stringToFlowWater(s);
                        switch (i)
                            {
                            case 0:
                                    {
                                    FlowWaterPos = val;
                                    break;
                                    }
                            case 1:
                                    {
                                    FlowWaterNeg = val;
                                    break;
                                    }
                            }
                        i++;

                        }
                    Thread.Sleep(100);
                    str = _sendRequest(client, "DIN&DL&DC\r");
                    i = 0;
                    foreach (var s in str)
                        {
                        switch (i)
                            {
                            case 0:
                                    {
                                    FlowWaterNet = _stringToFlowWater(s);
                                    break;
                                    }
                            case 1:
                                    {
                                    SignalQuality = s;
                                    break;
                                    }
                            case 2:
                                    {
                                    ErrorCode = s;
                                    break;
                                    }
                            }
                        i++;

                        }
                    //Level = ModbusHelper.TRM200(client, 2, FlowWaterHour);
                    //Mb16D = ModbusHelper.ReadM16D(client, 5);
                    Thread.Sleep(100);
                    str = _sendRequest(client, "DA&DT\r"); //&LCD
                    i = 0;
                    foreach (var s in str)
                        {
                        switch (i)
                            {
                            case 0:
                                    {
                                    foreach (var s3 in s.Split(','))
                                        {
                                        bool? val = s3.IndexOf("ON") > -1 ? true : (s3.IndexOf("OFF") > -1 ? false : null as bool?);
                                        if (s3.IndexOf("TR") > -1)
                                            IsErrorOCT = val;
                                        else
                                            IsErrorRelay = val;
                                        }
                                    break;
                                    }
                            case 1:
                                    {
                                    Dt = s;
                                    break;
                                    }
                            case 2:
                                    {
                                    MonitorValue = s;
                                    break;
                                    }
                            }
                        i++;
                        }
                    //client.Close();
                    }
                }
            catch (Exception ex)
                {
                IsExceptionClient = true;
                ExceptionMessage = "ErrorUpdateClient: " + ex.Message;
                }




            LastUpdate = DateTime.Now;
            }

        private List<string> _sendRequest(Socket sock, string query, string split = "\n")
            {
            byte[] reqCmd = new byte[100];
            byte[] resCmd = new byte[100];
            reqCmd = Encoding.ASCII.GetBytes(query);
            sock.Send(reqCmd);

            sock.Receive(resCmd, resCmd.Length, System.Net.Sockets.SocketFlags.None);

            var str = Encoding.ASCII.GetString(resCmd);
            var res = new List<string>();
            foreach (var s in str.Split(new string[] { split }, StringSplitOptions.RemoveEmptyEntries))
                {
                var ss = s.Replace("\0", "");
                if (ss.Length > 0)
                    res.Add(ss);
                }
            return res;
            }

        private List<string> _sendRequest(TcpClient client, string query, string split = "\n")
            {
            byte[] reqCmd = new byte[100];
            byte[] resCmd = new byte[100];
            reqCmd = Encoding.ASCII.GetBytes(query);
            client.Client.Send(reqCmd);

            client.Client.Receive(resCmd, resCmd.Length, System.Net.Sockets.SocketFlags.None);

            var str = Encoding.ASCII.GetString(resCmd);
            var res = new List<string>();
            foreach (var s in str.Split(new string[] { split }, StringSplitOptions.RemoveEmptyEntries))
                {
                var ss = s.Replace("\0", "").Replace("?", "").Trim();
                if (ss.Length > 0)
                    res.Add(ss);
                }
            return res;
            }

        private double _stringToFlowWater(string s)
            {
            var s1 = s.IndexOf("m") > -1 ? s.Substring(1, s.IndexOf("m") - 1) : s.Substring(1, s.IndexOf("."));
            var e = 0;
            var d = 0;
            var indexE = s1.IndexOf("E");
            if (indexE > -1)
                {
                var s2 = s1.Substring(indexE + 2);
                if (Convert.ToInt32(s2) > 0 || s1.Substring(0, indexE).Contains("."))
                    d = s1.Substring(0, indexE).Replace(".", "").Length - Convert.ToInt32(s2) - 1;
                }
            else
                {
                indexE = s1.IndexOf(".");
                }
            var val = Double.Parse(s1.Substring(0, indexE).Replace(".", ""));
            if (d > 0)
                {
                for (var i = 0; i < d; i++)
                    val /= 10;
                }
            return val;
            }

        public override string ToString()
            {
            return string.Format("StreamLux: Dt={0}, Level:{11} FWD={1}, FWH={2}, FWM={3}, FWC={4}, FWN={5}, FWP={6}, FWN={7}, EC={8}, SQ={9}  MB16D={12} \r\n {10}", LastUpdate, FlowWaterDay, FlowWaterHour, FlowWaterMinute, FlowWaterCurrent, FlowWaterNeg, FlowWaterPos, FlowWaterNet, ErrorCode, SignalQuality, ExceptionMessage, Level, Mb16D != null ? Mb16D.ToString() : "null");
            }
        }
    }
