using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutReader.Model
{
    public class MB16D
    {
        public MB16D(int val)
        {
            BitArray b = new BitArray(new int[]{val});
            DI = b.Cast<bool>().ToArray();
            _bits = string.Join("", DI.Select(x => x ? 1 : 0)).Substring(0,16);
            _bits = _bits.Insert(12, "_");
            _bits = _bits.Insert(8, "_");
            _bits = _bits.Insert(4, "_");
            D1 = DI[0];
            D2 = DI[1];
            D3 = DI[2];
            D4 = DI[3];
            D5 = DI[4]; 
            D6 = DI[5];
            D7 = DI[6];
            D8 = DI[7];
            D9 = DI[8];
            D10 = DI[9];
            D11 = DI[10];
            D12 = DI[11];
            D13 = DI[12];
            D14 = DI[13];
            D15 = DI[14];
            D16 = DI[15];
        }
        public MB16D(int val, int d1Counter, int d2Counter)
        {
            BitArray b = new BitArray(new int[] { val });
            DI = b.Cast<bool>().ToArray();
            _bits = string.Join("", DI.Select(x => x ? 1 : 0)).Substring(0, 16);
            _bits = _bits.Insert(12, "_");
            _bits = _bits.Insert(8, "_");
            _bits = _bits.Insert(4, "_");
            _bits += ",D1C:"+d1Counter.ToString() + ",D2C:"+ d2Counter.ToString();
            D1 = DI[0];
            D2 = DI[1];
            D3 = DI[2];
            D4 = DI[3];
            D5 = DI[4];
            D6 = DI[5];
            D7 = DI[6];
            D8 = DI[7];
            D9 = DI[8];
            D10 = DI[9];
            D11 = DI[10];
            D12 = DI[11];
            D13 = DI[12];
            D14 = DI[13];
            D15 = DI[14];
            D16 = DI[15];
            D1Counter = d1Counter;
            D2Counter = d2Counter;
        }        

        private string _bits { get; set; }
        public bool[] DI { get; set; }
        public int D1Counter { get; set; }
        public int D2Counter { get; set; }
        public bool D1 { get; set; }
        public bool D2 { get; set; }
        public bool D3 { get; set; }
        public bool D4 { get; set; }
        public bool D5 { get; set; }
        public bool D6 { get; set; }
        public bool D7 { get; set; }
        public bool D8 { get; set; }
        public bool D9 { get; set; }
        public bool D10 { get; set; }
        public bool D11 { get; set; }
        public bool D12 { get; set; }
        public bool D13 { get; set; }
        public bool D14 { get; set; }
        public bool D15 { get; set; }
        public bool D16 { get; set; }
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
