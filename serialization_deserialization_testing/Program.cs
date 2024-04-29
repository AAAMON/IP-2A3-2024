using dune_library;
using dune_library.Player_Resources;

namespace MyApp {
  internal class Program {
    static void Main(string[] args) {
      Game game = new Game();
      Faction faction = Faction.Atreides;
      game.Generate_Perspective(faction);
      Perspective perspective = new Perspective(faction, game);
      perspective.SerializeToJson("perspective.json");
      var perspective2 = Perspective.DeserializeFromJson("perspective.json");
    }
  }
}