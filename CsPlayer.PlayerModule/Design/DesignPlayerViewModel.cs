using CsPlayer.PlayerModule.ViewModels;
using CsPlayer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsPlayer.PlayerModule.Design
{
    class DesignPlayerViewModel
    {
        public PlaylistViewModel Playlist { get; private set; }

        public DesignPlayerViewModel()
        {
            var playlist = new Playlist("TestPlaylist");
            playlist.Songs.Add(new Song(@"C:\User\Desktop\TestSongOne.mp3"));
            playlist.Songs.Add(new Song(@"C:\User\Desktop\Files\Music\Songs\TestSongs\TestSongTwo.mp3"));
            playlist.Songs.Add(new Song(@"C:\User\Desktop\InvalidSongs\TestSongOne.mp3", false));
            playlist.Songs.Add(new Song(@"C:\User\Desktop\Files\Music\Songs\TestSongs\InvalidSongs\TestSongTwo.mp3", false));

            Playlist = new PlaylistViewModel(playlist);
        }
    }
}
