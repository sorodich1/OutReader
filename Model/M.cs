using System;
using System.Collections.Generic;
using System.Linq;
using OutReader.Helper;

namespace OutReader.Model
{
    public class M
    {
        public M()
        {
            DI = new List<bool>() { false, false, false, false, false, false, false, false };
        }
        public M(List<byte[]> b)
        {
            DI = new List<bool>() { false, false, false, false, false, false, false, false };
            A1 = ConverterHelper.ByteToDecimal(b[0], b[1]);
            A2 = ConverterHelper.ByteToDecimal(b[2], b[3]);
            A3 = ConverterHelper.ByteToDecimal(b[4], b[5]);
            A4 = ConverterHelper.ByteToDecimal(b[6], b[7]);
        }
        public M(List<bool> di, List<byte[]> b)
        {
            DI = di;
            A1 = ConverterHelper.ByteToDecimal(b[0], b[1]);
            A2 = ConverterHelper.ByteToDecimal(b[2], b[3]);
            A3 = ConverterHelper.ByteToDecimal(b[4], b[5]);
            A4 = ConverterHelper.ByteToDecimal(b[6], b[7]);
        }
        public decimal A1 { get; set; }
        public decimal A2 { get; set; }
        public decimal A3 { get; set; }
        public decimal A4 { get; set; }

        public List<bool> DI { get; set; }

        public string DiToString()
        {
            return String.Join(",", DI.Select(x => Convert.ToInt32(!x)));
        }

        public override string ToString()
        {
            return string.Format("M: A_{0}_{1}_{2}_{3} DI_{4}",A1,A2,A3,A4, DiToString());
        }
    }
}
