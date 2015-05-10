namespace LuceneNetExtensions.Analyzers
{
    using System.IO;

    using Lucene.Net.Analysis;

    public class LowerCaseKeywordAnalyzer : Analyzer
    {
        public override TokenStream ReusableTokenStream(string fieldName, TextReader reader)
        {
            var previousTokenStream = (LowerCaseFilter)this.PreviousTokenStream;
            if (previousTokenStream == null)
            {
                return this.TokenStream(fieldName, reader);
            }

            previousTokenStream.Reset();
            return previousTokenStream;
        }

        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            return new LowerCaseFilter(new KeywordTokenizer(reader));
        }
    }
}
