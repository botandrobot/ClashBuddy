using Robi.Clash.Engine;
using Robi.Clash.Engine.NativeObjects.LogicData;
using Robi.Clash.DefaultSelectors.Game;
using Robi.Clash.DefaultSelectors.Enemy;
using Robi.Common;
using Serilog;
using System.Collections.Concurrent;
using System.Linq;
using Robi.Clash.DefaultSelectors.Card;

namespace Robi.Clash.DefaultSelectors.Logic
{
    class CastDeploymentHandling
    {
        private static readonly ILogger Logger = LogProvider.CreateLogger<CastDeploymentHandling>();
        private static readonly ConcurrentQueue<string> _spellQueue = new ConcurrentQueue<string>();
        private static IOrderedEnumerable<Spell> Spells;

        public static ICard SpellMagic(FightState gameState)
        {
            if (ClashEngine.Instance.LocalPlayer == null) return null;
            Spells = CardClassifying.Troop;

            CardTypeOld cardTypeToPlay = ChooseCardType(gameState);

            switch (cardTypeToPlay)
            {
                case CardTypeOld.All:
                    return EarlyCycle();
                case CardTypeOld.Defense:
                    return Defense();
                case CardTypeOld.Troop:
                    return DefenseTroop();
                case CardTypeOld.NONE:
                    return null;
            }
            return null;
        }

        public static CardTypeOld ChooseCardType(FightState gameState)
        {
            switch (gameState)
            {
                case FightState.UAKT:
                case FightState.UALPT:
                case FightState.UARPT:
                    return CardTypeOld.Defense;
                case FightState.AKT:
                case FightState.ALPT:
                case FightState.ARPT:
                    return CardTypeOld.All;
                case FightState.DKT:
                case FightState.DLPT:
                case FightState.DRPT:
                    return CardTypeOld.Troop;
                case FightState.START:
                    return CardTypeOld.NONE;
                case FightState.WAIT:
                    return CardTypeOld.NONE;
                default:
                    return CardTypeOld.All;
            }
        }

        public static bool DamagingSpellDecision()
        {
            int count = 0;
            EnemyCharacterHandling.EnemyCharacterWithTheMostEnemiesAround(out count);

            /*
            Logger.Debug("enemyWhithTheMostEnemiesAround-Count: {count} enemy-Name {name}", count
                         , enemy.LogicGameObjectData.Name.Value);
                         */
            if (count > GameHandling.Settings.SpellDeployConditionCharCount)
                return true;

            return false;
        }

        public static Card.ICard EarlyCycle()
        {
            IOrderedEnumerable<Spell> troopCycleSpells = CardClassifying.TroopCycleCards;
            IOrderedEnumerable<Spell> damagingSpells = CardClassifying.Damaging;

            if (DamagingSpellDecision())
            {
                var damagingSpell = damagingSpells.FirstOrDefault();

                if (damagingSpell != null)
                    return new CardSpell(damagingSpell.Name.Value, SpellType.SpellDamaging);
            }

            if (IsAOEAttackNeeded())
            {
                var spell = CardClassifying.TroopAOEAttack.FirstOrDefault();
                if (spell == null)
                    spell = CardClassifying.TroopGroundAttack.FirstOrDefault();

                if (spell != null)
                    return new CardCharacter(spell.Name.Value, TroopType.AOEAttackGround);
            }

            if(IsFlyingAttackNeeded())
            {
                var spell = CardClassifying.TroopAirAttack.FirstOrDefault();
                if (spell != null)
                    return new CardCharacter(spell.Name.Value, TroopType.AOEAttackFlying);
            }

            // ToDo: Ranger, Flying usw.
            if (troopCycleSpells.Count() > 1)
            {
                var spell = troopCycleSpells.FirstOrDefault();
                return new CardCharacter(spell.Name.Value, TroopType.Ranger);
            }

            return TroopPowerSpells();
        }

        public static ICard DefenseTroop()
        {
            if (IsAOEAttackNeeded())
            {
                var spell = CardClassifying.TroopAOEAttack.FirstOrDefault();
                if (spell != null)
                    return new CardCharacter(spell.Name.Value, TroopType.AOEAttackGround);
            }

            if (IsFlyingAttackNeeded())
            {
                var spell = CardClassifying.TroopAirAttack.FirstOrDefault();

                if (spell != null)
                    return new CardCharacter(spell.Name.Value, TroopType.AOEAttackFlying);
            }

            // ToDo: Wann Tanks einsetzen usw.?
            return new CardCharacter(CardClassifying.Troop.FirstOrDefault().Name.Value, TroopType.Ranger);
        }

        public static ICard Defense()
        {
            IOrderedEnumerable<Spell> damagingSpells = CardClassifying.Damaging;

            if (DamagingSpellDecision())
            {
                var damagingSpell = damagingSpells.FirstOrDefault();

                if (damagingSpell != null)
                    return new CardSpell(damagingSpell.Name.Value, SpellType.SpellDamaging);
            }

            if (IsAOEAttackNeeded())
            {
                var spell = CardClassifying.TroopAOEAttack.FirstOrDefault();

                if (spell != null)
                    return new CardCharacter(spell.Name.Value, TroopType.AOEAttackGround);
            }

            if (IsFlyingAttackNeeded())
            {
                var spell = CardClassifying.TroopAirAttack.FirstOrDefault();

                if (spell != null)
                    return new CardCharacter(spell.Name.Value, TroopType.AirAttack);
            }

            {
                var spell = CardClassifying.TroopCycleCards.FirstOrDefault();

                // ToDo: Wann Tanks einsetzen usw.?
                if (spell != null)
                    return new CardCharacter(spell.Name.Value, TroopType.Ranger);

                // Maybe also flying possible
                return TroopPowerSpells();
            }
        }


        public static bool IsAOEAttackNeeded()
        {
            int biggestEnemieGroupCount;
            Engine.NativeObjects.Logic.GameObjects.Character @char = 
                EnemyCharacterHandling.EnemyCharacterWithTheMostEnemiesAround(out biggestEnemieGroupCount);

            if (biggestEnemieGroupCount > 3)
                return true;

            return false;
        }

        public static bool IsFlyingAttackNeeded()
        {
            return EnemyCharacterHandling.IsFlyingEnemyOnTheField();
        }

        private static CardCharacter TroopPowerSpells()
        {
            IOrderedEnumerable<Spell> troopPowerSpells = CardClassifying.TroopPowerCards;

            foreach (var s in troopPowerSpells)
            {
                if (_spellQueue.Count < 1)
                {
                    _spellQueue.Enqueue(s.Name.Value);
                }
                else
                {
                    // Maybe also flying possible
                    if (CSVCardClassifying.IsTank(s.Name.Value))
                        return new CardCharacter(s.Name.Value, TroopType.Tank);

                    return new CardCharacter(s.Name.Value, TroopType.Damager);
                }
            }
            return null;
        }
    }
}
