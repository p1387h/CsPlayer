using CsPlayer.Shared;
using Microsoft.Practices.ServiceLocation;
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
                        var viewModel = ServiceLocator.Current.GetInstance<SongViewModel>();
                        viewModel.Song = x;
                        return viewModel;
                    }));
                Songs.CollectionChanged += this.SongCollectionChanged;

                this.UpdatePlaylistInfo();
            }
        }

        public int SongCount
        {
            get { return Songs.Count; }
        }

        public ObservableCollection<SongViewModel> Songs { get; private set; }

        private IEventAggregator eventAggregator;

        public PlaylistViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        private void SongCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Playlist.Songs.AddRange(e.NewItems.OfType<Song>());
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    var removedFilePaths = e.OldItems.OfType<Song>().Select(x => x.FilePath);

                    Playlist.Songs.RemoveAll(x => removedFilePaths.Contains(x.FilePath));
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Playlist.Songs.Clear();
                    break;
            }

            this.UpdatePlaylistInfo();
            this.RaisePropertyChanged(nameof(SongCount));
        }

        private void UpdatePlaylistInfo()
        {
            if (Songs.Any())
            {
                TotalTime = Songs
                    .Select(x => x.TotalTime)
                    .Aggregate((total, x) => total.Add(x));
            }
            else
            {
                TotalTime = new TimeSpan();
            }
        }
    }
}
