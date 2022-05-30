using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutReader.Model
{
    public class SBI
    {
        public SBI(int u, int i, int state, string error, string errorCommunication, int modbusId, string knsId, DateTime dt)
        {
            U = u*0.1m;
            I = i * 0.1m;
            State = state;
            Error = error;
            ErrorCommunication = errorCommunication;
            ModbusId = modbusId;
            KNSId = knsId;
            Sysdate = dt;
        }
        public decimal U { get; set; }
        public decimal I { get; set; }
        public int State { get; set; }
        public int ModbusId { get; set; }
        public string KNSId { get; set; }
        public string Error { get; set; }
        public string ErrorCommunication { get; set; }
        public DateTime Sysdate { get; set; }

        public bool IsRun
        {
            get { return State == 2 || State == 3; }
        }

        public bool IsError
        {
            get { return State == 1; }
        }

        public string GetQueryDb()
        {
            return string.Format("IF NOT EXISTS(SELECT * FROM TeleskopSBIs WHERE ID IN(SELECT MAX(Id) FROM TeleskopSBIs WHERE TeleskopId = (SELECT TOP 1 Id FROM Teleskops WHERE Name='{0}') AND ModbusId={1}) AND State={2} AND U={3} AND I={4} AND Error ='{5}')" +
                                 "  INSERT TeleskopSBIs (TeleskopId,ModbusId,  State, U, I, Error, ErrorCommunication, Sysdate) " +
                                 "SELECT TOP 1 Id, {1}, {2}, {3}, {4}, '{5}', '{6}', '{7:yyyyMMdd hh:mm:ss}'  FROM Teleskops WHERE Name='{0}';\r\n",
                KNSId, ModbusId, State, U.ToString(CultureInfo.InvariantCulture), I.ToString(CultureInfo.InvariantCulture), Error, ErrorCommunication, Sysdate);
        }

        public override string ToString()
        {
            return string.Format("I:{0:F},U:{1:F},State:{2},Error:{3},ErrorCom:{4}", I.ToString(CultureInfo.InvariantCulture), U.ToString(CultureInfo.InvariantCulture), State, Error, ErrorCommunication);
        }
    }
}
