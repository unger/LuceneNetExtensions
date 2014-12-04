namespace LuceneNetExtensions.Sample.Index
{
    using LuceneNetExtensions.Analyzers;
    using LuceneNetExtensions.Mapping;
    using LuceneNetExtensions.Sample.Models;

    public class SightingMap : IndexClassMap<Sighting>
    {
        public SightingMap()
        {
            this.IndexName("Sightings");
            this.Analyzer(new LowerCaseKeywordAnalyzer());

            this.Id(s => s.Id);

            this.Map(s => s.SpeciesName).Analyzed();
            this.Map(s => s.Municipality).Analyzed();
            this.Map(s => s.Province).Analyzed();
            this.Map(s => s.Parish).Analyzed();
            this.Map(s => s.Site).Analyzed();

            this.MapPublicProperties();
        }
    }
}
