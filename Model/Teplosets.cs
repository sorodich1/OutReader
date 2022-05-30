//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Sockets;
//using Modbus.Device;
//using Modbus.IO;
//using OutReader.Helper;
//using ScxV6DbClient;

//namespace OutReader.Model
//{
//    public class Teplosets:IObject
//    {
//        public Teplosets()
//        {
//            Kots = new List<Teploset>();
//        }
//        public DateTime LastUpdate { get; set; }
//        public bool NotConnection { get { return !IsKots || IsExceptionClient; } }
//        public bool IsExceptionClient { get; set; }
//        public string ExceptionMessage { get; set; }
//        public DateTime LastSaveToScada { get; set; }
//        public List<Teploset> Kots { get; set; }
//        public bool IsKots { get { return Kots != null && Kots.Count > 0; } }

//        public void SaveToSql()
//        {
//            throw new NotImplementedException();
//        }

//        public void SaveLogToSql()
//        {
//            if (!string.IsNullOrEmpty(ExceptionMessage))
//                DbHelper.SetLog(ExceptionMessage, this.GetType().Name);
//            DbHelper.SetLog(ToString(), this.GetType().Name, false);
//        }

//        public void ConvertM()
//        {
//            throw new NotImplementedException();
//        }

//        public void Update()
//        {
//            ExceptionMessage = "";
//            try
//            {
//                HtmlHelper.GetTeplosets(this, ExceptionMessage);
//            }
//            catch (Exception ex)
//            {
//                IsExceptionClient = true;
//                ExceptionMessage = "ErrorUpdateClient: " + ex.Message;
//            }
//            LastUpdate = DateTime.Now;
//        }

//        public void Run()
//        {
//            Update();
//            if(Kots.Where(x=>x.Flow > 0 || x.Press > 0).Count() > 0)
//                SaveToScada();
//            SaveLogToSql();
//        }

//        public void SaveToScada()
//        {
//            try
//            {
//                ScadaHelper.SetTeploseti(this);
//            }
//            catch (Exception ex)
//            {
//                ExceptionMessage = (string.IsNullOrEmpty(ExceptionMessage) ? "" : "\r\n") + "ErrorScada: " + ex.Message;
//            }
//            LastSaveToScada = DateTime.Now;
//        }

//        public override string ToString()
//        {
//            var res = string.Format("Teploset: {0},{1} \r\n {2}", LastUpdate, LastSaveToScada, ExceptionMessage);
//            return res;
//        }
//    }
//}
