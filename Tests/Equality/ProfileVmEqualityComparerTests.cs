using ModEngine2ConfigTool.Equality;
using ModEngine2ConfigTool.Models;
using ModEngine2ConfigTool.Services.Interfaces;
using ModEngine2ConfigTool.ViewModels.Profiles;
using Moq;

namespace Tests.Equality
{
    [TestFixture]
    public class ProfileVmEqualityComparerTests
    {
        [Test]
        public void MatchingProfileId_Equals_ReturnsTrue()
        {
            var profileId = Guid.NewGuid();

            var profile1 = new Profile
            {
                ProfileId = profileId
            };

            var profile2 = new Profile
            {
                ProfileId = profileId
            };

            var profileVm1 = new ProfileVm(
                profile1, 
                Mock.Of<IDatabaseService>(), 
                Mock.Of<IDispatcherService>());

            var profileVm2 = new ProfileVm(
                profile2,
                Mock.Of<IDatabaseService>(),
                Mock.Of<IDispatcherService>());

            var comparer = new ProfileVmEqualityComparer();

            Assert.That(comparer.Equals(profileVm1, profileVm2), Is.True);
        }

        [Test]
        public void MatchingProfileId_DifferentProperties_Equals_ReturnsTrue()
        {
            var profileId = Guid.NewGuid();

            var profile1 = new Profile
            {
                ProfileId = profileId,
                Name = "Foo",
                Description = "Bar",
                ImagePath = "Buzz",
                Created = DateTime.Now,
                LastPlayed = DateTime.Now,
                Mods = new List<Mod>
                {
                    new Mod
                    {
                        ModId = Guid.NewGuid()
                    }
                }
            };

            var profile2 = new Profile
            {
                ProfileId = profileId,
                Name = "Hello",
                Description = "World",
                ImagePath = "B",
                Created = DateTime.Now,
                LastPlayed = DateTime.Now,
                Mods = new List<Mod>
                {
                    new Mod
                    {
                        ModId = Guid.NewGuid()
                    }
                }
            };

            var profileVm1 = new ProfileVm(
                profile1,
                Mock.Of<IDatabaseService>(),
                Mock.Of<IDispatcherService>());

            var profileVm2 = new ProfileVm(
                profile2,
                Mock.Of<IDatabaseService>(),
                Mock.Of<IDispatcherService>());

            var comparer = new ProfileVmEqualityComparer();

            Assert.That(comparer.Equals(profileVm1, profileVm2), Is.True);
        }

        [Test]
        public void DifferentProfileId_Equals_ReturnsFalse()
        {
            var profile1 = new Profile
            {
                ProfileId = Guid.NewGuid()
            };

            var profile2 = new Profile
            {
                ProfileId = Guid.NewGuid()
            };

            var profileVm1 = new ProfileVm(
                profile1,
                Mock.Of<IDatabaseService>(),
                Mock.Of<IDispatcherService>());

            var profileVm2 = new ProfileVm(
                profile2,
                Mock.Of<IDatabaseService>(),
                Mock.Of<IDispatcherService>());

            var comparer = new ProfileVmEqualityComparer();

            Assert.That(comparer.Equals(profileVm1, profileVm2), Is.False);
        }

        [Test]
        public void MatchingProfileId_GetHashCode_SameResult()
        {
            var profileId = Guid.NewGuid();

            var profile1 = new Profile
            {
                ProfileId = profileId
            };

            var profile2 = new Profile
            {
                ProfileId = profileId
            };

            var profileVm1 = new ProfileVm(
                profile1,
                Mock.Of<IDatabaseService>(),
                Mock.Of<IDispatcherService>());

            var profileVm2 = new ProfileVm(
                profile2,
                Mock.Of<IDatabaseService>(),
                Mock.Of<IDispatcherService>());

            var comparer = new ProfileVmEqualityComparer();

            Assert.That(comparer.GetHashCode(profileVm2), Is.EqualTo(comparer.GetHashCode(profileVm1)));
        }

        [Test]
        public void MatchingProfileId_DifferentProperties_GetHashCode_SameResult()
        {
            var ProfileId = Guid.NewGuid();

            var profile1 = new Profile
            {
                ProfileId = ProfileId,
                Name = "Foo",
                Description = "Bar",
                ImagePath = "Buzz",
                Created = DateTime.Now,
                LastPlayed = DateTime.Now,
                Mods = new List<Mod>
                {
                    new Mod
                    {
                        ModId = Guid.NewGuid()
                    }
                }
            };

            var profile2 = new Profile
            {
                ProfileId = ProfileId,
                Name = "Hello",
                Description = "World",
                ImagePath = "B",
                Created = DateTime.Now,
                LastPlayed = DateTime.Now,
                Mods = new List<Mod>
                {
                    new Mod
                    {
                        ModId = Guid.NewGuid()
                    }
                }
            };

            var profileVm1 = new ProfileVm(
                profile1,
                Mock.Of<IDatabaseService>(),
                Mock.Of<IDispatcherService>());

            var profileVm2 = new ProfileVm(
                profile2,
                Mock.Of<IDatabaseService>(),
                Mock.Of<IDispatcherService>());

            var comparer = new ProfileVmEqualityComparer();

            Assert.That(comparer.GetHashCode(profileVm2), Is.EqualTo(comparer.GetHashCode(profileVm1)));
        }

        [Test]
        public void DifferentProfileId_GetHashCode_DifferentResult()
        {
            var profile1 = new Profile
            {
                ProfileId = Guid.NewGuid()
            };

            var profile2 = new Profile
            {
                ProfileId = Guid.NewGuid()
            };

            var profileVm1 = new ProfileVm(
                profile1,
                Mock.Of<IDatabaseService>(),
                Mock.Of<IDispatcherService>());

            var profileVm2 = new ProfileVm(
                profile2,
                Mock.Of<IDatabaseService>(),
                Mock.Of<IDispatcherService>());

            var comparer = new ProfileVmEqualityComparer();

            Assert.That(comparer.GetHashCode(profileVm2), Is.Not.EqualTo(comparer.GetHashCode(profileVm1)));
        }
    }
}