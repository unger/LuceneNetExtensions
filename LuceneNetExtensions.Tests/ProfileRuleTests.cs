namespace LuceneNetExtensions.Tests
{
    using Lucene.Net.Search;

    using LuceneNetExtensions.Cfg;
    using LuceneNetExtensions.Tests.Mapping;
    using LuceneNetExtensions.Tests.Model;

    using NUnit.Framework;

    public class ProfileRuleTests
    {

        private IndexManager IndexManager { get; set; }

        [SetUp]
        public void Setup()
        {
            this.IndexManager = FluentIndexConfiguration.Create()
              .IndexRootPath(null)
              .Mappings(m =>
              {
                  m.Add<SightingMap>();
                  m.Add<ProfileRuleMap>();
              })
              .BuildIndexManager();

            var writer = this.IndexManager.GetWriter<Sighting>();

            writer.AddOrUpdateDocument(new Sighting { SpeciesName = "Praktejder", Municipality = "Göteborg", Province = "Bohuslän" });
            writer.AddOrUpdateDocument(new Sighting { SpeciesName = "Praktejder", Municipality = "Uppsala", Province = "Uppland" });
            writer.AddOrUpdateDocument(new Sighting { SpeciesName = "Praktejder", Municipality = "Varberg", Province = "Halland" });
            writer.Optimize();
            writer.Commit();
        }

        [Test]
        public void SearchWithProfileRule()
        {
            var qh = this.IndexManager.GetQueryHelper<Sighting>();
            int totalHits;

            var rule = new ProfileRule
                    {
                        Species = new[] { "Praktejder", "Vitnackad svärta" },
                        Provinces = new[] { "Bohuslän" }
                    };

            var profileQuery = new BooleanQuery();

            var speciesQuery = qh.CreateTermQuery(s => s.SpeciesName, rule.Species);
            if (speciesQuery != null)
            {
                profileQuery.Add(speciesQuery, Occur.MUST);
            }

            var provinceQuery = qh.CreateTermQuery(s => s.Province, rule.Provinces);
            if (provinceQuery != null)
            {
                profileQuery.Add(provinceQuery, Occur.MUST);
            }

            var muncipalityQuery = qh.CreateTermQuery(s => s.Municipality, rule.Municipalities);
            if (muncipalityQuery != null)
            {
                profileQuery.Add(muncipalityQuery, Occur.MUST);
            }

            using (var searcher = this.IndexManager.GetSearcher<Sighting>())
            {
                var result = searcher.Search(profileQuery);
                totalHits = result.TotalHits;
            }

            Assert.AreEqual(1, totalHits);
        }
    }
}
