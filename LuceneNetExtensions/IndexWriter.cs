namespace LuceneNetExtensions
{
    using System;
    using System.Linq;

    using Lucene.Net.Index;
    using Lucene.Net.Search;

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

        public void AddOrUpdateDocument(T entity)
        {
            if (this.mapper.HasIdentifier())
            {
                var identifiers = this.mapper.Identifiers;

                if (identifiers.Count == 1)
                {
                    var idField = identifiers.First();
                    var idValue = idField.PropertyInfo.GetValue(entity);
                    if (idValue != null)
                    {
                        this.writer.UpdateDocument(new Term(idField.Name, idValue.ToString()), this.mapper.GetDocument(entity));
                        return;
                    }
                }
                else
                {
                    var query = new BooleanQuery();
                    foreach (var idField in identifiers)
                    {
                        var idValue = idField.PropertyInfo.GetValue(entity);
                        if (idValue != null)
                        {
                            query.Add(new TermQuery(new Term(idField.Name, idValue.ToString())), Occur.MUST);
                        }
                    }

                    if (query.Clauses.Count > 0)
                    {
                        this.writer.DeleteDocuments(query);
                    }
                }
            }

            this.writer.AddDocument(this.mapper.GetDocument(entity));
        }

        public void Dispose()
        {
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

        public void DeleteDocuments(Query query)
        {
            this.writer.DeleteDocuments(query);
        }
    }
}
