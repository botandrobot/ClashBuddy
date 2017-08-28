using Buddy.Clash.Engine.NativeObjects.Logic.GameObjects;
using Buddy.Clash.Engine.NativeObjects.Native;
using Buddy.Common;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Buddy.Clash.DefaultSelectors.Utilities
{
    class PositionHandling
    {
        private static readonly ILogger Logger = LogProvider.CreateLogger<PositionHandling>();
        private static Random rnd = new Random();

        #region Left and Right Bridge
        private static Vector2f leftBridge = Vector2f.Zero;
        public Vector2f LeftBridge
        {
            get
            {
                if (leftBridge.Equals(Vector2f.Zero))
                    leftBridge = CalculatetLeftBridgePosition(0);


                return leftBridge;
            }
        }

        private static Vector2f rightBridge = Vector2f.Zero;
        public Vector2f RightBridge
        {
            get
            {
                if (rightBridge.Equals(Vector2f.Zero))
                    rightBridge = CalculatetRightBridgePosition(0);


                return rightBridge;
            }
        }
        #endregion

        #region middle line
        private static int middleLineY = 0;
        public static int MiddleLineY
        {
            get
            {
                if (middleLineY == 0)
                    middleLineY = (CharacterHandling.KingTower.StartPosition.Y + 
                                    CharacterHandling.EnemyKingTower.StartPosition.Y) / 2;

                return middleLineY;
            }
        }
        #endregion


        public static bool IsPositionOnOurSide(Vector2 position)
        {
            //Logger.Debug("PositionY: " + position.Y + " MiddleLinePositionY: " + MiddleLineY);
            // ToDo: Is not rdy for 2v2


            if (StaticValues.Player.OwnerIndex == 0)
                return (position.Y < MiddleLineY);
            else
                return (position.Y > MiddleLineY);

        }

        public Vector2f GetNextSpellPosition(GameState gameState)
        {
            Vector2f rndAddVector = new Vector2(rnd.Next(-200, 200), rnd.Next(-500, 500));
            Vector2f choosedPosition = Vector2f.Zero, nextPosition;

            // ToDo: Handle Defense Gamestates
            switch(gameState)
            {
                case GameState.UAKT:
                    choosedPosition = UAKT();
                    break;
                case GameState.UALPT:
                    choosedPosition = UALPT();
                    break;
                case GameState.UARPT:
                    choosedPosition = UARPT();
                    break;
                case GameState.AKT:
                    choosedPosition = AKT();
                    break;
                case GameState.ALPT:
                    choosedPosition = ALPT();
                    break;
                case GameState.ARPT:
                    choosedPosition = ARPT();
                    break;
                case GameState.DKT:
                    choosedPosition = DKT();
                    break;
                case GameState.DLPT:
                    choosedPosition = DLPT();
                    break;
                case GameState.DRPT:
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

        public Vector2f CalculatetLeftBridgePosition(uint ownerIndex)
        {
            IEnumerable<Character> ownPrincessTowers;

            ownPrincessTowers = CharacterHandling.PrincessTower;
            var pT = ownPrincessTowers.FirstOrDefault();
            Vector2f ownTowerPos = pT.StartPosition;

            IEnumerable<Character> enemyPrincessTowers;

            enemyPrincessTowers = CharacterHandling.EnemyPrincessTower;

            var pT2 = enemyPrincessTowers.FirstOrDefault();
            Vector2f enemyTowerPos = pT2.StartPosition;

            Vector2f brPosition = ((ownTowerPos + enemyTowerPos) / 2);
            //Logger.Debug("Bridge-Postion: " + brPosition);
                
            return brPosition;
        }

        public Vector2f CalculatetRightBridgePosition(uint ownerIndex)
        {
            IEnumerable<Character> ownPrincessTowers;

            ownPrincessTowers = CharacterHandling.PrincessTower;
            var pT = ownPrincessTowers.LastOrDefault();
            Vector2f ownTowerPos = pT.StartPosition;

            IEnumerable<Character> enemyPrincessTowers;

            enemyPrincessTowers = CharacterHandling.EnemyPrincessTower;

            var pT2 = enemyPrincessTowers.LastOrDefault();
            Vector2f enemyTowerPos = pT2.StartPosition;

            Vector2f brPosition = ((ownTowerPos + enemyTowerPos) / 2);
            //Logger.Debug("Bridge-Postion: " + brPosition);

            return brPosition;
        }

        public Vector2f GetPositionOfTheMostDangerousAttack()
        {
            // comes later
            return Vector2f.Zero;
        }

        public Vector2f GetPositionOfTheBestDamagingSpellDeploy()
        {
            // Prio1: Fireball if one of the towers health is really low
            // Prio2: Every damaging spell if there is a big group of enemies
            return Vector2f.Zero;
        }

        public static bool IsPositionOnTheRightSide(Vector2f position)
        {
            if (position.X > CharacterHandling.KingTower.StartPosition.X)
                return true;
            else
                return false;
        }

        private Vector2f UAKT()
        {
            return CharacterHandling.KingTower.StartPosition;
        }

        private Vector2f UALPT()
        {
            return CharacterHandling.PrincessTower.FirstOrDefault().StartPosition;
        }
        private Vector2f UARPT()
        {
                return CharacterHandling.PrincessTower.LastOrDefault().StartPosition;
        }
        private Vector2f DKT()
        {
            return CharacterHandling.KingTower.StartPosition;
        }
        private Vector2f DLPT()
        {
            return CharacterHandling.LeftPrincessTower.StartPosition;
        }
        private Vector2f DRPT()
        {
            return CharacterHandling.RightPrincessTower.StartPosition;
        }

        private Vector2f AKT()
        {
            return CharacterHandling.EnemyKingTower.StartPosition;
        }
        private Vector2f ALPT()
        {
            return CharacterHandling.EnemyLeftPrincessTower.StartPosition;
        }
        private Vector2f ARPT()
        {
            return CharacterHandling.EnemyRightPrincessTower.StartPosition;
        }
    }
}
