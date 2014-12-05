namespace LuceneNetExtensions.Tests
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Lucene.Net.Analysis;
    using Lucene.Net.Analysis.Standard;
    using Lucene.Net.Search;

    using LuceneNetExtensions.Cfg;
    using LuceneNetExtensions.Mapping;
    using LuceneNetExtensions.Tests.Mapping;
    using LuceneNetExtensions.Tests.Model;

    using NUnit.Framework;

    [TestFixture]
    public class CollectionPropertyTests
    {
        private IndexManager IndexManager { get; set; }

        [SetUp]
        public void Setup()
        {
            this.IndexManager = FluentIndexConfiguration.Create()
              .IndexRootPath(null)
              .Mappings(m =>
              {
                  m.Add<MyCollectionClassMap>();
              })
              .BuildIndexManager();

            var writer = this.IndexManager.GetWriter<MyCollectionClass>();

            writer.AddOrUpdateDocument(new MyCollectionClass
                                           {
                                               ArrayItems = new[] { "Item1", "Item2", "Item3" },
                                               ListItems = new List<string> { "Item1", "Item2", "Item3" },
                                               HashItems = new HashSet<string> { "Item1", "Item2", "Item3" },
                                           });

            writer.Commit();
            writer.Optimize();

        }

        [Test]
        public void GetArrayPropertyReturnSameNumberAsIndexed()
        {
            var query = new MatchAllDocsQuery();
            MyCollectionClass element;
            using (var searcher = this.IndexManager.GetSearcher<MyCollectionClass>())
            {
                element = searcher.Search(query).FirstOrDefault();
            }

            Assert.NotNull(element);
            Assert.AreEqual(3, element.ArrayItems.Length);
        }

        [Test]
        public void GetListPropertyReturnSameNumberAsIndexed()
        {
            var query = new MatchAllDocsQuery();
            MyCollectionClass element;
            using (var searcher = this.IndexManager.GetSearcher<MyCollectionClass>())
            {
                element = searcher.Search(query).FirstOrDefault();
            }

            Assert.NotNull(element);
            Assert.AreEqual(3, element.ListItems.Count);
        }

        [Test]
        public void GetHashSetPropertyReturnSameNumberAsIndexed()
        {
            var query = new MatchAllDocsQuery();
            MyCollectionClass element;
            using (var searcher = this.IndexManager.GetSearcher<MyCollectionClass>())
            {
                element = searcher.Search(query).FirstOrDefault();
            }

            Assert.NotNull(element);
            Assert.AreEqual(3, element.HashItems.Count);
        }


        private class MyCollectionClassMap : IndexClassMap<MyCollectionClass>
        {
            public MyCollectionClassMap()
            {
                this.MapPublicProperties();
            }
        }

        private class MyCollectionClass
        {
            public string[] ArrayItems { get; set; }

            public List<string> ListItems { get; set; }

            public HashSet<string> HashItems { get; set; }
        }

    }

}
