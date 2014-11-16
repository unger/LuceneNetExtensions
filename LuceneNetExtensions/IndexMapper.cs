using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuceneNetExtensions
{
    using System.Reflection;

    using Lucene.Net.Documents;

    public class IndexMapper<T>
    {
        public Document CreateDocument(T entity)
        {
            var document = new Document();

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                document.Add(this.CreateField(prop, entity));
            }

            return document;
        }

        private Field CreateField(PropertyInfo propertyInfo, T entity)
        {
            var value = propertyInfo.GetValue(entity);

            return new Field(propertyInfo.Name, value.ToString(), Field.Store.YES, Field.Index.ANALYZED);
        }
    }
}
