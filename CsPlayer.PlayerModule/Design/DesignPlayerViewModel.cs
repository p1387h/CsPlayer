using CsPlayer.PlayerModule.ViewModels;
using CsPlayer.Shared;
using Prism.Events;
using Prism.Logging;
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
            var eventAggregator = new EventAggregator();
            var playlistModel = new Playlist("TestPlaylist");

            playlistModel.Songs.Add(new Song(@"C:\User\Desktop\TestSongOne.mp3"));
            playlistModel.Songs.Add(new Song(@"C:\User\Desktop\Files\Music\Songs\TestSongs\TestSongTwo.mp3"));
            playlistModel.Songs.Add(new Song(@"C:\User\Desktop\InvalidSongs\TestSongOne.mp3", false));
            playlistModel.Songs.Add(new Song(@"C:\User\Desktop\Files\Music\Songs\TestSongs\InvalidSongs\TestSongTwo.mp3", false));

            Playlist = new PlaylistViewModel(eventAggregator) { Playlist = playlistModel };
        }
    }
}
