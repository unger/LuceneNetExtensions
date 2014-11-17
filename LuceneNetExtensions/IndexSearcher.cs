namespace LuceneNetExtensions
{
    using System;
    using System.Collections.Generic;

    using Lucene.Net.Index;
    using Lucene.Net.Search;

    public class IndexSearcher<T> : IDisposable
    {
        private readonly IndexSearcher searcher;

        private readonly IndexMapper mapper;

        public IndexSearcher(IndexWriter writer, IndexMapper mapper)
        {
            this.mapper = mapper;
            this.searcher = new IndexSearcher(writer.GetReader());
        }

        public SearchResult<T> Search(Query query, int count = 1000)
        {
            var searchResult = new SearchResult<T>();
            var topDocs = this.searcher.Search(query, count);

            searchResult.TotalHits = topDocs.TotalHits;
            searchResult.Hits = new List<T>();

            foreach (var scoreDoc in topDocs.ScoreDocs)
            {
                var doc = this.searcher.Doc(scoreDoc.Doc);
                searchResult.Hits.Add(this.mapper.CreateEntity<T>(doc));
            }

            return searchResult;
        }

        public void Dispose()
        {
            this.searcher.Dispose();
        }
    }
}
