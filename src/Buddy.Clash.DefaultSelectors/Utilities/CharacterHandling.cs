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
                uint ownerIndex = ClashEngine.Instance.LocalPlayer.OwnerIndex;

                IEnumerable<Character> enemies = chars.Where(
                                                            n => n.OwnerIndex != ownerIndex);

                return enemies;
            }
        }

        public static Character NearestEnemy
        {
            get
            {
                uint ownerIndex = ClashEngine.Instance.LocalPlayer.OwnerIndex;
                var om = ClashEngine.Instance.ObjectManager;
                var chars = om.OfType<Character>();
                var nearestChar = chars.Where(n => n.LogicGameObjectData.Name.Value != "PrincessTower" &&
                                                n.OwnerIndex != ownerIndex);

                var orderedChar = nearestChar.OrderBy(n => n.StartPosition.Y);

                if (ownerIndex == 0)
                {
                    Log.Debug("Nearest enemy-char: " + orderedChar.FirstOrDefault().LogicGameObjectData.Name);
                    return orderedChar.FirstOrDefault();
                }
                else
                {
                    Log.Debug("Nearest enemy-char: " + orderedChar.LastOrDefault().LogicGameObjectData.Name);
                    return orderedChar.LastOrDefault();
                }
            }
        }

        public static Character EnemieWithTheMostEnemiesAround(out int count)
        {
            int boarderX = 1000;
            int boarderY = 1000;
            IEnumerable<Character> enemies = Enemies;
            IEnumerable<Character> enemiesAroundTemp;
            Character enemie = null;
            count = 0;

            foreach (var item in enemies)
            {
                enemiesAroundTemp = enemies.Where(n => n.StartPosition.X > item.StartPosition.X - boarderX
                                                && n.StartPosition.X < item.StartPosition.X + boarderX &&
                                                n.StartPosition.Y > item.StartPosition.Y - boarderY &&
                                                n.StartPosition.Y < item.StartPosition.Y + boarderY);

                if(enemiesAroundTemp.Count() > count)
                {
                    count = enemiesAroundTemp.Count();
                    enemie = item;
                }
            }

            return enemie;
        }

        public static IEnumerable<Character> PrincessTower
        {
            get
            {
                var player = ClashEngine.Instance.LocalPlayer;
                var om = ClashEngine.Instance.ObjectManager;
                var chars = om.OfType<Character>();
                var princessTower = chars.Where(n => n.LogicGameObjectData.Name.Value == "PrincessTower" &&
                                                n.OwnerIndex == player.OwnerIndex).OrderBy(n => n.StartPosition.X);

                foreach (var s in princessTower)
                {
                    Log.Debug("PrincessTower: Owner - {0}; Position: {1}",
                                s.OwnerIndex, s.StartPosition);
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

        public static IEnumerable<Character> EnemiePrincessTower
        {
            get
            {
                var player = ClashEngine.Instance.LocalPlayer;
                var om = ClashEngine.Instance.ObjectManager;
                var chars = om.OfType<Character>();
                var princessTower = chars.Where(n => n.LogicGameObjectData.Name.Value == "PrincessTower" &&
                                                n.OwnerIndex != player.OwnerIndex).OrderBy(n => n.StartPosition.X);

                foreach (var s in princessTower)
                {
                    Log.Debug("PrincessTower: Owner - {0}; Position: {1}",
                                s.OwnerIndex, s.StartPosition);
                }
                return princessTower;
            }
        }

        public static Character KingTower
        {
            get
            {
                return ClashEngine.Instance.Battle.SummonerTowers.Where(n =>
                                            n.OwnerIndex == ClashEngine.Instance.LocalPlayer.OwnerIndex).FirstOrDefault();
            }
        }

        public static Character EnemieKingTower
        {
            get
            {
                return ClashEngine.Instance.Battle.SummonerTowers.Where(n =>
                                            n.OwnerIndex != ClashEngine.Instance.LocalPlayer.OwnerIndex).FirstOrDefault();
            }
        }

        public static Character GetEnemyPrincessTowerWithLowestHealth(uint ownerIndex)
        {
            // !! Not working !! No value for health bar
            //
            var om = ClashEngine.Instance.ObjectManager;
            var chars = om.OfType<Character>();
            var princessTower = chars.Where(n => n.LogicGameObjectData.Name.Value == "PrincessTower" &&
                                            n.OwnerIndex == ownerIndex).OrderBy
                                            (n => n.HealthComponent.Health).FirstOrDefault();
            
                Log.Debug("PrincessTower: Owner - {0}; Position: {1}",
                            princessTower.OwnerIndex, princessTower.HealthComponent.Health);

            return princessTower;
        }



        public static bool IsEnemyOnOurSide()
        {
            var om = ClashEngine.Instance.ObjectManager;
            var chars = om.OfType<Character>();

            foreach (var @char in chars)
            {
                var data = @char.LogicGameObjectData;
                if (data != null && data.IsValid)
                {
                    Logger.Debug("IsPositionOnOurSide: " + PositionHandling.IsPositionOnOurSide(@char.StartPosition));

                    if (@char.OwnerIndex != ClashEngine.Instance.LocalPlayer.OwnerIndex && PositionHandling.IsPositionOnOurSide(@char.StartPosition))
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
                                    "HealthBar {HealthNumber} Health {Health} Range {Range} FlyFromGround {FlyFromGround} FlyingHeight {FlyingHeight}" +
                                    " GameObjects-Count {LogicGameObjectManager}",
                        @char.OwnerIndex, charName, attacksAir, @char.StartPosition, @char.Mana, data.AreaBuffRadius, 
                        collisionRadius, @char.LogicGameObjectData.ShowHealthNumber, @char.HealthComponent.Health,
                        data.Range, data.FlyFromGround, data.FlyingHeight, @char.LogicGameObjectManager.GameObjects.Count);

                }
            }
        }
    }
}
