namespace LuceneNetExtensions.Cfg
{
    using System;

    using Lucene.Net.Documents;

    public class IndexFieldConfiguration
    {
        public IndexFieldConfiguration(string name, Type type, int fieldPrecision, Field.Store store, Field.Index index, Field.TermVector termVector)
        {
            this.Name = name;
            this.Type = type;
            this.IsNumeric = this.IsNumericType(type);
            this.FieldPrecision = fieldPrecision;
            this.Store = store;
            this.Index = index;
            this.TermVector = termVector;
        }

        public string Name { get; private set; }

        public Type Type { get; private set; }

        public bool IsNumeric { get; private set; }

        public int FieldPrecision { get; private set; }

        public Field.Store Store { get; private set; }

        public Field.Index Index { get; private set; }
        
        public Field.TermVector TermVector { get; private set; }

        private bool IsNumericType(Type type)
        {
            return type.IsAssignableFrom(typeof(int))
                || type.IsAssignableFrom(typeof(decimal))
                || type.IsAssignableFrom(typeof(float))
                || type.IsAssignableFrom(typeof(double))
                || type.IsAssignableFrom(typeof(long));
        }
    }
}
