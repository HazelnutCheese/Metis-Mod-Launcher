using ModEngine2ConfigTool.Models;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ModEngine2ConfigTool.Equality
{
    public class DllVmEqualityComparer : IEqualityComparer<DllVm>
    {
        public bool Equals(DllVm? x, DllVm? y)
        {
            return x?.Model.DllId == y?.Model.DllId;
        }

        public int GetHashCode([DisallowNull] DllVm obj)
        {
            int hash = 13;
            hash = (hash * 7) + new DllEqualityComparer().GetHashCode(obj.Model);

            return hash;
        }
    }
}
