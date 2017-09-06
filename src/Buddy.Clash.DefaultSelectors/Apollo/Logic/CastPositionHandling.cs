using Buddy.Clash.Engine.NativeObjects.Native;
using Buddy.Clash.DefaultSelectors.Game;
using Buddy.Clash.DefaultSelectors.Utilities;
using Buddy.Clash.DefaultSelectors.Enemy;
using Buddy.Common;
using Serilog;
using System;
using Buddy.Clash.Engine.NativeObjects.Logic.GameObjects;
using Buddy.Clash.DefaultSelectors.Player;
using Buddy.Clash.DefaultSelectors.Card;

namespace Buddy.Clash.DefaultSelectors.Logic
{
    class CastPositionHandling
    {
        private static readonly ILogger Logger = LogProvider.CreateLogger<CastDeploymentHandling>();
        private static ICard cardToDeploy;

        public static Vector2f GetNextSpellPosition(FightState gameState, ICard card)
        {
            cardToDeploy = card;

            #region Randomise
            Random rnd = StaticValues.rnd;
            float rndX = rnd.Next(-GameHandling.Settings.RandomDeploymentValue, GameHandling.Settings.RandomDeploymentValue);
            float rndY = rnd.Next(-GameHandling.Settings.RandomDeploymentValue, GameHandling.Settings.RandomDeploymentValue);
            Vector2f rndAddVector = new Vector2f(rndX, rndY);
            #endregion

            Vector2f choosedPosition = Vector2f.Zero, nextPosition;

            if (cardToDeploy is CardSpell)
                return GetPositionOfTheBestDamagingSpellDeploy();

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

        #region UnderAttack
        private static Vector2f UAKT()
        {
            return DKT();
        }

        private static Vector2f UALPT()
        {
            return DLPT();
        }
        private static Vector2f UARPT()
        {
            return DRPT();
        }
        #endregion

        #region Defense
        private static Vector2f DKT()
        {
            if (cardToDeploy is CardCharacter)
            {
                switch ((cardToDeploy as CardCharacter).Type)
                {
                    case TroopType.GroundAttack:
                    case TroopType.Flying:
                    case TroopType.Tank:
                        Logger.Debug("DKT Troop-Name {0} ; CartType GroundAttack, Flying or Tank", cardToDeploy.Name);
                        if (PlaygroundPositionHandling.IsPositionOnTheRightSide(EnemyCharacterHandling.NearestEnemy.StartPosition))
                            return PlayerCharacterHandling.KingTower.StartPosition + new Vector2f(1000, 0);
                        else
                            return PlayerCharacterHandling.KingTower.StartPosition - new Vector2f(1000, 0);
                    case TroopType.Ranger:
                    case TroopType.AirAttack:
                    case TroopType.AOEAttackGround:
                    case TroopType.AOEAttackFlying:
                    case TroopType.Damager:
                        Vector2f position = PositionHelper.AddYInDirection(PlayerCharacterHandling.KingTower.StartPosition, PlayerProperties.PlayerPosition);

                        if (PlaygroundPositionHandling.IsPositionOnTheRightSide(EnemyCharacterHandling.NearestEnemy.StartPosition))
                            return position + new Vector2f(300, 0);
                        else
                            return position - new Vector2f(300, 0);
                    default:
                        break;
                }
            }
            else if(cardToDeploy is CardBuilding)
            {
                switch ((cardToDeploy as CardBuilding).Type)
                {
                    case BuildingType.BuildingDefense:
                    case BuildingType.BuildingSpawning:
                        return GetPositionOfTheBestBuildingDeploy();
                }
            }

            if (PlaygroundPositionHandling.IsPositionOnTheRightSide(EnemyCharacterHandling.NearestEnemy.StartPosition))
                return PlayerCharacterHandling.KingTower.StartPosition + new Vector2f(1000, 0);
            else
                return PlayerCharacterHandling.KingTower.StartPosition - new Vector2f(1000, 0);

        }
        private static Vector2f DLPT()
        {
            Character lPT = PlayerCharacterHandling.LeftPrincessTower;
            if (lPT == null)
                return DKT();

            Logger.Debug("DLPT: LeftPrincessTower = " + lPT.ToString());
            Vector2f lPTP = lPT.StartPosition;
            Vector2f correctedPosition = PrincessTowerCharacterDeploymentCorrection(lPTP);
            return correctedPosition;
        }
        private static Vector2f DRPT()
        {
            Character rPT = PlayerCharacterHandling.RightPrincessTower;
            if (rPT == null)
                return DKT();

            Vector2f rPTP = PlayerCharacterHandling.RightPrincessTower.StartPosition;
            Vector2f correctedPosition = PrincessTowerCharacterDeploymentCorrection(rPTP);
            return correctedPosition;
        }
        #endregion

        #region Attack
        private static Vector2f AKT()
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
        private static Vector2f ALPT()
        {
            Logger.Debug("ALPT");
            Vector2f lPT = EnemyCharacterPositionHandling.EnemyLeftPrincessTower;
            return lPT;
        }
        private static Vector2f ARPT()
        {
            Logger.Debug("ARPT");
            Vector2f rPT = EnemyCharacterPositionHandling.EnemyRightPrincessTower;
            return rPT;
        }
        #endregion

        public static Vector2f GetPositionOfTheBestDamagingSpellDeploy()
        {
            // Prio1: Hit Enemy King Tower if health is low
            // Prio2: Every damaging spell if there is a big group of enemies

            if (EnemyCharacterHandling.EnemyKingTower.HealthComponent.CurrentHealth < GameHandling.Settings.KingTowerSpellDamagingHealth)
                return EnemyCharacterHandling.EnemyKingTower.StartPosition;
            else
            {
                int count;
                Character enemy = EnemyCharacterHandling.EnemyCharacterWithTheMostEnemiesAround(out count);

                if (enemy != null)
                {
                    
                    if (PlayerCharacterHandling.HowManyCharactersAroundCharacter(enemy) >= GameHandling.Settings.SpellCorrectionConditionCharCount)
                        return enemy.StartPosition;
                    else
                    {
                        // Position Correction
                        return PositionHelper.AddYInDirection(enemy.StartPosition, PlayerProperties.PlayerPosition, 2000);
                    }
                }
            }
            Logger.Debug("Error: Enemy for damaging spell is null!!!");
            return Vector2f.Zero;
        }

        public static Vector2f GetPositionOfTheBestBuildingDeploy()
        {
            // ToDo: Find the best position
            Vector2f nextPosition = PlayerCharacterHandling.KingTower.StartPosition;
            nextPosition = PositionHelper.AddYInDirection(nextPosition, PlayerProperties.PlayerPosition, 3000);
            return nextPosition;
        }

        private static Vector2f PrincessTowerCharacterDeploymentCorrection(Vector2f position)
        {
            Logger.Debug("PT Characer Position Correction: Name und Typ {0} " + cardToDeploy.Name, (cardToDeploy as CardCharacter).Type);
            Vector2f result = Vector2f.Zero;

            if (cardToDeploy is CardCharacter)
            {
                switch ((cardToDeploy as CardCharacter).Type)
                {
                    case TroopType.GroundAttack:
                    
                    case TroopType.Tank:
                        return PositionHelper.SubtractYInDirection(position, PlayerProperties.PlayerPosition);
                    case TroopType.Ranger:
                    case TroopType.AirAttack:
                    case TroopType.AOEAttackGround:
                    case TroopType.AOEAttackFlying:
                    case TroopType.Damager:
                    case TroopType.Flying:
                        return PositionHelper.AddYInDirection(position, PlayerProperties.PlayerPosition);
                    default:
                        break;
                }
            }
            else if (cardToDeploy is CardBuilding)
            {
                switch ((cardToDeploy as CardBuilding).Type)
                {
                    case BuildingType.BuildingDefense:
                    case BuildingType.BuildingSpawning:
                        return GetPositionOfTheBestBuildingDeploy();
                }
            }
            else
            Logger.Debug("Tower Correction: No Correction!!!");

            return position;
        }

    }
}
