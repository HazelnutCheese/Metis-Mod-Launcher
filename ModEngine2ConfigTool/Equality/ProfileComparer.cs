using ModEngine2ConfigTool.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ModEngine2ConfigTool.Equality
{
    public class ProfileEqualityComparer : IEqualityComparer<Profile>
    {
        public bool Equals(Profile? x, Profile? y)
        {
            return x?.ProfileId == y?.ProfileId;
        }

        public int GetHashCode([DisallowNull] Profile obj)
        {
            int hash = 173;
            foreach (var b in obj.ProfileId.ToByteArray())
            {
                hash = hash * 983 + b;
            }

            return hash;
        }
    }
}
