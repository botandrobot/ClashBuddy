using System;
using System.Collections.Generic;
using System.Text;
using Buddy.Clash.Engine.NativeObjects.Logic.GameObjects;

namespace Buddy.Clash.DefaultSelectors.Utilities
{
    // ToDo: Rebuild the enemies deck
    class EnemyCardHandling
    {
        public static Dictionary<String, Character> enemiesDeck = new Dictionary<string, Character>();

        // ToDo: Use Card counting to build the enemies hand
        public static Dictionary<String, Character> enemiesHand = new Dictionary<string, Character>();

        public static void AddCardToDeck(IEnumerable<Character> characters)
        {
            if (enemiesDeck.Count == 8)
                return;

            foreach (var @char in characters)
            {
                if (!enemiesDeck.ContainsKey(@char.LogicGameObjectData.Name.Value))
                    enemiesDeck.Add(@char.LogicGameObjectData.Name.Value, @char);
            }
        }

    }
}
