using ModEngine2ConfigTool.Equality;
using ModEngine2ConfigTool.Models;

namespace Tests.Equality
{
    [TestFixture]
    public class ProfileEqualityComparerTests
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

            var comparer = new ProfileEqualityComparer();

            Assert.That(comparer.Equals(profile1, profile2), Is.True);
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

            var comparer = new ProfileEqualityComparer();

            Assert.That(comparer.Equals(profile1, profile2), Is.True);
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

            var comparer = new ProfileEqualityComparer();

            Assert.That(comparer.Equals(profile1, profile2), Is.False);
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

            var comparer = new ProfileEqualityComparer();

            Assert.That(comparer.GetHashCode(profile2), Is.EqualTo(comparer.GetHashCode(profile1)));
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

            var comparer = new ProfileEqualityComparer();

            Assert.That(comparer.GetHashCode(profile2), Is.EqualTo(comparer.GetHashCode(profile1)));
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

            var comparer = new ProfileEqualityComparer();

            Assert.That(comparer.GetHashCode(profile2), Is.Not.EqualTo(comparer.GetHashCode(profile1)));
        }
    }
}