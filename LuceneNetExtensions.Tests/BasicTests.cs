namespace LuceneNetExtensions.Tests
{
    using Lucene.Net.Index;
    using Lucene.Net.Search;

    using LuceneNetExtensions.Tests.Model;

    using NUnit.Framework;

    [TestFixture]
    public class BasicTests
    {
        private IndexManager IndexManager { get; set; }

        [SetUp]
        public void Setup()
        {
            this.IndexManager = new IndexManager();

            var writer = this.IndexManager.GetWriter<Person>();

            writer.AddDocument(new Person { Name = "Magnus" });
            writer.AddDocument(new Person { Name = "Kalle" });

            writer.Optimize();
        }

        [Test]
        public void Test()
        {
            var query = new TermQuery(new Term("Name", "magnus"));
            var totalHits = 0;

            using (var searcher = this.IndexManager.GetSearcher<Person>())
            {
                var result = searcher.Search(query);
                totalHits = result.TotalHits;
            }

            Assert.AreEqual(1, totalHits);
        }

        [TearDown]
        public void TearDown()
        {
            this.IndexManager.Dispose();
        }
    }
}
