using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace LocalSearchEngine.Core
{
    public class IndexReader : IDisposable
    {
        private readonly IApplicationSettingsService applicationSettingsService;
        private readonly Queue<int> indices;
        private TextReader currentTextReader;

        public IndexReader()
        {
            applicationSettingsService = DependencyService.Get<IApplicationSettingsService>();
            var indicesFiles = Directory.GetFiles(applicationSettingsService.IndexFolder);
            var indices = indicesFiles
                .Select(x => Path.GetFileNameWithoutExtension(x))
                .Select(x => int.Parse(x))
                .OrderBy(x => x);
            this.indices = new Queue<int>(indices);
        }

        public int CurrentId { get; private set; } = -1;

        public string ReadLine()
        {
            CurrentId++;
            
            if (indices.Count > 0)
            {
                if (CurrentId == indices.Peek())
                {
                    currentTextReader?.Dispose();

                    CurrentId = indices.Dequeue();
                    var file = Path.Combine(applicationSettingsService.IndexFolder, $"{CurrentId}.txt");
                    currentTextReader = new StreamReader(file);
                }
            }

            var line = currentTextReader.ReadLine();
            return line;
        }

        public void Dispose()
        {
            currentTextReader?.Dispose();
        }
    }
}
