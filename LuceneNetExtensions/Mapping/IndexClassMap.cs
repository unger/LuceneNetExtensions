namespace LuceneNetExtensions.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Lucene.Net.Analysis;

    using LuceneNetExtensions.Reflection;

    public class IndexClassMap<T> : IIndexMappingProvider
    {
        private readonly Dictionary<string, IndexFieldMap> fields = new Dictionary<string, IndexFieldMap>();

        private string indexName;

        private Analyzer analyzer;

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
                return this.fields.Select(kvp => kvp.Value).ToList();
            }
        }

        public string IndexName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.indexName))
                {
                    return typeof(T).Name;
                }

                return this.indexName;
            }

            private set
            {
                this.indexName = value;
            }
        }

        public Analyzer Analyzer
        {
            get
            {
                if (this.analyzer == null)
                {
                    return new KeywordAnalyzer();
                }

                return this.analyzer;
            }

            private set
            {
                this.analyzer = value;
            }
        }

        public void Index(string name)
        {
            this.IndexName = name;
        }

        public IndexFieldMap Map(Expression<Func<T, object>> propertyExpression)
        {
            return this.Map(propertyExpression, null);
        }

        public IndexFieldMap Map(Expression<Func<T, object>> propertyExpression, string columnName)
        {
            return this.Map(ReflectionHelper.GetPropertyInfo(propertyExpression), columnName);
        }

        public void MapPublicProperties()
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                if (this.fields.ContainsKey(prop.Name))
                {
                    this.fields.Add(prop.Name, new IndexFieldMap(prop));
                }
            }
        }

        private IndexFieldMap Map(PropertyInfo property, string fieldName)
        {
            var fieldMap = new IndexFieldMap(property);

            if (!string.IsNullOrEmpty(fieldName))
            {
                fieldMap.FieldName = fieldName;
            }

            this.fields.Add(property.Name, fieldMap);

            return fieldMap;
        }
    }
}
