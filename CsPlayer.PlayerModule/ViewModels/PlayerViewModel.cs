using CsPlayer.PlayerEvents;
using CsPlayer.Shared;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Practices.Unity;
using NAudio.Wave;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CsPlayer.PlayerModule.ViewModels
{
    class PlayerViewModel : BindableBase
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
        public ICommand ButtonSavePlaylist { get; private set; }

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
            ButtonSavePlaylist = new DelegateCommand(async () => { await this.ButtonSavePlaylistClicked(); });

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


        // ---------- EventAggregator
        private async Task AddSongsToPlaylistAsync(List<Song> songs)
        {
            var viewModelsToAdd = songs.Select(async x =>
                {
                    var viewModel = this.container.Resolve<SongViewModel>();
                    await viewModel.SetSongAsync(x);
                    return viewModel;
                });

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

        private async Task ButtonSavePlaylistClicked()
        {
            var saveSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Save",
                NegativeButtonText = "Cancel",
                DefaultButtonFocus = MessageDialogResult.Affirmative,
                AnimateHide = false
            };
            var notificationSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "OK",
                DefaultButtonFocus = MessageDialogResult.Affirmative
            };
            var name = await this.dialogCoordinator.ShowInputAsync(this, "Save Playlist", 
                "Enter the name of the playlist.", saveSettings);

            // Null means the user aborted the save.
            if (name != null)
            {
                // Empty meand the user did not enter any name.
                if (name.Equals(String.Empty))
                {
                    await this.dialogCoordinator.ShowMessageAsync(this, "Save Playlist", 
                        "The playlist requires a name!", MessageDialogStyle.Affirmative, 
                        notificationSettings);
                }
                else
                {
                    this.eventAggregator.GetEvent<SavePlaylistEvent>()
                        .Publish(this.Playlist.Playlist);
                    await this.dialogCoordinator.ShowMessageAsync(this, "Save Playlist",
                        "Saving successfull", MessageDialogStyle.Affirmative,
                        notificationSettings);
                }
            }
        }
    }
}
