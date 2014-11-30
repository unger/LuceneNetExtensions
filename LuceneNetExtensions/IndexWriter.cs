namespace LuceneNetExtensions
{
    using System;

    using Lucene.Net.Index;

    using LuceneNetExtensions.Mapping;

    public class IndexWriter<T> : IDisposable
    {
        private readonly IndexWriter writer;

        private readonly IIndexMappingProvider<T> mapper;

        public IndexWriter(IndexWriter writer, IIndexMappingProvider<T> mapper)
        {
            this.writer = writer;
            this.mapper = mapper;
        }

        public void AddDocument(T entity)
        {
            this.writer.AddDocument(this.mapper.GetDocument(entity));
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

        public void DeleteAll()
        {
            this.writer.DeleteAll();
        }
    }
}
