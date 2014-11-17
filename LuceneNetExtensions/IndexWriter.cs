namespace LuceneNetExtensions
{
    using System;

    using Lucene.Net.Index;
    using Lucene.Net.Store;

    using Directory = Lucene.Net.Store.Directory;

    public class IndexWriter<T> : IDisposable
    {
        private readonly IndexWriter writer;

        private readonly IndexMapper mapper;

        public IndexWriter(string indexPath, IndexMapper mapper)
        {
            this.writer = new IndexWriter(this.GetDirectory(indexPath), mapper.GetAnalyzer<T>(), true, IndexWriter.MaxFieldLength.UNLIMITED);
            this.mapper = mapper;
        }

        public void AddDocument(T doc)
        {
            this.writer.AddDocument(this.mapper.CreateDocument(doc));
        }

        public void Dispose()
        {
            this.writer.Dispose();
        }

        public void Optimize()
        {
            this.writer.Optimize();
        }

        public IndexSearcher<T> GetSearcher()
        {
            return new IndexSearcher<T>(this.writer, this.mapper);
        }

        private Directory GetDirectory(string indexPath)
        {
            if (string.IsNullOrEmpty(indexPath))
            {
                return new RAMDirectory();
            }

            return FSDirectory.Open(indexPath);
        }
    }
}
