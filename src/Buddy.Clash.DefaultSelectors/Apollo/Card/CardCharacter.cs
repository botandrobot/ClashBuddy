using System;
using System.Collections.Generic;
using System.Text;

namespace Buddy.Clash.DefaultSelectors.Card
{
    class CardCharacter : ICard
    {
        public CardCharacter(string name, TroopType type)
        {
            Name = name;
            Type = type;
        }
        public string Name { get; set; }
        public uint Health { get; set; }
        public uint Damage { get; set; }
        public TroopType Type { get; set; }
    }
}
