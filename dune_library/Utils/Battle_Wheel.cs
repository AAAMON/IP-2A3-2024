using dune_library.Player_Resources;
using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace dune_library.Utils {
  public class Battle_Wheel {
    public Battle_Wheel() {
      _last_player = None;
    }
    public Battle_Wheel(Player Player) {
      _last_player = Player;
    }

    [JsonConstructor]
    public Battle_Wheel(Option<Player> _last_player) {
      this._last_player = _last_player;
    }

    [JsonInclude]
    private Option<Player> _last_player;
    [JsonIgnore]
    public Player Last_Player {
      get => _last_player.OrElseThrow(new InvalidOperationException("no player has used this battle wheel yet"));
      set => _last_player = value;
    }

    public static int Get_From_Range_Closed(int min, int max) {
      var result = max + 1;
      // while (result < min || result > max) {
      //   ask player for a number between min and max
      //   get number from player
      // }
      result = min;
      return result;
    }
  }
}
