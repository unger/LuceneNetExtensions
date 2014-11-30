namespace LuceneNetExtensions.Sample.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
            try
            {
                var sightings = this.database.Sightings.Page(1, 100).Items;
                var writer = this.indexManager.GetWriter<Sighting>();
                writer.DeleteAll();

                foreach (var sighting in sightings)
                {
                    writer.AddDocument(sighting);
                }
            }
            catch (Exception e)
            {
                return new ContentResult { Content = "Error: " + e.Message };
            }

            return new ContentResult { Content = "Done" };
        }
    }
}