namespace LuceneNetExtensions.Cfg
{
    using System;
    using System.Collections.Generic;

    using LuceneNetExtensions.Mapping;

    public class IndexMappingConfiguration
    {
        private readonly List<Type> types = new List<Type>();

        public void Add<T>() where T : IIndexClassMap
        {
            this.types.Add(typeof(T));
        }

        internal void Apply(IndexConfiguration config)
        {
            foreach (var type in this.types)
            {
                var instance = Activator.CreateInstance(type) as IIndexClassMap;
                if (instance != null)
                {
                    config.Mappings.Add(instance.BuildMappingProvider());
                }
            }
        }
    }
}
