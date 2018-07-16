using CsPlayer.PlayerEvents;
using CsPlayer.Shared;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CsPlayer.PlayerModule.ViewModels
{
    class PlayerViewModel : BindableBase
    {
        private PlaylistViewModel _playlist;
        public PlaylistViewModel Playlist
        {
            get { return _playlist; }
            private set { SetProperty<PlaylistViewModel>(ref _playlist, value); }
        }

        public ICommand ButtonPrevious { get; private set; }
        public ICommand ButtonPlay { get; private set; }
        public ICommand ButtonPause { get; private set; }
        public ICommand ButtonStop { get; private set; }
        public ICommand ButtonNext { get; private set; }
        public ICommand ButtonSaveChanges { get; private set; }

        private IUnityContainer container;
        private IEventAggregator eventAggregator;

        public PlayerViewModel(IUnityContainer container, IEventAggregator eventAggregator)
        {
            if (container == null || eventAggregator == null)
                throw new ArgumentException();

            this.container = container;
            this.eventAggregator = eventAggregator;

            Playlist = this.container.Resolve<PlaylistViewModel>();
            Playlist.Playlist = new Playlist("Empty Name");

            ButtonPrevious = new DelegateCommand(this.ButtonPreviousClicked);
            ButtonPlay = new DelegateCommand(this.ButtonPlayClicked);
            ButtonPause = new DelegateCommand(this.ButtonPauseClicked);
            ButtonStop = new DelegateCommand(this.ButtonStopClicked);
            ButtonNext = new DelegateCommand(this.ButtonNextClicked);
            ButtonSaveChanges = new DelegateCommand(this.ButtonSaveChangesClicked);

            this.eventAggregator.GetEvent<AddSongsToPlaylistEvent>()
                .Subscribe(this.AddSongsToPlaylist, ThreadOption.UIThread);
            this.eventAggregator.GetEvent<RemoveSongFromPlaylistEvent>()
                .Subscribe(this.RemoveSongFromPlaylist, ThreadOption.UIThread);
        }


        // ---------- EventAggregator
        private void AddSongsToPlaylist(List<Song> songs)
        {
            foreach (var song in songs)
            {
                var viewModel = this.container.Resolve<SongViewModel>();
                viewModel.Song = song;

                Playlist.Songs.Add(viewModel);
            }
        }

        private void RemoveSongFromPlaylist(Song song)
        {
            var toRemove = Playlist.Songs.FirstOrDefault(x => x.FilePath.Equals(song.FilePath));

            Playlist.Songs.Remove(toRemove);
        }


        // ---------- Buttons
        private void ButtonPreviousClicked()
        {
            throw new NotImplementedException();
        }

        private void ButtonPlayClicked()
        {
            throw new NotImplementedException();
        }

        private void ButtonPauseClicked()
        {
            throw new NotImplementedException();
        }

        private void ButtonStopClicked()
        {
            throw new NotImplementedException();
        }

        private void ButtonNextClicked()
        {
            throw new NotImplementedException();
        }

        private void ButtonSaveChangesClicked()
        {
            throw new NotImplementedException();
        }
    }
}
