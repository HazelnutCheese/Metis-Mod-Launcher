using ModEngine2ConfigTool.ViewModels.Dialogs;
using ModEngine2ConfigTool.ViewModels.Profiles;
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
        private readonly DialogService _dialogService;

        public PlayManagerService(
            ProfileService profileService,
            SaveManagerService saveManagerService,
            ModEngine2Service modEngine2Service,
            DialogService dialogService)
        {
            _profileService = profileService;
            _saveManagerService = saveManagerService;
            _modEngine2Service = modEngine2Service;
            _dialogService = dialogService;
        }

        public void PlaySilent(ProfileVm profileVm)
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

                    // wait a little bit for things to initialise
                    Thread.Sleep(5000);

                    // If using save manager wait for exit
                    if (profileVm.UseSaveManager)
                    {
                        if (!modEngineProcess.HasExited)
                        {
                            modEngineProcess.WaitForExit();
                        }

                        //// wait for elden ring to exit
                        using var eldenRingProcess = _modEngine2Service.GetProcessByName("eldenring");
                        if (eldenRingProcess is null)
                        {
                            throw new InvalidOperationException(
                                "Could not find Elden Ring Process.");
                        }

                        eldenRingProcess.WaitForExit();
                    }
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
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                var logger = Logger.GetLogger(nameof(App));
                logger.Error(e.Message);
            }
        }

        public async Task Play(ProfileVm profileVm)
        {
            var dialogVm = new CustomDialogViewModel(
                "Launch Elden Ring",
                $"Are you sure you want to launch Elden Ring with this profile selected?" +
                "\n\nNote: Steam must be running and logged in.",
                fields: null,
                new List<DialogButtonViewModel>()
                {
                    new DialogButtonViewModel(
                        "Play",
                        result: true,
                        isDefault: false),
                    new DialogButtonViewModel(
                        "Cancel",
                        result: false,
                        isDefault: true)
                });

            var result = await _dialogService.ShowDialog(dialogVm);

            if (result is bool && result.Equals(true))
            {
                using var cts = new CancellationTokenSource();
                var runEldenRingTask = GetPlayTask(profileVm, cts.Token);
                await ShowPlayingDialog(runEldenRingTask, cts);
            }
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

                    if(!modEngineProcess.HasExited)
                    {
                        try
                        {
                            await modEngineProcess
                                .WaitForExitAsync(cancellationToken)
                                .ConfigureAwait(false);
                        }
                        catch (TaskCanceledException)
                        {
                            modEngineProcess.Kill();
                        }
                    }

                    //// wait for elden ring to exit
                    await Task.Delay(3000).ConfigureAwait(false);
                    await _modEngine2Service
                        .WaitForEldenRingExit(cancellationToken)
                        .ConfigureAwait(false);
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
            var playingDialogVm = new CustomDialogViewModel(
                "Playing Elden Ring",
                $"Waiting for the Elden Ring process to exit...",
                fields: null,
                new List<DialogButtonViewModel>
                {
                    new DialogButtonViewModel(
                        "Force Exit",
                        result: false,
                        isDefault: false)
                });

            var dialogResult = await _dialogService.ShowProgressDialog(playingDialogVm, backgroundTask);

            if(dialogResult is bool && dialogResult.Equals(false))
            {
                cts.Cancel();
            }

            await backgroundTask;
        }
    }
}
