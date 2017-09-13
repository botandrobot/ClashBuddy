using Robi.Clash.DefaultSelectors.Enemy;
using Robi.Clash.DefaultSelectors.Game;
using Robi.Clash.DefaultSelectors.Player;
using Robi.Clash.DefaultSelectors.Settings;
using Robi.Clash.DefaultSelectors.Utilities;
using Robi.Clash.Engine;
using Robi.Clash.Engine.NativeObjects.Logic.GameObjects;
using Robi.Common;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Robi.Clash.DefaultSelectors.Logic
{
    enum FightState
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
        START,
        WAIT
    };

    enum EnemyPrincessTowerState
    {
        NPTD,       // No PrincessTower is down
        LPTD,       // Left PrincessTower is down
        RPTD,       // Right PrincessTower is down
        BPTD        // Both PrincessTower are down
    }

    enum PlayerPrincessTowerState
    {
        NPTD,       // No PrincessTower is down
        LPTD,       // Left PrincessTower is down
        RPTD,       // Right PrincessTower is down
        BPTD        // Both PrincessTower are down
    }

    enum GameMode
    {
        ONE_VERSUS_ONE,
        TWO_VERSUS_TWO,
        NOT_IMPLEMENTED
    }


    public enum Position
    {
        Down, // Starts from (0/0)
        Up    // Starts from (max/max)
    }

    class GameStateHandling
    {
        private static readonly ILogger Logger = LogProvider.CreateLogger<GameStateHandling>();
        public static bool GameBeginning = true;
        private static bool AttackMode = false;

        public static FightState CurrentFightState
        {
            get
            {
                SetAttackMode();

                switch (GameHandling.Settings.FightStyle)
                {
                    case FightStyle.Defensive:
                        return GetCurrentFightStateDefensive();
                    case FightStyle.Balanced:
                        return GetCurrentFightStateBalanced();
                    case FightStyle.Rusher:
                        return GetCurrentFightStateRusher();
                    default:
                        return FightState.DKT;
                }
            }
        }

        private static FightState GetCurrentFightStateBalanced()
        {
            FightState fightState = FightState.WAIT;

            if (GameBeginning)
                return GameBeginningDecision();

            if (EnemyCharacterHandling.IsAnEnemyOnOurSide())
                fightState = EnemyIsOnOurSideDecision();
            else if(StaticValues.Player.Mana >= GameHandling.Settings.ManaTillDeploy || AttackMode)
            {
                if (EnemyCharacterHandling.EnemiesWithoutTower.Count() > 2) // ToDo: CHeck more (Health, Damage etc) 
                    fightState = EnemyHasCharsOnTheFieldDecision();
                else
                    fightState = AttackDecision();
            }

            Logger.Debug("FightSate = {0}", fightState.ToString());
            return fightState;
        }

        private static FightState GetCurrentFightStateRusher()
        {
            if (EnemyCharacterHandling.IsAnEnemyOnOurSide())
                return EnemyIsOnOurSideDecision();
            else
                return AttackDecision();
        }

        private static FightState GetCurrentFightStateDefensive()
        {
            if (GameBeginning)
                return GameBeginningDecision();

            if (EnemyCharacterHandling.IsAnEnemyOnOurSide())
                return EnemyIsOnOurSideDecision();
            else if (EnemyCharacterHandling.EnemiesWithoutTower.Count() > 1)
                return EnemyHasCharsOnTheFieldDecision();
            else
                return DefenseDecision();
        }

        public static EnemyPrincessTowerState CurrentEnemyPrincessTowerState
        {
            get
            {
                int stateCode = 0;

                if (EnemyCharacterHandling.EnemyLeftPrincessTower == null)
                    stateCode += 1;

                if (EnemyCharacterHandling.EnemyRightPrincessTower == null)
                    stateCode += 2;

                return (EnemyPrincessTowerState)stateCode;
            }
        }

        public static PlayerPrincessTowerState CurrentPlayerPrincessTowerState
        {
            get
            {
                int stateCode = 0;

                if (PlayerCharacterHandling.LeftPrincessTower == null)
                    stateCode += 1;

                if (PlayerCharacterHandling.RightPrincessTower == null)
                    stateCode += 2;

                return (PlayerPrincessTowerState)stateCode;
            }
        }

        public static GameMode CurrentGameMode
        {
            get
            {
                if (StaticValues.PlayerCount == 2)
                    return GameMode.ONE_VERSUS_ONE;
                else if (StaticValues.PlayerCount == 4)
                    return GameMode.TWO_VERSUS_TWO;
                else
                {
                    Logger.Debug("GameMode: Seems to be not 1v1 or 2v2!");
                    return GameMode.NOT_IMPLEMENTED;
                }
                    
            }
        }

        private static void SetAttackMode()
        {
            AttackMode = (PlayerCharacterHandling.Troop.Count() > 0);
        }

#region GameState-Decisions

        private static FightState AttackDecision()
        {
            if (CurrentEnemyPrincessTowerState > 0)
                return FightState.AKT;

            Character princessTower = EnemyCharacterHandling.GetEnemyPrincessTowerWithLowestHealth(StaticValues.Player.OwnerIndex);

            if (PlaygroundPositionHandling.IsPositionOnTheRightSide(princessTower.StartPosition))
                return FightState.ARPT;
            else
                return FightState.ALPT;
        }

        private static FightState DefenseDecision()
        {
            if (CurrentEnemyPrincessTowerState > 0)
                return FightState.DKT;

            Character princessTower = EnemyCharacterHandling.GetEnemyPrincessTowerWithLowestHealth(StaticValues.Player.OwnerIndex);

            if (PlaygroundPositionHandling.IsPositionOnTheRightSide(princessTower.StartPosition))
                return FightState.DRPT;
            else
                return FightState.DLPT;
        }

        private static FightState GameBeginningDecision()
        {
            if (StaticValues.Player.Mana < GameHandling.Settings.ManaTillFirstAttack)
            {
                if (EnemyCharacterHandling.IsAnEnemyOnOurSide())
                    GameBeginning = false;

                return FightState.START;
            }
            else
            {
                GameBeginning = false;

                if (PlaygroundPositionHandling.IsPositionOnTheRightSide(EnemyCharacterHandling.NearestEnemy.StartPosition))
                    return FightState.DRPT;
                else
                    return FightState.DLPT;
            }
        }

        private static FightState EnemyIsOnOurSideDecision()
        {
            if (PlayerCharacterHandling.PrincessTower.Count() > 1)
            {
                if (PlaygroundPositionHandling.IsPositionOnTheRightSide(EnemyCharacterHandling.NearestEnemy.StartPosition))
                    return FightState.UARPT;
                else
                    return FightState.UALPT;
            }
            else
            {
                return FightState.UAKT;
            }
        }

        private static FightState EnemyHasCharsOnTheFieldDecision()
        {
            if (PlayerCharacterHandling.PrincessTower.Count() > 1)
            {
                if (PlaygroundPositionHandling.IsPositionOnTheRightSide(EnemyCharacterHandling.NearestEnemy.StartPosition))
                    return FightState.DRPT;
                else
                    return FightState.DLPT;
            }
            else
            {
                return FightState.DKT;
            }
        }
        #endregion
    }
}
