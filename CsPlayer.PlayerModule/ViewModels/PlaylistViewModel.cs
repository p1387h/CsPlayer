using CsPlayer.PlayerEvents;
using CsPlayer.Shared;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace CsPlayer.PlayerModule.ViewModels
{
    class PlaylistViewModel : BindableBase
    {
        public string Name
        {
            get { return Playlist.Name; }
            private set
            {
                Playlist.Name = value;
                this.RaisePropertyChanged(nameof(Name));
            }
        }

        public bool Valid
        {
            get { return Playlist.Valid; }
            private set
            {
                Playlist.Valid = value;
                this.RaisePropertyChanged(nameof(Valid));
            }
        }

        private TimeSpan _totalTime = new TimeSpan();
        public TimeSpan TotalTime
        {
            get { return _totalTime; }
            set { SetProperty<TimeSpan>(ref _totalTime, value); }
        }

        private Playlist _playlist;
        internal Playlist Playlist
        {
            get { return _playlist; }
            private set { _playlist = value; }
        }

        private int _activeSongSongNumber = 0;
        public int ActiveSongSongNumber
        {
            get { return _activeSongSongNumber; }
            set
            {
                SetProperty<int>(ref _activeSongSongNumber, value);
            }
        }

        private SongViewModel _activeSong;
        public SongViewModel ActiveSong
        {
            get { return _activeSong; }
            set
            {
                SetProperty<SongViewModel>(ref _activeSong, value);
                ActiveSongSongNumber = value?.SongNumber ?? 0;
            }
        }

        private SongViewModel _selectedSong;
        public SongViewModel SelectedSong
        {
            get { return _selectedSong; }
            set { SetProperty<SongViewModel>(ref _selectedSong, value); }
        }

        public int SongCount
        {
            get { return Songs.Count; }
        }

        public ObservableCollection<SongViewModel> Songs { get; private set; }

        private IUnityContainer container;
        private IEventAggregator eventAggregator;

        public PlaylistViewModel(IUnityContainer container, IEventAggregator eventAggregator)
        {
            if (container == null || eventAggregator == null)
                throw new ArgumentException();

            this.container = container;
            this.eventAggregator = eventAggregator;

            this.SetPlaylist(new Playlist(""));

            this.eventAggregator.GetEvent<MoveSongInPlaylistEvent>()
                .Subscribe(this.MoveSongInPlaylist, ThreadOption.UIThread);
        }

        internal void SetPlaylist(Playlist playlist)
        {
            Songs = new ObservableCollection<SongViewModel>();
            Playlist = playlist;
            var viewModels = playlist.Songs.Select(x =>
                {
                    var viewModel = this.container.Resolve<SongViewModel>();
                    viewModel.SetSong(x);
                    return viewModel;
                });

            foreach (var viewModel in viewModels)
            {
                Dispatcher.CurrentDispatcher.Invoke(() => { Songs.Add(viewModel); });
            }

            // Subscribe after adding the wrappers to prevent inserting a second instance
            // of the song into the model playlist.
            Songs.CollectionChanged += this.SongCollectionChanged;
            Dispatcher.CurrentDispatcher.Invoke(() => { Name = playlist.Name; });

            this.UpdatePlaylistTime();
            this.UpdatePlaylistSongNumbers();
        }

        internal async Task SetPlaylistAsync(Playlist playlist)
        {
            await Task.Run(() => { SetPlaylist(playlist); });
        }

        private void SongCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Playlist.Songs.AddRange(e.NewItems.OfType<SongViewModel>().Select(x => x.Song));
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    var removedSongNumbers = e.OldItems
                        .OfType<SongViewModel>()
                        .Select(x => x.SongNumber)
                        .ToList();
                    removedSongNumbers.Sort();
                    removedSongNumbers.Reverse();

                    // Reverse removal since the indices change.
                    foreach (var number in removedSongNumbers)
                    {
                        Playlist.Songs.RemoveAt(number - 1);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Playlist.Songs.Clear();
                    break;
            }

            this.UpdatePlaylistTime();
            this.UpdatePlaylistSongNumbers();
            this.RaisePropertyChanged(nameof(SongCount));
        }

        private void UpdatePlaylistTime()
        {
            if (Songs.Any())
            {
                TotalTime = Songs
                    .Where(x => x.Valid)
                    .Select(x => x.TotalTime)
                    .Aggregate((total, x) => total.Add(x));
            }
            else
            {
                TotalTime = new TimeSpan();
            }
        }

        private void UpdatePlaylistSongNumbers()
        {
            for (int i = 0; i < SongCount; i++)
            {
                Songs[i].SongNumber = i + 1;
            }
        }

        /// <summary>
        /// Function for moving the <see cref="ActiveSong"/> reference to the next 
        /// valid playable instance. This function must be called before starting 
        /// any audio player istances due to the <see cref="ActiveSong"/> returning 
        /// null otherwise.
        /// </summary>
        public void MoveToNextSong()
        {
            var tempRef = ActiveSong;
            ActiveSong = null;

            if (Songs.Any(x => x.Valid))
            {
                var possibleSongs = Songs.Where(x => x.Valid).ToList();

                // First time initialization.
                if (tempRef == null && SelectedSong != null)
                {
                    ActiveSong = SelectedSong;
                }
                else if (tempRef == null)
                {
                    ActiveSong = possibleSongs.First();
                }
                // Move through the collection of songs.
                else
                {
                    var nextIndex = possibleSongs.IndexOf(tempRef) + 1;
                    var nextSong = possibleSongs.ElementAt(nextIndex % possibleSongs.Count);

                    ActiveSong = nextSong;
                }
            }
        }

        /// <summary>
        /// Function for moving the <see cref="ActiveSong"/> reference to a previous 
        /// valid playable instance.
        /// </summary>
        public void MovePreviousSong()
        {
            var tempRef = ActiveSong;
            ActiveSong = null;

            if (tempRef != null && Songs.Any(x => x.Valid))
            {
                var possibleSongs = Songs.Where(x => x.Valid).ToList();
                var prevIndex = possibleSongs.IndexOf(tempRef) - 1;

                if (prevIndex < 0)
                {
                    prevIndex = possibleSongs.Count - 1;
                }

                ActiveSong = possibleSongs.ElementAt(prevIndex);
            }
        }


        // ---------- EventAggregator
        private void MoveSongInPlaylist(Tuple<SongMovementDirection, int> movement)
        {
            var direction = movement.Item1;
            var currentIndex = movement.Item2 - 1;
            var targetIndex = -1;

            // Differentiate between the (two) possibilities and prevent out of
            // bound exceptions.
            switch (direction)
            {
                case SongMovementDirection.DOWN:
                    targetIndex = (currentIndex + 1) % Songs.Count;
                    break;
                case SongMovementDirection.UP:
                    targetIndex = currentIndex - 1;

                    if (targetIndex < 0)
                    {
                        targetIndex = Songs.Count - 1;
                    }
                    break;
                default:
                    throw new ArgumentException();
            }

            Songs.Move(currentIndex, targetIndex);
        }
    }
}
