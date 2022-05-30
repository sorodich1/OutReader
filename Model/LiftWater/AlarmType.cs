namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;

    public partial class AlarmType
    {
        public AlarmType()
        {
            Alarms = new HashSet<Alarm>();
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public virtual ICollection<Alarm> Alarms { get; set; }
    }
}
