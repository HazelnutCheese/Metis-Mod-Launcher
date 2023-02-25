using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using ModEngine2ConfigTool.ViewModels.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModEngine2ConfigTool.Helpers
{
    public class ProfileVmModVmTuple
    {
        public ProfileVm ProfileVm { get; }

        public ModVm ModVm { get; }

        public ProfileVmModVmTuple(ProfileVm profileVm, ModVm modVm)
        {
            ProfileVm = profileVm;
            ModVm = modVm;
        }
    }
}
