namespace LuceneNetExtensions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Lucene.Net.Search;

    using LuceneNetExtensions.Cfg;
    using LuceneNetExtensions.Tests.Mapping;
    using LuceneNetExtensions.Tests.Model;

    using NUnit.Framework;

    [TestFixture]
    public class BasicTests
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
            var qh = this.IndexManager.GetQueryHelper<Sighting>();

            var query = new TermQuery(qh.CreateTerm(s => s.SpeciesName, "Praktejder"));
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
            var qh = this.IndexManager.GetQueryHelper<Sighting>();

            var query = new BooleanQuery { { new TermQuery(qh.CreateTerm(s => s.SpeciesName, "Praktejder")), Occur.MUST } };

            var multiShould = new BooleanQuery
                                  {
                                      { new TermQuery(qh.CreateTerm(s => s.Province, "Bohuslän")), Occur.SHOULD },
                                      { new TermQuery(qh.CreateTerm(s => s.Province, "Halland")), Occur.SHOULD }
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

        [Test]
        public void Test3()
        {
            var qh = this.IndexManager.GetQueryHelper<Sighting>();

            var sort = qh.CreateSort(qh.CreateSortField(s => s.Province, true));

            var query = qh.CreateTermQuery(s => s.SpeciesName, "Praktejder");
            int totalHits;

            using (var searcher = this.IndexManager.GetSearcher<Sighting>())
            {
                var result = searcher.Search(query, null, 1000, sort);
                totalHits = result.TotalHits;
            }

            Assert.AreEqual(3, totalHits);
        }

        [Test]
        public void Test4()
        {
            var qh = this.IndexManager.GetQueryHelper<Sighting>();

            var sort = qh.CreateSort(qh.CreateSortField(s => s.Province, true));

            var filter = new FieldCacheTermsFilter(qh.GetFieldName(s => s.Province), "Halland");

            var query = qh.CreateTermQuery(s => s.SpeciesName, "Praktejder");
            int totalHits;

            using (var searcher = this.IndexManager.GetSearcher<Sighting>())
            {
                var result = searcher.Search(query, filter, 1000, sort);
                totalHits = result.TotalHits;
            }

            Assert.AreEqual(1, totalHits);
        }

        [Test]
        public void Test5()
        {
            var qh = this.IndexManager.GetQueryHelper<Sighting>();

            var query = qh.CreateTermQuery(s => s.SpeciesName, "Praktejder");
            int totalHits;
            List<Sighting> sightings;

            using (var searcher = this.IndexManager.GetSearcher<Sighting>())
            {
                var result = searcher.Search(query, null, 1000, null);

                sightings = result.GetPage(1).ToList();
            }

            Assert.AreEqual(3, sightings.Count);
        }

        [TearDown]
        public void TearDown()
        {
            this.IndexManager.Dispose();
        }
    }
}
