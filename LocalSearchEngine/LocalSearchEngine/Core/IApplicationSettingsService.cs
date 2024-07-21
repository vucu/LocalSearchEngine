using System;
using System.Collections.Generic;
using System.Text;

namespace LocalSearchEngine.Core
{
    public interface IApplicationSettingsService
    {
        string StoreFolder { get; }
        string IndexFolder { get; }
    }
}
