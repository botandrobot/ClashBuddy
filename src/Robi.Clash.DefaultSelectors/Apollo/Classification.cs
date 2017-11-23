using Robi.Common;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Robi.Clash.DefaultSelectors.Apollo
{
    class Classification
    {
        private static readonly ILogger Logger = LogProvider.CreateLogger<Classification>();

        public static IEnumerable<Handcard> GetOwnHandCards(Playfield p, boardObjType cardType, SpecificCardType sCardType, MoreSpecificMobCardType msCardType = MoreSpecificMobCardType.None)
        {
            var cardsOfType = p.ownHandCards.Where(n => n.card.type == cardType).ToArray();

            if (cardsOfType.Length == 0)
                return cardsOfType;

            Func<Handcard, bool> @delegate = (n) => true;
            Func<Handcard, bool> @msDelegate = (n) => true;

            switch (sCardType)
            {
                case SpecificCardType.All:
                    break;

                // Mobs
                case SpecificCardType.MobsTank:
                    @delegate = IsMobsTank;
                    break;
                case SpecificCardType.MobsRanger:
                    @delegate = IsMobsRanger;
                    break;
                case SpecificCardType.MobsBigGroup:
                    @delegate = IsMobsBigGroup;
                    break;
                case SpecificCardType.MobsDamageDealer:
                    @delegate = IsMobsDamageDealer;
                    break;
                case SpecificCardType.MobsBuildingAttacker:
                    @delegate = IsMobsBuildingAttacker;
                    break;
                case SpecificCardType.MobsFlyingAttack:
                    @delegate = IsMobsFlyingAttack;
                    break;

                // Buildings
                case SpecificCardType.BuildingsDefense:
                    @delegate = IsBuildingsDefense; // TODO: Define
                    break;
                case SpecificCardType.BuildingsAttack:
                    @delegate = IsBuildingsAttack; // TODO: Define
                    break;
                case SpecificCardType.BuildingsSpawning:
                    @delegate = IsBuildingsSpawning;
                    break;
                case SpecificCardType.BuildingsMana:
                    @delegate = (n) => false;
                    break; // TODO: ManaProduction

                // Spells
                case SpecificCardType.SpellsDamaging:
                    @delegate = IsSpellsDamaging;
                    break;
                case SpecificCardType.SpellsNonDamaging:
                    @delegate = IsSpellsNonDamaging;
                    break;
                case SpecificCardType.SpellsTroopSpawning:
                    @delegate = IsSpellsTroopSpawning; // TODO: Check
                    break;
                case SpecificCardType.SpellsBuffs:
                    @delegate = IsSpellBuff; // TODO: Check
                    break;
                default:
                    @delegate = (n) => false;
                    break;
            }

            switch (msCardType)
            {
                case MoreSpecificMobCardType.None:
                    break;
                case MoreSpecificMobCardType.ShortDistance:
                    @msDelegate = IsShortDistance;
                    break;
                case MoreSpecificMobCardType.LongDistance:
                    @msDelegate = IsLongDistance;
                    break;
                case MoreSpecificMobCardType.BuildingAttacker:
                    @msDelegate = IsBuildingsAttack;
                    break;
                case MoreSpecificMobCardType.AOEGround:
                    @msDelegate = IsAOEGround;
                    break;
                case MoreSpecificMobCardType.AOEAll:
                    @msDelegate = IsAOEAll;
                    break;
                case MoreSpecificMobCardType.FlyingAttack:
                    @msDelegate = IsMobsFlyingAttack;
                    break;
                case MoreSpecificMobCardType.Flying:
                    @msDelegate = IsFlying;
                    break;
                case MoreSpecificMobCardType.NotFlying:
                    @msDelegate = IsNotFlying;
                    break;
                case MoreSpecificMobCardType.DamageDealer:
                    @msDelegate = IsMobsDamageDealer;
                    break;
            }

            if (@msDelegate == null)
                return cardsOfType.Where(@delegate).ToArray();
            else return cardsOfType.Where(@delegate).Where(msDelegate).ToArray();
        }

        public static MoreSpecificMobCardType GetMoreSpecificCardType(Handcard hc, SpecificCardType specificCardType)
        {
            switch(specificCardType)
            {
                case SpecificCardType.MobsRanger:
                    return (IsShortDistance(hc)) ? MoreSpecificMobCardType.ShortDistance : MoreSpecificMobCardType.LongDistance;
                case SpecificCardType.MobsTank:
                    return (IsMobsBuildingAttacker(hc) ? MoreSpecificMobCardType.BuildingAttacker : IsFlying(hc) ?
                                                         MoreSpecificMobCardType.Flying : MoreSpecificMobCardType.NotFlying);
                case SpecificCardType.MobsFlying:
                    return (IsMobsDamageDealer(hc) ? MoreSpecificMobCardType.DamageDealer : MoreSpecificMobCardType.Flying);
                case SpecificCardType.MobsDamageDealer:
                    return (IsMobsAOE(hc)) ? MoreSpecificMobCardType.AOEGround : (IsMobsFlyingAttack(hc)) ? MoreSpecificMobCardType.FlyingAttack
                                                : (IsFlying(hc)) ? MoreSpecificMobCardType.Flying : MoreSpecificMobCardType.NotFlying;
                case SpecificCardType.MobsBigGroup:

                    break;
                case SpecificCardType.MobsAOE:
                    return (IsAOEAll(hc)) ? MoreSpecificMobCardType.AOEAll : MoreSpecificMobCardType.AOEGround;
                case SpecificCardType.MobsBuildingAttacker:

                    break;
                case SpecificCardType.MobsFlyingAttack:

                    break;
            }
            return MoreSpecificMobCardType.None;
        }

        public static SpecificCardType GetSpecificCardType(Handcard hc)
        {
            switch (hc.card.type)
            {
                case boardObjType.BUILDING:
                    if (IsBuildingsDefense(hc))                         return SpecificCardType.BuildingsDefense;
                    if (IsBuildingsAttack(hc))                          return SpecificCardType.BuildingsAttack;
                    if (IsBuildingsMana(hc))                            return SpecificCardType.BuildingsMana;
                    if (IsBuildingsSpawning(hc))                        return SpecificCardType.BuildingsSpawning;
                    return SpecificCardType.All;
                case boardObjType.MOB:
                    if (IsMobsRanger(hc))                               return SpecificCardType.MobsRanger;
                    if (IsMobsAOE(hc))                                  return SpecificCardType.MobsAOE;
                    if (IsMobsBuildingAttacker(hc))                     return SpecificCardType.MobsBuildingAttacker;
                    if (IsMobsDamageDealer(hc))                         return SpecificCardType.MobsDamageDealer;
                    if (IsMobsBigGroup(hc))                             return SpecificCardType.MobsBigGroup;
                    if (IsMobsFlyingAttack(hc))                         return SpecificCardType.MobsFlyingAttack;
                    if (IsMobsTank(hc))                                 return SpecificCardType.MobsTank;
                    return SpecificCardType.All;
                case boardObjType.AOE:
                case boardObjType.PROJECTILE:
                    if (IsSpellBuff(hc))                                return SpecificCardType.SpellsBuffs;
                    if (IsSpellsDamaging(hc))                           return SpecificCardType.SpellsDamaging;
                    if (IsSpellsNonDamaging(hc))                        return SpecificCardType.SpellsNonDamaging;
                    if (IsSpellsTroopSpawning(hc))                      return SpecificCardType.SpellsTroopSpawning;
                    return SpecificCardType.All;
                default:
                    return SpecificCardType.All;
            }
        }

        // Conditions
        // Spells
        public static Func<Handcard, bool> IsSpellBuff = (Handcard hc) => (hc.card.affectType == affectType.ONLY_OWN); 
        public static Func<Handcard, bool> IsSpellsTroopSpawning = (Handcard hc) => (hc.card.SpawnNumber > 0);
        public static Func<Handcard, bool> IsSpellsNonDamaging = (Handcard hc) => (hc.card.DamageRadius == 0);
        public static Func<Handcard, bool> IsSpellsDamaging = (Handcard hc) => (hc.card.DamageRadius > 0);

        //Buildings
        public static Func<Handcard, bool> IsBuildingsDefense = (Handcard hc) => (hc.card.Atk > 0);
        public static Func<Handcard, bool> IsBuildingsAttack = (Handcard hc) => (hc.card.Atk > 0);
        public static Func<Handcard, bool> IsBuildingsSpawning = (Handcard hc) => (hc.card.SpawnNumber > 0);
        public static Func<Handcard, bool> IsBuildingsMana = (Handcard hc) => (false); // ToDo: Implement mana production

        // Mobs
        //public static Func<Handcard, bool> IsMobsNoTank = (Handcard hc) => (hc.card.TargetType != targetType.BUILDINGS && hc.card.MaxHP < Setting.MinHealthAsTank);
        public static Func<Handcard, bool> IsMobsFlyingAttack = (Handcard hc) => (hc.card.TargetType == targetType.ALL);
        public static Func<Handcard, bool> IsMobsAOE = (Handcard hc) => (hc.card.aoeGround);
        public static Func<Handcard, bool> IsMobsRanger = (Handcard hc) => (hc.card.MaxRange > 1);
        public static Func<Handcard, bool> IsMobsBuildingAttacker = (Handcard hc) => (hc.card.TargetType == targetType.BUILDINGS);
        public static Func<Handcard, bool> IsMobsDamageDealer = (Handcard hc) => ((hc.card.Atk * hc.card.SummonNumber) > 100);
        public static Func<Handcard, bool> IsMobsBigGroup = (Handcard hc) => ((hc.card.SummonNumber >= 8 ));
        public static Func<Handcard, bool> IsMobsTank = (Handcard hc) => (hc.card.MaxHP >= Setting.MinHealthAsTank);
        
        public static Func<Handcard, bool> IsCycleCard = (Handcard hc) => (hc.manacost <= 2);
        public static Func<Handcard, bool> IsPowerCard = (Handcard hc) => (hc.manacost >= 5);

        // More Specific
        public static Func<Handcard, bool> IsShortDistance = (Handcard hc) => (hc.card.MaxRange <= 4500);
        public static Func<Handcard, bool> IsLongDistance = (Handcard hc) => (hc.card.MaxRange > 4500);
        public static Func<Handcard, bool> IsAOEAll = (Handcard hc) => (hc.card.aoeAir);
        public static Func<Handcard, bool> IsAOEGround = (Handcard hc) => (hc.card.aoeGround);
        public static Func<Handcard, bool> IsFlying = (Handcard hc) => (hc.card.Transport == transportType.AIR);
        public static Func<Handcard, bool> IsNotFlying = (Handcard hc) => (hc.card.Transport == transportType.GROUND);


        // With BoardObj, no Handcard
        public static Func<BoardObj, bool> IsMobsTankCurrentHP = (BoardObj bo) => (bo.HP >= Setting.MinHealthAsTank);



    }
}
