namespace LuceneNetExtensions.Sample.Index
{
    using LuceneNetExtensions.Mapping;
    using LuceneNetExtensions.Sample.Models;

    public class SightingMap : IndexClassMap<Sighting>
    {
        public SightingMap()
        {
            this.IndexName("Sightings");
            this.MapPublicProperties();
        }
    }
}
