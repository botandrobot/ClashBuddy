namespace Robi.Clash.DefaultSelectors
{
    using System;
    using System.Collections.Generic;

    public class TimeMachine
    {

        private static TimeMachine instance;

        public static TimeMachine Instance
        {
            get
            {
                return instance ?? (instance = new TimeMachine());
            }
        }

        private TimeMachine()
        {

        }

        public void setTimeShift(Playfield p, int timeShift)
        {
            /*
             тут пересчитываем положение всех минионов согласно скорости движения и тому чем они занимаются
             */
            //getTimeFor1stCollision

            /*List<BoardObj> enemies = this.own ? p.enemyMinions : p.ownMinions;
            foreach (BoardObj bo in p.ownMinions)
            {
                if (e.Line != this.Line && this.Line != 3) continue;
                attackDef bo1ad = new attackDef(this, e);
                attackDef bo2ad = new attackDef(e, this);
                if (bo1ad.time < bo2ad.time) attackersList.Add(bo1ad);
                else if (bo2ad.time != int.MaxValue) attackersList.Add(bo2ad);
            }

            enemies = this.own ? p.enemyBuildings : p.ownBuildings;
            foreach (BoardObj e in enemies)
            {
                if (e.Line != this.Line && this.Line != 3) continue;
                attackDef bo1ad = new attackDef(this, e);
                attackDef bo2ad = new attackDef(e, this);
                if (bo1ad.time < bo2ad.time) attackersList.Add(bo1ad);
                else if (bo2ad.time != int.MaxValue) attackersList.Add(bo2ad);
            }

            enemies = this.own ? p.enemyTowers : p.ownTowers;
            foreach (BoardObj e in enemies)
            {
                if (e.Line != this.Line && this.Line != 3) continue;
                attackDef bo1ad = new attackDef(this, e);
                attackDef bo2ad = new attackDef(e, this);
                if (bo1ad.time < bo2ad.time) attackersList.Add(bo1ad);
                else if (bo2ad.time != int.MaxValue) attackersList.Add(bo2ad);
            }*/
        }


    }
}