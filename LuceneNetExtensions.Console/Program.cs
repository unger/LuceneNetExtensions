namespace LuceneNetExtensions.Console
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    using Lucene.Net.Search;

    using LuceneNetExtensions.Cfg;

    public class Program
    {
        public static void Main(string[] args)
        {
            const int Iterations = 1000000;

            IndexManager indexManager = FluentIndexConfiguration.Create()
              .IndexRootPath(null)
              .Mappings(m =>
              {
                  m.Add<SightingMap>();
              })
              .BuildIndexManager();

            var writer = indexManager.GetWriter<Sighting>();

            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < Iterations; i++)
            {
                writer.AddOrUpdateDocument(new Sighting { SpeciesName = "Praktejder", Municipality = "Göteborg", Province = "Bohuslän" });
            }

            sw.Stop();
            Console.WriteLine("Indexed {0} items in {1} ms", Iterations, sw.ElapsedMilliseconds);

            sw.Restart();
            var searcher = indexManager.GetSearcher<Sighting>();

            var results = searcher.Search(new MatchAllDocsQuery(), Iterations);
            sw.Stop();
            Console.WriteLine("Searched all {0} items in {1} ms", results.Count(), sw.ElapsedMilliseconds);

            sw.Restart();

            foreach (var item in results)
            {
                // iterate over all results
            }

            sw.Stop();
            Console.WriteLine("Enumerated all {0} items in {1} ms", results.Count(), sw.ElapsedMilliseconds);
        }
    }
}
