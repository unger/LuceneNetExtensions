namespace LuceneNetExtensions.Mapping
{
    using System;
    using System.Collections.Generic;

    public interface IIndexMappingProvider
    {
        Type ModelType { get; }

        List<IndexFieldMap> Fields { get; }
    }
}
