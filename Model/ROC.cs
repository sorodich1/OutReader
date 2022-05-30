using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutReader.Model
{
    public class ROC
    {
        public ROC(int sta1, int sta2, decimal s1, decimal s2, decimal t)
        {
            S1 = s1;
            S2 = s2;
            Temp = t;
            BitArray b = new BitArray(new int[] { sta1 });
            var di2 = b.Cast<bool>().ToArray();
            D1 = di2[0];
            D2 = di2[1];
            D3 = di2[2];
            D4 = di2[3];
            D5 = di2[4];
            D6 = di2[5];
            D7 = di2[6];
            _di = string.Join("", di2.Select(x => x ? 1 : 0)).Substring(0, 8);
            _di = _di.Insert(4, "_");

            b = new BitArray(new int[] { sta2 });
            var q2 = b.Cast<bool>().ToArray();
            Q1 = q2[0];
            Q2 = q2[1];
            Q3 = q2[2];
            Q4 = q2[3];
            Q5 = q2[4];
            Q6 = q2[5];
            _q = string.Join("", q2.Select(x => x ? 1 : 0)).Substring(0, 8);
            _q = _di.Insert(4, "_");
        }
        private string _di { get; set; }
        private string _q { get; set; }
        public decimal S1 { get; set; }
        public decimal S2 { get; set; }
        public decimal Temp { get; set; }
        public bool D1 { get; set; }
        public bool D2 { get; set; }
        public bool D3 { get; set; }
        public bool D4 { get; set; }
        public bool D5 { get; set; }
        public bool D6 { get; set; }
        public bool D7 { get; set; }
        public bool Q1 { get; set; }
        public bool Q2 { get; set; }
        public bool Q3 { get; set; }
        public bool Q4 { get; set; }
        public bool Q5 { get; set; }
        public bool Q6 { get; set; }

        public override string ToString()
        {
            return string.Format("STA1:{0} STA2(Q):{1} S1:{2} S2:{3} Temp:{4}", _di, _q, S1, S2, Temp);
        }
    }
}
