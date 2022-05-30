using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutReader.Model
{
    public class PR200
    {
        public PR200(int di, int q, decimal a1, decimal a2, decimal a3, decimal a4)
        {
            A1 = a1;
            A2 = a2;
            A3 = a3;
            A4 = a4;
            BitArray b = new BitArray(new int[] { di });
            var di2 = b.Cast<bool>().ToArray();
            D1 = di2[0];
            D2 = di2[1];
            D3 = di2[2];
            D4 = di2[3];
            D5 = di2[4];
            D6 = di2[5];
            D7 = di2[6];
            D8 = di2[7];
            _di = string.Join("", di2.Select(x => x ? 1 : 0)).Substring(0, 8);
            _di = _di.Insert(4, "_");

            b = new BitArray(new int[] { q });
            var q2 = b.Cast<bool>().ToArray();
            Q1 = q2[0];
            Q2 = q2[1];
            Q3 = q2[2];
            Q4 = q2[3];
            Q5 = q2[4];
            Q6 = q2[5];
            Q7 = q2[6];
            Q8 = q2[7];
            _q = string.Join("", q2.Select(x => x ? 1 : 0)).Substring(0, 8);
            _q = _q.Insert(4, "_");
        }
        private string _di { get; set; }
        private string _q { get; set; }
        public bool D1 { get; set; }
        public bool D2 { get; set; }
        public bool D3 { get; set; }
        public bool D4 { get; set; }
        public bool D5 { get; set; }
        public bool D6 { get; set; }
        public bool D7 { get; set; }
        public bool D8 { get; set; }
        public decimal A1 { get; set; }
        public decimal A2 { get; set; }
        public decimal A3 { get; set; }
        public decimal A4 { get; set; }
        public bool Q1 { get; set; }
        public bool Q2 { get; set; }
        public bool Q3 { get; set; }
        public bool Q4 { get; set; }
        public bool Q5 { get; set; }
        public bool Q6 { get; set; }
        public bool Q7 { get; set; }
        public bool Q8 { get; set; }
        public override string ToString()
        {
            return string.Format("DI:{0} Q:{1} A1:{2} A2:{3} A3:{4} A4:{5}",_di,_q, A1,A2,A3,A4);
        }
    }
}
