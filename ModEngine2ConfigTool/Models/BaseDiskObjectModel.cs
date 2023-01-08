namespace ModEngine2ConfigTool.Models
{
    public abstract class BaseDiskObjectModel
    {
        public string Location { get; }

        public BaseDiskObjectModel(string location)
        {
            Location = location;
        }
    }
}
