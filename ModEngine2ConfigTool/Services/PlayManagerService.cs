using ModEngine2ConfigTool.ViewModels.Dialogs;
using ModEngine2ConfigTool.ViewModels.Profiles;
using Sherlog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                    //// launch modengine2 with toml then wait for exit
                    using var modEngineProcess = _modEngine2Service.LaunchWithProfile(profileToml);

                    try
                    {
                        if (!modEngineProcess.HasExited)
                        {
                            await modEngineProcess
                                .WaitForExitAsync(cancellationToken)
                                .ConfigureAwait(false);
                        }
                    }
                    catch (TaskCanceledException)
                    {
                        TryKill(modEngineProcess);
                    }
                    catch
                    {
                        TryKill(modEngineProcess);
                        throw;
                    }

                    //// wait for elden ring to exit
                    using var eldenRingProcess = await _modEngine2Service
                        .PollForEldenRingProcess(
                            10000, 
                            cancellationToken)
                        .ConfigureAwait(false);

                    try
                    {
                        if (!eldenRingProcess.HasExited)
                        {
                            await eldenRingProcess
                                .WaitForExitAsync(cancellationToken)
                                .ConfigureAwait(false);
                        }
                    }
                    catch (TaskCanceledException)
                    {
                        TryKill(eldenRingProcess);
                    }
                    catch
                    {
                        TryKill(eldenRingProcess);
                        throw;
                    }
                }
                catch (TaskCanceledException)
                {

                }
                catch (InvalidOperationException e)
                {
                    MessageBox.Show(e.Message);
                    Log.Instance.Error(e.Message);
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
                Log.Instance.Error(e.Message);
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

        private void TryKill(Process process)
        {
            try
            {
                process.Kill();
            }
            catch(Exception e)
            {
                Log.Instance.Error(e.Message);
            }
        }
    }
}
