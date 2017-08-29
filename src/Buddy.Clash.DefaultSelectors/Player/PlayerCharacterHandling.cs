using Buddy.Clash.DefaultSelectors.Utilities;
using Buddy.Clash.Engine;
using Buddy.Clash.Engine.NativeObjects.Logic.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Buddy.Clash.DefaultSelectors.Player
{
    class PlayerCharacterHandling
    {
        public static void Reset()
        {
            leftPrincessTower = PrincessTower.FirstOrDefault();
            rightPrincessTower = PrincessTower.LastOrDefault();
        }

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

        private static Character leftPrincessTower;
        public static Character LeftPrincessTower
        {
            get
            {
                Character firstPrincessTower = PrincessTower.FirstOrDefault();

                if (leftPrincessTower == null)
                    leftPrincessTower = firstPrincessTower;

                // If the position is not equals, it means the LeftPrincessTower is already destroyed
                if (!firstPrincessTower.StartPosition.Equals(leftPrincessTower.StartPosition))
                    return null;

                return firstPrincessTower;
            }
        }

        private static Character rightPrincessTower;
        public static Character RightPrincessTower
        {
            get
            {
                Character lastPrincessTower = PrincessTower.LastOrDefault();

                if (rightPrincessTower == null)
                    rightPrincessTower = lastPrincessTower;

                // If the position is not equals, it means the LeftPrincessTower is already destroyed
                if (!lastPrincessTower.StartPosition.Equals(rightPrincessTower.StartPosition))
                    return null;

                return lastPrincessTower;
            }
        }

        public static int HowManyCharactersAroundCharacter(Character @char)
        {
            int boarderX = 1000;
            int boarderY = 1000;
            IEnumerable<Character> playerCharacter = PlayerCharacterHandling.Troop;
            IEnumerable<Character> characterAround;

            characterAround = playerCharacter.Where(n => n.StartPosition.X > @char.StartPosition.X - boarderX
                                            && n.StartPosition.X < @char.StartPosition.X + boarderX &&
                                            n.StartPosition.Y > @char.StartPosition.Y - boarderY &&
                                            n.StartPosition.Y < @char.StartPosition.Y + boarderY);

            return characterAround.Count();
        }

        public static IEnumerable<Character> Troop
        {
            get
            {
                var om = ClashEngine.Instance.ObjectManager;
                var chars = om.OfType<Character>();
                var troop = chars.Where(n => n.LogicGameObjectData.Name.Value != "PrincessTower" &&
                                                n.LogicGameObjectData.Name.Value != "KingTower" &&
                                                n.OwnerIndex == StaticValues.Player.OwnerIndex);
                return troop;

            }
        }
    }
}
