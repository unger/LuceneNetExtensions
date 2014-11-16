namespace LuceneNetExtensions
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Linq;

    public class IndexManager: IDisposable
    {
        private readonly string basePath = null;

        private readonly ConcurrentDictionary<string, IDisposable> writers = new ConcurrentDictionary<string, IDisposable>();

        public IndexManager()
            :this(string.Empty)
        {}

        public IndexManager(string basePath)
        {
            this.basePath = basePath;
        }

        public IndexWriter<T> GetWriter<T>()
        {
            var indexName = this.GetIndexName<T>();
            string indexPath = null;
            if (!string.IsNullOrEmpty(basePath))
            {
                indexPath = Path.Combine(this.basePath, indexName);
            }

            return writers.GetOrAdd(indexName, key => new IndexWriter<T>(indexPath)) as IndexWriter<T>;
        }

        public IndexSearcher<T> GetSearcher<T>()
        {
            var writer = this.GetWriter<T>();
            return writer.GetSearcher();
        }

        private string GetIndexName<T>()
        {
            return typeof(T).Name;
        }

        public void Dispose()
        {
            foreach (var key in writers.Keys)
            {
                IDisposable writer;
                if (writers.TryRemove(key, out writer))
                {
                    writer.Dispose();
                }
            }
        }
    }
}
