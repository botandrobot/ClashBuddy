using Robi.Clash.Engine.NativeObjects.Logic.GameObjects;
using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;
using Robi.Clash.Engine;
using Serilog;
using Robi.Common;
using Robi.Clash.DefaultSelectors.Utilities;

namespace Robi.Clash.DefaultSelectors.Enemy
{
    class Enemy
    {
        private bool LogDeck = true;
        public int firstPlayedCards = 8;

        private static readonly ILogger Logger = LogProvider.CreateLogger<Enemy>();
        public int Mana { get; set; }
        public int ManaAsCharactersOnTheField { get; set; }
        public uint OwnerIndex { get; set; }

        public Character KingTower { get; set; }
        public Character RightPrincessTower { get; set; }
        public Character LeftPrincessTower { get; set; }

        private Dictionary<String, Character> deck = new Dictionary<string, Character>();
        public Dictionary<String, Character> Deck
        {
            get { return deck; }
            set { deck = value; }
        }

        public Queue<KeyValuePair<string, Character>> nextCards = new Queue<KeyValuePair<string, Character>>(
                                                                new KeyValuePair<string, Character>[] {
                                                                new KeyValuePair<string, Character>("4", null),
                                                                new KeyValuePair<string, Character>("5", null),
                                                                new KeyValuePair<string, Character>("6", null),
                                                                new KeyValuePair<string, Character>("7", null)});

        public Queue<KeyValuePair<string,Character>> NextCards
        {
            get { return nextCards; }
            set { nextCards = value; }
        }

        // ToDo: Use Card counting to build the enemies hand
        private Dictionary<string, Character> hand = new Dictionary<string, Character>()
        {
            { "0", null },
            { "1", null },
            { "2", null },
            { "3", null }
        };
        public Dictionary<string, Character> Hand
        {
            get { return hand; }
            set { hand = value; }
        }

        public Enemy(uint ownerIndex)
        {
            OwnerIndex = ownerIndex;
            IEnumerable<Character> PrincessTower = CharacterHandling.PrincessTowerOfOwner(ownerIndex);

            #region PrincessTower
            if (StaticValues.PlayerCount == 2)
            {
                if (ownerIndex == 0)
                {
                    LeftPrincessTower = PrincessTower.FirstOrDefault();
                    RightPrincessTower = PrincessTower.LastOrDefault();
                }
                else
                {
                    LeftPrincessTower = PrincessTower.LastOrDefault();
                    RightPrincessTower = PrincessTower.FirstOrDefault();
                }
            }
            else if (StaticValues.PlayerCount == 4)
            {
                switch (StaticValues.Player.OwnerIndex)
                {

                    case 0:
                    case 1:
                        LeftPrincessTower = PrincessTower.FirstOrDefault();
                        RightPrincessTower = PrincessTower.LastOrDefault();
                        break;
                    case 2:
                    case 3:
                        LeftPrincessTower = PrincessTower.LastOrDefault();
                        RightPrincessTower = PrincessTower.FirstOrDefault();
                        break;
                };
            }
            else
                Logger.Debug("Player-Count not correct or mode not implemented");
            #endregion

            KingTower = CharacterHandling.KingTowerOfOwner(ownerIndex);
            Mana = (uint)StaticValues.Player.Mana;
        }

        public void AddCardsToDeck(IEnumerable<Character> characters)
        {
            if (Deck.Count == 8)
                return;

            foreach (var @char in characters)
            {
                if (!Deck.ContainsKey(@char.LogicGameObjectData.Name.Value))
                    Deck.Add(@char.LogicGameObjectData.Name.Value, @char);
            }
        }

        public void AddCardToDeck(Character character)
        {
            if (Deck.Count == 8)
            {
                if(LogDeck)
                {
                    LogDeck = false;
                    //Logger.Debug("Deck-Log: Owner-Index {OwnerIndex}", OwnerIndex);

                    //foreach (var @char in Deck)
                    //{
                    //    Logger.Debug("Character-Name {CharacterName}", @char.Key);
                    //}
                }
                return;
            }

            if (!Deck.ContainsKey(character.LogicGameObjectData.Name.Value))
                Deck.Add(character.LogicGameObjectData.Name.Value, character);
        }


    }
}
