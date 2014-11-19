namespace LuceneNetExtensions.Cfg
{
    using System;
    using System.Collections.Generic;

    public class FluentIndexConfiguration
    {
        private readonly List<Action<IndexMappingConfiguration>> mappingsBuilders = new List<Action<IndexMappingConfiguration>>();

        private IndexConfiguration indexConfiguration;

        internal FluentIndexConfiguration()
            : this(new IndexConfiguration())
        {
        }

        internal FluentIndexConfiguration(IndexConfiguration indexConfiguration)
        {
            this.indexConfiguration = indexConfiguration;
        }

        public static FluentIndexConfiguration Create()
        {
            return new FluentIndexConfiguration();
        }

        public FluentIndexConfiguration IndexRootPath(string path)
        {
            this.indexConfiguration.IndexRootPath = path;
            return this;
        }

        public FluentIndexConfiguration Mappings(Action<IndexMappingConfiguration> mappings)
        {
            this.mappingsBuilders.Add(mappings);
            return this;
        }

        public IndexConfiguration BuildIndexConfiguration()
        {
            var mappingCfg = new IndexMappingConfiguration();

            foreach (var builder in this.mappingsBuilders)
            {
                builder(mappingCfg);
            }

            mappingCfg.Apply(this.indexConfiguration);


            return this.indexConfiguration;
        }


        public IndexManager BuildIndexManager()
        {
            return this.BuildIndexConfiguration()
                .BuildIndexManager();
        }
    }
}
