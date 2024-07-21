using System;
using LocalSearchEngine.Core;

namespace LocalSearchEngine.Services
{
    public class UriService : IUriService
    {
        public (bool success, Uri result) TryCreate(string uriString)
        {
            if (!uriString.StartsWith("http"))
            {
                uriString = "http://" + uriString;
            }

            var success = Uri.TryCreate(uriString, UriKind.Absolute, out Uri uri);
            return (success, uri);
        }
    }
}
