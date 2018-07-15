using CsPlayer.Shared;
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

        public ObservableCollection<SongViewModel> Songs { get; private set; }

        private Playlist playlist;

        public PlaylistViewModel(Playlist playlist)
        {
            this.playlist = playlist;

            // Wrap the references in view model ones.
            Songs = new ObservableCollection<SongViewModel>();
            Songs.AddRange(playlist.Songs.Select(x => new SongViewModel(x)));
            Songs.CollectionChanged += this.SongCollectionChanged;
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
        }
    }
}
