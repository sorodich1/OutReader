using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutReader.Model
{
    public class GKNS_3
    {
        public GKNS_3(int val)
        {
            BitArray b = new BitArray(new int[] { val });
            DI = b.Cast<bool>().ToArray();
            _bits = string.Join("", DI.Select(x => x ? 1 : 0)).Substring(0, 8);
            //_bits = _bits.Insert(8, "_");
            _bits = _bits.Insert(4, "_");
            D1 = DI[0];
            D2 = DI[1];
            D3 = DI[2];
            D4 = DI[3];
            D5 = DI[4];
            D6 = DI[5];
            D7 = DI[6];
            D8 = DI[7];
        }
        public GKNS_3(int val, int d1p2, int d2p3)
        {
            BitArray b = new BitArray(new int[] { val });
            DI = b.Cast<bool>().ToArray();
            _bits = string.Join("", DI.Select(x => x ? 1 : 0)).Substring(0, 8);
            _bits = _bits.Insert(4, "_");
            //_bits += string.Join("p2:", DI2.Select(x => x ? 1 : 0)).Substring(0, 8);
            //_bits += "p3:" + d2p3.ToString();
            D1 = DI[0];
            D2 = DI[1];
            D3 = DI[2];
            D4 = DI[3];
            D5 = DI[4];
            D6 = DI[5];
            D7 = DI[6];
            D8 = DI[7];

        }


        private string _bits { get; set; }
        public bool[] DI { get; set; }
        //public bool D1p1 { get; set; }
        //public bool D1p2 { get; set; }
        //public bool D1p3 { get; set; }
        //public bool D1p4 { get; set; }
        //public bool D1p5 { get; set; }
        //public bool D1p6 { get; set; }
        //public bool D1p7 { get; set; }
        //public bool D1p8 { get; set; }
        //public bool D2p3 { get; set; }
        public bool D1 { get; set; }
        //public bool [] DI2 { get; set; }
        public bool D2 { get; set; }
        public bool D3 { get; set; }
        public bool D4 { get; set; }
        public bool D5 { get; set; }
        public bool D6 { get; set; }
        public bool D7 { get; set; }
        public bool D8 { get; set; }
        public override string ToString()
        {
            return _bits;
        }
        public string DiToString()
        {
            return String.Join(",", DI.Select(x => Convert.ToInt32(x)));
        }
    }
}
