using Buddy.Clash.Engine.NativeObjects.Native;
using Buddy.Clash.DefaultSelectors.Game;
using Buddy.Clash.DefaultSelectors.Utilities;
using Buddy.Clash.DefaultSelectors.Enemy;
using Buddy.Common;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Buddy.Clash.DefaultSelectors.Player
{
    class PlayerCastPositionHandling
    {
        private static readonly ILogger Logger = LogProvider.CreateLogger<PlayerCastHandling>();

        public Vector2f GetNextSpellPosition(FightState gameState)
        {
            Random rnd = StaticValues.rnd;

            Vector2f rndAddVector = new Vector2(rnd.Next(-200, 200), rnd.Next(-500, 500));
            Vector2f choosedPosition = Vector2f.Zero, nextPosition;

            // ToDo: Handle Defense Gamestates
            switch (gameState)
            {
                case FightState.UAKT:
                    choosedPosition = UAKT();
                    break;
                case FightState.UALPT:
                    choosedPosition = UALPT();
                    break;
                case FightState.UARPT:
                    choosedPosition = UARPT();
                    break;
                case FightState.AKT:
                    choosedPosition = AKT();
                    break;
                case FightState.ALPT:
                    choosedPosition = ALPT();
                    break;
                case FightState.ARPT:
                    choosedPosition = ARPT();
                    break;
                case FightState.DKT:
                    choosedPosition = DKT();
                    break;
                case FightState.DLPT:
                    choosedPosition = DLPT();
                    break;
                case FightState.DRPT:
                    choosedPosition = DRPT();
                    break;
                default:
                    Logger.Debug("GameState unknown");
                    break;
            }
            //Logger.Debug("GameState: {GameState}", gameState.ToString());
            nextPosition = (choosedPosition + rndAddVector);
            //Logger.Debug("nextPosition: " + nextPosition);

            return nextPosition;
        }

        private Vector2f UAKT()
        {
            return PlayerCharacterHandling.KingTower.StartPosition;
        }

        private Vector2f UALPT()
        {
            return PlayerCharacterHandling.PrincessTower.FirstOrDefault().StartPosition;
        }
        private Vector2f UARPT()
        {
            return PlayerCharacterHandling.PrincessTower.LastOrDefault().StartPosition;
        }
        private Vector2f DKT()
        {
            return PlayerCharacterHandling.KingTower.StartPosition;
        }
        private Vector2f DLPT()
        {
            return PlayerCharacterHandling.LeftPrincessTower.StartPosition;
        }
        private Vector2f DRPT()
        {
            return PlayerCharacterHandling.RightPrincessTower.StartPosition;
        }

        private Vector2f AKT()
        {
            switch (GameStateHandling.CurrentEnemyPrincessTowerState)
            {
                case EnemyPrincessTowerState.NPTD:
                    Logger.Debug("Bug: NoPrincessTowerDown-State in Attack-King-Tower-State!");
                    return EnemyCharacterPositionHandling.EnemyLeftPrincessTower;
                case EnemyPrincessTowerState.LPTD:
                    return EnemyCharacterPositionHandling.EnemyLeftPrincessTower;
                case EnemyPrincessTowerState.RPTD:
                    return EnemyCharacterPositionHandling.EnemyRightPrincessTower;
                case EnemyPrincessTowerState.BPTD:
                    return EnemyCharacterHandling.EnemyKingTower.StartPosition;
                default:
                    return EnemyCharacterHandling.EnemyKingTower.StartPosition;
            }
        }
        private Vector2f ALPT()
        {
            return EnemyCharacterHandling.EnemyLeftPrincessTower.StartPosition;
        }
        private Vector2f ARPT()
        {
            return EnemyCharacterHandling.EnemyRightPrincessTower.StartPosition;
        }

        public Vector2f GetPositionOfTheBestDamagingSpellDeploy()
        {
            // Prio1: Fireball if one of the towers health is really low
            // Prio2: Every damaging spell if there is a big group of enemies
            return Vector2f.Zero;
        }
    }
}
