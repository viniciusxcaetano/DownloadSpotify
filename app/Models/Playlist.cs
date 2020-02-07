using System;
using System.Collections.Generic;
using System.IO;

namespace app
{
    public class Playlist
    {
        public Playlist() { }
        public Playlist(string Path, string Url)
        {
            this.Device = Path;
            this.Url = Url;
        }

        public const int Attempts = 1;

        public string Name { get; set; }
        public string Url { get; set; }
        public string Device { get; set; }
        public string PathFolder { get; set; }
        public string PathUrlFile { get; set; }
        public List<Music> Music { get; set; }
        public List<Music> MusicsToDownload { get; set; }
    }
}