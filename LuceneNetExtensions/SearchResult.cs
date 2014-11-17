namespace LuceneNetExtensions
{
    using System.Collections.Generic;

    public class SearchResult<T>
    {
        public int TotalHits { get; set; }

        public List<T> Hits { get; set; }
    }
}
