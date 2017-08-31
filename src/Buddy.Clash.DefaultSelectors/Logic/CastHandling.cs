using Buddy.Clash.Engine;
using Buddy.Clash.Engine.NativeObjects.Logic.GameObjects;
using Buddy.Clash.Engine.NativeObjects.LogicData;
using Buddy.Clash.Engine.NativeObjects.Native;
using Buddy.Clash.DefaultSelectors.Game;
using Buddy.Clash.DefaultSelectors.Utilities;
using Buddy.Clash.DefaultSelectors.Enemy;
using Buddy.Common;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Buddy.Clash.DefaultSelectors.Logic;
using Buddy.Clash.DefaultSelectors.Player;

namespace Buddy.Clash.DefaultSelectors.Logic
{
    class CastHandling
    {
        private static readonly ILogger Logger = LogProvider.CreateLogger<CastHandling>();
        private static readonly ConcurrentQueue<string> _spellQueue = new ConcurrentQueue<string>();
        private static IOrderedEnumerable<Spell> Spells;

        public static CastRequest SpellMagic(Vector2f nextPosition, FightState gameState)
        {
            if (ClashEngine.Instance.LocalPlayer == null) return null;
            Spells = PlayerCardClassifying.Troop;

            CardType cardTypeToPlay = ChooseCardType(gameState);

            switch (cardTypeToPlay)
            {
                case CardType.All:
                    return EarlyCycle(nextPosition);
                case CardType.Defense:
                    return Defense(nextPosition);
                case CardType.Troop:
                    return DefenseTroop(nextPosition);
                case CardType.NONE:
                    return null;
            }
            return null;
        }

        public static CardType ChooseCardType(FightState gameState)
        {
            switch (gameState)
            {
                case FightState.UAKT:
                case FightState.UALPT:
                case FightState.UARPT:
                    return CardType.Defense;
                case FightState.AKT:
                case FightState.ALPT:
                case FightState.ARPT:
                    return CardType.All;
                case FightState.DKT:
                case FightState.DLPT:
                case FightState.DRPT:
                    return CardType.Troop;
                case FightState.START:
                    return CardType.NONE;
                default:
                    return CardType.All;
            }
        }

        public static bool DamagingSpellDecision(out Engine.NativeObjects.Logic.GameObjects.Character enemy)
        {
            int count = 0;
            enemy = EnemyCharacterHandling.EnemyCharacterWithTheMostEnemiesAround(out count);

            /*
            Logger.Debug("enemyWhithTheMostEnemiesAround-Count: {count} enemy-Name {name}", count
                         , enemy.LogicGameObjectData.Name.Value);
                         */
            if (count >= GameHandling.Settings.SpellDeployConditionCharCount)
                return true;

            return false;
        }

        public static CastRequest EarlyCycle(Vector2f nextPosition)
        {
            IOrderedEnumerable<Spell> troopCycleSpells = PlayerCardClassifying.TroopCycleCards;
            IOrderedEnumerable<Spell> troopPowerSpells = PlayerCardClassifying.TroopPowerCards;
            IOrderedEnumerable<Spell> damagingSpells = PlayerCardClassifying.Damaging;

            Vector2f spellPosition = CastPositionHandling.GetPositionOfTheBestDamagingSpellDeploy();
            if (!spellPosition.Equals(Vector2f.Zero))
            {
                var damagingSpell = damagingSpells.FirstOrDefault();

                if (damagingSpell != null)
                    return new CastRequest(damagingSpell.Name.Value, spellPosition);
            }

            if (IsAOEAttackNeeded())
            {
                var spell = PlayerCardClassifying.TroopAOEAttack.FirstOrDefault();
                if (spell == null)
                    spell = PlayerCardClassifying.TroopGroundAttack.FirstOrDefault();

                if (spell != null)
                    return new CastRequest(spell.Name.Value, nextPosition);
            }

            if(IsFlyingAttackNeeded())
            {
                var spell = PlayerCardClassifying.TroopAirAttack.FirstOrDefault();
                if (spell != null)
                    return new CastRequest(spell.Name.Value, nextPosition);
            }

            if (troopCycleSpells.Count() > 1)
            {
                var spell = troopCycleSpells.FirstOrDefault();
                return new CastRequest(spell.Name.Value, nextPosition);
            }

            foreach (var s in troopPowerSpells)
            {
                if (_spellQueue.Count < 1)
                {
                    _spellQueue.Enqueue(s.Name.Value);
                }
                else
                {
                    return new CastRequest(s.Name.Value, nextPosition);
                }
            }

            return null;
        }

        public static CastRequest DefenseTroop(Vector2f nextPosition)
        {
            if (IsAOEAttackNeeded())
            {
                var spell = PlayerCardClassifying.TroopAOEAttack.FirstOrDefault();
                if (spell == null)
                    spell = PlayerCardClassifying.TroopGroundAttack.FirstOrDefault();

                if(spell != null)
                    return new CastRequest(spell.Name.Value, nextPosition);
            }

            if (IsFlyingAttackNeeded())
            {
                var spell = PlayerCardClassifying.TroopAirAttack.FirstOrDefault();

                if (spell != null)
                    return new CastRequest(spell.Name.Value, nextPosition);
            }

            return new CastRequest(PlayerCardClassifying.Troop
                                    .FirstOrDefault().Name.Value, nextPosition);
        }

        public static CastRequest Defense(Vector2f nextPosition)
        {
            IOrderedEnumerable<Spell> damagingSpells = PlayerCardClassifying.Damaging;

            Vector2f spellPosition = CastPositionHandling.GetPositionOfTheBestDamagingSpellDeploy();
            if (!spellPosition.Equals(Vector2f.Zero))
            {
                var damagingSpell = damagingSpells.FirstOrDefault();

                if (damagingSpell != null)
                    return new CastRequest(damagingSpell.Name.Value, spellPosition);
            }

            if (IsAOEAttackNeeded())
            {
                var spell = PlayerCardClassifying.TroopAOEAttack.FirstOrDefault();
                if (spell == null)
                    spell = PlayerCardClassifying.TroopGroundAttack.FirstOrDefault();

                if (spell != null)
                    return new CastRequest(spell.Name.Value, nextPosition);
            }

            if (IsFlyingAttackNeeded())
            {
                var spell = PlayerCardClassifying.TroopAirAttack.FirstOrDefault();

                if (spell != null)
                    return new CastRequest(spell.Name.Value, nextPosition);
            }

            {
                var spell = PlayerCardClassifying.TroopCycleCards.FirstOrDefault();

                if (spell != null)
                    return new CastRequest(spell.Name.Value, nextPosition);

                return new CastRequest(PlayerCardClassifying.Troop
                                        .FirstOrDefault().Name.Value, nextPosition);
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
    }
}
