namespace LuceneNetExtensions
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;

    using Lucene.Net.Index;
    using Lucene.Net.Search;
    using Lucene.Net.Store;

    using LuceneNetExtensions.Mapping;

    using Directory = Lucene.Net.Store.Directory;

    public class IndexManager : IDisposable
    {
        private readonly string basePath;

        private readonly ConcurrentDictionary<string, IndexWriter> indexWriters = new ConcurrentDictionary<string, IndexWriter>();

        private readonly IndexMappers mapper;

        public IndexManager(IndexMappers mapper)
            : this(string.Empty, mapper)
        {
        }

        public IndexManager(string basePath, IndexMappers mapper)
        {
            this.basePath = basePath;
            this.mapper = mapper;
        }

        public IndexWriter<T> GetWriter<T>()
        {
            var classMapper = this.mapper.GetMapper<T>();
            if (classMapper.IsReadonly())
            {
                throw new Exception("Cannot create writer on readonly index");
            }

            var writer = this.InternalGetWriter(classMapper);
            return new IndexWriter<T>(writer, classMapper);
        }

        public IndexSearcher<T> GetSearcher<T>()
        {
            var classMapper = this.mapper.GetMapper<T>();
            var searcher = this.InternalGetSearcher(classMapper);
            return new IndexSearcher<T>(searcher, classMapper);
        }

        public QueryHelper<T> GetQueryHelper<T>()
        {
            var classMapper = this.mapper.GetMapper<T>();
            return new QueryHelper<T>(classMapper);
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

        private string GetIndexFullPath(string indexName)
        {
            string indexPath = null;
            if (!string.IsNullOrEmpty(this.basePath))
            {
                indexPath = Path.Combine(this.basePath, indexName);
            }

            return indexPath;
        }

        private IndexWriter InternalGetWriter<T>(IIndexMappingProvider<T> classMapper)
        {
            var indexName = classMapper.GetIndexName();
            var indexPath = this.GetIndexFullPath(indexName);

            return this.indexWriters.GetOrAdd(indexName, key => new IndexWriter(this.GetDirectory(indexPath), classMapper.GetAnalyzer(), IndexWriter.MaxFieldLength.UNLIMITED));
        }

        private IndexSearcher InternalGetSearcher<T>(IIndexMappingProvider<T> classMapper)
        {
            var indexName = classMapper.GetIndexName();
            var indexPath = this.GetIndexFullPath(indexName);

            if (classMapper.IsReadonly())
            {
                return new IndexSearcher(this.GetDirectory(indexPath), true);
            }

            var writer = this.InternalGetWriter(classMapper);
            return new IndexSearcher(writer.GetReader());
        }
    }
}
