using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ModEngine2ConfigTool.Equality
{
    public class ModVmEqualityComparer : IEqualityComparer<ModVm>
    {
        public bool Equals(ModVm? x, ModVm? y)
        {
            return new ModEqualityComparer().Equals(x?.Model, y?.Model);
        }

        public int GetHashCode([DisallowNull] ModVm obj)
        {
            int hash = 13;
            hash = (hash * 7) + new ModEqualityComparer().GetHashCode(obj.Model);

            return hash;
        }
    }
}
