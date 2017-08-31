using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Buddy.Clash.Engine.Csv.CsvLogic;
using Buddy.Clash.DefaultSelectors.Game;

namespace Buddy.Clash.DefaultSelectors.Utilities
{
    class CSVClassifying
    {
        public static bool IsTank(string name)
        {
            // ToDo: Char-Level beachten
            Characters.CharacterEntry characterEntry = Characters.Entries.Where(n => n.Name == name).FirstOrDefault();

            if(characterEntry == null)
                return false;

            return (characterEntry.Hitpoints >= GameHandling.Settings.MinHealthAsTank);
        }

        public static bool IsBuilding(string name)
        {
            // ToDo: Char-Level beachten
            Buildings.BuildingEntry buildingEntry = Buildings.Entries.Where(n => n.Name == name).FirstOrDefault();

            if (buildingEntry == null)
                return false;

            return true;
        }

        public static bool IsNonDamagingSpell(string name)
        {
            // ToDo: Char-Level beachten
            SpellsOther.SpellsOtherEntry buildingEntry = SpellsOther.Entries.Where(n => n.Name == name).FirstOrDefault();

            if (buildingEntry == null)
                return false;
            
            return true;
        }
    }
}
