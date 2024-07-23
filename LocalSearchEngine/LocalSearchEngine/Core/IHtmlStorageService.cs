using HtmlAgilityPack;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LocalSearchEngine.Core
{
    public interface IHtmlStorageService
    {
        void DownloadAndStoreZipFile(string url, IProgress<DownloadProgress> progress);
        IEnumerable<ExtendedHtmlDocument> GetStoredHtmlDocuments();
        ExtendedHtmlDocument GetStoredHtmlDocument(string fileName);
        string ReadFile(int id);
    }
}
