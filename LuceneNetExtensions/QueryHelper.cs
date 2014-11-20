namespace LuceneNetExtensions
{
    using System;
    using System.Linq.Expressions;

    using Lucene.Net.Index;

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
    }
}
