namespace LuceneNetExtensions.Cfg
{
    using System.Collections.Generic;

    using LuceneNetExtensions.Mapping;

    public class IndexConfiguration
    {
        private readonly List<IIndexMappingProvider> mappings = new List<IIndexMappingProvider>();

        public string IndexRootPath { get; set; }

        public List<IIndexMappingProvider> Mappings
        {
            get
            {
                return this.mappings;
            }
        }

        public IndexManager BuildIndexManager()
        {
            var mapper = new IndexMappers(this.Mappings);
            return new IndexManager(this.IndexRootPath, mapper);
        }
    }
}
