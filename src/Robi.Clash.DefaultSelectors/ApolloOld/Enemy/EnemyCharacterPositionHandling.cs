using Robi.Clash.DefaultSelectors.Utilities;
using Robi.Clash.Engine.NativeObjects.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Robi.Clash.DefaultSelectors.Enemy
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

        public static Vector2f EnemyLeftPrincessTower { get; set; }

        public static Vector2f EnemyRightPrincessTower { get; set; }

    }
}
