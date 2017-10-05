using Robi.Common;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Robi.Clash.DefaultSelectors.Apollo
{
    class PlayfieldAnalyse
    {
        // ToDo: Use ATK per second btw: Whats with tower damage?
        // ToDo: Involve KT in analyses
        public static readonly ILogger Logger = LogProvider.CreateLogger<Decision>();
        public static Line[] lines;

        public static void AnalyseLines(Playfield p)
        {
            lines = new Line[2];
            lines[0] = new Line();
            lines[1] = new Line();

            Tower(p);
            Minions(p);
            Comparision();
            Level();
        }

        private static void Tower(Playfield p)
        {
            // TODO: Level must be fixed, is everytime 0
            lines[0].OwnPtMaxHp = (int)Helper.LevelMultiplicator(p.ownPrincessTower1.MaxHP, p.ownPrincessTower1.level);
            lines[1].OwnPtMaxHp = (int)Helper.LevelMultiplicator(p.ownPrincessTower2.MaxHP, p.ownPrincessTower2.level);

            // ToDo: Calc with lvl
            lines[0].OwnPtAtk = p.ownPrincessTower1.Atk;
            lines[1].OwnPtAtk = p.ownPrincessTower2.Atk;

            #region TowerAnalyses
            double oPtHpL1 = Helper.Quotient(p.ownPrincessTower1.HP, lines[0].OwnPtMaxHp) * 100;
            double oPtHpL2 = Helper.Quotient(p.ownPrincessTower2.HP, lines[1].OwnPtMaxHp) * 100;
            double ePtHpL1 = Helper.Quotient(p.enemyPrincessTower1.HP, lines[0].OwnPtMaxHp) * 100;
            double ePtHpL2 = Helper.Quotient(p.enemyPrincessTower2.HP, lines[1].OwnPtMaxHp) * 100;

            if (oPtHpL1 == 0) lines[0].OwnPtHp = Apollo.Level.ZERO;
            else if (oPtHpL1 <= 30) lines[0].OwnPtHp = Apollo.Level.LOW;
            else if (oPtHpL1 <= 70) lines[0].OwnPtHp = Apollo.Level.MEDIUM;
            else lines[0].OwnPtHp = Apollo.Level.HIGH;

            if (oPtHpL2 == 0) lines[1].OwnPtHp = Apollo.Level.ZERO;
            else if (oPtHpL2 <= 30) lines[1].OwnPtHp = Apollo.Level.LOW;
            else if (oPtHpL2 <= 70) lines[1].OwnPtHp = Apollo.Level.MEDIUM;
            else lines[1].OwnPtHp = Apollo.Level.HIGH;
            #endregion
        }

        private static void Minions(Playfield p)
        {
            #region minion sums (atk and health; Line 1 and 2)
            IEnumerable<BoardObj> enemyMinionsL1 = p.enemyMinions.Where(n => n.Line == 1);
            IEnumerable<BoardObj> enemyMinionsL2 = p.enemyMinions.Where(n => n.Line == 2);

            lines[0].EnemyMinionAtk = enemyMinionsL1.Sum(n => n.Atk);
            lines[0].EnemyMinionHP = enemyMinionsL1.Sum(n => n.HP);
            lines[1].EnemyMinionAtk = enemyMinionsL2.Sum(n => n.Atk);
            lines[1].EnemyMinionHP = enemyMinionsL2.Sum(n => n.HP);

            IEnumerable<BoardObj> ownMinionsL1 = p.ownMinions.Where(n => n.Line == 1);
            IEnumerable<BoardObj> ownMinionsL2 = p.ownMinions.Where(n => n.Line == 2);

            lines[0].OwnMinionHP = ownMinionsL1.Sum(n => n.HP);
            lines[0].OwnMinionAtk = ownMinionsL1.Sum(n => n.Atk);
            lines[1].OwnMinionHP = ownMinionsL2.Sum(n => n.HP);
            lines[1].OwnMinionAtk = ownMinionsL2.Sum(n => n.Atk);
            #endregion

            lines[0].OwnMinionCount = ownMinionsL1.Count();
            lines[1].OwnMinionCount = ownMinionsL2.Count();
            lines[0].EnemyMinionCount = enemyMinionsL1.Count();
            lines[1].EnemyMinionCount = enemyMinionsL2.Count();
        }

        private static void Comparision()
        {
            lines[0].ComparisionHP = lines[0].OwnMinionHP - lines[0].EnemyMinionHP;
            lines[0].ComparisionAtk = lines[0].OwnMinionAtk - lines[0].EnemyMinionAtk;

            lines[1].ComparisionHP = lines[1].OwnMinionHP - lines[1].EnemyMinionHP;
            lines[1].ComparisionAtk = lines[1].OwnMinionAtk - lines[1].EnemyMinionAtk;
        }

        private static void Level()
        {
            lines[0].Danger = GetDangerLevel(0);
            lines[1].Danger = GetDangerLevel(1);

            lines[0].Chance = GetChanceLevel(0);
            lines[1].Chance = GetChanceLevel(1);
        }

        private static Level GetDangerLevel(int line)
        {
            int dangerLevel = 0, dangerLvlHP = 0, dangerLvlAtk = 0, dangerLvlTower = 0;
            int sensitivity = (int)Setting.DangerSensitivity;
            sensitivity = 2; // Remove: Just for debugging
            int enemyMinionHP = lines[line].EnemyMinionHP;
            int enemyMinionAtk = lines[line].EnemyMinionAtk;

            #region Minion HP
            if (enemyMinionHP != 0)
            {
                if (enemyMinionHP > (lines[line].OwnPtMaxHp / (2 * sensitivity)))
                    dangerLvlHP += 3;
                else if (enemyMinionHP > (lines[line].OwnPtMaxHp / (3 * sensitivity)))
                    dangerLvlHP += 2;
                else if (enemyMinionHP > (lines[line].OwnPtMaxHp / (4 * sensitivity)))
                    dangerLvlHP += 1;

                // ToDo: Use this, but Atk is zero at the moment
                //if (enemyMinionHP < -(lines[line].OwnPtAtk * sensitivity * 2))
                //    dangerLvlHP += 3;
                //else if (enemyMinionHP < -(lines[line].OwnPtAtk * sensitivity * 1.5))
                //    dangerLvlHP += 2;
                //else if (enemyMinionHP < -(lines[line].OwnPtAtk * sensitivity))
                //    dangerLvlHP += 1;
            }
            #endregion

            #region Minion Atk
            if (enemyMinionAtk != 0)
            {
                if (enemyMinionAtk > (lines[line].OwnPtMaxHp / (5 * sensitivity)))
                    dangerLvlAtk += 3;
                else if (enemyMinionAtk > (lines[line].OwnPtMaxHp / (10 * sensitivity)))
                    dangerLvlAtk += 2;
                else if (enemyMinionAtk > (lines[line].OwnPtMaxHp / (15 * sensitivity)))
                    dangerLvlAtk += 1;
            }
            #endregion

            #region PrincessTower HP
            //switch (lines[line].OwnPtHp)
            //{
            //    case Apollo.Level.LOW:
            //        dangerLvlTower += 1;
            //        break;
            //    case Apollo.Level.MEDIUM:
            //        dangerLvlTower += 2;
            //        break;
            //    case Apollo.Level.HIGH:
            //        dangerLvlTower += 3;
            //        break;
            //    default:
            //        break;
            //}
            #endregion

            Logger.Debug("Danger-Analyses-Level");
            Logger.Debug("Atk       :" + dangerLvlAtk);
            Logger.Debug("HP        :" + dangerLvlHP);
            Logger.Debug("Tower-HP  :" + dangerLvlTower);
            Logger.Debug("Danger-Analyses-End");

            dangerLevel = dangerLvlAtk + dangerLvlHP + dangerLvlTower;
            // Maybe round up
            return (Level)(dangerLevel / 2);
        }

        private static Level GetChanceLevel(int line)
        {
            int chanceLevel = 0, chanceLvlHP = 0, chanceLvlAtk = 0, chanceLvlTower = 0;
            int sensitivity = (int)Setting.ChanceSensitivity;
            sensitivity = 2; // Remove: Just for debugging
            int ownMinionHP = lines[line].OwnMinionHP;
            int ownMinionAtk = lines[line].OwnMinionAtk;

            #region Minion HP
            if (ownMinionHP != 0)
            {
                if (ownMinionHP > (lines[line].OwnPtMaxHp / (2 * sensitivity)))
                    chanceLvlHP += 3;
                else if (ownMinionHP > (lines[line].OwnPtMaxHp / (3 * sensitivity)))
                    chanceLvlHP += 2;
                else if (ownMinionHP > (lines[line].OwnPtMaxHp / (4 * sensitivity)))
                    chanceLvlHP += 1;

            }
            #endregion

            #region Minion Atk
            if (ownMinionAtk != 0)
            {
                if (ownMinionAtk > (lines[line].OwnPtMaxHp / (5 * sensitivity)))
                    chanceLvlAtk += 3;
                else if (ownMinionAtk > (lines[line].OwnPtMaxHp / (10 * sensitivity)))
                    chanceLvlAtk += 2;
                else if (ownMinionAtk > (lines[line].OwnPtMaxHp / (15 * sensitivity)))
                    chanceLvlAtk += 1;


            }
            #endregion

            #region PrincessTower HP
            //switch (lines[line].EnemyPtHp)
            //{
            //    case Apollo.Level.LOW:
            //        chanceLvlTower += 1;
            //        break;
            //    case Apollo.Level.MEDIUM:
            //        chanceLvlTower += 2;
            //        break;
            //    case Apollo.Level.HIGH:
            //        chanceLvlTower += 3;
            //        break;
            //    default:
            //        break;
            //}
            #endregion

            Logger.Debug("Chance-Analyses-Level");
            Logger.Debug("Atk       :" + chanceLvlAtk);
            Logger.Debug("HP        :" + chanceLvlHP);
            Logger.Debug("Tower-HP  :" + chanceLvlTower);
            Logger.Debug("Chance-Analyses-End");

            chanceLevel = chanceLvlAtk + chanceLvlHP + chanceLvlTower;

            return (Level)(chanceLevel / 2);
        }

    }
}
