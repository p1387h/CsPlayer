using CsPlayer.Shared;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CsPlayer.SongModule.ViewModels
{
    class SongCollectionViewModel : BindableBase
    {
        // Header is displayed in the tab control.
        private string _header = "Songs";
        public string Header
        {
            get { return _header; }
            private set { SetProperty<string>(ref _header, value); }
        }

        private ObservableCollection<SongViewModel> _displayedSongs;
        public ObservableCollection<SongViewModel> DisplayedSongs
        {
            get { return _displayedSongs; }
            set { SetProperty<ObservableCollection<SongViewModel>>(ref _displayedSongs, value); }
        }

        public ICommand ButtonClear { get; private set; }
        public ICommand ButtonClearInvalid { get; private set; }
        public ICommand ButtonCheck { get; private set; }
        public ICommand ButtonAdd { get; private set; }

        public SongCollectionViewModel()
        {
            ButtonClear = new DelegateCommand(this.ButtonClearClicked);
            ButtonClearInvalid = new DelegateCommand(this.ButtonClearInvalidClicked);
            ButtonCheck = new DelegateCommand(this.ButtonCheckClicked);
            ButtonAdd = new DelegateCommand(this.ButtonAddClicked);
        }


        // ---------- Buttons
        public void ButtonClearClicked()
        {
            DisplayedSongs = null;
        }

        public void ButtonClearInvalidClicked()
        {

        }

        public void ButtonCheckClicked()
        {

        }

        public void ButtonAddClicked()
        {

        }
    }
}
