namespace LuceneNetExtensions.Tests
{
    using System.Collections.Generic;

    using Lucene.Net.Analysis.Standard;
    using Lucene.Net.Index;
    using Lucene.Net.QueryParsers;
    using Lucene.Net.Search;
    using Lucene.Net.Util;

    using LuceneNetExtensions.Cfg;
    using LuceneNetExtensions.Mapping;
    using LuceneNetExtensions.Tests.Model;

    using NUnit.Framework;

    [TestFixture]
    public class BasicTests
    {
        private IndexManager IndexManager { get; set; }

        public class SightingMap : IndexClassMap<Sighting>
        {
            public SightingMap()
            {
                
            }
        }

        [SetUp]
        public void Setup()
        {
            this.IndexManager = FluentIndexConfiguration.Create()
              .IndexRootPath(null)
              .Mappings(m =>
              {
                  m.Add<SightingMap>();
              })
              .BuildIndexManager();

            var writer = this.IndexManager.GetWriter<Sighting>();

            writer.AddDocument(new Sighting { SpeciesName = "Praktejder", Municipality = "Göteborg", Province = "Bohuslän" });
            writer.AddDocument(new Sighting { SpeciesName = "Praktejder", Municipality = "Uppsala", Province = "Uppland" });
            writer.AddDocument(new Sighting { SpeciesName = "Praktejder", Municipality = "Varberg", Province = "Halland" });

            writer.Optimize();
        }

        [Test]
        public void Test()
        {
            var query = new TermQuery(new Term("SpeciesName", "Praktejder"));
            int totalHits;

            using (var searcher = this.IndexManager.GetSearcher<Sighting>())
            {
                var result = searcher.Search(query);
                totalHits = result.TotalHits;
            }

            Assert.AreEqual(3, totalHits);
        }

        [Test]
        public void Test2()
        {

            var query = new BooleanQuery { { new TermQuery(new Term("SpeciesName", "Praktejder")), Occur.MUST } };

            var multiShould = new BooleanQuery
                                  {
                                      { new TermQuery(new Term("Province", "Bohuslän")), Occur.SHOULD },
                                      { new TermQuery(new Term("Province", "Halland")), Occur.SHOULD }
                                  };
            query.Add(multiShould, Occur.MUST);

            int totalHits;

            using (var searcher = this.IndexManager.GetSearcher<Sighting>())
            {
                var result = searcher.Search(query);
                totalHits = result.TotalHits;
            }

            Assert.AreEqual(2, totalHits);
        }

        [TearDown]
        public void TearDown()
        {
            this.IndexManager.Dispose();
        }
    }
}
