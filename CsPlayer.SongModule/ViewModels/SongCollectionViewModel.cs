using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public SongCollectionViewModel()
        {

        }
    }
}
