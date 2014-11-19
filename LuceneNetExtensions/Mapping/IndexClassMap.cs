namespace LuceneNetExtensions.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class IndexClassMap<T> : IIndexMappingProvider
    {
        public Type ModelType
        {
            get
            {
                return typeof(T);
            }
        }

        public List<IndexFieldMap> Fields
        {
            get
            {
                var fields = new List<IndexFieldMap>();
                var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var prop in properties)
                {
                    fields.Add(new IndexFieldMap(prop)
                                   {
                                       FieldName = prop.Name,
                                       
                                   });
                }

                return fields;
            }
        }
    }
}
