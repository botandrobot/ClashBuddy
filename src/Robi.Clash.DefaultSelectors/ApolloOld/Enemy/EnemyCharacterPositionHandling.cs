using Robi.Clash.DefaultSelectors.Utilities;
using Robi.Clash.Engine.NativeObjects.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Robi.Clash.Engine.NativeObjects.Logic.GameObjects;

namespace Robi.Clash.DefaultSelectors.Enemy
{
    class EnemyCharacterPositionHandling
    {
        public static void SetPositions()
        {
            var ept = EnemyCharacterHandling.EnemyPrincessTower as Character[] ?? EnemyCharacterHandling.EnemyPrincessTower.ToArray();

            var lpt = ept.FirstOrDefault();
            var rpt = ept.LastOrDefault();

            if (lpt != null)
            {
                EnemyLeftPrincessTower = lpt.StartPosition;
            }
            if (rpt != null)
            {
                EnemyRightPrincessTower = rpt.StartPosition;
            }
        }

        public static Vector2 GetPositionOfTheMostDangerousAttack()
        {
            // comes later
            return Vector2.Zero;
        }

        public static Vector2 EnemyLeftPrincessTower { get; set; }

        public static Vector2 EnemyRightPrincessTower { get; set; }

    }
}
