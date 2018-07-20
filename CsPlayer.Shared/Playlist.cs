using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsPlayer.Shared
{
    public class Playlist : IPlaylist
    {
        public string Name { get; set; }
        public bool Valid { get; set; }
        public List<Song> Songs { get; private set; }

        public Playlist(string name, bool valid = true)
        {
            Name = name;
            Valid = valid;
            Songs = new List<Song>();
        }

        public bool Verify()
        {
            var noInvalids = Songs.Select(x => x.Verify()).All(x => x);
            Valid = noInvalids;

            return noInvalids;
        }
    }
}
