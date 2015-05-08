namespace LuceneNetExtensions.Console
{
    using LuceneNetExtensions.Mapping;

    public class SightingMap : IndexClassMap<Sighting>
    {
        public SightingMap()
        {
            this.IndexName("Sightings");

            this.Map(s => s.SpeciesName, "Artnamn");
            this.Map(s => s.Province, "Landskap");
            this.Map(s => s.Municipality, "Kommun");
        }
    }
}
