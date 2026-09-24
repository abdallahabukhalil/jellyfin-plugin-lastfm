namespace Jellyfin.Plugin.Lastfm.Scrobbling
{
    using System;
    using System.Collections.Concurrent;

    public static class CustomThreshold
    {
        public const int DefaultPercentage = 50;
        public const int DefaultTimeMinutes = 2;

        public static long CalculateThresholdTicks(long runtimeTicks, int percentage, int timeMinutes)
        {
            if (runtimeTicks <= 0)
            {
                return 0;
            }

            var validPercentage = Math.Clamp(percentage, 0, 100);
            var validTimeMinutes = timeMinutes > 0 ? timeMinutes : DefaultTimeMinutes;
            var percentageTicks = (long)((decimal)runtimeTicks * validPercentage / 100m);
            var timeTicks = (long)TimeSpan.FromMinutes(validTimeMinutes).Ticks;
            return Math.Min(percentageTicks, timeTicks);
        }

        public static bool IsReached(long runtimeTicks, long positionTicks, int percentage, int timeMinutes)
        {
            return positionTicks >= CalculateThresholdTicks(runtimeTicks, percentage, timeMinutes);
        }
    }

    public sealed class ScrobbleTracker
    {
        private readonly ConcurrentDictionary<string, byte> _scrobbled = new();
        private readonly ConcurrentDictionary<string, byte> _started = new();

        public void Begin(string playbackKey)
        {
            _started.TryAdd(playbackKey, 0);
        }

        public bool IsStarted(string playbackKey)
        {
            return _started.ContainsKey(playbackKey);
        }

        public bool TryMarkScrobbled(string playbackKey)
        {
            return _scrobbled.TryAdd(playbackKey, 0);
        }

        public bool IsScrobbled(string playbackKey)
        {
            return _scrobbled.ContainsKey(playbackKey);
        }

        public bool TryScrobbleAtProgress(string playbackKey, long runtimeTicks, long positionTicks, int percentage, int timeMinutes)
        {
            return IsStarted(playbackKey)
                && CustomThreshold.IsReached(runtimeTicks, positionTicks, percentage, timeMinutes)
                && TryMarkScrobbled(playbackKey);
        }

        public void Remove(string playbackKey)
        {
            _scrobbled.TryRemove(playbackKey, out _);
            _started.TryRemove(playbackKey, out _);
        }

        public void Clear()
        {
            _scrobbled.Clear();
            _started.Clear();
        }
    }
}
