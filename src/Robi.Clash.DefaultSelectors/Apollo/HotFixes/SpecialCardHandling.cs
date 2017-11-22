using System;
using System.Collections.Generic;
using System.Text;

namespace Robi.Clash.DefaultSelectors.Apollo.HotFixes
{
    class SpecialPositionHandling
    {
        public static VectorAI GetPosition(Playfield p, Handcard hc)
        {
            if(hc.name == "Goblin Barrel")
            {
                return (p.enemyKingsTower.HP < p.enemyPrincessTower1.HP && p.enemyKingsTower.HP < p.enemyPrincessTower2.HP) 
                        ? p.enemyKingsTower.Position
                        : (p.enemyPrincessTower1.HP < p.enemyPrincessTower2.HP) 
                        ? p.enemyPrincessTower1.Position 
                        : p.enemyPrincessTower2.Position;
            }

            return null;
        }
    }
}
