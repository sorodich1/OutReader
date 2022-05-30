namespace OutReader.Model.LiftWater
{
    using System;
    using System.Collections.Generic;

    public partial class Error
    {
        public int Id { get; set; }
        
        public string FullText { get; set; }

        public DateTime Sysdate { get; set; }
    }
}
