using dune_library.Decks.Treachery;
using dune_library.Map_Resources;
using dune_library.Map_Resoures;
using dune_library.Player_Resources;
using dune_library.Player_Resources.Knowledge_Manager_Interfaces;
using dune_library.Utils;
using EnumsNET;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.UnsafeValueAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Phases
{
    internal class Battle_Phase : Phase
    {
        public override string name => "Battle";

        public override string moment { get; protected set; } = "";

        public I_Input_Provider Input_Provider { get; set; }

        private I_Setup_Initializers_And_Getters Init { get; }

        private IReadOnlySet<Faction> Factions_In_Play => Init.Factions_Distribution.Factions_In_Play;

        private I_Treachery_Cards_Manager Treachery_Cards_Manager => Init.Knowledge_Manager;

        private Forces Reserves => Init.Reserves;

        public Map_Resources.Map Map { get; }

        public uint Storm_Position => Map.Storm_Sector;

        private Tleilaxu_Tanks Tleilaxu_Tanks { get; }

        private I_Perspective_Generator Perspective_Generator { get; }

        private IReadOnlySet<Player> Players { get; }

        public (Battle_Wheel first, Battle_Wheel second) Battle_Wheels { get; set; }

        public bool[] Factions_To_Move { get; }

        public Faction_Battles Faction_Battles { get; set; }

        private bool Las_Gun_Explosion = false;

        public Battle_Phase(Game game)
        {
            Input_Provider = game.Input_Provider;
            Init = game;
            Map = game.Map;
            Tleilaxu_Tanks = game.Tleilaxu_Tanks;
            Perspective_Generator = game;
            Players = game.Players;
            Battle_Wheels = game.Battle_Wheels;
            Factions_To_Move = game.Factions_To_Move;
            Faction_Battles = game.Faction_Battles;
        }

        public List<Faction> GetFactionOrder()
        {

            // Convert positions to a list of factions sorted by their positions
            List<Faction> sortedFactions = new List<Faction>();
            Factions_In_Play.ForEach(faction => sortedFactions.Add(faction));

            // Find the starting point based on the given number
            int startIndex = 0;
            for (int i = 0; i < sortedFactions.Count; i++)
            {
                if (Init.Player_Markers.Marker_Of(sortedFactions[i]) > Storm_Position)
                {
                    startIndex = i;
                    break;
                }
            }

            // Generate the ordered list starting from the determined position
            List<Faction> result = new List<Faction>();
            for (int i = startIndex; i < sortedFactions.Count; i++)
            {
                result.Add(sortedFactions[i]);
            }
            for (int i = 0; i < startIndex; i++)
            {
                result.Add(sortedFactions[i]);
            }

            return result;
        }
        private void next_faction(Faction faction)
        {
            switch (faction)
            {
                case Faction.Atreides:
                    Factions_To_Move[0] = false;
                    Factions_To_Move[1] = true;
                    break;
                case Faction.Bene_Gesserit:
                    Factions_To_Move[1] = false;
                    Factions_To_Move[2] = true;
                    break;
                case Faction.Emperor:
                    Factions_To_Move[2] = false;
                    Factions_To_Move[3] = true;
                    break;
                case Faction.Fremen:
                    Factions_To_Move[3] = false;
                    Factions_To_Move[4] = true;
                    break;
                case Faction.Spacing_Guild:
                    Factions_To_Move[4] = false;
                    Factions_To_Move[5] = true;
                    break;
                case Faction.Harkonnen:
                    Factions_To_Move[5] = false;
                    Factions_To_Move[0] = true;
                    break;
            }
        }
        public override void Play_Out()
        {
            moment = "battle initialization";
            
            IList<Faction> faction_order = new List<Faction>(GetFactionOrder());

            Battle_Wheels.first.Empty_Battle_Wheel();
            Battle_Wheels.second.Empty_Battle_Wheel();

            ///player1/phase_7_input/section_id/player_id/number/general_name/treachery_card/treachery_card
            moment = "choosing battle";

            faction_order.ForEach(faction =>
            {

                for (int i = 0; i < Factions_To_Move.Length; i++) { Factions_To_Move[i] = false; }
                switch (faction)
                {
                    case Faction.Atreides:
                        Factions_To_Move[0] = true;
                        break;
                    case Faction.Bene_Gesserit:
                        Factions_To_Move[1] = true;
                        break;
                    case Faction.Emperor:
                        Factions_To_Move[2] = true;
                        break;
                    case Faction.Fremen:
                        Factions_To_Move[3] = true;
                        break;
                    case Faction.Spacing_Guild:
                        Factions_To_Move[4] = true;
                        break;
                    case Faction.Harkonnen:
                        Factions_To_Move[5] = true;
                        break;
                }

                Faction_Battles.faction = faction;

                Map.Sections.ForEach(section =>
                {
                    if (section.Forces.Number_Of_Factions_Present > 1 && section.Forces.Of(faction) > 0 && section.Id != 85)
                    {
                        Faction_Battles.Battle_Sections.Add(section.Id);
                    }
                });

                Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));

                while (Faction_Battles.Battle_Sections.Count > 0)
                {
                    
                    Console.WriteLine("/player1/phase_7_input/section_id/player_id");

                    string[] line = Input_Provider.GetInputAsync().Result.Split("/");

                    if (line[1] == Init.Factions_Distribution.Player_Of(faction).Id && line[2] == "phase_7_input"
                        && Is_Valid_Player(line[4]) && Is_Valid_Section(line[3]))
                    {
                        int section_id = Int32.Parse(line[3]);
                        Faction enemy = Faction.Atreides;
                        Factions_In_Play.ForEach(faction =>
                        {
                            if (Init.Factions_Distribution.Player_Of(faction).Id == line[4])
                            {
                                enemy = faction;
                            }
                        });

                        if (Map.Sections[section_id].Forces.Of(enemy) > 0
                            && Faction_Battles.Battle_Sections.Contains(Map.Sections[section_id].Id)
                            && enemy != (Faction)Faction_Battles.faction)
                        {
                            Map.Sections[section_id].Origin_Territory.Sections.ForEach(section =>
                            {
                                Faction_Battles.Battle_Sections.Remove((uint)section_id);
                            });

                            Faction_Battles.enemy = enemy;
                            Faction_Battles.Chosen_Battle_Section = Map.Sections[section_id].Id;
                            Console.WriteLine("Battle Wheel Time");
                            Handle_Battle_Wheel();
                        }
                        else
                        {
                            Console.WriteLine("Failure");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Failure");
                    }
                }
            });

            moment = "End of battle";
            Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));

        }
        public void Handle_Battle_Wheel()
        {
            moment = "Battle Wheel";

            IList<Faction> Factions_In_Battle = [(Faction)Faction_Battles.faction, (Faction)Faction_Battles.enemy];

            Battle_Wheels.first.Last_Player = Init.Factions_Distribution.Player_Of((Faction)Faction_Battles.faction);
            Battle_Wheels.second.Last_Player = Init.Factions_Distribution.Player_Of((Faction)Faction_Battles.enemy);

            Factions_In_Battle.ForEach(f =>
            {
                switch (f)
                {
                    case Faction.Atreides:
                        Factions_To_Move[0] = true;
                        break;
                    case Faction.Bene_Gesserit:
                        Factions_To_Move[1] = true;
                        break;
                    case Faction.Emperor:
                        Factions_To_Move[2] = true;
                        break;
                    case Faction.Fremen:
                        Factions_To_Move[3] = true;
                        break;
                    case Faction.Spacing_Guild:
                        Factions_To_Move[4] = true;
                        break;
                    case Faction.Harkonnen:
                        Factions_To_Move[5] = true;
                        break;
                }
            });

            Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));

            while (Factions_In_Battle.Count > 0)
            {

                Console.WriteLine("ex: /player_id/phase_7_input/number/general_name/treachery_card/treachery_card");
                string[] line = Input_Provider.GetInputAsync().Result.Split("/");
                if (Is_Valid_Player(line[1]) && line[2] == "phase_7_input" && line.Length <= 7 && line.Length >= 5)
                {
                    Factions_In_Play.ForEach(faction =>
                    {
                        bool correct = false;
                        if (Init.Factions_Distribution.Player_Of(faction).Id == line[1] && Factions_In_Battle.Contains(faction))
                        {
                            Factions_In_Battle.Remove(faction);
                            bool aggresor = false;
                            if (faction == (Faction)Faction_Battles.faction)
                            {
                                aggresor = true;
                            }
                            if (Handle_General(line[4], faction, aggresor) && Handle_Number(line[3], faction, aggresor))
                            {
                                if (line.Length == 5)
                                {
                                    correct = true;
                                }
                                else if (line.Length == 6)
                                {
                                    if (Handle_One_Card(faction, line[5], aggresor))
                                    {
                                        correct = true;
                                    }
                                }
                                else if (line.Length == 7)
                                {
                                    if (Handle_Two_Cards(faction, line[5], line[6], aggresor))
                                    {
                                        correct = true;
                                    }
                                }
                            }
                        }
                        if (correct)
                        {
                            switch (faction)
                            {
                                case Faction.Atreides:
                                    Factions_To_Move[0] = false;
                                    break;
                                case Faction.Bene_Gesserit:
                                    Factions_To_Move[1] = false;
                                    break;
                                case Faction.Emperor:
                                    Factions_To_Move[2] = false;
                                    break;
                                case Faction.Fremen:
                                    Factions_To_Move[3] = false;
                                    break;
                                case Faction.Spacing_Guild:
                                    Factions_To_Move[4] = false;
                                    break;
                                case Faction.Harkonnen:
                                    Factions_To_Move[5] = false;
                                    break;
                            }
                            Handle_Battle_Result();
                            Factions_In_Battle.Remove(faction);
                        }
                        else
                        {
                            Console.WriteLine("Failure");
                        }

                    });
                }
                else
                {
                    Console.WriteLine("Failure");
                }

            }

        }

        public bool Is_Valid_Player(string input)
        {
            bool result = false;
            Factions_In_Play.ForEach(faction =>
            {
                if (Init.Factions_Distribution.Player_Of(faction).Id == input)
                    result = true;
            });
            return result;
        }

        public bool Is_Valid_Section(string input)
        {
            int section_id;
            if (Int32.TryParse(input, out section_id))
            {
                if (section_id >= 0 && section_id <= 85)
                {
                    return true;
                }
            }
            return false;
        }
        public void Handle_Battle_Result()
        {
            moment = "Battle in progress";
            Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));

            Handle_Treachery_Cards_Battle();
            Faction aggresor = Init.Factions_Distribution.Faction_Of(Battle_Wheels.first.Last_Player);
            Faction victim = Init.Factions_Distribution.Faction_Of(Battle_Wheels.second.Last_Player);
            uint number_aggresor = Battle_Wheels.first.number;
            uint number_victim = Battle_Wheels.second.number;

            if (Battle_Wheels.first.General.IsSome)
            {
                if (!Tleilaxu_Tanks.Non_Revivable_Generals_Of(aggresor).Contains((General)Battle_Wheels.first.General)
                    && !Tleilaxu_Tanks.Revivable_Generals_Of(aggresor).Contains((General)Battle_Wheels.first.General))
                {
                    number_aggresor += (uint)((General)Battle_Wheels.first.General).Strength;
                }

                if (Init.Alliances.Ally_Of(victim).IsSome)
                {
                    if ((Faction)Init.Alliances.Ally_Of(victim) == Faction.Harkonnen)
                    {
                        if (Init.Knowledge_Manager.Of(Faction.Harkonnen).Traitors.Contains((General)Battle_Wheels.first.General))
                        {
                            number_victim = 999;
                        }
                    }
                }
                else if (Init.Knowledge_Manager.Of(victim).Traitors.Contains((General)Battle_Wheels.first.General))
                {
                    number_victim = 999;
                }

            }

            if (Battle_Wheels.second.General.IsSome)
            {
                if (!Tleilaxu_Tanks.Non_Revivable_Generals_Of(aggresor).Contains((General)Battle_Wheels.second.General)
                    && !Tleilaxu_Tanks.Revivable_Generals_Of(aggresor).Contains((General)Battle_Wheels.second.General))
                {
                    number_victim += (uint)((General)Battle_Wheels.second.General).Strength;
                }

                if (Init.Alliances.Ally_Of(aggresor).IsSome)
                {
                    if ((Faction)Init.Alliances.Ally_Of(aggresor) == Faction.Harkonnen)
                    {
                        if (Init.Knowledge_Manager.Of(Faction.Harkonnen).Traitors.Contains((General)Battle_Wheels.first.General))
                        {
                            number_aggresor = 999;
                        }
                    }
                }
                else if (Init.Knowledge_Manager.Of(aggresor).Traitors.Contains((General)Battle_Wheels.first.General))
                {
                    number_aggresor = 999;
                }
            }

            if (number_aggresor == 999 && number_victim == 999)
            {
                Handle_Lasgun();
            }
            else if (number_aggresor >= number_victim)
            {
                if(number_aggresor != 999)
                {
                    Tleilaxu_Tanks.Forces.Transfer_From(aggresor, Map.Sections[(int)Faction_Battles.Chosen_Battle_Section].Forces, Battle_Wheels.first.number);
                }
                else
                {
                    Init.Knowledge_Manager.Add_Spice_To(aggresor, (uint)((General)Battle_Wheels.second.General).Strength);
                }

                uint all_forces = Map.Sections[(int)Faction_Battles.Chosen_Battle_Section].Forces.Of(victim);
                Tleilaxu_Tanks.Forces.Transfer_From(victim, Map.Sections[(int)Faction_Battles.Chosen_Battle_Section].Forces, all_forces);

                if (Battle_Wheels.second.Defensive_Treachery_Card.IsSome)
                {
                    Treachery_Cards_Manager.Remove_Treachery_Card(aggresor, (Treachery_Cards.Treachery_Card)Battle_Wheels.second.Defensive_Treachery_Card);
                }
                if (Battle_Wheels.second.Offensive_Treachery_Card.IsSome)
                {
                    Treachery_Cards_Manager.Remove_Treachery_Card(aggresor, (Treachery_Cards.Treachery_Card)Battle_Wheels.second.Offensive_Treachery_Card);
                }

                Handle_Treachery_Cards_Discard(aggresor);
            }
            else
            {
                if (number_victim != 999)
                {
                    Tleilaxu_Tanks.Forces.Transfer_From(victim, Map.Sections[(int)Faction_Battles.Chosen_Battle_Section].Forces, Battle_Wheels.second.number);
                }
                else
                {
                    Init.Knowledge_Manager.Add_Spice_To(victim, (uint)((General)Battle_Wheels.second.General).Strength);
                }

                uint all_forces = Map.Sections[(int)Faction_Battles.Chosen_Battle_Section].Forces.Of(aggresor);
                Tleilaxu_Tanks.Forces.Transfer_From(aggresor, Map.Sections[(int)Faction_Battles.Chosen_Battle_Section].Forces, all_forces);

                if (Battle_Wheels.first.Defensive_Treachery_Card.IsSome)
                {
                    Treachery_Cards_Manager.Remove_Treachery_Card(aggresor, (Treachery_Cards.Treachery_Card)Battle_Wheels.first.Defensive_Treachery_Card);
                }
                if (Battle_Wheels.first.Offensive_Treachery_Card.IsSome)
                {
                    Treachery_Cards_Manager.Remove_Treachery_Card(aggresor, (Treachery_Cards.Treachery_Card)Battle_Wheels.first.Offensive_Treachery_Card);
                }

                Handle_Treachery_Cards_Discard(victim);
            }
        }
        public void Handle_Treachery_Cards_Discard(Faction faction)
        {
            moment = "discard treachery cards";
            Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));

            List<Treachery_Cards.Treachery_Card> cards_to_discard = new List<Treachery_Cards.Treachery_Card>();
            if (Faction_Battles.faction == faction)
            {
                if (Battle_Wheels.first.Offensive_Treachery_Card.IsSome)
                {

                    cards_to_discard.Add((Treachery_Cards.Treachery_Card)Battle_Wheels.first.Offensive_Treachery_Card);
                }
                if (Battle_Wheels.first.Defensive_Treachery_Card.IsSome)
                {
                    cards_to_discard.Add((Treachery_Cards.Treachery_Card)Battle_Wheels.first.Defensive_Treachery_Card);
                }
            }
            else
            {
                if (Battle_Wheels.second.Offensive_Treachery_Card.IsSome)
                {
                    cards_to_discard.Add((Treachery_Cards.Treachery_Card)Battle_Wheels.second.Offensive_Treachery_Card);
                }
                if (Battle_Wheels.second.Defensive_Treachery_Card.IsSome)
                {
                    cards_to_discard.Add((Treachery_Cards.Treachery_Card)Battle_Wheels.second.Defensive_Treachery_Card);
                }
            }
            while (cards_to_discard.Any())
            {

                Console.WriteLine("ex: /player_id/phase_7_input/treachery_card");
                Console.WriteLine("ex: /player_id/phase_7_input/pass");
                string[] line = Input_Provider.GetInputAsync().Result.Split("/");
                if (line[1] == Init.Factions_Distribution.Player_Of(faction).Id && line[2] == "phase_7_input")
                {
                    if (line[3] == "pass")
                    {
                        break;
                    }
                    else
                    {

                        if (line[3] == cards_to_discard[0].GetName())
                        {
                            cards_to_discard.Remove(cards_to_discard[0]);
                            Treachery_Cards_Manager.Remove_Treachery_Card(faction, cards_to_discard[0]);

                        }
                        else if (line[3] == cards_to_discard[1].GetName())
                        {
                            cards_to_discard.Remove(cards_to_discard[1]);
                            Treachery_Cards_Manager.Remove_Treachery_Card(faction, cards_to_discard[1]);
                        }

                    }
                }
            }
            moment = "Going to the next battle";
        }
        public void Handle_Treachery_Cards_Battle()
        {
            Faction agreesor_faction = Init.Factions_Distribution.Faction_Of(Battle_Wheels.first.Last_Player);
            Faction victim_faction = Init.Factions_Distribution.Faction_Of(Battle_Wheels.second.Last_Player);

            if (Battle_Wheels.first.Offensive_Treachery_Card.IsSome)
            {
                Treachery_Cards.Treachery_Card aggresor_card = (Treachery_Cards.Treachery_Card)Battle_Wheels.first.Offensive_Treachery_Card;
                if (Battle_Wheels.first.Defensive_Treachery_Card.IsSome)
                {
                    Treachery_Cards.Treachery_Card suicide = (Treachery_Cards.Treachery_Card)Battle_Wheels.first.Defensive_Treachery_Card;
                    if (suicide == Treachery_Cards.Treachery_Card.Shield)
                    {
                        Las_Gun_Explosion = true;
                    }
                }
                if (Battle_Wheels.second.Defensive_Treachery_Card.IsSome)
                {
                    Treachery_Cards.Treachery_Card victim_card = (Treachery_Cards.Treachery_Card)Battle_Wheels.second.Defensive_Treachery_Card;

                    if ((aggresor_card.Is_Poison_Weapon() && victim_card.Is_Projectile_Defense()
                        || aggresor_card.Is_Projectile_Weapon() && victim_card.Is_Poison_Defense() || victim_card.Is_Worthless()) && !Battle_Wheels.second.Special_Treachery_Card.IsSome)
                    {
                        Tleilaxu_Tanks.Kill(((General)Battle_Wheels.second.General).Id);
                    }
                    else if (aggresor_card.Is_Lasgun() && victim_card == Treachery_Cards.Treachery_Card.Shield)
                    {
                        Las_Gun_Explosion = true;
                    }
                }
                else
                {
                    Tleilaxu_Tanks.Kill(((General)Battle_Wheels.second.General).Id);
                }
            }

            if (Battle_Wheels.second.Offensive_Treachery_Card.IsSome)
            {
                Treachery_Cards.Treachery_Card aggresor_card = (Treachery_Cards.Treachery_Card)Battle_Wheels.second.Offensive_Treachery_Card;
                if (Battle_Wheels.second.Defensive_Treachery_Card.IsSome)
                {
                    Treachery_Cards.Treachery_Card suicide = (Treachery_Cards.Treachery_Card)Battle_Wheels.second.Defensive_Treachery_Card;
                    if (suicide == Treachery_Cards.Treachery_Card.Shield)
                    {
                        Las_Gun_Explosion = true;
                    }
                }
                if (Battle_Wheels.first.Defensive_Treachery_Card.IsSome)
                {
                    Treachery_Cards.Treachery_Card victim_card = (Treachery_Cards.Treachery_Card)Battle_Wheels.first.Defensive_Treachery_Card;

                    if ((aggresor_card.Is_Poison_Weapon() && victim_card.Is_Projectile_Defense()
                        || aggresor_card.Is_Projectile_Weapon() && victim_card.Is_Poison_Defense() || victim_card.Is_Worthless()) && !Battle_Wheels.first.Special_Treachery_Card.IsSome)
                    {
                        Tleilaxu_Tanks.Kill(((General)Battle_Wheels.first.General).Id);
                    }
                    else if (aggresor_card.Is_Lasgun() && victim_card == Treachery_Cards.Treachery_Card.Shield)
                    {
                        Las_Gun_Explosion = true;
                    }
                }
                else
                {
                    Tleilaxu_Tanks.Kill(((General)Battle_Wheels.second.General).Id);
                }
            }
        }
        private void Handle_Lasgun()
        {
            Faction agreesor_faction = Init.Factions_Distribution.Faction_Of(Battle_Wheels.first.Last_Player);
            Faction victim_faction = Init.Factions_Distribution.Faction_Of(Battle_Wheels.second.Last_Player);
            Map.Sections[(int)Faction_Battles.Chosen_Battle_Section].Origin_Territory.Sections.ForEach(s =>
            {
                s.Forces.Remove_By_Storm(Tleilaxu_Tanks);
                s.Delete_Spice();
            });

            if (Battle_Wheels.first.Defensive_Treachery_Card.IsSome)
            {
                Treachery_Cards_Manager.Remove_Treachery_Card(agreesor_faction, (Treachery_Cards.Treachery_Card)Battle_Wheels.first.Defensive_Treachery_Card);
            }
            if (Battle_Wheels.first.Offensive_Treachery_Card.IsSome)
            {
                Treachery_Cards_Manager.Remove_Treachery_Card(agreesor_faction, (Treachery_Cards.Treachery_Card)Battle_Wheels.first.Offensive_Treachery_Card);
            }
            if (Battle_Wheels.second.Defensive_Treachery_Card.IsSome)
            {
                Treachery_Cards_Manager.Remove_Treachery_Card(agreesor_faction, (Treachery_Cards.Treachery_Card)Battle_Wheels.second.Defensive_Treachery_Card);
            }
            if (Battle_Wheels.second.Offensive_Treachery_Card.IsSome)
            {
                Treachery_Cards_Manager.Remove_Treachery_Card(agreesor_faction, (Treachery_Cards.Treachery_Card)Battle_Wheels.second.Offensive_Treachery_Card);
            }

            if (!Battle_Wheels.first.Special_Treachery_Card.IsSome)
            {
                Tleilaxu_Tanks.Kill(((General)Battle_Wheels.first.General).Id);
            }
            if (!Battle_Wheels.second.Special_Treachery_Card.IsSome)
            {
                Tleilaxu_Tanks.Kill(((General)Battle_Wheels.second.General).Id);
            }
            Las_Gun_Explosion = true;
        }
        public bool Handle_One_Card(Faction faction, string input, bool aggresor)
        {
            bool result = false;
            Init.Knowledge_Manager.Of(faction).Treachery_Cards.ForEach(card =>
            {
                if (card.Key.GetName() == input && card.Value > 0)
                {
                    if (card.Key.Is_Weapon())
                    {
                        if (aggresor)
                        {
                            Battle_Wheels.first.Offensive_Treachery_Card = card.Key;
                        }
                        else
                        {
                            Battle_Wheels.second.Offensive_Treachery_Card = card.Key;
                        }
                        result = true;
                    }
                    else if (card.Key.Is_Defense())
                    {
                        if (aggresor)
                        {
                            Battle_Wheels.first.Defensive_Treachery_Card = card.Key;
                        }
                        else
                        {
                            Battle_Wheels.second.Defensive_Treachery_Card = card.Key;
                        }
                        result = true;
                    }
                    else if (card.Key.Is_Worthless())
                    {
                        if (aggresor)
                        {
                            if (Battle_Wheels.first.Defensive_Treachery_Card.IsNone)
                            {
                                Battle_Wheels.first.Defensive_Treachery_Card = card.Key;
                            }
                            else if (Battle_Wheels.first.Offensive_Treachery_Card.IsNone)
                            {
                                Battle_Wheels.first.Offensive_Treachery_Card = card.Key;
                            }
                        }
                        else
                        {
                            if (Battle_Wheels.second.Defensive_Treachery_Card.IsNone)
                            {
                                Battle_Wheels.second.Defensive_Treachery_Card = card.Key;
                            }
                            else if (Battle_Wheels.second.Offensive_Treachery_Card.IsNone)
                            {
                                Battle_Wheels.second.Offensive_Treachery_Card = card.Key;
                            }
                        }
                        result = true;
                    }
                }
            });

            return result;
        }

        public bool Handle_Two_Cards(Faction faction, string input1, string input2, bool aggresor)
        {
            Treachery_Cards.Treachery_Card first_card = new Treachery_Cards.Treachery_Card();
            Treachery_Cards.Treachery_Card second_card = new Treachery_Cards.Treachery_Card();
            bool[] result = new bool[2];
            Treachery_Cards.Default_Treachery_Deck_Composition.ForEach(card =>
            {
                if (card.Key.GetName() == input1)
                {
                    first_card = card.Key;
                    result[0] = true;
                }
                else if (card.Key.GetName() == input2)
                {
                    second_card = card.Key;
                    result[1] = true;
                }
            });

            if (!result.Contains(false) &&
                (first_card.Is_Defense() && second_card.Is_Weapon() || second_card.Is_Defense() && first_card.Is_Weapon()
                 || first_card.Is_Worthless() || second_card.Is_Worthless()))
            {
                if (Handle_One_Card(faction, input1, aggresor) && Handle_One_Card(faction, input2, aggresor))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Handle_Number(string number, Faction faction, bool aggresor)
        {
            int troop_number;
            if (Int32.TryParse(number, out troop_number))
            {
                if (Map.Sections[(int)Faction_Battles.Chosen_Battle_Section].Forces.Of(faction) >= troop_number)
                {
                    if (aggresor)
                    {
                        Battle_Wheels.first.number = Map.Sections[(int)Faction_Battles.Chosen_Battle_Section].Forces.Of(faction);
                    }
                    else
                    {
                        Battle_Wheels.second.number = Map.Sections[(int)Faction_Battles.Chosen_Battle_Section].Forces.Of(faction);
                    }
                    return true;
                }
            }
            return false;
        }
        public bool Handle_General(string general_name, Faction faction, bool aggresor)
        {
            bool result = false;
            if (general_name == "Cheap_Hero")
            {
                if (Init.Knowledge_Manager.Of(faction).Treachery_Cards.ContainsKey(Treachery_Cards.Treachery_Card.Cheap_Hero))
                {
                    if (Init.Knowledge_Manager.Of(faction).Treachery_Cards[Treachery_Cards.Treachery_Card.Cheap_Hero] > 0)
                    {
                        Treachery_Cards_Manager.Remove_Treachery_Card(faction, Treachery_Cards.Treachery_Card.Cheap_Hero);
                        if (aggresor)
                        {
                            Battle_Wheels.first.Special_Treachery_Card = Treachery_Cards.Treachery_Card.Cheap_Hero;
                        }
                        else
                        {
                            Battle_Wheels.second.Special_Treachery_Card = Treachery_Cards.Treachery_Card.Cheap_Hero;
                        }
                        result = true;
                    }

                }
            }
            else
            {
                Generals_Manager.Get_Default_Generals_Of(faction).ForEach(general =>
                {
                    if (general.Name == general_name.Replace("_", " "))
                    {
                        if (!Tleilaxu_Tanks.Revivable_Generals_Of(faction).Contains(general) && !Tleilaxu_Tanks.Non_Revivable_Generals_Of(faction).Contains(general))
                        {
                            if (aggresor)
                            {
                                Battle_Wheels.first.General = general;
                            }
                            else
                            {
                                Battle_Wheels.second.General = general;
                            }
                            result = true;
                        }
                    }
                });

            }
            return result;

        }
    }
}
