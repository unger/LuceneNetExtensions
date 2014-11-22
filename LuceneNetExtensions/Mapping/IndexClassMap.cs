namespace LuceneNetExtensions.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Lucene.Net.Analysis;
    using Lucene.Net.Analysis.Standard;

    using LuceneNetExtensions.Reflection;

    using Version = Lucene.Net.Util.Version;

    public class IndexClassMap<T> : IIndexMappingProvider
    {
        private readonly Dictionary<string, IndexFieldMap> fields = new Dictionary<string, IndexFieldMap>();

        private string indexName;

        private Version version = Version.LUCENE_30;

        private Analyzer documentAnalyzer;

        public Type ModelType
        {
            get
            {
                return typeof(T);
            }
        }

        public IReadOnlyCollection<IndexFieldMap> Fields
        {
            get
            {
                return new ReadOnlyCollection<IndexFieldMap>(this.fields.Select(kvp => kvp.Value).ToList());
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

        public Analyzer GetAnalyzer()
        {
            if (this.documentAnalyzer == null)
            {
                var analyzer = new StandardAnalyzer(this.version);
                var usePerFieldAnalyzer = false;

                var fieldAnalyzers = new List<KeyValuePair<string, Analyzer>>();
                foreach (var field in this.Fields)
                {
                    var fieldAnalyzer = field.GetAnalyzer();
                    if (fieldAnalyzer != null)
                    {
                        fieldAnalyzers.Add(new KeyValuePair<string, Analyzer>(field.FieldName, fieldAnalyzer));
                        if (!fieldAnalyzer.GetType().FullName.Equals(analyzer.GetType().FullName))
                        {
                            usePerFieldAnalyzer = true;
                        }
                    }
                }

                if (usePerFieldAnalyzer)
                {
                    this.documentAnalyzer = new PerFieldAnalyzerWrapper(analyzer, fieldAnalyzers);
                }
                else
                {
                    this.documentAnalyzer = analyzer;
                }
            }

            return this.documentAnalyzer;
        }

        protected void Index(string name)
        {
            this.IndexName = name;
        }

        protected void Analyzer(Analyzer analyzer)
        {
            this.documentAnalyzer = analyzer;
        }

        protected IndexFieldMap Map(Expression<Func<T, object>> propertyExpression)
        {
            return this.Map(propertyExpression, null);
        }

        protected IndexFieldMap Map(Expression<Func<T, object>> propertyExpression, string columnName)
        {
            return this.Map(ReflectionHelper.GetPropertyInfo(propertyExpression), columnName);
        }

        protected void MapPublicProperties()
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
