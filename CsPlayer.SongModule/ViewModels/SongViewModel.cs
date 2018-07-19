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
            get { return Song.Name; }
        }

        public string FilePath
        {
            get { return Song.FilePath; }
        }

        public bool Valid
        {
            get { return Song.Valid; }
            private set
            {
                Song.Valid = value;
                this.RaisePropertyChanged(nameof(Valid));
            }
        }

        public int Index { get; set; }

        internal Song Song { get; set; }

        public ICommand ButtonAdd { get; private set; }
        public ICommand ButtonCheck { get; private set; }
        public ICommand ButtonDelete { get; private set; }

        private IEventAggregator eventAggregator;

        public SongViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            ButtonAdd = new DelegateCommand(this.ButtonAddClicked);
            ButtonCheck = new DelegateCommand(this.ButtonCheckClicked);
            ButtonDelete = new DelegateCommand(this.ButtonDeleteClicked);
        }

        public void Verify()
        {
            Song.Verify();
            this.RaisePropertyChanged(nameof(Valid));
        }


        // ---------- Buttons
        public void ButtonAddClicked()
        {
            this.eventAggregator.GetEvent<AddSongsToPlaylistEvent>()
                .Publish(new List<Song>() { Song });
        }

        public void ButtonCheckClicked()
        {
            Song.Verify();
            Valid = Song.Valid;
        }

        public void ButtonDeleteClicked()
        {
            this.eventAggregator.GetEvent<RemoveSongFromSongListEvent>()
                .Publish(Index);
        }
    }
}
