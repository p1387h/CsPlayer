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

        public ICommand ButtonClearAll { get; private set; }
        public ICommand ButtonClearInvalid { get; private set; }
        public ICommand ButtonCheckAll { get; private set; }
        public ICommand ButtonAddAll { get; private set; }
        public ICommand ButtonLoad { get; private set; }

        public SongCollectionViewModel()
        {
            ButtonClearAll = new DelegateCommand(this.ButtonClearAllClicked);
            ButtonClearInvalid = new DelegateCommand(this.ButtonClearInvalidClicked);
            ButtonCheckAll = new DelegateCommand(this.ButtonCheckAllClicked);
            ButtonAddAll = new DelegateCommand(this.ButtonAddAllClicked);
            ButtonLoad = new DelegateCommand(this.ButtonLoadClicked);

            DisplayedSongs = new ObservableCollection<SongViewModel>();
        }


        // ---------- Buttons
        public void ButtonClearAllClicked()
        {
            DisplayedSongs = null;
        }

        public void ButtonClearInvalidClicked()
        {

        }

        public void ButtonCheckAllClicked()
        {

        }

        public void ButtonAddAllClicked()
        {

        }

        public void ButtonLoadClicked()
        {

        }
    }
}
