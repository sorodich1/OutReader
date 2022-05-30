namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;

    public partial class Alarm
    {
        public int Id { get; set; }

        public int AlarmTypeId { get; set; }

        public bool IsState { get; set; }

        public DateTime Sysdate { get; set; }

        public bool IsConfirmed { get; set; }

        public virtual AlarmType AlarmType { get; set; }
        //public void GetAlarmId()
        //{
        //    AlarmTypeId=DbHelper.GetLWAlarmId("");
        //}
        //insert into tablename (code) Select '1448523' Where not exists(select * from tablename where code='1448523')
        public string ToSql()
        {
            return string.Format("IF not exists(select * FROM Alarms WHERE AlarmTypeId={0} and IsState = {1} and Sysdate >= (SELECT TOP 1 Sysdate FROM Alarms WHERE AlarmTypeId={0} Order by Sysdate desc)) INSERT INTO Alarms (AlarmTypeId, IsState, Sysdate, IsConfirmed) VALUES({0}, {1}, '{2:yyyyMMdd HH:mm:ss}', {3});", AlarmTypeId, IsState ? 1 : 0, Sysdate, IsConfirmed ? 1 : 0);
        }
    }
}
