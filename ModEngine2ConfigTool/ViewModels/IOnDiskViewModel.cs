using System.ComponentModel;

namespace ModEngine2ConfigTool.ViewModels
{
    public interface IOnDiskViewModel : INotifyPropertyChanged
    {
        string Name { get; }

        string Location { get; }
    }
}
