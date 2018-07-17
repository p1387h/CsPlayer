using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsPlayer.PlayerEvents
{
    public class MoveSongInPlaylistEvent : PubSubEvent<Tuple<SongMovementDirection, int>>
    {
        public MoveSongInPlaylistEvent()
        {

        }
    }
}
