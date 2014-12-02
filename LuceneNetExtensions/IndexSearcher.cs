namespace LuceneNetExtensions
{
    using System;

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
            TopDocs topDocs = sort == null ? this.searcher.Search(query, filter, n) : this.searcher.Search(query, filter, n, sort);
            var searchResult = new SearchResult<T>(this.searcher, this.mapper, topDocs);
            return searchResult;
        }

        public void Dispose()
        {
            this.searcher.Dispose();
        }
    }
}
