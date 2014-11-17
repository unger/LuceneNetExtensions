namespace LuceneNetExtensions
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;

    public class IndexManager : IDisposable
    {
        private readonly string basePath;

        private readonly ConcurrentDictionary<string, IDisposable> writers = new ConcurrentDictionary<string, IDisposable>();

        private readonly IndexMapper mapper;

        public IndexManager(IndexMapper mapper)
            : this(string.Empty, mapper)
        {
        }

        public IndexManager(string basePath, IndexMapper mapper)
        {
            this.basePath = basePath;
            this.mapper = mapper;
        }

        public IndexWriter<T> GetWriter<T>()
        {
            var indexName = this.mapper.GetIndexName<T>();
            string indexPath = null;
            if (!string.IsNullOrEmpty(this.basePath))
            {
                indexPath = Path.Combine(this.basePath, indexName);
            }

            return this.writers.GetOrAdd(indexName, key => new IndexWriter<T>(indexPath, this.mapper)) as IndexWriter<T>;
        }

        public IndexSearcher<T> GetSearcher<T>()
        {
            var writer = this.GetWriter<T>();
            return writer.GetSearcher();
        }

        public void Dispose()
        {
            foreach (var key in this.writers.Keys)
            {
                IDisposable writer;
                if (this.writers.TryRemove(key, out writer))
                {
                    writer.Dispose();
                }
            }
        }
    }
}
