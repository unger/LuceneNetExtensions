using System;

namespace LuceneNetExtensions
{
    using System.Web.Hosting;

    using Lucene.Net.Analysis;
    using Lucene.Net.Analysis.Standard;
    using Lucene.Net.Index;
    using Lucene.Net.Store;
    using Lucene.Net.Util;

    using Directory = Lucene.Net.Store.Directory;

    public class IndexWriter<T> : IDisposable
    {
        private readonly IndexWriter writer;

        private readonly IndexMapper<T> mapper;

        public IndexWriter(string indexPath)
        {
            writer = new IndexWriter(this.GetDirectory(indexPath), this.GetAnalyzer(), true, IndexWriter.MaxFieldLength.UNLIMITED);
            mapper = new IndexMapper<T>();
        }

        public void AddDocument(T doc)
        {
            this.writer.AddDocument(this.mapper.CreateDocument(doc));
        }

        public void Dispose()
        {
            writer.Dispose();
        }

        public void Optimize()
        {
            this.writer.Optimize();
        }

        public IndexSearcher<T> GetSearcher()
        {
            return new IndexSearcher<T>(this.writer);
        }

        private Analyzer GetAnalyzer()
        {
            return new StandardAnalyzer(Version.LUCENE_30);
        }

        private Directory GetDirectory(string indexPath)
        {
            if (string.IsNullOrEmpty(indexPath))
            {
                return new RAMDirectory();
            }

            return FSDirectory.Open(HostingEnvironment.MapPath(indexPath));
        }
    }
}
