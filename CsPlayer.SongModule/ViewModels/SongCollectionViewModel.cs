using CsPlayer.PlayerEvents;
using CsPlayer.Shared;
using MahApps.Metro.Controls.Dialogs;
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
using System.Windows.Threading;

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

                if (_songFilter.Any())
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
        private IDialogCoordinator dialogCoordinator;

        public SongCollectionViewModel(IUnityContainer container, IEventAggregator eventAggregator,
            IDialogCoordinator dialogCoordinator)
        {
            if (container == null || eventAggregator == null || dialogCoordinator == null)
                throw new ArgumentException();

            this.container = container;
            this.eventAggregator = eventAggregator;
            this.dialogCoordinator = dialogCoordinator;

            ButtonClearAll = new DelegateCommand(this.ButtonClearAllClicked);
            ButtonClearInvalid = new DelegateCommand(this.ButtonClearInvalidClicked);
            ButtonCheckAll = new DelegateCommand(async () => { await this.ButtonCheckAllClicked(); });
            ButtonAddAll = new DelegateCommand(this.ButtonAddAllClicked);
            ButtonLoad = new DelegateCommand(async () => { await this.ButtonLoadClicked(); });

            this.eventAggregator.GetEvent<RemoveSongFromSongListEvent>()
                .Subscribe(this.RemoveSongFromSongList, ThreadOption.UIThread);
        }

        private void UpdateSongIndices()
        {
            // Only the visible ones must be updated since they can be removed.
            for (int i = 0; i < DisplayedSongs.Count; i++)
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
            if (!DisplayedSongs.Any())
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
            var newCollection = this.songs.Where(x => x.Valid);

            DisplayedSongs = new ObservableCollection<SongViewModel>(newCollection);
            this.songs = newCollection.ToList();
            this.UpdateSongIndices();
        }

        private async Task ButtonCheckAllClicked()
        {
            var message = "Checking songs...";
            var controller = await this.dialogCoordinator.ShowProgressAsync(this, "Song Check", message, false);
            controller.Maximum = this.songs.Count;
            controller.Minimum = 0;

            // A separate Task is needed in order for the progress bar content
            // to be drawn. Otherwise only a blank, grey overlay is shown.
            await Task.Run(() => {
                for (int i = 0; i < this.songs.Count; i++)
                {
                    var currentCount = i + 1;
                    this.songs[i].Song.Verify();

                    controller.SetProgress((double)(i + 1));
                    controller.SetMessage(String.Format("{0} ({1}/{2})", message, currentCount, this.songs.Count));
                }
            });

            await controller.CloseAsync();
        }

        private void ButtonAddAllClicked()
        {
            this.eventAggregator.GetEvent<AddSongsToPlaylistEvent>()
                .Publish(this.DisplayedSongs.Select(x => x.Song).ToList());
        }

        private async Task ButtonLoadClicked()
        {
            var dialogSettings = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = "MP3 files |*.mp3"
            };

            await this.LoadSongsAsync(dialogSettings);

            this.UpdateSongIndices();
        }

        private async Task<bool> LoadSongsAsync(OpenFileDialog dialogSettings)
        {
            var success = dialogSettings.ShowDialog() ?? false;

            if (success)
            {
                var message = "Loading songs...";
                var files = dialogSettings.FileNames;
                var fileCount = files.Count();
                var songViewModels = files
                    .Select(x => new Song(x))
                    .Select(x =>
                    {
                        var viewModel = this.container.Resolve<SongViewModel>();
                        viewModel.Song = x;
                        return viewModel;
                    });
                var enumerator = songViewModels.GetEnumerator();
                var viewModelCollection = DisplayedSongs;
                var uiDispatcher = Dispatcher.CurrentDispatcher;

                var controller = await this.dialogCoordinator.ShowProgressAsync(this, "Load Songs", message, false);
                controller.Maximum = fileCount;
                controller.Minimum = 0;

                // Do not overwrite any existing / already loaded instances but
                // keep the reference since it is needed for the Task's context.
                if (songViewModels.Any() && DisplayedSongs == null)
                {
                    viewModelCollection = new ObservableCollection<SongViewModel>();
                    uiDispatcher.Invoke(() => { DisplayedSongs = viewModelCollection; });
                }

                // A separate Task is needed in order for the progress bar content
                // to be drawn. Otherwise only a blank, grey overlay is shown.
                await Task.Run(() =>
                {
                    for (int i = 0; i < fileCount; i++)
                    {
                        enumerator.MoveNext();
                        var viewModel = enumerator.Current;
                        var currentCount = i + 1;

                        uiDispatcher.Invoke(() => { DisplayedSongs.Add(viewModel); });
                        this.songs.Add(viewModel);

                        controller.SetProgress(currentCount);
                        controller.SetMessage(String.Format("{0} ({1}/{2})", message, currentCount, fileCount));
                    }
                });

                await controller.CloseAsync();
            }

            return success;
        }
    }
}
