using CsPlayer.PlayerEvents;
using CsPlayer.Shared;
using Microsoft.Practices.Unity;
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
        public ICommand ButtonClearFilter { get; private set; }

        private IUnityContainer container;
        private IEventAggregator eventAggregator;

        public SongCollectionViewModel(IUnityContainer container, IEventAggregator eventAggregator)
        {
            if (container == null || eventAggregator == null)
                throw new ArgumentException();

            this.container = container;
            this.eventAggregator = eventAggregator;

            ButtonClearAll = new DelegateCommand(this.ButtonClearAllClicked);
            ButtonClearInvalid = new DelegateCommand(this.ButtonClearInvalidClicked);
            ButtonCheckAll = new DelegateCommand(this.ButtonCheckAllClicked);
            ButtonAddAll = new DelegateCommand(this.ButtonAddAllClicked);
            ButtonLoad = new DelegateCommand(this.ButtonLoadClicked);
            ButtonClearFilter = new DelegateCommand(this.ButtonClearFilterClicked);

            this.eventAggregator.GetEvent<RemoveSongFromSongListEvent>()
                .Subscribe(this.RemoveSongFromSongList, ThreadOption.UIThread);
        }


        // ---------- EventAggregator
        private void RemoveSongFromSongList(Song song)
        {
            var toRemove = this.DisplayedSongs.FirstOrDefault(x => x.FilePath.Equals(song.FilePath));

            DisplayedSongs.Remove(toRemove);

            // Reset view.
            if(!DisplayedSongs.Any())
            {
                DisplayedSongs = null;
            }
        }


        // ---------- Buttons
        private void ButtonClearAllClicked()
        {
            DisplayedSongs = null;
        }

        private void ButtonClearInvalidClicked()
        {
            throw new NotImplementedException();
        }

        private void ButtonCheckAllClicked()
        {
            throw new NotImplementedException();
        }

        private void ButtonAddAllClicked()
        {
            this.eventAggregator.GetEvent<AddSongsToPlaylistEvent>()
                .Publish(this.DisplayedSongs.Select(x => x.Song).ToList());
        }

        private void ButtonLoadClicked()
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
                    .Select(x =>
                    {
                        var viewModel = this.container.Resolve<SongViewModel>();
                        viewModel.Song = x;
                        return viewModel;
                    });

                // Do not overwrite any existing / already loaded instances.
                if(songViewModels.Any() && DisplayedSongs == null)
                {
                    DisplayedSongs = new ObservableCollection<SongViewModel>();
                }

                foreach (var viewModel in songViewModels)
                {
                    DisplayedSongs.Add(viewModel);
                }
            }
        }

        private void ButtonClearFilterClicked()
        {
            throw new NotImplementedException();
        }
    }
}
