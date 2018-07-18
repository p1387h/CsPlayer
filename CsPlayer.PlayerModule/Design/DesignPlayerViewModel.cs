using CsPlayer.PlayerModule.ViewModels;
using CsPlayer.Shared;
using Microsoft.Practices.Unity;
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
            var container = new UnityContainer();
            var playlistModel = new Playlist("TestPlaylist");

            container.RegisterInstance<IEventAggregator>(new EventAggregator());
            container.RegisterInstance<ILoggerFacade>(new DesignLogger());
            container.RegisterInstance<IUnityContainer>(container);

            playlistModel.Songs.Add(new Song(@"C:\User\Desktop\TestSongOne.mp3"));
            playlistModel.Songs.Add(new Song(@"C:\User\Desktop\Files\Music\Songs\TestSongs\TestSongTwo.mp3"));
            playlistModel.Songs.Add(new Song(@"C:\User\Desktop\InvalidSongs\TestSongOne.mp3", false));
            playlistModel.Songs.Add(new Song(@"C:\User\Desktop\Files\Music\Songs\TestSongs\InvalidSongs\TestSongTwo.mp3", false));

            var viewModel = container.Resolve<PlaylistViewModel>();
            viewModel.SetPlaylist(playlistModel);

            // List of songs:
            Playlist = viewModel;

            // Playing song:
            Playlist.Songs.First().IsPlaying = true;
        }
    }
}
