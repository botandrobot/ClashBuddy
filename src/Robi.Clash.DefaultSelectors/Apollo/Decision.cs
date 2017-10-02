using Robi.Common;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Robi.Clash.DefaultSelectors.Apollo
{
    class Decision
    {
        public static readonly ILogger Logger = LogProvider.CreateLogger<Decision>();

        public static bool CanWaitDecision(Playfield p, FightState currentSituation)
        {
            if (p.noEnemiesOnMySide())
            {
                switch (currentSituation)
                {
                    //case FightState.DLPT:
                    //case FightState.DRPT:
                    //    break;
                    //case FightState.DKT:
                    //    break;
                    case FightState.ALPT:
                        {
                            if (p.BattleTime.TotalSeconds < 20)
                                return false;
                            else if (p.enemyPrincessTower1.HP < 300 && p.enemyPrincessTower1.HP > 0)
                                return false;
                            break;
                        }
                    case FightState.ARPT:
                        {
                            if (p.BattleTime.TotalSeconds < 20)
                                return false;
                            else if (p.enemyPrincessTower2.HP < 300 && p.enemyPrincessTower2.HP > 0)
                                return false;
                            break;
                        }
                    case FightState.AKT:
                        {
                            if (p.BattleTime.TotalSeconds < 30)
                                return false;
                            else if (p.enemyKingsTower.HP < 300 && p.enemyKingsTower.HP > 0)
                                return false;
                            break;
                        }
                }
            }
            else
            {
                if (p.BattleTime.TotalSeconds < 20)
                    return false;
                if (p.ownKingsTower.HP < 1000)
                    return false;
                if (Helper.IsAnEnemyObjectInArea(p, p.ownKingsTower.Position, 4000, boardObjType.MOB))
                    return false;
            }

            return true;
        }
        public static FightState DefenseDecision(Playfield p)
        {
            if (p.ownTowers.Count < 3)
                return FightState.DKT;

            BoardObj princessTower = p.enemyPrincessTowers.OrderBy(n => n.HP).FirstOrDefault(); // Because they are going to attack this tower

            if (princessTower.Line == 2)
                return FightState.DRPT;
            else
                return FightState.DLPT;
        }

        public static FightState EnemyHasCharsOnTheFieldDecision(Playfield p)
        {
            if (p.ownTowers.Count > 2)
            {
                //BoardObj obj = GetNearestEnemy(p);

                // ToDo: Get most dangeroust group
                group mostDangeroustGroup = p.getGroup(false, 200, boPriority.byTotalBuildingsDPS, 3000);

                if (mostDangeroustGroup == null)
                {
                    Logger.Debug("mostDangeroustGroup = null");
                    return FightState.DKT;
                }
                int line = mostDangeroustGroup.Position.X > 8700 ? 2 : 1;
                Logger.Debug("mostDangeroustGroup.Position.X = {0} ; line = {1}", mostDangeroustGroup?.Position?.X, line);


                if (line == 2)
                    return FightState.DRPT;
                else
                    return FightState.DLPT;
            }
            else
            {
                return FightState.DKT;
            }
        }

        public static FightState DangerousSituationDecision(Playfield p, int line)
        {
            if (p.ownTowers.Count > 2)
            {
                if (line == 2)
                    return FightState.UARPT;
                else if (line == 1)
                    return FightState.UALPT;
                else
                    return FightState.UAKT;
            }
            else
            {
                return FightState.UAKT;
            }
        }

        public static FightState EnemyIsOnOurSideDecision(Playfield p)
        {
            Logger.Debug("Enemy is on our Side!!");
            if (p.ownTowers.Count > 2)
            {
                BoardObj obj = Helper.GetNearestEnemy(p);

                if (obj != null && obj.Line == 2)
                    return FightState.UARPT;
                else
                    return FightState.UALPT;
            }
            else
            {
                return FightState.UAKT;
            }
        }

        public static FightState AttackDecision(Playfield p)
        {
            if (p.enemyTowers.Count < 3)
                return FightState.AKT;


            BoardObj princessTower = p.enemyPrincessTowers.OrderBy(n => n.HP).FirstOrDefault();

            if (princessTower.Line == 2)
                return FightState.ARPT;
            else
                return FightState.ALPT;
        }

        public static FightState GoodAttackChanceDecision(Playfield p, int line)
        {
            if (p.enemyTowers.Count < 3)
                return FightState.AKT;

            if (line == 2)
                return FightState.ARPT;
            else
                return FightState.ALPT;
        }

        public static FightState GameBeginningDecision(Playfield p, bool gameBeginning)
        {
            bool StartFirstAttack = true;

            try
            {
                StartFirstAttack = (p.ownMana < Settings.ManaTillFirstAttack);
            }
            catch (Exception e)
            {

            }


            if (StartFirstAttack)
            {
                if (!p.noEnemiesOnMySide())
                    gameBeginning = false;

                return FightState.START;
            }
            else
            {
                gameBeginning = false;
                BoardObj obj = Helper.GetNearestEnemy(p);

                if (obj.Line == 2)
                    return FightState.DRPT;
                else
                    return FightState.DLPT;
            }
        }

        public static bool SupportDeployment(Playfield p, int line)
        {
            // If own characters already attacking and you are deploying as support
            // The chars should be deployed behind the own chars
            IEnumerable<BoardObj> attackingChars = p.ownMinions.Where(n => n.Line == line && Helper.IsObjectAtOtherSide(p, n));

            // Maybe check also which card type: Tank deployed in front, Ranger behinde ...

            if (attackingChars.Count() > 0)
                return true;
            else
                return false;
        }

        public static int DangerOrBestAttackingLine(Playfield p) // Good chance for an attack?
        {
            float hpBorder = p.ownKingsTower.Atk * 3;

            // comparison
            int compHpL1 = PlayfieldAnalyse.lines[0].OwnMinionHP - PlayfieldAnalyse.lines[0].EnemyMinionHP;
            int compHpL2 = PlayfieldAnalyse.lines[1].OwnMinionHP - PlayfieldAnalyse.lines[1].EnemyMinionHP;
            int compAtkL1 = PlayfieldAnalyse.lines[0].OwnMinionAtk - PlayfieldAnalyse.lines[0].EnemyMinionAtk;
            int compAtkL2 = PlayfieldAnalyse.lines[1].OwnMinionAtk - PlayfieldAnalyse.lines[1].EnemyMinionAtk;

            if (compHpL1 == 0 && compHpL2 == 0)
                return 0;


            if (compHpL1 < compHpL2)
            {
                if (compHpL1 < -hpBorder)
                    return -1;

                if (compHpL2 > hpBorder)
                    return 2;
            }
            else
            {
                if (compHpL1 > hpBorder)
                    return 1;

                if (compHpL2 < hpBorder)
                    return -2;
            }



            #region check if buildings can attack own towers
            List<BoardObj> enemyBuildings = p.enemyBuildings;

            if (enemyBuildings?.Count() > 0)
            {
                BoardObj bKT = enemyBuildings.Where(n => n.IsPositionInArea(p, p.ownKingsTower.Position)).FirstOrDefault();
                BoardObj bPT1 = enemyBuildings.Where(n => n.IsPositionInArea(p, p.ownPrincessTower1.Position)).FirstOrDefault();
                BoardObj bPT2 = enemyBuildings.Where(n => n.IsPositionInArea(p, p.ownPrincessTower2.Position)).FirstOrDefault();

                if (bKT != null)
                    return 3;

                if (bPT1 != null)
                    return 1;

                if (bPT2 != null)
                    return 2;
            }
            #endregion


            return 0;

            #region just as comments (.attacker is not implemented atm)
            // Check if building attacks Tower (.attacker is not implemented atm)
            //if (p.ownKingsTower?.attacker?.type == boardObjType.BUILDING)
            //    return 3;
            //if (p.ownPrincessTower1?.attacker?.type == boardObjType.BUILDING)
            //    return 1;
            //if (p.ownPrincessTower2?.attacker?.type == boardObjType.BUILDING)
            //    return 2;
            #endregion
        }

        public static BoardObj GetBestDefender(Playfield p)
        {
            // TODO: Find better condition
            int count = 0;
            BoardObj enemy = Helper.EnemyCharacterWithTheMostEnemiesAround(p, out count, transportType.NONE);

            if (enemy == null)
                return p.ownKingsTower;

            if (enemy.Line == 2)
                return p.ownPrincessTower2;
            else if (enemy.Line == 1)
                return p.ownPrincessTower1;
            else
                return p.ownKingsTower;

        }
    }
}
