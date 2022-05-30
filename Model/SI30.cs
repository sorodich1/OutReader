using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutReader.Model
{
    public class SI30
    {
        public SI30(decimal v)
        {
            V = v;
        }
        public decimal V { get; set; }

        public override string ToString()
        {
            return string.Format("V:{0:F}", V.ToString(CultureInfo.InvariantCulture));
        }
    }
}
