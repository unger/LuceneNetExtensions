namespace LuceneNetExtensions.Sample.Models
{
    using System;

    public class Site
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        public string SuperSite { get; set; }

        public string Municipality { get; set; }

        public string Parish { get; set; }

        public string Province { get; set; }

        public int Xkoord { get; set; }

        public int Ykoord { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public int UseCount { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}