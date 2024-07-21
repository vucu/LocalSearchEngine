using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace LocalSearchEngine.Core
{
    public class ExtendedHtmlDocument : HtmlDocument
    {
        public string FileName { get; set; }
    }
}
