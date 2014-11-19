namespace LuceneNetExtensions
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Lucene.Net.Analysis;
    using Lucene.Net.Documents;

    using LuceneNetExtensions.Mapping;

    public class IndexMapper
    {
        private readonly ConcurrentDictionary<string, IIndexMappingProvider> mappers = new ConcurrentDictionary<string, IIndexMappingProvider>();
        private List<IIndexMappingProvider> mappingProviders;

        public IndexMapper(List<IIndexMappingProvider> mappingProviders)
        {
            this.mappingProviders = mappingProviders;
        }

        public Document CreateDocument<T>(T entity)
        {
            var document = new Document();
            var mapper = this.GetMapper<T>();

            foreach (var field in mapper.Fields)
            {
                document.Add(this.CreateField(field, entity));
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
                    var typedValue = SimpleTypeConverter.ConvertValue(field.FieldType, propertyValues);
                    field.SetValue(entity, typedValue);
                }
            }

            return entity;
        }

        public Analyzer GetAnalyzer<T>()
        {
            return new KeywordAnalyzer();
        }

        public string GetIndexName<T>()
        {
            return typeof(T).Name;
        }

        private Field CreateField<T>(IndexFieldMap field, T entity)
        {
            var value = field.GetValue(entity);

            return new Field(field.FieldName, value.ToString(), Field.Store.YES, Field.Index.ANALYZED);
        }

        private IIndexMappingProvider GetMapper<T>()
        {
            var fullname = typeof(T).FullName;
            return this.mappers.GetOrAdd(fullname, key => this.InternalGetMapper<T>());
        }

        private IIndexMappingProvider InternalGetMapper<T>()
        {
            return this.mappingProviders.FirstOrDefault(p => p.ModelType == typeof(T));
        }
    }
}
