namespace LuceneNetExtensions.Mapping
{
    using System;
    using System.Reflection;

    using Lucene.Net.Analysis;
    using Lucene.Net.Documents;

    using LuceneNetExtensions.Cfg;

    public class IndexFieldMap
    {
        private const int FieldPrecision = 4;

        private readonly PropertyInfo propertyInfo;
        private readonly Type fieldType;
        private readonly bool isIdentifier;

        private Field.Store fieldStore = Field.Store.YES;
        private Field.Index fieldIndex = Field.Index.NOT_ANALYZED;
        private Field.TermVector fieldTermVector = Field.TermVector.NO;

        public IndexFieldMap(PropertyInfo propertyInfo, bool isIdentifier = false)
            : this(propertyInfo, propertyInfo.Name, isIdentifier)
        {
        }

        public IndexFieldMap(PropertyInfo propertyInfo, string fieldName, bool isIdentifier = false)
        {
            this.propertyInfo = propertyInfo;
            this.FieldName = string.IsNullOrWhiteSpace(fieldName) ? propertyInfo.Name : fieldName;
            this.fieldType = propertyInfo.PropertyType;
            this.isIdentifier = isIdentifier;
        }

        public string FieldName { get; private set; }

        public Analyzer Analyzer { get; private set; }

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

        public IndexFieldConfiguration BuildFieldConfiguration()
        {
            return new IndexFieldConfiguration(
                name: this.FieldName,
                type: this.fieldType,
                propertyInfo: this.propertyInfo,
                fieldPrecision: FieldPrecision,
                store: this.fieldStore,
                index: this.fieldIndex,
                termVector: this.fieldTermVector,
                isIdentifier: this.isIdentifier);
        }

        private IndexFieldMap SetAnalyzed(Field.Index index, Analyzer fieldAnalyzer = null)
        {
            this.Analyzer = fieldAnalyzer;
            this.fieldIndex = index;
            return this;
        }
    }
}
