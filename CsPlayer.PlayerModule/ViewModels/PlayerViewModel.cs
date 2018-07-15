using CsPlayer.PlayerEvents;
using CsPlayer.Shared;
using Prism.Commands;
using Prism.Events;
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

        private IEventAggregator eventAggregator;

        public PlayerViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            Playlist = new PlaylistViewModel(new Playlist("Empty Name"), eventAggregator);

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
                Playlist.Songs.Add(new SongViewModel(song, this.eventAggregator));
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
