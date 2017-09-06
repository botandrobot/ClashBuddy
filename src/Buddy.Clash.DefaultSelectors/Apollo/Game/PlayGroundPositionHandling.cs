using Buddy.Clash.DefaultSelectors.Enemy;
using Buddy.Clash.DefaultSelectors.Player;
using Buddy.Clash.DefaultSelectors.Utilities;
using Buddy.Clash.Engine.NativeObjects.Logic.GameObjects;
using Buddy.Clash.Engine.NativeObjects.Native;
using Buddy.Clash.DefaultSelectors.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Buddy.Clash.DefaultSelectors.Game
{
    class PlaygroundPositionHandling
    {

        public Vector2f CalculatetLeftBridgePosition()
        {
            IEnumerable<Character> ownPrincessTowers;

            ownPrincessTowers = PlayerCharacterHandling.PrincessTower;
            var pT = ownPrincessTowers.FirstOrDefault();
            Vector2f ownTowerPos = pT.StartPosition;

            IEnumerable<Character> enemyPrincessTowers;

            enemyPrincessTowers = EnemyCharacterHandling.EnemyPrincessTower;

            var pT2 = enemyPrincessTowers.FirstOrDefault();
            Vector2f enemyTowerPos = pT2.StartPosition;

            Vector2f brPosition = ((ownTowerPos + enemyTowerPos) / 2);
            //Logger.Debug("Bridge-Postion: " + brPosition);

            return brPosition;
        }

        public Vector2f CalculatetRightBridgePosition()
        {
            IEnumerable<Character> ownPrincessTowers;

            ownPrincessTowers = PlayerCharacterHandling.PrincessTower;
            var pT = ownPrincessTowers.LastOrDefault();
            Vector2f ownTowerPos = pT.StartPosition;

            IEnumerable<Character> enemyPrincessTowers;

            enemyPrincessTowers = EnemyCharacterHandling.EnemyPrincessTower;

            var pT2 = enemyPrincessTowers.LastOrDefault();
            Vector2f enemyTowerPos = pT2.StartPosition;

            Vector2f brPosition = ((ownTowerPos + enemyTowerPos) / 2);
            //Logger.Debug("Bridge-Postion: " + brPosition);

            return brPosition;
        }

        public static bool IsPositionOnTheRightSide(Vector2f position)
        {
            if (position.X > PlayerCharacterHandling.KingTower.StartPosition.X)
                return true;
            else
                return false;
        }

        public static bool IsPositionOnPlayerSide(Vector2 position)
        {
            //Logger.Debug("PositionY: " + position.Y + " MiddleLinePositionY: " + MiddleLineY);
            // ToDo: Is not rdy for 2v2


            if (PlayerProperties.PlayerPosition == Position.Down)
                return (position.Y < MiddleLineY);
            else
                return (position.Y > MiddleLineY);

        }

        public static Position IsPositionUpOrDown(Vector2 position)
        {
            if (position.Y > MiddleLineY)
                return Position.Up;
            else
                return Position.Down;
        }

        #region Left and Right Bridge
        private static Vector2f leftBridge = Vector2f.Zero;
        public Vector2f LeftBridge
        {
            get
            {
                if (leftBridge.Equals(Vector2f.Zero))
                    leftBridge = CalculatetLeftBridgePosition();


                return leftBridge;
            }
        }

        private static Vector2f rightBridge = Vector2f.Zero;
        public Vector2f RightBridge
        {
            get
            {
                if (rightBridge.Equals(Vector2f.Zero))
                    rightBridge = CalculatetRightBridgePosition();


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
                    middleLineY = (PlayerCharacterHandling.KingTower.StartPosition.Y +
                                    EnemyCharacterHandling.EnemyKingTower.StartPosition.Y) / 2;

                return middleLineY;
            }
        }
        #endregion
    }
}
