namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;

    public partial class Compressor
    {
        public Compressor()
        {
            CompressorDatas = new HashSet<CompressorData>();
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<CompressorData> CompressorDatas { get; set; }
    }
}
