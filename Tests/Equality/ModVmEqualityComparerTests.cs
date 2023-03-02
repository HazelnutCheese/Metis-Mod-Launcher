using ModEngine2ConfigTool.Equality;
using ModEngine2ConfigTool.Models;
using ModEngine2ConfigTool.Services.Interfaces;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using Moq;

namespace Tests.Equality
{
    [TestFixture]
    public class ModVmEqualityComparerTests
    {
        [Test]
        public void MatchingModId_Equals_ReturnsTrue()
        {
            var modId = Guid.NewGuid();

            var mod1 = new Mod
            {
                ModId = modId
            };

            var mod2 = new Mod
            {
                ModId = modId
            };

            var modVm1 = new ModVm(mod1, Mock.Of<IDatabaseService>());
            var modVm2 = new ModVm(mod2, Mock.Of<IDatabaseService>());

            var comparer = new ModVmEqualityComparer();

            Assert.That(comparer.Equals(modVm1, modVm2), Is.True);
        }

        [Test]
        public void MatchingModId_DifferentProperties_Equals_ReturnsTrue()
        {
            var modId = Guid.NewGuid();

            var mod1 = new Mod
            {
                ModId = modId,
                Name = "Foo",
                Description = "Bar",
                FolderPath = "Fizz",
                ImagePath = "Buzz",
                Added = DateTime.Now,
                Profiles = new List<Profile>
                {
                    new Profile
                    {
                        ProfileId = Guid.NewGuid()
                    }
                }
            };

            var mod2 = new Mod
            {
                ModId = modId,
                Name = "Hello",
                Description = "World",
                FolderPath = "A",
                ImagePath = "B",
                Added = DateTime.Now,
                Profiles = new List<Profile>
                {
                    new Profile
                    {
                        ProfileId = Guid.NewGuid()
                    }
                }
            };

            var modVm1 = new ModVm(mod1, Mock.Of<IDatabaseService>());
            var modVm2 = new ModVm(mod2, Mock.Of<IDatabaseService>());

            var comparer = new ModVmEqualityComparer();

            Assert.That(comparer.Equals(modVm1, modVm2), Is.True);
        }

        [Test]
        public void DifferentModId_Equals_ReturnsFalse()
        {
            var mod1 = new Mod
            {
                ModId = Guid.NewGuid()
            };

            var mod2 = new Mod
            {
                ModId = Guid.NewGuid()
            };

            var modVm1 = new ModVm(mod1, Mock.Of<IDatabaseService>());
            var modVm2 = new ModVm(mod2, Mock.Of<IDatabaseService>());

            var comparer = new ModVmEqualityComparer();

            Assert.That(comparer.Equals(modVm1, modVm2), Is.False);
        }

        [Test]
        public void MatchingModId_GetHashCode_SameResult()
        {
            var modId = Guid.NewGuid();

            var mod1 = new Mod
            {
                ModId = modId
            };

            var mod2 = new Mod
            {
                ModId = modId
            };

            var modVm1 = new ModVm(mod1, Mock.Of<IDatabaseService>());
            var modVm2 = new ModVm(mod2, Mock.Of<IDatabaseService>());

            var comparer = new ModVmEqualityComparer();

            Assert.That(comparer.GetHashCode(modVm2), Is.EqualTo(comparer.GetHashCode(modVm1)));
        }

        [Test]
        public void MatchingModId_DifferentProperties_GetHashCode_SameResult()
        {
            var modId = Guid.NewGuid();

            var mod1 = new Mod
            {
                ModId = modId,
                Name = "Foo",
                Description = "Bar",
                FolderPath = "Fizz",
                ImagePath = "Buzz",
                Added = DateTime.Now,
                Profiles = new List<Profile>
                {
                    new Profile
                    {
                        ProfileId = Guid.NewGuid()
                    }
                }
            };

            var mod2 = new Mod
            {
                ModId = modId,
                Name = "Hello",
                Description = "World",
                FolderPath = "A",
                ImagePath = "B",
                Added = DateTime.Now,
                Profiles = new List<Profile>
                {
                    new Profile
                    {
                        ProfileId = Guid.NewGuid()
                    }
                }
            };

            var modVm1 = new ModVm(mod1, Mock.Of<IDatabaseService>());
            var modVm2 = new ModVm(mod2, Mock.Of<IDatabaseService>());

            var comparer = new ModVmEqualityComparer();

            Assert.That(comparer.GetHashCode(modVm2), Is.EqualTo(comparer.GetHashCode(modVm1)));
        }

        [Test]
        public void DifferentModId_GetHashCode_DifferentResult()
        {
            var mod1 = new Mod
            {
                ModId = Guid.NewGuid()
            };

            var mod2 = new Mod
            {
                ModId = Guid.NewGuid()
            };

            var modVm1 = new ModVm(mod1, Mock.Of<IDatabaseService>());
            var modVm2 = new ModVm(mod2, Mock.Of<IDatabaseService>());

            var comparer = new ModVmEqualityComparer();

            Assert.That(comparer.GetHashCode(modVm2), Is.Not.EqualTo(comparer.GetHashCode(modVm1)));
        }
    }
}