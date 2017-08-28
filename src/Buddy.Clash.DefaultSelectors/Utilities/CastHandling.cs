using Buddy.Clash.Engine;
using Buddy.Clash.Engine.NativeObjects.Logic.GameObjects;
using Buddy.Clash.Engine.NativeObjects.LogicData;
using Buddy.Clash.Engine.NativeObjects.Native;
using Buddy.Common;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Buddy.Clash.DefaultSelectors.Utilities
{
    class CastHandling
    {
        private static readonly ILogger Logger = LogProvider.CreateLogger<EarlyCycleSelector>();
        private static readonly ConcurrentQueue<string> _spellQueue = new ConcurrentQueue<string>();
        private static IOrderedEnumerable<Spell> Spells;

        public static CastRequest SpellMagic(Vector2f nextPosition, GameState gameState)
        {
            if (ClashEngine.Instance.LocalPlayer == null) return null;
            Spells = OwnCardHandling.Troop;

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

        public static CardType ChooseCardType(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.UAKT:
                case GameState.UALPT:
                case GameState.UARPT:
                    return CardType.Defense;
                case GameState.AKT:
                case GameState.ALPT:
                case GameState.ARPT:
                    return CardType.All;
                case GameState.DKT:
                case GameState.DLPT:
                case GameState.DRPT:
                    return CardType.Troop;
                case GameState.START:
                    return CardType.NONE;
                default:
                    return CardType.All;
            }
        }

        public static bool DamagingSpellDecision(out Engine.NativeObjects.Logic.GameObjects.Character enemy)
        {
            int count = 0;
            enemy = CharacterHandling.EnemyCharacterWithTheMostEnemiesAround(out count);

            /*
            Logger.Debug("enemyWhithTheMostEnemiesAround-Count: {count} enemy-Name {name}", count
                         , enemy.LogicGameObjectData.Name.Value);
                         */
            if (count > 5)
                return true;

            return false;
        }

        public static CastRequest EarlyCycle(Vector2f nextPosition)
        {
            IOrderedEnumerable<Spell> troopCycleSpells = OwnCardHandling.TroopCycleCards;
            IOrderedEnumerable<Spell> troopPowerSpells = OwnCardHandling.TroopPowerCards;
            IOrderedEnumerable<Spell> damagingSpells = OwnCardHandling.Damaging;

            Engine.NativeObjects.Logic.GameObjects.Character enemy;

            if (DamagingSpellDecision(out enemy))
            {
                var damagingSpell = damagingSpells.FirstOrDefault();

                if(damagingSpell != null)
                    return new CastRequest(damagingSpell.Name.Value, enemy.StartPosition);
            }

            if (IsAOEAttackNeeded())
            {
                var spell = OwnCardHandling.TroopAOEAttack.FirstOrDefault();
                if (spell == null)
                    spell = OwnCardHandling.TroopGroundAttack.FirstOrDefault();

                if (spell != null)
                    return new CastRequest(spell.Name.Value, nextPosition);
            }

            if(IsFlyingAttackNeeded())
            {
                var spell = OwnCardHandling.TroopAirAttack.FirstOrDefault();
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
                var spell = OwnCardHandling.TroopAOEAttack.FirstOrDefault();
                if (spell == null)
                    spell = OwnCardHandling.TroopGroundAttack.FirstOrDefault();

                if(spell != null)
                    return new CastRequest(spell.Name.Value, nextPosition);
            }

            if (IsFlyingAttackNeeded())
            {
                var spell = OwnCardHandling.TroopAirAttack.FirstOrDefault();

                if (spell != null)
                    return new CastRequest(spell.Name.Value, nextPosition);
            }

            return new CastRequest(OwnCardHandling.Troop
                                    .FirstOrDefault().Name.Value, nextPosition);
        }

        public static CastRequest Defense(Vector2f nextPosition)
        {
            IOrderedEnumerable<Spell> damagingSpells = OwnCardHandling.Damaging;
            Engine.NativeObjects.Logic.GameObjects.Character enemy;

            if (DamagingSpellDecision(out enemy))
            {
                var damagingSpell = damagingSpells.FirstOrDefault();

                if (damagingSpell != null)
                    return new CastRequest(damagingSpell.Name.Value, enemy.StartPosition);
            }

            if (IsAOEAttackNeeded())
            {
                var spell = OwnCardHandling.TroopAOEAttack.FirstOrDefault();
                if (spell == null)
                    spell = OwnCardHandling.TroopGroundAttack.FirstOrDefault();

                if (spell != null)
                    return new CastRequest(spell.Name.Value, nextPosition);
            }

            if (IsFlyingAttackNeeded())
            {
                var spell = OwnCardHandling.TroopAirAttack.FirstOrDefault();

                if (spell != null)
                    return new CastRequest(spell.Name.Value, nextPosition);
            }

            {
                var spell = OwnCardHandling.TroopCycleCards.FirstOrDefault();

                if (spell != null)
                    return new CastRequest(spell.Name.Value, nextPosition);

                return new CastRequest(OwnCardHandling.Troop
                                        .FirstOrDefault().Name.Value, nextPosition);
            }
        }


        public static bool IsAOEAttackNeeded()
        {
            int biggestEnemieGroupCount;
            Engine.NativeObjects.Logic.GameObjects.Character @char = 
                CharacterHandling.EnemyCharacterWithTheMostEnemiesAround(out biggestEnemieGroupCount);

            if (biggestEnemieGroupCount > 3)
                return true;

            return false;
        }

        public static bool IsFlyingAttackNeeded()
        {
            return CharacterHandling.IsFlyingEnemyOnTheField();
        }
    }
}
