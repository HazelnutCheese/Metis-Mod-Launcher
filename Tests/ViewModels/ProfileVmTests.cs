using ModEngine2ConfigTool.Equality;
using ModEngine2ConfigTool.Models;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using ModEngine2ConfigTool.ViewModels.Profiles;
using Moq;

namespace Tests.ViewModels
{
    [TestFixture]
    public class ProfileVmTests
    {
        [Test]
        public void Ctor_FromProfile_SetsProperties()
        {
            var dispatcherService = Mock.Of<IDispatcherService>();
            var databaseService = Mock.Of<IDatabaseService>();

            var profile = new Profile
            {
                ProfileId = Guid.NewGuid(),
                Name = "HelloWorld",
                Description = "FizzBuzz",
                Created = DateTime.Now,
                LastPlayed = DateTime.Now,
                UseDebugMode = true,
                UseScyllaHide = true
            };

            var profileVm = new ProfileVm(
                profile,
                databaseService,
                dispatcherService);

            Assert.Multiple(() =>
            {
                Assert.That(profileVm.Name, Is.EqualTo(profile.Name));
                Assert.That(profileVm.Description, Is.EqualTo(profile.Description));
                Assert.That(profileVm.Created, Is.EqualTo(profile.Created));
                Assert.That(profileVm.LastPlayed, Is.EqualTo(profile.LastPlayed));
                Assert.That(profileVm.UseDebugMode, Is.EqualTo(profile.UseDebugMode));
                Assert.That(profileVm.UseScyllaHide, Is.EqualTo(profile.UseScyllaHide));
            });
        }

        [Test]
        public void Ctor_FromProfile_SetsMods_FromDatabaseService()
        {
            var mods = new List<Mod>()
            {
                new Mod()
                {
                    ModId = Guid.NewGuid(),
                },
                new Mod()
                {
                    ModId = Guid.NewGuid(),
                }
            };

            var databaseService = new Mock<IDatabaseService>();
            databaseService.Setup(x => x.GetMods()).Returns(mods);

            var profile = new Profile()
            {
                ProfileId = Guid.NewGuid(),
                Mods = mods
            };

            var profileVm = new ProfileVm(
                profile, 
                databaseService.Object, 
                Mock.Of<IDispatcherService>());

            var expected = mods.Select(x => new ModVm(x, Mock.Of<IDatabaseService>()));
            Assert.That(profileVm.Mods, Is.EquivalentTo(expected).Using(new ModVmEqualityComparer()));
        }

        [Test]
        public async Task RefreshAsync_UpdatesPropertiesFromDatabase()
        {
            var databaseService = new Mock<IDatabaseService>();

            var profileVm = new ProfileVm(
                "HelloWorld",
                databaseService.Object,
                Mock.Of<IDispatcherService>());

            var profile = new Profile()
            {
                ProfileId = profileVm.Model.ProfileId,
                Name = "new Name",
                Description = "new Description"
            };

            databaseService.Setup(x => x.GetProfiles()).Returns(new List<Profile> { profile });

            await profileVm.RefreshAsync();

            Assert.Multiple(() =>
            {
                Assert.That(profileVm.Name, Is.EqualTo(profile.Name));
                Assert.That(profileVm.Description, Is.EqualTo(profile.Description));
            });
        }

        [Test]
        public async Task RefreshAsync_UpdatesModsFromDatabase_OnDispatcherUiThread()
        {
            var databaseService = new Mock<IDatabaseService>();
            var dispatcherService = new Mock<IDispatcherService>();

            dispatcherService
                .Setup(x => x.InvokeUiAsync(It.IsAny<Action>()))
                .Callback<Action>(a => a.Invoke());

            var profileVm = new ProfileVm(
                "HelloWorld",
                databaseService.Object,
                dispatcherService.Object);

            var profile = new Profile()
            {
                ProfileId = profileVm.Model.ProfileId,
                Mods = new List<Mod>()
                {
                    new Mod()
                    {
                        ModId = Guid.NewGuid()
                    },
                    new Mod()
                    {
                        ModId = Guid.NewGuid()
                    }
                }
            };

            databaseService.Setup(x => x.GetProfiles()).Returns(new List<Profile> { profile });

            await profileVm.RefreshAsync();

            dispatcherService.Verify(x => x.InvokeUiAsync(It.IsAny<Action>()), Times.Once);

            var expected = profile.Mods.Select(x => new ModVm(x, Mock.Of<IDatabaseService>()));
            Assert.That(profileVm.Mods, Is.EquivalentTo(expected).Using(new ModVmEqualityComparer()));
        }
    }
}
