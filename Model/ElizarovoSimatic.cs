using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Modbus.Device;
using Modbus.IO;
using OutReader.Helper;
using ScxV6DbClient;

namespace OutReader.Model
{
    public class ElizarovoSimatic:IObject
    {
        public ElizarovoSimatic()
        {
            Tags = new List<Tag>();
        }
        public DateTime LastUpdate { get; set; }
        public bool NotConnection { get { return !IsTags || IsExceptionClient; } }
        public bool IsExceptionClient { get; set; }
        public string ExceptionMessage { get; set; }
        public DateTime LastSaveToScada { get; set; }
        public List<Tag> Tags { get; set; }
        public bool IsTags { get { return Tags != null && Tags.Count > 0; } }

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
            try
            {
                Tags = HtmlHelper.GetTags(ExceptionMessage);
            }
            catch (Exception ex)
            {
                IsExceptionClient = true;
                ExceptionMessage = "ErrorUpdateClient: " + ex.Message;
            }
            LastUpdate = DateTime.Now;
        }

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
                ScadaHelper.SetElizarovoSimaticClient(this);
            }
            catch (Exception ex)
            {
                ExceptionMessage = (string.IsNullOrEmpty(ExceptionMessage) ? "" : "\r\n") + "ErrorScada: " + ex.Message;
            }
            LastSaveToScada = DateTime.Now;
        }

        public override string ToString()
        {
            var res = string.Format("ElizarovoSimatic: {0},{1}", LastUpdate, LastSaveToScada);
            foreach (var t in Tags)
            {
                res += String.Format(",{0}={1}", t.Name, t.Value.GetType() == typeof(bool) ? Convert.ToInt32(t.Value).ToString() : t.Value);
            }
            res += String.Format(" \r\n {0}", ExceptionMessage);
            return res;
        }
    }
}
