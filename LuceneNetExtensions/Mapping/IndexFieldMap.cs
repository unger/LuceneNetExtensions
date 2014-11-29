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
        private Field.TermVector fieldTermVector = Field.TermVector.NO;

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

        public IndexFieldMap TermVector()
        {
            this.fieldTermVector = Field.TermVector.YES;
            return this;
        }

        public IndexFieldMap TermVectorWithOffsets()
        {
            this.fieldTermVector = Field.TermVector.WITH_OFFSETS;
            return this;
        }

        public IndexFieldMap TermVectorWithPositions()
        {
            this.fieldTermVector = Field.TermVector.WITH_POSITIONS;
            return this;
        }

        public IndexFieldMap TermVectorWithPositionsAndOffsets()
        {
            this.fieldTermVector = Field.TermVector.WITH_POSITIONS_OFFSETS;
            return this;
        }

        public Field CreateEmptyField()
        {
            return new Field(this.FieldName, string.Empty, this.fieldStore, this.fieldIndex, this.fieldTermVector);
        }

        public Field CreateField<T>(T entity)
        {
            var value = this.GetValue(entity);
            return new Field(this.FieldName, value.ToString(), this.fieldStore, this.fieldIndex, this.fieldTermVector);
        }

        public void SetValue(object obj, object value)
        {
            this.prop.SetValue(obj, value);
        }

        public object GetValue(object obj)
        {
            return this.prop.GetValue(obj);
        }

        public Analyzer GetAnalyzer()
        {
            return this.analyzer;
        }

        private IndexFieldMap SetAnalyzed(Field.Index index, Analyzer fieldAnalyzer = null)
        {
            this.analyzer = fieldAnalyzer;
            this.fieldIndex = index;
            return this;
        }
    }
}
