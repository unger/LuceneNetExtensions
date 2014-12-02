namespace LuceneNetExtensions.Tests
{
    using Lucene.Net.Analysis;
    using Lucene.Net.Analysis.Standard;

    using LuceneNetExtensions.Mapping;
    using LuceneNetExtensions.Tests.Model;

    using NUnit.Framework;

    [TestFixture]
    public class IndexClassMapTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ClassMapWithoutConfigurationShouldHaveStandardAnalyzer()
        {
            var mapping = new SightingsMapWithoutConfiguration();

            var analyzer = mapping.BuildMappingProvider().GetAnalyzer();

            Assert.AreEqual(typeof(StandardAnalyzer).FullName, analyzer.GetType().FullName);
        }

        [Test]
        public void ClassMapWithKeywordAnalyzerShouldHaveKeywordAnalyzer()
        {
            var mapping = new SightingsMapWithKeyWordAnalyzer();

            var analyzer = mapping.BuildMappingProvider().GetAnalyzer();

            Assert.AreEqual(typeof(KeywordAnalyzer).FullName, analyzer.GetType().FullName);
        }

        [Test]
        public void ClassMapWithKeywordAnalyzerOnAFieldShouldHavePerFieldAnalyzer()
        {
            var mapping = new SightingsMapWithPerFieldAnalyzer();

            var analyzer = mapping.BuildMappingProvider().GetAnalyzer();

            Assert.AreEqual(typeof(PerFieldAnalyzerWrapper).FullName, analyzer.GetType().FullName);
        }

        private class SightingsMapWithoutConfiguration : IndexClassMap<Sighting>
        {
        }

        private class SightingsMapWithKeyWordAnalyzer : IndexClassMap<Sighting>
        {
            public SightingsMapWithKeyWordAnalyzer()
            {
                this.Analyzer(new KeywordAnalyzer());
            }
        }

        private class SightingsMapWithPerFieldAnalyzer : IndexClassMap<Sighting>
        {
            public SightingsMapWithPerFieldAnalyzer()
            {
                this.Map(s => s.SpeciesName).Analyzed(new KeywordAnalyzer());
            }
        }
    }
}
