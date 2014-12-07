namespace LuceneNetExtensions.Tests
{
    using System;
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

            this.IndexManager.GetWriter<MyCollectionClass>().DeleteAll();
        }

        [Test]
        public void GetStringArrayPropertyReturnSameNumberAsIndexed()
        {
            // Arrange
            var writer = this.IndexManager.GetWriter<MyCollectionClass>();
            writer.AddOrUpdateDocument(new MyCollectionClass
            {
                StringArrayItems = new[] { "Item1", "Item2", "Item3" },
            });
            writer.Commit();

            // Act
            var query = new MatchAllDocsQuery();
            MyCollectionClass element;
            using (var searcher = this.IndexManager.GetSearcher<MyCollectionClass>())
            {
                element = searcher.Search(query).FirstOrDefault();
            }

            // Assert
            Assert.NotNull(element);
            Assert.AreEqual(3, element.StringArrayItems.Length);
        }

        [Test]
        public void GetStringListPropertyReturnSameNumberAsIndexed()
        {
            // Arrange
            var writer = this.IndexManager.GetWriter<MyCollectionClass>();
            writer.AddOrUpdateDocument(new MyCollectionClass
            {
                StringListItems = new List<string> { "Item1", "Item2", "Item3" },
            });
            writer.Commit();

            // Act
            var query = new MatchAllDocsQuery();
            MyCollectionClass element;
            using (var searcher = this.IndexManager.GetSearcher<MyCollectionClass>())
            {
                element = searcher.Search(query).FirstOrDefault();
            }

            // Assert
            Assert.NotNull(element);
            Assert.AreEqual(3, element.StringListItems.Count);
        }

        [Test]
        public void GetStringHashSetPropertyReturnSameNumberAsIndexed()
        {
            // Arrange
            var writer = this.IndexManager.GetWriter<MyCollectionClass>();
            writer.AddOrUpdateDocument(new MyCollectionClass
            {
                StringHashItems = new HashSet<string> { "Item1", "Item2", "Item3" },
            });
            writer.Commit();

            // Act
            var query = new MatchAllDocsQuery();
            MyCollectionClass element;
            using (var searcher = this.IndexManager.GetSearcher<MyCollectionClass>())
            {
                element = searcher.Search(query).FirstOrDefault();
            }

            // Assert
            Assert.NotNull(element);
            Assert.AreEqual(3, element.StringHashItems.Count);
        }

        [Test]
        public void GetIntArrayPropertyReturnSameNumberAsIndexed()
        {
            // Arrange
            var writer = this.IndexManager.GetWriter<MyCollectionClass>();
            writer.AddOrUpdateDocument(new MyCollectionClass
            {
                IntArrayItems = new int[] { 1, 2, 3 },
            });
            writer.Commit();

            // Act
            var query = new MatchAllDocsQuery();
            MyCollectionClass element;
            using (var searcher = this.IndexManager.GetSearcher<MyCollectionClass>())
            {
                element = searcher.Search(query).FirstOrDefault();
            }

            // Assert
            Assert.NotNull(element);
            Assert.AreEqual(3, element.IntArrayItems.Length);
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
            public string[] StringArrayItems { get; set; }

            public int[] IntArrayItems { get; set; }

            public decimal[] DecimalArrayItems { get; set; }

            public bool[] BoolArrayItems { get; set; }

            public List<string> StringListItems { get; set; }

            public HashSet<string> StringHashItems { get; set; }

            public Dictionary<string, string> StringDictionaryItems { get; set; }
        }

    }

}
