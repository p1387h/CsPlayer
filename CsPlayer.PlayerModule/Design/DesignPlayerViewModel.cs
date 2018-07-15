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
        private class DesignLogger : ILoggerFacade
        {
            public void Log(string message, Category category, Priority priority)
            {
                throw new NotImplementedException();
            }
        }

        public PlaylistViewModel Playlist { get; private set; }

        public DesignPlayerViewModel()
        {
            var eventAggregator = new EventAggregator();
            var logger = new DesignLogger();
            var playlist = new Playlist("TestPlaylist");

            playlist.Songs.Add(new Song(@"C:\User\Desktop\TestSongOne.mp3"));
            playlist.Songs.Add(new Song(@"C:\User\Desktop\Files\Music\Songs\TestSongs\TestSongTwo.mp3"));
            playlist.Songs.Add(new Song(@"C:\User\Desktop\InvalidSongs\TestSongOne.mp3", false));
            playlist.Songs.Add(new Song(@"C:\User\Desktop\Files\Music\Songs\TestSongs\InvalidSongs\TestSongTwo.mp3", false));

            Playlist = new PlaylistViewModel(playlist, eventAggregator, logger);
        }
    }
}
