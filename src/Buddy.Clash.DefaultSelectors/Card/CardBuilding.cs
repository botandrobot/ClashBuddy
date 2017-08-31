using System;
using System.Collections.Generic;
using System.Text;

namespace Buddy.Clash.DefaultSelectors.Card
{
    class CardBuilding : ICard
    {
        public CardBuilding(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public uint Health { get; set; }
        public BuildingType Type { get; set; }
    }
}
