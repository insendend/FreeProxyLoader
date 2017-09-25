using System.Collections.Generic;

namespace FreeProxyListLoader.Models.Loader
{
    interface ILoader<out T>
    {
        IEnumerable<T> LoadedItems { get; }
        string Url { get; }

        void Load();
    }
}
