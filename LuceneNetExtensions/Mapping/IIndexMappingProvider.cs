namespace LuceneNetExtensions.Mapping
{
    using System;
    using System.Collections.Generic;

    using Lucene.Net.Analysis;

    public interface IIndexMappingProvider
    {
        Type ModelType { get; }

        IReadOnlyCollection<IndexFieldMap> Fields { get; }

        string IndexName { get; }

        Analyzer GetAnalyzer();
    }
}
