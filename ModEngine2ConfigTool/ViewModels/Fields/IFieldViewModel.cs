using System.ComponentModel;

namespace ModEngine2ConfigTool.ViewModels.Fields
{
    public interface IFieldViewModel : INotifyPropertyChanged, IChangeTracking
    {
        string Label { get; }

        string ToolTip { get; }
    }
}
