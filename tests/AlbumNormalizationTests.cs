namespace Jellyfin.Plugin.Lastfm.Tests
{
    using Jellyfin.Plugin.Lastfm.Models;
    using Jellyfin.Plugin.Lastfm.Utils;
    using Xunit;

    public class AlbumNormalizationTests
    {
        [Theory]
        [InlineData("Album Name - EP", "Album Name")]
        [InlineData("Album Name - Single", "Album Name")]
        [InlineData("Album Name - single", "Album Name")]
        [InlineData("Album Name - ep", "Album Name")]
        [InlineData("Album Name - Unreleased", "Album Name")]
        [InlineData("Album Name - unreleased", "Album Name")]
        [InlineData("my album - SINGLE", "my album")]
        [InlineData("Album Name - EP Deluxe", "Album Name - EP Deluxe")]
        [InlineData("Album Name - Single Edition", "Album Name - Single Edition")]
        [InlineData("Album Name - Unreleased Edition", "Album Name - Unreleased Edition")]
        [InlineData("My Album", "My Album")]
        [InlineData("EP", "EP")]
        [InlineData("Single", "Single")]
        [InlineData("Unreleased", "Unreleased")]
        public void NormalizesOnlyExactAlbumSuffixes(string album, string expected)
        {
            Assert.Equal(expected, Helpers.NormalizeAlbumForLastfm(album));
        }

        [Theory]
        [InlineData("Album Name - EP")]
        [InlineData("Album Name - single")]
        [InlineData("Album Name - Unreleased")]
        public void DisabledNormalizationLeavesTheAlbumUntouched(string album)
        {
            Assert.Equal(album, Helpers.NormalizeAlbumForLastfm(album, false));
        }

        [Fact]
        public void DisabledNormalizationReturnsNullAlbumAsNull()
        {
            Assert.Null(Helpers.NormalizeAlbumForLastfm(null, false));
        }

        [Fact]
        public void AlbumSuffixNormalizationIsEnabledByDefault()
        {
            Assert.True(new LastFmUserOptions().NormalizeAlbumSuffixes);
        }

        [Theory]
        [InlineData("Album Name - EP")]
        [InlineData("Album Name - Single")]
        public void NormalizationDoesNotMutateTheOriginalAlbumMetadata(string album)
        {
            var originalAlbum = album;

            _ = Helpers.NormalizeAlbumForLastfm(album);

            Assert.Equal(originalAlbum, album);
        }
    }
}
