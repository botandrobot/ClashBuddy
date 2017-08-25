using Buddy.Clash.DefaultSelectors.Utilities;
using Buddy.Clash.Engine;
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
        private static readonly ILogger Logger = LogProvider.CreateLogger<EarlyCycleSelector>();
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
                                    CharacterHandling.EnemieKingTower.StartPosition.Y) / 2;

                return middleLineY;
            }
        }
        #endregion


        public static bool IsPositionOnOurSide(Vector2 position)
        {
            Logger.Debug("PositionY: " + position.Y + " MiddleLinePositionY: " + MiddleLineY);

            if (ClashEngine.Instance.LocalPlayer.OwnerIndex == 0)
                return (position.Y < MiddleLineY);
            else
                return (position.Y > MiddleLineY);
        }

        public Vector2f GetNextSpellPosition(GameState gameState)
        {
            Vector2f rndAddVector = new Vector2(rnd.Next(-500, 500), rnd.Next(-1000, 1000));
            Vector2f choosedPosition = Vector2f.Zero, nextPosition;
            uint ownerIndex = ClashEngine.Instance.LocalPlayer.OwnerIndex;

            // ToDo: Handle Defense Gamestates
            switch(gameState)
            {
                case GameState.UAKT:
                case GameState.UALPT:
                case GameState.UARPT:
                    Log.Debug("GameState: {GameState}", gameState.ToString());

                    if (CharacterHandling.PrincessTower.Count() > 1)
                    {
                            if (CharacterHandling.NearestEnemy.StartPosition.X > CharacterHandling.KingTower.StartPosition.X)
                                choosedPosition = CharacterHandling.PrincessTower
                                                                    .LastOrDefault().StartPosition;
                            else
                                choosedPosition = CharacterHandling.PrincessTower
                                                                    .FirstOrDefault().StartPosition;
                    }
                    else
                    {
                        choosedPosition = CharacterHandling.KingTower.StartPosition;
                    }
                    break;

                case GameState.AKT: // ToDo
                case GameState.ALPT:
                    choosedPosition = LeftBridge;
                    break;
                case GameState.ARPT:
                    choosedPosition = RightBridge;
                    break;
                case GameState.DKT:
                case GameState.DLPT:
                    choosedPosition = CharacterHandling.LeftPrincessTower.StartPosition;
                    break;
                case GameState.DRPT:
                    choosedPosition = CharacterHandling.RightPrincessTower.StartPosition;
                    break;
                default:
                    Log.Debug("GameState unknown");
                    break;
            }
            Log.Debug("GameState: {GameState}", gameState.ToString());
            nextPosition = (choosedPosition + rndAddVector);
            Logger.Debug("nextPosition: " + nextPosition);

            return nextPosition;
        }

        public Vector2f CalculatetLeftBridgePosition(uint ownerIndex)
        {
            IEnumerable<Character> ownPrincessTowers;

            ownPrincessTowers = CharacterHandling.PrincessTower;
            var pT = ownPrincessTowers.FirstOrDefault();
            Vector2f ownTowerPos = pT.StartPosition;

            IEnumerable<Character> enemyPrincessTowers;

            enemyPrincessTowers = CharacterHandling.EnemiePrincessTower;

            var pT2 = enemyPrincessTowers.FirstOrDefault();
            Vector2f enemyTowerPos = pT2.StartPosition;

            Vector2f brPosition = ((ownTowerPos + enemyTowerPos) / 2);
            Log.Debug("Bridge-Postion: " + brPosition);
                
            return brPosition;
        }

        public Vector2f CalculatetRightBridgePosition(uint ownerIndex)
        {
            IEnumerable<Character> ownPrincessTowers;

            ownPrincessTowers = CharacterHandling.PrincessTower;
            var pT = ownPrincessTowers.LastOrDefault();
            Vector2f ownTowerPos = pT.StartPosition;

            IEnumerable<Character> enemyPrincessTowers;

            enemyPrincessTowers = CharacterHandling.EnemiePrincessTower;

            var pT2 = enemyPrincessTowers.LastOrDefault();
            Vector2f enemyTowerPos = pT2.StartPosition;

            Vector2f brPosition = ((ownTowerPos + enemyTowerPos) / 2);
            Log.Debug("Bridge-Postion: " + brPosition);

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
    }
}
