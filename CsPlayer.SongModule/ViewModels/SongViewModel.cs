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

namespace CsPlayer.SongModule.ViewModels
{
    class SongViewModel : BindableBase
    {
        public string Name
        {
            get { return this.song.Name; }
        }

        public string FilePath
        {
            get { return this.song.FilePath; }
        }

        public bool Valid
        {
            get { return this.song.Valid; }
            private set
            {
                this.song.Valid = value;
                this.RaisePropertyChanged(nameof(Valid));
            }
        }

        public ICommand ButtonAdd { get; private set; }
        public ICommand ButtonCheck { get; private set; }
        public ICommand ButtonDelete { get; private set; }

        internal Song song;
        private IEventAggregator eventAggregator;

        public SongViewModel(Song song, IEventAggregator eventAggregator)
        {
            this.song = song;
            this.eventAggregator = eventAggregator;

            ButtonAdd = new DelegateCommand(this.ButtonAddClicked);
            ButtonCheck = new DelegateCommand(this.ButtonCheckClicked);
            ButtonDelete = new DelegateCommand(this.ButtonDeleteClicked);
        }


        // ---------- Buttons
        public void ButtonAddClicked()
        {
            this.eventAggregator.GetEvent<AddSongsToPlaylistEvent>()
                .Publish(new List<Song>() { this.song });
        }

        public void ButtonCheckClicked()
        {
            throw new NotImplementedException();
        }

        public void ButtonDeleteClicked()
        {
            this.eventAggregator.GetEvent<RemoveSongFromSongListEvent>()
                .Publish(this.song);
        }
    }
}
