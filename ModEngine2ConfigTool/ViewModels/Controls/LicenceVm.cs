using CommunityToolkit.Mvvm.ComponentModel;

namespace ModEngine2ConfigTool.ViewModels.Controls
{
    public class LicenceVm : ObservableObject
    {
        public string Title { get; }
        public string Link { get; }
        public string Authors { get; }
        public string Version { get; }
        public string Licence { get; }

        public LicenceVm(
            string title, 
            string link, 
            string authors, 
            string version, 
            string licence)
        {
            Title = title;
            Link = link;
            Authors = authors;
            Version = version;
            Licence = licence;
        }
    }
}
