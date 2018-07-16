using CsPlayer.PlayerEvents;
using CsPlayer.PlayerModule.Helper;
using CsPlayer.Shared;
using NAudio.Wave;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace CsPlayer.PlayerModule.ViewModels
{
    class SongViewModel : BindableBase
    {
        public string Name
        {
            get { return Song.Name; }
        }

        public string FilePath
        {
            get { return Song.FilePath; }
        }

        public bool Valid
        {
            get { return Song.Valid; }
            private set
            {
                Song.Valid = value;
                this.RaisePropertyChanged(nameof(Valid));
            }
        }

        private bool _isPlaying = false;
        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                SetProperty<bool>(ref _isPlaying, value);

                // Starting / stopping the timer resets the internal saved
                // remaining time.
                if (value)
                {
                    this.timer.Start();
                }
                else
                {
                    this.timer.Stop();
                }
            }
        }

        private TimeSpan _currentTime;
        public TimeSpan CurrentTime
        {
            get { return _currentTime; }
            set
            {
                SetProperty<TimeSpan>(ref _currentTime, value);

                // Only allow the user to change the current time.
                // -> prevents stuttering.
                if (!this.isTimerTickSetter)
                {
                    Mp3Reader.CurrentTime = value;
                }
            }
        }

        public ICommand ButtonUp { get; private set; }
        public ICommand ButtonDown { get; private set; }
        public ICommand ButtonDelete { get; private set; }

        public Mp3FileReader Mp3Reader { get; private set; }

        private TimeSpan _totalTime;
        public TimeSpan TotalTime
        {
            get { return _totalTime; }
            set { SetProperty<TimeSpan>(ref _totalTime, value); }
        }

        private Song _song;
        internal Song Song
        {
            get { return _song; }
            set
            {
                _song = value;

                try
                {
                    // Prevent exceptions by testing the existence of the file itself.
                    // DesignMode must be checked since the FileReader must not load
                    // design data.
                    if (_song != null && _song.Valid && !DesignModeChecker.IsInDesignMode())
                    {
                        Mp3Reader = new Mp3FileReader(_song.FilePath);
                        TotalTime = Mp3Reader.TotalTime;
                    }
                }
                catch (DirectoryNotFoundException e)
                {
                    this.logger.Log(e.Message, Category.Exception, Priority.High);
                }
                catch (FileNotFoundException e)
                {
                    this.logger.Log(e.Message, Category.Exception, Priority.High);
                }
            }
        }

        // DispatcherTimer since updating the UI regarding the current time of the
        // mp3 reader is necessary.
        private DispatcherTimer timer = new DispatcherTimer();
        // Flag for signaling the CurrentTime property that the timer updated the 
        // value. Since the user has access to it as well and is able to change 
        // the player's position in the mp3, setting the value with the timer 
        // would otherwise cause the song to stutter.
        private bool isTimerTickSetter = false;

        private IEventAggregator eventAggregator;
        private ILoggerFacade logger;

        public SongViewModel(IEventAggregator eventAggregator, ILoggerFacade logger)
        {
            this.eventAggregator = eventAggregator;
            this.logger = logger;

            ButtonUp = new DelegateCommand(this.ButtonUpClicked);
            ButtonDown = new DelegateCommand(this.ButtonDownClicked);
            ButtonDelete = new DelegateCommand(this.ButtonDeleteClicked);

            // Timer for updating the 
            this.timer.Tick += this.HandleTimerTick;
            this.timer.Interval = TimeSpan.FromSeconds(1);
        }

        private void HandleTimerTick(object sender, EventArgs e)
        {
            // Prevent stuttering.
            this.isTimerTickSetter = true;
            CurrentTime = Mp3Reader.CurrentTime;
            this.isTimerTickSetter = false;
        }


        // ---------- Buttons
        public void ButtonUpClicked()
        {
            throw new NotImplementedException();
        }

        public void ButtonDownClicked()
        {
            throw new NotImplementedException();
        }

        public void ButtonDeleteClicked()
        {
            this.eventAggregator.GetEvent<RemoveSongFromPlaylistEvent>()
                .Publish(Song);
        }
    }
}
