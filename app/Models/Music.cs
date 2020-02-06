using System;

namespace app
{
    public class Music
    {
        public Music() { }

        public Music(string name, string artist)
        {
            Name = name;
            Artist = artist;
        }

        public string Name { get; set; }
        public string Artist { get; set; }
        public string Track { get; set; }
        public string Id { get; set; }
        public DateTime Duration { get; set; }
        public int alreadyTried { get; set; }
    }
}