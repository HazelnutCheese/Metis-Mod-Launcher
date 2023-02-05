using ModEngine2ConfigTool.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ModEngine2ConfigTool.Equality
{
    public class ModEqualityComparer : IEqualityComparer<Mod>
    {
        public bool Equals(Mod? x, Mod? y)
        {
            return x?.ModId == y?.ModId;
        }

        public int GetHashCode([DisallowNull] Mod obj)
        {
            int hash = 173;
            foreach (var b in obj.ModId.ToByteArray())
            {
                hash = hash * 983 + b;
            }

            return hash;
        }
    }
}
