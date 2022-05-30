using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutReader.Model
{
    public class MB8A
    {
        public MB8A() { }
        public MB8A(decimal ai1, decimal ai2, decimal ai3, decimal ai4, decimal ai5, decimal ai6, decimal ai7, decimal ai8)
        {
            A1 = ai1;
            A2 = ai2;
            A3 = ai3;
            A4 = ai4;
            A5 = ai5;
            A6 = ai6;
            A7 = ai7;
            A8 = ai8;
        }        
        public decimal A1 { get; set; }
        public decimal A2 { get; set; }
        public decimal A3 { get; set; }
        public decimal A4 { get; set; }
        public decimal A5 { get; set; }
        public decimal A6 { get; set; }
        public decimal A7 { get; set; }
        public decimal A8 { get; set; }
        public override string ToString()
        {
            return string.Format("A1:{0:F},A2:{1:F},A3:{2:F},A4:{3:F},A5:{4:F},A6:{5:F},A7:{6:F},A8:{7:F}", A1.ToString(CultureInfo.InvariantCulture), A2.ToString(CultureInfo.InvariantCulture), A3.ToString(CultureInfo.InvariantCulture), A4.ToString(CultureInfo.InvariantCulture), A5.ToString(CultureInfo.InvariantCulture), A6.ToString(CultureInfo.InvariantCulture), A7.ToString(CultureInfo.InvariantCulture), A8.ToString(CultureInfo.InvariantCulture));
        }
    }
}
