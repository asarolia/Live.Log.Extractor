using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Live.Log.Extractor.IndexerService.Infrastructure;
using System.IO;
using System.Configuration;

namespace Live.Log.Extractor.IndexerService.Test
{
    [TestClass]
    public class IndexerServiceUnitTest
    {
        [TestMethod]
        public void CreatingIndexTest()
        {
            LuceneIndexer indexer= new LuceneIndexer();
            HashSet<string> set = new HashSet<string>{ "MMV012132313", "asdasdasdasdrtyrt", "MMV21346687966" };
            HashSet<string> set2 = new HashSet<string> { "MMV012132313", "asdasdasdasdrtyrtsdf", "MMV21346645687966" };
            indexer.IndexFile(@"C:\apps\IndexFiles", @"C:\apps\IndexFiles\10", DateTime.Now.ToShortDateString(), set);
            indexer.IndexFile(@"C:\apps\IndexFiles", @"C:\apps\IndexFiles\11", DateTime.Now.ToShortDateString(), new HashSet<string>());
            indexer.IndexFile(@"C:\apps\IndexFiles", @"C:\apps\IndexFiles\12", DateTime.Now.ToShortDateString(), set2);
            var result = indexer.SearchIndex(@"C:\apps\IndexFiles", "MMV012132313");
            var result2 = indexer.SearchIndex(@"C:\apps\IndexFiles", "MMV21346645687966");
            Assert.AreEqual(result.Count, 2, "Result count should be 2");
            Assert.AreEqual(result2.Count, 1, "Result count should be 1");
        }

        [TestMethod]
        public void DecompressFileTest()
        {
            XmlProcessor xmlProcessor = new XmlProcessor();
            xmlProcessor.DecompressFile(new DirectoryInfo(ConfigurationManager.AppSettings.Get("DecompressedFolder")).GetFiles("*.zip").FirstOrDefault());
        }
    }
}
