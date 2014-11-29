using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuceneNetExtensions
{
    using System.Diagnostics;

    using LuceneNetExtensions.Cfg;

    class Program
    {
        static void Main(string[] args)
        {
            IndexManager indexManager = FluentIndexConfiguration.Create()
              .IndexRootPath(null)
              .Mappings(m =>
              {
                  m.Add<SightingMap>();
              })
              .BuildIndexManager();

            var writer = indexManager.GetWriter<Sighting>();

            Stopwatch sw = Stopwatch.StartNew();
            int iterations = 1000000;
            for (int i = 0; i < iterations; i++)
            {
                writer.AddDocument(new Sighting { SpeciesName = "Praktejder", Municipality = "Göteborg", Province = "Bohuslän" });
            }

            sw.Stop();

            Console.WriteLine((sw.ElapsedMilliseconds).ToString());
        }
    }
}
