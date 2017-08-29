using Buddy.Clash.DefaultSelectors.Game;
using Buddy.Clash.DefaultSelectors.Utilities;
using Buddy.Clash.Engine;
using Buddy.Clash.Engine.NativeObjects.Logic.GameObjects;
using Buddy.Common;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Buddy.Clash.DefaultSelectors.Enemy
{
    class EnemyHandling
    {
        private static readonly ILogger Logger = LogProvider.CreateLogger<EnemyHandling>();
        private static Dictionary<uint,Enemy> Enemies = new Dictionary<uint, Enemy>();

        public static void CreateEnemies()
        {
            switch (GameStateHandling.CurrentGameMode)
            {
                case GameMode.ONE_VERSUS_ONE:
                    CreateEnemiesTwoPlayerMode();
                    break;
                case GameMode.TWO_VERSUS_TWO:
                    CreateEnemiesFourPlayerMode();
                    break;
                case GameMode.NOT_IMPLEMENTED:
                    break;
                default:
                    break;
            }
        }

        private static void CreateEnemiesTwoPlayerMode()
        {
            switch (StaticValues.Player.OwnerIndex)
            {

                case 0:
                    Enemy enemie1 = new Enemy(1);
                    Enemies.Add(enemie1.OwnerIndex, enemie1);
                    break;
                case 1:
                    Enemy enemie0 = new Enemy(0);
                    Enemies.Add(enemie0.OwnerIndex, enemie0);
                    break;
            };
        }

        private static void CreateEnemiesFourPlayerMode()
        {
            switch (StaticValues.Player.OwnerIndex)
            {

                case 0:
                case 1:
                    Enemy enemie2 = new Enemy(2);
                    Enemy enemie3 = new Enemy(3);
                    Enemies.Add(enemie2.OwnerIndex, enemie2);
                    Enemies.Add(enemie3.OwnerIndex, enemie3);
                    break;
                case 2:
                case 3:
                    Enemy enemie0 = new Enemy(0);
                    Enemy enemie1 = new Enemy(1);
                    Enemies.Add(enemie0.OwnerIndex, enemie0);
                    Enemies.Add(enemie1.OwnerIndex, enemie1);
                    break;
            };
        }

        public static Dictionary<uint,Enemy> GetEnemies()
        {
            return Enemies;
        }

        /*
        public static void BuildEnemieDecks()
        {
            var om = ClashEngine.Instance.ObjectManager;
            var chars = CharacterHandling.EnemiesWithoutTower;

            foreach (var @char in chars)
            {
                //Logger.Debug("OwnerIndex {0}", StaticValues.Player.OwnerIndex);
                //Logger.Debug("Char-OwnerIndex {0}", @char.OwnerIndex);
                Enemies[@char.OwnerIndex].AddCardToDeck(@char);

                //foreach (var item in Enemies.Keys)
                //{
                //    Logger.Debug("Key: {0}", item);
                //}
            }
        }
        */

        public static void BuildEnemiesNextCardsAndHand()
        {
            Character spawnedCharacter = EnemyCharacterHandling.EnemyNewSpawnedCharacter;
            

            if (spawnedCharacter != null)
            {
                String spawnedCharacterName = spawnedCharacter.LogicGameObjectData.Name.Value;
                Logger.Debug("Build-Next-Cards: spawnedCharacter = {0}", spawnedCharacter.LogicGameObjectData.Name.Value);
                Enemy enemie = Enemies[spawnedCharacter.OwnerIndex];
                enemie.Mana = enemie.Mana - Convert.ToUInt32(spawnedCharacter.Mana);

                if ((enemie.NextCards.Where(item => item.Key == spawnedCharacterName).Count() > 0)
                                                || spawnedCharacterName.Contains("Bomb"))
                    return;

                if(!enemie.Hand.Remove(spawnedCharacterName))
                {
                    String key = "";

                    foreach (var slot in enemie.Hand)
                    {
                        if (slot.Value == null)
                            key = slot.Key;
                    }
                    enemie.Hand.Remove(key);
                }
                enemie.NextCards.Enqueue(new KeyValuePair<String, Character>(spawnedCharacterName, spawnedCharacter));

                KeyValuePair<String,Character> newCardOnHand = enemie.NextCards.Dequeue();
                enemie.Hand.Add(newCardOnHand.Key, newCardOnHand.Value);

                Logger.Debug("Enemie-Index: {0}", enemie.OwnerIndex);
                foreach (var item in enemie.NextCards)
                {
                    Logger.Debug("Next-Card {0}", item.Key);
                }

                foreach (var item in enemie.Hand)
                {
                    Logger.Debug("Hand-Card {0}", item.Key);

                }
            }
        }

    }
}
