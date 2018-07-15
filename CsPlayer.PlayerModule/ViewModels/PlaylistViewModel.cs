using CsPlayer.Shared;
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
            get { return this.playlist.Name; }
            private set
            {
                this.playlist.Name = value;
                this.RaisePropertyChanged(nameof(Name));
            }
        }

        public bool Valid
        {
            get { return this.playlist.Valid; }
            private set
            {
                this.playlist.Valid = value;
                this.RaisePropertyChanged(nameof(Valid));
            }
        }

        private TimeSpan _totalTime = new TimeSpan();
        public TimeSpan TotalTime
        {
            get { return _totalTime; }
            set { SetProperty<TimeSpan>(ref _totalTime, value); }
        }

        public int SongCount
        {
            get { return Songs.Count; }
        }

        public ObservableCollection<SongViewModel> Songs { get; private set; }

        private Playlist playlist;
        private IEventAggregator eventAggregator;
        private ILoggerFacade logger;

        public PlaylistViewModel(Playlist playlist, IEventAggregator eventAggregator, ILoggerFacade logger)
        {
            this.playlist = playlist;
            this.eventAggregator = eventAggregator;
            this.logger = logger;

            // Wrap the references in view model ones.
            Songs = new ObservableCollection<SongViewModel>();
            Songs.AddRange(playlist.Songs.Select(x => new SongViewModel(x, this.eventAggregator, this.logger)));
            Songs.CollectionChanged += this.SongCollectionChanged;

            this.UpdatePlaylistInfo();
        }

        private void SongCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.playlist.Songs.AddRange(e.NewItems.OfType<Song>());
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    var removedFilePaths = e.OldItems.OfType<Song>().Select(x => x.FilePath);

                    this.playlist.Songs.RemoveAll(x => removedFilePaths.Contains(x.FilePath));
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.playlist.Songs.Clear();
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
