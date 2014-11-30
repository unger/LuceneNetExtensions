namespace LuceneNetExtensions.Sample.Models
{
    using System;

    public class Sighting
    {
        public int Id { get; set; }

        public string SpeciesName { get; set; }

        public int? Number { get; set; }

        public string Sex { get; set; }

        public string Age { get; set; }

        public string Activity { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public string Site { get; set; }

        public string SuperSite { get; set; }

        public string Municipality { get; set; }

        public string Parish { get; set; }

        public string Province { get; set; }

        public int Xkoord { get; set; }
        
        public int Ykoord { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public string Observer { get; set; }

        public string Kommentar { get; set; }

        public bool Ospontan { get; set; }

        public bool Unsure { get; set; }
    }
}
