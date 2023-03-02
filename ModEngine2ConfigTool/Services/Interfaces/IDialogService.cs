using ModEngine2ConfigTool.ViewModels.Dialogs;
using System.Threading.Tasks;

namespace ModEngine2ConfigTool.Services.Interfaces
{
    public interface IDialogService
    {
        Task<object?> ShowDialog(CustomDialogViewModel dialogVm);

        string? ShowFolderDialog(
            string title,
            string? defaultFolder = null,
            string? folderPath = null);

        string? ShowOpenFileDialog(
            string title,
            string filter,
            string? defaultFolder = null,
            string? filePath = null);

        Task<object?> ShowProgressDialog(
            CustomDialogViewModel dialogVm,
            Task progressTask);

        string? ShowSaveFileDialog(
            string title,
            string? filter = null,
            string? fileExtension = null,
            string? defaultFolder = null,
            string? filePath = null);

        void ShowMessageBox(string title, string message);
    }
}