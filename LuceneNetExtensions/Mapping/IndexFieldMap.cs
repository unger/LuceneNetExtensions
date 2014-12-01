namespace LuceneNetExtensions.Mapping
{
    using System;
    using System.Reflection;

    using Lucene.Net.Analysis;
    using Lucene.Net.Documents;

    public class IndexFieldMap
    {
        private const int FieldPrecision = 4;

        private readonly PropertyInfo propertyInfo;

        private readonly IFieldable fieldable;

        private Field.Store fieldStore = Field.Store.YES;
        private Field.Index fieldIndex = Field.Index.NOT_ANALYZED;
        private Field.TermVector fieldTermVector = Field.TermVector.NO;

        private Analyzer analyzer;

        private bool? isNumeric;

        public IndexFieldMap(PropertyInfo propertyInfo)
        {
            this.propertyInfo = propertyInfo;
            this.FieldName = propertyInfo.Name;
            this.FieldType = propertyInfo.PropertyType;
            this.fieldable = this.CreateEmptyField();
        }

        public string FieldName { get; set; }

        public Type FieldType { get; set; }

        public string PropertyName
        {
            get
            {
                return this.propertyInfo.Name;
            }
        }

        public Type PropertyType
        {
            get
            {
                return this.propertyInfo.PropertyType;
            }
        }

        public IFieldable Fieldable
        {
            get
            {
                return this.fieldable;
            }
        }

        public bool IsNumeric
        {
            get
            {
                if (!this.isNumeric.HasValue)
                {
                    this.isNumeric = this.FieldType.IsAssignableFrom(typeof(int))
                                     || this.FieldType.IsAssignableFrom(typeof(decimal))
                                     || this.FieldType.IsAssignableFrom(typeof(float))
                                     || this.FieldType.IsAssignableFrom(typeof(double))
                                     || this.FieldType.IsAssignableFrom(typeof(long));
                }

                return this.isNumeric.Value;
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

        public void SetPropertyValue<T>(T obj, object value)
        {
            this.propertyInfo.SetValue(obj, value);
        }

        public object GetPropertyValue<T>(T obj)
        {
            return this.propertyInfo.GetValue(obj);
        }

        public void UpdateFieldValue<T>(T entity)
        {
            this.SetFieldValue(this.GetPropertyValue(entity));
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

        private IFieldable CreateEmptyField()
        {
            if (this.IsNumeric)
            {
                return new NumericField(this.FieldName, FieldPrecision, this.fieldStore, this.fieldIndex != Field.Index.NO);
            }

            return new Field(this.FieldName, string.Empty, this.fieldStore, this.fieldIndex, this.fieldTermVector);
        }

        private void SetFieldValue(object value)
        {
            var field = this.fieldable as Field;
            if (field != null)
            {
                var stringValue = (value ?? string.Empty).ToString();
                field.SetValue(stringValue);
                return;
            }

            var numericField = this.fieldable as NumericField;
            if (numericField != null)
            {
                if (value is int)
                {
                    numericField.SetIntValue((int)value);
                }
                else if (value is long)
                {
                    numericField.SetLongValue((long)value);
                }
                else if (value is decimal || value is double)
                {
                    numericField.SetDoubleValue(Convert.ToDouble(value));
                }
                else if (value is float)
                {
                    numericField.SetDoubleValue(Convert.ToSingle(value));
                }
                else
                {
                    numericField.SetIntValue(0);
                }
            }
        }
    }
}
