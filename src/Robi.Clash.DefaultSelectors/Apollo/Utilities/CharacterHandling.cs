using Robi.Clash.Engine;
using Robi.Clash.Engine.NativeObjects.Logic.GameObjects;
using Robi.Common;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Robi.Clash.DefaultSelectors.Utilities
{
    class CharacterHandling
    {
        private static readonly ILogger Logger = LogProvider.CreateLogger<CharacterHandling>();

       
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
                                    "Health {Health} Shield {Shield} CurrentHealth {CurrentHealth} Range {Range} FlyFromGround {FlyFromGround} FlyingHeight {FlyingHeight}" +
                                    " GameObjects-Count {LogicGameObjectManager}",
                        @char.OwnerIndex, charName, attacksAir, @char.StartPosition, @char.Mana, data.AreaBuffRadius, 
                        collisionRadius, @char.HealthComponent.Health, @char.HealthComponent.ShieldHealth, @char.HealthComponent.CurrentHealth,
                        data.Range, data.FlyFromGround, data.FlyingHeight, @char.LogicGameObjectManager.GameObjects.Count);

                }
            }
        }
    }
}
