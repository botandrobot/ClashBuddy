using Buddy.Clash.Engine;
using Buddy.Clash.Engine.NativeObjects.Logic.GameObjects;
using Buddy.Common;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buddy.Clash.DefaultSelectors.Utilities
{
    class EnemieHandling
    {
        private static readonly ILogger Logger = LogProvider.CreateLogger<EarlyCycleSelector>();
        private static Dictionary<uint,Enemie> Enemies = new Dictionary<uint, Enemie>();

        public static void CreateEnemies()
        {
            //Logger.Debug("Enemies-Count: {0}", Enemies.Count);
            //Logger.Debug("Player-Count: {0}", GameStateHandling.PlayerCount);

            if (Enemies.Count > 0)
                return;
            //Logger.Debug("Player-Count: {PlayerCount}", GameStateHandling.PlayerCount);

            if (GameStateHandling.PlayerCount == 2)
            {
                switch (StaticValues.Player.OwnerIndex)
                {

                    case 0:
                        Enemie enemie1 = new Enemie(1);
                        Enemies.Add(enemie1.OwnerIndex,enemie1);
                        break;
                    case 1:
                        Enemie enemie0 = new Enemie(0);
                        Enemies.Add(enemie0.OwnerIndex,enemie0);
                        break;
                };
            }
            else if (GameStateHandling.PlayerCount == 4)
            {
                switch (StaticValues.Player.OwnerIndex)
                {

                    case 0:
                    case 1:
                        Enemie enemie2 = new Enemie(2);
                        Enemie enemie3 = new Enemie(3);
                        Enemies.Add(enemie2.OwnerIndex, enemie2);
                        Enemies.Add(enemie3.OwnerIndex, enemie3);
                        break;
                    case 2:
                    case 3:
                        Enemie enemie0 = new Enemie(0);
                        Enemie enemie1 = new Enemie(1);
                        Enemies.Add(enemie0.OwnerIndex, enemie0);
                        Enemies.Add(enemie1.OwnerIndex, enemie1);
                        break;
                };
            }
            else
                Logger.Debug("Player-Count not correct or mode not implemented");

        }

        public static Dictionary<uint,Enemie> GetEnemies()
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
            Character spawnedCharacter = CharacterHandling.EnemyNewSpawnedCharacter;
            

            if (spawnedCharacter != null)
            {
                String spawnedCharacterName = spawnedCharacter.LogicGameObjectData.Name.Value;
                Logger.Debug("Build-Next-Cards: spawnedCharacter = {0}", spawnedCharacter.LogicGameObjectData.Name.Value);
                Enemie enemie = Enemies[spawnedCharacter.OwnerIndex];
                enemie.Mana = enemie.Mana - Convert.ToUInt32(spawnedCharacter.Mana);

                if (enemie.NextCards.Contains(new KeyValuePair<string, Character>(spawnedCharacterName, spawnedCharacter))
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
