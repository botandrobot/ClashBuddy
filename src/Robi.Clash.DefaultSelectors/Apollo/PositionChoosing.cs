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
            if (hc == null || hc.card == null)
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
                case FightState.UAKT:
                    choosedPosition = UAKT(p, hc);
                    break;
                case FightState.UALPT:
                    choosedPosition = UALPT(p, hc);
                    break;
                case FightState.UARPT:
                    choosedPosition = UARPT(p, hc);
                    break;
                case FightState.AKT:
                    choosedPosition = AKT(p);
                    break;
                case FightState.ALPT:
                    choosedPosition = ALPT(p);
                    break;
                case FightState.ARPT:
                    choosedPosition = ARPT(p);
                    break;
                case FightState.DKT:
                    choosedPosition = DKT(p, hc);
                    break;
                case FightState.DLPT:
                    choosedPosition = DLPT(p, hc);
                    break;
                case FightState.DRPT:
                    choosedPosition = DRPT(p, hc);
                    break;
                default:
                    //Logger.Debug("GameState unknown");
                    break;
            }

            if (choosedPosition == null)
                return null;

            //Logger.Debug("GameState: {GameState}", gameState.ToString());
            //Logger.Debug("nextPosition: " + nextPosition);

            return choosedPosition;
        }

        #region UnderAttack
        private static VectorAI UAKT(Playfield p, Handcard hc)
        {
            return DKT(p, hc);
        }

        private static VectorAI UALPT(Playfield p, Handcard hc)
        {
            return DLPT(p, hc);
        }
        private static VectorAI UARPT(Playfield p, Handcard hc)
        {
            return DRPT(p, hc);
        }
        #endregion

        #region Defense
        private static VectorAI DKT(Playfield p, Handcard hc)
        {
            if (hc.card.type == boardObjType.MOB)
            {
                try
                {
                    if (hc.card.MaxHP >= Settings.MinHealthAsTank)
                    {

                        // TODO: Analyse which is the most dangerous line
                        if (PlayfieldAnalyse.lines[1].ComparisionHP < PlayfieldAnalyse.lines[0].ComparisionHP)
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
                }
                catch (Exception) { }

                if (hc.card.Transport == transportType.AIR)
                {
                    // TODO: Analyse which is the most dangerous line
                    if (PlayfieldAnalyse.lines[1].ComparisionHP < PlayfieldAnalyse.lines[0].ComparisionHP)
                        return p.getDeployPosition(deployDirectionAbsolute.ownPrincessTowerLine2);
                    else
                        return p.getDeployPosition(deployDirectionAbsolute.ownPrincessTowerLine1);
                }
                else
                {
                    if (PlayfieldAnalyse.lines[1].ComparisionHP < PlayfieldAnalyse.lines[0].ComparisionHP)
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
                return GetPositionOfTheBestBuildingDeploy(p);
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
        private static VectorAI DLPT(Playfield p, Handcard hc)
        {
            BoardObj lPT = p.ownPrincessTower1;

            if (lPT == null || lPT.Position == null)
                return DKT(p, hc);

            VectorAI lPTP = lPT.Position;
            VectorAI correctedPosition = PrincessTowerCharacterDeploymentCorrection(lPTP, p, hc);
            return correctedPosition;
        }
        private static VectorAI DRPT(Playfield p, Handcard hc)
        {
            BoardObj rPT = p.ownPrincessTower2;

            if (rPT == null && rPT.Position == null)
                return DKT(p, hc);

            VectorAI rPTP = rPT.Position;
            VectorAI correctedPosition = PrincessTowerCharacterDeploymentCorrection(rPTP, p, hc);
            return correctedPosition;
        }
        #endregion

        #region Attack
        private static VectorAI AKT(Playfield p)
        {
            Logger.Debug("AKT");

            if (p.enemyPrincessTowers.Count == 2)
            {
                if (p.enemyPrincessTower1.HP < p.enemyPrincessTower2.HP)
                    return ALPT(p);
                else
                    return ARPT(p);
            }

            if (p.enemyPrincessTower1.HP == 0)
                return ALPT(p);

            if (p.enemyPrincessTower2.HP == 0)
                return ARPT(p);

            VectorAI position = p.enemyKingsTower?.Position;

            if (Decision.SupportDeployment(p, 1))
                position = p.getDeployPosition(position, deployDirectionRelative.Down, 500);

            return position;
        }
        private static VectorAI ALPT(Playfield p)
        {
            Logger.Debug("ALPT");

            VectorAI lPT = p.getDeployPosition(deployDirectionAbsolute.enemyPrincessTowerLine1);

            if (Decision.SupportDeployment(p, 1))
                lPT = p.getDeployPosition(lPT, deployDirectionRelative.Down, 500);

            return lPT;
        }
        private static VectorAI ARPT(Playfield p)
        {
            Logger.Debug("ARPT");

            VectorAI rPT = p.getDeployPosition(deployDirectionAbsolute.enemyPrincessTowerLine2);

            if (Decision.SupportDeployment(p, 2))
                rPT = p.getDeployPosition(rPT, deployDirectionRelative.Down, 500);

            return rPT;
        }
        #endregion

        public static VectorAI GetPositionOfTheBestDamagingSpellDeploy(Playfield p)
        {
            // Prio1: Hit Enemy King Tower if health is low
            // Prio2: Every damaging spell if there is a big group of enemies
            Logger.Debug("GetPositionOfTheBestDamaingSpellDeploy");

            try
            {
                if (p.enemyKingsTower?.HP < Settings.KingTowerSpellDamagingHealth || (p.enemyMinions.Count + p.enemyBuildings.Count) < 1)
                    return p.enemyKingsTower?.Position;
            }
            catch (Exception)
            {
                BoardObj enemy = Helper.EnemyCharacterWithTheMostEnemiesAround(p, out int count, transportType.NONE);

                if (enemy != null && enemy.Position != null)
                {
                    try
                    {
                        if (Helper.HowManyNFCharactersAroundCharacter(p, enemy) >= Settings.SpellCorrectionConditionCharCount)
                        {
                            Logger.Debug("enemy.Name = {Name}", enemy.Name);
                            if (enemy.Position != null) Logger.Debug("enemy.Position = {position}", enemy.Position);

                            return enemy.Position;
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
            }

            Logger.Debug("Error: 0/0");
            return new VectorAI(0, 0);
        }

        public static VectorAI GetPositionOfTheBestBuildingDeploy(Playfield p)
        {
            // ToDo: Find the best position
            VectorAI betweenBridges = p.getDeployPosition(deployDirectionAbsolute.betweenBridges);
            VectorAI result = p.getDeployPosition(betweenBridges, deployDirectionRelative.Down, 4000);
            return result;
        }

        private static VectorAI PrincessTowerCharacterDeploymentCorrection(VectorAI position, Playfield p, Handcard hc)
        {
            if (hc == null || hc.card == null || position == null)
                return null;

            //Logger.Debug("PT Characer Position Correction: Name und Typ {0} " + cardToDeploy.Name, (cardToDeploy as CardCharacter).Type);
            if (hc.card.type == boardObjType.MOB)
            {
                try
                {
                    if (hc.card.MaxHP >= Settings.MinHealthAsTank)
                    {
                        //position.SubtractYInDirection(p);
                        return p.getDeployPosition(position, deployDirectionRelative.Up, 100);
                    }
                }
                catch (Exception) { }

                {
                    //position.AddYInDirection(p);
                    return p.getDeployPosition(position, deployDirectionRelative.Down, 2000);
                }

            }
            else if (hc.card.type == boardObjType.BUILDING)
                return GetPositionOfTheBestBuildingDeploy(p);
            else
                Logger.Debug("Tower Correction: No Correction!!!");

            return position;
        }
    }
}