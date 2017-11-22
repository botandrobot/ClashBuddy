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

        public static IEnumerable<Handcard> GetOwnHandCards(Playfield p, boardObjType cardType, SpecificCardType sCardType)
        {
            var cardsOfType = p.ownHandCards.Where(n => n.card.type == cardType).ToArray();

            if (cardsOfType.Length == 0)
                return cardsOfType;

            Func<Handcard, bool> @delegate = (n) => true;
            
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
                case SpecificCardType.MobsAOEGround:
                    @delegate = IsMobsAOEGround;
                    break;
                case SpecificCardType.MobsAOEAll:
                    @delegate = IsMobsAOEAll;
                    break;
                case SpecificCardType.MobsFlyingAttack:
                    @delegate = IsMobsFlyingAttack;
                    break;
                case SpecificCardType.MobsNoTank:
                    @delegate = IsMobsNoTank;
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

            return cardsOfType.Where(@delegate).ToArray();
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
                    if (IsMobsDamageDealer(hc) && IsMobsAOEGround(hc))  return SpecificCardType.MobsDamageDealerAOE;
                    if (IsMobsAOEAll(hc))                               return SpecificCardType.MobsAOEAll;
                    if (IsMobsAOEGround(hc))                            return SpecificCardType.MobsAOEGround;
                    if (IsMobsBuildingAttacker(hc))                     return SpecificCardType.MobsBuildingAttacker;
                    if (IsMobsDamageDealer(hc))                         return SpecificCardType.MobsDamageDealer;
                    if (IsMobsBigGroup(hc))                             return SpecificCardType.MobsBigGroup;
                    if (IsMobsFlyingAttack(hc))                         return SpecificCardType.MobsFlyingAttack;
                    if (IsMobsTank(hc))                                 return SpecificCardType.MobsTank;
                    if (IsMobsNoTank(hc))                               return SpecificCardType.MobsNoTank;
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
        public static Func<Handcard, bool> IsMobsNoTank = (Handcard hc) => (hc.card.TargetType != targetType.BUILDINGS && hc.card.MaxHP < Setting.MinHealthAsTank);
        public static Func<Handcard, bool> IsMobsFlyingAttack = (Handcard hc) => (hc.card.TargetType == targetType.ALL);
        public static Func<Handcard, bool> IsMobsAOEAll = (Handcard hc) => (hc.card.aoeAir);
        public static Func<Handcard, bool> IsMobsAOEGround = (Handcard hc) => (hc.card.aoeGround);
        public static Func<Handcard, bool> IsMobsRanger = (Handcard hc) => (hc.card.MaxRange > 4500);
        public static Func<Handcard, bool> IsMobsBuildingAttacker = (Handcard hc) => (hc.card.TargetType == targetType.BUILDINGS);
        public static Func<Handcard, bool> IsMobsDamageDealer = (Handcard hc) => ((hc.card.Atk * hc.card.SummonNumber) > 100);
        public static Func<Handcard, bool> IsMobsBigGroup = (Handcard hc) => ((hc.card.SummonNumber >= 8 ));
        public static Func<Handcard, bool> IsMobsTank = (Handcard hc) => (hc.card.MaxHP >= Setting.MinHealthAsTank);
        public static Func<BoardObj, bool> IsMobsTankCurrentHP = (BoardObj bo) => (bo.HP >= Setting.MinHealthAsTank);
        
        public static Func<Handcard, bool> IsCycleCard = (Handcard hc) => (hc.manacost <= 2);
        public static Func<Handcard, bool> IsPowerCard = (Handcard hc) => (hc.manacost >= 5);
    }
}
