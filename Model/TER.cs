using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutReader.Model
{
    /// <summary>
    /// TER MR
    /// </summary>
    public class TER
    {
        public TER(decimal q, decimal v)
        {
            Q = q;
            V = v;
        }
        public decimal Q { get; set; }
        public decimal V { get; set; }

        public override string ToString()
        {
            return string.Format("Q:{0:F},V:{1:F}", Q.ToString(CultureInfo.InvariantCulture), V.ToString(CultureInfo.InvariantCulture));
        }
    }
}
