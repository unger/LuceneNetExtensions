namespace LuceneNetExtensions
{
    using System.Collections;
    using System.Collections.Generic;

    using Lucene.Net.Search;

    using LuceneNetExtensions.Mapping;

    public class SearchResultEnumerator<T> : IEnumerator<T>
    {
        private readonly IndexSearcher searcher;

        private readonly IIndexMappingProvider<T> mapper;

        private readonly TopDocs topDocs;

        private int index = -1;

        public SearchResultEnumerator(IndexSearcher searcher, IIndexMappingProvider<T> mapper, TopDocs topDocs)
        {
            this.searcher = searcher;
            this.mapper = mapper;
            this.topDocs = topDocs;
        }

        public T Current
        {
            get
            {
                var scoredoc = this.topDocs.ScoreDocs[this.index];
                var doc = this.searcher.Doc(scoredoc.Doc);
                return this.mapper.CreateEntity(doc);
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            this.index++;
            return this.index < this.topDocs.ScoreDocs.Length;
        }

        public void Reset()
        {
            this.index = -1;
        }
    }
}