﻿using CsPlayer.Shared;
using CsPlayer.SongModule.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsPlayer.SongModule.Design
{
    class DesignSongCollectionViewModel
    {
        public List<SongViewModel> DisplayedSongs { get; private set; }

        public DesignSongCollectionViewModel()
        {
            DisplayedSongs = new List<SongViewModel>()
            {
                new SongViewModel(new Song(@"C:\User\Desktop\TestSongOne.mp3")),
                new SongViewModel(new Song(@"C:\User\Desktop\Files\Music\Songs\TestSongs\TestSongTwo.mp3")),
                new SongViewModel(new Song(@"C:\User\Desktop\InvalidSongs\TestSongOne.mp3", false)),
                new SongViewModel(new Song(@"C:\User\Desktop\Files\Music\Songs\TestSongs\InvalidSongs\TestSongTwo.mp3", false)),
            };
        }
    }
}
