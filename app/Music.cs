using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app
{
    class Music
    {
        public string Name { get; set; }
        public string Artist { get; set; }
        public string Track { get; set; }
        public string SongId { get; set; }
        public DateTime Duration { get; set; }

        public int alreadyTried = 0;

        public Music(string name, string artist)
        {
            Name = name;
            Artist = artist;
        }
        public Music()
        {

        }

    }
}
