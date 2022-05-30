namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;

    public partial class Dos
    {
        public Dos()
        {
            DoseDatas = new HashSet<DoseData>();
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<DoseData> DoseDatas { get; set; }
    }
}
