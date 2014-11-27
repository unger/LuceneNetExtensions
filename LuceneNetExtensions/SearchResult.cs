namespace LuceneNetExtensions
{
    using System.Collections;
    using System.Collections.Generic;

    using Lucene.Net.Search;

    using LuceneNetExtensions.Mapping;

    public class SearchResult<T> : IEnumerable<T>
    {
        private readonly IndexSearcher searcher;

        private readonly IIndexMappingProvider<T> mapper;

        private readonly TopDocs topDocs;

        public SearchResult(IndexSearcher searcher, IIndexMappingProvider<T> mapper, TopDocs topDocs)
        {
            this.searcher = searcher;
            this.mapper = mapper;
            this.topDocs = topDocs;
        }

        public int TotalHits
        {
            get
            {
                return this.topDocs.TotalHits;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new SearchResultEnumerator<T>(this.searcher, this.mapper, this.topDocs);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
