using System;

namespace LuceneNetExtensions
{
    using Lucene.Net.Index;
    using Lucene.Net.Search;

    public class IndexSearcher<T> : IDisposable
    {
        private readonly IndexSearcher searcher;

        public IndexSearcher(IndexWriter writer)
        {
            this.searcher = new IndexSearcher(writer.GetReader());
        }

        public TopDocs Search(Query query)
        {
            return this.searcher.Search(query, 1000);
        }

        public void Dispose()
        {
            this.searcher.Dispose();
        }
    }
}
