using CsPlayer.PlayerEvents;
using CsPlayer.Shared;
using NAudio.Wave;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
                    if (_song != null && _song.Valid)
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

        private IEventAggregator eventAggregator;
        private ILoggerFacade logger;

        public SongViewModel(IEventAggregator eventAggregator, ILoggerFacade logger)
        {
            this.eventAggregator = eventAggregator;
            this.logger = logger;

            ButtonUp = new DelegateCommand(this.ButtonUpClicked);
            ButtonDown = new DelegateCommand(this.ButtonDownClicked);
            ButtonDelete = new DelegateCommand(this.ButtonDeleteClicked);
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
