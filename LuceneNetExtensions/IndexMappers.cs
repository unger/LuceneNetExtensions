namespace LuceneNetExtensions
{
    using System.Collections.Generic;

    using LuceneNetExtensions.Mapping;

    public class IndexMappers
    {
        private readonly Dictionary<string, IIndexMappingProvider> mappers = new Dictionary<string, IIndexMappingProvider>();

        public IndexMappers(IEnumerable<IIndexMappingProvider> mappingProviders)
        {
            foreach (var mapper in mappingProviders)
            {
                this.mappers.Add(mapper.ModelType.FullName, mapper);
            }
        }

        public IIndexMappingProvider<T> GetMapper<T>()
        {
            var typeName = typeof(T).FullName;
            if (this.mappers.ContainsKey(typeName))
            {
                return this.mappers[typeName] as IIndexMappingProvider<T>;
            }

            return null;
        }
    }
}
