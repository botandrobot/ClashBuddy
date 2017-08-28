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
    class CharacterHandling
    {
        private static readonly ILogger Logger = LogProvider.CreateLogger<EarlyCycleSelector>();

        public static IEnumerable<Character> TempLastEnemieCharacters = new List<Character>();

        #region OfOwner
        public static IEnumerable<Character> PrincessTowerOfOwner(uint ownerIndex)
        {
            var om = ClashEngine.Instance.ObjectManager;
            var chars = om.OfType<Character>();
            var princessTower = chars.Where(n => n.LogicGameObjectData.Name.Value == "PrincessTower" &&
                                            n.OwnerIndex == ownerIndex).OrderBy(n => n.OwnerIndex).OrderBy(n => n.StartPosition.X);
            return princessTower;
        }

        public static Character KingTowerOfOwner(uint ownerIndex)
        {
            return ClashEngine.Instance.Battle.SummonerTowers.Where(n =>
                                        n.OwnerIndex == ownerIndex).FirstOrDefault();
        }
        #endregion

        #region player
        public static Character KingTower
        {
            get
            {
                return ClashEngine.Instance.Battle.SummonerTowers.Where(n =>
                                            n.OwnerIndex == StaticValues.Player.OwnerIndex).FirstOrDefault();
            }
        }

        public static IEnumerable<Character> PrincessTower
        {
            get
            {
                var om = ClashEngine.Instance.ObjectManager;
                var chars = om.OfType<Character>();
                var princessTower = chars.Where(n => n.LogicGameObjectData.Name.Value == "PrincessTower" &&
                                                n.OwnerIndex == StaticValues.Player.OwnerIndex).OrderBy(n => n.StartPosition.X);

                foreach (var s in princessTower)
                {
                    // Logger.Debug("PrincessTower: Owner - {0}; Position: {1}",
                    //             s.OwnerIndex, s.StartPosition);
                }
                return princessTower;
            }
        }

        public static Character LeftPrincessTower
        {
            get
            {
                return PrincessTower.FirstOrDefault();
            }
        }

        public static Character RightPrincessTower
        {
            get
            {
                return PrincessTower.LastOrDefault();
            }
        }
        #endregion

        #region enemie

        #region characters
        public static IEnumerable<Character> EnemiesOnOurSide
        {
            get
            {
                IEnumerable<Character> enemiesOnOurSide = Enemies.Where(
                                                            n => PositionHandling.IsPositionOnOurSide(n.StartPosition));

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
        public static uint HealthOfEnemiesOnOurSide
        {
            get
            {
                uint healthAmount = 0;
                IEnumerable<Character> enemiesOnOurSide = Enemies.Where(
                                                            n => PositionHandling.IsPositionOnOurSide(n.StartPosition));

                foreach (var @char in enemiesOnOurSide)
                {
                    healthAmount += @char.HealthComponent.Field8;
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

        public static Character EnemyLeftPrincessTower
        {
            get
            {
                return EnemyPrincessTower.FirstOrDefault();
            }
        }

        public static Character EnemyRightPrincessTower
        {
            get
            {
                return EnemyPrincessTower.LastOrDefault();
            }
        }

        public static Character GetEnemyPrincessTowerWithLowestHealth(uint ownerIndex)
        {
            var om = ClashEngine.Instance.ObjectManager;
            var chars = om.OfType<Character>();
            var princessTower = chars.Where(n => n.LogicGameObjectData.Name.Value == "PrincessTower" &&
                                            n.OwnerIndex == ownerIndex).OrderBy
                                            (n => n.HealthComponent.Field8).FirstOrDefault();

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

                    if (@char.OwnerIndex != StaticValues.Player.OwnerIndex && PositionHandling.IsPositionOnOurSide(@char.StartPosition))
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

        #endregion

        /*
        public static void AddCardsToEnemieDeck()
        {
            var om = ClashEngine.Instance.ObjectManager;
            var chars = om.OfType<Character>().Where(n => n.OwnerIndex != StaticValues.Player.OwnerIndex);
            Enemie.AddCardToDeck(chars);
        }
        */

        public void LogCharInformations()
        {
            var om = ClashEngine.Instance.ObjectManager;
            var chars = om.OfType<Character>();

            foreach (var @char in chars)
            {
                var data = @char.LogicGameObjectData;
                if (data != null && data.IsValid)
                {
                    var charName = data.Name.Value;
                    var isFlying = data.FlyFromGround != 0;
                    var attacksAir = data.AttacksAir != 0;
                    var attacksGround = data.AttacksGround != 0;
                    var collisionRadius = data.CollisionRadius;
                    Logger.Debug("Found Character with owner {OwnerIndex} name {charName} AttacksAir {attacksAir} startposition {StartPosition} " +
                                    "mana {Mana} areabuff {AreaBuffRadius} collisionRadius {collisionRadius} " +
                                    "Health {Health} Shield {Shield} Field14 {Field14} Field1C {Field1C} Field8 {Field8} Range {Range} FlyFromGround {FlyFromGround} FlyingHeight {FlyingHeight}" +
                                    " GameObjects-Count {LogicGameObjectManager}",
                        @char.OwnerIndex, charName, attacksAir, @char.StartPosition, @char.Mana, data.AreaBuffRadius, 
                        collisionRadius, @char.HealthComponent.Health, @char.HealthComponent.ShieldHealth, @char.HealthComponent.Field14,
                        @char.HealthComponent.Field1C, @char.HealthComponent.Field8,
                        data.Range, data.FlyFromGround, data.FlyingHeight, @char.LogicGameObjectManager.GameObjects.Count);

                }
            }
        }
    }
}
