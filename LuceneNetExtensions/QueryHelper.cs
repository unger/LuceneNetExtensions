namespace LuceneNetExtensions
{
    using System;
    using System.Linq.Expressions;

    using Lucene.Net.Index;
    using Lucene.Net.Search;

    public class QueryHelper<T>
    {
        private readonly IndexMapper mapper;

        public QueryHelper(IndexMapper mapper)
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
    }
}
