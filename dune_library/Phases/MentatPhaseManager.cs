using dune_library.Map_Resources;
using dune_library.Player_Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace dune_library.Phases
{
   public class MentatPauseManager
{
    private readonly Game _game;

    public MentatPauseManager(Game game)
    {
        _game = game;
    }

    public GameResult CheckWinConditions()
    {
        var strongholdOccupations = CalculateStrongholdOccupations();
        var individualWinner = CheckIndividualWinConditions(strongholdOccupations);
        if (individualWinner != null)
        {
            return new GameResult { GameContinues = false, Winner = individualWinner };
        }

        var allianceWinner = CheckAllianceWinConditions(strongholdOccupations);
        if (allianceWinner != null)
        {
            return new GameResult { GameContinues = false, Winner = allianceWinner };
        }

        return new GameResult { GameContinues = true };
    }

        private string CheckIndividualWinConditions(Dictionary<Faction, List<Strongholds>> strongholdOccupations)
        {
            foreach (var entry in strongholdOccupations)
            {
                if (entry.Value.Count >= 3)
                {
                    return entry.Key.ToString(); //Faction.ToString() gives a meaningful name
                }
            }
            return null;
        }

        private string CheckAllianceWinConditions(Dictionary<Faction, List<Strongholds>> strongholdOccupations)
        {
            var allianceWinners = new List<string>();

            foreach (var faction in strongholdOccupations.Keys)
            {
                var allyOption = _game.Alliances.Ally_Of(faction);
                allyOption.IfSome(ally =>
                {
                    if (faction != ally && strongholdOccupations.ContainsKey(ally))
                    {
                        var combinedStrongholds = new HashSet<Strongholds>(strongholdOccupations[faction]);
                        combinedStrongholds.UnionWith(strongholdOccupations[ally]);

                        if (combinedStrongholds.Count >= 4)
                        {
                            allianceWinners.Add($"{faction} and {ally}");
                        }
                    }
                });
            }

if (allianceWinners.Any())
{
    return string.Join(", ", allianceWinners);
}

return null;        }


        private Dictionary<Faction, List<Strongholds>> CalculateStrongholdOccupations(){
            var result = new Dictionary<Faction, List<Strongholds>>();
            // This needs to be implemented based on how strongholds and their occupations are tracked
            return result;
        }
}

public class GameResult
{
    public bool GameContinues { get; set; }
    public string Winner { get; set; }
}

}
