using Robi.Clash.DefaultSelectors.Player;
using Robi.Clash.DefaultSelectors.Enemy;
using Robi.Clash.DefaultSelectors.Card;
using Serilog;
using Robi.Common;
using Robi.Clash.DefaultSelectors.Utilities;
using Robi.Clash.DefaultSelectors.Logic;
using Robi.Clash.DefaultSelectors.Settings;

namespace Robi.Clash.DefaultSelectors.Game
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
