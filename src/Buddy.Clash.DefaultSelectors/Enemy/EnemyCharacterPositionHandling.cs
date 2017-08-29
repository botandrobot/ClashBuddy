using Buddy.Clash.DefaultSelectors.Utilities;
using Buddy.Clash.Engine.NativeObjects.Native;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buddy.Clash.DefaultSelectors.Enemy
{
    class EnemyCharacterPositionHandling
    {
        public static void Reset()
        {
            enemyLeftPrincessTower = EnemyCharacterHandling.EnemyLeftPrincessTower.StartPosition;
            enemyRightPrincessTower = EnemyCharacterHandling.EnemyRightPrincessTower.StartPosition;
        }

        public static Vector2f GetPositionOfTheMostDangerousAttack()
        {
            // comes later
            return Vector2f.Zero;
        }

        private static Vector2f enemyLeftPrincessTower = Vector2f.Zero;
        public static Vector2f EnemyLeftPrincessTower
        {
            get
            {
                if (enemyLeftPrincessTower.Equals(Vector2f.Zero))
                    enemyLeftPrincessTower = EnemyCharacterHandling.EnemyLeftPrincessTower.StartPosition;

                return enemyLeftPrincessTower;
            }
        }

        private static Vector2f enemyRightPrincessTower = Vector2f.Zero;
        public static Vector2f EnemyRightPrincessTower
        {
            get
            {
                if (enemyRightPrincessTower.Equals(Vector2f.Zero))
                    enemyRightPrincessTower = EnemyCharacterHandling.EnemyRightPrincessTower.StartPosition;

                return enemyRightPrincessTower;
            }
        }
    }
}
