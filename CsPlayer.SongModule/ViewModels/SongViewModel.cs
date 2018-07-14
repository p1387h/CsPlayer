using CsPlayer.Shared;
using Prism.Commands;
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

        private bool _valid;
        public bool Valid
        {
            get { return _valid; }
            private set {
                SetProperty<bool>(ref _valid, value);
                this.song.Valid = value; }
        }

        public ICommand ButtonAdd { get; private set; }
        public ICommand ButtonCheck { get; private set; }
        public ICommand ButtonDelete { get; private set; }

        private Song song;

        public SongViewModel(Song song)
        {
            this.song = song;

            ButtonAdd = new DelegateCommand(this.ButtonAddClicked);
            ButtonCheck = new DelegateCommand(this.ButtonCheckClicked);
            ButtonDelete = new DelegateCommand(this.ButtonDeleteClicked);
        }


        // ---------- Buttons
        public void ButtonAddClicked()
        {

        }

        public void ButtonCheckClicked()
        {

        }

        public void ButtonDeleteClicked()
        {

        }
    }
}
