namespace Jellyfin.Plugin.Lastfm.Tests
{
    using Jellyfin.Plugin.Lastfm.Utils;
    using Xunit;

    public class AlbumNormalizationTests
    {
        [Theory]
        [InlineData("Album Name - EP", "Album Name")]
        [InlineData("Album Name - Single", "Album Name")]
        [InlineData("Album Name - EP Deluxe", "Album Name - EP Deluxe")]
        [InlineData("Album Name - Single Edition", "Album Name - Single Edition")]
        [InlineData("My Album", "My Album")]
        [InlineData("EP", "EP")]
        [InlineData("Single", "Single")]
        public void NormalizesOnlyExactAlbumSuffixes(string album, string expected)
        {
            Assert.Equal(expected, Helpers.NormalizeAlbumForLastfm(album));
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
