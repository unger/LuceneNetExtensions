namespace LuceneNetExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Lucene.Net.Analysis;
    using Lucene.Net.Documents;

    using LuceneNetExtensions.Mapping;
    using LuceneNetExtensions.Reflection;

    public class IndexMapper
    {
        private readonly Dictionary<string, IIndexMappingProvider> mappers = new Dictionary<string, IIndexMappingProvider>();

        public IndexMapper(IEnumerable<IIndexMappingProvider> mappingProviders)
        {
            foreach (var mapper in mappingProviders)
            {
                this.mappers.Add(mapper.ModelType.FullName, mapper);
            }
        }

        public Document CreateDocument<T>(T entity)
        {
            var document = new Document();
            var mapper = this.GetMapper<T>();

            foreach (var field in mapper.Fields)
            {
                document.Add(field.CreateField(entity));
            }

            return document;
        }

        public T CreateEntity<T>(Document doc)
        {
            var entity = Activator.CreateInstance<T>();
            var mapper = this.GetMapper<T>();

            foreach (var field in mapper.Fields)
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

        public Analyzer GetAnalyzer<T>()
        {
            var mapper = this.GetMapper<T>();
            return mapper.Analyzer;
        }

        public string GetIndexName<T>()
        {
            var mapper = this.GetMapper<T>();
            return mapper.IndexName;
        }

        public string GetFieldName<TMapping, TReturn>(Expression<Func<TMapping, TReturn>> expression)
        {
            var field = this.GetFieldMap(expression);
            return (field == null) ? string.Empty : field.FieldName;
        }

        private IndexFieldMap GetFieldMap<TMapping, TReturn>(Expression<Func<TMapping, TReturn>> expression)
        {
            var mapper = this.GetMapper<TMapping>();
            var prop = ReflectionHelper.GetPropertyInfo(expression);

            foreach (var field in mapper.Fields)
            {
                if (prop.Name == field.PropertyName)
                {
                    return field;
                }
            }

            return null;
        }

        private IIndexMappingProvider GetMapper<T>()
        {
            return this.mappers[typeof(T).FullName];
        }
    }
}
