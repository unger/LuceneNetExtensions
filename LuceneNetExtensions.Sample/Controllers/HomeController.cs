namespace LuceneNetExtensions.Sample.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;

    using Lucene.Net.Search;

    using LuceneNetExtensions.Sample.Db;
    using LuceneNetExtensions.Sample.Models;

    using Newtonsoft.Json;

    public class HomeController : Controller
    {
        private readonly MySqlDatabase database;

        private readonly IndexManager indexManager;

        public HomeController(MySqlDatabase database, IndexManager indexManager)
        {
            this.database = database;
            this.indexManager = indexManager;
        }

        // GET: Home
        public ActionResult Index()
        {
            List<Sighting> sightings;
            using (var searcher = this.indexManager.GetSearcher<Sighting>())
            {
                sightings = searcher.Search(new MatchAllDocsQuery(), null).ToList();
            }

            return new ContentResult { Content = JsonConvert.SerializeObject(sightings) };
        }

        public ActionResult ReIndex()
        {
            var sb = new StringBuilder();

            try
            {
                sb.AppendLine("Start: 0ms");
                Stopwatch sw = Stopwatch.StartNew();
                var sightings = this.database.Sightings.All().ToList();
                var writer = this.indexManager.GetWriter<Sighting>();
                writer.DeleteAll();
                sb.AppendLine(string.Format("Delete all: {0}ms", sw.ElapsedMilliseconds));

                foreach (var sighting in sightings)
                {
                    try
                    {
                        writer.AddOrUpdateDocument(sighting);
                        writer.AddOrUpdateDocument(sighting);
                    }
                    catch (Exception e)
                    {
                        
                        throw;
                    }
                }

                sb.AppendLine(string.Format("{0} documents added: {1}ms", sightings.Count, sw.ElapsedMilliseconds));

                writer.Commit();

                sb.AppendLine(string.Format("Committed: {0}ms", sw.ElapsedMilliseconds));

                writer.Optimize();

                sb.AppendLine(string.Format("Optimized: {0}ms", sw.ElapsedMilliseconds));

                sw.Stop();
            }
            catch (Exception e)
            {
                return new ContentResult { Content = "Error: " + e.Message };
            }

            return new ContentResult { Content = sb.ToString().Replace("\n", "<br>") };
        }
    }
}