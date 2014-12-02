namespace LuceneNetExtensions.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;

    using Lucene.Net.Analysis;
    using Lucene.Net.Documents;

    using LuceneNetExtensions.Reflection;

    public class IndexMappingProvider<T> : IIndexMappingProvider<T>
    {
        private readonly string indexName;

        private readonly Analyzer analyzer;

        private readonly bool isReadonly;

        private readonly List<IndexFieldMap> identifierFields;

        private readonly List<IndexFieldMap> fields;

        private Document reusableDocument;

        public IndexMappingProvider(string indexName, Analyzer analyzer, bool isReadonly, List<IndexFieldMap> identifierFields, List<IndexFieldMap> fields)
        {
            this.indexName = indexName;
            this.analyzer = analyzer;
            this.isReadonly = isReadonly;
            this.identifierFields = identifierFields ?? new List<IndexFieldMap>();
            this.fields = fields ?? new List<IndexFieldMap>();
        }

        public Type ModelType
        {
            get
            {
                return typeof(T);
            }
        }

        public IReadOnlyCollection<IndexFieldMap> Identifiers
        {
            get
            {
                return new ReadOnlyCollection<IndexFieldMap>(this.identifierFields);
            }
        }

        public IReadOnlyCollection<IndexFieldMap> Fields
        {
            get
            {
                return new ReadOnlyCollection<IndexFieldMap>(this.fields);
            }
        }

        private Document ReusableDocument
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
            return this.indexName;
        }

        public Analyzer GetAnalyzer()
        {
            return this.analyzer;
        }

        public bool IsReadonly()
        {
            return this.isReadonly;
        }

        public bool HasIdentifier()
        {
            return this.identifierFields.Any();
        }

        public Document GetDocument(T entity)
        {
            foreach (var field in this.Fields)
            {
                field.UpdateFieldValue(entity);
            }

            return this.ReusableDocument;
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
                    field.SetPropertyValue(entity, typedValue);
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

        private Document CreateEmptyDocument()
        {
            var document = new Document();
            foreach (var field in this.Fields)
            {
                document.Add(field.Fieldable);
            }

            return document;
        }
    }
}
