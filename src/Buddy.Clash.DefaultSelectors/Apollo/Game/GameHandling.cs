using Buddy.Clash.DefaultSelectors.Player;
using Buddy.Clash.DefaultSelectors.Enemy;
using Buddy.Clash.DefaultSelectors.Card;
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
        private static CharacterHandling characterHandling = new CharacterHandling();
        private static CardClassifying cardHandling = new CardClassifying();
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
    }
}
