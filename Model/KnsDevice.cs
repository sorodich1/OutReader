using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutReader.Model
{
    public class KnsDevice
    {
        public KnsDevice(int modbusId, string name)
        {
            ModbusId = modbusId;
            Name = name;
        }
        public int ModbusId { get; set; }
        public string Name { get; set; }

        public bool IsMB16D {
            get { return Name == "MB16D"; }
        }
        public bool IsMB8A { get { return Name == "MB8A"; } }
        public bool IsTER { get { return Name == "TER"; } }
        public bool IsMR { get { return Name == "MR"; } }
        /// <summary>
        /// Instar иь
        /// </summary>
        public bool IsSBI { get { return Name == "SBI"; } }
        /// <summary>
        /// Analog
        /// </summary>
        public bool IsTn4017 { get { return Name == "Tn4017"; } }
        /// <summary>
        /// DI
        /// </summary>
        public bool IsTn4050 { get { return Name == "Tn4050"; } }
        public bool IsMe3M { get { return Name == "ME3M"; } }
    }
}
