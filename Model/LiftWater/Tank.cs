namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;

    public partial class Tank
    {
        public Tank()
        {
            TankDatas = new HashSet<TankData>();
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<TankData> TankDatas { get; set; }
    }
}
