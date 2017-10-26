using Robi.Common;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Robi.Clash.DefaultSelectors.Apollo
{
    class Classification
    {
        private static readonly ILogger Logger = LogProvider.CreateLogger<Classification>();

        public static IEnumerable<Handcard> GetOwnHandCards(Playfield p, boardObjType cardType, SpecificCardType sCardType)
        {
            IEnumerable<Handcard> cardsOfType = p.ownHandCards.Where(n => n.card.type == cardType);

            if (cardsOfType.FirstOrDefault() == null)
                return cardsOfType;

            switch (sCardType)
            {
                case SpecificCardType.All:
                    return cardsOfType;

                // Mobs
                case SpecificCardType.MobsTank:
                    return cardsOfType.Where(n => IsMobsTank(n));
                case SpecificCardType.MobsDamageDealer:
                    return cardsOfType.Where(n => IsMobsDamageDealer(n));
                case SpecificCardType.MobsBuildingAttacker:
                    return cardsOfType.Where(n => IsMobsBuildingAttacker(n));
                case SpecificCardType.MobsRanger:
                    return cardsOfType.Where(n => IsMobsRanger(n));
                case SpecificCardType.MobsAOEGround:
                    return cardsOfType.Where(n => IsMobsAOEGround(n));
                case SpecificCardType.MobsAOEAll:
                    return cardsOfType.Where(n => IsMobsAOEAll(n));
                case SpecificCardType.MobsFlyingAttack:
                    return cardsOfType.Where(n => IsMobsFlyingAttack(n));
                case SpecificCardType.MobsNoTank:
                    return cardsOfType.Where(n => IsMobsNoTank(n));

                // Buildings
                case SpecificCardType.BuildingsDefense:
                    return cardsOfType.Where(n => IsBuildingsDefense(n)); // TODO: Define
                case SpecificCardType.BuildingsAttack:
                    return cardsOfType.Where(n => IsBuildingsAttack(n)); // TODO: Define
                case SpecificCardType.BuildingsSpawning:
                    return cardsOfType.Where(n => IsBuildingsSpawning(n));
                case SpecificCardType.BuildingsMana:
                    break; // TODO: ManaProduction

                // Spells
                case SpecificCardType.SpellsDamaging:
                    return cardsOfType.Where(n => IsSpellsDamaging(n));
                case SpecificCardType.SpellsNonDamaging:
                    return cardsOfType.Where(n => IsSpellsNonDamaging(n));
                case SpecificCardType.SpellsTroopSpawning:
                    return cardsOfType.Where(n => IsSpellsTroopSpawning(n)); // TODO: Check
                case SpecificCardType.SpellsBuffs:
                    return cardsOfType.Where(n => IsSpellBuff(n)); // TODO: Check
            }

            return null;
        }

        public static SpecificCardType GetSpecificCardType(Handcard hc)
        {
            switch (hc.card.type)
            {
                case boardObjType.BUILDING:
                    if (IsBuildingsDefense(hc))         return SpecificCardType.BuildingsDefense;
                    if (IsBuildingsAttack(hc))          return SpecificCardType.BuildingsAttack;
                    if (IsBuildingsMana(hc))            return SpecificCardType.BuildingsMana;
                    if (IsBuildingsSpawning(hc))        return SpecificCardType.BuildingsSpawning;
                    return SpecificCardType.All;
                case boardObjType.MOB:
                    if (IsMobsAOEAll(hc))               return SpecificCardType.MobsAOEAll;
                    if (IsMobsAOEGround(hc))            return SpecificCardType.MobsAOEGround;
                    if (IsMobsBuildingAttacker(hc))     return SpecificCardType.MobsBuildingAttacker;
                    if (IsMobsDamageDealer(hc))         return SpecificCardType.MobsDamageDealer;
                    if (IsMobsFlyingAttack(hc))         return SpecificCardType.MobsFlyingAttack;
                    if (IsMobsRanger(hc))               return SpecificCardType.MobsRanger;
                    if (IsMobsTank(hc))                 return SpecificCardType.MobsTank;
                    if (IsMobsNoTank(hc))               return SpecificCardType.MobsNoTank;
                    return SpecificCardType.All;
                case boardObjType.AOE:
                case boardObjType.PROJECTILE:
                    if (IsSpellBuff(hc))                return SpecificCardType.SpellsBuffs;
                    if (IsSpellsDamaging(hc))           return SpecificCardType.SpellsDamaging;
                    if (IsSpellsNonDamaging(hc))        return SpecificCardType.SpellsNonDamaging;
                    if (IsSpellsTroopSpawning(hc))      return SpecificCardType.SpellsTroopSpawning;
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
        public static Func<Handcard, bool> IsMobsTank = (Handcard hc) => (hc.card.MaxHP >= Setting.MinHealthAsTank);
        public static Func<BoardObj, bool> IsMobsTankCurrentHP = (BoardObj bo) => (bo.HP >= Setting.MinHealthAsTank);
        
        public static Func<Handcard, bool> IsCycleCard = (Handcard hc) => (hc.manacost <= 2);
        public static Func<Handcard, bool> IsPowerCard = (Handcard hc) => (hc.manacost >= 5);
    }
}
