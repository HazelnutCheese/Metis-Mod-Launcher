using MaterialDesignThemes.Wpf;
using ModEngine2ConfigTool.ViewModels.Dialogs;
using ModEngine2ConfigTool.ViewModels.Fields;
using ModEngine2ConfigTool.ViewModels.Profiles;
using ModEngine2ConfigTool.Views.Dialogs;
using Sherlog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ModEngine2ConfigTool.Services
{
    public class PlayManagerService
    {
        private readonly ProfileService _profileService;
        private readonly SaveManagerService _saveManagerService;
        private readonly ModEngine2Service _modEngine2Service;
        private readonly IDispatcherService _dispatcherService;

        public PlayManagerService(
            ProfileService profileService,
            SaveManagerService saveManagerService,
            ModEngine2Service modEngine2Service,
            IDispatcherService dispatcherService)
        {
            _profileService = profileService;
            _saveManagerService = saveManagerService;
            _modEngine2Service = modEngine2Service;
            _dispatcherService = dispatcherService;
        }

        public async Task Play(ProfileVm profileVm)
        {
            var dialog = new CustomDialogView();
            var dialogVm = new CustomDialogViewModel(
                "Launch Elden Ring",
                $"Are you sure you want to launch Elden Ring with this profile selected?" +
                "\n\nNote: Steam must be running and logged in.",
                new List<IFieldViewModel>(),
                new List<DialogButtonViewModel>()
                {
                    new DialogButtonViewModel(
                        "Play",
                        CustomDialogViewModel.GetCloseDialogCommand(true, dialog),
                        isDefault: false),
                    new DialogButtonViewModel(
                        "Cancel",
                        CustomDialogViewModel.GetCloseDialogCommand(false, dialog),
                        isDefault: true)
                });

            dialog.DataContext = dialogVm;

            var result = await DialogHost.Show(dialog, App.DialogHostId);

            if (result is not bool || result.Equals(false))
            {
                return;
            }

            using var cts = new CancellationTokenSource();
            var runEldenRingTask = GetPlayTask(profileVm, cts.Token);
            await ShowPlayingDialog(runEldenRingTask, cts);
        }

        private async Task GetPlayTask(ProfileVm profileVm, CancellationToken cancellationToken)
        {
            try
            {
                profileVm.UpdateLastPlayed();

                var profileId = profileVm.Model.ProfileId.ToString();

                // create Toml file from profileVm
                var profileToml = _profileService.WriteProfile(profileVm);

                // Backup base game saves
                _saveManagerService.CreateUnmoddedBackups();

                //// Backup current profile saves
                if (profileVm.UseSaveManager)
                {
                    _saveManagerService.CreateBackups(profileId);
                }

                //// push current profile saves
                if (profileVm.UseSaveManager)
                {
                    _saveManagerService.InstallProfileSaves(profileId);
                }

                try
                {
                    //// launch modengine2 with toml
                    using var modEngineProcess = _modEngine2Service.LaunchWithProfile(profileToml);

                    //// wait for elden ring to exit
                    try
                    {
                        await modEngineProcess.WaitForExitAsync(cancellationToken);
                    }
                    catch (TaskCanceledException)
                    {
                        modEngineProcess.Kill();
                    }

                    await Task.Delay(3000);
                    await _modEngine2Service.WaitForEldenRingExit(cancellationToken);
                    //await Task.Delay(15000, cancellationToken);
                }
                catch (TaskCanceledException)
                {

                }
                catch (InvalidOperationException e)
                {
                    MessageBox.Show(e.Message);
                }

                //// pop current profile saves
                if (profileVm.UseSaveManager)
                {
                    _saveManagerService.UninstallProfileSaves(profileId);
                }

                // delete toml file
                File.Delete(profileToml);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                var logger = Logger.GetLogger(nameof(App));
                logger.Error(e.Message);
            }
        }

        private async Task ShowPlayingDialog(Task backgroundTask, CancellationTokenSource cts)
        {
            var playingDialog = new ProgressDialogView();
            var playingDialogVm = new CustomDialogViewModel(
                "Playing Elden Ring",
                $"Waiting for the Elden Ring process to exit...",
                new List<IFieldViewModel>(),
                new List<DialogButtonViewModel>()
                {
                        new DialogButtonViewModel(
                            "Force Exit",
                            CustomDialogViewModel.GetCloseDialogCommand(true, playingDialog),
                            isDefault: true)
                });
            playingDialog.DataContext = playingDialogVm;

            var areYouSureDialog = new CustomDialogView();
            var areYouSureDialogVm = new CustomDialogViewModel(
                "Cancel",
                $"Are you sure you want to cancel?" +
                "\n\nNote: This will close Elden Ring without saving. Progress may be lost.",
                new List<IFieldViewModel>(),
                new List<DialogButtonViewModel>()
                {
                    new DialogButtonViewModel(
                        "Yes",
                        CustomDialogViewModel.GetCloseDialogCommand(true, areYouSureDialog),
                        isDefault: true),
                    new DialogButtonViewModel(
                        "No",
                        CustomDialogViewModel.GetCloseDialogCommand(false, areYouSureDialog),
                        isDefault: false)
            });
            areYouSureDialog.DataContext = areYouSureDialogVm;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            backgroundTask.ContinueWith(t =>
            {
                _dispatcherService.InvokeUi(() => DialogHost.GetDialogSession(App.DialogHostId)?.Close(true));
            }, cts.Token);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            var hasRequestedForcedExit = false;
            while (
                !(backgroundTask.IsCompleted || backgroundTask.IsFaulted) &&
                !hasRequestedForcedExit && 
                !cts.Token.IsCancellationRequested)
            {
                var playResult = await DialogHost.Show(playingDialog, App.DialogHostId);

                if (playResult is not bool || playResult.Equals(false) || !(backgroundTask.IsCompleted || backgroundTask.IsFaulted))
                {
                    var sureResult = await DialogHost.Show(areYouSureDialog, App.DialogHostId);

                    if (sureResult is bool && sureResult.Equals(true))
                    {
                        hasRequestedForcedExit = true;
                        cts.Cancel();
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}
