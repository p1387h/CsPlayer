using CsPlayer.PlayerEvents;
using CsPlayer.Shared;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
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

        private IEventAggregator eventAggregator;

        public SongCollectionViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            ButtonClearAll = new DelegateCommand(this.ButtonClearAllClicked);
            ButtonClearInvalid = new DelegateCommand(this.ButtonClearInvalidClicked);
            ButtonCheckAll = new DelegateCommand(this.ButtonCheckAllClicked);
            ButtonAddAll = new DelegateCommand(this.ButtonAddAllClicked);
            ButtonLoad = new DelegateCommand(this.ButtonLoadClicked);

            this.eventAggregator.GetEvent<RemoveSongFromSongListEvent>()
                .Subscribe(this.RemoveSongFromSongList, ThreadOption.UIThread);
        }


        // ---------- EventAggregator
        private void RemoveSongFromSongList(Song song)
        {
            var toRemove = this.DisplayedSongs.FirstOrDefault(x => x.FilePath.Equals(song.FilePath));

            DisplayedSongs.Remove(toRemove);
        }


        // ---------- Buttons
        public void ButtonClearAllClicked()
        {
            DisplayedSongs = null;
        }

        public void ButtonClearInvalidClicked()
        {
            throw new NotImplementedException();
        }

        public void ButtonCheckAllClicked()
        {
            throw new NotImplementedException();
        }

        public void ButtonAddAllClicked()
        {
            this.eventAggregator.GetEvent<AddSongsToPlaylistEvent>()
                .Publish(this.DisplayedSongs.Select(x => x.song).ToList());
        }

        public void ButtonLoadClicked()
        {
            var fileDialog = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = "MP3 files |*.mp3"
            };

            if (fileDialog.ShowDialog() ?? false)
            {
                var files = fileDialog.FileNames;
                var songViewModels = files
                    .Select(x => new Song(x))
                    .Select(x => new SongViewModel(x, this.eventAggregator));
                DisplayedSongs = new ObservableCollection<SongViewModel>();

                foreach (var viewModel in songViewModels)
                {
                    DisplayedSongs.Add(viewModel);
                }
            }
        }
    }
}
