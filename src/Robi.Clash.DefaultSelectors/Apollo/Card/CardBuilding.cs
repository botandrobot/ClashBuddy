using System;
using System.Collections.Generic;
using System.Text;

namespace Robi.Clash.DefaultSelectors.Card
{
    class CardBuilding : ICard
    {
        public CardBuilding(string name, BuildingType type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; set; }
        public uint Health { get; set; }
        public BuildingType Type { get; set; }
    }
}
