using ModEngine2ConfigTool.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ModEngine2ConfigTool.Equality
{
    public class DllEqualityComparer : IEqualityComparer<Dll>
    {
        public bool Equals(Dll? x, Dll? y)
        {
            return x?.DllId == y?.DllId;
        }

        public int GetHashCode([DisallowNull] Dll obj)
        {
            int hash = 173;
            foreach (var b in obj.DllId.ToByteArray())
            {
                hash = hash * 983 + b;
            }

            return hash;
        }
    }
}
