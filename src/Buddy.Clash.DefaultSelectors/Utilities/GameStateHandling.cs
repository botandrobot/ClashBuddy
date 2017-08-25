using Buddy.Clash.Engine;
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
        private static readonly ILogger Logger = LogProvider.CreateLogger<EarlyCycleSelector>();
        public static bool GameBeginning = true;

        public static GameState CurrentGameState
        {
            get
            {
                uint ownerIndex = ClashEngine.Instance.LocalPlayer.OwnerIndex;

                if (GameBeginning == true)
                {
                    if (ClashEngine.Instance.LocalPlayer.Mana < 9)
                    {
                        if (CharacterHandling.IsEnemyOnOurSide())
                            GameBeginning = false;

                        return GameState.START;
                    }
                    else
                    {
                        GameBeginning = false;

                        if (CharacterHandling.NearestEnemy.StartPosition.GetX() > CharacterHandling.KingTower.StartPosition.GetX())
                            return GameState.DLPT;
                        else
                            return GameState.DRPT;
                    }
                }

                if (CharacterHandling.IsEnemyOnOurSide())
                {
                    if (CharacterHandling.PrincessTower.Count() > 1)
                    {
                        if (CharacterHandling.NearestEnemy.StartPosition.GetX() > CharacterHandling.KingTower.StartPosition.GetX())
                            return GameState.UALPT;
                        else
                            return GameState.UARPT;
                    }
                    else
                    {
                        return GameState.UAKT;
                    }
                }
                else
                {
                    // ToDo: Implement logic for Defense and Attack-Mode
                    return GameState.ALPT;
                }
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
                    playerCount = ClashEngine.Instance.Battle.SummonerTowers.Where(n => n.StartPosition.GetX() != 0).Count();

                return playerCount;
            }
        }
    }
}
