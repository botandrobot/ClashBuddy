using Buddy.Clash.Engine;
using Buddy.Clash.Engine.NativeObjects.Logic.GameObjects;
using Buddy.Common;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Buddy.Clash.DefaultSelectors.Utilities
{
    enum GameState
    {
        DLPT,       // Defense LeftPrincessTower
        DKT,        // Defense KingTower
        DRPT,       // Defense RightPrincessTower
        UALPT,      // UnderAttack LeftPrincessTower
        UAKT,       // UnderAttack KingTower
        UARPT,      // UnderAttack RightPrincessTower
        ALPT,       // Attack LeftPrincessTower
        AKT,        // Attack KingTower
        ARPT,        // Attack RightPrincessTower
        START
    };

    class GameStateHandling
    {
        private static readonly ILogger Logger = LogProvider.CreateLogger<GameStateHandling>();
        public static bool GameBeginning = true;

        public static GameState CurrentGameState
        {
            get
            {
                if (GameBeginning)
                    return GameBeginningDecision();

                if (CharacterHandling.IsAnEnemyOnOurSide())
                    return EnemyIsOnOurSideDecision();
                else
                    return AttackDecision();
            }
        }

        public static void GamePhase()
        {

        }

        private static int playerCount;
        public static int PlayerCount
        {
            set
            {
                playerCount = value;
            }
            get
            {
                if (playerCount == 0)
                    playerCount = ClashEngine.Instance.Battle.SummonerTowers.Where(n => n.StartPosition.X != 0).Count();

                return playerCount;
            }
        }

        private static GameState AttackDecision()
        {
            Character princessTower = CharacterHandling.GetEnemyPrincessTowerWithLowestHealth(StaticValues.Player.OwnerIndex);

            if (PositionHandling.IsPositionOnTheRightSide(princessTower.StartPosition))
                return GameState.ARPT;
            else
                return GameState.ALPT;
        }

        private static GameState GameBeginningDecision()
        {
            if (StaticValues.Player.Mana < 9)
            {
                if (CharacterHandling.IsAnEnemyOnOurSide())
                    GameBeginning = false;

                return GameState.START;
            }
            else
            {
                GameBeginning = false;

                if (PositionHandling.IsPositionOnTheRightSide(CharacterHandling.NearestEnemy.StartPosition))
                    return GameState.DRPT;
                else
                    return GameState.DLPT;
            }
        }

        private static GameState EnemyIsOnOurSideDecision()
        {
            if (CharacterHandling.PrincessTower.Count() > 1)
            {
                if (PositionHandling.IsPositionOnTheRightSide(CharacterHandling.NearestEnemy.StartPosition))
                    return GameState.UARPT;
                else
                    return GameState.UALPT;
            }
            else
            {
                return GameState.UAKT;
            }
        }
    }
}
