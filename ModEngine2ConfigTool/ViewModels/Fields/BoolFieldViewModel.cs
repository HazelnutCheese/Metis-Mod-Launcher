namespace ModEngine2ConfigTool.ViewModels.Fields
{
    public class BoolFieldViewModel : BaseFieldViewModel<bool>
    {
        public BoolFieldViewModel(
            string label, 
            string toolTip, 
            bool defaultValue) : base(
                label, 
                toolTip, 
                defaultValue)
        {
        }
    }
}
