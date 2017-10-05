using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Robi.Clash.DefaultSelectors.Apollo
{
    class Helper
    {
        public static int? HowManyCharactersAroundCharacter(Playfield p, BoardObj obj)
        {
            int boarderX = 1000;
            int boarderY = 1000;
            IEnumerable<BoardObj> playerCharacter = p.ownMinions;
            IEnumerable<BoardObj> characterAround;

            characterAround = playerCharacter.Where(n => n.Position.X > obj.Position.X - boarderX
                                            && n.Position.X < obj.Position.X + boarderX &&
                                            n.Position.Y > obj.Position.Y - boarderY &&
                                            n.Position.Y < obj.Position.Y + boarderY);

            if (characterAround == null)
                return null;

            return characterAround.Count();
        }

        // NF = not flying
        public static int? HowManyNFCharactersAroundCharacter(Playfield p, BoardObj obj)
        {
            int boarderX = 1000;
            int boarderY = 1000;
            IEnumerable<BoardObj> playerCharacter = p.ownMinions;
            IEnumerable<BoardObj> characterAround;

            characterAround = playerCharacter.Where(n => n.Position.X > obj.Position.X - boarderX
                                            && n.Position.X < obj.Position.X + boarderX &&
                                            n.Position.Y > obj.Position.Y - boarderY &&
                                            n.Position.Y < obj.Position.Y + boarderY && n.card.Transport == transportType.GROUND);

            if (characterAround == null)
                return null;

            return characterAround.Count();
        }

        public static BoardObj EnemyCharacterWithTheMostEnemiesAround(Playfield p, out int count, transportType tP)
        {
            int boarderX = 1000;
            int boarderY = 1000;
            IEnumerable<BoardObj> enemies = p.enemyMinions;
            IEnumerable<BoardObj> enemiesAroundTemp;
            BoardObj enemy = null;
            count = 0;

            foreach (var item in enemies)
            {
                if (tP != transportType.NONE)
                {
                    enemiesAroundTemp = enemies.Where(n => n.Position.X > item.Position.X - boarderX
                                                    && n.Position.X < item.Position.X + boarderX &&
                                                    n.Position.Y > item.Position.Y - boarderY &&
                                                    n.Position.Y < item.Position.Y + boarderY && n.Transport == tP);
                }
                else
                {
                    enemiesAroundTemp = enemies.Where(n => n.Position.X > item.Position.X - boarderX
                                && n.Position.X < item.Position.X + boarderX &&
                                n.Position.Y > item.Position.Y - boarderY &&
                                n.Position.Y < item.Position.Y + boarderY);
                }

                if (enemiesAroundTemp?.Count() > count)
                {
                    count = enemiesAroundTemp.Count();
                    enemy = item;
                }
            }

            return enemy;
        }

        public static BoardObj GetNearestEnemy(Playfield p)
        {
            var nearestChar = p.enemyMinions;

            var orderedChar = nearestChar.OrderBy(n => n.Position.Y);

            if (p.home)
                return orderedChar.FirstOrDefault();
            else
                return orderedChar.LastOrDefault();
        }

        public static bool IsObjectAtOtherSide(Playfield p, BoardObj bo)
        {
            if (p.home && bo.own || !p.home && !bo.own)
            {
                if (bo.Position.Y >= MiddleLineY(p))
                    return true;
                else
                    return false;
            }
            else
            {
                if (bo.Position.Y <= MiddleLineY(p))
                    return true;
                else
                    return false;
            }
        }

        public static int MiddleLineY(Playfield p)
        {
            return (p.ownKingsTower.Position.Y +
                    p.enemyKingsTower.Position.Y) / 2;

        }

        public static bool IsAnEnemyObjectInArea(Playfield p, VectorAI position, int areaSize, boardObjType type)
        {
            Func<BoardObj, bool> whereClause = n => n.Position.X >= position.X - areaSize && n.Position.X <= position.X + areaSize &&
                                                    n.Position.Y >= position.Y - areaSize && n.Position.Y <= position.Y + areaSize;


            if (type == boardObjType.MOB)
                return p.enemyMinions.Where(whereClause).Count() > 0;
            else if (type == boardObjType.BUILDING)
                return p.enemyBuildings.Where(whereClause).Count() > 0;
            else if (type == boardObjType.AOE)
                return p.enemyAreaEffects.Where(whereClause).Count() > 0;

            return false;
        }

        public static double LevelMultiplicator(int value, int level)
        {
            return value *  Math.Pow((1 + 1.1d / 100), level);
        }

        public static double Quotient(int a, double b)
        {
            if (a == 0 || b == 0)
                return 0;

            return a / b;
        }

        public static VectorAI DeployBehindTank(Playfield p, int line)
        {
            IEnumerable<BoardObj> tankChar = p.ownMinions.Where(n => n.Line == line && n.card.MaxHP >= Setting.MinHealthAsTank);

            if (tankChar.FirstOrDefault() != null)
                return p.getDeployPosition(tankChar.FirstOrDefault(), deployDirectionRelative.Down);
            else
                return null;

        }
    }
}
