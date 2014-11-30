namespace LuceneNetExtensions.Sample.Db
{
    using Dapper;

    using LuceneNetExtensions.Sample.Models;

    public class MySqlDatabase : Database<MySqlDatabase>
    {
        public Table<Site> Sites { get; set; }

        public Table<Sighting> Sightings { get; set; }
    }
}