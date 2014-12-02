﻿namespace LuceneNetExtensions
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
                    var id = identifiers.First();
                    var idValue = id.GetPropertyValue(entity);
                    if (idValue != null)
                    {
                        this.writer.UpdateDocument(new Term(id.FieldName, idValue.ToString()), this.mapper.GetDocument(entity));
                        return;
                    }
                }
                else
                {
                    var query = new BooleanQuery();
                    foreach (var id in identifiers)
                    {
                        var idValue = id.GetPropertyValue(entity);
                        if (idValue != null)
                        {
                            query.Add(new TermQuery(new Term(id.FieldName, idValue.ToString())), Occur.MUST);
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
