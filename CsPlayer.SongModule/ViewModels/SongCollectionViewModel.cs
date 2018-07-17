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
using System.Text.RegularExpressions;
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

        // Differentiate between the shown songs (= filtered ones) and 
        // the collection of all songs.
        private List<SongViewModel> songs = new List<SongViewModel>();
        private ObservableCollection<SongViewModel> _displayedSongs;
        public ObservableCollection<SongViewModel> DisplayedSongs
        {
            get { return _displayedSongs; }
            set { SetProperty<ObservableCollection<SongViewModel>>(ref _displayedSongs, value); }
        }

        private string _songFilter;
        public string SongFilter
        {
            get { return _songFilter; }
            set
            {
                SetProperty<string>(ref _songFilter, value);

                IEnumerable<SongViewModel> newCollection = this.songs;

                if(_songFilter.Any())
                {
                    // Highly ineffective, but works.
                    newCollection = this.songs.Where(x => new Regex(value, RegexOptions.IgnoreCase).IsMatch(x.Name));
                }

                DisplayedSongs = new ObservableCollection<SongViewModel>(newCollection);
                this.UpdateSongIndices();
            }
        }

        public ICommand ButtonClearAll { get; private set; }
        public ICommand ButtonClearInvalid { get; private set; }
        public ICommand ButtonCheckAll { get; private set; }
        public ICommand ButtonAddAll { get; private set; }
        public ICommand ButtonLoad { get; private set; }

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

            this.eventAggregator.GetEvent<RemoveSongFromSongListEvent>()
                .Subscribe(this.RemoveSongFromSongList, ThreadOption.UIThread);
        }

        private void UpdateSongIndices()
        {
            // Only the visible ones must be updated since they can be removed.
            for(int i = 0; i < DisplayedSongs.Count; i++)
            {
                DisplayedSongs[i].Index = i;
            }
        }


        // ---------- EventAggregator
        private void RemoveSongFromSongList(int index)
        {
            var toRemove = DisplayedSongs.ElementAt(index);

            DisplayedSongs.Remove(toRemove);
            this.songs.Remove(toRemove);
            this.UpdateSongIndices();

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
            this.songs.Clear();
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
                    this.songs.Add(viewModel);
                    this.UpdateSongIndices();
                }
            }
        }
    }
}
