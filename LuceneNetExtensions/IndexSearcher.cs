namespace LuceneNetExtensions
{
    using System;
    using System.Collections.Generic;

    using Lucene.Net.Index;
    using Lucene.Net.Search;
    using Lucene.Net.Store;

    using LuceneNetExtensions.Mapping;

    public class IndexSearcher<T> : IDisposable
    {
        private readonly IndexSearcher searcher;

        private readonly IIndexMappingProvider<T> mapper;

        public IndexSearcher(Directory path, IIndexMappingProvider<T> mapper, bool readOnly = false)
            : this(new IndexSearcher(path, readOnly), mapper)
        {
        }

        public IndexSearcher(IndexReader reader, IIndexMappingProvider<T> mapper) 
            : this(new IndexSearcher(reader), mapper)
        {
        }

        public IndexSearcher(IndexSearcher searcher, IIndexMappingProvider<T> mapper)
        {
            this.mapper = mapper;
            this.searcher = searcher;
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
                searchResult.Hits.Add(this.mapper.CreateEntity(doc));
            }

            return searchResult;
        }

        public void Dispose()
        {
            this.searcher.Dispose();
        }
    }
}
