using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Robi.Clash.DefaultSelectors.Apollo
{
    class PlayfieldAnalyse
    {
        public static Line[] lines;

        public static void AnalyseLines(Playfield p)
        {
            

            Line[] lines = new Line[2];
            lines[0] = new Line();
            lines[1] = new Line();

            #region TowerAnalyses
            double oPtHpL1 = Helper.Quotient(p.ownPrincessTower1.HP, Helper.LevelMultiplicator(p.ownPrincessTower1.MaxHP, p.ownPrincessTower1.level)) * 100;
            double oPtHpL2 = Helper.Quotient(p.ownPrincessTower2.HP, Helper.LevelMultiplicator(p.ownPrincessTower2.MaxHP,p.ownPrincessTower2.level)) * 100;
            double ePtHpL1 = Helper.Quotient(p.enemyPrincessTower1.HP, Helper.LevelMultiplicator(p.enemyPrincessTower1.MaxHP,p.enemyPrincessTower1.level)) * 100;
            double ePtHpL2 = Helper.Quotient(p.enemyPrincessTower2.HP, Helper.LevelMultiplicator(p.enemyPrincessTower2.MaxHP,p.enemyPrincessTower2.level)) * 100;

            // TODO: Level must be fixed, is everytime 0
            if (oPtHpL1 == 0) lines[0].OwnPtHp = Level.ZERO;
            else if (oPtHpL1 <= 30) lines[0].OwnPtHp = Level.LOW;
            else if (oPtHpL1 <= 70) lines[0].OwnPtHp = Level.MEDIUM;
            else lines[0].OwnPtHp = Level.HIGH;

            if (oPtHpL2 == 0) lines[1].OwnPtHp = Level.ZERO;
            else if (oPtHpL2 <= 30) lines[1].OwnPtHp = Level.LOW;
            else if (oPtHpL2 <= 70) lines[1].OwnPtHp = Level.MEDIUM;
            else lines[1].OwnPtHp = Level.HIGH;
            #endregion


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

            PlayfieldAnalyse.lines = lines;
        }

    }
}
