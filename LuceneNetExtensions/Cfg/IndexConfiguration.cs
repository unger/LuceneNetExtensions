using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuceneNetExtensions.Cfg
{
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
            var mapper = new IndexMapper(this.Mappings);
            return new IndexManager(this.IndexRootPath, mapper);
        }
    }
}
