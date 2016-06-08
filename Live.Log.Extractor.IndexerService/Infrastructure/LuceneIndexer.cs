using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Store;
using System.IO;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;

namespace Live.Log.Extractor.IndexerService.Infrastructure
{
    public class LuceneIndexer
    {
        public const string RegularExpression = "RegularExpressions";
        public const string FilePath = "FilePath";
        public const string Date = "Date";

        /// <summary>
        /// Indexes the file.
        /// </summary>
        /// <param name="indexPath">The index path.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="date">The date.</param>
        /// <param name="indexFields">The index fields.</param>
        public void IndexFile(string indexPath, string filePath, string date, HashSet<string> indexFields)
        {
            try
            {
                using (Lucene.Net.Store.Directory directory = FSDirectory.Open(new DirectoryInfo(indexPath)))
                {
                    // Delete any existing document with the same file path before adding a new document
                    DeleteIndex(indexPath, new List<string> { filePath });
                    Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
                    using (IndexWriter writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
                    {
                        Document doc = new Document();
                        doc.Add(new Field(Date, date, Field.Store.YES, Field.Index.NOT_ANALYZED));
                        doc.Add(new Field(RegularExpression, indexFields.ToCustomString(), Field.Store.YES, Field.Index.ANALYZED));
                        doc.Add(new Field(FilePath, filePath, Field.Store.YES, Field.Index.NOT_ANALYZED));
                        writer.AddDocument(doc);
                        writer.Optimize();
                        writer.Commit();
                    }
                }
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Searches the index.
        /// </summary>
        /// <param name="indexPath">The index path.</param>
        /// <param name="searchText">The search text.</param>
        /// <returns></returns>
        public List<IndexInformation> SearchIndex(string indexPath, string searchText)
        {
            List<IndexInformation> searchResults = new List<IndexInformation>();
            try
            {
                using (Lucene.Net.Store.Directory directory = FSDirectory.Open(new DirectoryInfo(indexPath)))
                {
                    Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
                    var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, RegularExpression, analyzer);
                    Query query = parser.Parse(searchText);
                    using (var searcher = new IndexSearcher(directory, true))
                    {
                        Hits topDocs = searcher.Search(query);
                        if (topDocs != null)
                        {
                            for (int i = 0; i < topDocs.Length(); i++)
                            {
                                searchResults.Add(new IndexInformation { FilePath = topDocs.Doc(i).Get(FilePath), Date = topDocs.Doc(i).Get(Date) });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return searchResults;
        }

        /// <summary>
        /// Deletes the index.
        /// </summary>
        /// <param name="indexPath">The index path.</param>
        /// <param name="docList">The doc list.</param>
        /// <returns></returns>
        public bool DeleteIndex(string indexPath, List<string> docList)
        {
            bool result = false;
            try
            {
                using (Lucene.Net.Store.Directory directory = FSDirectory.Open(new DirectoryInfo(indexPath)))
                {
                    Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
                    var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, RegularExpression, analyzer);
                    IndexModifier modifier = new Lucene.Net.Index.IndexModifier(indexPath, analyzer, false);
                    foreach (string doc in docList)
                    {
                        modifier.DeleteDocuments(new Lucene.Net.Index.Term(FilePath, doc));
                    }
                    modifier.Flush();
                    modifier.Close();
                }
            }
            catch (Exception ex)
            {
                // Swallow Exceptions here
            }

            return result;
        }
    }
}
