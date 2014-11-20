namespace LuceneNetExtensions.Tests.Mapping
{
    using LuceneNetExtensions.Mapping;
    using LuceneNetExtensions.Tests.Model;

    public class SightingMap : IndexClassMap<Sighting>
    {
        public SightingMap()
        {
            this.Index("Sightings");

            this.Map(s => s.SpeciesName, "Artnamn");
            this.Map(s => s.Province, "Landskap");
            this.Map(s => s.Municipality, "Kommun");
        }
    }
}
