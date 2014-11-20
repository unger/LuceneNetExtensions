namespace LuceneNetExtensions.Mapping
{
    using System;
    using System.Reflection;

    public class IndexFieldMap
    {
        private readonly PropertyInfo prop;

        public IndexFieldMap(PropertyInfo prop)
        {
            this.prop = prop;
            this.FieldName = prop.Name;
            this.FieldType = prop.PropertyType;
        }

        public string FieldName { get; set; }

        public Type FieldType { get; set; }

        public string PropertyName
        {
            get
            {
                return this.prop.Name;
            }
        }

        public Type PropertyType
        {
            get
            {
                return this.prop.PropertyType;
            }
        }

        public void SetValue(object obj, object value)
        {
            this.prop.SetValue(obj, value);
        }

        public object GetValue(object obj)
        {
            return this.prop.GetValue(obj);
        }
    }
}
