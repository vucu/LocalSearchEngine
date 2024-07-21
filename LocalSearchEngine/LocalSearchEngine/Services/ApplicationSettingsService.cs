using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LocalSearchEngine.Core;

namespace LocalSearchEngine.Services
{
    internal sealed class ApplicationSettingsService : IApplicationSettingsService
    {
        public string StoreFolder 
        {
            get
            {
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LocalSearchEngine");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                return path;
            }
        }

        public string IndexFolder
        {
            get
            {
                var indexFolder = Path.Combine(StoreFolder, "Index");

                if (!Directory.Exists(indexFolder))
                {
                    Directory.CreateDirectory(indexFolder);
                }

                return indexFolder;
            }    
        }
    }
}
