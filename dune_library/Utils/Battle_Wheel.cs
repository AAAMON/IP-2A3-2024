using dune_library.Player_Resources;
using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using dune_library.Decks.Treachery;

namespace dune_library.Utils
{
    public class Battle_Wheel
    {
        public Battle_Wheel()
        {
            _last_player = None;
            Offensive_Treachery_Card = None;
            Defensive_Treachery_Card = None;
            Special_Treachery_Card = None;
        }
        public Battle_Wheel(Player Player)
        {
            _last_player = Player;
        }

        [JsonConstructor]
        public Battle_Wheel(Option<Player> _last_player)
        {
            this._last_player = _last_player;
        }

        [JsonInclude]
        private Option<Player> _last_player;
        [JsonIgnore]
        public Player Last_Player
        {
            get => _last_player.OrElseThrow(new InvalidOperationException("no player has used this battle wheel yet"));
            set => _last_player = value;
        }

        public Option<General> General { get; set; }

        public uint number;

        public Option<Treachery_Cards.Treachery_Card> Offensive_Treachery_Card { get; set; }

        public Option<Treachery_Cards.Treachery_Card> Defensive_Treachery_Card { get; set; }

        public Option<Treachery_Cards.Treachery_Card> Special_Treachery_Card { get; set; }

        public void Empty_Battle_Wheel()
        {
            _last_player = None;
            Offensive_Treachery_Card = None;
            Defensive_Treachery_Card = None;
            Special_Treachery_Card = None;
            General = None;
            number = 0;
    }

    }
}

