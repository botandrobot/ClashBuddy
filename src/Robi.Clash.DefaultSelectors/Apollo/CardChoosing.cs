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

        #region Which Card Old
        private static IOrderedEnumerable<Handcard> CycleCard(Playfield p)
        {
            return p.ownHandCards.Where(s => s != null && s.manacost <= 3 && s.card.type == boardObjType.MOB).OrderBy(s => s.manacost);
        }

        private static IOrderedEnumerable<Handcard> PowerCard(Playfield p)
        {
            return p.ownHandCards.Where(s => s != null && s.manacost > 3 && s.card.type == boardObjType.MOB).OrderBy(s => s.manacost);
        }

        private static Handcard AttackKingTowerWithSpell(Playfield p)
        {
            IEnumerable<Handcard> spells = Classification.GetOwnHandCards(p, boardObjType.AOE, SpecificCardType.SpellsDamaging);

            if (spells != null)
                return spells.FirstOrDefault();

            return null;
        }


        public static Handcard All(Playfield p, FightState currentSituation, out VectorAI choosedPosition)
        {
            // TODO: Use more current situation
            Logger.Debug("Path: Spell - All");
            Logger.Debug("FightState: " + currentSituation);

            Handcard damagingSpell = DamagingSpellDecision(p, out choosedPosition);
            if (damagingSpell != null)
                return damagingSpell;

            Handcard aoeCard = AOEDecision(p, out choosedPosition, currentSituation);
            if (aoeCard != null)
                return aoeCard;

            if (p.enemyMinions.Where(n => n.Transport == transportType.AIR).Count() > 0)
            {
                Logger.Debug("AttackFlying Needed");
                Handcard atkFlying = Classification.GetOwnHandCards(p, boardObjType.MOB, SpecificCardType.MobsFlyingAttack).FirstOrDefault();
                if (atkFlying != null)
                    return atkFlying;
            }

            if ((int)currentSituation < 3) // Just for Defense
            {
                if (DeployBuildingDecision(p, out choosedPosition))
                {
                    // ToDo: Take right building and set right Building-Type
                    var buildingCard = p.ownHandCards.Where(n => n.card.type == boardObjType.BUILDING).FirstOrDefault();
                    if (buildingCard != null)
                        return new Handcard(buildingCard.name, buildingCard.lvl);
                }
                choosedPosition = null;
            }

            if ((int)currentSituation < 3 || (int)currentSituation > 6) // Just not at Under Attack
            {
                var tank = Classification.GetOwnHandCards(p, boardObjType.MOB, SpecificCardType.MobsTank).OrderBy(n => n.card.MaxHP);
                if (tank.LastOrDefault() != null && tank.LastOrDefault().manacost <= p.ownMana)
                    return tank.LastOrDefault();
            }

            // ToDo: Decision for building attacker
            if ((int)currentSituation > 6 && (int)currentSituation < 10)
                return Classification.GetOwnHandCards(p, boardObjType.MOB, SpecificCardType.MobsBuildingAttacker).FirstOrDefault();

            var rangerCard = Classification.GetOwnHandCards(p, boardObjType.MOB, SpecificCardType.MobsRanger).FirstOrDefault();
            if (rangerCard != null && rangerCard.manacost <= p.ownMana)
                return rangerCard;

            var damageDealerCard = Classification.GetOwnHandCards(p, boardObjType.MOB, SpecificCardType.MobsDamageDealer).FirstOrDefault();
            if (damageDealerCard != null && damageDealerCard.manacost <= p.ownMana)
                return damageDealerCard;

            if((int)currentSituation >= 3 && (int)currentSituation <= 6)
                return Classification.GetOwnHandCards(p, boardObjType.MOB, SpecificCardType.MobsUnderAttack).FirstOrDefault();

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


        private static Handcard Building(Playfield p)
        {
            Logger.Debug("Path: Spell - Building");
            var buildingCard = p.ownHandCards.Where(n => n.card.type == boardObjType.BUILDING).FirstOrDefault();
            if (buildingCard != null)
                return new Handcard(buildingCard.name, buildingCard.lvl);

            return null;
        }


        // TODO: Check this out
        public static Handcard DamagingSpellDecision(Playfield p, out VectorAI choosedPosition)
        {
            choosedPosition = null;

            IEnumerable<Handcard> damagingSpells = Classification.GetOwnHandCards(p, boardObjType.PROJECTILE, SpecificCardType.SpellsDamaging);

            if (damagingSpells.FirstOrDefault() == null)
                return null;

            Logger.Debug("Damaging-Spell: tower damage first card = " + damagingSpells.FirstOrDefault().card?.towerDamage);

            if (p.suddenDeath)
            {
                IEnumerable<Handcard> ds3 = damagingSpells.Where(n => (n.card.towerDamage >= p.enemyPrincessTower1.HP));
                IEnumerable<Handcard> ds4 = damagingSpells.Where(n => (n.card.towerDamage >= p.enemyPrincessTower2.HP));
                IEnumerable<Handcard> ds5 = damagingSpells.Where(n => (n.card.towerDamage >= p.enemyKingsTower.HP));


                if (ds3.FirstOrDefault() != null && p.enemyPrincessTower1.HP > 0)
                {
                    Logger.Debug("towerDamage: {td} ; pt1.hp: {pt1hp}", ds3.FirstOrDefault().card.towerDamage,
                        p.enemyPrincessTower1.HP);
                    choosedPosition = p.enemyPrincessTower1.Position;
                    return ds3.FirstOrDefault();
                }

                if (ds4.FirstOrDefault() != null && p.enemyPrincessTower2.HP > 0)
                {
                    Logger.Debug("towerDamage: {td} ; pt1.hp: {pt1hp}", ds4.FirstOrDefault().card.towerDamage,
                        p.enemyPrincessTower2.HP);
                    choosedPosition = p.enemyPrincessTower2.Position;
                    return ds4.FirstOrDefault();
                }

                if (ds5.FirstOrDefault() != null)
                {
                    Logger.Debug("towerDamage: {td} ; pt1.hp: {pt1hp}", ds5.FirstOrDefault().card.towerDamage,
                        p.enemyKingsTower.HP);
                    choosedPosition = p.enemyKingsTower.Position;
                    return ds5.FirstOrDefault();
                }
            }

            IOrderedEnumerable<Handcard> radiusOrderedDS = damagingSpells.OrderBy(n => n.card.DamageRadius);

            group Group = p.getGroup(false, 200, boPriority.byTotalNumber, radiusOrderedDS.LastOrDefault().card.DamageRadius);

            if (Group == null)
                return null;

            int grpCount = Group.lowHPbo.Count() + Group.avgHPbo.Count() + Group.hiHPbo.Count();
            int hpSum = Group.lowHPboHP + Group.hiHPboHP + Group.avgHPboHP;

            IEnumerable<Handcard> ds1 = damagingSpells.Where(n => n.card.DamageRadius > 3 && grpCount > 4);

            if (ds1.FirstOrDefault() != null)
            {
                Logger.Debug("Damaging-Spell-Decision: HP-Sum of group = " + hpSum);
                choosedPosition = p.getDeployPosition(Group.Position, deployDirectionRelative.Down, 1000);
                return ds1.FirstOrDefault();
            }

            IEnumerable<Handcard> ds2 = damagingSpells.Where(n => n.card.DamageRadius <= 3 && grpCount > 1 && hpSum >= n.card.Atk * 2);

            if (ds2.FirstOrDefault() != null)
            {
                Logger.Debug("Damaging-Spell-Decision: HP-Sum of group = " + hpSum);
                choosedPosition = p.getDeployPosition(Group.Position, deployDirectionRelative.Down, 1000);
                return ds2.FirstOrDefault();
            }

            return null;


        }

        public static bool DeployBuildingDecision(Playfield p, out VectorAI choosedPosition)
        {
            choosedPosition = p.getDeployPosition(p.ownKingsTower, deployDirectionRelative.Up, 4000);
            return Helper.IsAnEnemyObjectInArea(p, choosedPosition, 3000, boardObjType.MOB);
        }


        private static Handcard AOEDecision(Playfield p, out VectorAI choosedPosition, FightState currentSituation)
        {
            choosedPosition = null;
            Handcard aoeGround = null, aoeAir = null;

            BoardObj objGround = Helper.EnemyCharacterWithTheMostEnemiesAround(p, out int biggestEnemieGroupCount, transportType.GROUND);
            if (biggestEnemieGroupCount > 3)
                aoeGround = Classification.GetOwnHandCards(p, boardObjType.MOB, SpecificCardType.MobsAOEGround).FirstOrDefault();

            BoardObj objAir = Helper.EnemyCharacterWithTheMostEnemiesAround(p, out biggestEnemieGroupCount, transportType.AIR);
            if (biggestEnemieGroupCount > 3)
                aoeAir = Classification.GetOwnHandCards(p, boardObjType.MOB, SpecificCardType.MobsAOEAll).FirstOrDefault();

            switch (currentSituation)
            {
                case FightState.DPTL1:
                case FightState.UAPTL1:
                    choosedPosition = p.getDeployPosition(deployDirectionAbsolute.ownPrincessTowerLine1);
                    if (aoeAir != null)
                        return aoeAir;

                    return aoeGround;
                case FightState.APTL1:
                    choosedPosition = p.getDeployPosition(deployDirectionAbsolute.enemyPrincessTowerLine1);
                    if (aoeAir != null)
                        return aoeAir;

                    return aoeGround;
                case FightState.DPTL2:
                case FightState.UAPTL2:
                    choosedPosition = p.getDeployPosition(deployDirectionAbsolute.ownPrincessTowerLine2);
                    if (aoeAir != null)
                        return aoeAir;

                    return aoeGround;
                case FightState.APTL2:
                    choosedPosition = p.getDeployPosition(deployDirectionAbsolute.enemyPrincessTowerLine2);
                    if (aoeAir != null)
                        return aoeAir;

                    return aoeGround;
                case FightState.DKT:
                case FightState.UAKTL1:
                case FightState.UAKTL2:
                    if (aoeAir != null)
                    {
                        choosedPosition = objAir.Line == 1 ? p.getDeployPosition(deployDirectionAbsolute.behindKingsTowerLine1)
                            : p.getDeployPosition(deployDirectionAbsolute.behindKingsTowerLine2);
                        return aoeAir;

                    }

                    if (aoeGround != null)
                    {
                        choosedPosition = objGround.Line == 1 ? p.getDeployPosition(deployDirectionAbsolute.behindKingsTowerLine1)
                            : p.getDeployPosition(deployDirectionAbsolute.behindKingsTowerLine2);
                    }
                    return aoeGround;
                case FightState.AKT:
                    choosedPosition = p.enemyKingsTower.Line == 3 ? p.enemyKingsTower.Position
                        : p.enemyKingsTower.Line == 1 ? p.getDeployPosition(deployDirectionAbsolute.enemyPrincessTowerLine1)
                        : p.getDeployPosition(deployDirectionAbsolute.enemyPrincessTowerLine2);

                    if (aoeAir != null)
                        return aoeAir;

                    return aoeGround;
            }
            return null;
        }
        #endregion

        public static Handcard GetOppositeCard(Playfield p, FightState currentSituation)
        {
            // Debugging: try - catch is just for debugging
            try
            {
                if (p.enemyKingsTower.HP < Apollo.Setting.KingTowerSpellDamagingHealth)
                {
                    Handcard hc = AttackKingTowerWithSpell(p);

                    if (hc != null)
                        return hc;
                }
            }
            catch (Exception) { }

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
            }
            return null;
        }
    }
}
