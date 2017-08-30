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
using Buddy.Clash.Engine.NativeObjects.Logic.GameObjects;
using Buddy.Clash.DefaultSelectors.Settings;
using Buddy.Clash.DefaultSelectors.Player;

namespace Buddy.Clash.DefaultSelectors.Logic
{
    class CastPositionHandling
    {
        private static readonly ILogger Logger = LogProvider.CreateLogger<CastHandling>();

        public Vector2f GetNextSpellPosition(FightState gameState)
        {
            Random rnd = StaticValues.rnd;
            float rndX = rnd.Next(-GameHandling.Settings.RandomDeploymentValue, GameHandling.Settings.RandomDeploymentValue);
            float rndY = rnd.Next(-GameHandling.Settings.RandomDeploymentValue, GameHandling.Settings.RandomDeploymentValue);

            Vector2f rndAddVector = new Vector2f(rndX, rndY);
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
                    //Logger.Debug("GameState unknown");
                    break;
            }
            Logger.Debug("GameState: {GameState}", gameState.ToString());
            nextPosition = (choosedPosition + rndAddVector);
            //Logger.Debug("nextPosition: " + nextPosition);

            return nextPosition;
        }

        private Vector2f UAKT()
        {
            if (PlaygroundPositionHandling.IsPositionOnTheRightSide(EnemyCharacterHandling.NearestEnemy.StartPosition))
                return PlayerCharacterHandling.KingTower.StartPosition + new Vector2f(1000, 0);
            else
                return PlayerCharacterHandling.KingTower.StartPosition - new Vector2f(1000, 0);
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
            if (PlaygroundPositionHandling.IsPositionOnTheRightSide(EnemyCharacterHandling.NearestEnemy.StartPosition))
                return PlayerCharacterHandling.KingTower.StartPosition + new Vector2f(1000, 0);
            else
                return PlayerCharacterHandling.KingTower.StartPosition - new Vector2f(1000, 0);

        }
        private Vector2f DLPT()
        {
            Vector2f lPT = PlayerCharacterHandling.LeftPrincessTower.StartPosition;
            Vector2f positionBehindTower = PositionHelper.AddYInDirection(lPT, PlayerProperties.PlayerPosition);
            return positionBehindTower;
        }
        private Vector2f DRPT()
        {
            Vector2f rPT = PlayerCharacterHandling.RightPrincessTower.StartPosition;
            Vector2f positionBehindTower = PositionHelper.AddYInDirection(rPT, PlayerProperties.PlayerPosition);
            return positionBehindTower;
        }

        private Vector2f AKT()
        {
            switch (GameStateHandling.CurrentEnemyPrincessTowerState)
            {
                case EnemyPrincessTowerState.NPTD:
                    Logger.Debug("Bug: NoPrincessTowerDown-State in Attack-King-Tower-State!");
                    return EnemyCharacterPositionHandling.EnemyLeftPrincessTower;
                case EnemyPrincessTowerState.LPTD:
                    {
                        Logger.Debug("LPTD");
                        return EnemyCharacterPositionHandling.EnemyLeftPrincessTower;
                    }
                case EnemyPrincessTowerState.RPTD:
                    {
                        Logger.Debug("RPTD");
                        return EnemyCharacterPositionHandling.EnemyRightPrincessTower;
                    }
                case EnemyPrincessTowerState.BPTD:
                    {
                        Logger.Debug("BPTD");
                        return EnemyCharacterHandling.EnemyKingTower.StartPosition;
                    }
                default:
                    return EnemyCharacterHandling.EnemyKingTower.StartPosition;
            }
        }
        private Vector2f ALPT()
        {
            Logger.Debug("ALPT");
            Vector2f lPT = EnemyCharacterPositionHandling.EnemyLeftPrincessTower;
            return lPT;
        }
        private Vector2f ARPT()
        {
            Logger.Debug("ARPT");
            Vector2f rPT = EnemyCharacterPositionHandling.EnemyRightPrincessTower;
            return rPT;
        }

        public static Vector2f GetPositionOfTheBestDamagingSpellDeploy()
        {
            // Prio1: Hit Enemy King Tower if health is low
            // Prio2: Every damaging spell if there is a big group of enemies

            if (EnemyCharacterHandling.EnemyKingTower.HealthComponent.CurrentHealth < GameHandling.Settings.KingTowerSpellDamagingHealth)
                return EnemyCharacterHandling.EnemyKingTower.StartPosition;
            else
            {
                Character enemy;

                if (CastHandling.DamagingSpellDecision(out enemy))
                {
                    
                    if (PlayerCharacterHandling.HowManyCharactersAroundCharacter(enemy) >= GameHandling.Settings.SpellCorrectionConditionCharCount)
                        return enemy.StartPosition;
                    else
                    {
                        // Position Correction
                        return PositionHelper.AddYInDirection(enemy.StartPosition, PlayerProperties.PlayerPosition, 4000);
                    }
                }
            }

            return Vector2f.Zero;
        }
    }
}
