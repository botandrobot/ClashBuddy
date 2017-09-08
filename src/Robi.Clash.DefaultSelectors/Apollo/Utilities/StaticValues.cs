using Robi.Clash.Engine;
using System;
using System.Linq;

namespace Robi.Clash.DefaultSelectors.Utilities
{
    class StaticValues
    {
        public static Random rnd = new Random();

        public static Engine.NativeObjects.Logic.GameObjects.Player Player
        {
            get{ return ClashEngine.Instance.LocalPlayer; }
        }

        private static int playerCount;
        public static int PlayerCount
        {
            set
            {
                playerCount = value;
            }
            get
            {
                if (playerCount == 0)
                    playerCount = ClashEngine.Instance.Battle.SummonerTowers.Where(n => n.StartPosition.X != 0).Count();

                return playerCount;
            }
        }
    }
}
