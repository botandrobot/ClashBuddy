using Robi.Common;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Robi.Clash.DefaultSelectors.Apollo
{
    class CardChoosing
    {
        private static readonly ILogger Logger = LogProvider.CreateLogger<CardChoosing>();

        private static Handcard AttackKingTowerWithSpell(Playfield p)
        {
            var spells = Classification.GetOwnHandCards(p, boardObjType.AOE, SpecificCardType.SpellsDamaging);

            return spells?.FirstOrDefault();
        }


        public static Handcard All(Playfield p, FightState currentSituation, out VectorAI choosedPosition)
        {
            // TODO: Use more current situation
            Logger.Debug("Path: Spell - All");
            Logger.Debug("FightState: " + currentSituation);

            Handcard damagingSpell = DamagingSpellDecision(p, out choosedPosition);
            if (damagingSpell != null)
                return damagingSpell;

            Handcard aoeCard = AOEDecision(p);
            if (aoeCard != null)
                return aoeCard;

            Handcard bigGroupCard = BigGroupDecision(p, currentSituation);
            if (bigGroupCard != null)
                return bigGroupCard;

            if (p.enemyMinions.Any(n => n.Transport == transportType.AIR))
            {
                Logger.Debug("AttackFlying Needed");
                Handcard atkFlying = Classification.GetOwnHandCards(p, boardObjType.MOB, SpecificCardType.MobsFlyingAttack).FirstOrDefault();
                if (atkFlying != null)
                    return atkFlying;
            }

            if (DeployBuildingDecision(p, out Handcard buildingCard, currentSituation))
            {
                if (buildingCard != null)
                    return new Handcard(buildingCard.name, buildingCard.lvl);
            }

            // ToDo: Don´t play a tank, if theres already one on this side
            if ((int)currentSituation < 3 || (int)currentSituation > 6) // Just not at Under Attack
            {
                var tank = Classification.GetOwnHandCards(p, boardObjType.MOB, SpecificCardType.MobsTank).OrderBy(n => n.card.MaxHP);
                var lt = tank.LastOrDefault();
                if (lt != null && lt.manacost <= p.ownMana)
                    return lt;
            }

            // ToDo: Decision for building attacker
            if ((int)currentSituation > 6 && (int)currentSituation < 10)
            {
                var buildingAtkCard = Classification.GetOwnHandCards(p, boardObjType.MOB, SpecificCardType.MobsBuildingAttacker).FirstOrDefault();
                if (buildingAtkCard != null && buildingAtkCard.manacost <= p.ownMana)
                    return buildingAtkCard;
            }

            if((int)currentSituation < 3)
            {
                var highestHP = Classification.GetOwnHandCards(p, boardObjType.MOB, SpecificCardType.All)
                    .Where(n => n.manacost - p.ownMana <= 0)
                    .OrderBy(n => n.card.MaxHP).LastOrDefault();

                return highestHP;
            }

            var rangerCard = Classification.GetOwnHandCards(p, boardObjType.MOB, SpecificCardType.MobsRanger).FirstOrDefault();
            if (rangerCard != null && rangerCard.manacost <= p.ownMana)
                return rangerCard;

            var damageDealerCard = Classification.GetOwnHandCards(p, boardObjType.MOB, SpecificCardType.MobsDamageDealer).FirstOrDefault();
            if (damageDealerCard != null && damageDealerCard.manacost <= p.ownMana)
                return damageDealerCard;

            if((int)currentSituation >= 3 && (int)currentSituation <= 6)
                return Classification.GetOwnHandCards(p, boardObjType.MOB, SpecificCardType.MobsNoTank).FirstOrDefault();

            Logger.Debug("Wait - No card selected...");
            return null;
        }

        //private static Handcard DefenseTroop(Playfield p)
        //{
        //    Logger.Debug("Path: Spell - DefenseTroop");

        //    if (IsAOEAttackNeeded(p))
        //    {
        //        var atkAOE = GetOwnHandCards(p, boardObjType.MOB, SpecificCardType.MobsAOEGround).FirstOrDefault(); // Todo: just AOE-Attack

        //        if (atkAOE != null)
        //            return new Handcard(atkAOE.name, atkAOE.lvl);
        //    }

        //    if (p.enemyMinions.Where(n => n.Transport == transportType.AIR).Count() > 0)
        //    {
        //        var atkFlying = GetOwnHandCards(p, boardObjType.MOB, SpecificCardType.MobsFlyingAttack).FirstOrDefault();
        //        if (atkFlying != null)
        //            return new Handcard(atkFlying.name, atkFlying.lvl);
        //    }

        //    var powerSpell = powerCard(p).FirstOrDefault();
        //    if (powerSpell != null)
        //        return new Handcard(powerSpell.name, powerSpell.lvl);

        //    return cycleCard(p).FirstOrDefault();
        //}

        //private static Handcard Defense(Playfield p, FightState currentSituation, out VectorAI choosedPosition)
        //{
        //    Logger.Debug("Path: Spell - Defense");
        //    choosedPosition = null;
        //    IEnumerable<Handcard> damagingSpells = GetOwnHandCards(p, boardObjType.AOE, SpecificCardType.SpellsDamaging);


        //    Handcard damagingSpell = DamagingSpellDecision(p, out choosedPosition);
        //    if (damagingSpell != null)
        //        return new Handcard(damagingSpell.name, damagingSpell.lvl);

        //    Handcard aoeCard = AOEDecision(p, out choosedPosition, currentSituation);
        //    if (aoeCard != null)
        //        return aoeCard;

        //    if (p.enemyMinions.Where(n => n.Transport == transportType.AIR).Count() > 0)
        //    {
        //        Handcard atkFlying = GetOwnHandCards(p, boardObjType.MOB, SpecificCardType.MobsFlyingAttack).FirstOrDefault();
        //        if (atkFlying != null)
        //            return atkFlying;
        //    }

        //    var powerSpell = powerCard(p).FirstOrDefault();
        //    if (powerSpell != null)
        //        return new Handcard(powerSpell.name, powerSpell.lvl);

        //    return p.ownHandCards.FirstOrDefault();
        //}


        // ToDo: Create a Building concept

        private static Handcard Building(Playfield p)
        {
            Logger.Debug("Path: Spell - Building");
            var buildingCard = p.ownHandCards.FirstOrDefault(n => n.card.type == boardObjType.BUILDING);
            if (buildingCard != null)
                return new Handcard(buildingCard.name, buildingCard.lvl);

            return null;
        }


        // TODO: Check this out
        public static Handcard DamagingSpellDecision(Playfield p, out VectorAI choosedPosition)
        {
            choosedPosition = null;

            var damagingSpellsSource =
                Classification.GetOwnHandCards(p, boardObjType.PROJECTILE, SpecificCardType.SpellsDamaging);
            var damagingSpells = damagingSpellsSource as Handcard[] ?? damagingSpellsSource.ToArray();
            var fds = damagingSpells.FirstOrDefault();
            if (fds == null)
                return null;


            #region Tower
            var ds5 = damagingSpells.FirstOrDefault(n => (n.card.towerDamage >= p.enemyKingsTower.HP));
            if (ds5 != null)
            {
                Logger.Debug("towerDamage: {td} ; kt.hp: {kthp}", ds5.card.towerDamage,
                    p.enemyKingsTower.HP);
                choosedPosition = p.enemyKingsTower.Position;
                return ds5;
            }

            if (p.suddenDeath)
            {
                var ds3 = damagingSpells.FirstOrDefault(n => (n.card.towerDamage >= p.enemyPrincessTower1.HP));
                var ds4 = damagingSpells.FirstOrDefault(n => (n.card.towerDamage >= p.enemyPrincessTower2.HP));
               
                if (ds3 != null && p.enemyPrincessTower1.HP > 0)
                {
                    Logger.Debug("towerDamage: {td} ; pt1.hp: {pt1hp}", ds3.card.towerDamage,
                        p.enemyPrincessTower1.HP);
                    choosedPosition = p.enemyPrincessTower1.Position;
                    return ds3;
                }

                if (ds4 != null && p.enemyPrincessTower2.HP > 0)
                {
                    Logger.Debug("towerDamage: {td} ; pt1.hp: {pt1hp}", ds4.card.towerDamage,
                        p.enemyPrincessTower2.HP);
                    choosedPosition = p.enemyPrincessTower2.Position;
                    return ds4;
                }
            }
            #endregion

            var radiusOrderedDS = damagingSpells.OrderBy(n => n.card.DamageRadius).LastOrDefault();

            if (radiusOrderedDS == null) return null;
            
            var Group = p.getGroup(false, 200, boPriority.byTotalNumber, radiusOrderedDS.card.DamageRadius);

            if (Group == null)
                return null;

            var grpCount = Group.lowHPbo.Count() + Group.avgHPbo.Count() + Group.hiHPbo.Count();
            var hpSum = Group.lowHPboHP + Group.hiHPboHP + Group.avgHPboHP;

            var ds1 = damagingSpells.FirstOrDefault(n => n.card.DamageRadius > 3 && grpCount > 4);

            if (ds1 != null)
            {
                Logger.Debug("Damaging-Spell-Decision: HP-Sum of group = " + hpSum);
                choosedPosition = p.getDeployPosition(Group.Position, deployDirectionRelative.Down, 1000);
                return ds1;
            }

            var ds2 = damagingSpells.FirstOrDefault(n => n.card.DamageRadius <= 3 && grpCount > 1 && hpSum >= n.card.Atk * 2);

            if (ds2 != null)
            {
                Logger.Debug("Damaging-Spell-Decision: HP-Sum of group = " + hpSum);
                choosedPosition = p.getDeployPosition(Group.Position, deployDirectionRelative.Down, 1000);
                return ds2;
            }
            
            return null;
        }

        public static bool DeployBuildingDecision(Playfield p, out Handcard buildingCard, FightState currentSituation)
        {
            buildingCard = null;
            var condition = false;

            var hcMana = Classification.GetOwnHandCards(p, boardObjType.BUILDING, SpecificCardType.BuildingsMana).FirstOrDefault();
            var hcDefense = Classification.GetOwnHandCards(p, boardObjType.BUILDING, SpecificCardType.BuildingsDefense).FirstOrDefault();
            var hcAttack = Classification.GetOwnHandCards(p, boardObjType.BUILDING, SpecificCardType.BuildingsAttack).FirstOrDefault();
            var hcSpawning = Classification.GetOwnHandCards(p, boardObjType.BUILDING, SpecificCardType.BuildingsSpawning).FirstOrDefault();


            // Just for Defense
            if ((int) currentSituation < 3) return false;

            if (hcMana != null) condition = true;
            if (hcSpawning != null) condition = true;
            if (hcDefense != null) condition = true;

            // ToDo: Attack condition

            // ToDo: Underattack condition

            return condition;
        }


        private static Handcard AOEDecision(Playfield p)
        {
            Handcard aoeGround = null, aoeAir = null;

            var objGround = Helper.EnemyCharacterWithTheMostEnemiesAround(p, out int biggestEnemieGroupCount, transportType.GROUND);
            if (biggestEnemieGroupCount > 3)
                aoeGround = Classification.GetOwnHandCards(p, boardObjType.MOB, SpecificCardType.MobsAOEGround).FirstOrDefault();

            var objAir = Helper.EnemyCharacterWithTheMostEnemiesAround(p, out biggestEnemieGroupCount, transportType.AIR);
            if (biggestEnemieGroupCount > 3)
                aoeAir = Classification.GetOwnHandCards(p, boardObjType.MOB, SpecificCardType.MobsAOEAll).FirstOrDefault();

            return aoeAir ?? aoeGround;
        }

        private static Handcard BigGroupDecision(Playfield p, FightState fightState)
        {
            var aoe = p.enemyMinions.Where(n => n.card.aoeAir || n.card.aoeGround);
            var tanks = p.ownMinions.Where(n => Classification.IsMobsTankCurrentHP(n));

            // ToDo: Improve condition 
            switch (fightState)
            {
                case FightState.UAPTL1:
                case FightState.UAKTL1:
                case FightState.APTL1:
                case FightState.DPTL1:
                    if (aoe.Any(n => n.Line == 1) || !tanks.Any(n => n.Line == 1))
                        return null;
                    break;
                case FightState.UAKTL2:
                case FightState.UAPTL2:
                case FightState.APTL2:
                case FightState.DPTL2:
                    if (aoe.Any(n => n.Line == 2) || !tanks.Any(n => n.Line == 2))
                        return null;
                    break;
                case FightState.AKT:
                case FightState.DKT:
                    if (aoe.Any(n => n.Line == 1) || !tanks.Any(n => n.Line == 1)
                        || aoe.Any(n => n.Line == 2) || !tanks.Any(n => n.Line == 2))
                        return null;
                    break;
                default:
                    break;
            }
            return Classification.GetOwnHandCards(p, boardObjType.MOB, SpecificCardType.MobsBigGroup).FirstOrDefault();
        }

        public static Handcard GetOppositeCard(Playfield p, FightState currentSituation)
        {
            if (p.enemyKingsTower.HP < Apollo.Setting.KingTowerSpellDamagingHealth)
            {
                Handcard hc = AttackKingTowerWithSpell(p);

                if (hc != null)
                    return hc;
            }

            switch (currentSituation)
            {
                case FightState.UAKTL1:
                case FightState.UAKTL2:
                case FightState.UAPTL1:
                case FightState.UAPTL2:
                case FightState.AKT:
                case FightState.APTL1:
                case FightState.APTL2:
                case FightState.DKT:
                case FightState.DPTL1:
                case FightState.DPTL2:
                    {
                        BoardObj defender = Decision.GetBestDefender(p);

                        if (defender == null)
                            return null;

                        Logger.Debug("BestDefender: {Defender}", defender.ToString());
                        opposite spell = KnowledgeBase.Instance.getOppositeToAll(p, defender, Decision.CanWaitDecision(p, currentSituation));

                        if (spell != null && spell.hc != null)
                        {
                            Logger.Debug("Spell: {Sp} - MissingMana: {MM}", spell.hc.name, spell.hc.missingMana);
                            if (spell.hc.missingMana == 100) // Oposite-Card is already on the field
                                return null;
                            else if (spell.hc.missingMana > 0)
                                return null;
                            else
                                return spell.hc;
                        }
                    }
                    break;
                case FightState.START:
                case FightState.WAIT:
                default:
                    break;
            }
            return null;
        }


        public static Handcard GetMobInPeace(Playfield p, FightState currentSituation)
        {
            if(PlayfieldAnalyse.lines[0].Danger <= Level.LOW || PlayfieldAnalyse.lines[1].Danger <= Level.LOW)
            {
                var tanks = p.ownMinions.Where(n => Classification.IsMobsTankCurrentHP(n))
                                                            .OrderBy(n => n.HP).ToArray();
                switch (currentSituation)
                {
                    case FightState.DPTL1:
                    case FightState.APTL1:
                        BoardObj tankL1 = tanks.Where(n => n.Line == 1).OrderBy(n => n.HP).FirstOrDefault();

                        if (tankL1 != null)
                            return p.getPatnerForMobInPeace(tankL1);
                        else
                            return p.getPatnerForMobInPeace(p.ownMinions.Where(n => n.Line == 1).OrderBy(n => n.Atk).FirstOrDefault());
                    case FightState.DPTL2:
                    case FightState.APTL2:
                        BoardObj tankL2 = tanks.Where(n => n.Line == 2).OrderBy(n => n.HP).FirstOrDefault();

                        if (tankL2 != null)
                            return p.getPatnerForMobInPeace(tankL2);
                        else
                            return p.getPatnerForMobInPeace(p.ownMinions.Where(n => n.Line == 2).OrderBy(n => n.Atk).FirstOrDefault());
                    case FightState.DKT:
                    case FightState.AKT:
                        if (tanks.FirstOrDefault() != null)
                            return p.getPatnerForMobInPeace(tanks.FirstOrDefault());
                        else
                            return p.getPatnerForMobInPeace(p.ownMinions.OrderBy(n => n.Atk).FirstOrDefault());
                    case FightState.UAPTL1:
                    case FightState.UAPTL2:
                    case FightState.UAKTL1:
                    case FightState.UAKTL2:
                    case FightState.START:
                    case FightState.WAIT:
                    default:
                        break;
                }
            }
            return null;
        }
    }
}