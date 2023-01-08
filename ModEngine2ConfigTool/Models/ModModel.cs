namespace ModEngine2ConfigTool.Models
{
    public class ModModel : BaseDiskObjectModel
    {
        public string Name { get; }

        public bool IsEnabled { get; }

        public ModModel(string name, string location, bool isEnabled) : base(location)
        {
            Name = name;
            IsEnabled = isEnabled;
        }
    }
}
