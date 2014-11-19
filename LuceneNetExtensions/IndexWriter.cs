namespace LuceneNetExtensions
{
    using System;

    using Lucene.Net.Index;

    public class IndexWriter<T> : IDisposable
    {
        private readonly IndexWriter writer;

        private readonly IndexMapper mapper;

        public IndexWriter(IndexWriter writer, IndexMapper mapper)
        {
            this.writer = writer;
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

        public void Commit()
        {
            this.writer.Commit();
        }
    }
}
