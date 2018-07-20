using System.Collections.Generic;

namespace CsPlayer.Shared
{
    public interface IPlaylist
    {
        string Name { get; set; }
        List<Song> Songs { get; }
        bool Valid { get; set; }
    }
}