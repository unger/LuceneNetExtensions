namespace LuceneNetExtensions.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Lucene.Net.Analysis;
    using Lucene.Net.Documents;

    using LuceneNetExtensions.Cfg;

    public interface IIndexMappingProvider<T> : IIndexMappingProvider
    {
        Document GetDocument(T entity);

        T CreateEntity(Document doc);

        string GetFieldName<TReturn>(Expression<Func<T, TReturn>> expression);

        IndexFieldConfiguration GetField<TReturn>(Expression<Func<T, TReturn>> expression);
    }

    public interface IIndexMappingProvider
    {
        Type ModelType { get; }

        IReadOnlyCollection<IndexFieldConfiguration> Identifiers { get; }

        IReadOnlyCollection<IndexFieldConfiguration> Fields { get; }

        string GetIndexName();

        Analyzer GetAnalyzer();

        bool IsReadonly();

        bool HasIdentifier();
    }
}
