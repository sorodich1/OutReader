using OutReader.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutReader.Model
{
    public class ObjectReader:IObject
    {

        public DateTime LastUpdate { get; set; }
        public DateTime LastSaveToScada { get; set; }

        public virtual bool NotConnection
        {
            get { return false; }
        }

        public virtual bool IsExceptionClient { get; set; }

        public virtual string ExceptionMessage { get; set; }

        public virtual void Run()
        {
            RunWithSQL();
        }
        protected void RunWithSQL()
        {
            Update();
            if (string.IsNullOrEmpty(ExceptionMessage))
                SaveToSql();
            SaveLogToSql();
        }

        protected void RunWithScada()
        {
            Update();
            if (string.IsNullOrEmpty(ExceptionMessage))
                SaveToScada();            
            SaveLogToSql();
        }



        public virtual void SaveToScada()
        {
            throw new NotImplementedException();
        }

        public virtual void SaveToSql()
        {
            throw new NotImplementedException();
        }

        public virtual void SaveLogToSql()
        {
            if (!string.IsNullOrEmpty(ExceptionMessage))
                DbHelper.SetLog(ExceptionMessage, this.GetType().Name);
            DbHelper.SetLog(ToString(), this.GetType().Name, false);
        }

        public virtual void ConvertM()
        {
            throw new NotImplementedException();
        }

        public virtual void Update()
        {
            throw new NotImplementedException();
        }
    }

}
