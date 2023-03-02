using FluentAssertions;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.Services.Interfaces;
using ModEngine2ConfigTool.ViewModels.Profiles;
using Moq;

namespace Tests.Services
{
    [TestFixture]
    public class PlayManagerServiceTests
    {
        [Test]
        public void Play_VanillaSavesBackedUp()
        {
            var saveManagerService = new Mock<ISaveManagerService>();

            var playManagerService = new PlayManagerService(
                Mock.Of<IProfileService>(),
                saveManagerService.Object,
                Mock.Of<IModEngine2Service>(),
                Mock.Of<IDialogService>());

            playManagerService.Play(Mock.Of<IProfileVm>(), true);

            saveManagerService.Verify(x => x.BackupVanilla());
        }

        [Test]
        public void Play_VanillaBackUpFailed_Rethrows()
        {
            var saveManagerService = new Mock<ISaveManagerService>();
            saveManagerService
                .Setup(x => x.BackupVanilla())
                .Throws<InvalidOperationException>();

            var dialogService = new Mock<IDialogService>();

            var playManagerService = new PlayManagerService(
                Mock.Of<IProfileService>(),
                saveManagerService.Object,
                Mock.Of<IModEngine2Service>(),
                dialogService.Object);

            var call = () => playManagerService.Play(Mock.Of<IProfileVm>(), true);

            call.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void Play_ProfileUsesSaveManager_CallsBackupProfile()
        {
            var profile = new Mock<IProfileVm>();
            profile.SetupProperty(x => x.UseSaveManager, true);

            var saveManagerService = new Mock<ISaveManagerService>();

            var playManagerService = new PlayManagerService(
                Mock.Of<IProfileService>(),
                saveManagerService.Object,
                Mock.Of<IModEngine2Service>(),
                Mock.Of<IDialogService>());

            playManagerService.Play(profile.Object, true);

            saveManagerService.Verify(x => x.BackupProfile(profile.Object));
        }

        [Test]
        public void Play_ProfileBackupFails_Rethrows()
        {
            var profile = new Mock<IProfileVm>();
            profile.SetupProperty(x => x.UseSaveManager, true);

            var saveManagerService = new Mock<ISaveManagerService>();
            saveManagerService
                .Setup(x => x.BackupProfile(profile.Object))
                .Throws<InvalidOperationException>();

            var playManagerService = new PlayManagerService(
                Mock.Of<IProfileService>(),
                saveManagerService.Object,
                Mock.Of<IModEngine2Service>(),
                Mock.Of<IDialogService>());

            var call = () => playManagerService.Play(profile.Object, true);

            call.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void Play_ProfileUsesSaveManager_CallsPushSaves()
        {
            var profile = new Mock<IProfileVm>();
            profile.SetupProperty(x => x.UseSaveManager, true);

            var saveManagerService = new Mock<ISaveManagerService>();

            var playManagerService = new PlayManagerService(
                Mock.Of<IProfileService>(),
                saveManagerService.Object,
                Mock.Of<IModEngine2Service>(),
                Mock.Of<IDialogService>());

            playManagerService.Play(profile.Object, true);

            saveManagerService.Verify(x => x.Push(profile.Object));
        }

        [Test]
        public void Play_PushProfileSavesFails_Rethrows()
        {
            var profile = new Mock<IProfileVm>();
            profile.SetupProperty(x => x.UseSaveManager, true);

            var saveManagerService = new Mock<ISaveManagerService>();
            saveManagerService
                .Setup(x => x.Push(profile.Object))
                .Throws<InvalidOperationException>();

            var playManagerService = new PlayManagerService(
                Mock.Of<IProfileService>(),
                saveManagerService.Object,
                Mock.Of<IModEngine2Service>(),
                Mock.Of<IDialogService>());

            var call = () => playManagerService.Play(profile.Object, true);

            call.Should().Throw<InvalidOperationException>();
        }


        [Test]
        public void Play_PushProfileSavesFails_CallsPopSaves()
        {
            var profile = new Mock<IProfileVm>();
            profile.SetupProperty(x => x.UseSaveManager, true);

            var saveManagerService = new Mock<ISaveManagerService>();
            saveManagerService
                .Setup(x => x.Push(profile.Object))
                .Throws<InvalidOperationException>();

            var playManagerService = new PlayManagerService(
                Mock.Of<IProfileService>(),
                saveManagerService.Object,
                Mock.Of<IModEngine2Service>(),
                Mock.Of<IDialogService>());

            var call = () => playManagerService.Play(profile.Object, true);

            call.Should().Throw<InvalidOperationException>();

            saveManagerService.Verify(x => x.Pop(profile.Object));
        }

        [Test]
        public void Play_CallsProfileServiceWrite()
        {
            var profile = new Mock<IProfileVm>();
            var profileService = new Mock<IProfileService>();

            var playManagerService = new PlayManagerService(
                profileService.Object,
                Mock.Of<ISaveManagerService>(),
                Mock.Of<IModEngine2Service>(),
                Mock.Of<IDialogService>());

            playManagerService.Play(profile.Object, true);

            profileService.Verify(x => x.WriteProfile(profile.Object));
        }

        [Test]
        public void Play_ProfileServiceWriteFails_Rethrows()
        {
            var profile = new Mock<IProfileVm>();
            profile.SetupProperty(x => x.UseSaveManager, true);

            var profileService = new Mock<IProfileService>();
            profileService
                .Setup(x => x.WriteProfile(profile.Object))
                .Throws<InvalidOperationException>();

            var saveManagerService = new Mock<ISaveManagerService>();

            var playManagerService = new PlayManagerService(
                profileService.Object,
                saveManagerService.Object,
                Mock.Of<IModEngine2Service>(),
                Mock.Of<IDialogService>());

            var call = () => playManagerService.Play(profile.Object, true);

            call.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void Play_ProfileUsesSaveManager_ProfileServiceWriteFails_CallsPopSaves()
        {
            var profile = new Mock<IProfileVm>();
            profile.SetupProperty(x => x.UseSaveManager, true);

            var profileService = new Mock<IProfileService>();
            profileService
                .Setup(x => x.WriteProfile(profile.Object))
                .Throws<InvalidOperationException>();

            var saveManagerService = new Mock<ISaveManagerService>();

            var playManagerService = new PlayManagerService(
                profileService.Object,
                saveManagerService.Object,
                Mock.Of<IModEngine2Service>(),
                Mock.Of<IDialogService>());

            var call = () => playManagerService.Play(profile.Object, true);

            call.Should().Throw<InvalidOperationException>();

            saveManagerService.Verify(x => x.Pop(profile.Object));
        }
    }
}
