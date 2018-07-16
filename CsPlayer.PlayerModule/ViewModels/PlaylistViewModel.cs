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
            set
            {
                _playlist = value;

                // Wrap the references in view model ones.
                Songs = new ObservableCollection<SongViewModel>();
                Songs.AddRange(_playlist.Songs.Select(x =>
                    {
                        var viewModel = this.container.Resolve<SongViewModel>();
                        viewModel.Song = x;
                        return viewModel;
                    }));
                Songs.CollectionChanged += this.SongCollectionChanged;

                this.UpdatePlaylistTime();
            }
        }

        private SongViewModel _activeSong;
        public SongViewModel ActiveSong
        {
            get { return _activeSong; }
            set { SetProperty<SongViewModel>(ref _activeSong, value); }
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
                    var removedFilePaths = e.OldItems.OfType<SongViewModel>().Select(x => x.FilePath);

                    Playlist.Songs.RemoveAll(x => removedFilePaths.Contains(x.FilePath));
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Playlist.Songs.Clear();
                    break;
            }

            this.UpdatePlaylistTime();
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
                if(tempRef == null && SelectedSong != null)
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
    }
}
