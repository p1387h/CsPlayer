using CsPlayer.PlayerEvents;
using CsPlayer.Shared;
using GongSolutions.Wpf.DragDrop;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Practices.Unity;
using NAudio.Wave;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CsPlayer.PlayerModule.ViewModels
{
    class PlayerViewModel : BindableBase, IDropTarget
    {
        private PlaylistViewModel _playlist;
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
        public ICommand ButtonClearPlaylist { get; private set; }

        // The player itself.
        private WaveOut waveOut = new WaveOut();

        private IUnityContainer container;
        private IEventAggregator eventAggregator;
        private ILoggerFacade logger;
        private IDialogCoordinator dialogCoordinator;

        public PlayerViewModel(IUnityContainer container, IEventAggregator eventAggregator,
            ILoggerFacade logger, IDialogCoordinator dialogCoordinator)
        {
            if (container == null || eventAggregator == null
                || logger == null || dialogCoordinator == null)
                throw new ArgumentException();

            this.container = container;
            this.eventAggregator = eventAggregator;
            this.logger = logger;
            this.dialogCoordinator = dialogCoordinator;

            Playlist = this.container.Resolve<PlaylistViewModel>();

            ButtonPrevious = new DelegateCommand(this.ButtonPreviousClicked);
            ButtonPlay = new DelegateCommand(this.ButtonPlayClicked);
            ButtonPause = new DelegateCommand(this.ButtonPauseClicked);
            ButtonStop = new DelegateCommand(this.ButtonStopClicked);
            ButtonNext = new DelegateCommand(this.ButtonNextClicked);
            ButtonClearPlaylist = new DelegateCommand(this.ButtonClearPlaylistClicked);

            this.eventAggregator.GetEvent<AddSongsToPlaylistEvent>()
                .Subscribe(async (songs) => { await this.AddSongsToPlaylistAsync(songs); }, ThreadOption.UIThread);
            this.eventAggregator.GetEvent<RemoveSongFromPlaylistEvent>()
                .Subscribe(this.RemoveSongFromPlaylist, ThreadOption.UIThread);

            // Reset once in order to intialize all necessary handlers and references.
            this.ResetWaveOut();
        }

        private void ResetWaveOut()
        {
            this.waveOut.Stop();
            this.waveOut.PlaybackStopped -= this.HandlePlaybackStopped;
            this.waveOut.Dispose();

            if (this.Playlist.ActiveSong != null)
            {
                Playlist.ActiveSong.CurrentTime = TimeSpan.FromSeconds(0);
                Playlist.ActiveSong.Mp3Reader.CurrentTime = TimeSpan.FromSeconds(0);
                Playlist.ActiveSong.IsPlaying = false;
            }

            this.waveOut = new WaveOut();
            this.waveOut.PlaybackStopped += this.HandlePlaybackStopped;
        }

        private void HandlePlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (e.Exception != null)
            {
                this.logger.Log(e.Exception.Message, Category.Exception, Priority.High);
            }

            this.ButtonNextClicked();
        }

        private IEnumerable<Task<SongViewModel>> GenerateSongViewModels(IEnumerable<Song> songs)
        {
            return songs.Select(async (x) =>
                {
                    var viewModel = this.container.Resolve<SongViewModel>();
                    await viewModel.SetSongAsync(x);
                    return viewModel;
                });
        }


        // ---------- EventAggregator
        private async Task AddSongsToPlaylistAsync(IEnumerable<Song> songs)
        {
            var viewModelsToAdd = this.GenerateSongViewModels(songs);

            // Insert directly after the selected one if possible.
            if (Playlist.SelectedSong != null)
            {
                var vmList = viewModelsToAdd.ToList();
                vmList.Reverse();

                foreach (var vm in vmList)
                {
                    Playlist.Songs.Insert(Playlist.SelectedSong.SongNumber, await vm);
                }
            }
            else
            {
                foreach (var vm in viewModelsToAdd)
                {
                    Playlist.Songs.Add(await vm);
                }
            }
        }

        private void RemoveSongFromPlaylist(int songNumber)
        {
            Playlist.Songs.RemoveAt(songNumber - 1);
        }

        // ---------- Drag / Drop
        public void DragOver(IDropInfo dropInfo)
        {
            var songViewModel = dropInfo.Data as ISong;
            var songViewModels = dropInfo.Data as IEnumerable<ISong>;

            if(songViewModel != null || songViewModels != null)
            {
                var positionChange = (songViewModel as SongViewModel) != null
                    || (songViewModels != null && songViewModels.All(x => x is SongViewModel));

                if(positionChange)
                {
                    dropInfo.Effects = DragDropEffects.Move;
                }
                else
                {
                    dropInfo.Effects = DragDropEffects.Copy;
                }

                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            }
            else
            {
                dropInfo.Effects = DragDropEffects.None;
            }
        }

        public async void Drop(IDropInfo dropInfo)
        {
            var song = dropInfo.Data as ISong;
            var songs = dropInfo.Data as IEnumerable<ISong>;

            if (song != null || songs != null)
            {
                var observable = dropInfo.TargetCollection as ObservableCollection<SongViewModel>;
                IEnumerable<ISong> songCollection = songs;

                if(song != null)
                {
                    songCollection = new List<ISong>() { song };
                }

                // Position change inside the player itself.
                if(songCollection.All(x => x is SongViewModel))
                {
                    var viewModels = songCollection
                        .Cast<SongViewModel>()
                        .Reverse();

                    foreach (var viewModel in viewModels)
                    {
                        var indexOfMovedItem = observable.IndexOf(viewModel);
                        var targetIndex = dropInfo.InsertIndex;

                        // The target index does include the moved item as well. I.e.
                        // a element at index 0 moved after element index 1 has the 
                        // target index 2. The result of this would be that the first 
                        // element ends up at index 2 with 2 elements before it even
                        // though only one single element needed to be skipped. 
                        // Therefore the target index must be reduced by one, when it 
                        // is larger than the index of the moved element.
                        if(targetIndex > indexOfMovedItem)
                        {
                            targetIndex--;
                        }

                        observable.Move(indexOfMovedItem, targetIndex);
                    }

                    dropInfo.Effects = DragDropEffects.Move;
                }
                // New songs from the collection.
                else
                {
                    var droppedSongs = songCollection
                        .Select(x => new Song(x.FilePath) { Valid = x.Valid })
                        .Reverse();
                    var viewModels = this.GenerateSongViewModels(droppedSongs);

                    foreach (var viewModel in viewModels)
                    {
                        observable.Insert(dropInfo.InsertIndex, await viewModel);
                    }

                    dropInfo.Effects = DragDropEffects.Copy;
                }

                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            }
            else
            {
                dropInfo.Effects = DragDropEffects.None;
            }
        }


        // ---------- Buttons
        private void ButtonPreviousClicked()
        {
            this.ResetWaveOut();
            Playlist.MovePreviousSong();
            this.ButtonPlayClicked();
        }

        private void ButtonPlayClicked()
        {
            // Force the playlist to move to the next song if no other one 
            // is currently selected. Needed for initial song to be chosen.
            if (Playlist.ActiveSong == null)
            {
                Playlist.MoveToNextSong();
            }

            // Differentiate between pausing and reinitializing the player
            // in order to prevent playing the same song twice.
            if (this.waveOut.PlaybackState.Equals(PlaybackState.Paused))
            {
                this.waveOut.Resume();
            }
            else if (Playlist.ActiveSong != null && !this.waveOut.PlaybackState.Equals(PlaybackState.Playing))
            {
                this.waveOut.Init(Playlist.ActiveSong.Mp3Reader);
                this.waveOut.Play();

                Playlist.ActiveSong.IsPlaying = true;
            }
        }

        private void ButtonPauseClicked()
        {
            this.waveOut.Pause();
        }

        private void ButtonStopClicked()
        {
            this.ResetWaveOut();
            Playlist.ActiveSong = null;
        }

        private void ButtonNextClicked()
        {
            this.ResetWaveOut();
            Playlist.MoveToNextSong();
            this.ButtonPlayClicked();
        }

        private void ButtonClearPlaylistClicked()
        {
            ButtonStopClicked();
            Playlist.Songs.Clear();
        }
    }
}
