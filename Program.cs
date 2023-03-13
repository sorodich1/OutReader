using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Modbus.Device;
using OutReader.Helper;
using OutReader.Model;
using OutReader.Properties;
using Timer = System.Timers.Timer;

namespace OutReader
{
    class Program
    {
        private static Timer _timerB;
        private static Timer _timerE;
        private static Timer _timerE2;
        private static Timer _timerSL;
        private static Timer _timerM;
        private static Timer _timerT;
        private static Timer _timerV;
        private static Timer _timerApb;
        private static Timer _timerLW;
        private static Timer _timerOLW;
        private static Timer _timerU;
        private static Timer _timerKNSAlechina;
        private static bool _isStop = false;
        private static ScadaHelper _scada;
        private static Properties.Config _config;
        static void Main(string[] args)
        {
            _config = Config.Default;
            Console.WriteLine("Для выхода нажмите любую клавишу");

            if (_config.GeoKiselevaEnabled)
            {
                _timerU = new Timer { AutoReset = false };
                _timerU.Elapsed += OnTimerU;
                _timerU.Start();
            }

            if (_config.KNSEnabled)
            {
                _timerKNSAlechina = new Timer { AutoReset = false }; //This will set the default interval
                _timerKNSAlechina.Elapsed += OnTimerKns;
                _timerKNSAlechina.Start();
            }

            if (_config.StreamLuxEnabled)
            {
                _timerSL = new Timer { AutoReset = false }; //This will set the default interval
                _timerSL.Elapsed += OnTimerSL;
                _timerSL.Start();
            }

            if (_config.BrickEnabled)
            {
                _timerB = new Timer { AutoReset = false }; //This will set the default interval
                _timerB.Elapsed += OnTimerB;
                _timerB.Start();
            }

            if (_config.Elizarovo2Enabled)
            {
                _timerE = new Timer { AutoReset = false }; //This will set the default interval
                _timerE.Elapsed += OnTimerE;
                _timerE.Start();
            }

            if (_config.ElizarovoSimaticEnabled)
            {
                _timerE2 = new Timer { AutoReset = false }; //This will set the default interval
                _timerE2.Elapsed += OnTimerE2;
                _timerE2.Start();
            }

            //if (_config.MerkurEnabled)
            //{
            //    _timerM = new Timer { AutoReset = false }; //This will set the default interval
            //    _timerM.Elapsed += OnTimerM;
            //    _timerM.Start();
            //}

            //if (_config.TeplosetEnabled)
            //{
            //    _timerT = new Timer { AutoReset = false }; //This will set the default interval
            //    _timerT.Elapsed += OnTimerT;
            //    _timerT.Start();
            //}

            if (_config.LiftWaterEnabled)
            {
                _timerLW = new Timer { AutoReset = false };
                _timerLW.Elapsed += OnTimerLW;
                _timerLW.Start();
            }
            if (_config.OwenLiftWaterEnabled)
            {
                _timerOLW = new Timer { AutoReset = false };
                _timerOLW.Elapsed += OnTimerOLW;
                _timerOLW.Start();
            }

            Console.ReadLine();
            Console.WriteLine("Подождите... Закрываются потоки");
            _isStop = true;
        }

        private static void OnTimerKns(object sender, ElapsedEventArgs e)
        {
            _timerKNSAlechina.Stop();
            if (_isStop) return;

            try
            {
                var kns = DbHelper.GetKns()/*.Where(x => x.KNSId == "KNS14")*/;
                Parallel.ForEach(kns, k =>
                {
                    var str = "";
                    try
                    {
                        k.Run();
                        str += k + "\r\n";
                    }
                    catch (Exception ex)
                    {
                        str += ex.Message + "\r\n";
                        k.ExceptionMessage += ex.Message + "\r\n";
                        DbHelper.SetLog(k.ExceptionMessage, k.KNSId);
                    }

                    Console.WriteLine(str);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("TimerKNSError " + ex.Message);
            }

            _timerKNSAlechina.Interval = _config.KNSInterval;
            _timerKNSAlechina.Start();

        }

        private static void OnTimerU(object sender, ElapsedEventArgs ev)
        {
            _timerU.Stop();
            try
            {
                DbHelper.SetLog("Start Timer", "GeoKiseleva", false);
                if (_isStop) return;
                var o = new GeoKiseleva();
                var str = "";
                try
                {
                    o.Run();
                    str += o + "\r\n";
                }
                catch (Exception ex)
                {
                    DbHelper.SetLog("Exception in Timer", "GeoKiseleva", false);
                    try
                    {
                        str += ex.Message + "\r\n";
                        o.ExceptionMessage += ex.Message + "\r\n";
                        DbHelper.SetLog(o.ExceptionMessage, "GeoKiseleva");
                    }
                    catch (Exception e1)
                    {
                        DbHelper.SetLog(string.Format("Message: {0}       Stack: {1}", e1.Message, e1.StackTrace), "GeoKiseleva");
                    }
                }

                Console.WriteLine(str);
                _timerU.Interval = _config.GeoKiselevaInterval;
                DbHelper.SetLog("End Timer", "GeoKiseleva", false);
            }
            catch (Exception e)
            {
                DbHelper.SetLog(string.Format("Message: {0}       Stack: {1}", e.Message, e.StackTrace), "GeoKiseleva");
            }

            _timerU.Start();
        }


        private static void OnTimerB(object sender, ElapsedEventArgs args)
        {
            //Do some work here
            _timerB.Stop();

            if (_isStop) return;
            var o = new Brick();
            var str = "";
            try
            {
                o.Update();
                o.Run();
                str += o + "\r\n";
            }
            catch (Exception ex)
            {
                str += ex.Message + "\r\n";
                o.ExceptionMessage += ex.Message + "\r\n";
                DbHelper.SetLog(o.ExceptionMessage, "Brick");
            }
            Console.WriteLine(str);

            _timerB.Interval = _config.BrickInterval; //Set your new interval here
            _timerB.Start();
        }
        private static void OnTimerE(object sender, ElapsedEventArgs args)
        {
            //Do some work here
            _timerE.Stop();
            if (_isStop) return;
            var o = new Elizarovo();
            var str = "";
            try
            {
                o.Update();
                o.Run();
                str += o + "\r\n";
            }
            catch (Exception ex)
            {
                str += ex.Message + "\r\n";
                o.ExceptionMessage += ex.Message + "\r\n";
                DbHelper.SetLog(o.ExceptionMessage, "Elizarovo");
            }
            Console.WriteLine(str);
            _timerE.Interval = _config.ElizarovoInterval; //Set your new interval here
            _timerE.Start();

        }
        private static void OnTimerE2(object sender, ElapsedEventArgs args)
        {
            //Do some work here
            _timerE2.Stop();
            if (_isStop) return;
            var o = new ElizarovoSimatic();
            var str = "";
            try
            {
                o.Run();
                str += o + "\r\n";
            }
            catch (Exception ex)
            {
                str += ex.Message + "\r\n";
                o.ExceptionMessage += ex.Message + "\r\n";
                DbHelper.SetLog(o.ExceptionMessage, "ElizarovoSimatic");
            }
            Console.WriteLine(str);
            _timerE2.Interval = _config.ElizarovoSimaticInterval; //Set your new interval here
            _timerE2.Start();
        }
        private static void OnTimerSL(object sender, ElapsedEventArgs args)
        {
            //Do some work here
            _timerSL.Stop();
            if (_isStop) return;
            var o = new StreamLux();
            var str = "";
            try
            {
                o.Run();
                str += o + "\r\n";
            }
            catch (Exception ex)
            {
                str += ex.Message + "\r\n";
                o.ExceptionMessage += ex.Message + "\r\n";
            }

            Console.WriteLine(str);
            _timerSL.Interval = 10000;//_config.StreamLuxInterval; //Set your new interval here
            _timerSL.Start();
        }
        //private static void OnTimerM(object sender, ElapsedEventArgs args)
        //{
        //    //Do some work here
        //    _timerM.Stop();
        //    if (_isStop) return;
        //    var merkurs = DbHelper.GetMerkurs();
        //    foreach (var o in merkurs)
        //    {
        //        //if (o.ModbusId == 79) continue;
        //        var str = "";
        //        try
        //        {

        //            o.Run();
        //            str += o + "\r\n";
        //        }
        //        catch (Exception ex)
        //        {
        //            str += ex.Message + "\r\n";
        //            o.ExceptionMessage += ex.Message + "\r\n";
        //        }
        //        Console.WriteLine(str);
        //    }

        //    _timerM.Interval = _config.MerkurInterval; //Set your new interval here
        //    _timerM.Start();
        //}

        //public static byte[] StringToByteArray(string hex)
        //{
        //    return Enumerable.Range(0, hex.Length)
        //        .Where(x => x % 2 == 0)
        //        .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
        //        .ToArray();
        //}

        //private static void OnTimerT(object sender, ElapsedEventArgs args)
        //{
        //    //Do some work here
        //    _timerT.Stop();
        //    if (_isStop) return;

        //    var str = "";
        //    try
        //    {
        //        var o = DbHelper.GetTeplosets();
        //        o.Run();
        //        str += o + "\r\n";
        //    }
        //    catch (Exception ex)
        //    {
        //        str += ex.Message + "\r\n";
        //        o.ExceptionMessage += ex.Message + "\r\n";
        //    }
        //    Console.WriteLine(str);
        //    _timerT.Interval = _config.TeplosetInterval; //Set your new interval here
        //    _timerT.Start();
        //}


        //private static void OnTimerV(object sender, ElapsedEventArgs args)
        //{
        //    //Do some work here
        //    _timerV.Stop();
        //    if (_isStop) return;
        //    var str = "";
        //    try
        //    {
        //        var v = new Vzlet();
        //        var er = "";
        //        try
        //        {
        //            v = ModbusHelper.ReadVzlet(_config.VzletComPort);
        //        }
        //        catch (Exception ex)
        //        {
        //            er = ", ErrorUpdateClient: " + ex.Message;
        //        }

        //        if (!string.IsNullOrEmpty(er))
        //            DbHelper.SetLog(er, "Vzlet");
        //        DbHelper.SetLog(v.ToString(), "Vzlet", false);
        //        str += v.ToString();
        //        str += er;
        //    }
        //    catch (Exception ex)
        //    {
        //        str += ex.Message + "\r\n";
        //    }
        //    Console.WriteLine(str);
        //    _timerV.Interval = _config.VzletInterval; //Set your new interval here
        //    _timerV.Start();
        //}

        private static void OnTimerOLW(object sender, ElapsedEventArgs e)
        {
            _timerOLW.Stop();
            if (_isStop) return;
            var str = "";
            try
            {
                var o = new OwenLiftWater();
                o.Run();
                str += o + "\r\n";
            }
            catch (Exception ex)
            {
                str += ex.Message + "\r\n";
            }
            Console.WriteLine(str);
            _timerOLW.Interval = _config.OwenLiftWaterInterval; //Set your new interval here
            _timerOLW.Start();
        }

        private static void OnTimerLW(object sender, ElapsedEventArgs e)
        {
            _timerLW.Stop();
            if (_isStop) return;
            var str = "";
            try
            {
                var o = new LW();
                o.Run();
                str += o + "\r\n";
            }
            catch (Exception ex)
            {
                str += ex.Message + "\r\n";
            }
            Console.WriteLine(str);
            _timerLW.Interval = _config.LiftWaterInterval; //Set your new interval here
            _timerLW.Start();
        }
    }
}
