namespace LuceneNetExtensions.Mapping
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Lucene.Net.Analysis;
    using Lucene.Net.Documents;

    using LuceneNetExtensions.Cfg;
    using LuceneNetExtensions.Reflection;

    public class IndexMappingProvider<T> : IIndexMappingProvider<T>
    {
        private readonly string indexName;

        private readonly Analyzer analyzer;

        private readonly bool isReadonly;

        private readonly Dictionary<string, PropertyInfo> fieldPropertyMap = new Dictionary<string, PropertyInfo>();

        private Document reusableDocument;

        public IndexMappingProvider(string indexName, Analyzer analyzer, bool isReadonly, List<IndexFieldConfiguration> fields)
        {
            fields = fields ?? new List<IndexFieldConfiguration>();
            this.indexName = indexName;
            this.analyzer = analyzer;
            this.isReadonly = isReadonly;

            foreach (var field in fields)
            {
                this.fieldPropertyMap.Add(field.Name, field.PropertyInfo);
            }

            this.Fields = new ReadOnlyCollection<IndexFieldConfiguration>(fields);
            this.Identifiers = new ReadOnlyCollection<IndexFieldConfiguration>(this.Fields.Where(f => f.IsIdentifier).ToList());
        }

        public Type ModelType
        {
            get
            {
                return typeof(T);
            }
        }

        public IReadOnlyCollection<IndexFieldConfiguration> Identifiers { get; private set; }

        public IReadOnlyCollection<IndexFieldConfiguration> Fields { get; private set; }

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
            return this.Identifiers.Any();
        }

        public Document GetDocument(T entity)
        {
            foreach (var fieldConfig in this.Fields)
            {
                if (fieldConfig.IsCollection)
                {
                    this.ReusableDocument.RemoveFields(fieldConfig.Name);

                    var propertyValue = fieldConfig.PropertyInfo.GetValue(entity);

                    if (propertyValue is IEnumerable)
                    {
                        var values = propertyValue as IEnumerable;

                        foreach (var value in values)
                        {
                            var field = fieldConfig.CreateField();
                            this.SetFieldValue(field, value);
                            this.ReusableDocument.Add(field);
                        }
                    }
                }
                else
                {
                    this.SetFieldValue(fieldConfig.CreateField(), fieldConfig.PropertyInfo.GetValue(entity));
                }
            }

            return this.ReusableDocument;
        }

        public T CreateEntity(Document doc)
        {
            var entity = Activator.CreateInstance<T>();

            foreach (var field in this.Fields)
            {
                var propertyValues = doc.GetValues(field.Name);

                if (propertyValues.Length > 0)
                {
                    var typedValue = SimpleTypeConverter.ConvertValue(field.PropertyInfo.PropertyType, propertyValues);
                    field.PropertyInfo.SetValue(entity, typedValue);
                }
            }

            return entity;
        }

        public string GetFieldName<TReturn>(Expression<Func<T, TReturn>> expression)
        {
            var field = this.GetField(expression);
            return (field == null) ? string.Empty : field.Name;
        }

        public IndexFieldConfiguration GetField<TReturn>(Expression<Func<T, TReturn>> expression)
        {
            var prop = ReflectionHelper.GetPropertyInfo(expression);

            foreach (var field in this.Fields)
            {
                if (prop.Name == field.PropertyInfo.Name)
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
                document.Add(field.CreateField());
            }

            return document;
        }

        private void SetFieldValue(IFieldable fieldable, object value)
        {
            var field = fieldable as Field;
            if (field != null)
            {
                var stringValue = (value ?? string.Empty).ToString();
                field.SetValue(stringValue);
                return;
            }

            var numericField = fieldable as NumericField;
            if (numericField != null)
            {
                if (value is int)
                {
                    numericField.SetIntValue((int)value);
                }
                else if (value is long)
                {
                    numericField.SetLongValue((long)value);
                }
                else if (value is decimal || value is double)
                {
                    numericField.SetDoubleValue(Convert.ToDouble(value));
                }
                else if (value is float)
                {
                    numericField.SetDoubleValue(Convert.ToSingle(value));
                }
                else
                {
                    numericField.SetIntValue(0);
                }
            }
        }
    }
}
