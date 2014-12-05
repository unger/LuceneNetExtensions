namespace LuceneNetExtensions.Cfg
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Lucene.Net.Documents;

    public class IndexFieldConfiguration
    {
        private IFieldable fieldable;

        public IndexFieldConfiguration(string name, Type type, PropertyInfo propertyInfo, int fieldPrecision, Field.Store store, Field.Index index, Field.TermVector termVector, bool isIdentifier)
        {
            this.Name = name;
            this.Type = type;
            this.PropertyInfo = propertyInfo;
            this.IsNumeric = this.IsNumericType(type);
            this.FieldPrecision = fieldPrecision;
            this.Store = store;
            this.Index = index;
            this.TermVector = termVector;
            this.IsIdentifier = isIdentifier;
            this.IsCollection = this.IsCollectionType(type);
        }

        public string Name { get; private set; }

        public Type Type { get; private set; }

        public PropertyInfo PropertyInfo { get; private set; }

        public bool IsNumeric { get; private set; }

        public bool IsIdentifier { get; private set; }

        public bool IsCollection { get; private set; }

        public int FieldPrecision { get; private set; }

        public Field.Store Store { get; private set; }

        public Field.Index Index { get; private set; }
        
        public Field.TermVector TermVector { get; private set; }

        public IFieldable CreateField()
        {
            if (this.IsCollection)
            {
                return this.InternalCreateField();
            }

            return this.fieldable ?? (this.fieldable = this.InternalCreateField());
        }

        private IFieldable InternalCreateField()
        {
            if (this.IsNumeric)
            {
                return new NumericField(this.Name, this.FieldPrecision, this.Store, this.Index != Field.Index.NO);
            }

            return new Field(this.Name, string.Empty, this.Store, this.Index, this.TermVector);
        }

        private bool IsNumericType(Type type)
        {
            return type.IsAssignableFrom(typeof(int))
                || type.IsAssignableFrom(typeof(decimal))
                || type.IsAssignableFrom(typeof(float))
                || type.IsAssignableFrom(typeof(double))
                || type.IsAssignableFrom(typeof(long));
        }

        private bool IsCollectionType(Type type)
        {
            return type.IsArray
                || (!type.IsAssignableFrom(typeof(string)) && type.GetInterfaces().Any(t => t == typeof(IEnumerable)));
        }
    }
}
