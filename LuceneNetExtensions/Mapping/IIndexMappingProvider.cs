namespace LuceneNetExtensions.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Lucene.Net.Analysis;
    using Lucene.Net.Documents;

    public interface IIndexMappingProvider<T> : IIndexMappingProvider
    {
        Document CreateDocument(T entity);

        T CreateEntity(Document doc);

        string GetFieldName<TReturn>(Expression<Func<T, TReturn>> expression);
    }

    public interface IIndexMappingProvider
    {
        Type ModelType { get; }

        IReadOnlyCollection<IndexFieldMap> Fields { get; }

        string GetIndexName();

        Analyzer GetAnalyzer();

        bool IsReadonly();
    }
}
