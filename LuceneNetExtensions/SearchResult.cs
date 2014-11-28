namespace LuceneNetExtensions
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

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
            this.PageSize = 20;
        }

        public int TotalHits
        {
            get
            {
                return this.topDocs.TotalHits;
            }
        }

        public int PageSize { get; set; }

        public int TotalPages
        {
            get
            {
                if (this.TotalHits % this.PageSize == 0)
                {
                    return this.TotalHits / this.PageSize;
                }

                return this.TotalHits / this.PageSize + 1;
            }
        }

        public IEnumerable<T> GetPage(int page)
        {
            return this.Skip((page - 1) * this.PageSize).Take(this.PageSize);
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
