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

        public IndexSearcher(IndexSearcher searcher, IIndexMappingProvider<T> mapper)
        {
            this.mapper = mapper;
            this.searcher = searcher;
        }

        public SearchResult<T> Search(Query query, int n = 1000)
        {
            return this.Search(query, null, n, null);
        }

        public SearchResult<T> Search(Query query, Filter filter, int n = 1000)
        {
            return this.Search(query, filter, n, null);
        }

        public SearchResult<T> Search(Query query, Filter filter, int n, Sort sort)
        {
            var searchResult = new SearchResult<T>();
            TopDocs topDocs = sort == null ? this.searcher.Search(query, filter, n) : this.searcher.Search(query, filter, n, sort);

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
