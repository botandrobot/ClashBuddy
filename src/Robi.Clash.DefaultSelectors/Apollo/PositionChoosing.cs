using Robi.Common;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Robi.Clash.DefaultSelectors.Apollo
{
    class PositionChoosing
    {
        private static readonly ILogger Logger = LogProvider.CreateLogger<PositionChoosing>();

        public static VectorAI GetNextSpellPosition(FightState gameState, Handcard hc, Playfield p)
        {
            if (hc?.card == null)
                return null;

            VectorAI choosedPosition = null;


            if (hc.card.type == boardObjType.AOE || hc.card.type == boardObjType.PROJECTILE)
            {
                Logger.Debug("AOE or PROJECTILE");
                return GetPositionOfTheBestDamagingSpellDeploy(p);
            }

            // ToDo: Handle Defense Gamestates
            switch (gameState)
            {
                case FightState.UAKTL1:
                    choosedPosition = UAKT(p, hc, 1);
                    break;
                case FightState.UAKTL2:
                    choosedPosition = UAKT(p, hc, 2);
                    break;
                case FightState.UAPTL1:
                    choosedPosition = UAPTL1(p, hc);
                    break;
                case FightState.UAPTL2:
                    choosedPosition = UAPTL2(p, hc);
                    break;
                case FightState.AKT:
                    choosedPosition = AKT(p, hc);
                    break;
                case FightState.APTL1:
                    choosedPosition = APTL1(p, hc);
                    break;
                case FightState.APTL2:
                    choosedPosition = APTL2(p, hc);
                    break;
                case FightState.DKT:
                    choosedPosition = DKT(p, hc,0);
                    break;
                case FightState.DPTL1:
                    choosedPosition = DPTL1(p, hc);
                    break;
                case FightState.DPTL2:
                    choosedPosition = DPTL2(p, hc);
                    break;
                default:
                    //Logger.Debug("GameState unknown");
                    break;
            }

            //Logger.Debug("GameState: {GameState}", gameState.ToString());
            //Logger.Debug("nextPosition: " + nextPosition);

            return choosedPosition;
        }

        #region UnderAttack
        private static VectorAI UAKT(Playfield p, Handcard hc, int line)
        {
            return DKT(p, hc, line);
        }

        private static VectorAI UAPTL1(Playfield p, Handcard hc)
        {
            return DPTL1(p, hc);
        }
        private static VectorAI UAPTL2(Playfield p, Handcard hc)
        {
            return DPTL2(p, hc);
        }
        #endregion

        #region Defense
        private static VectorAI DKT(Playfield p, Handcard hc, int line)
        {
            // ToDo: Improve
            if(line == 0)
            {
                line = p.enemyPrincessTower1.HP < p.enemyPrincessTower2.HP ? 1 : 2;
            }

            if (hc.card.type == boardObjType.MOB)
            {
                if (hc.card.MaxHP >= Setting.MinHealthAsTank)
                {

                    // TODO: Analyse which is the most dangerous line
                    if (line == 2)
                    {
                        Logger.Debug("KT RightUp");
                        VectorAI v = p.getDeployPosition(p.ownKingsTower.Position, deployDirectionRelative.RightUp, 100);
                        return v;
                    }
                    else
                    {
                        Logger.Debug("KT LeftUp");
                        VectorAI v = p.getDeployPosition(p.ownKingsTower.Position, deployDirectionRelative.LeftUp, 100);
                        return v;
                    }
                }

                if (hc.card.Transport == transportType.AIR)
                {
                    return p.getDeployPosition(line == 2 ? deployDirectionAbsolute.ownPrincessTowerLine2 : deployDirectionAbsolute.ownPrincessTowerLine1);
                }
                else
                {
                    if (line == 2)
                    {
                        Logger.Debug("BehindKT: Line2");
                        VectorAI position = p.getDeployPosition(deployDirectionAbsolute.behindKingsTowerLine2);
                        return position;
                    }
                    else
                    {
                        Logger.Debug("BehindKT: Line1");
                        VectorAI position = p.getDeployPosition(deployDirectionAbsolute.behindKingsTowerLine1);
                        return position;
                    }
                }
            }
            else if (hc.card.type == boardObjType.BUILDING)
            {
                //switch ((cardToDeploy as CardBuilding).Type)
                //{
                //    case BuildingType.BuildingDefense:
                //    case BuildingType.BuildingSpawning:
                return GetPositionOfTheBestBuildingDeploy(p, hc, FightState.DKT);
                //}
            }
            else if (hc.card.type == boardObjType.AOE || hc.card.type == boardObjType.PROJECTILE)
                return GetPositionOfTheBestDamagingSpellDeploy(p);
            else
            {
                Logger.Debug("DKT: Handcard equals NONE!");
                return p.ownKingsTower?.Position;
            }

        }
        private static VectorAI DPTL1(Playfield p, Handcard hc)
        {
            BoardObj lPT = p.ownPrincessTower1;

            if (lPT?.Position == null)
                return DKT(p, hc,1);

            switch (hc.card.type)
            {
                case boardObjType.MOB:
                    return PrincessTowerCharacterDeploymentCorrection(lPT.Position, p, hc);
                case boardObjType.BUILDING:
                    //switch ((cardToDeploy as CardBuilding).Type)
                    //{
                    //    case BuildingType.BuildingDefense:
                    //    case BuildingType.BuildingSpawning:
                    return GetPositionOfTheBestBuildingDeploy(p, hc, FightState.DPTL1);
                    //}
                case boardObjType.AOE:
                case boardObjType.PROJECTILE:
                    return GetPositionOfTheBestDamagingSpellDeploy(p);
            }

            return lPT.Position;
        }
        private static VectorAI DPTL2(Playfield p, Handcard hc)
        {
            BoardObj rPT = p.ownPrincessTower2;

            if (rPT == null && rPT.Position == null)
                return DKT(p, hc,2);

            if (hc.card.type == boardObjType.MOB)
            {
                return PrincessTowerCharacterDeploymentCorrection(rPT.Position, p, hc);
            }
            else if (hc.card.type == boardObjType.BUILDING)
            {
                //switch ((cardToDeploy as CardBuilding).Type)
                //{
                //    case BuildingType.BuildingDefense:
                //    case BuildingType.BuildingSpawning:
                return GetPositionOfTheBestBuildingDeploy(p, hc, FightState.DPTL2);
                //}
            }
            else if (hc.card.type == boardObjType.AOE || hc.card.type == boardObjType.PROJECTILE)
                return GetPositionOfTheBestDamagingSpellDeploy(p);

            return rPT.Position;
        }
        #endregion

        #region Attack
        private static VectorAI AKT(Playfield p, Handcard hc)
        {
            Logger.Debug("AKT");

            if (p.enemyPrincessTowers.Count == 2)
            {
                if (p.enemyPrincessTower1.HP < p.enemyPrincessTower2.HP)
                    return APTL1(p, hc);
                else
                    return APTL2(p, hc);
            }

            if (p.enemyPrincessTower1.HP == 0 && p.enemyPrincessTower2.HP > 0)
                return APTL1(p, hc);

            if (p.enemyPrincessTower2.HP == 0 && p.enemyPrincessTower1.HP > 0)
                return APTL2(p, hc);

            VectorAI position = p.enemyKingsTower?.Position;

            //if (Decision.SupportDeployment(p, 1))
            //    position = p.getDeployPosition(position, deployDirectionRelative.Down, 500);

            return position;
        }
        

        private static VectorAI APTL1(Playfield p, Handcard hc)
        {
            return APT(p, hc, 1);
        }
        private static VectorAI APTL2(Playfield p, Handcard hc)
        {
            return APT(p, hc, 2);
        }

        private static VectorAI APT(Playfield p, Handcard hc, int line)
        {
            Logger.Debug("ALPT");

            if (hc.card.type == boardObjType.BUILDING)
                return line == 1 ? GetPositionOfTheBestBuildingDeploy(p, hc, FightState.APTL1) 
                                    : GetPositionOfTheBestBuildingDeploy(p, hc, FightState.APTL2);

            if (hc.card.MaxHP >= Setting.MinHealthAsTank)
            {
                VectorAI tankInFront = Helper.DeployTankInFront(p, line);

                if (tankInFront != null)
                    return tankInFront;
            }
            else
            {
                VectorAI behindTank = Helper.DeployBehindTank(p, line);

                if (behindTank != null)
                    return behindTank;
            }

            VectorAI PT;

            if (PlayfieldAnalyse.lines[line - 1].OwnMobSide)
            {
                PT = p.getDeployPosition(deployDirectionAbsolute.ownPrincessTowerLine1);

                if (Decision.SupportDeployment(p, line, true))
                    PT = p.getDeployPosition(PT, deployDirectionRelative.Down);
            }
            else
            {
                PT = p.getDeployPosition(deployDirectionAbsolute.enemyPrincessTowerLine1);

                if (Decision.SupportDeployment(p, line, false))
                    PT = p.getDeployPosition(PT, deployDirectionRelative.Down);
            }

            return PT;
        }
        #endregion

        public static VectorAI GetPositionOfTheBestDamagingSpellDeploy(Playfield p)
        {
            // Prio1: Hit Enemy King Tower if health is low
            // Prio2: Every damaging spell if there is a big group of enemies
            Logger.Debug("GetPositionOfTheBestDamaingSpellDeploy");

            if (p.enemyKingsTower?.HP < Setting.KingTowerSpellDamagingHealth || (p.enemyMinions.Count + p.enemyBuildings.Count) < 1)
                return p.enemyKingsTower?.Position;

            BoardObj enemy = Helper.EnemyCharacterWithTheMostEnemiesAround(p, out int count, transportType.NONE);

            if (enemy?.Position != null)
            {
                // Debugging: try - catch is just for debugging
                try
                {
                    // ToDo: Use a mix of the HP and count of the Enemy Units
                    // How fast are the enemy units, needed for a better correction
                    if (Helper.HowManyNFCharactersAroundCharacter(p, enemy) >= Setting.SpellCorrectionConditionCharCount)
                    {
                        Logger.Debug("With correction; enemy.Name = {Name}", enemy.Name);
                        if (enemy.Position != null)
                        {
                            Logger.Debug("enemy.Position = {position}", enemy.Position);
                            return p.getDeployPosition(enemy.Position, deployDirectionRelative.Down, 500);
                        }
                    }
                    else
                    {
                        Logger.Debug("No correction; enemy.Name = {Name}", enemy.Name);
                        if (enemy.Position != null)
                        {
                            Logger.Debug("enemy.Position = {position}", enemy.Position);
                            return enemy.Position;
                        }
                    }
                }
                catch (Exception)
                {
                    //enemy.Position.AddYInDirection(p, 3000); // Position Correction
                    VectorAI result = p.getDeployPosition(enemy.Position, deployDirectionRelative.Down, 500);

                    Logger.Debug("enemy.Name = {Name}", enemy.Name);
                    if (enemy.Position != null) Logger.Debug("enemy.Position = {position}", enemy.Position);
                    Logger.Debug("result = {position}", result);

                    return result;
                }
            }
            Logger.Debug("enemy = null?{enemy} ; enemy.position = null?{position}", enemy == null, enemy.Position == null);

            Logger.Debug("Error: 0/0");
            return new VectorAI(0, 0);
        }

        public static VectorAI GetPositionOfTheBestBuildingDeploy(Playfield p, Handcard hc, FightState currentSituation)
        {
            // ToDo: Find the best position
            VectorAI betweenBridges = p.getDeployPosition(deployDirectionAbsolute.betweenBridges);

            //switch (currentSituation)
            //{
            //    case FightState.UAPTL1:
            //    case FightState.DPTL1:
            //        return p.getDeployPosition(p.ownPrincessTower1.Position, deployDirectionRelative.RightDown);
            //    case FightState.UAPTL2:
            //    case FightState.DPTL2:
            //        return p.getDeployPosition(p.ownPrincessTower2.Position, deployDirectionRelative.LeftDown);
            //    case FightState.UAKTL1:
            //    case FightState.UAKTL2:
            //        return p.getDeployPosition(p.ownKingsTower.Position, deployDirectionRelative.Down);
            //    case FightState.APTL1:
            //        return p.getDeployPosition(betweenBridges, deployDirectionRelative.Left, 1000);
            //    case FightState.APTL2:
            //        return p.getDeployPosition(betweenBridges, deployDirectionRelative.Right, 1000);
            //    case FightState.AKT:
            //        return p.getDeployPosition(p.enemyKingsTower, deployDirectionRelative.Down, 500);
            //}

            return p.getDeployPosition(betweenBridges, deployDirectionRelative.Down, 4000);
        }

        private static VectorAI PrincessTowerCharacterDeploymentCorrection(VectorAI position, Playfield p, Handcard hc)
        {
            if (hc?.card == null || position == null)
                return null;

            //Logger.Debug("PT Characer Position Correction: Name und Typ {0} " + cardToDeploy.Name, (cardToDeploy as CardCharacter).Type);
            if (hc.card.type == boardObjType.MOB)
            {
                if (hc.card.MaxHP >= Setting.MinHealthAsTank)
                    return p.getDeployPosition(position, deployDirectionRelative.Up, 100);
                
                // ToDo: Maybe if there is already a tank, place it behind him

                //if(Classification.GetMoreSpecificCardType(hc, SpecificCardType.MobsAOE) == MoreSpecificMobCardType.AOEGround)
                //{
                //    return p.getDeployPosition(position, deployDirectionRelative.Up, 100);
                //}

                if (Classification.GetSpecificCardType(hc) == SpecificCardType.MobsRanger)
                    return p.getDeployPosition(position, deployDirectionRelative.Down, 2000);

                return p.getDeployPosition(position, deployDirectionRelative.Up, 100);
            }
            else
                Logger.Debug("Tower Correction: No Correction!!!");

            return position;
        }


    }
}