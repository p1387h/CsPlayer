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

        private ObservableCollection<string> _displayedSongPaths;
        public ObservableCollection<string> DisplayedSongPaths
        {
            get { return _displayedSongPaths; }
            set { SetProperty<ObservableCollection<string>>(ref _displayedSongPaths, value); }
        }

        public ICommand ButtonClear { get; private set; }
        public ICommand ButtonClearInvalid { get; private set; }
        public ICommand ButtonCheck { get; private set; }
        public ICommand ButtonAdd { get; private set; }

        public SongCollectionViewModel()
        {
            ButtonClear = new DelegateCommand(ButtonClearClicked);
            ButtonClearInvalid = new DelegateCommand(ButtonClearInvalidClicked);
            ButtonCheck = new DelegateCommand(ButtonCheckClicked);
            ButtonAdd = new DelegateCommand(ButtonAddClicked);
        }


        // ---------- Buttons
        public void ButtonClearClicked()
        {
            DisplayedSongPaths = null;
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
