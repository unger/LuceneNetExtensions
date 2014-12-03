namespace LuceneNetExtensions.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Lucene.Net.Analysis;
    using Lucene.Net.Analysis.Standard;

    using LuceneNetExtensions.Reflection;

    using Version = Lucene.Net.Util.Version;

    public class IndexClassMap<T> : IIndexClassMap
    {
        private readonly Dictionary<string, IndexFieldMap> fields = new Dictionary<string, IndexFieldMap>();

        private string indexName;

        private Version version = Version.LUCENE_30;

        private Analyzer documentAnalyzer;

        private bool readonlyIndex;

        public IIndexMappingProvider BuildMappingProvider()
        {
            return new IndexMappingProvider<T>(
                indexName: this.GetIndexName(),
                analyzer: this.GetAnalyzer(),
                isReadonly: this.readonlyIndex,
                fields: this.fields.Select(kvp => kvp.Value.BuildFieldConfiguration()).ToList());
        }

        protected void Readonly()
        {
            this.readonlyIndex = true;
        }

        protected void IndexName(string name)
        {
            this.indexName = name;
        }

        protected void Analyzer(Analyzer analyzer)
        {
            this.documentAnalyzer = analyzer;
        }

        protected IndexFieldMap Id(Expression<Func<T, object>> propertyExpression, string fieldName = null)
        {
            var prop = ReflectionHelper.GetPropertyInfo(propertyExpression);
            var fieldMap = new IndexFieldMap(prop, fieldName, isIdentifier: true);
            this.fields.Add(prop.Name, fieldMap);
            return fieldMap;
        }

        protected IndexFieldMap Map(Expression<Func<T, object>> propertyExpression, string fieldName = null)
        {
            var prop = ReflectionHelper.GetPropertyInfo(propertyExpression);
            var fieldMap = new IndexFieldMap(prop, fieldName);
            this.fields.Add(prop.Name, fieldMap);
            return fieldMap;
        }

        protected void MapPublicProperties()
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                if (!this.fields.ContainsKey(prop.Name))
                {
                    this.fields.Add(prop.Name, new IndexFieldMap(prop));
                }
            }
        }

        private string GetIndexName()
        {
            if (string.IsNullOrWhiteSpace(this.indexName))
            {
                return typeof(T).Name;
            }

            return this.indexName;
        }

        private Analyzer GetAnalyzer()
        {
            if (this.documentAnalyzer == null)
            {
                var analyzer = new StandardAnalyzer(this.version);
                var usePerFieldAnalyzer = false;

                var fieldAnalyzers = new List<KeyValuePair<string, Analyzer>>();
                foreach (var field in this.fields.Select(kvp => kvp.Value))
                {
                    var fieldAnalyzer = field.Analyzer;
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
    }
}
