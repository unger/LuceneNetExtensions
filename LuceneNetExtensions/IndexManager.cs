namespace LuceneNetExtensions
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;

    using Lucene.Net.Index;
    using Lucene.Net.Store;

    using Directory = Lucene.Net.Store.Directory;

    public class IndexManager : IDisposable
    {
        private readonly string basePath;

        private readonly ConcurrentDictionary<string, IndexWriter> indexWriters = new ConcurrentDictionary<string, IndexWriter>();

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
            var writer = this.InternalGetWriter<T>();
            return new IndexWriter<T>(writer, this.mapper);
        }

        public IndexSearcher<T> GetSearcher<T>()
        {
            var writer = this.InternalGetWriter<T>();
            return new IndexSearcher<T>(writer.GetReader(), this.mapper);
        }

        public void Dispose()
        {
            foreach (var key in this.indexWriters.Keys)
            {
                IndexWriter writer;
                if (this.indexWriters.TryRemove(key, out writer))
                {
                    writer.Dispose();
                }
            }
        }

        private Directory GetDirectory(string indexPath)
        {
            if (string.IsNullOrEmpty(indexPath))
            {
                return new RAMDirectory();
            }

            return FSDirectory.Open(indexPath);
        }

        private string GetIndexFullPath<T>()
        {
            var indexName = this.mapper.GetIndexName<T>();
            string indexPath = null;
            if (!string.IsNullOrEmpty(this.basePath))
            {
                indexPath = Path.Combine(this.basePath, indexName);
            }

            return indexPath;
        }

        private IndexWriter InternalGetWriter<T>()
        {
            var indexName = this.mapper.GetIndexName<T>();
            var indexPath = this.GetIndexFullPath<T>();

            return this.indexWriters.GetOrAdd(indexName, key => new IndexWriter(this.GetDirectory(indexPath), this.mapper.GetAnalyzer<T>(), IndexWriter.MaxFieldLength.UNLIMITED));
        }
    }
}
