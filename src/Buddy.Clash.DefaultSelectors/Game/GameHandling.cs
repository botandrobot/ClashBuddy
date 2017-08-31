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
using Buddy.Clash.DefaultSelectors.Logic;
using Buddy.Clash.DefaultSelectors.Settings;

namespace Buddy.Clash.DefaultSelectors.Game
{
    class GameHandling
    {
        private static readonly ILogger Logger = LogProvider.CreateLogger<GameHandling>();
        private static CastPositionHandling playerCastPositionHandling = new CastPositionHandling();
        private static CharacterHandling characterHandling = new CharacterHandling();
        private static PlayerCardClassifying cardHandling = new PlayerCardClassifying();
        public static ApolloSettings Settings;

        public FightState FightState { get; set; }

        public GameHandling()
        {
        }

        public void IniGame(ApolloSettings settings, FightStyle fightStyle = FightStyle.Balanced)
        {
            //Logger.Debug("Set game beginning = true");
            GameStateHandling.GameBeginning = true;
            Settings = settings;

            PlayerCharacterHandling.Reset();
            EnemyCharacterPositionHandling.SetPositions();

            Logger.Debug("IniGame");
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
