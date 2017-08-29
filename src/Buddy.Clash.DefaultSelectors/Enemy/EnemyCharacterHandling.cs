using Buddy.Clash.DefaultSelectors.Game;
using Buddy.Clash.DefaultSelectors.Utilities;
using Buddy.Clash.Engine;
using Buddy.Clash.Engine.NativeObjects.Logic.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Buddy.Clash.DefaultSelectors.Enemy
{
    class EnemyCharacterHandling
    {
        public static IEnumerable<Character> TempLastEnemieCharacters = new List<Character>();

        public static void Reset()
        {
            enemyLeftPrincessTower = EnemyPrincessTower.FirstOrDefault();
            enemyRightPrincessTower = EnemyPrincessTower.LastOrDefault();
        }

        #region characters
        public static IEnumerable<Character> EnemiesOnOurSide
        {
            get
            {
                IEnumerable<Character> enemiesOnOurSide = Enemies.Where(
                                                            n => PlaygroundPositionHandling.IsPositionOnPlayerSide(n.StartPosition));

                return enemiesOnOurSide;
            }
        }

        public static IEnumerable<Character> Enemies
        {
            get
            {
                var om = ClashEngine.Instance.ObjectManager;
                var chars = om.OfType<Character>();
                uint ownerIndex = StaticValues.Player.OwnerIndex;

                IEnumerable<Character> enemies = chars.Where(
                                                            n => n.OwnerIndex != ownerIndex);

                return enemies;
            }
        }

        public static IEnumerable<Character> EnemiesWithoutTower
        {
            get
            {
                var om = ClashEngine.Instance.ObjectManager;
                var chars = om.OfType<Character>();
                uint ownerIndex = StaticValues.Player.OwnerIndex;

                IEnumerable<Character> enemies = chars.Where(
                                                            n => n.OwnerIndex != ownerIndex
                                                            && n.LogicGameObjectData.Name.Value != "PrincessTower"
                                                            && n.LogicGameObjectData.Name.Value != "KingTower");

                return enemies;
            }
        }

        #endregion

        #region single character
        public static Character NearestEnemy
        {
            get
            {
                uint ownerIndex = StaticValues.Player.OwnerIndex;
                var om = ClashEngine.Instance.ObjectManager;
                var chars = om.OfType<Character>();
                var nearestChar = chars.Where(n => n.LogicGameObjectData.Name.Value != "PrincessTower" &&
                                                n.OwnerIndex != ownerIndex);

                var orderedChar = nearestChar.OrderBy(n => n.StartPosition.Y);

                if (ownerIndex == 0)
                {
                    //Logger.Debug("Nearest enemy-char: " + orderedChar.FirstOrDefault().LogicGameObjectData.Name);
                    return orderedChar.FirstOrDefault();
                }
                else
                {
                    //Logger.Debug("Nearest enemy-char: " + orderedChar.LastOrDefault().LogicGameObjectData.Name);
                    return orderedChar.LastOrDefault();
                }
            }
        }

        public static Character EnemyCharacterWithTheMostEnemiesAround(out int count)
        {
            int boarderX = 1000;
            int boarderY = 1000;
            IEnumerable<Character> enemies = Enemies;
            IEnumerable<Character> enemiesAroundTemp;
            Character enemy = null;
            count = 0;

            foreach (var item in enemies)
            {
                enemiesAroundTemp = enemies.Where(n => n.StartPosition.X > item.StartPosition.X - boarderX
                                                && n.StartPosition.X < item.StartPosition.X + boarderX &&
                                                n.StartPosition.Y > item.StartPosition.Y - boarderY &&
                                                n.StartPosition.Y < item.StartPosition.Y + boarderY);

                if (enemiesAroundTemp.Count() > count)
                {
                    count = enemiesAroundTemp.Count();
                    enemy = item;
                }
            }

            return enemy;
        }

        public static Character EnemyCharacterWithTheMostGroundEnemiesAround(out int count)
        {
            int boarderX = 1000;
            int boarderY = 1000;
            IEnumerable<Character> enemies = Enemies;
            IEnumerable<Character> enemiesAroundTemp;
            Character enemy = null;
            count = 0;

            foreach (var item in enemies)
            {
                enemiesAroundTemp = enemies.Where(n => n.StartPosition.X > item.StartPosition.X - boarderX
                                                && n.StartPosition.X < item.StartPosition.X + boarderX &&
                                                n.StartPosition.Y > item.StartPosition.Y - boarderY &&
                                                n.StartPosition.Y < item.StartPosition.Y + boarderY).Where(n => n.LogicGameObjectData.FlyingHeight == 0);

                if (enemiesAroundTemp.Count() > count)
                {
                    count = enemiesAroundTemp.Count();
                    enemy = item;
                }
            }

            return enemy;
        }

        public static Character EnemyNewSpawnedCharacter
        {
            get
            {
                var enemiesWithoutTower = EnemiesWithoutTower;

                foreach (var @char in enemiesWithoutTower)
                {
                    bool isNewCharacter = true;

                    foreach (var pastChar in TempLastEnemieCharacters)
                    {
                        //Logger.Debug("Char = {0} ; PastChar = {1}", @char.LogicGameObjectData.Name.Value, pastChar.LogicGameObjectData.Name.Value);

                        if (@char.LogicGameObjectData.Name.Value == pastChar.LogicGameObjectData.Name.Value
                            && @char.OwnerIndex == pastChar.OwnerIndex)
                            isNewCharacter = false;
                    }

                    if (isNewCharacter)
                    {

                        TempLastEnemieCharacters = enemiesWithoutTower.ToList();
                        return @char;
                    }
                }
                //TempLastEnemieCharacters = enemiesWithoutTower;
                return null;
            }
        }
        #endregion

        #region integer value as return
        public static int HealthOfEnemiesOnOurSide
        {
            get
            {
                int healthAmount = 0;
                IEnumerable<Character> enemiesOnOurSide = Enemies.Where(
                                                            n => PlaygroundPositionHandling.IsPositionOnPlayerSide(n.StartPosition));

                foreach (var @char in enemiesOnOurSide)
                {
                    healthAmount += @char.HealthComponent.CurrentHealth;
                }

                return healthAmount;
            }
        }
        #endregion

        #region Tower
        public static Character EnemyKingTower
        {
            get
            {
                return ClashEngine.Instance.Battle.SummonerTowers.Where(n =>
                                            n.OwnerIndex != StaticValues.Player.OwnerIndex).FirstOrDefault();
            }
        }

        public static IEnumerable<Character> EnemyPrincessTower
        {
            get
            {
                var om = ClashEngine.Instance.ObjectManager;
                var chars = om.OfType<Character>();
                var princessTower = chars.Where(n => n.LogicGameObjectData.Name.Value == "PrincessTower" &&
                                                n.OwnerIndex != StaticValues.Player.OwnerIndex).OrderBy(n => n.OwnerIndex).OrderBy(n => n.StartPosition.X);

                //foreach (var s in princessTower)
                //{
                //    Logger.Debug("PrincessTower: Owner - {0}; Position: {1}",
                //                s.OwnerIndex, s.StartPosition);
                //}
                return princessTower;
            }
        }

        private static Character enemyLeftPrincessTower;
        public static Character EnemyLeftPrincessTower
        {
            get
            {
                Character firstPrincessTower = EnemyPrincessTower.FirstOrDefault();

                if (enemyLeftPrincessTower == null)
                    enemyLeftPrincessTower = firstPrincessTower;

                // If the position is not equals, it means the LeftPrincessTower is already destroyed
                if (!firstPrincessTower.StartPosition.Equals(enemyLeftPrincessTower.StartPosition))
                    return null;

                return firstPrincessTower;
            }
        }

        private static Character enemyRightPrincessTower;
        public static Character EnemyRightPrincessTower
        {
            get
            {
                Character lastPrincessTower = EnemyPrincessTower.LastOrDefault();

                if (enemyRightPrincessTower == null)
                    enemyRightPrincessTower = lastPrincessTower;

                // If the position is not equals, it means the LeftPrincessTower is already destroyed
                if (!lastPrincessTower.StartPosition.Equals(enemyRightPrincessTower.StartPosition))
                    return null;

                return lastPrincessTower;
            }
        }

        public static Character GetEnemyPrincessTowerWithLowestHealth(uint ownerIndex)
        {
            var om = ClashEngine.Instance.ObjectManager;
            var chars = om.OfType<Character>();
            var princessTower = chars.Where(n => n.LogicGameObjectData.Name.Value == "PrincessTower" &&
                                            n.OwnerIndex == ownerIndex).OrderBy
                                            (n => n.HealthComponent.CurrentHealth).FirstOrDefault();

            //Logger.Debug("PrincessTower: Owner - {0}; Position: {1}",
            //            princessTower.OwnerIndex, princessTower.LogicGameObjectData.HealthBar.Value);

            return princessTower;
        }
        #endregion

        #region booleans as return
        public static bool IsAnEnemyOnOurSide()
        {
            var om = ClashEngine.Instance.ObjectManager;
            var chars = om.OfType<Character>();

            foreach (var @char in chars)
            {
                var data = @char.LogicGameObjectData;
                if (data != null && data.IsValid)
                {
                    //Logger.Debug("IsPositionOnOurSide: " + PositionHandling.IsPositionOnOurSide(@char.StartPosition));

                    if (@char.OwnerIndex != StaticValues.Player.OwnerIndex && PlaygroundPositionHandling.IsPositionOnPlayerSide(@char.StartPosition))
                        return true;
                }
            }
            return false;
        }

        public static bool IsFlyingEnemyOnTheField()
        {
            var om = ClashEngine.Instance.ObjectManager;
            var chars = om.OfType<Character>();

            foreach (var @char in chars)
            {
                if (@char.LogicGameObjectData.FlyingHeight > 0)
                    return true;
            }
            return false;
        }
        #endregion
    }
}
