namespace Jellyfin.Plugin.Lastfm.Tests
{
    using System;
    using Jellyfin.Plugin.Lastfm.Scrobbling;
    using Xunit;

    public class CustomThresholdTests
    {
        [Theory]
        [InlineData(1, 30)]
        [InlineData(3, 90)]
        [InlineData(4, 120)]
        [InlineData(10, 120)]
        public void CalculateThresholdUsesTheEarlierCondition(int runtimeMinutes, int expectedSeconds)
        {
            var threshold = CustomThreshold.CalculateThresholdTicks(
                TimeSpan.FromMinutes(runtimeMinutes).Ticks,
                50,
                2);

            Assert.Equal(TimeSpan.FromSeconds(expectedSeconds).Ticks, threshold);
        }

        [Fact]
        public void TrackerMarksAPlaybackOnlyOnce()
        {
            var tracker = new ScrobbleTracker();

            Assert.True(tracker.TryMarkScrobbled("session-a:user-a:item-a"));
            Assert.False(tracker.TryMarkScrobbled("session-a:user-a:item-a"));
        }

        [Fact]
        public void TrackerDoesNotShareStateBetweenTracksOrSessions()
        {
            var tracker = new ScrobbleTracker();

            Assert.True(tracker.TryMarkScrobbled("session-a:user-a:item-a"));
            Assert.True(tracker.TryMarkScrobbled("session-a:user-a:item-b"));
            Assert.True(tracker.TryMarkScrobbled("session-b:user-a:item-a"));
        }

        [Fact]
        public void TrackerCanResetAPlaybackWhenTheTrackStartsAgain()
        {
            var tracker = new ScrobbleTracker();
            const string playbackKey = "session-a:user-a:item-a";

            tracker.Begin(playbackKey);
            Assert.True(tracker.IsStarted(playbackKey));
            Assert.True(tracker.TryMarkScrobbled(playbackKey));

            tracker.Remove(playbackKey);

            Assert.False(tracker.IsStarted(playbackKey));
            Assert.True(tracker.TryMarkScrobbled(playbackKey));
        }

        [Fact]
        public void InvalidSettingsAreSafelyNormalized()
        {
            var runtime = TimeSpan.FromMinutes(10).Ticks;

            Assert.Equal(0, CustomThreshold.CalculateThresholdTicks(runtime, -1, 2));
            Assert.Equal(TimeSpan.FromMinutes(2).Ticks, CustomThreshold.CalculateThresholdTicks(runtime, 50, 0));
        }

        [Fact]
        public void TrackCompletionPositionIsAlwaysAccepted()
        {
            var runtime = TimeSpan.FromSeconds(20).Ticks;
            var threshold = CustomThreshold.CalculateThresholdTicks(runtime, 50, 2);

            Assert.Equal(TimeSpan.FromSeconds(10).Ticks, threshold);
            Assert.True(CustomThreshold.IsReached(runtime, runtime, 50, 2));
        }
    }
}
