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
                    return cardsOfType.Where(n => n.card.MaxHP >= Setting.MinHealthAsTank);
                case SpecificCardType.MobsDamageDealer:
                    return cardsOfType.Where(n => (n.card.Atk * n.card.SummonNumber) > 100);
                case SpecificCardType.MobsBuildingAttacker:
                    return cardsOfType.Where(n => n.card.TargetType == targetType.BUILDINGS);

                case SpecificCardType.MobsRanger:
                    return cardsOfType.Where(n => n.card.MaxRange > 4500);
                case SpecificCardType.MobsAOEGround:
                    return cardsOfType.Where(n => n.card.aoeGround);
                case SpecificCardType.MobsAOEAll:
                    return cardsOfType.Where(n => n.card.aoeAir);
                case SpecificCardType.MobsFlyingAttack:
                    return cardsOfType.Where(n => n.card.TargetType == targetType.ALL);
                case SpecificCardType.MobsUnderAttack:
                    return cardsOfType.Where(n => n.card.TargetType != targetType.BUILDINGS && n.card.MaxHP < Setting.MinHealthAsTank);

                // Buildings
                case SpecificCardType.BuildingsDefense:
                    return cardsOfType.Where(n => n.card.Atk > 0); // TODO: Define
                case SpecificCardType.BuildingsAttack:
                    return cardsOfType.Where(n => n.card.Atk > 0); // TODO: Define
                case SpecificCardType.BuildingsSpawning:
                    return cardsOfType.Where(n => n.card.SpawnNumber > 0);
                case SpecificCardType.BuildingsMana:
                    break; // TODO: ManaProduction


                // Spells
                case SpecificCardType.SpellsDamaging:
                    return cardsOfType.Where(n => n.card.DamageRadius > 0);
                case SpecificCardType.SpellsNonDamaging:
                    return cardsOfType.Where(n => n.card.DamageRadius == 0);
                case SpecificCardType.SpellsTroopSpawning:
                    return cardsOfType.Where(n => n.card.SpawnNumber > 0); // TODO: Check
                case SpecificCardType.SpellsBuffs:
                    return cardsOfType.Where(n => n.card.affectType == affectType.ONLY_OWN); // TODO: Check

            }

            return null;
        }
    }
}
