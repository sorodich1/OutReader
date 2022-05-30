using System;
using System.Collections.Generic;
using System.Linq;
using OutReader.Helper;
using System.Globalization;

namespace OutReader.Model
{
    public class Tn
    {
        public Tn()
        {
            DI = new List<bool>(){false,false,false,false,false,false};
        }
        public Tn(List<bool> di, List<byte[]> b)
        {
            DI = di;
            Ai1 = Math.Round(ConverterHelper.ByteToReal(b[0], b[1]),2);
            Ai2 = Math.Round(ConverterHelper.ByteToReal(b[2], b[3]),2);
            Ai3 = Math.Round(ConverterHelper.ByteToReal(b[4], b[5]),2);
            Au1 = Math.Round(ConverterHelper.ByteToReal(b[6], b[7]),2);
            Au2 = Math.Round(ConverterHelper.ByteToReal(b[8], b[9]),2);
            Au3 = Math.Round(ConverterHelper.ByteToReal(b[10], b[11]), 2);
        }
        public double Ai1 { get; set; }
        public double Ai2 { get; set; }
        public double Ai3 { get; set; }
        public double Au1 { get; set; }
        public double Au2 { get; set; }
        public double Au3 { get; set; }
        public List<bool> DI { get; set; }

        public string DiToString()
        {
            return String.Join(",", DI.Select(x => Convert.ToInt32(!x)));
        }

        public override string ToString()
        {
            return string.Format("Ai1:{0} Ai2:{1} Ai3:{2} Au1:{3} Au2:{4} Au3:{5}", Ai1, Ai2, Ai3, Au1, Au2, Au3);
        }

        public string InsertQuery(string knsId, DateTime dt)
        {
            return string.Format("IF NOT EXISTS(SELECT * FROM TeleskopTns tt WHERE tt.TeleskopId=(SELECT TOP 1 Id FROM Teleskops WHERE Name='{1}') AND A={2} AND B={3} AND C={4}) " +
                "Insert TeleskopTns (Sysdate, A, B, C, TeleskopId) VALUES ('{0}', {2}, {3}, {4}, (SELECT TOP 1 Id FROM Teleskops WHERE Name='{1}'));", dt, knsId, Au1.ToString(CultureInfo.InvariantCulture), Au2.ToString(CultureInfo.InvariantCulture), Au3.ToString(CultureInfo.InvariantCulture));
        }
    }
}
