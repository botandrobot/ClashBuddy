using System;
using System.Collections.Generic;
using System.Text;

namespace Robi.Clash.DefaultSelectors.Card
{
    class CardSpell : ICard
    {
        public CardSpell(string name, SpellType type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; set; }
        public SpellType Type { get; set; }
    }
}
