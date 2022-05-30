using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutReader.Model
{
    public class Vzlet:Object
    {
        public Vzlet(byte[] q1, byte[] q2, byte[] v1, byte[] v2)
        {
            bytes = new List<byte>();
            bytes.AddRange(q1);
            bytes.AddRange(q2);
            bytes.AddRange(v1);
            bytes.AddRange(v2);
        }
        public decimal Q1 { get; set; }
        public decimal Q2 { get; set; }
        public decimal V1 { get; set; }
        public decimal V2 { get; set; }
        public List<byte> bytes;

        public override string ToString()
        {
            var res = string.Format("Vzlet: {0}, Q1={1}, Q2={2}, V1={3}, V2={4}, Hex={5}", DateTime.Now, Q1,Q2,V1,V2, string.Join(" ", bytes));
            return res;
        }
    }
}
