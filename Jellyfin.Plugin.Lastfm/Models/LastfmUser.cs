namespace Jellyfin.Plugin.Lastfm.Models
{
    using System;

    public class LastfmUser
    {
        public string Username { get; set; }

        //We wont store the password, but instead store the session key since its a lifetime key
        public string SessionKey { get; set; }

        public Guid MediaBrowserUserId { get; set; }

        public LastFmUserOptions Options { get; set; }
    }

    public class LastFmUserOptions
    {
        public bool Scrobble        { get; set; }
        public bool SyncFavourites  { get; set; }
        // Retained for compatibility with existing configuration files.
        public bool AlternativeMode { get; set; }
        public ScrobblingMode ScrobblingMode { get; set; }
        public int MinimumPercentage { get; set; } = 50;
        public int MinimumTimeMinutes { get; set; } = 2;
    }

    public enum ScrobblingMode
    {
        PlaybackStopped = 0,
        UserDataSaved = 1,
        CustomThreshold = 2
    }
}
