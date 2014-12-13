namespace LuceneNetExtensions.TypeConverters
{
    public class ObjectMapper<TSource, TDestination>
    {
        public TDestination ConvertToDestination(TSource source)
        {
            return default(TDestination);
        }

        public TSource ConvertToSource(TDestination destination)
        {
            return default(TSource);
        }
    }
}
