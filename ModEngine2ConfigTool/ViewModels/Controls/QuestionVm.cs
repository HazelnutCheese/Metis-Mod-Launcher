using CommunityToolkit.Mvvm.ComponentModel;

namespace ModEngine2ConfigTool.ViewModels.Controls
{
    public class QuestionVm : ObservableObject
    {
        public string Question { get; }

        public string Answer { get; }

        public QuestionVm(string question, string answer)
        {
            Question = question; 
            Answer = answer;
        }
    }
}
