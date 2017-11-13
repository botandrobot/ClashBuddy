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

            var characterAround = playerCharacter.Count(n => n.Position.X > obj.Position.X - boarderX
                                                                     && n.Position.X < obj.Position.X + boarderX &&
                                                                     n.Position.Y > obj.Position.Y - boarderY &&
                                                                     n.Position.Y < obj.Position.Y + boarderY);
            return characterAround;
        }

        // NF = not flying
        public static int? HowManyNFCharactersAroundCharacter(Playfield p, BoardObj obj)
        {
            int boarderX = 1000;
            int boarderY = 1000;
            IEnumerable<BoardObj> playerCharacter = p.ownMinions;

            var characterAround = playerCharacter.Count(n => n.Position.X > obj.Position.X - boarderX
                                                                     && n.Position.X < obj.Position.X + boarderX &&
                                                                     n.Position.Y > obj.Position.Y - boarderY &&
                                                                     n.Position.Y < obj.Position.Y + boarderY && n.card.Transport == transportType.GROUND);
            
            return characterAround;
        }

        public static BoardObj EnemyCharacterWithTheMostEnemiesAround(Playfield p, out int count, transportType tP)
        {
            int boarderX = 1000;
            int boarderY = 1000;
            IEnumerable<BoardObj> enemies = p.enemyMinions;
            BoardObj enemy = null;
            count = 0;

            foreach (var item in enemies)
            {
                BoardObj[] enemiesAroundTemp;
                if (tP != transportType.NONE)
                {
                    enemiesAroundTemp = enemies.Where(n => n.Position.X > item.Position.X - boarderX
                                                    && n.Position.X < item.Position.X + boarderX &&
                                                    n.Position.Y > item.Position.Y - boarderY &&
                                                    n.Position.Y < item.Position.Y + boarderY && n.Transport == tP).ToArray();
                }
                else
                {
                    enemiesAroundTemp = enemies.Where(n => n.Position.X > item.Position.X - boarderX
                                && n.Position.X < item.Position.X + boarderX &&
                                n.Position.Y > item.Position.Y - boarderY &&
                                n.Position.Y < item.Position.Y + boarderY).ToArray();
                }

                if (!(enemiesAroundTemp?.Count() > count)) continue;

                count = enemiesAroundTemp.Count();
                enemy = item;
            }

            return enemy;
        }

        public static BoardObj GetNearestEnemy(Playfield p)
        {
            var nearestChar = p.enemyMinions;

            var orderedChar = nearestChar.OrderBy(n => n.Position.Y);

            return p.home ? orderedChar.FirstOrDefault() : orderedChar.LastOrDefault();
        }
        
        public static bool IsAnEnemyObjectInArea(Playfield p, VectorAI position, int areaSize, boardObjType type)
        {
            bool WhereClause(BoardObj n) => n.Position.X >= position.X - areaSize && n.Position.X <= position.X + areaSize && n.Position.Y >= position.Y - areaSize && n.Position.Y <= position.Y + areaSize;


            if (type == boardObjType.MOB)
                return p.enemyMinions.Where(WhereClause).Any();
            else if (type == boardObjType.BUILDING)
                return p.enemyBuildings.Where(WhereClause).Any();
            else if (type == boardObjType.AOE)
                return p.enemyAreaEffects.Where(WhereClause).Any();

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
            var tankChar = p.ownMinions.Where(n => n.Line == line && n.HP >= Setting.MinHealthAsTank).OrderBy(n => n.HP).FirstOrDefault();

            return tankChar != null ? p.getDeployPosition(tankChar, deployDirectionRelative.Down) : null;
        }

        public static VectorAI DeployTankInFront(Playfield p, int line)
        {
            var ownChar = p.ownMinions.Where(n => n.Line == line && n.MaxHP < Setting.MinHealthAsTank).OrderBy(n => n.Position.Y).ToArray();
            var lc = ownChar.LastOrDefault();
            var fc = ownChar.FirstOrDefault();

            if (p.home)
            {
                return lc != null ? p.getDeployPosition(lc, deployDirectionRelative.Up) : null;
            }
            else
            {
                return fc != null ? p.getDeployPosition(fc, deployDirectionRelative.Up) : null;
            }

        }
    }
}
