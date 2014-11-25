namespace LuceneNetExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Lucene.Net.Index;
    using Lucene.Net.Search;

    using LuceneNetExtensions.Mapping;

    public class QueryHelper<T>
    {
        private readonly IIndexMappingProvider<T> mapper;

        public QueryHelper(IIndexMappingProvider<T> mapper)
        {
            this.mapper = mapper;
        }

        public Term CreateTerm<TReturn>(Expression<Func<T, TReturn>> expression, string value)
        {
            return new Term(this.mapper.GetFieldName(expression), value);
        }

        public Query CreateTermQuery<TReturn>(Expression<Func<T, TReturn>> expression, string value)
        {
            return new TermQuery(this.CreateTerm(expression, value));
        }

        public Query CreateTermQuery<TReturn>(Expression<Func<T, TReturn>> expression, string[] values, Occur occur = Occur.SHOULD)
        {
            if (values == null || values.Length == 0)
            {
                return null;
            }

            if (values.Length == 1)
            {
                return this.CreateTermQuery(expression, values[0]);
            }

            var fieldname = this.mapper.GetFieldName(expression);
            var query = new BooleanQuery();
            foreach (var value in values)
            {
                query.Add(new TermQuery(new Term(fieldname, value)), occur);
            }

            return query;
        }

        public SortField CreateSortField<TReturn>(Expression<Func<T, TReturn>> expression, bool reverse = false)
        {
            var field = this.mapper.GetFieldMap(expression);
            return new SortField(field.FieldName, this.GetSortFieldType(field), reverse);
        }

        public Sort CreateSort(params SortField[] sortfields)
        {
            return new Sort(sortfields);
        }

        private int GetSortFieldType(IndexFieldMap field)
        {
            if (field.FieldType.IsAssignableFrom(typeof(int)))
            {
                return SortField.INT;
            }

            if (field.FieldType.IsAssignableFrom(typeof(string)))
            {
                return SortField.STRING;
            }

            return SortField.STRING;
        }
    }
}
