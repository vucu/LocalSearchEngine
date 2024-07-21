using System;
using System.Collections.Generic;
using System.Text;

namespace LocalSearchEngine.Core
{
    public interface IUriService
    {
        (bool success, Uri result) TryCreate(string uriString);
    }
}
