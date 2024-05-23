﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dune_library.Phases
{
    public class MentatPausePhase : Phase
    {
        private readonly Game _game;
        private readonly MentatPauseManager _pauseManager;

        public MentatPausePhase(Game game)
        {
            _game = game;
            _pauseManager = new MentatPauseManager(game);
        }

        public override string name => "Mentat Pause";

        public override string moment { get; protected set; }

        public override void Play_Out()
        {
            var result = _pauseManager.CheckWinConditions();
            if (!result.GameContinues)
            {
                Console.WriteLine("End Game");
                //_game.EndGame();
                Console.WriteLine($"Congratulations, {result.Winner} has won the game!");
            }
            else
            {
                Console.WriteLine("waiting for players to confirm");
                /*while (!_game.AreAllPlayersReady())
                {
                    Console.WriteLine("Waiting for all players to be ready...");
                    Task.Delay(1000).Wait();
                }
                Console.WriteLine("All players are ready, proceeding to the next round.");
                _game.ProceedToNextPhase();*/
                Console.WriteLine("Go to next round");
            }
        }

    }

}