using HtmlAgilityPack;
using System.IO.Compression;
using System.Text;
using LocalSearchEngine;
using LocalSearchEngine.Core;

namespace LocalSearchEngine.Services
{
    internal sealed class HtmlStorageService : IHtmlStorageService
    {
        private const int IndexSize = 1000;
        private readonly IApplicationSettingsService applicationSettingsService;
        private readonly IUriService uriService;
        private readonly string htmlFolder;
        private readonly string indexFolder;

        public HtmlStorageService()
        {
            this.applicationSettingsService = DependencyService.Get<IApplicationSettingsService>();
            this.uriService = DependencyService.Get<IUriService>();
            htmlFolder = Path.Combine(applicationSettingsService.StoreFolder, "HTMLs");

            if (!Directory.Exists(htmlFolder))
            {
                Directory.CreateDirectory(htmlFolder);
            }

            indexFolder = applicationSettingsService.IndexFolder;
        }

        public void DownloadAndStoreZipFile(string url, IProgress<DownloadProgress> progress)
        {
            url = url.Trim();
            var result = uriService.TryCreate(url);
            if (!result.success)
            {
                throw new UriFormatException("Url is not valid");
            }

            var uri = result.result;
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(10);
                var response = client.GetAsync(uri).Result;

                if (!response.IsSuccessStatusCode)
                {
                    return;
                }

                var content = response.Content;

                var tmpZipPath = Path.Combine(applicationSettingsService.StoreFolder, "tmp.zip");
                using (var file = File.Create(tmpZipPath))
                {
                    var contentStream = content.ReadAsStreamAsync().Result;
                    contentStream.CopyTo(file);
                }

                var extractZipPath = Path.Combine(applicationSettingsService.StoreFolder, "TmpExtract");
                if (Directory.Exists(extractZipPath))
                {
                    Directory.Delete(extractZipPath, true);
                }
                Directory.CreateDirectory(extractZipPath);
                ZipFile.ExtractToDirectory(tmpZipPath, extractZipPath);


                Directory.Delete(htmlFolder, true);
                Directory.CreateDirectory(htmlFolder);

                var docId = 0;
                var htmlFiles = Directory.GetFiles(extractZipPath, "*.html", SearchOption.AllDirectories);

                var indexBuilder = new StringBuilder();
                foreach (var htmlFile in htmlFiles)
                {
                    var destHtmlPath = Path.Combine(htmlFolder, $"{docId}.html");
                    File.Copy(htmlFile, destHtmlPath);

                    var html = new HtmlDocument();
                    html.Load(htmlFile);
                    var titleNode = html.DocumentNode.SelectSingleNode("//head/title");
                    var title = titleNode?.InnerText?.Replace("\r", "  ")?.Replace("\n", "  ") ?? "Untitled";
                    indexBuilder.Append(title);
                    indexBuilder.Append(Constants.TitleDelimiter);
                    var textNodes = html.DocumentNode.SelectNodes("//text()[normalize-space(.) != '']");
                    var texts = textNodes.Select(x => x.InnerText);
                    var textContent = string.Join("  ", texts);
                    textContent = textContent.Replace("\r", "  ");
                    textContent = textContent.Replace("\n", "  ");
                    indexBuilder.Append(textContent);
                    indexBuilder.Append("  ");
                    indexBuilder.Append("\r\n");

                    docId++;
                    if (docId % IndexSize == 0)
                    {
                        var prevIndex = docId / IndexSize - 1;
                        var prevIdStart = prevIndex * IndexSize;
                        var destIndexPath = Path.Combine(indexFolder, $"{prevIdStart}.txt");
                        File.WriteAllText(destIndexPath, indexBuilder.ToString());
                        indexBuilder.Clear();
                    }
                }

                if (indexBuilder.Length > 0)
                {
                    var lastIndex = docId / IndexSize;
                    var idStart = lastIndex * IndexSize;
                    var destIndexPath = Path.Combine(indexFolder, $"{idStart}.txt");
                    File.WriteAllText(destIndexPath, indexBuilder.ToString());
                    indexBuilder.Clear();
                }

                File.Delete(tmpZipPath);
                Directory.Delete(extractZipPath, true);
            }
        }

        public IEnumerable<ExtendedHtmlDocument> GetStoredHtmlDocuments()
        {
            var htmlFiles = Directory.GetFiles(htmlFolder, "*.html");
            foreach (var htmlFile in htmlFiles)
            {
                var doc = new ExtendedHtmlDocument();
                doc.FileName = htmlFile;
                doc.Load(htmlFile);
                yield return doc;
            }

            yield break;
        }

        public ExtendedHtmlDocument GetStoredHtmlDocument(string fileName)
        {
            var doc = new ExtendedHtmlDocument();
            doc.FileName = fileName;
            doc.Load(fileName);
            return doc;
        }

        public string ReadFile(int id)
        {
            var htmlPath = Path.Combine(htmlFolder, $"{id}.html");
            return File.ReadAllText(htmlPath);
        }
    }
}
