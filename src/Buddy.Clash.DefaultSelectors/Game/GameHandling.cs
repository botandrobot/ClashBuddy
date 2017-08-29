using Buddy.Clash.Engine.NativeObjects.Native;
using Buddy.Clash.DefaultSelectors.Player;
using System;
using System.Collections.Generic;
using System.Text;
using Buddy.Clash.DefaultSelectors.Enemy;
using Buddy.Clash.DefaultSelectors.Game;
using Serilog;
using Buddy.Common;
using Buddy.Clash.DefaultSelectors.Utilities;

namespace Buddy.Clash.DefaultSelectors.Game
{
    class GameHandling
    {
        private static readonly ILogger Logger = LogProvider.CreateLogger<GameHandling>();
        private static PlayerCastPositionHandling playerCastPositionHandling = new PlayerCastPositionHandling();
        private static CharacterHandling characterHandling = new CharacterHandling();
        private static PlayerCardHandling cardHandling = new PlayerCardHandling();

        public FightState FightState { get; set; }

        public GameHandling()
        {
        }

        public void IniGame(FightStyle fightStyle = FightStyle.Balanced)
        {
                //Logger.Debug("Set game beginning = true");
                GameStateHandling.GameBeginning = true;
                PlayerProperties.FightStyle = fightStyle;

                PlayerCharacterHandling.Reset();
                EnemyCharacterHandling.Reset();
                EnemyCharacterPositionHandling.Reset();


                //EnemyHandling.CreateEnemies();
        }

        public void IniRound()
        {
                //EnemyHandling.BuildEnemiesNextCardsAndHand();
                FightState = GameStateHandling.CurrentFightState;
        }

        public Vector2f GetSpellPosition()
        {
            return playerCastPositionHandling.GetNextSpellPosition(FightState);
        }
    }
}
