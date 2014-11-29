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
    using Lucene.Net.Documents;

    using LuceneNetExtensions.Reflection;

    using Version = Lucene.Net.Util.Version;

    public class IndexClassMap<T> : IIndexMappingProvider<T>
    {
        private readonly Dictionary<string, IndexFieldMap> fields = new Dictionary<string, IndexFieldMap>();

        private string indexName;

        private Version version = Version.LUCENE_30;

        private Analyzer documentAnalyzer;

        private bool readonlyIndex;

        private Document reusableDocument;

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

        protected Document ReusableDocument
        {
            get
            {
                if (this.reusableDocument == null)
                {
                    this.reusableDocument = this.CreateEmptyDocument();
                }

                return this.reusableDocument;
            }
        }

        public string GetIndexName()
        {
            if (string.IsNullOrWhiteSpace(this.indexName))
            {
                return typeof(T).Name;
            }

            return this.indexName;
        }

        public bool IsReadonly()
        {
            return this.readonlyIndex;
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

        public Document GetDocument(T entity)
        {
            foreach (var field in this.Fields)
            {
                this.ReusableDocument.GetField(field.FieldName).SetValue(field.GetValue(entity).ToString());
            }

            return this.ReusableDocument;
        }

        public Document CreateDocument(T entity)
        {
            var document = new Document();

            foreach (var field in this.Fields)
            {
                document.Add(field.CreateField(entity));
            }

            return document;
        }

        public T CreateEntity(Document doc)
        {
            var entity = Activator.CreateInstance<T>();

            foreach (var field in this.Fields)
            {
                var propertyValues = doc.GetValues(field.FieldName);

                if (propertyValues.Length > 0)
                {
                    var typedValue = SimpleTypeConverter.ConvertValue(field.PropertyType, propertyValues);
                    field.SetValue(entity, typedValue);
                }
            }

            return entity;
        }

        public string GetFieldName<TReturn>(Expression<Func<T, TReturn>> expression)
        {
            var field = this.GetFieldMap(expression);
            return (field == null) ? string.Empty : field.FieldName;
        }

        public IndexFieldMap GetFieldMap<TReturn>(Expression<Func<T, TReturn>> expression)
        {
            var prop = ReflectionHelper.GetPropertyInfo(expression);

            foreach (var field in this.Fields)
            {
                if (prop.Name == field.PropertyName)
                {
                    return field;
                }
            }

            return null;
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

        private Document CreateEmptyDocument()
        {
            var document = new Document();
            foreach (var field in this.Fields)
            {
                document.Add(field.CreateEmptyField());
            }

            return document;
        }
    }
}
