using MaterialDesignThemes.Wpf;
using ModEngine2ConfigTool.Services.Interfaces;
using ModEngine2ConfigTool.ViewModels.Dialogs;
using ModEngine2ConfigTool.Views.Dialogs;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModEngine2ConfigTool.Services
{
    public class DialogService : IDialogService
    {
        private readonly IDispatcherService _dispatcherService;

        public const string AllImageFilter =
            "All Picture Files(*.bmp;*.jpg;*.gif;*.ico;*.png;*.wdp;*.tiff)|" +
            "*.BMP;*.JPG;*.GIF;*.ICO;*.PNG;*.WDP;*.TIFF|" +
            "All files (*.*)|*.*";

        public DialogService(IDispatcherService dispatcherService)
        {
            _dispatcherService = dispatcherService;
        }

        public string? ShowOpenFileDialog(
            string title,
            string filter,
            string? defaultFolder = null,
            string? filePath = null)
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = false,
                Title = title,
                CheckFileExists = true,
                CheckPathExists = true
            };

            return GetFileDialogValue(fileDialog,
                filter,
                defaultFolder,
                filePath);
        }

        public string? ShowSaveFileDialog(
            string title,
            string? filter = null,
            string? fileExtension = null,
            string? defaultFolder = null,
            string? filePath = null)
        {
            var fileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = title
            };

            if (fileExtension is not null)
            {
                fileDialog.AddExtension = true;
                fileDialog.DefaultExt = fileExtension;
            }

            return GetFileDialogValue(fileDialog,
                filter,
                defaultFolder,
                filePath);
        }

        public string? ShowFolderDialog(
            string title,
            string? defaultFolder = null,
            string? folderPath = null)
        {
            var dialog = new FolderBrowserEx.FolderBrowserDialog
            {
                Title = title,
                AllowMultiSelect = false
            };

            if (defaultFolder is not null)
            {
                dialog.InitialFolder = defaultFolder;
            }

            if (folderPath is not null)
            {
                dialog.DefaultFolder = folderPath;
            }

            return dialog.ShowDialog().Equals(DialogResult.OK)
                ? dialog.SelectedFolder
                : null;
        }

        public async Task<object?> ShowDialog(CustomDialogViewModel dialogVm)
        {
            var view = new CustomDialogView
            {
                DataContext = dialogVm
            };

            return await DialogHost.Show(view, App.DialogHostId);
        }

        public async Task<object?> ShowProgressDialog(
            CustomDialogViewModel dialogVm,
            Task progressTask)
        {
            var view = new ProgressDialogView
            {
                DataContext = dialogVm
            };

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            progressTask.ContinueWith(t =>
            {
                CloseCurrentDialogSession();
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            return await DialogHost.Show(view, App.DialogHostId);
        }

        private static string? GetFileDialogValue(
            Microsoft.Win32.FileDialog fileDialog,
            string? filter = null,
            string? defaultFolder = null,
            string? filePath = null)
        {
            if (filter is not null)
            {
                fileDialog.Filter = filter;
            }

            if (defaultFolder is not null)
            {
                fileDialog.InitialDirectory = defaultFolder;
            }

            if (filePath is not null)
            {
                fileDialog.FileName = filePath;
            }

            return fileDialog.ShowDialog().Equals(true) && !string.IsNullOrWhiteSpace(fileDialog.FileName)
                ? fileDialog.FileName
                : null;
        }

        private void CloseCurrentDialogSession()
        {
            _dispatcherService.InvokeUi(() => DialogHost.GetDialogSession(App.DialogHostId)?.Close(true));
        }

        public void ShowMessageBox(string title, string message)
        {
            MessageBox.Show(title, message);
        }
    }
}
