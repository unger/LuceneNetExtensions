namespace LuceneNetExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Reflection;

    using Lucene.Net.Analysis;
    using Lucene.Net.Analysis.Standard;
    using Lucene.Net.Documents;

    public class IndexMapper
    {
        public Document CreateDocument<T>(T entity)
        {
            var document = new Document();

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                document.Add(this.CreateField(prop, entity));
            }

            return document;
        }

        public T CreateEntity<T>(Document doc)
        {
            var entity = Activator.CreateInstance<T>();

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                var propertyValues = doc.GetValues(prop.Name);

                if (propertyValues.Length > 0)
                {
                    var typedValue = SimpleTypeConverter.ConvertValue(prop.PropertyType, propertyValues);
                    prop.SetValue(entity, typedValue, null);
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

        private Field CreateField<T>(PropertyInfo propertyInfo, T entity)
        {
            var value = propertyInfo.GetValue(entity);

            return new Field(propertyInfo.Name, value.ToString(), Field.Store.YES, Field.Index.ANALYZED);
        }
    }
}
