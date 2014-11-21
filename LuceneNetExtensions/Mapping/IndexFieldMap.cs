namespace LuceneNetExtensions.Mapping
{
    using System;
    using System.Reflection;

    using Lucene.Net.Analysis;
    using Lucene.Net.Documents;

    public class IndexFieldMap
    {
        private readonly PropertyInfo prop;

        private Field.Store fieldStore = Field.Store.YES;
        private Field.Index fieldIndex = Field.Index.NOT_ANALYZED;

        private Analyzer analyzer;

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

        public IndexFieldMap NotStored()
        {
            this.fieldStore = Field.Store.NO;
            return this;
        }

        public IndexFieldMap NotIndexed()
        {
            this.fieldIndex = Field.Index.NO;
            return this;
        }

        public IndexFieldMap Analyzed(Analyzer fieldAnalyzer = null)
        {
            return this.SetAnalyzed(Field.Index.ANALYZED, fieldAnalyzer);
        }

        public IndexFieldMap AnalyzedNoNorms(Analyzer fieldAnalyzer = null)
        {
            return this.SetAnalyzed(Field.Index.ANALYZED_NO_NORMS, fieldAnalyzer);
        }

        public IndexFieldMap NotAnalyzed()
        {
            return this.SetAnalyzed(Field.Index.NOT_ANALYZED);
        }

        public IndexFieldMap NotAnalyzedNoNorms()
        {
            return this.SetAnalyzed(Field.Index.ANALYZED_NO_NORMS);
        }

        public Field CreateField<T>(T entity)
        {
            var value = this.GetValue(entity);
            return new Field(this.FieldName, value.ToString(), this.fieldStore, this.fieldIndex);
        }

        public void SetValue(object obj, object value)
        {
            this.prop.SetValue(obj, value);
        }

        public object GetValue(object obj)
        {
            return this.prop.GetValue(obj);
        }

        private IndexFieldMap SetAnalyzed(Field.Index index, Analyzer fieldAnalyzer = null)
        {
            this.analyzer = fieldAnalyzer;
            this.fieldIndex = index;
            return this;
        }
    }
}
