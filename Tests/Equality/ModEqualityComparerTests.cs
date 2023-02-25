using ModEngine2ConfigTool.Equality;
using ModEngine2ConfigTool.Models;

namespace Tests.Equality
{
    [TestFixture]
    public class ModEqualityComparerTests
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

            var comparer = new ModEqualityComparer();

            Assert.That(comparer.Equals(mod1, mod2), Is.True);
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

            var comparer = new ModEqualityComparer();

            Assert.That(comparer.Equals(mod1, mod2), Is.True);
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

            var comparer = new ModEqualityComparer();

            Assert.That(comparer.Equals(mod1, mod2), Is.False);
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

            var comparer = new ModEqualityComparer();

            Assert.That(comparer.GetHashCode(mod2), Is.EqualTo(comparer.GetHashCode(mod1)));
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

            var comparer = new ModEqualityComparer();

            Assert.That(comparer.GetHashCode(mod2), Is.EqualTo(comparer.GetHashCode(mod1)));
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

            var comparer = new ModEqualityComparer();

            Assert.That(comparer.GetHashCode(mod2), Is.Not.EqualTo(comparer.GetHashCode(mod1)));
        }
    }
}