using CsPlayer.Shared;
using Prism.Commands;
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
        private PlaylistViewModel _playlist = new PlaylistViewModel(new Playlist("Empty Name"));
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

        public PlayerViewModel()
        {
            ButtonPrevious = new DelegateCommand(this.ButtonPreviousClicked);
            ButtonPlay = new DelegateCommand(this.ButtonPlayClicked);
            ButtonPause = new DelegateCommand(this.ButtonPauseClicked);
            ButtonStop = new DelegateCommand(this.ButtonStopClicked);
            ButtonNext = new DelegateCommand(this.ButtonNextClicked);
            ButtonSaveChanges = new DelegateCommand(this.ButtonSaveChangesClicked);
        }


        // ---------- Buttons
        private void ButtonPreviousClicked()
        {

        }

        private void ButtonPlayClicked()
        {

        }

        private void ButtonPauseClicked()
        {

        }

        private void ButtonStopClicked()
        {

        }

        private void ButtonNextClicked()
        {

        }

        private void ButtonSaveChangesClicked()
        {

        }
    }
}
