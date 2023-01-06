using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModEngine2ConfigTool.Models
{
    public class ModModel
    {
        public string Name { get; }

        public string Location { get; }

        public bool IsEnabled { get; }

        public ModModel(string name, string location, bool isEnabled)
        {
            Name = name;
            Location = location;
            IsEnabled = isEnabled;
        }
    }
}
