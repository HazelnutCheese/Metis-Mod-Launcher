using ModEngine2ConfigTool.ViewModels.Profiles;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ModEngine2ConfigTool.Equality
{
    public class ProfileVmEqualityComparer : IEqualityComparer<ProfileVm>
    {
        public bool Equals(ProfileVm? x, ProfileVm? y)
        {
            return new ProfileEqualityComparer().Equals(x.Model, y.Model);;
        }

        public int GetHashCode([DisallowNull] ProfileVm obj)
        {
            int hash = 13;
            hash = (hash * 7) + new ProfileEqualityComparer().GetHashCode(obj.Model);

            return hash;
        }
    }
}
