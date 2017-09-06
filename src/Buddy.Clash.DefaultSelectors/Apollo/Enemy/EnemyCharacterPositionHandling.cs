using Buddy.Clash.DefaultSelectors.Utilities;
using Buddy.Clash.Engine.NativeObjects.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Buddy.Clash.DefaultSelectors.Enemy
{
    class EnemyCharacterPositionHandling
    {
        public static void SetPositions()
        {
            EnemyLeftPrincessTower = EnemyCharacterHandling.EnemyPrincessTower.FirstOrDefault().StartPosition;
            EnemyRightPrincessTower = EnemyCharacterHandling.EnemyPrincessTower.LastOrDefault().StartPosition;
        }

        public static Vector2f GetPositionOfTheMostDangerousAttack()
        {
            // comes later
            return Vector2f.Zero;
        }

        public static Vector2 EnemyLeftPrincessTower { get; set; }

        public static Vector2 EnemyRightPrincessTower { get; set; }

    }
}
