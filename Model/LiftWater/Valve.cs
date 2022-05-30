namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;
    public partial class Valve
    {
        public Valve()
        {
            ValveDatas = new HashSet<ValveData>();
        }

        public int Id { get; set; }

        
        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<ValveData> ValveDatas { get; set; }
    }
}
