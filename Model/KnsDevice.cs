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
        public bool IsMB8A_KRESTY { get { return Name == "MB8A_KRESTY"; } }
        public bool IsMB8A_OBEH { get { return Name == "MB8A_OBEH"; } }
        public bool IsOBEH { get { return Name == "OBEH"; } } //true
        public bool IsOBEH_2 { get { return Name == "OBEH_2"; } } //true
        public bool IsOBEH_3 { get { return Name == "OBEH_3"; } } //true
        public bool IsOBEH_VRU { get { return Name == "OBEH_VRU"; } } //true
        public bool IsOBEH_Alarm { get { return Name == "OBEH_Alarm"; } } //true
        public bool IsOBEH_level { get { return Name == "OBEH_level"; } } //true

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
