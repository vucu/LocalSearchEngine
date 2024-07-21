using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalSearchEngine.ExtensionMethods
{
    public static class CollectionExtensionMethods
    {
        public static void ReplaceContentWith<T>(this IList<T> list, IEnumerable<T> values)
        {
            list.Clear();
            foreach (T value in values)
            {
                list.Add(value);
            }
        }
    }
}
