namespace LuceneNetExtensions.Mapping
{
    public interface IIndexClassMap
    {
        IIndexMappingProvider BuildMappingProvider();
    }
}