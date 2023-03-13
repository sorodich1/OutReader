using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Threading;
using ClearScada.Client;
using ClearScada.Client.Simple;
using OutReader.Model;
using ScxV6DbClient;

namespace OutReader.Helper
{
    public class ScadaHelper
    {
        private ScadaUser _ourReaderUser;
        private const string NODE = "asu-cs";
        public bool IsBlocked { get; set; }
        private ScxV6Server _server { get; set; }
        public ScadaHelper()
        {
            _ourReaderUser = new ScadaUser(NODE, "Outreader", "ZdmEj'ARz]");
            _server = new ScxV6Server();
            //_server.ConnectTimeout = 20;
            _server.Connect(NODE, _ourReaderUser.Login, _ourReaderUser.Pass);
        }

        public void Disconnect()
        {
            _server.Disconnect();
        }

        public void Restart()
        {
            _server.Disconnect();
            Thread.Sleep(10000);
            _server = new ScxV6Server();
            //_server.ConnectTimeout = 20;
            _server.Connect(NODE, _ourReaderUser.Login, _ourReaderUser.Pass);
        }

        //private static void Connection(string node = "asu-cs", string login = "Utility", string pass = "qRIjO8z[")
        //{
        //    _server.Connect(node, login, pass);
        //}

        //public static void SetTeploseti(string node = "asu-cs", string login = "Utility", string pass = "qRIjO8z[")
        //{
        //    Connection(node, login, pass);
        //    var x = _server.FindObject("SVP_ELZ.Tags.1.Test_analogue");

        //    //var r = new Random();
        //    //for (int i = 0; i < 1000; i++)
        //    //{
        //    x.set_Property("HighHighLimitStd", 66);
        //        //Thread.Sleep(500);
        //    //}

        //    _server.Disconnect();
        //}
        public static void SetElizarovoCLient(Elizarovo el)
        {
            using (Connection conn = new Connection("OutReader"))
            {
                
                // Connect to local server
                conn.Connect("asu-cs");
                var tag =conn.GetObject("SVP_ELZ.Tags.2.Events.Alarm_ConnFault");
                tag["CurrentValue"] = Convert.ToInt32(!el.NotConnection);
                if (el.IsPulsar)
                {
                    if (el.Pulsar.Flow1 < 9999999)
                    {
                        tag = conn.GetObject("SVP_ELZ.Tags.2.Flow_water1");
                        tag["CurrentValue"] = el.Pulsar.Flow1;
                    }
                    if (el.Pulsar.Flow2 < 9999999)
                    {
                        tag = conn.GetObject("SVP_ELZ.Tags.2.Flow_water2");
                        tag["CurrentValue"] = el.Pulsar.Flow2;
                    }
                }
                if (el.IsTn)
                {
                    tag = conn.GetObject("SVP_ELZ.Tags.2.Volt1");
                    tag["CurrentValue"] = el.Tn.Au1;
                    tag = conn.GetObject("SVP_ELZ.Tags.2.Volt2");
                    tag["CurrentValue"] = el.Tn.Au2;
                    tag = conn.GetObject("SVP_ELZ.Tags.2.Volt3");
                    tag["CurrentValue"] = el.Tn.Au3;
                    tag = conn.GetObject("SVP_ELZ.Tags.2.Events.UV");
                    tag["CurrentValue"] = Convert.ToInt32(Convert.ToInt32(el.Tn.DI[1]).ToString() + Convert.ToInt32(el.Tn.DI[2]).ToString(), 2);
                    tag =conn.GetObject("SVP_ELZ.Tags.2.Events.UPS");
                    tag["CurrentValue"] = Convert.ToInt32(el.Tn.DI[0]);
                }
                if (el.IsM2)
                {
                    tag =conn.GetObject("SVP_ELZ.Tags.2.Freq1");
                    tag["CurrentValue"] =  el.Fq1;
                    tag =conn.GetObject("SVP_ELZ.Tags.2.Freq2");
                    tag["CurrentValue"] =  el.Fq2;
                    tag =conn.GetObject("SVP_ELZ.Tags.2.Freq3");
                    tag["CurrentValue"] =  el.Fq3;
                    tag =conn.GetObject("SVP_ELZ.Tags.2.Freq4");
                    tag["CurrentValue"] =  el.Fq4;
                }
                if (el.IsM1)
                {
                    tag =conn.GetObject("SVP_ELZ.Tags.2.Freq5");
                    tag["CurrentValue"] =  el.Fq5;
                    tag =conn.GetObject("SVP_ELZ.Tags.2.Freq6");
                    tag["CurrentValue"] =  el.Fq6;
                    tag =conn.GetObject("SVP_ELZ.Tags.2.PressIn");
                    tag["CurrentValue"] =  el.PressIn;
                    tag =conn.GetObject("SVP_ELZ.Tags.2.PressOut");
                    tag["CurrentValue"] =  el.PressOut;
                }
            }

        }


        public static void SetUsCLient(Us us)
        {
            using (Connection conn = new Connection("OutReader"))
            {

                // Connect to local server
                conn.Connect("asu-cs");
                if (!us.IsExceptionClient)
                {
                    var tag = conn.GetObject("Teleskop.VNS_ZudKvartal.Tags.Flow_water1m");
                    tag["CurrentValue"] = us.FW1;
                    tag = conn.GetObject("Teleskop.VNS_ZudKvartal.Tags.Flow_water2m");
                    tag["CurrentValue"] = us.FW2;
                    tag = conn.GetObject("Teleskop.VNS_ZudKvartal.Tags.Flow_water1");
                    tag["CurrentValue"] = us.FWIndex1;
                    tag = conn.GetObject("Teleskop.VNS_ZudKvartal.Tags.Flow_water2");
                    tag["CurrentValue"] = us.FWIndex2;
                }
                
            }

        }

        internal static void SetKnsPress(decimal press, string name, Tn tn)
        {
            using (Connection conn = new Connection("OutReader"))
            {
                if (name == "Unit_5249")
                    name = "KNS22";
                // Connect to local server
                conn.Connect("asu-cs");
                var tag = conn.GetObject(string.Format("KNS.{0}.Tags.Press", name));
                tag["CurrentValue"] = (double)press;

                try
                {
                    if (tn!=null)
                    {
                        tag = conn.GetObject(string.Format("KNS.{0}.Tags.volt_A",name));
                        tag["CurrentValue"] = Math.Round(tn.Au1, 1);
                        tag = conn.GetObject(string.Format("KNS.{0}.Tags.volt_B",name));
                        tag["CurrentValue"] = Math.Round(tn.Au2, 1);
                        tag = conn.GetObject(string.Format("KNS.{0}.Tags.volt_C", name));
                        tag["CurrentValue"] = Math.Round(tn.Au3,1);
                    }
                }
                catch (Exception ex)
                {
                    //b.ExceptionMessage += " Error save to Scada Massd:" + ex.Message + " Stack:" + ex.StackTrace;
                }
            }
        }

        internal static void SetGeoKiselev(GeoKiseleva obj)
        {
            using (Connection conn = new Connection("OutReader"))
            {

                // Connect to local server
                conn.Connect("asu-cs");
                if (!obj.IsExceptionClient)
                {
                    var tag = conn.GetObject("Teleskop.VNS_ZudKvartal.Tags.PressIn");
                    tag["CurrentValue"] = (double)obj.PressIn;
                    tag = conn.GetObject("Teleskop.VNS_ZudKvartal.Tags.PressOut");
                    tag["CurrentValue"] = (double)obj.PressOut;
                    tag = conn.GetObject("Teleskop.VNS_ZudKvartal.Tags.PressOut2");
                    tag["CurrentValue"] = (double)obj.PressOut2;

                    tag = conn.GetObject("Teleskop.VNS_ZudKvartal.Tags.Events.AlarmDoor");
                    tag["CurrentValue"] = obj.M1.DI[0];
                    tag = conn.GetObject("Teleskop.VNS_ZudKvartal.Tags.Events.PumpOn_1");
                    tag["CurrentValue"] = !obj.M1.DI[1];
                    tag = conn.GetObject("Teleskop.VNS_ZudKvartal.Tags.Events.PumpOn_2");
                    tag["CurrentValue"] = !obj.M1.DI[2];
                    tag = conn.GetObject("Teleskop.VNS_ZudKvartal.Tags.Events.PumpOn_3");
                    tag["CurrentValue"] = !obj.M1.DI[3];
                    tag = conn.GetObject("Teleskop.VNS_ZudKvartal.Tags.Events.Alarm_Station");
                    tag["CurrentValue"] = !obj.M1.DI[5];
                    tag = conn.GetObject("Teleskop.VNS_ZudKvartal.Tags.Events.Station_OK");
                    tag["CurrentValue"] = !obj.M1.DI[4];
                }

            }
        }

        public void SetElizarovo(Elizarovo el)
        {
            IsBlocked = true;
            //var _server = new ScxV6Server();
            //_server.Connect(NODE, "Outreader", "ZdmEj'ARz]");

            var tag =_server.FindObject("SVP_ELZ.Tags.2.Events.Alarm_ConnFault");
            tag.set_Property("CurrentValue", Convert.ToInt32(!el.NotConnection));
            if (el.IsPulsar)
            {
                if (el.Pulsar.Flow1 < 9999999)
                {
                    tag = _server.FindObject("SVP_ELZ.Tags.2.Flow_water1");
                    tag.set_Property("CurrentValue", el.Pulsar.Flow1);
                }
                if (el.Pulsar.Flow2 < 9999999)
                {
                    tag = _server.FindObject("SVP_ELZ.Tags.2.Flow_water2");
                    tag.set_Property("CurrentValue", el.Pulsar.Flow2);
                }
            }
            if (el.IsTn)
            {
                tag =_server.FindObject("SVP_ELZ.Tags.2.Volt1");
                tag.set_Property("CurrentValue", el.Tn.Au1);
                tag =_server.FindObject("SVP_ELZ.Tags.2.Volt2");
                tag.set_Property("CurrentValue", el.Tn.Au2);
                tag =_server.FindObject("SVP_ELZ.Tags.2.Volt3");
                tag.set_Property("CurrentValue", el.Tn.Au3);
                tag =_server.FindObject("SVP_ELZ.Tags.2.Events.UV");
                tag.set_Property("CurrentValue", Convert.ToInt32(el.Tn.DI[1]).ToString() + Convert.ToInt32(el.Tn.DI[2]).ToString());
                tag =_server.FindObject("SVP_ELZ.Tags.2.Events.UPS");
                tag.set_Property("CurrentValue", Convert.ToInt32(el.Tn.DI[0]));
            }
            if (el.IsM2)
            {
                tag =_server.FindObject("SVP_ELZ.Tags.2.Freq1");
                tag.set_Property("CurrentValue", el.Fq1);
                tag =_server.FindObject("SVP_ELZ.Tags.2.Freq2");
                tag.set_Property("CurrentValue", el.Fq2);
                tag =_server.FindObject("SVP_ELZ.Tags.2.Freq3");
                tag.set_Property("CurrentValue", el.Fq3);
                tag =_server.FindObject("SVP_ELZ.Tags.2.Freq4");
                tag.set_Property("CurrentValue", el.Fq4);
            }
            if (el.IsM1)
            {
                tag =_server.FindObject("SVP_ELZ.Tags.2.Freq5");
                tag.set_Property("CurrentValue", el.Fq5);
                tag =_server.FindObject("SVP_ELZ.Tags.2.Freq6");
                tag.set_Property("CurrentValue", el.Fq6);
                tag =_server.FindObject("SVP_ELZ.Tags.2.PressIn");
                tag.set_Property("CurrentValue", el.PressIn);
                tag =_server.FindObject("SVP_ELZ.Tags.2.PressOut");
                tag.set_Property("CurrentValue", el.PressOut);
            }
            IsBlocked = false;
            //_server.Disconnect();
        }

        public static void SetElizarovoSimaticClient(ElizarovoSimatic el)
        {
            using (var conn = new Connection("OutReader"))
            {
                conn.Connect("asu-cs");
                var tag = conn.GetObject("SVP_ELZ.Tags.1.Events.Alarm_ConnFault");
                tag["CurrentValue"] = Convert.ToInt32(!el.NotConnection);
                if (el.IsTags)
                {
                    foreach (Tag t in el.Tags)
                    {
                        try
                        {
                            var path = string.Format("SVP_ELZ.Tags.1.{0}{1}", t.TypeClass.Contains("BOOL") ? "Events." : "",
                                t.Name);
                            tag = conn.GetObject(path);
                            if (tag != null)
                            {
                                if (t.TypeClass.Contains("BOOL"))
                                    tag["CurrentValue"] = t.Value == "true" ? 1 : 0;
                                else if (t.TypeClass.Contains("FLOAT"))
                                    tag["CurrentValue"] = Convert.ToDouble(t.Value.Replace(".", ","));
                                else
                                    tag["CurrentValue"] = t.Value;
                            }
                            else
                            {
                                switch (t.Name)
                                {
                                    case "LE_ish_max":
                                        {
                                            tag = conn.GetObject("SVP_ELZ.Tags.1.LE6_scaled");
                                            tag["HighLimitStd"] = Convert.ToDouble(t.Value.Replace(".", ","));
                                            tag = conn.GetObject("SVP_ELZ.Tags.1.LE7_scaled");
                                            tag["HighLimitStd"] = Convert.ToDouble(t.Value.Replace(".", ","));
                                            break;
                                        }
                                    case "LE_ish_min":
                                        {
                                            tag = conn.GetObject("SVP_ELZ.Tags.1.LE6_scaled");
                                            tag["LowLimitStd"] = Convert.ToDouble(t.Value.Replace(".", ","));
                                            tag = conn.GetObject("SVP_ELZ.Tags.1.LE7_scaled");
                                            tag["LowLimitStd"] = Convert.ToDouble(t.Value.Replace(".", ","));
                                            break;
                                        }
                                    case "LE3_max":
                                        {
                                            tag = conn.GetObject("SVP_ELZ.Tags.1.LE3_scaled");
                                            tag["HighLimitStd"] = Convert.ToDouble(t.Value.Replace(".", ","));
                                            break;
                                        }
                                    case "LE3_min":
                                        {
                                            tag = conn.GetObject("SVP_ELZ.Tags.1.LE3_scaled");
                                            tag["LowLimitStd"] = Convert.ToDouble(t.Value.Replace(".", ","));
                                            break;
                                        }
                                    case "LE3_avar_max":
                                        {
                                            tag = conn.GetObject("SVP_ELZ.Tags.1.LE3_scaled");
                                            tag["HighHighLimitStd"] = Convert.ToDouble(t.Value.Replace(".", ","));
                                            break;
                                        }
                                    case "LE3_avar_min":
                                        {
                                            tag = conn.GetObject("SVP_ELZ.Tags.1.LE3_scaled");
                                            tag["LowLowLimitStd"] = Convert.ToDouble(t.Value.Replace(".", ","));
                                            break;
                                        }
                                    case "Pereliv_LE3":
                                        {
                                            tag = conn.GetObject("SVP_ELZ.Tags.1.LE3_scaled");
                                            tag["FullScale"] = Convert.ToDouble(t.Value.Replace(".", ","));
                                            break;
                                        }
                                    case "LE1_avar_max":
                                        {
                                            tag = conn.GetObject("SVP_ELZ.Tags.1.LE1_scaled");
                                            tag["HighHighLimitStd"] = Convert.ToDouble(t.Value.Replace(".", ","));
                                            break;
                                        }
                                    case "LE1_avar_min":
                                        {
                                            tag = conn.GetObject("SVP_ELZ.Tags.1.LE1_scaled");
                                            tag["LowLowLimitStd"] = Convert.ToDouble(t.Value.Replace(".", ","));
                                            break;
                                        }
                                    case "LE2_avar_max":
                                        {
                                            tag = conn.GetObject("SVP_ELZ.Tags.1.LE2_scaled");
                                            tag["HighHighLimitStd"] = Convert.ToDouble(t.Value.Replace(".", ","));
                                            break;
                                        }
                                    case "LE2_avar_min":
                                        {
                                            tag = conn.GetObject("SVP_ELZ.Tags.1.LE2_scaled");
                                            tag["LowLowLimitStd"] = Convert.ToDouble(t.Value.Replace(".", ","));
                                            break;
                                        }
                                    case "LE4_avar_max":
                                        {
                                            tag = conn.GetObject("SVP_ELZ.Tags.1.LE4_scaled");
                                            tag["HighHighLimitStd"] = Convert.ToDouble(t.Value.Replace(".", ","));
                                            break;
                                        }
                                    case "LE4_avar_min":
                                        {
                                            tag = conn.GetObject("SVP_ELZ.Tags.1.LE4_scaled");
                                            tag["LowLowLimitStd"] = Convert.ToDouble(t.Value.Replace(".", ","));
                                            break;
                                        }
                                    case "LE5_avar_max":
                                        {
                                            tag = conn.GetObject("SVP_ELZ.Tags.1.LE5_scaled");
                                            tag["HighHighLimitStd"] = Convert.ToDouble(t.Value.Replace(".", ","));
                                            break;
                                        }
                                    case "LE5_avar_min":
                                        {
                                            tag = conn.GetObject("SVP_ELZ.Tags.1.LE5_scaled");
                                            tag["LowLowLimitStd"] = Convert.ToDouble(t.Value.Replace(".", ","));
                                            break;
                                        }
                                    case "LE6_avar_max":
                                        {
                                            tag = conn.GetObject("SVP_ELZ.Tags.1.LE6_scaled");
                                            tag["HighHighLimitStd"] = Convert.ToDouble(t.Value.Replace(".", ","));
                                            break;
                                        }
                                    case "LE6_avar_min":
                                        {
                                            tag = conn.GetObject("SVP_ELZ.Tags.1.LE6_scaled");
                                            tag["LowLowLimitStd"] = Convert.ToDouble(t.Value.Replace(".", ","));
                                            break;
                                        }
                                    case "LE7_avar_max":
                                        {
                                            tag = conn.GetObject("SVP_ELZ.Tags.1.LE7_scaled");
                                            tag["HighHighLimitStd"] = Convert.ToDouble(t.Value.Replace(".", ","));
                                            break;
                                        }
                                    case "LE7_avar_min":
                                        {
                                            tag = conn.GetObject("SVP_ELZ.Tags.1.LE7_scaled");
                                            tag["LowLowLimitStd"] = Convert.ToDouble(t.Value.Replace(".", ","));
                                            break;
                                        }
                                    case "LE_chist_max":
                                        {
                                            tag = conn.GetObject("SVP_ELZ.Tags.1.LE1_scaled");
                                            tag["HighLimitStd"] = Convert.ToDouble(t.Value.Replace(".", ","));
                                            tag = conn.GetObject("SVP_ELZ.Tags.1.LE2_scaled");
                                            tag["HighLimitStd"] = Convert.ToDouble(t.Value.Replace(".", ","));
                                            tag = conn.GetObject("SVP_ELZ.Tags.1.LE4_scaled");
                                            tag["HighLimitStd"] = Convert.ToDouble(t.Value.Replace(".", ","));
                                            tag = conn.GetObject("SVP_ELZ.Tags.1.LE5_scaled");
                                            tag["HighLimitStd"] = Convert.ToDouble(t.Value.Replace(".", ","));
                                            break;
                                        }
                                    case "LE_chist_min":
                                        {
                                            tag = conn.GetObject("SVP_ELZ.Tags.1.LE1_scaled");
                                            tag["LowLimitStd"] = Convert.ToDouble(t.Value.Replace(".", ","));
                                            tag = conn.GetObject("SVP_ELZ.Tags.1.LE2_scaled");
                                            tag["LowLimitStd"] = Convert.ToDouble(t.Value.Replace(".", ","));
                                            tag = conn.GetObject("SVP_ELZ.Tags.1.LE4_scaled");
                                            tag["LowLimitStd"] = Convert.ToDouble(t.Value.Replace(".", ","));
                                            tag = conn.GetObject("SVP_ELZ.Tags.1.LE5_scaled");
                                            tag["LowLimitStd"] = Convert.ToDouble(t.Value.Replace(".", ","));
                                            break;
                                        }
                                    default:
                                        {
                                            var gg = tag;
                                            var gfd = gg;
                                            break;
                                        }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            el.ExceptionMessage += string.Format("{0}={1}, Error: {2} \r\n", t.Name, t.Value, e.Message);
                        }
                    }
                }
            }
        }

        public void SetElizarovoSimatic(ElizarovoSimatic el)
        {
            //IsBlocked = true;
            var _server = new ScxV6Server();
            //_server.ConnectTimeout = 20;
            _server.Connect(NODE, "ELZ", "\"k=zdeOFPb");
            var tag = _server.FindObject("SVP_ELZ.Tags.1.Events.Alarm_ConnFault");
            tag.set_Property("CurrentValue", Convert.ToInt32(!el.NotConnection));
            if (el.IsTags)
            {
                foreach (Tag t in el.Tags)
                {
                    try
                    {
                        var path = string.Format("SVP_ELZ.Tags.1.{0}{1}", t.TypeClass.Contains("BOOL") ? "Events." : "",
                            t.Name);
                        tag = _server.FindObject(path);
                        if (tag != null)
                        {
                            if (t.TypeClass.Contains("BOOL"))
                                tag.set_Property("CurrentValue", t.Value == "true" ? 1 : 0);
                            else if (t.TypeClass.Contains("FLOAT"))
                                tag.set_Property("CurrentValue", Convert.ToDouble(t.Value.Replace(".", ",")));
                            else
                                tag.set_Property("CurrentValue", t.Value);
                        }
                        else
                        {
                            switch (t.Name)
                            {
                                case "LE_ish_max":
                                    {
                                        tag = _server.FindObject("SVP_ELZ.Tags.1.LE6_scaled");
                                        tag.set_Property("HighLimitStd", Convert.ToDouble(t.Value.Replace(".", ",")));
                                        tag = _server.FindObject("SVP_ELZ.Tags.1.LE7_scaled");
                                        tag.set_Property("HighLimitStd", Convert.ToDouble(t.Value.Replace(".", ",")));
                                        break;
                                    }
                                case "LE_ish_min":
                                    {
                                        tag = _server.FindObject("SVP_ELZ.Tags.1.LE6_scaled");
                                        tag.set_Property("LowLimitStd", Convert.ToDouble(t.Value.Replace(".", ",")));
                                        tag = _server.FindObject("SVP_ELZ.Tags.1.LE7_scaled");
                                        tag.set_Property("LowLimitStd", Convert.ToDouble(t.Value.Replace(".", ",")));
                                        break;
                                    }
                                case "LE3_max":
                                    {
                                        tag = _server.FindObject("SVP_ELZ.Tags.1.LE3_scaled");
                                        tag.set_Property("HighLimitStd", Convert.ToDouble(t.Value.Replace(".", ",")));
                                        break;
                                    }
                                case "LE3_min":
                                    {
                                        tag = _server.FindObject("SVP_ELZ.Tags.1.LE3_scaled");
                                        tag.set_Property("LowLimitStd", Convert.ToDouble(t.Value.Replace(".", ",")));
                                        break;
                                    }
                                case "LE3_avar_max":
                                    {
                                        tag = _server.FindObject("SVP_ELZ.Tags.1.LE3_scaled");
                                        tag.set_Property("HighHighLimitStd", Convert.ToDouble(t.Value.Replace(".", ",")));
                                        break;
                                    }
                                case "LE3_avar_min":
                                    {
                                        tag = _server.FindObject("SVP_ELZ.Tags.1.LE3_scaled");
                                        tag.set_Property("LowLowLimitStd", Convert.ToDouble(t.Value.Replace(".", ",")));
                                        break;
                                    }
                                case "Pereliv_LE3":
                                    {
                                        tag = _server.FindObject("SVP_ELZ.Tags.1.LE3_scaled");
                                        tag.set_Property("FullScale", Convert.ToDouble(t.Value.Replace(".", ",")));
                                        break;
                                    }
                                case "LE1_avar_max":
                                    {
                                        tag = _server.FindObject("SVP_ELZ.Tags.1.LE1_scaled");
                                        tag.set_Property("HighHighLimitStd", Convert.ToDouble(t.Value.Replace(".", ",")));
                                        break;
                                    }
                                case "LE1_avar_min":
                                    {
                                        tag = _server.FindObject("SVP_ELZ.Tags.1.LE1_scaled");
                                        tag.set_Property("LowLowLimitStd", Convert.ToDouble(t.Value.Replace(".", ",")));
                                        break;
                                    }
                                case "LE2_avar_max":
                                    {
                                        tag = _server.FindObject("SVP_ELZ.Tags.1.LE2_scaled");
                                        tag.set_Property("HighHighLimitStd", Convert.ToDouble(t.Value.Replace(".", ",")));
                                        break;
                                    }
                                case "LE2_avar_min":
                                    {
                                        tag = _server.FindObject("SVP_ELZ.Tags.1.LE2_scaled");
                                        tag.set_Property("LowLowLimitStd", Convert.ToDouble(t.Value.Replace(".", ",")));
                                        break;
                                    }
                                case "LE4_avar_max":
                                    {
                                        tag = _server.FindObject("SVP_ELZ.Tags.1.LE4_scaled");
                                        tag.set_Property("HighHighLimitStd", Convert.ToDouble(t.Value.Replace(".", ",")));
                                        break;
                                    }
                                case "LE4_avar_min":
                                    {
                                        tag = _server.FindObject("SVP_ELZ.Tags.1.LE4_scaled");
                                        tag.set_Property("LowLowLimitStd", Convert.ToDouble(t.Value.Replace(".", ",")));
                                        break;
                                    }
                                case "LE5_avar_max":
                                    {
                                        tag = _server.FindObject("SVP_ELZ.Tags.1.LE5_scaled");
                                        tag.set_Property("HighHighLimitStd", Convert.ToDouble(t.Value.Replace(".", ",")));
                                        break;
                                    }
                                case "LE5_avar_min":
                                    {
                                        tag = _server.FindObject("SVP_ELZ.Tags.1.LE5_scaled");
                                        tag.set_Property("LowLowLimitStd", Convert.ToDouble(t.Value.Replace(".", ",")));
                                        break;
                                    }
                                case "LE6_avar_max":
                                    {
                                        tag = _server.FindObject("SVP_ELZ.Tags.1.LE6_scaled");
                                        tag.set_Property("HighHighLimitStd", Convert.ToDouble(t.Value.Replace(".", ",")));
                                        break;
                                    }
                                case "LE6_avar_min":
                                    {
                                        tag = _server.FindObject("SVP_ELZ.Tags.1.LE6_scaled");
                                        tag.set_Property("LowLowLimitStd", Convert.ToDouble(t.Value.Replace(".", ",")));
                                        break;
                                    }
                                case "LE7_avar_max":
                                    {
                                        tag = _server.FindObject("SVP_ELZ.Tags.1.LE7_scaled");
                                        tag.set_Property("HighHighLimitStd", Convert.ToDouble(t.Value.Replace(".", ",")));
                                        break;
                                    }
                                case "LE7_avar_min":
                                    {
                                        tag = _server.FindObject("SVP_ELZ.Tags.1.LE7_scaled");
                                        tag.set_Property("LowLowLimitStd", Convert.ToDouble(t.Value.Replace(".", ",")));
                                        break;
                                    }
                                case "LE_chist_max":
                                    {
                                        tag = _server.FindObject("SVP_ELZ.Tags.1.LE1_scaled");
                                        tag.set_Property("HighLimitStd", Convert.ToDouble(t.Value.Replace(".", ",")));
                                        tag = _server.FindObject("SVP_ELZ.Tags.1.LE2_scaled");
                                        tag.set_Property("HighLimitStd", Convert.ToDouble(t.Value.Replace(".", ",")));
                                        tag = _server.FindObject("SVP_ELZ.Tags.1.LE4_scaled");
                                        tag.set_Property("HighLimitStd", Convert.ToDouble(t.Value.Replace(".", ",")));
                                        tag = _server.FindObject("SVP_ELZ.Tags.1.LE5_scaled");
                                        tag.set_Property("HighLimitStd", Convert.ToDouble(t.Value.Replace(".", ",")));
                                        break;
                                    }
                                case "LE_chist_min":
                                    {
                                        tag = _server.FindObject("SVP_ELZ.Tags.1.LE1_scaled");
                                        tag.set_Property("LowLimitStd", Convert.ToDouble(t.Value.Replace(".", ",")));
                                        tag = _server.FindObject("SVP_ELZ.Tags.1.LE2_scaled");
                                        tag.set_Property("LowLimitStd", Convert.ToDouble(t.Value.Replace(".", ",")));
                                        tag = _server.FindObject("SVP_ELZ.Tags.1.LE4_scaled");
                                        tag.set_Property("LowLimitStd", Convert.ToDouble(t.Value.Replace(".", ",")));
                                        tag = _server.FindObject("SVP_ELZ.Tags.1.LE5_scaled");
                                        tag.set_Property("LowLimitStd", Convert.ToDouble(t.Value.Replace(".", ",")));
                                        break;
                                    }
                                default:
                                    {
                                        var gg = tag;
                                        var gfd = gg;
                                        break;
                                    }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        el.ExceptionMessage += string.Format("{0}={1}, Error: {2} \r\n", t.Name, t.Value, e.Message);
                    }
                }
            }
            //IsBlocked = false;
            _server.Disconnect();
        }
        public static void SetBrickClient(Brick b)
        {
            using (Connection conn = new Connection("OutReader"))
            {
                // Connect to local server
                conn.Connect("asu-cs");
                var tag = conn.GetObject("SVP_PKP.Tags.Events.Alarm_ConnFault");
                tag["CurrentValue"] =  Convert.ToInt32(!b.NotConnection);
                //if (b.IsPulsar)
                //{
                //    if (b.Pulsar.Flow2 > 0)
                //    {
                //        tag = conn.GetObject("SVP_PKP.Tags.Flow_water2");
                //        tag["CurrentValue"] = b.Pulsar.Flow2;
                //    }
                //    if (b.Pulsar.Flow1 > 0)
                //    {
                //        tag = conn.GetObject("SVP_PKP.Tags.Flow_waterFlush");
                //        var flow1 = Math.Round(Convert.ToDecimal(tag["CurrentValue"]),2);
                //        tag["CurrentValue"] = b.Pulsar.Flow1;
                //        tag = conn.GetObject("SVP_PKP.Tags.Flow_waterFlushm");
                //        tag["CurrentValue"] = (double)(flow1 - Math.Round(Convert.ToDecimal(b.Pulsar.Flow1), 2));
                //    }
                //}
                try
                {
                    if (b.Apb != null)
                    {
                        //input
                        //tag = conn.GetObject("SVP_PKP.Tags.Flow_water1x");
                        //tag["CurrentValue"] = b.Apb.FWInput;
                        //tag = conn.GetObject("SVP_PKP.Tags.Flow_water1");
                        //tag["CurrentValue"] = b.Apb.FlowWaterInput;
                        ////output
                        //tag = conn.GetObject("SVP_PKP.Tags.Flow_water2x");
                        //tag["CurrentValue"] = b.Apb.FWOutput;
                        //tag = conn.GetObject("SVP_PKP.Tags.Flow_water2");
                        //tag["CurrentValue"] = b.Apb.FlowWaterOutput;
                        ////flush
                        //tag = conn.GetObject("SVP_PKP.Tags.Flow_water3x");
                        //tag["CurrentValue"] = b.Apb.FWWashing;
                        //tag = conn.GetObject("SVP_PKP.Tags.Flow_water3");
                        //tag["CurrentValue"] = b.Apb.FlowWaterWashing;
                        //return;
                        if (b.Apb.FWInput > 0)
                            SetFlowWaterBrick(conn, "SVP_PKP.Tags.Flow_water1", b.Apb.FlowWaterInput,b.Apb.FWInput); //входное
                        if (b.Apb.FWOutput > 0) 
                            SetFlowWaterBrick(conn, "SVP_PKP.Tags.Flow_water2", b.Apb.FlowWaterOutput, b.Apb.FWOutput); //На выходе
                        if (b.Apb.FWWashing > 0) 
                            SetFlowWaterBrick(conn, "SVP_PKP.Tags.Flow_water3", b.Apb.FlowWaterWashing,b.Apb.FWWashing); //промывка

                        tag = conn.GetObject("SVP_PKP.Tags.Events.Flush_F1");
                        tag["CurrentValue"] = Convert.ToInt32(b.Apb.Filter1ToString,2);
                        tag = conn.GetObject("SVP_PKP.Tags.Events.Flush_F2");
                        tag["CurrentValue"] = Convert.ToInt32(b.Apb.Filter2ToString, 2);
                        tag = conn.GetObject("SVP_PKP.Tags.Events.Flush_F3");
                        tag["CurrentValue"] = Convert.ToInt32(b.Apb.Filter3ToString, 2);

                        for (var i = 0; i < 15; i++)
                        {
                            var str = "SVP_PKP.Tags.Boolean.Q";
                            if (i < 10) str += "0";
                            str += i;
                            tag = conn.GetObject(str);
                            tag["CurrentValue"] = b.Apb.Qs[i];
                        }

                        for (var i = 11; i < 22; i++)
                        {
                            if (i == 12 || i == 17 || i == 18) continue;
                            var str = "SVP_PKP.Tags.Boolean.I";
                            if (i < 15) str += (i - 1).ToString("X").ToLower();
                            else
                                str += (i - 5);

                            tag = conn.GetObject(str);
                            tag["CurrentValue"] = b.Apb.Is[i - 1];
                        }
                    }
                }
                catch (Exception ex)
                {
                    b.ExceptionMessage += " Error save to Scada Apb:" + ex.Message + " Stack:" + ex.StackTrace;
                }
                try
                {
                    if (b.IsTn)
                    {
                        tag = conn.GetObject("SVP_PKP.Tags.Volt1");
                        tag["CurrentValue"] =  b.Tn.Au1;
                        tag = conn.GetObject("SVP_PKP.Tags.Volt2");
                        tag["CurrentValue"] =  b.Tn.Au2;
                        tag = conn.GetObject("SVP_PKP.Tags.Volt3");
                        tag["CurrentValue"] =  b.Tn.Au3;
                        tag = conn.GetObject("SVP_PKP.Tags.Events.UPS");
                        tag["CurrentValue"] =  Convert.ToInt32(b.Tn.DI[0]);
                    }
                }
                catch (Exception ex)
                {
                    b.ExceptionMessage += " Error save to Scada Tn:" + ex.Message + " Stack:" + ex.StackTrace;
                }
                try
                {
                    if (b.IsM1)
                    {
                        tag = conn.GetObject("SVP_PKP.Tags.Press_F3i");
                        tag["CurrentValue"] =  b.PressIn3;
                        tag = conn.GetObject("SVP_PKP.Tags.Press_Flush");
                        tag["CurrentValue"] =  b.PressOut3;
                        tag = conn.GetObject("SVP_PKP.Tags.Press_F2i");
                        tag["CurrentValue"] =  b.PressIn2;
                        tag = conn.GetObject("SVP_PKP.Tags.Press_F2o");
                        tag["CurrentValue"] =  b.PressOut2;
                        tag = conn.GetObject("SVP_PKP.Tags.Events.Alarm_Door");
                        tag["CurrentValue"] =  Convert.ToInt32(!b.M1.DI[0]);
                        tag = conn.GetObject("SVP_PKP.Tags.Events.Alarm_Station1");
                        tag["CurrentValue"] =  Convert.ToInt32(!b.M1.DI[1]);
                        tag = conn.GetObject("SVP_PKP.Tags.Events.Alarm_Station2");
                        tag["CurrentValue"] =  Convert.ToInt32(!b.M1.DI[2]);
                        tag = conn.GetObject("SVP_PKP.Tags.Events.PumpOn1");
                        tag["CurrentValue"] =  Convert.ToInt32(!b.M1.DI[3]);
                        tag = conn.GetObject("SVP_PKP.Tags.Events.Alarm_Pump1");
                        tag["CurrentValue"] =  Convert.ToInt32(!b.M1.DI[4]);
                        tag = conn.GetObject("SVP_PKP.Tags.Events.PumpOn2");
                        tag["CurrentValue"] =  Convert.ToInt32(!b.M1.DI[5]);
                        tag = conn.GetObject("SVP_PKP.Tags.Events.Alarm_Pump2");
                        tag["CurrentValue"] =  Convert.ToInt32(!b.M1.DI[6]);
                        tag = conn.GetObject("SVP_PKP.Tags.Events.PumpOn3");
                        tag["CurrentValue"] = Convert.ToInt32(!b.M1.DI[7]);
                    }
                }
                catch (Exception ex)
                {
                    b.ExceptionMessage += " Error save to Scada M1:" + ex.Message + " Stack:" + ex.StackTrace;
                }
                try
                {
                    if (b.IsM2)
                    {
                        tag = conn.GetObject("SVP_PKP.Tags.Press_F1i");
                        tag["CurrentValue"] =  b.PressIn1;
                        tag = conn.GetObject("SVP_PKP.Tags.Press_F1o");
                        tag["CurrentValue"] =  b.PressOut1;
                        tag = conn.GetObject("SVP_PKP.Tags.PressIn");
                        tag["CurrentValue"] =  b.PressIn;
                        tag = conn.GetObject("SVP_PKP.Tags.PressOut");
                        tag["CurrentValue"] =  b.PressOut;
                        tag = conn.GetObject("SVP_PKP.Tags.Events.Alarm_DryRun");
                        tag["CurrentValue"] = Convert.ToInt32(!b.M2.DI[0]);
                        tag = conn.GetObject("SVP_PKP.Tags.Events.Alarm_AlarmR");
                        tag["CurrentValue"] = Convert.ToInt32(!b.M2.DI[1]);
                        tag = conn.GetObject("SVP_PKP.Tags.Events.LevelR");
                        tag["CurrentValue"] = Convert.ToInt32(Convert.ToInt32(!b.M2.DI[7]).ToString() + Convert.ToInt32(!b.M2.DI[6]).ToString() + Convert.ToInt32(!b.M2.DI[2]).ToString(), 2);
                        tag = conn.GetObject("SVP_PKP.Tags.Events.Level");
                        tag["CurrentValue"] = Convert.ToInt32(Convert.ToInt32(!b.M2.DI[5]).ToString() + Convert.ToInt32(!b.M2.DI[4]).ToString() + Convert.ToInt32(!b.M2.DI[3]).ToString(), 2);
                    }
                }
                catch (Exception ex)
                {
                    b.ExceptionMessage += " Error save to Scada M2:" + ex.Message + " Stack:" + ex.StackTrace;
                }

                try
                {
                    if (b.IsMass1)
                    {
                        tag = conn.GetObject("SVP_PKP.Tags.Osmos.Weight");
                        tag["CurrentValue"] = Convert.ToDouble(b.Mass1);
                    }
                }
                catch (Exception ex)
                {
                    b.ExceptionMessage += " Error save to Scada Massd:" + ex.Message + " Stack:" + ex.StackTrace;
                }
                try
                {
                    if (b.IsSi1)
                    {
                        tag = conn.GetObject("SVP_PKP.Tags.Osmos.Flow_water1");
                        tag["CurrentValue"] = Convert.ToDouble(b.Si1.V/100)+27614.5;
                    }
                }
                catch (Exception ex)
                {
                    b.ExceptionMessage += " Error save to Scada Massd:" + ex.Message + " Stack:" + ex.StackTrace;
                }
                try
                {
                    if (b.IsSi2)
                    {
                        tag = conn.GetObject("SVP_PKP.Tags.Osmos.Flow_water2");
                        tag["CurrentValue"] = Convert.ToDouble(b.Si2.V/10)+4899;
                    }
                }
                catch (Exception ex)
                {
                    b.ExceptionMessage += " Error save to Scada Massd:" + ex.Message + " Stack:" + ex.StackTrace;
                }
                try
                {
                    if (b.IsPR200)
                    {
                        tag = conn.GetObject("SVP_PKP.Tags.Osmos.Osmos_state");
                        tag["CurrentValue"] = Convert.ToInt32(b.PR200.D1); //RO запуск
                        tag = conn.GetObject("SVP_PKP.Tags.Osmos.PR200_I2"); 
                        tag["CurrentValue"] = Convert.ToInt32(b.PR200.D2);
                        tag = conn.GetObject("SVP_PKP.Tags.Osmos.Level_1");
                        tag["CurrentValue"] = Convert.ToInt32(b.PR200.D3);
                        tag = conn.GetObject("SVP_PKP.Tags.Osmos.Level_Dryrun");
                        tag["CurrentValue"] = Convert.ToInt32(b.PR200.D4);
                        tag = conn.GetObject("SVP_PKP.Tags.Osmos.Level_2");
                        tag["CurrentValue"] = Convert.ToInt32(b.PR200.D5);
                        tag = conn.GetObject("SVP_PKP.Tags.Osmos.Level_Dryrun2");
                        tag["CurrentValue"] = Convert.ToInt32(b.PR200.D6);
                        tag = conn.GetObject("SVP_PKP.Tags.Osmos.Osmos_flush");
                        tag["CurrentValue"] = Convert.ToInt32(b.PR200.D7);
                        //tag = conn.GetObject("SVP_PKP.Tags.Osmos.Pump_1");
                        //tag["CurrentValue"] = Convert.ToInt32(b.PR200.D8);
                        tag = conn.GetObject("SVP_PKP.Tags.Osmos.Pump_1");
                        tag["CurrentValue"] = Convert.ToInt32(b.PR200.Q1);
                        tag = conn.GetObject("SVP_PKP.Tags.Osmos.Pump_2");
                        tag["CurrentValue"] = Convert.ToInt32(b.PR200.Q2);
                        tag = conn.GetObject("SVP_PKP.Tags.Osmos.Pump_3");
                        tag["CurrentValue"] = Convert.ToInt32(b.PR200.Q4); //Насос осмоса
                        tag = conn.GetObject("SVP_PKP.Tags.Osmos.Valve_Y1");
                        tag["CurrentValue"] = Convert.ToInt32(b.PR200.Q5);
                        tag = conn.GetObject("SVP_PKP.Tags.Osmos.Valve_Y2");
                        tag["CurrentValue"] = Convert.ToInt32(b.PR200.Q6);
                        tag = conn.GetObject("SVP_PKP.Tags.Osmos.Valve_Y3");
                        tag["CurrentValue"] = Convert.ToInt32(b.PR200.Q7);
                        tag = conn.GetObject("SVP_PKP.Tags.Osmos.Osmos_out");
                        tag["CurrentValue"] = Convert.ToInt32(b.PR200.Q8);
                    }
                }
                catch (Exception ex)
                {
                    b.ExceptionMessage += " Error save to Scada PR200:" + ex.Message + " Stack:" + ex.StackTrace;
                }
            }

        }

        private static void SetFlowWaterBrick(Connection conn, string tagName, double value, int plcValue)
        {
            var tag = conn.GetObject(tagName+"x");
            var old = Convert.ToInt32(tag["CurrentValue"]);
            if (old > plcValue)
                old = 0;
            tag["CurrentValue"] = plcValue;
            tag = conn.GetObject(tagName);
            var res = Convert.ToDouble(tag["CurrentValue"]) + (plcValue - old)*0.1;
            //if (value < 700)
            //    tag["CurrentValue"] = value;//Convert.ToDouble(tag["CurrentValue"]) - 16.5;
            //else if (value > 150000)
            //    tag["CurrentValue"] = value;// Convert.ToDouble(tag["CurrentValue"]) - 789.6;
            //else
            //    tag["CurrentValue"] = value;//Convert.ToDouble(tag["CurrentValue"]) - 828.8;
            if (res < 999999)
                tag["CurrentValue"] = res;
            else
                tag["CurrentValue"] = res - 999999;
        }

        //public static void SetTeploseti(Teplosets t)
        //{
        //    using (Connection conn = new Connection("OutReader"))
        //    {
        //        // Connect to local server
        //        conn.Connect("asu-cs");
        //        conn.LogOn("Outreader", "ZdmEj'ARz]");
        //        var teploseti = conn.GetObject("Teploseti");
        //        foreach (DBObject teplo in teploseti.GetChildren("",""))
        //        {
        //            var name = teplo["AsuName"].ToString();
        //            var teploset = t.Kots.FirstOrDefault(x => x.ShortName == name.Replace(" ", ""));
        //            if (teploset != null && teploset.Date > DateTime.MinValue)
        //            {
        //                try
        //                {
        //                    if (teploset.ReplaceName)
        //                        teplo["NoteText"] = teploset.NewName;
        //                    else
        //                    {
        //                        var note = teplo["NoteText"].ToString();
        //                        if (!string.IsNullOrEmpty(note)) teplo["NoteText"] = "";
        //                    }

        //                    var tags = teplo.GetChild("Tags");

        //                    var
        //                    Tag = tags.GetChild("Press");
        //                    var flowDate = Convert.ToDateTime(pressTag["CurrentTime"].ToString());

        //                    if (flowDate != teploset.Date)
        //                    {
        //                        var flowTag = tags.GetChild("Flow");

        //                        if (teploset.Press >= 0 && !pressTag["CurrentValue"].ToString().Contains(teploset.Press.ToString()))
        //                            pressTag["CurrentValue"] = (double)teploset.Press;
        //                        if (teploset.Flow >= 0 && !flowTag["CurrentValue"].ToString().Contains(teploset.Flow.ToString()))
        //                            flowTag["CurrentValue"] = (double)teploset.Flow;
        //                    }
        //                }
        //                catch (Exception e)
        //                {
        //                    t.ExceptionMessage += string.Format("{0} Press:{1}, Error: {2} \r\n", teploset.Name, teploset.Press, e.Message);
        //                }
        //            }
        //        }
        //    }
        //}


        public static void SetStreamLux(StreamLux streamLux)
            {
            using (Connection conn = new Connection("OutReader"))
                {
                // Connect to local server
                conn.Connect("asu-cs");
                if (streamLux.Mb16D != null)
                    {
                    var tag = conn.GetObject("OSV.Tags.JAW.korund");
                    tag["CurrentValue"] = streamLux.Level / 10;

                    tag = conn.GetObject("OSV.Tags.JAW.pump1_counter");
                    var prev = Convert.ToInt32(tag["CurrentValue"]);
                    tag["CurrentValue"] = streamLux.Mb16D.D1Counter;
                    tag = conn.GetObject("OSV.Tags.JAW.pump2_counter");
                    var prev2 = Convert.ToInt32(tag["CurrentValue"]);
                    tag["CurrentValue"] = streamLux.Mb16D.D2Counter;

                    tag = conn.GetObject("OSV.Tags.JAW.drain_pump1");
                    tag["CurrentValue"] = streamLux.Mb16D.DI[0] || prev < streamLux.Mb16D.D1Counter;
                    tag = conn.GetObject("OSV.Tags.JAW.drain_pump2");
                    tag["CurrentValue"] = streamLux.Mb16D.DI[1] || prev2 < streamLux.Mb16D.D2Counter;
                    tag = conn.GetObject("OSV.Tags.JAW.drain_overflow");
                    tag["CurrentValue"] = streamLux.Mb16D.DI[2];
                    tag = conn.GetObject("OSV.Tags.JAW.pn_pump1");
                    tag["CurrentValue"] = streamLux.Mb16D.DI[14];
                    tag = conn.GetObject("OSV.Tags.JAW.pn_pump2");
                    tag["CurrentValue"] = streamLux.Mb16D.DI[15];

                }
                }
            }

        
    }
}
